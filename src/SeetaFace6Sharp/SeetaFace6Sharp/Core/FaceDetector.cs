using SeetaFace6Sharp.Models;
using SeetaFace6Sharp.Native;
using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace SeetaFace6Sharp
{
    /// <summary>
    /// 识别人脸的信息。
    /// </summary>
    public sealed class FaceDetector : BaseSeetaFace6<FaceDetectConfig>
    {
        private readonly IntPtr _handle = IntPtr.Zero;
        private readonly static object _locker = new object();

        /// <summary>
        /// 人脸识别所需model
        /// </summary>
        public override Model Model { get; }

        /// <inheritdoc/>
        /// <exception cref="ModuleInitializeException"></exception>
        public FaceDetector(FaceDetectConfig config = null) : base(config ?? new FaceDetectConfig())
        {
            this.Model = new Model("face_detector.csta", this.Config.DeviceType);
            _handle = SeetaFace6Native.GetFaceDetectorHandler(this.Model.Ptr, this.Config.FaceSize, this.Config.Threshold, this.Config.MaxWidth, this.Config.MaxHeight);
            if (_handle == IntPtr.Zero)
            {
                throw new ModuleInitializeException(nameof(FaceDetector), "Get face detector handle failed.");
            }
        }

        /// <summary>
        /// 识别 <paramref name="image"/> 中的人脸，并返回人脸的信息。
        /// <para>
        /// 可以通过 <see cref="FaceDetectConfig.FaceDetectConfig"/> 属性对人脸检测器进行配置，以应对不同场景的图片。
        /// </para>
        /// </summary>
        /// <param name="image">人脸图像信息</param>
        /// <param name="maxFaceCount">单张图片中最多人脸数量（预分配内存）</param>
        /// <returns>人脸信息集合。</returns>
        public FaceInfo[] Detect(FaceImage image, int maxFaceCount = 30)
        {
            if (maxFaceCount <= 0 || maxFaceCount >= byte.MaxValue)
            {
                throw new ArgumentOutOfRangeException("The parameter must be greater than 0 and less than 255.");
            }
            lock (_locker)
            {
                if (disposedValue)
                    throw new ObjectDisposedException(nameof(FaceAntiSpoofing));

                int sizeOfFaceInfo = Marshal.SizeOf(typeof(FaceInfo));
                IntPtr buffer = Marshal.AllocHGlobal(maxFaceCount * sizeOfFaceInfo);
                try
                {
                    int size = 0;
                    int rtCode = SeetaFace6Native.FaceDetectV2(_handle, ref image, maxFaceCount, buffer, ref size);
                    if (rtCode != 0)
                    {
                        throw new Exception($"Face detect failed, result code id {rtCode}");
                    }
                    if (size == 0)
                    {
                        return new FaceInfo[0];
                    }
                    FaceInfo[] result = new FaceInfo[size];
                    for (int i = 0; i < size; i++)
                    {
                        IntPtr p = new IntPtr(buffer.ToInt64() + i * sizeOfFaceInfo);
                        result[i] = (FaceInfo)Marshal.PtrToStructure(p, typeof(FaceInfo));
                    }
                    return result.OrderBy(p => p.Score).ToArray();
                }
                finally
                {
                    if (buffer != IntPtr.Zero)
                        Marshal.FreeHGlobal(buffer);
                }
            }
        }

        /// <summary>
        /// <see cref="IDisposable"/>
        /// </summary>
        public override void Dispose()
        {
            if (disposedValue) return;

            lock (_locker)
            {
                if (disposedValue) return;
                disposedValue = true;
                if (_handle == IntPtr.Zero) return;
                SeetaFace6Native.DisposeFaceDetector(_handle);
                this.Model?.Dispose();
            }
        }
    }
}