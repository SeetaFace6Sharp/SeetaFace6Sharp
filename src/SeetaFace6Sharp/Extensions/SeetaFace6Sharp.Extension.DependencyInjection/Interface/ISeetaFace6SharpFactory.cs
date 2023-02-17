using System;
using System.Collections.Generic;
using System.Text;
using SeetaFace6Sharp;

namespace SeetaFace6Sharp.Extension.DependencyInjection
{
    /// <summary>
    /// 获取SeetaFace对象工厂接口
    /// </summary>
    public interface ISeetaFace6SharpFactory
    {
        /// <summary>
        /// 获取SeetaFace对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T Get<T>() where T : ISeetaFace6;
    }
}
