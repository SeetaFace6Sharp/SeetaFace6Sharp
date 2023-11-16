using SeetaFace6Sharp.Models;
using SeetaFace6Sharp.Native;
using System;

namespace SeetaFace6Sharp
{
    /// <summary>
    /// 性别预测。
    /// 需要模型 <a href="https://www.nuget.org/packages/SeetaFace6Sharp.model.gender_predictor">gender_predictor.csta</a>
    /// </summary>
    public sealed class GenderPredictor : BasePredictor<GenderPredictConfig>
    {
        private readonly IntPtr _handle = IntPtr.Zero;
        private readonly static object _locker = new object();

        /// <summary>
        /// 所需模型：gender_predictor.csta
        /// </summary>
        public override Model Model { get; }

        /// <inheritdoc/>
        /// <exception cref="ModuleInitializeException"></exception>
        public GenderPredictor(GenderPredictConfig config = null) : base(config ?? new GenderPredictConfig())
        {
            this.Model = new Model("gender_predictor.csta", this.Config.DeviceType);
            if ((_handle = SeetaFace6Native.GetGenderPredictorHandler(this.Model.Ptr, this.Config.ThreadNumber)) == IntPtr.Zero)
            {
                throw new ModuleInitializeException(nameof(GenderPredictor), "Get gender predictor handle failed.");
            }
        }

        /// <summary>
        /// 性别预测
        /// <para>
        /// 需要模型 <a href="https://www.nuget.org/packages/SeetaFace6Sharp.model.gender_predictor">gender_predictor.csta</a>
        /// </para>
        /// </summary>
        /// <param name="image">人脸图像信息</param>
        /// <returns>性别。<see cref="Gender.Unknown"/> 代表识别失败</returns>
        [Obsolete($"此方法不支持自动裁剪，请使用{nameof(PredictGenderWithCrop)}。")]
        public Gender PredictGender(FaceImage image)
        {
            lock (_locker)
            {
                if (disposedValue)
                    throw new ObjectDisposedException(nameof(GenderPredictor));

                int result = SeetaFace6Native.PredictGender(_handle, ref image);
                return Enum.IsDefined(typeof(Gender), result) ? (Gender)result : Gender.Unknown;
            }
        }

        /// <summary>
        /// 性别预测（自动裁剪）
        /// <para>
        /// 需要模型 <a href="https://www.nuget.org/packages/SeetaFace6Sharp.model.gender_predictor">gender_predictor.csta</a>
        /// </para>
        /// </summary>
        /// <param name="image">人脸图像信息</param>
        /// <param name="points">关键点坐标<para>通过 <see cref="MaskDetector.Detect(FaceImage, FaceInfo)"/> 获取</para></param>
        /// <returns>性别。<see cref="Gender.Unknown"/> 代表识别失败</returns>
        public Gender PredictGenderWithCrop(FaceImage image, FaceMarkPoint[] points)
        {
            lock (_locker)
            {
                if (disposedValue)
                    throw new ObjectDisposedException(nameof(GenderPredictor));

                int result = SeetaFace6Native.PredictGenderWithCrop(_handle, ref image, points);
                return Enum.IsDefined(typeof(Gender), result) ? (Gender)result : Gender.Unknown;
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
                SeetaFace6Native.DisposeGenderPredictor(_handle);
                this.Model?.Dispose();
            }
        }
    }
}