#include "bridge.h"
#include "common/str.h"
#include "common/common.h"

using namespace std;
using namespace seeta;

#pragma region Common

#if WINDOWS

// 设置人脸模型目录
EXPORTAPI void SetModelPath(const wchar_t* path)
{
	wstring wstrModelPath(path);
	modelPath = str::wstr_to_str(wstrModelPath);
}

// 获取人脸模型目录
EXPORTAPI void GetModelPath(wchar_t* outPath, int* size)
{
	wstring path = str::str_to_wstr(modelPath);
	*size = path.length();
	if (*size > 1024)
	{
		return;
	}
	wcscpy(outPath, path.c_str());
}

#elif LINUX

// 设置人脸模型目录
EXPORTAPI void SetModelPath(const char* path)
{
	modelPath = path;
}

// 获取人脸模型目录
EXPORTAPI void GetModelPath(char* outPath, int* size)
{
	*size = modelPath.length();
	if (*size > 1024)
	{
		return;
	}
	strcpy(outPath, modelPath.c_str());
}

#endif

// 释放由 malloc 分配的内存
EXPORTAPI void FreeMemory(void* address)
{
	try
	{
		free(address);
	}
	catch (int e)
	{
	}
}

EXPORTAPI seeta::ModelSetting* GetModel(const char* model, const char* extModel, const SeetaDevice deviceType = SEETA_DEVICE_AUTO) {
	ModelSetting* newModel = new ModelSetting(model, deviceType);
	if (extModel != nullptr && strlen(extModel) > 0) {
		newModel->append(extModel);
	}
	return newModel;
}

EXPORTAPI void DisposeModel(seeta::ModelSetting* model) {
	_dispose(model);
}


#pragma endregion

#pragma region FaceDetector

/// <summary>
/// 创建人脸识别句柄
/// </summary>
/// <param name="faceSize"></param>
/// <param name="threshold"></param>
/// <param name="maxWidth"></param>
/// <param name="maxHeight"></param>
/// <returns></returns>
EXPORTAPI seeta::v6::FaceDetector* GetFaceDetectorHandler(const ModelSetting &model, const double faceSize = 20, const double threshold = 0.9, const double maxWidth = 2000, const double maxHeight = 2000)
{
	seeta::v6::FaceDetector* faceDetector = new seeta::v6::FaceDetector(model);
	faceDetector->set(FaceDetector::Property::PROPERTY_MIN_FACE_SIZE, faceSize);
	faceDetector->set(FaceDetector::Property::PROPERTY_THRESHOLD, threshold);
	faceDetector->set(FaceDetector::Property::PROPERTY_MAX_IMAGE_WIDTH, maxWidth);
	faceDetector->set(FaceDetector::Property::PROPERTY_MAX_IMAGE_HEIGHT, maxHeight);
	return faceDetector;
}

/// <summary>
/// 获取人脸数量
/// </summary>
/// <param name="faceDetector"></param>
/// <param name="img"></param>
/// <param name="size"></param>
/// <returns></returns>
EXPORTAPI SeetaFaceInfo* FaceDetectV2(seeta::v6::FaceDetector* handler, const SeetaImageData& img, int* size)
{
	if (handler == nullptr)
	{
		return 0;
	}
	*size = 0;
	std::vector<SeetaFaceInfo> detectFaces = handler->detect_v2(img);
	if (!detectFaces.empty())
	{
		*size = detectFaces.size();
		SeetaFaceInfo* resultFaces = (SeetaFaceInfo*)malloc((*size) * sizeof(SeetaFaceInfo));
		if (resultFaces == nullptr)
		{
			return 0;
		}
		memcpy(resultFaces, &detectFaces[0], detectFaces.size() * sizeof(SeetaFaceInfo));
		std::vector<SeetaFaceInfo>().swap(detectFaces);
		return resultFaces;
	}
	return 0;
}

