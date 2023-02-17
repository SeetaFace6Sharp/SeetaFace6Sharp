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
                else return GlobalConfig.GetDefaultDeviceType();
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
    }
}
