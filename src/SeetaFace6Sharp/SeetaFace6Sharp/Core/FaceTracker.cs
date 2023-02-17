﻿using SeetaFace6Sharp.Native;
using System;
using System.Runtime.InteropServices;

namespace SeetaFace6Sharp
{
    /// <summary>
    /// 人脸追踪器
    /// </summary>
    public sealed class FaceTracker : BaseSeetaFace6<FaceTrackerConfig>
    {
        private readonly IntPtr _handle = IntPtr.Zero;
        private readonly static object _locker = new object();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ModuleInitializeException"></exception>
        public FaceTracker(FaceTrackerConfig config) : base(config ?? throw new ArgumentNullException(nameof(config), $"Param '{nameof(config)}' can not null."))
        {
            _handle = SeetaFace6Native.GetFaceTrackerHandler(config.Width, config.Height, config.Stable, config.Interval, config.MinFaceSize, config.Threshold, (int)config.DeviceType);
            if (_handle == IntPtr.Zero)
            {
                throw new ModuleInitializeException(nameof(FaceLandmarker), "Get face track handle failed.");
            }
        }

        /// <summary>
        /// 识别 <paramref name="image"/> 中的人脸，并返回可跟踪的人脸信息。
        /// <para>
        /// 当 <c><see cref="FaceType"/> <see langword="="/> <see cref="FaceType.Normal"/> <see langword="||"/> <see cref="FaceType.Light"/></c> 时， 需要模型：<a href="https://www.nuget.org/packages/SeetaFace6Sharp.model.face_detector">face_detector.csta</a><br/>
        /// 当 <c><see cref="FaceType"/> <see langword="="/> <see cref="FaceType.Mask"/></c> 时， 需要模型：<a href="https://www.nuget.org/packages/SeetaFace6Sharp.model.mask_detector">mask_detector.csta</a><br/>
        /// </para>
        /// </summary>
        /// <param name="image">人脸图像信息</param>
        /// <returns>人脸信息集合。若 <see cref="Array.Length"/> == 0 ，代表未检测到人脸信息。如果图片中确实有人脸，可以修改 <see cref="FaceTrackerConfig"/> 重新检测。</returns>
        public FaceTrackInfo[] Track(FaceImage image)
        {
            lock (_locker)
            {
                if (disposedValue)
                    throw new ObjectDisposedException(nameof(FaceTracker));

                int size = 0;
                var ptr = SeetaFace6Native.FaceTrack(_handle, ref image, ref size);
                if (ptr == IntPtr.Zero) return new FaceTrackInfo[0];
                try
                {
                    FaceTrackInfo[] result = new FaceTrackInfo[size];
                    for (int i = 0; i < size; i++)
                    {
                        int ofs = i * Marshal.SizeOf(typeof(FaceTrackInfo));
                        var info = (FaceTrackInfo)Marshal.PtrToStructure(ptr + ofs, typeof(FaceTrackInfo));
                        result[i] = info;
                    }
                    return result;
                }
                finally
                {
                    SeetaFace6Native.Free(ptr);
                }
            }
        }

        /// <summary>
        /// 重置追踪的视频
        /// </summary>
        public void Reset()
        {
            lock (_locker)
            {
                if (disposedValue)
                    throw new ObjectDisposedException(nameof(FaceTracker));

                SeetaFace6Native.FaceTrackReset(_handle);
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
                SeetaFace6Native.DisposeFaceTracker(_handle);
            }
        }
    }
}