/// <summary>
/// 释放人脸识别句柄
/// </summary>
/// <param name="faceDetector"></param>
/// <returns></returns>
EXPORTAPI void DisposeFaceDetector(seeta::v6::FaceDetector* handler)
{
	_dispose(handler);
}

#pragma endregion

#pragma region MaskDetector

/// <summary>
/// 创建人脸识别句柄（口罩识别）
/// </summary>
/// <param name="deviceType"></param>
/// <returns></returns>
EXPORTAPI seeta::v2::MaskDetector* GetMaskDetectorHandler(const ModelSetting& model)
{
	return new seeta::v2::MaskDetector(model);
}

/// <summary>
/// 口罩检测
/// </summary>
/// <param name="handler"></param>
/// <param name="img"></param>
/// <param name="size"></param>
/// <returns></returns>
EXPORTAPI bool MaskDetect(seeta::v2::MaskDetector* handler, const SeetaImageData& img, const SeetaRect faceRect, float* score)
{
	if (handler == nullptr)
	{
		return 0;
	}
	bool result = handler->detect(img, faceRect, score);
	return result;
}

/// <summary>
/// 释放口罩识别句柄
/// </summary>
/// <param name="faceDetector"></param>
/// <returns></returns>
EXPORTAPI void DisposeMaskDetector(seeta::v2::MaskDetector* handler)
{
	_dispose(handler);
}

#pragma endregion

#pragma region FaceMark

/// <summary>
/// 获取人脸关键点句柄
/// </summary>
/// <param name="type"></param>
/// <returns></returns>
EXPORTAPI seeta::v6::FaceLandmarker* GetFaceLandmarkerHandler(const ModelSetting& model, const int type = 0)
{
	return new seeta::v6::FaceLandmarker(model);
}

/// <summary>
/// 人脸关键点器
/// </summary>
/// <param name="img"></param>
/// <param name="faceRect"></param>
/// <param name="size"></param>
/// <param name="type"></param>
/// <returns></returns>
EXPORTAPI SeetaPointF* FaceMark(seeta::v6::FaceLandmarker* handler, const SeetaImageData& img, const SeetaRect faceRect, long* size)
{
	if (handler == nullptr)
	{
		return 0;
	}
	*size = 0;
	std::vector<SeetaPointF> markPoints = handler->mark(img, faceRect);
	if (!markPoints.empty())
	{
		*size = markPoints.size();
		SeetaPointF* resultPoints = (SeetaPointF*)malloc((*size) * sizeof(SeetaPointF));
		if (resultPoints == nullptr)
		{
			return 0;
		}
		memcpy(resultPoints, &markPoints[0], markPoints.size() * sizeof(SeetaPointF));
		std::vector<SeetaPointF>().swap(markPoints);
		return resultPoints;
	}
	return 0;
}

EXPORTAPI void DisposeFaceLandmarker(seeta::v6::FaceLandmarker* handler)
{
	_dispose(handler);
}

#pragma endregion

#pragma region FaceRecognizer

/// <summary>
/// 获取人脸特征值句柄
/// </summary>
/// <param name="type"></param>
/// <returns></returns>
EXPORTAPI seeta::v6::FaceRecognizer* GetFaceRecognizerHandler(const ModelSetting &model)
{
	return new seeta::v6::FaceRecognizer(model);
}

EXPORTAPI int GetExtractFeatureSize(seeta::v6::FaceRecognizer* handler)
{
	if (handler == nullptr)
	{
		return 0;
	}
	return handler->GetExtractFeatureSize();
}

EXPORTAPI void GetExtractFeature(seeta::v6::FaceRecognizer* handler, const SeetaImageData& img, const SeetaPointF* points, int size, float* buffer)
{
	if (handler == nullptr)
	{
		return;
	}
	handler->Extract(img, points, buffer);
}

EXPORTAPI void DisposeFaceRecognizer(seeta::v6::FaceRecognizer* handler)
{
	_dispose(handler);
}

