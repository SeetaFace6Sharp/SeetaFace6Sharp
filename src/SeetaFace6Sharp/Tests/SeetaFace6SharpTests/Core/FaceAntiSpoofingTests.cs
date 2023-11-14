using Microsoft.VisualStudio.TestTools.UnitTesting;
using SeetaFace6Sharp;
using SeetaFace6SharpTests.Utils;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeetaFace6Sharp.Tests
{
    [TestClass()]
    public class FaceAntiSpoofingTests
    {
        /// <summary>
        /// 活体检测测试
        /// </summary>
        [TestMethod()]
        public void PredictTest()
        {
            using var bitmap = SKBitmap.Decode(@"images/more.jpg").ToFaceImage();
            using FaceDetector faceDetector = new FaceDetector();
            using FaceLandmarker faceMark = new FaceLandmarker();

            var info = faceDetector.Detect(bitmap).First();
            var markPoints = FaceMarkUtil.GetFaceMarkPoint(faceDetector, faceMark, bitmap);

            using FaceAntiSpoofing faceAntiSpoofing = new FaceAntiSpoofing(new FaceAntiSpoofingConfig()
            {
                Global = true
            });

            var result = faceAntiSpoofing.Predict(bitmap, info, markPoints);
        }
    }
}