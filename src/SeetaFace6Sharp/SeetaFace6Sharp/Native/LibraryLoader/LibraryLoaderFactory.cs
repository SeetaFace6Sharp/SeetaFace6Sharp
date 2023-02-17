using SeetaFace6Sharp.Native.LibraryLoader.Interface;
using SeetaFace6Sharp.Native.LibraryLoader.LibraryLoaders;
using System;
using System.Runtime.InteropServices;

namespace SeetaFace6Sharp.Native.LibraryLoader
{
    internal class LibraryLoaderFactory
    {
        public static ILibraryLoader Create()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return new WinLibraryLoader();
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return new UnixLibraryLoader();
            }
            else
            {
                throw new PlatformNotSupportedException($"Unsupported operating system platform: {RuntimeInformation.OSDescription}");
            }
        }
    }
}
