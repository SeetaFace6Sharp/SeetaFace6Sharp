using System;
using System.Linq;
using System.Runtime.CompilerServices;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace SeetaFace6Sharp
{
    /// <summary>
    /// ImageSharp扩展
    /// </summary>
    public static class SeetaFace6SharpImageSharpExtension
    {
        /// <summary>
        /// Image convert to FaceImage
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public static FaceImage ToFaceImage(this Image image)
        {
            byte[] data = To24BGRByteArray(image, out int width, out int height, out int channels);
            return new FaceImage(width, height, channels, data);
        }

        /// <summary>
        /// <see cref="Image"/> 转为 3*8bit BGR <see cref="byte"/> 数组。
        /// </summary>
        /// <param name="source">待转换图像</param>
        /// <param name="width">图像宽度</param>
        /// <param name="height">图像高度</param>
        /// <param name="channels">图像通道</param>
        /// <returns>图像的 BGR <see cref="byte"/> 数组</returns>
        private static byte[] To24BGRByteArray(Image source, out int width, out int height, out int channels)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            channels = 3;
            if (source.GetType()?.GenericTypeArguments?.Any(p => p == typeof(Bgra32)) == true)
            {
                width = source.Width;
                height = source.Height;
                return ConvertToByte((Image<Bgra32>)source, channels);
            }
            else
            {
                using (var bitmap = source.CloneAs<Bgra32>())
                {
                    width = bitmap.Width;
                    height = bitmap.Height;
                    return ConvertToByte((Image<Bgra32>)bitmap, channels);
                }
            }
        }

        /// <summary>
        /// 转为BGR Bytes
        /// </summary>
        /// <param name="source"></param>
        /// <param name="channels"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private static byte[] ConvertToByte(Image<Bgra32> source, int channels)
        {
            byte[] pixelBytes = new byte[source.Width * source.Height * Unsafe.SizeOf<Bgra32>()];
            source.CopyPixelDataTo(pixelBytes);
            byte[] bgra = new byte[pixelBytes.Length / 4 * channels];
            // brga
            int j = 0;
            for (int i = 0; i < pixelBytes.Length; i++)
            {
                if ((i + 1) % 4 == 0) continue;
                bgra[j] = pixelBytes[i];
                j++;
            }
            return bgra;
        }
    }
}
