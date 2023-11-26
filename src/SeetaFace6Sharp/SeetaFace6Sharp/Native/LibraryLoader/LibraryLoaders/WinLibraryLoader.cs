using System;
using SeetaFace6Sharp.Native.LibraryLoader.Interface;
using SeetaFace6Sharp.Native.LibraryLoader.LibraryLoaders.Platforms;
using System.IO;
using System.Runtime.InteropServices;
using System.Reflection;

namespace SeetaFace6Sharp.Native.LibraryLoader.LibraryLoaders
{
    internal sealed class WinLibraryLoader : BaseLibraryLoader
    {
        protected override void LoadLibrary()
        {
            GlobalConfig.WriteLog($"Loading library from {PathResolver.GetLibraryPath()}");
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
                    LoadSeetaFace6Bridge(libraryPath);
                    continue;
                }
                else
                {
#if NETCOREAPP3_1_OR_GREATER
                    IntPtr ptr = NativeLibrary.Load(libraryPath);
                    if (ptr == IntPtr.Zero)
                    {
                        throw new BadImageFormatException($"Can not load native library {libraryPath}.");
                    }
#else
                    IntPtr ptr = Kernel32.LoadLibrary(libraryPath);
                    if (ptr == IntPtr.Zero)
                    {
                        throw new BadImageFormatException($"Can not load native library {libraryPath}.");
                    }
#endif
                    Ptrs.Add(ptr);
                }
            }
        }

        private void LoadSeetaFace6Bridge(string libraryPath)
        {
#if NETCOREAPP3_1_OR_GREATER

            NativeLibrary.SetDllImportResolver(Assembly.GetAssembly(typeof(SeetaFace6Native)), (libraryName, assembly, searchPath) =>
            {
                IntPtr ptr = NativeLibrary.Load(libraryPath, assembly, searchPath ?? DllImportSearchPath.AssemblyDirectory);
                return ptr;
            });
#else
            IntPtr ptr = Kernel32.LoadLibrary(libraryPath);
            if (ptr == IntPtr.Zero)
            {
                throw new BadImageFormatException($"Can not load native library {libraryPath}.");
            }
#endif
        }
    }
}