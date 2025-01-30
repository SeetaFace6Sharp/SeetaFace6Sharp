@echo off

set configuration=Release
set mainVersion=1.0.9
set output=%~dp0publish_cpu

call copy_cpu_seetaface6binary.bat 1

rd /s /q %output%
cd ..

echo 开始打包SeetaFace6Sharp...

echo ====================== pack SeetaFace6Sharp ======================
cd .\SeetaFace6Sharp
rd /s /q bin obj
call pack.bat %configuration% %mainVersion% %output%
cd ..

echo 开始打包Runtimes...

echo ====================== pack SeetaFace6Sharp.runtime.win.x64 ======================
cd .\Runtimes\SeetaFace6Sharp.runtime.win.x64
rd /s /q bin obj
call pack.bat %configuration% %mainVersion% %output%
cd ..\..

echo ====================== pack SeetaFace6Sharp.runtime.win.x86 ======================
cd .\Runtimes\SeetaFace6Sharp.runtime.win.x86
rd /s /q bin obj
call pack.bat %configuration% %mainVersion% %output%
cd ..\..

echo ====================== pack SeetaFace6Sharp.runtime.linux.debian.x64 ======================
cd .\Runtimes\SeetaFace6Sharp.runtime.linux.debian.x64
rd /s /q bin obj
call pack.bat %configuration% %mainVersion% %output%
cd ..\..

echo ====================== pack SeetaFace6Sharp.runtime.linux.arm ======================
cd .\Runtimes\SeetaFace6Sharp.runtime.linux.arm
rd /s /q bin obj
call pack.bat %configuration% %mainVersion% %output%
cd ..\..

echo ====================== pack SeetaFace6Sharp.runtime.linux.arm64 ======================
cd .\Runtimes\SeetaFace6Sharp.runtime.linux.arm64
rd /s /q bin obj
call pack.bat %configuration% %mainVersion% %output%
cd ..\..

echo ====================== pack SeetaFace6Sharp.runtime.linux.loongarch64-abi1 ======================
cd .\Runtimes\SeetaFace6Sharp.runtime.linux.loongarch64-abi1
rd /s /q bin obj
call pack.bat %configuration% %mainVersion% %output%
cd ..\..

echo 开始打包Extensions...

echo ====================== pack SeetaFace6Sharp.Extension.DependencyInjection ======================
cd .\Extensions\SeetaFace6Sharp.Extension.DependencyInjection
rd /s /q bin obj
call pack.bat %configuration% %mainVersion% %output%
cd ..\..

echo ====================== pack SeetaFace6Sharp.Extension.ImageSharp ======================
cd .\Extensions\SeetaFace6Sharp.Extension.ImageSharp
rd /s /q bin obj
call pack.bat %configuration% %mainVersion% %output%
cd ..\..

echo ====================== pack SeetaFace6Sharp.Extension.SkiaSharp ======================
cd .\Extensions\SeetaFace6Sharp.Extension.SkiaSharp
rd /s /q bin obj
call pack.bat %configuration% %mainVersion% %output%
cd ..\..

echo ====================== pack SeetaFace6Sharp.Extension.SystemDrawing ======================
cd .\Extensions\SeetaFace6Sharp.Extension.SystemDrawing
rd /s /q bin obj
call pack.bat %configuration% %mainVersion% %output%
cd ..\..

echo 发布完成，请按任意键退出...
pause > nul
exit 0