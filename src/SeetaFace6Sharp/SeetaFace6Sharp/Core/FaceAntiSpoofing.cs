using SeetaFace6Sharp.Models;
using SeetaFace6Sharp.Native;
using System;

namespace SeetaFace6Sharp
{
    /// <summary>
    /// 活体检测器
    /// </summary>
    public sealed class FaceAntiSpoofing : BaseSeetaFace6<FaceAntiSpoofingConfig>
    {
        private readonly IntPtr _handle = IntPtr.Zero;
        private readonly static object _locker = new object();

        /// <summary>
        /// 活体检测所需Model（fas_first.csta），全局检测需增加fas_second.csta
        /// </summary>
        public override Model Model { get; }

        /// <inheritdoc/>
        /// <exception cref="ModuleInitializeException"></exception>
        public FaceAntiSpoofing(FaceAntiSpoofingConfig config = null) : base(config ?? new FaceAntiSpoofingConfig())
        {
            this.Model = new Model("fas_first.csta", this.Config.DeviceType);
            if (this.Config.Global)
            {
                this.Model.Append("fas_second.csta");
            }
            _handle = SeetaFace6Native.GetFaceAntiSpoofingHandler(this.Model.Ptr, this.Config.VideoFrameCount, this.Config.BoxThresh, this.Config.Threshold.Clarity, this.Config.Threshold.Reality, this.Config.ThreadNumber);
            if (_handle == IntPtr.Zero)
            {
                throw new ModuleInitializeException(nameof(FaceAntiSpoofing), "Get face anti spoofing handle failed.");
            }
        }

        /// <summary>
        /// 活体检测器。(单帧图片)
        /// </summary>
        /// <param name="image">人脸图像信息</param>
        /// <param name="info">面部信息<para>通过 <see cref="FaceDetector.Detect(FaceImage,int)"/> 获取</para></param>
        /// <param name="points"><paramref name="info"/> 对应的关键点坐标<para>通过 <see cref="MaskDetector.Detect(FaceImage, FaceInfo)"/> 获取</para></param>
        /// <returns>活体检测状态</returns>
        public AntiSpoofingResult Predict(FaceImage image, FaceInfo info, FaceMarkPoint[] points)
        {
            lock (_locker)
            {
                if (disposedValue)
                    throw new ObjectDisposedException(nameof(FaceAntiSpoofing));

                float clarity = 0, reality = 0;
                AntiSpoofingStatus status = (AntiSpoofingStatus)SeetaFace6Native.FaceAntiSpoofingPredict(_handle, ref image, info.Location, points, ref clarity, ref reality);
                return new AntiSpoofingResult(status, clarity, reality);
            }
        }

        /// <summary>
        /// 活体检测器。(视频帧图片)
        /// </summary>
        /// <param name="image">人脸图像信息</param>
        /// <param name="info">面部信息<para>通过 <see cref="FaceDetector.Detect(FaceImage, int)"/> 获取</para></param>
        /// <param name="points"><paramref name="info"/> 对应的关键点坐标<para>通过 <see cref="FaceLandmarker.Mark(FaceImage, FaceInfo)"/> 获取</para></param>
        /// <returns>如果为 <see cref="AntiSpoofingStatus.Detecting"/>，则说明需要继续调用此方法，传入更多的图片</returns>
        public AntiSpoofingResult PredictVideo(FaceImage image, FaceInfo info, FaceMarkPoint[] points)
        {
            lock (_locker)
            {
                if (disposedValue)
                    throw new ObjectDisposedException(nameof(FaceAntiSpoofing));

                float clarity = 0, reality = 0;
                AntiSpoofingStatus status = (AntiSpoofingStatus)SeetaFace6Native.FaceAntiSpoofingPredictVideo(_handle, ref image, info.Location, points, ref clarity, ref reality);
                return new AntiSpoofingResult(status, clarity, reality);
            }
        }

        /// <summary>
        /// 释放非托管资源
        /// </summary>
        public override void Dispose()
        {
            if (disposedValue)
                return;

            lock (_locker)
            {
                if (disposedValue)
                    return;
                disposedValue = true;
                if (_handle == IntPtr.Zero)
                    return;
                SeetaFace6Native.DisposeFaceAntiSpoofing(_handle);
                this.Model?.Dispose();
            }
        }
    }
}