/// <summary>
/// 人脸特征值相似度计算
/// </summary>
/// <param name="lhs"></param>
/// <param name="rhs"></param>
/// <param name="size"></param>
/// <returns></returns>
EXPORTAPI float Compare(const float* lhs, const float* rhs, int size)
{
	float sum = 0;
	for (int i = 0; i < size; ++i)
	{
		sum += *lhs * *rhs;
		++lhs;
		++rhs;
	}
	return sum;
}

#pragma endregion

#pragma region FaceAntiSpoofing

EXPORTAPI seeta::v6::FaceAntiSpoofing* GetFaceAntiSpoofingHandler(const ModelSetting& model, const int videoFrameCount = 10, const float boxThresh = 0.8, const float clarity = 0.3, const float reality = 0.8, const bool global = false)
{
	seeta::v6::FaceAntiSpoofing* faceAntiSpoofing = new seeta::v6::FaceAntiSpoofing(model);
	faceAntiSpoofing->SetVideoFrameCount(videoFrameCount);
	faceAntiSpoofing->SetBoxThresh(boxThresh);
	faceAntiSpoofing->SetThreshold(clarity, reality);
	return faceAntiSpoofing;
}

/// <summary>
/// 活体检测 - 单帧
/// </summary>
/// <param name="img"></param>
/// <param name="faceRect"></param>
/// <param name="points"></param>
/// <param name="global"></param>
/// <returns></returns>
EXPORTAPI int FaceAntiSpoofingPredict(seeta::v6::FaceAntiSpoofing* handler, const SeetaImageData& img, const SeetaRect faceRect, const SeetaPointF* points, float* clarity, float* reality)
{
	if (handler == nullptr)
	{
		return -1;
	}
	FaceAntiSpoofing::Status state = handler->Predict(img, faceRect, points);
	handler->GetPreFrameScore(clarity, reality);
	return state;
}

/// <summary>
/// 活体检测 - 视频
/// </summary>
/// <param name="img"></param>
/// <param name="faceRect"></param>
/// <param name="points"></param>
/// <param name="global"></param>
/// <returns></returns>
EXPORTAPI int FaceAntiSpoofingPredictVideo(seeta::v6::FaceAntiSpoofing* handler, const SeetaImageData& img, const SeetaRect faceRect, const SeetaPointF* points, float* clarity, float* reality)
{
	if (handler == nullptr)
	{
		return -1;
	}
	auto status = handler->PredictVideo(img, faceRect, points);
	handler->GetPreFrameScore(clarity, reality);
	if (status != FaceAntiSpoofing::Status::DETECTING)
	{
		handler->ResetVideo();
	}
	return status;
}

EXPORTAPI void DisposeFaceAntiSpoofing(seeta::v6::FaceAntiSpoofing* handler)
{
	_dispose(handler);
}

#pragma endregion

#pragma region FaceTracker

/// <summary>
/// 获取人脸追踪句柄
/// </summary>
/// <param name="width"></param>
/// <param name="height"></param>
/// <param name="type"></param>
/// <param name="stable"></param>
/// <param name="interval"></param>
/// <param name="faceSize"></param>
/// <param name="threshold"></param>
/// <returns></returns>
EXPORTAPI seeta::v6::FaceTracker* GetFaceTrackerHandler(const ModelSetting& model, const int width, const int height, const bool stable = false, const int interval = 10, const int faceSize = 20, const float threshold = 0.9)
{
	seeta::v6::FaceTracker* faceTracker = new seeta::v6::FaceTracker(model, width, height);
	faceTracker->SetVideoStable(stable);
	faceTracker->SetMinFaceSize(faceSize);
	faceTracker->SetThreshold(threshold);
	faceTracker->SetInterval(interval);
	return faceTracker;
}

