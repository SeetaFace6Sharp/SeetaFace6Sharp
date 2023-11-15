using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;

namespace SeetaFace6Sharp
{
    /// <summary>
    /// 人脸图像信息数据
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct FaceImage : IDisposable, IEquatable<FaceImage>, IFormattable
    {
        [MarshalAs(UnmanagedType.I4)]
        private readonly int width;

        [MarshalAs(UnmanagedType.I4)]
        private readonly int height;

        [MarshalAs(UnmanagedType.I4)]
        private readonly int channels;

        [MarshalAs(UnmanagedType.SysInt)]
        private readonly IntPtr data = IntPtr.Zero;

        private readonly int _length = 0;

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="width">人脸图像宽度</param>
        /// <param name="height">人脸图像高度</param>
        /// <param name="channels">人脸图像通道数</param>
        /// <param name="buffer">人脸图像像素数据<para>按照 BGR 排列的 <see cref="byte"/> 列表</para></param>
        public FaceImage(int width, int height, int channels, byte[] buffer)
        {
            if (width <= 0 || height <= 0 || channels <= 0 || buffer == null || buffer.Length == 0)
            {
                throw new ArgumentException("Invalid argument");
            }

            this.width = width;
            this.height = height;
            this.channels = channels;
            this.data = Marshal.AllocHGlobal(buffer.Length);
            this._length = buffer.Length;

            Marshal.Copy(buffer, 0, data, buffer.Length);
        }

        /// <summary>
        /// 获取人脸图像宽度
        /// </summary>
        public readonly int Width => width;
        /// <summary>
        /// 获取人脸图像高度
        /// </summary>
        public readonly int Height => height;
        /// <summary>
        /// 获取人脸图像通道数
        /// </summary>
        public readonly int Channels => channels;

        /// <summary>
        /// 获取人脸图像BGR数据
        /// </summary>
        public byte[] Data
        {
            get
            {
                if (disposedValue)
                {
                    throw new ObjectDisposedException(nameof(FaceImage));
                }
                if (this.data == IntPtr.Zero || _length == 0)
                {
                    return Array.Empty<byte>();
                }
                var buffer = new byte[_length];
                for (int i = 0; i < _length; i++)
                {
                    buffer[i] = Marshal.ReadByte(data, i);
                }
                return buffer;
            }
        }

#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
        public static bool operator ==(FaceImage a, FaceImage b)
            => a.Width == b.Width
            && a.Height == b.Height
            && a.Channels == b.Channels
            && a._length == b._length
            && a.Data.SequenceEqual(b.Data);

        public static bool operator !=(FaceImage a, FaceImage b)
            => a.Width != b.Width
            || a.Height != b.Height
            || a.Channels != b.Channels
            || a._length != b._length
            || !a.Data.SequenceEqual(b.Data);

        public override bool Equals(object obj)
        {
            if (obj is FaceImage other)
            {
                return this == other;
            }
            else
            {
                return false;
            }
        }

        public bool Equals(FaceImage other)
        {
            return this == other;
        }

        public override int GetHashCode()
        {
            int hashCode = 1213452459;
            hashCode = hashCode * -1521134295 + Width.GetHashCode();
            hashCode = hashCode * -1521134295 + Height.GetHashCode();
            hashCode = hashCode * -1521134295 + Channels.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<IEnumerable<byte>>.Default.GetHashCode(Data);
            return hashCode;
        }

        private bool disposedValue;
        private static readonly object disposedLocker = new object();

        /// <summary>
        /// <see cref="IDisposable"/>
        /// </summary>
        public void Dispose()
        {
            if (disposedValue)
            {
                return;
            }

            lock (disposedLocker)
            {
                if (disposedValue)
                {
                    return;
                }
                disposedValue = true;
                if (this.data == IntPtr.Zero)
                {
                    return;
                }
                Marshal.FreeHGlobal(data);
            }
        }

        #region IFormattable
        /// <summary>
        /// 返回可视化字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString() => ToString(null, null);
        /// <summary>
        /// 返回可视化字符串
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        public string ToString(string format) => ToString(format, null);
        /// <summary>
        /// 返回可视化字符串
        /// </summary>
        /// <param name="format"></param>
        /// <param name="formatProvider"></param>
        /// <returns></returns>
        public string ToString(string format, IFormatProvider formatProvider)
        {
            string wtips = nameof(Width), htips = nameof(Height), ctips = nameof(Channels);

            if ((formatProvider ?? Thread.CurrentThread.CurrentCulture) is CultureInfo cultureInfo && cultureInfo.Name.StartsWith("zh"))
            { wtips = "宽度"; htips = "高度"; ctips = "通道数"; }

            return $"{{{wtips}:{Width}, {htips}:{Height}, {ctips}:{Channels}}}";
        }
        #endregion

#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释
    }
}