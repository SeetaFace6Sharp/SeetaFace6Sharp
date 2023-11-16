using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Threading;

namespace SeetaFace6Sharp
{
    /// <summary>
    /// 点坐标
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public class FaceMarkPointMask : IFormattable
    {
        private readonly FaceMarkPoint point;

        private readonly bool mask;

        /// <summary>
        /// 坐标
        /// </summary>
        public FaceMarkPoint Point => point;

        /// <summary>
        /// 是否遮挡
        /// </summary>
        public bool IsMask => mask;

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
            string xtips = nameof(point.X), ytips = nameof(point.Y);

            if ((formatProvider ?? Thread.CurrentThread.CurrentCulture) is CultureInfo cultureInfo && cultureInfo.Name.StartsWith("zh"))
            { xtips = "X坐标"; ytips = "Y坐标"; }

            return $"{{{xtips}:{point.X}, {ytips}:{point.Y}}}, {mask}";
        }
        #endregion
    }
}
