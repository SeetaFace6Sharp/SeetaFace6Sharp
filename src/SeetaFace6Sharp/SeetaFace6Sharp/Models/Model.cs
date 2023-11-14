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
        private IntPtr _ptr = IntPtr.Zero;

        /// <summary>
        /// Model对象指针
        /// </summary>
        internal IntPtr Ptr
        {
            get
            {
                if (disposedValue) throw new ObjectDisposedException(nameof(Model));
                if (_ptr == IntPtr.Zero) throw new ModuleInitializeException(nameof(Model), "The model not init.");
                return _ptr;
            }
        }

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

            if ((_ptr = SeetaFace6Native.GetModel(this.Path, (int)this.DeviceType)) == IntPtr.Zero)
            {
                throw new ModuleInitializeException(nameof(FaceRecognizer), "Get model ptr failed.");
            }
        }

        /// <summary>
        /// Append model
        /// </summary>
        /// <param name="extModel"></param>
        /// <exception cref="FileNotFoundException"></exception>
        public void Append(string extModel)
        {
            if (File.Exists(extModel))
            {
                extModel = System.IO.Path.GetFullPath(extModel);
            }
            else
            {
                extModel = System.IO.Path.Combine(GlobalConfig.GetPathResolver().GetModelsPath(), extModel);
                if (!File.Exists(extModel))
                {
                    throw new FileNotFoundException($"The model file '{this.Path}' not found.");
                }
            }

            if (disposedValue) return;

            lock (_locker)
            {
                if (disposedValue) return;
                SeetaFace6Native.Append(this.Ptr, extModel);
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            if (disposedValue)
                return;

            lock (_locker)
            {
                if (disposedValue) return;
                if (this.Ptr != IntPtr.Zero) 
                    SeetaFace6Native.DisposeModel(this.Ptr);
                disposedValue = true;
            }
        }
    }
}
