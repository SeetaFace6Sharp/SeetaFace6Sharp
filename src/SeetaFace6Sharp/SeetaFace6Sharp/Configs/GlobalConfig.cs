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
        private static Action<string> _logEvent { get; set; }

        /// <summary>
        /// 绑定log event
        /// </summary>
        /// <param name="action"></param>
        public static void SetLog(Action<string> action)
        {
            if (action == null)
                return;
            if (_logEvent != null)
                return;
            _logEvent = action;
        }

        /// <summary>
        /// 对外写日志
        /// </summary>
        /// <param name="log"></param>
        internal static void WriteLog(string log)
        {
            if (_logEvent == null)
                return;
            _logEvent(log);
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
            if (_pathResolver == null)
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

        #region Instruction

        private static bool _isSetX86Instruction = false;
        private static readonly object _setX86InstructionLocker = new object();
        private static X86Instruction _x86Instruction = DefaultX86Instruction;
        internal static X86Instruction DefaultX86Instruction = X86Instruction.AVX2 | X86Instruction.SSE2 | X86Instruction.FMA;

        /// <summary>
        /// x86或者x64环境下支持的指令集
        /// </summary>
        public static X86Instruction X86Instruction
        {
            get
            {
                if (!_isSetX86Instruction)
                {
                    lock (_setX86InstructionLocker)
                    {
                        if (!_isSetX86Instruction)
                        {
                            _isSetX86Instruction = true;
                        }
                    }
                }
                return _x86Instruction;
            }
            set
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
                    _x86Instruction = value;
                }
            }
        }

        #endregion

        #region DeviceType

        private static DeviceType _deviceType = DeviceType.AUTO;

        /// <summary>
        /// 全局使用的设备类型
        /// </summary>
        public static DeviceType DefaultDeviceType
        {
            get
            {
                return _deviceType;
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

        #endregion

        /// <summary>
        /// 特殊的操作系统平台
        /// </summary>
        public static SpecialOSPlatform SpecialOSPlatform { get; set; }

        #region Threads

        private static int _maxThreadNumber = 8;

        /// <summary>
        /// 默认能使用的最大线程数量（默认8）
        /// </summary>
        public static int MaxThreadNumber
        {
            get
            {
                return _maxThreadNumber;
            }
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException(nameof(MaxThreadNumber), "The number of threads max used cannot be less than or equal to 0.");
                _maxThreadNumber = value;
            }
        }

        private static int _threadNumber = -1;

        /// <summary>
        /// 指定线程数量
        /// </summary>
        public static int ThreadNumber
        {
            get
            {
                if (_threadNumber == -1)
                {
                    return Environment.ProcessorCount > MaxThreadNumber ? MaxThreadNumber : Environment.ProcessorCount;
                }
                return _threadNumber;
            }
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException(nameof(ThreadNumber), "The number of threads cannot be less than or equal to 0.");
                _threadNumber = value;
                if (MaxThreadNumber > _threadNumber)
                {
                    WriteLog($"The input number of threads exceeds the maximum thread limit of {MaxThreadNumber}, the number of threads will be set to the maximum thread limit of {MaxThreadNumber}.");
                    _threadNumber = MaxThreadNumber;
                }
                if (_threadNumber > Environment.ProcessorCount)
                {
                    WriteLog($"The number of threads exceeds logical processors count {Environment.ProcessorCount}. It is not recommended for the number of threads to exceed the number of logical processors");
                }
            }
        }

        #endregion
    }
}