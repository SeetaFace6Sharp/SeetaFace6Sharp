#pragma once

#include "../../../SeetaFace/index/build/include/seeta/FaceDetector.h"
#include "../../../SeetaFace/index/build/include/seeta/FaceLandmarker.h"
#include "../../../SeetaFace/index/build/include/seeta/FaceRecognizer.h"
#include "../../../SeetaFace/index/build/include/seeta/FaceAntiSpoofing.h"
#include "../../../SeetaFace/index/build/include/seeta/FaceTracker.h"
#include "../../../SeetaFace/index/build/include/seeta/MaskDetector.h"

#include "../../../SeetaFace/index/build/include/seeta/QualityOfBrightness.h"
#include "../../../SeetaFace/index/build/include/seeta/QualityOfClarity.h"
#include "../../../SeetaFace/index/build/include/seeta/QualityOfIntegrity.h"
#include "../../../SeetaFace/index/build/include/seeta/QualityOfPose.h"
#include "../../../SeetaFace/index/build/include/seeta/QualityOfPoseEx.h"
#include "../../../SeetaFace/index/build/include/seeta/QualityOfResolution.h"

#include "seetaEx/QualityOfClarityEx.h"
#include "seetaEx/QualityOfNoMask.h"

#include "../../../SeetaFace/index/build/include/seeta/AgePredictor.h"
#include "../../../SeetaFace/index/build/include/seeta/GenderPredictor.h"
#include "../../../SeetaFace/index/build/include/seeta/EyeStateDetector.h"

#include <iostream>
#include <string>
#include <math.h>


#if WINDOWS

#define STDCALL _stdcall
#define EXPORTAPI extern "C" __declspec(dllexport)

#elif LINUX

#define STDCALL __attribute__((stdcall))
#define EXPORTAPI extern "C"

#endif // WINDOWS or LINUX

// ģ������·��
string modelPath = "./runtimes/models/";

#if WINDOWS

// ��������ģ��Ŀ¼
EXPORTAPI void SetModelPath(const wchar_t* path);
// ��ȡ����ģ��Ŀ¼
EXPORTAPI void GetModelPath(wchar_t* outPath, int* size);

#elif LINUX

// ��������ģ��Ŀ¼
EXPORTAPI void SetModelPath(const char* path);

// ��ȡ����ģ��Ŀ¼
EXPORTAPI void GetModelPath(char* outPath, int* size);
#endif

EXPORTAPI void Free(void* address);