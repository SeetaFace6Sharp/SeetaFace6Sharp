using SeetaFace6Sharp.Models;
using SeetaFace6Sharp.Native;
using System;

namespace SeetaFace6Sharp;

/// <summary>
/// 眼睛状态检测。<br />
/// 眼睛的左右是相对图片内容而言的左右。<br />
/// 需要模型 <a href="https://www.nuget.org/packages/SeetaFace6Sharp.model.eye_state">eye_state.csta</a>
/// </summary>
public sealed class EyeStateDetector : BasePredictor<EyeStateDetectConfig>
{
    private readonly IntPtr _handle = IntPtr.Zero;
    private readonly static object _locker = new object();

    /// <summary>
    /// 所需模型：eye_state.csta
    /// </summary>
    public override Model Model { get; }

    /// <inheritdoc/>
    /// <exception cref="ModuleInitializeException"></exception>
    public EyeStateDetector(EyeStateDetectConfig config = null) : base(config ?? new EyeStateDetectConfig())
    {
        this.Model = new Model("eye_state.csta", this.Config.DeviceType);
        if ((_handle = SeetaFace6Native.GetEyeStateDetectorHandler(this.Model.Ptr)) == IntPtr.Zero)
        {
            throw new ModuleInitializeException(nameof(EyeStateDetector), "Get eye state detector handle failed.");
        }
    }

    /// <summary>
    /// 眼睛状态检测。
    /// <para>
    /// 眼睛的左右是相对图片内容而言的左右。<br />
    /// 需要模型 <a href="https://www.nuget.org/packages/SeetaFace6Sharp.model.eye_state">eye_state.csta</a>
    /// </para>
    /// </summary>
    /// <param name="image">人脸图像信息</param>
    /// <param name="points">关键点坐标<para>通过 <see cref="MaskDetector.Detect(FaceImage, FaceInfo)"/> 获取</para></param>
    /// <returns></returns>
    public EyeStateResult Detect(FaceImage image, FaceMarkPoint[] points)
    {
        lock (_locker)
        {
            if (disposedValue)
                throw new ObjectDisposedException(nameof(EyeStateDetector));

            int left_eye = 0, right_eye = 0;
            SeetaFace6Native.EyeStateDetect(_handle, ref image, points, ref left_eye, ref right_eye);
            return new EyeStateResult((EyeState)left_eye, (EyeState)right_eye);
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
            SeetaFace6Native.DisposeEyeStateDetector(_handle);
            this.Model.Dispose();
        }
    }
}
