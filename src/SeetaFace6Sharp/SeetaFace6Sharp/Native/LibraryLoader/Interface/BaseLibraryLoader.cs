using SeetaFace6Sharp.Native.PathResolvers;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace SeetaFace6Sharp.Native.LibraryLoader.Interface
{
    internal abstract class BaseLibraryLoader : ILibraryLoader
    {
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
        protected virtual void BeforeLoad()
        {
            SetInstructionSupport();
        }

        /// <summary>
        /// 加载静态库之后需要做的事
        /// </summary>
        protected virtual void AfterLoad()
        {
            string defaultModelsPath = PathResolver.GetModelsPath();
            SetModelsPath(defaultModelsPath);
        }

        /// <summary>
        /// 加载核心逻辑
        /// </summary>
        public virtual void Load()
        {
            BeforeLoad();
            Loading();
            AfterLoad();
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
        /// 设置模型路径
        /// </summary>
        /// <param name="path"></param>
        protected abstract void SetModelsPath(string path);

        /// <summary>
        /// 设置支持的指令集
        /// </summary>
        protected abstract void SetInstructionSupport();

        /// <summary>
        /// 加载静态库
        /// </summary>
        protected abstract void Loading();

        /// <summary>
        /// Dispose
        /// </summary>
        public abstract void Dispose();

        #endregion
    }
}
