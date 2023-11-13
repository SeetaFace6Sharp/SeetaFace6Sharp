using SeetaFace6Sharp.Models;
using SeetaFace6Sharp.Native;
using System;
using System.Runtime.InteropServices;

namespace SeetaFace6Sharp
{
    /// <summary>
    /// 识别指定的人脸信息的关键点坐标。
    /// </summary>
    public sealed class FaceLandmarker : BaseSeetaFace6<FaceLandmarkConfig>
    {
        private readonly IntPtr _handle = IntPtr.Zero;
        private readonly static object _locker = new object();

        /// <summary>
        /// 人脸识别所需model
        /// </summary>
        public override Model Model { get; }

        /// <summary>
        /// 关键点坐标大小
        /// </summary>
        private int MarkPointSize
        {
            get
            {
                return this.Config.MarkType switch
                {
                    MarkType.Normal => 68,
                    MarkType.Mask => 5,
                    MarkType.Light => 5,
                    _ => throw new NotSupportedException($"Not support face type: {this.Config.MarkType}."),
                };
            }
        }

        /// <inheritdoc/>
        /// <exception cref="ModuleInitializeException"></exception>
        public FaceLandmarker(FaceLandmarkConfig config = null) : base(config ?? new FaceLandmarkConfig())
        {
            string model = this.Config.MarkType switch
            {
                MarkType.Normal => "face_landmarker_pts68.csta",
                MarkType.Mask => "face_landmarker_mask_pts5.csta",
                MarkType.Light => "face_landmarker_pts5.csta",
                _ => throw new NotSupportedException($"Not support face type: {this.Config.MarkType}."),
            };
            this.Model = new Model(model, this.Config.DeviceType);
            _handle = SeetaFace6Native.GetFaceLandmarkerHandler(this.Model.Ptr);
            if (_handle == IntPtr.Zero)
            {
                throw new ModuleInitializeException(nameof(FaceLandmarker), "Get face landmarker handle failed.");
            }
        }

        /// <summary>
        /// 识别 <paramref name="image"/> 中指定的人脸信息 <paramref name="info"/> 的关键点坐标。
        /// </summary>
        /// <param name="image">人脸图像信息</param>
        /// <param name="info">指定的人脸信息</param>
        /// <exception cref="ObjectDisposedException"/>
        /// <returns>若失败，则返回结果 Length == 0</returns>
        public FaceMarkPoint[] Mark(FaceImage image, FaceInfo info)
        {
            lock (_locker)
            {
                if (disposedValue)
                    throw new ObjectDisposedException(nameof(FaceAntiSpoofing));

                int sizeOfFaceMarkPoint = Marshal.SizeOf(typeof(FaceMarkPoint));
                IntPtr buffer = Marshal.AllocHGlobal(this.MarkPointSize * sizeOfFaceMarkPoint);
                if (buffer == IntPtr.Zero)
                    return new FaceMarkPoint[0];
                try
                {
                    long size = 0;
                    int rtCode = SeetaFace6Native.FaceMark(_handle, ref image, info.Location, this.MarkPointSize, buffer, ref size);
                    if (rtCode != 0)
                    {
                        throw rtCode switch
                        {
                            -2 => new Exception($"Face mark failed, default mark point size {this.MarkPointSize} is not same as face mark size {size}."),
                            _ => new Exception($"Face mark failed, result value is {rtCode}"),
                        };
                    }

                    FaceMarkPoint[] result = new FaceMarkPoint[size];
                    for (int i = 0; i < size; i++)
                    {
                        IntPtr p = new IntPtr(buffer.ToInt64() + i * sizeOfFaceMarkPoint);
                        result[i] = (FaceMarkPoint)Marshal.PtrToStructure(p, typeof(FaceMarkPoint));
                    }
                    return result;
                }
                finally
                {
                    if (buffer != IntPtr.Zero)
                        Marshal.FreeHGlobal(buffer);
                }
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
                SeetaFace6Native.DisposeFaceLandmarker(_handle);
                this.Model?.Dispose();
            }
        }
    }
}
