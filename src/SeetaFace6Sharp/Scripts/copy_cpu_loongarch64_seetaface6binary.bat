@echo off

set source=CPU-LoongArch64-ABI1.0
set sourcePath=%~dp0..\..\..\..\SeetaFace6OpenBinary\%source%\
set destPath=%~dp0..\..\

if not exist %sourcePath% (
   echo SeetaFace6OpenBinaryԴ�ļ������ڣ�������SeetaFace6OpenBinary��������SeetaFace6Sharp��Ŀͬһ����Ŀ¼!!!
   pause > nul
   exit 0
)

echo %source%�汾�滻��...���Ժ�...

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
   echo �ѽ�SeetaFace6OpenBinary�滻ΪLoongArch64-ABI1.0�汾���밴������˳�...
   pause > nul
)