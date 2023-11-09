using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SeetaFace6Sharp.Native;

namespace SeetaFace6Sharp.Models
{
    /// <summary>
    /// 需要的模型
    /// </summary>
    public sealed class Model : IDisposable
    {
        /// <summary>
        /// Model对象指针
        /// </summary>
        internal IntPtr Ptr { get; }

        /// <summary>
        /// 模型路径
        /// </summary>
        public string Path { get; }

        /// <summary>
        /// 设备类型
        /// </summary>
        public DeviceType DeviceType { get; }

        private readonly static object _locker = new object();

        private bool disposedValue = false;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <param name="deviceType"></param>
        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="ModuleInitializeException"></exception>
        public Model(string model, DeviceType deviceType = DeviceType.AUTO)
        {
            this.Path = System.IO.Path.Combine(GlobalConfig.GetPathResolver().GetModelsPath(), model);
            if (!File.Exists(this.Path))
            {
                throw new FileNotFoundException($"The model file '{this.Path}' not found.");
            }
            this.DeviceType = deviceType;

            if ((this.Ptr = SeetaFace6Native.GetModel(this.Path, (int)this.DeviceType)) == IntPtr.Zero)
            {
                throw new ModuleInitializeException(nameof(FaceRecognizer), "Get model ptr failed.");
            }
        }

        public void Dispose()
        {
            if (disposedValue)
                return;

            lock (_locker)
            {
                if (disposedValue) return;

                disposedValue = true;
                if (this.Ptr != IntPtr.Zero) SeetaFace6Native.DisposeModel(this.Ptr);
            }
        }
    }
}
