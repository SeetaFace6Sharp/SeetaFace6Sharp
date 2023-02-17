using System;

namespace SeetaFace6Sharp
{

    /// <summary>
    /// 句柄获取异常
    /// </summary>
    public class ModuleInitializeException : Exception
    {
        /// <summary>
        /// 模块
        /// </summary>
        public string Module { get; private set; }

        /// <summary>
        /// 句柄获取异常
        /// </summary>
        /// <param name="module">模块</param>
        /// <param name="message">异常信息</param>
        public ModuleInitializeException(string module, string message) : base(message)
        {
            this.Module= module;
        }

        /// <summary>
        /// 句柄获取异常
        /// </summary>
        /// <param name="module">模块</param>
        /// <param name="message">异常信息</param>
        /// <param name="innerException">引发的异常</param>
        public ModuleInitializeException(string module, string message, Exception innerException) : base(message, innerException)
        {
            this.Module = module;
        }
    }
}