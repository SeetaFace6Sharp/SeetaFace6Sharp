@echo off

set source=CPU-LoongArch64-ABI1.0
set sourcePath=%~dp0..\..\..\..\SeetaFace6OpenBinary\%source%\
set destPath=%~dp0..\..\

if not exist %sourcePath% (
   echo SeetaFace6OpenBinary源文件不存在，请下载SeetaFace6OpenBinary并放置于SeetaFace6Sharp项目同一级别目录!!!
   pause > nul
   exit 0
)

echo %source%版本替换中...请稍后...

if exist %~dp0..\Bridges\Linux\SeetaFace6Bridge\bin (
   rd /s /q %~dp0..\Bridges\Linux\SeetaFace6Bridge\bin
)
if exist %~dp0..\Bridges\Windows\SeetaFace6Bridge\bin (
   rd /s /q %~dp0..\Bridges\Windows\SeetaFace6Bridge\bin
)
if exist %~dp0..\..\SeetaFace\index (
   rd /s /q %~dp0..\..\SeetaFace\index
)

xcopy /y /e /i %sourcePath% %destPath% > nul

if "%~1" == "" (
   echo 已将SeetaFace6OpenBinary替换为LoongArch64-ABI1.0版本，请按任意键退出...
   pause > nul
)