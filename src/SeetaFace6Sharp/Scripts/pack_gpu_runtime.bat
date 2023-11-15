@echo off

set configuration=Release
set mainVersion=1.0.2
set output=%~dp0publish

rd /s /q %output%
cd ..

echo 开始打包Runtimes...

echo ====================== pack SeetaFace6Sharp.runtime.win.x64_gpu ======================
cd .\Runtimes\SeetaFace6Sharp.runtime.win.x64_gpu
rd /s /q bin obj
call pack.bat %configuration% %mainVersion% %output%
cd ..\..

echo 发布完成，请按任意键退出...
pause > nul
exit 0