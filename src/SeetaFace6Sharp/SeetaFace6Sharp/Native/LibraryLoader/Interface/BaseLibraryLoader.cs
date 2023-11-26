using SeetaFace6Sharp.Models;
using SeetaFace6Sharp.Native.PathResolvers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

#if NETCOREAPP3_1_OR_GREATER
using System.Runtime.Intrinsics.X86;
#endif

namespace SeetaFace6Sharp.Native.LibraryLoader.Interface
{
    internal abstract class BaseLibraryLoader : ILibraryLoader
    {
        protected readonly List<IntPtr> Ptrs = new List<IntPtr>();

        public BaseLibraryLoader()
        {
            //Set default path resolver
            SetPathResolver(GlobalConfig.GetPathResolver());
            //Write log
            GlobalConfig.WriteLog($"SeetaFace6Sharp runtime environment \t\n" +
                $"OS Description: {RuntimeInformation.OSDescription}\t\n" +
                $"Process Architecture: {RuntimeInformation.ProcessArchitecture}\t\n" +
                $"Framework Description: {RuntimeInformation.FrameworkDescription}\t\n" +
                $"Models Path: {PathResolver.GetModelsPath()}\t\n" +
                $"Library Path: {PathResolver.GetLibraryPath()}\t");
        }

        /// <summary>
        /// SeetaFace6Bridge 的所有依赖库。(按照依赖顺序排列)
        /// </summary>
        protected readonly HashSet<string> LibraryNameContainer = new HashSet<string>()
        {
            "tennis",
            "tennis_haswell",
            "tennis_pentium",
            "tennis_sandy_bridge",
            "SeetaAuthorize",
            "SeetaMaskDetector200",
            "SeetaAgePredictor600",
            "SeetaEyeStateDetector200",
            "SeetaFaceAntiSpoofingX600",
            "SeetaFaceDetector600",
            "SeetaFaceLandmarker600",
            "SeetaFaceRecognizer610",
            "SeetaFaceTracking600",
            "SeetaGenderPredictor600",
            "SeetaPoseEstimation600",
            "SeetaQualityAssessor300",
            SeetaFace6Native.BRIDGE_LIBRARY_NAME
        };

        /// <summary>
        /// 默认的路径解析器
        /// </summary>
        protected virtual IPathResolver PathResolver { get; private set; }

        /// <summary>
        /// 加载静态库之前需要做的事
        /// </summary>
        protected virtual void Before()
        {
            SetInstructionSupport();
        }

        /// <summary>
        /// 加载静态库之后需要做的事
        /// </summary>
        protected virtual void After()
        {

        }

        /// <summary>
        /// 加载核心逻辑
        /// </summary>
        public virtual void Load()
        {
            Before();
            LoadLibrary();
            After();
        }

        /// <summary>
        /// 获取静态库加载路径
        /// </summary>
        /// <returns></returns>
        public virtual string GetLibraryPath()
        {
            return PathResolver.GetLibraryPath();
        }

        /// <summary>
        /// 获取模型加载路径
        /// </summary>
        /// <returns></returns>
        public virtual string GetModelsPath()
        {
            return PathResolver.GetModelsPath();
        }

        /// <summary>
        /// 设置路径解析器
        /// </summary>
        /// <param name="pathResolver"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual void SetPathResolver(IPathResolver pathResolver)
        {
            if (pathResolver == null)
                throw new ArgumentNullException(nameof(pathResolver));

            this.PathResolver = pathResolver;
        }

        #region Abstract

