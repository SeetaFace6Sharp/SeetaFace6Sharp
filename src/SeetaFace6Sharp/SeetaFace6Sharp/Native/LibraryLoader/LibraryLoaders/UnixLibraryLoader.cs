using SeetaFace6Sharp.Native.LibraryLoader.Interface;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace SeetaFace6Sharp.Native.LibraryLoader.LibraryLoaders
{
    internal sealed class UnixLibraryLoader : BaseLibraryLoader
    {
        protected override void LoadLibrary()
        {
            GlobalConfig.WriteLog($"Loading library from {PathResolver.GetLibraryPath()}");
#if NETCOREAPP3_1_OR_GREATER
            foreach (var library in LibraryNameContainer)
            {
                string libraryPath = PathResolver.GetLibraryFullName(library);
                if (!File.Exists(libraryPath))
                {
                    if (library.IndexOf("tennis_", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        continue;
                    }
                    throw new FileNotFoundException($"Can not found library {libraryPath}.");
                }

                if (library.IndexOf(SeetaFace6Native.BRIDGE_LIBRARY_NAME, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    NativeLibrary.SetDllImportResolver(Assembly.GetAssembly(typeof(SeetaFace6Native)), (libraryName, assembly, searchPath) =>
                    {
                        return NativeLibrary.Load(libraryPath, assembly, searchPath ?? DllImportSearchPath.AssemblyDirectory);
                    });
                    continue;
                }

                IntPtr ptr = NativeLibrary.Load(libraryPath);
                if (ptr == IntPtr.Zero)
                {
                    throw new BadImageFormatException($"Can not load native library {libraryPath}.");
                }
                Ptrs.Add(ptr);
            }
#else
            throw new NotSupportedException("On Linux, only .net core 3.1 and above are supported");
#endif
        }
    }
}