/// <summary>
/// 获取跟踪的人脸个数
/// </summary>
/// <param name="faceTracker"></param>
/// <param name="img"></param>
/// <param name="size"></param>
/// <returns></returns>
EXPORTAPI SeetaTrackingFaceInfo* FaceTrack(seeta::v6::FaceTracker* handler, const SeetaImageData& img, int* size)
{
	if (handler == nullptr)
	{
		return 0;
	}
	*size = 0;
	auto cfaces = handler->Track(img);
	std::vector<SeetaTrackingFaceInfo> faceTrackResult(cfaces.data, cfaces.data + cfaces.size);
	if (!faceTrackResult.empty())
	{
		*size = faceTrackResult.size();
		SeetaTrackingFaceInfo* resultFaceInfos = (SeetaTrackingFaceInfo*)malloc((*size) * sizeof(SeetaTrackingFaceInfo));
		if (resultFaceInfos == nullptr)
		{
			return 0;
		}
		memcpy(resultFaceInfos, &faceTrackResult[0], faceTrackResult.size() * sizeof(SeetaTrackingFaceInfo));
		std::vector<SeetaTrackingFaceInfo>().swap(faceTrackResult);
		return resultFaceInfos;
	}
	return 0;
}

/// <summary>
/// 重置追踪
/// </summary>
/// <param name="faceTracker"></param>
/// <returns></returns>
EXPORTAPI void FaceTrackReset(seeta::v6::FaceTracker* handler)
{
	if (handler == nullptr)
	{
		return;
	}
	handler->Reset();
}

/// <summary>
/// 释放追踪对象
/// </summary>
/// <param name="faceTracker"></param>
/// <returns></returns>
EXPORTAPI void DisposeFaceTracker(seeta::v6::FaceTracker* handler)
{
	_dispose(handler);
}

#pragma endregion

#pragma region Quality

// 亮度评估
EXPORTAPI void Quality_Brightness(const SeetaImageData& img, const SeetaRect faceRect, const SeetaPointF* points, const int pointsLength, int* level, float* score, const float v0 = 70, const float v1 = 100, const float v2 = 210, const float v3 = 230)
{
	seeta::v3::QualityOfBrightness* quality_Brightness = new seeta::v3::QualityOfBrightness(v0, v1, v2, v3);
	auto result = quality_Brightness->check(img, faceRect, points, pointsLength);

	*level = result.level;
	*score = result.score;

	delete quality_Brightness;
}

// 清晰度评估
EXPORTAPI void Quality_Clarity(const SeetaImageData& img, const SeetaRect faceRect, const SeetaPointF* points, const int pointsLength, int* level, float* score, const float low = 0.1f, const float high = 0.2f)
{
	seeta::v3::QualityOfClarity* quality_Clarity = new seeta::v3::QualityOfClarity(low, high);
	auto result = quality_Clarity->check(img, faceRect, points, pointsLength);

	*level = result.level;
	*score = result.score;

	delete quality_Clarity;
}

// 完整度评估
EXPORTAPI void Quality_Integrity(const SeetaImageData& img, const SeetaRect faceRect, const SeetaPointF* points, const int pointsLength, int* level, float* score, const float low = 10, const float high = 1.5f)
{
	seeta::v3::QualityOfIntegrity* quality_Integrity = new seeta::v3::QualityOfIntegrity(low, high);
	auto result = quality_Integrity->check(img, faceRect, points, pointsLength);

	*level = result.level;
	*score = result.score;

	delete quality_Integrity;
}

// 姿态评估
EXPORTAPI void Quality_Pose(const SeetaImageData& img, const SeetaRect faceRect, const SeetaPointF* points, const int pointsLength, int* level, float* score)
{
	seeta::v3::QualityOfPose* quality_Pose = new seeta::v3::QualityOfPose();
	auto result = quality_Pose->check(img, faceRect, points, pointsLength);

	*level = result.level;
	*score = result.score;

	delete quality_Pose;
}