        /// <summary>
        /// 设置支持的指令集
        /// </summary>
        protected virtual void SetInstructionSupport()
        {
            //Arm不需要处理
            if (RuntimeInformation.ProcessArchitecture != Architecture.X86 && RuntimeInformation.ProcessArchitecture != Architecture.X64)
            {
                return;
            }
#if NETCOREAPP3_1_OR_GREATER
            //不支持Fma
            if (!Fma.IsSupported)
            {
                GlobalConfig.X86Instruction = X86Instruction.SSE2;
                if (LibraryNameContainer.Contains("tennis_sandy_bridge"))
                {
                    GlobalConfig.WriteLog("Detected that the CPU instruction does not support FMA, disable tennis_sandy_bridge.");
                    LibraryNameContainer.Remove("tennis_sandy_bridge");
                }
            };
            //不支持Avx2
            if (!Avx2.IsSupported)
            {
                GlobalConfig.X86Instruction = X86Instruction.FMA | X86Instruction.SSE2;
                if (LibraryNameContainer.Contains("tennis_haswell"))
                {
                    GlobalConfig.WriteLog("Detected that the CPU instruction does not support AVX2, disable tennis_haswell.");
                    LibraryNameContainer.Remove("tennis_haswell");
                }
                if (LibraryNameContainer.Contains("tennis_sandy_bridge"))
                {
                    GlobalConfig.WriteLog("Detected that the CPU instruction does not support AVX2, disable tennis_sandy_bridge.");
                    LibraryNameContainer.Remove("tennis_sandy_bridge");
                }
            };
            if (!Sse2.IsSupported)
            {
                GlobalConfig.WriteLog("Detected that the CPU instruction does not support SSE2.");
                throw new NotSupportedException("Your CPU model is not supported.");
            }
#endif
            TargetInstructionInfo targetInstruction = null;
            List<string> removeLibs = new List<string>();

            switch (GlobalConfig.X86Instruction)
            {
                case X86Instruction.AVX2 | X86Instruction.SSE2:
                case X86Instruction.AVX2:
                    {
                        //只支持AVX2
                        GlobalConfig.WriteLog("CPU only support AVX2 instruction, will use tennis_sandy_bridge.");
                        removeLibs.Add("tennis_haswell");
                        removeLibs.Add("tennis_pentium");
                        targetInstruction = new TargetInstructionInfo("tennis_haswell", GlobalConfig.X86Instruction);
                    }
                    break;
                case X86Instruction.FMA | X86Instruction.SSE2:
                case X86Instruction.SSE2:
                    {
                        //只支持SSE2
                        GlobalConfig.WriteLog("CPU only support SSE2 instruction, will use tennis_pentium.");
                        removeLibs.Add("tennis_haswell");
                        removeLibs.Add("tennis_sandy_bridge");
                        targetInstruction = new TargetInstructionInfo("tennis_pentium", GlobalConfig.X86Instruction);
                    }
                    break;
                default:
                    {
                        string keyFile = PathResolver.GetLibraryFullName("tennis_using");
                        if (File.Exists(keyFile))
                        {
                            targetInstruction = new TargetInstructionInfo("tennis_haswell", GlobalConfig.X86Instruction);
                        }
                    }
                    break;
            }

            if (removeLibs?.Any() == true)
            {
                foreach (var item in removeLibs)
                {
                    if (LibraryNameContainer.Contains(item))
                    {
                        LibraryNameContainer.Remove(item);
                    }
                }
            }

            if (targetInstruction != null)
            {
                string keyFile = PathResolver.GetLibraryFullName("tennis_using");
                if (!File.Exists(keyFile) || File.ReadAllText(keyFile)?.Equals(((int)targetInstruction.TargetX86Instruction).ToString(), StringComparison.OrdinalIgnoreCase) != true)
                {
                    //用对应指令集的tennis替换tennis.xx
                    string baseLibraryPath = PathResolver.GetLibraryFullName("tennis");
                    string targetLibraryPath = PathResolver.GetLibraryFullName(targetInstruction.TargetTennisName);
                    File.Delete(baseLibraryPath);
                    File.Copy(targetLibraryPath, baseLibraryPath);

                    if (targetInstruction.TargetX86Instruction != (X86Instruction.AVX2 | X86Instruction.FMA | X86Instruction.SSE2))
                    {
                        File.WriteAllText(keyFile, ((int)targetInstruction.TargetX86Instruction).ToString());
                    }
                    else
                    {
                        File.Delete(keyFile);
                    }
                }
            }
        }

        /// <summary>
        /// 加载静态库
        /// </summary>
        protected abstract void LoadLibrary();

        /// <summary>
        /// Dispose
        /// </summary>
        public virtual void Dispose()
        {
            if (Ptrs?.Any() != true)
            {
                return;
            }
            foreach (var item in Ptrs)
            {
                try
                {
#if NETCOREAPP3_1_OR_GREATER
                    NativeLibrary.Free(item);
#endif
                }
                catch { }
            }
        }

        #endregion
    }
}
