using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using System;

namespace SeetaFace6Sharp.Extension.DependencyInjection
{
    public static class SeetaFace6SharpHostExtensions
    {
        public static IServiceCollection AddSeetaFace6Sharp(this IServiceCollection services, Action<SeetaFace6SharpOptions> option = null)
        {
            try
            {
                //当option参数为空是，初始化默认参数
                option ??= (o) => { };
                services.Configure(SeetaFace6SharpOptions.OptionName, option);
                var options = GetOptions(services);
                if (options == null)
                {
                    throw new Exception("Can not get view face core options.");
                }

                //添加默认的能力
                //人脸检测
                services.TryAddSingleton(new FaceDetector(options.FaceDetectConfig));
                //人脸标记
                services.TryAddSingleton(new FaceLandmarker(options.FaceLandmarkConfig));
                //人脸识别
                services.TryAddSingleton(new FaceRecognizer(options.FaceRecognizeConfig));

                //活体检测
                if (options.IsEnableAll || options.IsEnableFaceAntiSpoofing)
                {
                    services.TryAddSingleton(new FaceAntiSpoofing(options.FaceAntiSpoofingConfig));
                }
                //年龄与猜测
                if (options.IsEnableAll || options.IsEnableAgePredict)
                {
                    services.TryAddSingleton(new AgePredictor(options.AgePredictConfig));
                }
                //眼睛状态检测
                if (options.IsEnableAll || options.IsEnableEyeStateDetect)
                {
                    services.TryAddSingleton(new EyeStateDetector(options.EyeStateDetectConfig));
                }
                //性别预测
                if (options.IsEnableAll || options.IsEnableGenderPredict)
                {
                    services.TryAddSingleton(new GenderPredictor(options.GenderPredictConfig));
                }
                //人脸追踪
                if (options.IsEnableAll || options.IsEnableFaceTrack)
                {
                    if (options.FaceTrackerConfig == null)
                    {
                        throw new ArgumentNullException(nameof(options.FaceTrackerConfig), "FaceTrackerConfig can not null, when enable face tracker.");
                    }
                    services.TryAddSingleton(new FaceTracker(options.FaceTrackerConfig));
                }
                //口罩识别
                if (options.IsEnableAll || options.IsEnablMaskDetect)
                {
                    services.TryAddSingleton(new MaskDetector(options.MaskDetectConfig));
                }
                //质量检测
                if (options.IsEnableAll || options.IsEnableQuality)
                {
                    services.TryAddSingleton(new FaceQuality(options.QualityConfig));
                }
                return services;
            }
            finally
            {
                services.TryAddSingleton<ISeetaFace6SharpFactory>(new SeetaFace6SharpFactory(services));
            }
        }

        private static SeetaFace6SharpOptions GetOptions(IServiceCollection services)
        {
            using (var provider = services.BuildServiceProvider())
            {
                var options = provider.GetRequiredService<IOptionsMonitor<SeetaFace6SharpOptions>>();
                return options.Get(SeetaFace6SharpOptions.OptionName);
            }
        }
    }
}
