@echo off

:: ���ð���������
set configuration=%1
set version=%2
set output=%3

:: ��ȡ��ǰĿ¼������Ϊ����
for %%a in ("%cd%") do set packageName=%%~nxa

dotnet pack "%packageName%.csproj" ^
    -p:NuspecProperties="version=%version%" ^
    --configuration %configuration% ^
    --output "%output%\%configuration%\%version%"

:: ɾ�����Ŀ¼
rd /q /s "bin" "obj\"