// 姿态 (深度)评估
EXPORTAPI void Quality_PoseEx(const ModelSetting& model, const SeetaImageData& img, const SeetaRect faceRect, const SeetaPointF* points, const int pointsLength, int* level, float* score,
	const float yawLow = 25, const float yawHigh = 10, const float pitchLow = 20, const float pitchHigh = 10, const float rollLow = 33.33f, const float rollHigh = 16.67f)
{
	seeta::v3::QualityOfPoseEx quality_PoseEx(model);
	quality_PoseEx.set(QualityOfPoseEx::YAW_LOW_THRESHOLD, yawLow);
	quality_PoseEx.set(QualityOfPoseEx::YAW_HIGH_THRESHOLD, yawHigh);
	quality_PoseEx.set(QualityOfPoseEx::PITCH_LOW_THRESHOLD, pitchLow);
	quality_PoseEx.set(QualityOfPoseEx::PITCH_HIGH_THRESHOLD, pitchHigh);
	quality_PoseEx.set(QualityOfPoseEx::ROLL_LOW_THRESHOLD, rollLow);
	quality_PoseEx.set(QualityOfPoseEx::ROLL_HIGH_THRESHOLD, rollHigh);

	auto result = quality_PoseEx.check(img, faceRect, points, pointsLength);

	*level = result.level;
	*score = result.score;
}

// 分辨率评估
EXPORTAPI void Quality_Resolution(const SeetaImageData& img, const SeetaRect faceRect, const SeetaPointF* points, const int pointsLength, int* level, float* score, const float low = 80, const float high = 120)
{
	seeta::v3::QualityOfResolution quality_Resolution(low, high);
	auto result = quality_Resolution.check(img, faceRect, points, pointsLength);

	*level = result.level;
	*score = result.score;
}

EXPORTAPI seeta::QualityOfClarityEx* GetQualityOfClarityExHandler(const float blur_thresh = 0.8f, const SeetaDevice deviceType = SEETA_DEVICE_AUTO)
{
	return new seeta::QualityOfClarityEx(blur_thresh, modelPath, deviceType);
}

// 清晰度 (深度)评估
EXPORTAPI void Quality_ClarityEx(seeta::QualityOfClarityEx* handler, const SeetaImageData& img, const SeetaRect faceRect, const SeetaPointF* points, const int pointsLength, int* level, float* score)
{
	auto result = handler->check(img, faceRect, points, pointsLength);

	*level = result.level;
	*score = result.score;
}

EXPORTAPI void DisposeQualityOfClarityEx(seeta::QualityOfClarityEx* handler)
{
	_dispose(handler);
}

EXPORTAPI seeta::QualityOfNoMask* GetQualityOfNoMaskHandler(const SeetaDevice deviceType = SEETA_DEVICE_AUTO)
{
	return new seeta::QualityOfNoMask(modelPath, deviceType);
}

// 遮挡评估
EXPORTAPI void Quality_NoMask(seeta::QualityOfNoMask* handler, const SeetaImageData& img, const SeetaRect faceRect, const SeetaPointF* points, const int pointsLength, int* level, float* score)
{
	auto result = handler->check(img, faceRect, points, pointsLength);

	*level = result.level;
	*score = result.score;
}

EXPORTAPI void DisposeQualityOfNoMask(seeta::QualityOfNoMask* handler)
{
	_dispose(handler);
}

#pragma endregion

#pragma region AgePredictor / GenderPredictor / EyeStateDetector

#pragma region AgePredictor

/// <summary>
/// 获取年龄预测句柄
/// </summary>
/// <returns></returns>
EXPORTAPI seeta::v6::AgePredictor* GetAgePredictorHandler(const SeetaDevice deviceType = SEETA_DEVICE_AUTO)
{
	return new seeta::v6::AgePredictor(ModelSetting(modelPath + "age_predictor.csta", deviceType));
}

/// <summary>
/// 年龄预测
/// </summary>
/// <param name="img"></param>
/// <param name="points"></param>
/// <returns></returns>
EXPORTAPI int PredictAge(seeta::v6::AgePredictor* handler, const SeetaImageData& img)
{
	if (handler == nullptr)
	{
		return -1;
	}
	int age = 0;
	bool result = handler->PredictAge(img, age);
	if (result)
	{
		return age;
	}
	else
	{
		return -1;
	}
}

