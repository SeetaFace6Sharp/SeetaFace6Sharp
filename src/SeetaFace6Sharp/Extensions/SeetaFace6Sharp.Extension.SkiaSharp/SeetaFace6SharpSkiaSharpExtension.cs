using SkiaSharp;
using System;

namespace SeetaFace6Sharp
{
    /// <summary>
    /// SkiaSharp扩展
    /// </summary>
    public static class SeetaFace6SharpSkiaSharpExtension
    {
        private const SKColorType TargetColorType = SKColorType.Bgra8888;

        /// <summary>
        /// SKBitmap convert to FaceImage
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public static FaceImage ToFaceImage(this SKBitmap image)
        {
            var data = To24BGRByteArray(image, out int width, out int height, out int channels);
            return new FaceImage(width, height, channels, data);
        }

        #region Private

        /// <summary>
        /// <see cref="SKBitmap"/> 转为 3*8bit BGR <see cref="byte"/> 数组。
        /// </summary>
        /// <param name="source">待转换图像</param>
        /// <param name="width">图像宽度</param>
        /// <param name="height">图像高度</param>
        /// <param name="channels">图像通道</param>
        /// <returns>图像的 BGR <see cref="byte"/> 数组</returns>
        private static byte[] To24BGRByteArray(SKBitmap source, out int width, out int height, out int channels)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            channels = 3;

            // 如果源图像的色彩类型不是目标色彩类型，则进行转换
            if (source.ColorType != TargetColorType)
            {
                using var bitmap = ConvertToBgra8888(source);
                width = bitmap.Width;
                height = bitmap.Height;
                return ConvertToByte(bitmap, channels);
            }

            // 如果源图像已经是目标色彩类型，直接处理
            width = source.Width;
            height = source.Height;
            return ConvertToByte(source, channels);
        }

        /// <summary>
        /// 转换图像格式
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private static SKBitmap ConvertToBgra8888(SKBitmap source)
        {
            if (!source.CanCopyTo(TargetColorType))
            {
                throw new Exception("Cannot copy image color type to Bgra8888");
            }

            // 直接在构造函数中指定目标色彩类型，避免额外的赋值操作
            var bitmap = new SKBitmap(source.Width, source.Height, TargetColorType, source.AlphaType);
            if (!source.CopyTo(bitmap))
            {
                throw new Exception("Copy image to Bgra8888 failed");
            }

            return bitmap;
        }

        /// <summary>
        /// 转为BGR Bytes
        /// </summary>
        /// <param name="source"></param>
        /// <param name="channels"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private static byte[] ConvertToByte(SKBitmap source, int channels)
        {
            var array = source.Bytes;
            if (array == null || array.Length == 0)
            {
                throw new Exception("SKBitmap data is null");
            }

            // 提前计算目标数组的大小，避免在循环中动态调整
            var bgra = new byte[source.Width * source.Height * channels];

            // 优化循环逻辑，减少条件判断
            int srcIndex = 0, dstIndex = 0;
            int pixelCount = source.Width * source.Height;

            for (int i = 0; i < pixelCount; i++)
            {
                bgra[dstIndex++] = array[srcIndex++]; // B
                bgra[dstIndex++] = array[srcIndex++]; // G
                bgra[dstIndex++] = array[srcIndex++]; // R
                srcIndex++; // 跳过 Alpha 通道
            }

            return bgra;
        }

        #endregion
    }
}
