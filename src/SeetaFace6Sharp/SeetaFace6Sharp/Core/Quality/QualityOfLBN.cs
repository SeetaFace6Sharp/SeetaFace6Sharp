using SeetaFace6Sharp.Models;
using SeetaFace6Sharp.Native;
using System;

namespace SeetaFace6Sharp
{
    /// <summary>
    /// 深度学习的人脸清晰度评估器。
    /// </summary>
    public sealed class QualityOfLBN : BaseSeetaFace6<QualityOfLBNConfig>
    {
        private readonly IntPtr _handle = IntPtr.Zero;
        private readonly static object _locker = new object();

        /// <summary>
        /// 所需模型：mask_detector.csta
        /// </summary>
        public override Model Model { get; }

        private readonly Lazy<FaceLandmarker> _faceLandmarker = null;

        /// <summary>
        /// 深度学习的人脸清晰度评估器。
        /// </summary>
        /// <param name="config"></param>
        /// <exception cref="ModuleInitializeException"></exception>
        public QualityOfLBN(QualityOfLBNConfig config = null) : base(config ?? new QualityOfLBNConfig())
        {
            this.Model = new Model("quality_lbn.csta", this.Config.DeviceType);
            if ((_handle = SeetaFace6Native.GetQualityOfLBNHandler(this.Model.Ptr, this.Config.BlurThresh, this.Config.ThreadNumber)) == IntPtr.Zero)
            {
                throw new ModuleInitializeException(nameof(QualityOfLBN), "Get quality of LBN handle failed.");
            }
            //Landmarker
            _faceLandmarker = new Lazy<FaceLandmarker>(() =>
            {
                return new FaceLandmarker(new FaceLandmarkConfig(MarkType.Normal));
            });
        }

        /// <summary>
        /// 清晰度评估，传入68个特征点数组
        /// </summary>
        /// <param name="image"></param>
        /// <param name="points"></param>
        /// <returns></returns>
        public QualityOfLBNResult Detect(FaceImage image, FaceMarkPoint[] points)
        {
            lock (_locker)
            {
                if (disposedValue)
                    throw new ObjectDisposedException(nameof(QualityOfLBN));
                if (points.Length != 68)
                    throw new ArgumentException(nameof(FaceMarkPoint), "Only supports an array of 68 feature points.");

                int light = 0, blur = 0, noise = 0;
                SeetaFace6Native.QualityOfLBNDetect(_handle, ref image, points, ref light, ref blur, ref noise);
                return new QualityOfLBNResult(light, blur, noise);
            }
        }

        public QualityOfLBNResult Detect(FaceImage image, FaceInfo faceInfo)
        {
            lock (_locker)
            {
                if (disposedValue)
                    throw new ObjectDisposedException(nameof(QualityOfLBN));

                FaceMarkPoint[] points = _faceLandmarker.Value.Mark(image, faceInfo);

                int light = 0, blur = 0, noise = 0;
                SeetaFace6Native.QualityOfLBNDetect(_handle, ref image, points, ref light, ref blur, ref noise);
                return new QualityOfLBNResult(light, blur, noise);
            }
        }

        /// <inheritdoc/>
        public override void Dispose()
        {
            if (disposedValue) return;

            lock (_locker)
            {
                if (disposedValue) return;
                disposedValue = true;
                if (_handle == IntPtr.Zero) return;

                SeetaFace6Native.DisposeQualityOfLBN(_handle);
                this.Model.Dispose();

                if (_faceLandmarker != null && _faceLandmarker.IsValueCreated)
                {
                    _faceLandmarker.Value.Dispose();
                }
            }
        }
    }
}