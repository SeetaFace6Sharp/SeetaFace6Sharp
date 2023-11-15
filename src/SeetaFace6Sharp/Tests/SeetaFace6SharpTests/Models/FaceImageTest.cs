using Microsoft.VisualStudio.TestTools.UnitTesting;
using SeetaFace6Sharp;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace SeetaFace6SharpTests.Models
{
    [TestClass()]
    public class FaceImageTest
    {
        private const SKColorType targetColorType = SKColorType.Bgra8888;

        [TestMethod()]
        public void IsEqualTest()
        {
            using var skBitmap = SKBitmap.Decode(@"images/more.jpg");
            using var bitmap = skBitmap.ToFaceImage();

            byte[] sourceData = To24BGRByteArray(skBitmap, out int width, out int height, out int channels);
            var targetData = bitmap.Data;
            if(sourceData.Length != targetData.Length)
            {
                Assert.Fail();
            }
            for(int i = 0; i < sourceData.Length; i++)
            {
                if (sourceData[i] != targetData[i]) Assert.Fail();
            }
        }


        [TestMethod()]
        public void IsEqualTest2()
        {
            using var skBitmap = SKBitmap.Decode(@"images/more.jpg");
            using var bitmap1 = skBitmap.ToFaceImage();
            using var bitmap2 = skBitmap.ToFaceImage();

            if(bitmap1 != bitmap2)
            {
                Assert.Fail();
            }
            if (!(bitmap1 == bitmap2))
            {
                Assert.Fail();
            }
        }

        #region Private

        /// <summary>
        /// <see cref="Bitmap"/> 转为 3*8bit BGR <see cref="byte"/> 数组。
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
            if (source.ColorType != targetColorType)
            {
                using (SKBitmap bitmap = ConvertToBgra8888(source))
                {
                    width = bitmap.Width;
                    height = bitmap.Height;
                    return ConvertToByte(bitmap, channels);
                }
            }
            else
            {
                width = source.Width;
                height = source.Height;
                return ConvertToByte(source, channels);
            }
        }

        /// <summary>
        /// 转换图像格式
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private static SKBitmap ConvertToBgra8888(SKBitmap source)
        {
            if (!source.CanCopyTo(targetColorType))
            {
                throw new Exception("Can not copy image color type to Bgra8888");
            }
            SKBitmap bitmap = new SKBitmap();
            source.CopyTo(bitmap, targetColorType);
            if (bitmap == null)
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
            byte[] array = source.Bytes;
            if (array == null || array.Length == 0)
            {
                throw new Exception("SKBitmap data is null");
            }
            byte[] bgra = new byte[array.Length / 4 * channels];
            // brga
            int j = 0;
            for (int i = 0; i < array.Length; i++)
            {
                if ((i + 1) % 4 == 0) continue;
                bgra[j] = array[i];
                j++;
            }
            return bgra;
        }

        #endregion
    }
}
