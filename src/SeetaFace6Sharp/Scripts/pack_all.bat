@echo off

set configuration=Release
set mainVersion=1.0.9
set modelVersion=1.0.0
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

echo 开始打包Models...

echo ====================== pack SeetaFace6Sharp.model.age_predictor ======================
cd .\Models\SeetaFace6Sharp.model.age_predictor
rd /s /q bin obj
call pack.bat %configuration% %modelVersion% %output%
cd ..\..

echo ====================== pack SeetaFace6Sharp.model.eye_state ======================
cd .\Models\SeetaFace6Sharp.model.eye_state
rd /s /q bin obj
call pack.bat %configuration% %modelVersion% %output%
cd ..\..

echo ====================== pack SeetaFace6Sharp.model.face_detector ======================
cd .\Models\SeetaFace6Sharp.model.face_detector
rd /s /q bin obj
call pack.bat %configuration% %modelVersion% %output%
cd ..\..

echo ====================== pack SeetaFace6Sharp.model.face_landmarker_mask_pts5 ======================
cd .\Models\SeetaFace6Sharp.model.face_landmarker_mask_pts5
rd /s /q bin obj
call pack.bat %configuration% %modelVersion% %output%
cd ..\..

echo ====================== pack SeetaFace6Sharp.model.face_landmarker_pts5 ======================
cd .\Models\SeetaFace6Sharp.model.face_landmarker_pts5
rd /s /q bin obj
call pack.bat %configuration% %modelVersion% %output%
cd ..\..

echo ====================== pack SeetaFace6Sharp.model.face_landmarker_pts68 ======================
cd .\Models\SeetaFace6Sharp.model.face_landmarker_pts68
rd /s /q bin obj
call pack.bat %configuration% %modelVersion% %output%
cd ..\..

echo ====================== pack SeetaFace6Sharp.model.face_recognizer ======================
cd .\Models\SeetaFace6Sharp.model.face_recognizer
rd /s /q bin obj
call pack.bat %configuration% %modelVersion% %output%
cd ..\..

echo ====================== pack SeetaFace6Sharp.model.face_recognizer_light ======================
cd .\Models\SeetaFace6Sharp.model.face_recognizer_light
rd /s /q bin obj
call pack.bat %configuration% %modelVersion% %output%
cd ..\..

echo ====================== pack SeetaFace6Sharp.model.face_recognizer_mask ======================
cd .\Models\SeetaFace6Sharp.model.face_recognizer_mask
rd /s /q bin obj
call pack.bat %configuration% %modelVersion% %output%
cd ..\..

echo ====================== pack SeetaFace6Sharp.model.fas_first ======================
cd .\Models\SeetaFace6Sharp.model.fas_first
rd /s /q bin obj
call pack.bat %configuration% %modelVersion% %output%
cd ..\..

echo ====================== pack SeetaFace6Sharp.model.fas_second ======================
cd .\Models\SeetaFace6Sharp.model.fas_second
rd /s /q bin obj
call pack.bat %configuration% %modelVersion% %output%
cd ..\..

echo ====================== pack SeetaFace6Sharp.model.gender_predictor ======================
cd .\Models\SeetaFace6Sharp.model.gender_predictor
rd /s /q bin obj
call pack.bat %configuration% %modelVersion% %output%
cd ..\..

echo ====================== pack SeetaFace6Sharp.model.mask_detector ======================
cd .\Models\SeetaFace6Sharp.model.mask_detector
rd /s /q bin obj
call pack.bat %configuration% %modelVersion% %output%
cd ..\..

echo ====================== pack SeetaFace6Sharp.model.pose_estimation ======================
cd .\Models\SeetaFace6Sharp.model.pose_estimation
rd /s /q bin obj
call pack.bat %configuration% %modelVersion% %output%
cd ..\..

echo ====================== pack SeetaFace6Sharp.model.quality_lbn ======================
cd .\Models\SeetaFace6Sharp.model.quality_lbn
rd /s /q bin obj
call pack.bat %configuration% %modelVersion% %output%
cd ..\..

echo ====================== pack SeetaFace6Sharp.model.all ======================
cd .\Models\SeetaFace6Sharp.model.all
rd /s /q bin obj
call pack.bat %configuration% %modelVersion% %output%
cd ..\..

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