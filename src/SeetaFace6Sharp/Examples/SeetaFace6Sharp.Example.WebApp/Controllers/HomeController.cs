using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SeetaFace6Sharp;
using SkiaSharp;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SeetaFace6Sharp.Example.WebApp.Models;
using SeetaFace6Sharp.Example.WebApp.Models.Interface;
using SeetaFace6Sharp.Example.WebApp.Utils;
using SeetaFace6Sharp.Extension.DependencyInjection;

namespace SeetaFace6Sharp.Example.WebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly FaceDetector _faceDetector;
        private readonly ISeetaFace6SharpFactory _faceFactory;

        public HomeController(ILogger<HomeController> logger
            , FaceDetector faceDetector
            , ISeetaFace6SharpFactory faceFactory)
        {
            _logger = logger;
            _faceDetector = faceDetector ?? throw new ArgumentNullException(nameof(faceDetector));
            _faceFactory = faceFactory ?? throw new ArgumentNullException(nameof(faceFactory));
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Detect(PhotoDetectRequest @params)
        {
            if (string.IsNullOrWhiteSpace(@params.Image))
            {
                return Json(new BaseResponse<PhotoDetectResponseData>(10001, "图片不能为空"));
            }
            try
            {
                var result = new PhotoDetectResponseData()
                {
                    Image = @params.Image,
                };
                using SKData imageData = ImageBase64Converter.Base64ToImage(@params.Image, out SKEncodedImageFormat format);
                if (imageData == null)
                {
                    return Json(new BaseResponse<PhotoDetectResponseData>(10002, "解析图片失败", result));
                }
                using SKBitmap bitmap = SKBitmap.Decode(imageData);
                using FaceImage image = bitmap.ToFaceImage();
                Stopwatch stopwatch = Stopwatch.StartNew();
                //检测人脸
                FaceInfo[] faceInfos = _faceDetector.Detect(image);
                if (faceInfos == null || !faceInfos.Any())
                {
                    stopwatch.Stop();
                    return Json(new BaseResponse<PhotoDetectResponseData>(10002, "未识别到任何人脸", result));
                }
                foreach (var faceInfo in faceInfos)
                {
                    PhotoDetectFaceInfo faceData = new PhotoDetectFaceInfo()
                    {
                        FaceInfo = faceInfo,
                    };
                    //关键点标记
                    FaceMarkPoint[] points = _faceFactory.Get<FaceLandmarker>()?.Mark(image, faceInfo);
                    //口罩检测
                    faceData.MaskResult = _faceFactory.Get<MaskDetector>()?.Detect(image, faceInfo);
                    //年龄预测
                    faceData.Age = _faceFactory.Get<AgePredictor>()?.PredictAgeWithCrop(image, points);
                    //性别预测
                    faceData.Gender = _faceFactory.Get<GenderPredictor>()?.PredictGenderWithCrop(image, points);
                    //活体检测
                    faceData.AntiSpoofing = _faceFactory.Get<FaceAntiSpoofing>()?.Predict(image, faceInfo, points);
                    //眼睛状态检测
                    faceData.EyeState = _faceFactory.Get<EyeStateDetector>()?.Detect(image, points);
                    //清晰度检测
                    faceData.Quality = _faceFactory.Get<FaceQuality>()?.Detect(image, faceInfo, points, QualityType.Clarity);
                    result.Infos.Add(faceData);
                    //完成识别计时
                    stopwatch.Stop();
                    result.Elapsed = stopwatch.ElapsedMilliseconds;
                    //绘制矩形
                    DrawFaceRect(bitmap, faceInfo.Location);
                }
                result.Width = bitmap.Width;
                result.Height = bitmap.Height;
                result.Image = ImageBase64Converter.BitmapToBase64WithFormat(bitmap, format);
                return Json(new BaseResponse<PhotoDetectResponseData>(result));
            }
            catch (Exception ex)
            {
                return Json(new BaseResponse<PhotoDetectResponseData>(10099, ex.Message));
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private void DrawFaceRect(SKBitmap bitmap, FaceRect faceRect)
        {
            using (SKCanvas canvas = new SKCanvas(bitmap))
            {
                using (SKPaint paint = new SKPaint())
                {
                    paint.Style = SKPaintStyle.Stroke;
                    paint.Color = SKColors.Red;
                    paint.StrokeWidth = 3;
                    paint.StrokeCap = SKStrokeCap.Round;
                    canvas.DrawRect(faceRect.X, faceRect.Y, faceRect.Width, faceRect.Height, paint);
                }
            }
        }
    }
}