using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;

namespace SeetaFace6Sharp.Extension.DependencyInjection
{
    /// <summary>
    /// 获取SeetaFace对象工厂
    /// </summary>
    public class SeetaFace6SharpFactory : ISeetaFace6SharpFactory, IDisposable
    {
        private readonly ServiceProvider _provider;
        private readonly SeetaFace6SharpOptions _options;
        private readonly Dictionary<Type, bool> _capabilityStatus;

        public SeetaFace6SharpFactory(IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }
            _provider = services.BuildServiceProvider();
            //获取Options
            var options = _provider.GetRequiredService<IOptionsMonitor<SeetaFace6SharpOptions>>();
            _options = options.Get(SeetaFace6SharpOptions.OptionName) ?? throw new ArgumentNullException("Can not load SeetaFace6Sharp options.");
            //构建基础能力
            _capabilityStatus = BuildCapabilityStatus();
        }

        /// <summary>
        /// 获取SeetaFace6对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        public T Get<T>() where T : ISeetaFace6
        {
            if (_capabilityStatus.TryGetValue(typeof(T), out bool status) && !status)
            {
                throw new NotSupportedException($"{typeof(T).Name} capability is not enabled, please enable it at first.");
            }
            return (T)_provider.GetService(typeof(T));
        }

        private Dictionary<Type, bool> BuildCapabilityStatus()
        {
            Dictionary<Type, bool> result = new Dictionary<Type, bool>()
            {
                //基础能力
                { typeof(FaceDetector),true},
                { typeof(FaceLandmarker),true},
                { typeof(FaceRecognizer),true},

                //非基础能力
                { typeof(FaceAntiSpoofing),(_options.IsEnableFaceAntiSpoofing || _options.IsEnableAll)},
                { typeof(AgePredictor),(_options.IsEnableAgePredict || _options.IsEnableAll)},
                { typeof(EyeStateDetector),(_options.IsEnableEyeStateDetect || _options.IsEnableAll)},
                { typeof(GenderPredictor),(_options.IsEnableGenderPredict || _options.IsEnableAll)},
                { typeof(FaceTracker),(_options.IsEnableFaceTrack || _options.IsEnableAll)},
                { typeof(MaskDetector),(_options.IsEnablMaskDetect || _options.IsEnableAll)},
                { typeof(FaceQuality),(_options.IsEnableQuality || _options.IsEnableAll)},
            };
            return result;
        }

        /// <summary>
        /// 释放工厂
        /// </summary>
        public void Dispose()
        {
            if (_provider != null)
            {
                _provider.Dispose();
            }
        }
    }
}
