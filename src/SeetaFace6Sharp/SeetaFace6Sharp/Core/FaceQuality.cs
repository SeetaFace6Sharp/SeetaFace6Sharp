using SeetaFace6Sharp.Models;
using SeetaFace6Sharp.Native;
using System;
using System.Linq;

namespace SeetaFace6Sharp
{
    /// <summary>
    /// 质量评估
    /// </summary>
    public sealed class FaceQuality : BaseSeetaFace6<QualityConfig>
    {
        private readonly static object _locker = new object();
        private readonly static object _poseLocker = new object();

        /// <summary>
        /// 人脸识别所需model（quality_lbn）
        /// </summary>
        public override Model Model { get; }

        /// <summary>
        /// 模型：pose_estimation
        /// </summary>
        public Model PoseEstimationModel { get; }

        private readonly Lazy<QualityOfLBN> _qualityOfLBN = null;
        private readonly Lazy<FaceLandmarker> _faceLandmarker = null;

        /// <inheritdoc/>
        public FaceQuality(QualityConfig config = null) : base(config ?? new QualityConfig())
        {
            this.Model = new Model("quality_lbn.csta", this.Config.DeviceType);
            this.PoseEstimationModel = new Model("pose_estimation.csta", this.Config.DeviceType);

            _qualityOfLBN = new Lazy<QualityOfLBN>(() =>
            {
                return new QualityOfLBN(new QualityOfLBNConfig()
                {
                    BlurThresh = this.Config.ClarityEx.BlurThresh,
                    DeviceType = this.Config.DeviceType,
                    ThreadNumber = this.Config.ThreadNumber,
                });
            });

            _faceLandmarker = new Lazy<FaceLandmarker>(() =>
            {
                return new FaceLandmarker(new FaceLandmarkConfig()
                {
                    MarkType = MarkType.Light,
                    DeviceType = this.Config.DeviceType,
                    ThreadNumber = this.Config.ThreadNumber,
                });
            });
        }

        /// <summary>
        /// 人脸质量评估
        /// </summary>
        /// <param name="image">人脸图像信息</param>
        /// <param name="info">面部信息<para>通过 <see cref="FaceDetector.Detect(FaceImage, int)"/> 获取</para></param>
        /// <param name="points"><paramref name="info"/> 对应的关键点坐标<para>通过 <see cref="FaceLandmarker.Mark(FaceImage, FaceInfo)"/> 获取</para></param>
        /// <param name="type">质量评估类型</param>
        /// <returns></returns>
        public QualityResult Detect(FaceImage image, FaceInfo info, FaceMarkPoint[] points, QualityType type)
        {
            lock (_locker)
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
                        SeetaFace6Native.QualityOfPoseEx(this.PoseEstimationModel.Ptr, ref image, info.Location, points, points.Length, ref level, ref score,
                           this.Config.PoseEx.YawLow, this.Config.PoseEx.YawHigh, this.Config.PoseEx.PitchLow, this.Config.PoseEx.PitchHigh, this.Config.PoseEx.RollLow, this.Config.PoseEx.RollHigh);
                        break;
                    case QualityType.Resolution:
                        SeetaFace6Native.QualityOfResolution(ref image, info.Location, points, points.Length, ref level, ref score, this.Config.Resolution.Low, this.Config.Resolution.High);
                        break;
                    case QualityType.ClarityEx:
                        {
                            QualityOfLBNResult qualityReault = points.Length == 68 ? _qualityOfLBN.Value.Detect(image, points) : _qualityOfLBN.Value.Detect(image, info);
                            return qualityReault.QualityResult;
                        }
                    case QualityType.Structure:
                        {
                            FaceMarkPointMask[] markResult = _faceLandmarker.Value.MarkV2(image, info);
                            if (markResult?.Any() != true)
                                throw new Exception("Land mark result is null.");
                            int maskCount = markResult.Count(p => p.IsMask);
                            if (maskCount > 0)
                            {
                                level = (int)QualityLevel.Low;
                                score = (1.0f - (float)maskCount / markResult.Length);
                            }
                            else
                            {
                                level = (int)QualityLevel.High;
                                score = 1.0f;
                            }
                            break;
                        }
                }
                return new QualityResult((QualityLevel)level, score, type);
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

                if (_qualityOfLBN != null && _qualityOfLBN.IsValueCreated)
                {
                    _qualityOfLBN.Value.Dispose();
                }
                if (_faceLandmarker != null && _faceLandmarker.IsValueCreated)
                {
                    _faceLandmarker.Value.Dispose();
                }

                this.Model.Dispose();
                this.PoseEstimationModel.Dispose();
            }
        }
    }
}
