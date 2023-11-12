﻿using SeetaFace6Sharp.Models;
using SeetaFace6Sharp.Native;
using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace SeetaFace6Sharp
{
    /// <summary>
    /// 提取和对比人脸特征值。
    /// </summary>
    public sealed class FaceRecognizer : BaseSeetaFace6<FaceRecognizeConfig>
    {
        private readonly IntPtr _handle = IntPtr.Zero;
        private readonly static object _locker = new object();

        /// <summary>
        /// 所需模型(face_recognizer/face_recognizer_mask/face_recognizer_light)
        /// </summary>
        public override Model Model { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="config">配置</param>
        /// <para>
        /// 当 <see cref="FaceType"/> <see langword="="/> <see cref="FaceType.Normal"/> 时， 需要模型：<a href="https://www.nuget.org/packages/SeetaFace6Sharp.model.face_recognizer">face_recognizer.csta</a><br/>
        /// 当 <see cref="FaceType"/> <see langword="="/> <see cref="FaceType.Mask"/> 时， 需要模型：<a href="https://www.nuget.org/packages/SeetaFace6Sharp.model.face_recognizer_mask">face_recognizer_mask.csta</a><br/>
        /// 当 <see cref="FaceType"/> <see langword="="/> <see cref="FaceType.Light"/> 时， 需要模型：<a href="https://www.nuget.org/packages/SeetaFace6Sharp.model.face_recognizer_light">face_recognizer_light.csta</a><br/>
        /// </para>
        /// <exception cref="ModuleInitializeException"></exception>
        public FaceRecognizer(FaceRecognizeConfig config = null) : base(config ?? new FaceRecognizeConfig())
        {
            string model = this.Config.FaceType switch
            {
                FaceType.Normal => "face_recognizer.csta",
                FaceType.Mask => "face_recognizer_mask.csta",
                FaceType.Light => "face_recognizer_light.csta",
                _ => throw new NotSupportedException($"Not support face type: {this.Config.FaceType}."),
            };
            this.Model = new Model(model, this.Config.DeviceType);

            if ((_handle = SeetaFace6Native.GetFaceRecognizerHandler(this.Model.Ptr)) == IntPtr.Zero)
            {
                throw new ModuleInitializeException(nameof(FaceRecognizer), "Get face recognizer handle failed.");
            }
        }

        /// <summary>
        /// 提取人脸特征值。
        /// </summary>
        /// <param name="image">人脸图像信息</param>
        /// <param name="points">人脸关键点数据</param>
        /// <returns></returns>
        public float[] Extract(FaceImage image, FaceMarkPoint[] points)
        {
            lock (_locker)
            {
                if (disposedValue)
                    throw new ObjectDisposedException(nameof(FaceRecognizer));

                int size = SeetaFace6Native.GetExtractFeatureSize(_handle);
                if (size <= 0)
                    throw new Exception("Can not get face recognizer extract size.");

                IntPtr buffer = Marshal.AllocHGlobal(size * sizeof(float));
                SeetaFace6Native.GetExtractFeature(_handle, ref image, points, size, buffer);
                try
                {
                    float[] result = new float[size];
                    Marshal.Copy(buffer, result, 0, size);
                    return result;
                }
                finally
                {
                    if (buffer != IntPtr.Zero)
                        Marshal.FreeHGlobal(buffer);
                }
            }
        }

        #region 特征值
        /// <summary>
        /// 计算特征值相似度。
        /// </summary>
        /// <param name="lfs"></param>
        /// <param name="rfs"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public float Compare(float[] lfs, float[] rfs)
        {
            if (lfs == null || !lfs.Any() || rfs == null || !rfs.Any())
            {
                throw new ArgumentNullException(nameof(lfs), "参数不能为空");
            }
            if (lfs.Length != rfs.Length)
            {
                throw new ArgumentException("两个人脸特征值数组长度不一致，请使用同一检测模型");
            }
            float sum = 0;
            for (int i = 0; i < lfs.Length; i++)
            {
                sum += lfs[i] * rfs[i];
            }
            return sum;

            //调用Native组件
            //return SeetaFace6SharpNative.Compare(_lfs, _rfs, _lfs.Length);
        }

        /// <summary>
        /// 判断相似度是否为同一个人。
        /// </summary>
        /// <param name="similarity">相似度</param>
        /// <returns></returns>
        public bool IsSelf(float similarity) => similarity > this.Config.Threshold;

        /// <summary>
        /// 判断两个特征值是否为同一个人。
        /// <para>只能对比相同 <see cref="FaceType"/> 提取出的特征值</para>
        /// </summary>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="ArgumentNullException"/>
        /// <param name="lfs"></param>
        /// <param name="rfs"></param>
        /// <returns></returns>
        public bool IsSelf(float[] lfs, float[] rfs) => IsSelf(Compare(lfs, rfs));

        #endregion

        /// <inheritdoc/>
        public override void Dispose()
        {
            if (disposedValue)
                return;

            lock (_locker)
            {
                if (disposedValue) return;
                disposedValue = true;
                if (_handle != IntPtr.Zero) SeetaFace6Native.DisposeFaceRecognizer(_handle);
                this.Model?.Dispose();
            }
        }
    }
}