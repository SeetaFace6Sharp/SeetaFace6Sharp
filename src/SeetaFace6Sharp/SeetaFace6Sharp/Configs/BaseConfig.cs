using System;

namespace SeetaFace6Sharp
{
    /// <summary>
    /// 通用配置
    /// </summary>
    public abstract class BaseConfig
    {
        private DeviceType? _deviceType = null;

        /// <summary>
        /// 识别用的设备类型
        /// </summary>
        /// <remarks>
        /// 默认只支持CPU，GPU需要自行编译或引入已经编译好的GPU版本
        /// </remarks>
        public DeviceType DeviceType
        {
            get
            {
                if (_deviceType != null) return _deviceType.Value;
                else return GlobalConfig.DefaultDeviceType;
            }
            set
            {
                if (value < DeviceType.AUTO || value > DeviceType.GPU)
                {
                    throw new ArgumentException($"Invalid parameter. Value cannot be {(int)value}");
                }
                _deviceType = value;
            }
        }

        private int? _threadNumber = null;

        /// <summary>
        /// 检测所用的线程数量
        /// </summary>
        /// <remarks>
        /// 如果不设置，默认使用逻辑处理器数量（默认不超过16个）
        /// </remarks>
        public int ThreadNumber
        {
            get
            {
                if (_threadNumber != null) return _threadNumber.Value;
                else return GlobalConfig.ThreadNumber;
            }
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException(nameof(ThreadNumber), "The number of threads cannot be less than or equal to 0.");
                _threadNumber = value;
            }
        }
    }
}
