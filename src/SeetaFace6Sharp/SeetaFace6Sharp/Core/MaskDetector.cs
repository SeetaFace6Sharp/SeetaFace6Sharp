using SeetaFace6Sharp.Models;
using SeetaFace6Sharp.Native;
using System;

namespace SeetaFace6Sharp
{
    /// <summary>
    /// 口罩人脸识别
    /// </summary>
    public sealed class MaskDetector : BaseSeetaFace6<MaskDetectConfig>
    {
        private readonly IntPtr _handle = IntPtr.Zero;
        private readonly static object _locker = new object();

        /// <summary>
        /// 所需模型(mask_detector.csta)
        /// </summary>
        public override Model Model { get; }

        /// <summary>
        /// 口罩人脸识别
        /// </summary>
        /// <param name="config"></param>
        /// <exception cref="ModuleInitializeException"></exception>
        public MaskDetector(MaskDetectConfig config = null) : base(config ?? new MaskDetectConfig())
        {
            this.Model = new Model("", this.Config.DeviceType);

            if ((_handle = SeetaFace6Native.GetMaskDetectorHandler(this.Model.Ptr)) == IntPtr.Zero)
            {
                throw new ModuleInitializeException(nameof(FaceLandmarker), "Get mask detector handle failed.");
            }
        }

        /// <summary>
        /// 戴口罩人脸识别
        /// </summary>
        /// <param name="image"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        public PlotMaskResult Detect(FaceImage image, FaceInfo info)
        {
            lock (_locker)
            {
                if (disposedValue)
                    throw new ObjectDisposedException(nameof(MaskDetector));

                float score = 0;
                bool status = SeetaFace6Native.MaskDetect(_handle, ref image, info.Location, ref score);
                return new PlotMaskResult(score, status, status && score > this.Config.Threshold);
            }
        }

        /// <inheritdoc/>
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
                SeetaFace6Native.DisposeMaskDetector(_handle);
                this.Model.Dispose();
            }
        }
    }
}