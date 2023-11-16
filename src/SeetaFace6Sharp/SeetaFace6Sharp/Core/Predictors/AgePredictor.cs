using SeetaFace6Sharp.Models;
using SeetaFace6Sharp.Native;
using System;

namespace SeetaFace6Sharp;

/// <summary>
/// 年龄预测，需要模型 <a href="https://www.nuget.org/packages/SeetaFace6Sharp.model.age_predictor">age_predictor.csta</a>
/// </summary>
public sealed class AgePredictor : BasePredictor<AgePredictConfig>
{
    private readonly IntPtr _handle = IntPtr.Zero;
    private readonly static object _locker = new object();

    /// <summary>
    /// 所需模型：age_predictor.csta
    /// </summary>
    public override Model Model { get; }

    /// <inheritdoc/>
    /// <exception cref="ModuleInitializeException"></exception>
    public AgePredictor(AgePredictConfig config = null) : base(config ?? new AgePredictConfig())
    {
        this.Model = new Model("age_predictor.csta", this.Config.DeviceType);
        if ((_handle = SeetaFace6Native.GetAgePredictorHandler(this.Model.Ptr, this.Config.ThreadNumber)) == IntPtr.Zero)
        {
            throw new ModuleInitializeException(nameof(AgePredictor), "Get age predictor handle failed.");
        }
    }

    /// <summary>
    /// 年龄预测
    /// <para>
    /// 需要模型 <a href="https://www.nuget.org/packages/SeetaFace6Sharp.model.age_predictor">age_predictor.csta</a>
    /// </para>
    /// </summary>
    /// <param name="image">人脸图像信息</param>
    /// <returns>-1: 预测失败失败，其它: 预测的年龄。</returns>
    [Obsolete($"此方法不支持自动裁剪，请使用{nameof(PredictAgeWithCrop)}。")]
    public int PredictAge(FaceImage image)
    {
        lock (_locker)
        {
            if (disposedValue)
                throw new ObjectDisposedException(nameof(AgePredictor));

            return SeetaFace6Native.PredictAge(_handle, ref image);
        }
    }

    /// <summary>
    /// 年龄预测（自动裁剪）
    /// <para>
    /// 需要模型 <a href="https://www.nuget.org/packages/SeetaFace6Sharp.model.age_predictor">age_predictor.csta</a>
    /// </para>
    /// </summary>
    /// <param name="image">人脸图像信息</param>
    /// <param name="points">关键点坐标<para>通过 <see cref="MaskDetector.Detect(FaceImage, FaceInfo)"/> 获取</para></param>
    /// <returns>-1: 预测失败失败，其它: 预测的年龄。</returns>
    public int PredictAgeWithCrop(FaceImage image, FaceMarkPoint[] points)
    {
        lock (_locker)
        {
            if (disposedValue)
                throw new ObjectDisposedException(nameof(AgePredictor));

            return SeetaFace6Native.PredictAgeWithCrop(_handle, ref image, points);
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
            SeetaFace6Native.DisposeAgePredictor(_handle);
            this.Model?.Dispose();
        }
    }
}
