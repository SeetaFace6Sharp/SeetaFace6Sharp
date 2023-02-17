using SeetaFace6Sharp.Native;
using System;

namespace SeetaFace6Sharp
{
    /// <summary>
    /// 质量评估
    /// </summary>
    public sealed class FaceQuality : BaseSeetaFace6<QualityConfig>
    {
        private readonly IntPtr _clarityHandle = IntPtr.Zero;
        private readonly static object _clarityLocker = new object();

        private readonly IntPtr _maskHandle = IntPtr.Zero;
        private readonly static object _maskLocker = new object();

        private readonly static object _disposeLocker = new object();

        /// <inheritdoc/>
        public FaceQuality(QualityConfig config = null) : base(config ?? new QualityConfig())
        {
            _clarityHandle = SeetaFace6Native.GetQualityOfClarityExHandler(this.Config.ClarityEx.BlurThresh, (int)this.Config.DeviceType);
            if (_clarityHandle == IntPtr.Zero)
            {
                throw new ModuleInitializeException(nameof(FaceQuality), "Get quality of clarityEx handle failed.");
            }

            _maskHandle = SeetaFace6Native.GetQualityOfNoMaskHandler((int)this.Config.DeviceType);
            if (_maskHandle == IntPtr.Zero)
            {
                throw new ModuleInitializeException(nameof(FaceQuality), "Get quality of nomask handle failed.");
            }
        }

        /// <summary>
        /// 人脸质量评估
        /// </summary>
        /// <param name="image">人脸图像信息</param>
        /// <param name="info">面部信息<para>通过 <see cref="FaceDetector.Detect(FaceImage)"/> 获取</para></param>
        /// <param name="points"><paramref name="info"/> 对应的关键点坐标<para>通过 <see cref="FaceLandmarker.Mark(FaceImage, FaceInfo)"/> 获取</para></param>
        /// <param name="type">质量评估类型</param>
        /// <returns></returns>
        public QualityResult Detect(FaceImage image, FaceInfo info, FaceMarkPoint[] points, QualityType type)
        {
            if (disposedValue)
                throw new ObjectDisposedException(nameof(FaceQuality));

            int level = -1;
            float score = -1;

            switch (type)
            {
                case QualityType.Brightness:
                    SeetaFace6Native.QualityOfBrightness(ref image, info.Location, points, points.Length, ref level, ref score,
                        this.Config.Brightness.V0, this.Config.Brightness.V1, this.Config.Brightness.V2, this.Config.Brightness.V3);
                    break;
                case QualityType.Clarity:
                    SeetaFace6Native.QualityOfClarity(ref image, info.Location, points, points.Length, ref level, ref score, this.Config.Clarity.Low, this.Config.Clarity.High);
                    break;
                case QualityType.Integrity:
                    SeetaFace6Native.QualityOfIntegrity(ref image, info.Location, points, points.Length, ref level, ref score,
                        this.Config.Integrity.Low, this.Config.Integrity.High);
                    break;
                case QualityType.Pose:
                    SeetaFace6Native.QualityOfPose(ref image, info.Location, points, points.Length, ref level, ref score);
                    break;
                case QualityType.PoseEx:
                    SeetaFace6Native.QualityOfPoseEx(ref image, info.Location, points, points.Length, ref level, ref score,
                       this.Config.PoseEx.YawLow, this.Config.PoseEx.YawHigh,
                       this.Config.PoseEx.PitchLow, this.Config.PoseEx.PitchHigh,
                       this.Config.PoseEx.RollLow, this.Config.PoseEx.RollHigh);
                    break;
                case QualityType.Resolution:
                    SeetaFace6Native.QualityOfResolution(ref image, info.Location, points, points.Length, ref level, ref score, this.Config.Resolution.Low, this.Config.Resolution.High);
                    break;
                case QualityType.ClarityEx:
                    {
                        lock (_clarityLocker)
                        {
                            SeetaFace6Native.QualityOfClarityEx(_clarityHandle, ref image, info.Location, points, points.Length, ref level, ref score);
                        }
                    }
                    break;
                case QualityType.Structure:
                    {
                        lock (_maskLocker)
                        {
                            SeetaFace6Native.QualityOfNoMask(_maskHandle, ref image, info.Location, points, points.Length, ref level, ref score);
                        }
                    }
                    break;
            }

            return new QualityResult((QualityLevel)level, score, type);
        }

        /// <inheritdoc/>
        public override void Dispose()
        {
            if (disposedValue)
                return;

            lock (_disposeLocker)
            {
                if (disposedValue)
                    return;
                disposedValue = true;
            }

            lock (_clarityLocker)
            {
                if (_clarityHandle == IntPtr.Zero)
                    return;
                SeetaFace6Native.DisposeQualityOfClarityEx(_clarityHandle);
            }

            lock (_maskLocker)
            {
                if (_maskHandle == IntPtr.Zero)
                    return;
                SeetaFace6Native.DisposeQualityOfNoMask(_maskHandle);
            }
        }
    }
}
