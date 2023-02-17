using System;

namespace SeetaFace6Sharp
{
    /// <summary>
    /// SeetaFace6 Interface
    /// </summary>
    public interface ISeetaFace6 : IDisposable
    {
        /// <summary>
        /// 获取模型路径
        /// </summary>
        public string ModelPath { get; }

        /// <summary>
        /// 获取库路径
        /// </summary>
        public string LibraryPath { get; }
    }
}