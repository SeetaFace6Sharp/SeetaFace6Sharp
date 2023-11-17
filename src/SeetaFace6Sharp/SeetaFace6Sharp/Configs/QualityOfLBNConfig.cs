using System;

namespace SeetaFace6Sharp
{
    /// <summary>
    /// 质量检测（深度）配置
    /// </summary>
    public class QualityOfLBNConfig : BaseConfig
    {
        /// <inheritdoc/>
        public QualityOfLBNConfig()
        {
        }

        /// <inheritdoc/>
        public QualityOfLBNConfig(float blurThresh)
        {
            if (blurThresh <= 0) throw new ArgumentOutOfRangeException(nameof(blurThresh), "Blur thresh can not less than 0.");
            this.BlurThresh = blurThresh;
        }

        /// <summary>
        /// 评估对应分值超过选项之后就认为是模糊图片
        /// </summary>
        public float BlurThresh { get; set; } = 0.8f;
    }
}