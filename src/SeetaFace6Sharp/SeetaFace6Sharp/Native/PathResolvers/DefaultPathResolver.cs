using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using SeetaFace6Sharp.Native.LibraryLoader.Interface;

namespace SeetaFace6Sharp.Native.PathResolvers
{
    /// <summary>
    /// 默认路径解析器
    /// </summary>
    public class DefaultPathResolver : BasePathResolver
    {
        private string _modelsPath = null;
        private string _libraryPath = null;

        private const string DEFAULT_LIBRARY_PATH = "runtimes";
        private const string DEFAULT_MODELS_PATH = "models";

        /// <summary>
        /// 获取默认模型存放位置
        /// </summary>
        /// <returns></returns>
        /// <exception cref="DirectoryNotFoundException"></exception>
        public override string GetModelsPath()
        {
            if (!string.IsNullOrEmpty(_modelsPath))
            {
                return _modelsPath;
            }
            if (TryCombine(out string modelsPath, DEFAULT_LIBRARY_PATH, DEFAULT_MODELS_PATH))
            {
                _modelsPath = modelsPath;
                return modelsPath;
            }
            throw new DirectoryNotFoundException("Can not found default models path.");
        }

        /// <summary>
        /// 获取默认动态库存放路径
        /// </summary>
        /// <returns></returns>
        /// <exception cref="PlatformNotSupportedException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        public override string GetLibraryPath()
        {
            if (!string.IsNullOrEmpty(_libraryPath))
                return _libraryPath;

            string archPath = GetProcessArchitecturePath();
            if (string.IsNullOrWhiteSpace(archPath))
                throw new PlatformNotSupportedException($"Unsupported processor architecture: {RuntimeInformation.ProcessArchitecture}, system type: {RuntimeInformation.OSDescription}");

            string gpuMark = null;
            if (GlobalConfig.DefaultDeviceType == DeviceType.GPU)
                gpuMark = "gpu";

            if (!TryCombine(out string libraryPath, DEFAULT_LIBRARY_PATH, archPath, "native", gpuMark))
                throw new DirectoryNotFoundException("Can not found library path.");

            _libraryPath = libraryPath;
            return _libraryPath;
        }

        #region private
        private string GetProcessArchitecturePath()
        {
            string architecture = string.Empty;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                architecture = RuntimeInformation.ProcessArchitecture switch
                {
                    Architecture.X86 => "win-x86",
                    Architecture.X64 => "win-x64",
                    Architecture.Arm64 => "win-arm64",
                    _ => throw new PlatformNotSupportedException($"Unsupported processor architecture: {RuntimeInformation.ProcessArchitecture}"),
                };
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                architecture = RuntimeInformation.ProcessArchitecture switch
                {
                    Architecture.X64 => "linux-x64",
                    Architecture.Arm => "linux-arm",
                    Architecture.Arm64 => "linux-arm64",
#if NET7_0_OR_GREATER
                    Architecture.LoongArch64 => "linux-loongarch64",
#endif
                    _ => throw new PlatformNotSupportedException($"Unsupported processor architecture: {RuntimeInformation.ProcessArchitecture}"),
                };
            }
            else
            {
                throw new PlatformNotSupportedException($"Unsupported system type: {RuntimeInformation.OSDescription}");
            }
            return architecture;
        }

        private bool TryCombine(out string path, params string[] paths)
        {
            if (paths?.Any() != true)
                throw new ArgumentException("params 'paths' can not null.");
            paths = paths.Where(p => !string.IsNullOrWhiteSpace(p)).ToArray();
            if (paths?.Any() != true)
                throw new ArgumentException("params 'paths' can not null.");

            string[] prepareCombinePaths = new string[paths.Length + 1];
            for (int i = 0; i < paths.Length; i++)
            {
                prepareCombinePaths[i + 1] = paths[i];
            }
            if (IsDeployByIIS())
            {
                path = PathCombine(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin"), prepareCombinePaths);
                if (!string.IsNullOrWhiteSpace(path))
                {
                    return true;
                }
            }
            path = PathCombine(AppDomain.CurrentDomain.BaseDirectory, prepareCombinePaths);
            if (!string.IsNullOrWhiteSpace(path))
            {
                return true;
            }
            path = PathCombine(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin"), prepareCombinePaths);
            if (!string.IsNullOrWhiteSpace(path))
            {
                return true;
            }
            path = PathCombine(Path.GetDirectoryName(Assembly.GetAssembly(typeof(SeetaFace6Native)).Location), prepareCombinePaths);
            if (!string.IsNullOrWhiteSpace(path))
            {
                return true;
            }
            return false;
        }

        private string PathCombine(string basePath, string[] paths)
        {
            if (paths == null || paths.Length < 1)
            {
                return null;
            }
            paths[0] = basePath;
            string outPath = Path.Combine(paths) + Path.DirectorySeparatorChar;
            if (Directory.Exists(outPath))
            {
                return outPath;
            }
            return null;
        }

        private bool IsDeployByIIS()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                //windiws环境下，如果当前进程名称包含iis或者w3wp，优先返回
                string processName = Process.GetCurrentProcess().ProcessName;
                if (!string.IsNullOrEmpty(processName)
                    && (processName.IndexOf("iis", StringComparison.OrdinalIgnoreCase) >= 0
                    || processName.IndexOf("w3wp", StringComparison.OrdinalIgnoreCase) >= 0))
                {
                    return true;
                }
            }
            return false;
        }


        #endregion
    }
}
