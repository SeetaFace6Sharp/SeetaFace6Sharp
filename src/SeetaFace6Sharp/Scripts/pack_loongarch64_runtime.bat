@echo off

set configuration=Release
set mainVersion=1.0.9
set output=%~dp0publish_cpu_loongarch64_abi1

call copy_cpu_loongarch64_seetaface6binary.bat 1

rd /s /q %output%
cd ..

echo 开始打包Runtimes...

echo ====================== pack SeetaFace6Sharp.runtime.linux.loongarch64-abi1 ======================
cd .\Runtimes\SeetaFace6Sharp.runtime.linux.loongarch64-abi1
rd /s /q bin obj
call pack.bat %configuration% %mainVersion% %output%
cd ..\..

echo 发布完成，请按任意键退出...
pause > nul
exit 0