@echo off

set configuration=Release
set mainVersion=1.0.10
set output=%~dp0publish_gpu

call copy_gpu_seetaface6binary.bat 1

rd /s /q %output%
cd ..

echo ��ʼ���Runtimes...

echo ====================== pack SeetaFace6Sharp.runtime.win.x64_gpu ======================
cd .\Runtimes\SeetaFace6Sharp.runtime.win.x64_gpu
rd /s /q bin obj
call pack.bat %configuration% %mainVersion% %output%
cd ..\..

echo ������ɣ��밴������˳�...
pause > nul
exit 0