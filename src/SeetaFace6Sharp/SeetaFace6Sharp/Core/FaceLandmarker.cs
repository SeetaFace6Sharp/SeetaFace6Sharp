﻿using SeetaFace6Sharp.Native;
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

        /// <inheritdoc/>
        /// <exception cref="ModuleInitializeException"></exception>
        public FaceLandmarker(FaceLandmarkConfig config = null) : base(config ?? new FaceLandmarkConfig())
        {
            _handle = SeetaFace6Native.GetFaceLandmarkerHandler((int)this.Config.MarkType, (int)this.Config.DeviceType);
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

                long size = 0;
                var ptr = SeetaFace6Native.FaceMark(_handle, ref image, info.Location, ref size);
                if (ptr == IntPtr.Zero) return new FaceMarkPoint[0];
                try
                {
                    FaceMarkPoint[] result = new FaceMarkPoint[size];
                    for (int i = 0; i < size; i++)
                    {
                        var ofs = i * Marshal.SizeOf(typeof(FaceMarkPoint));
                        result[i] = (FaceMarkPoint)Marshal.PtrToStructure(ptr + ofs, typeof(FaceMarkPoint));
                    }
                    return result;
                }
                finally
                {
                    SeetaFace6Native.FreeMemory(ptr);
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
            }
        }
    }
}
