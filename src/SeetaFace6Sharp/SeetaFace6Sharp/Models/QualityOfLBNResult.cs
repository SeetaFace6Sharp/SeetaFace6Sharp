using System;
using System.Collections.Generic;
using System.Text;

namespace SeetaFace6Sharp.Models
{
    /// <summary>
    /// 深度学习的人脸清晰度评估结果
    /// </summary>
    public class QualityOfLBNResult
    {
        /// <inheritdoc/>
        internal QualityOfLBNResult(int light, int blur, int noise)
        {
            this.Light = light;
            this.Blur = blur;
            this.Noise = noise;
        }

        /// <summary>
        /// 亮度返回结果，暂不推荐使用该返回结果
        /// </summary>
        public int Light { get; private set; }

        /// <summary>
        /// 模糊度返回结果
        /// blur 结果返回 0 说明人脸是清晰的，blur 为 1 说明人脸是模糊的。
        /// </summary>
        public int Blur { get; private set; }

        /// <summary>
        /// 是否有噪声返回结果，暂不推荐使用该返回结果
        /// </summary>
        public int Noise { get; private set; }

        /// <summary>
        /// 质量
        /// </summary>
        public QualityResult QualityResult
        {
            get
            {
                if (this.Blur == 1) return new QualityResult(QualityLevel.Low, 0, QualityType.ClarityEx);
                else return new QualityResult(QualityLevel.High, 1, QualityType.ClarityEx);
            }
        }
    }
}