/// <summary>
/// 年龄预测（自动裁剪）
/// </summary>
/// <param name="img"></param>
/// <param name="points"></param>
/// <returns></returns>
EXPORTAPI int PredictAgeWithCrop(seeta::v6::AgePredictor* handler, const SeetaImageData& img, const SeetaPointF* points)
{
	if (handler == nullptr)
	{
		return -1;
	}
	int age = 0;
	bool result = handler->PredictAgeWithCrop(img, points, age);
	if (result)
	{
		return age;
	}
	else
	{
		return -1;
	}
}

/// <summary>
/// 释放年龄预测句柄
/// </summary>
/// <param name="handler"></param>
/// <returns></returns>
EXPORTAPI void DisposeAgePredictor(seeta::v6::AgePredictor* handler)
{
	_dispose(handler);
}

#pragma endregion

#pragma region GenderPredictor

/// <summary>
/// 获取性别预测句柄
/// </summary>
/// <returns></returns>
EXPORTAPI seeta::v6::GenderPredictor* GetGenderPredictorHandler(const SeetaDevice deviceType = SEETA_DEVICE_AUTO)
{
	return new seeta::v6::GenderPredictor(ModelSetting(modelPath + "gender_predictor.csta", deviceType));
}

/// <summary>
/// 性别预测
/// </summary>
/// <param name="img"></param>
/// <param name="points"></param>
/// <returns></returns>
EXPORTAPI int PredictGender(seeta::v6::GenderPredictor* handler, const SeetaImageData& img)
{
	if (handler == nullptr)
	{
		return -1;
	}
	GenderPredictor::GENDER gender = GenderPredictor::GENDER::MALE;
	auto result = handler->PredictGender(img, gender);
	if (result)
	{
		return gender;
	}
	else
	{
		return -1;
	}
}

/// <summary>
/// 性别预测（自动裁剪）
/// </summary>
/// <param name="img"></param>
/// <param name="points"></param>
/// <returns></returns>
EXPORTAPI int PredictGenderWithCrop(seeta::v6::GenderPredictor* handler, const SeetaImageData& img, const SeetaPointF* points)
{
	if (handler == nullptr)
	{
		return -1;
	}
	GenderPredictor::GENDER gender = GenderPredictor::GENDER::MALE;
	auto result = handler->PredictGenderWithCrop(img, points, gender);
	if (result)
	{
		return gender;
	}
	else
	{
		return -1;
	}
}

/// <summary>
/// 释放性别预测句柄
/// </summary>
/// <param name="handler"></param>
/// <returns></returns>
EXPORTAPI void DisposeGenderPredictor(seeta::v6::GenderPredictor* handler)
{
	_dispose(handler);
}

#pragma endregion

#pragma region EyeStateDetector

/// <summary>
/// 获取眼睛状态检测句柄
/// </summary>
/// <returns></returns>
EXPORTAPI seeta::v6::EyeStateDetector* GetEyeStateDetectorHandler(const SeetaDevice deviceType = SEETA_DEVICE_AUTO)
{
	return new seeta::v6::EyeStateDetector(ModelSetting(modelPath + "eye_state.csta", deviceType));
}

/// <summary>
/// 眼睛状态检测
/// </summary>
/// <param name="img"></param>
/// <param name="points"></param>
/// <returns></returns>
EXPORTAPI void EyeStateDetect(seeta::v6::EyeStateDetector* handler, const SeetaImageData& img, const SeetaPointF* points, EyeStateDetector::EYE_STATE& left_eye, EyeStateDetector::EYE_STATE& right_eye)
{
	if (handler == nullptr)
	{
		return;
	}
	handler->Detect(img, points, left_eye, right_eye);
}

/// <summary>
/// 释放眼睛状态检测句柄
/// </summary>
/// <param name="handler"></param>
/// <returns></returns>
EXPORTAPI void DisposeEyeStateDetector(seeta::v6::EyeStateDetector* handler)
{
	_dispose(handler);
}

#pragma endregion

#pragma endregion
