using SeetaFace6Sharp.Native.PathResolvers;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SeetaFace6Sharp
{

    /// <summary>
    /// 全局配置
    /// </summary>
    public static class GlobalConfig
    {
        #region Log

        /// <summary>
        /// 日志事件绑定
        /// </summary>
        private static Action<string> LogEvent { get; set; }

        /// <summary>
        /// 绑定log event
        /// </summary>
        /// <param name="action"></param>
        public static void SetLog(Action<string> action)
        {
            if (action == null)
                return;
            if (LogEvent != null)
                return;
            LogEvent = action;
        }

        /// <summary>
        /// 对外写日志
        /// </summary>
        /// <param name="log"></param>
        internal static void WriteLog(string log)
        {
            if (LogEvent == null)
                return;
            LogEvent(log);
        }

        #endregion

        #region Instruction

        private static bool _isSetX86Instruction = false;

        private static readonly object _setX86InstructionLocker = new object();

        /// <summary>
        /// 在x86环境下支持的指令集
        /// </summary>
        private static X86Instruction _x86Instruction = X86Instruction.AVX2 | X86Instruction.SSE2 | X86Instruction.FMA;

        /// <summary>
        /// 设置支持的指令集
        /// </summary>
        /// <param name="instruction"></param>
        public static void SetX86Instruction(X86Instruction instruction)
        {
            if (_isSetX86Instruction)
                return;
            if (RuntimeInformation.ProcessArchitecture != Architecture.X86 && RuntimeInformation.ProcessArchitecture != Architecture.X64)
                return;
            lock (_setX86InstructionLocker)
            {
                if (_isSetX86Instruction)
                    return;
                _isSetX86Instruction = true;
                _x86Instruction = instruction;
            }
        }

        internal static X86Instruction GetX86Instruction()
        {
            return _x86Instruction;
        }

        #endregion

        #region PathResolver

        /// <summary>
        /// 使用的路径解析器
        /// </summary>
        private static IPathResolver _pathResolver = null;

        /// <summary>
        /// 获取路径解析器
        /// </summary>
        /// <returns></returns>
        internal static IPathResolver GetPathResolver()
        {
            if(_pathResolver == null)
            {
                return new DefaultPathResolver();
            }
            return _pathResolver;
        }

        /// <summary>
        /// 设置路径解析器
        /// </summary>
        /// <param name="pathResolver"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void SetPathResolver(IPathResolver pathResolver)
        {
            _pathResolver = pathResolver ?? throw new ArgumentNullException(nameof(pathResolver));
        }

        #endregion

        #region DeviceType

        private static DeviceType _deviceType = DeviceType.AUTO;

        /// <summary>
        /// 获取全局设备类型配置
        /// </summary>
        /// <returns></returns>
        public static DeviceType GetDefaultDeviceType()
        {
            return _deviceType;
        }

        /// <summary>
        /// 设置全局设备类型（不设置默认值为自动）
        /// </summary>
        /// <param name="deviceType"></param>
        public static void SetDefaultDeviceType(DeviceType deviceType)
        {
            if (deviceType < DeviceType.AUTO || deviceType > DeviceType.GPU)
            {
                throw new ArgumentException($"Invalid parameter. Value cannot be {(int)deviceType}");
            }
            _deviceType = deviceType;
        }

        #endregion
    }
}