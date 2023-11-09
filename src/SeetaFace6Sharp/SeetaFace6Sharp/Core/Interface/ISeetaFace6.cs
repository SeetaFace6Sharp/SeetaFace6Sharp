using System;
using SeetaFace6Sharp.Models;

namespace SeetaFace6Sharp
{
    /// <summary>
    /// SeetaFace6 Interface
    /// </summary>
    public interface ISeetaFace6 : IDisposable
    {
        /// <summary>
        /// 获取库路径
        /// </summary>
        string LibraryPath { get; }

        /// <summary>
        /// 所需模型
        /// </summary>
        Model Model { get; }
    }
}