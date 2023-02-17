
namespace SeetaFace6Sharp
{
    /// <summary>
    /// x86环境下支持的架构枚举
    /// </summary>
    public enum X86Instruction
    {
        /// <summary>
        /// AVX2
        /// </summary>
        AVX2 = 1 << 0,

        /// <summary>
        /// SSE2
        /// </summary>
        SSE2 = 1 << 1,

        /// <summary>
        /// FMA
        /// </summary>
        FMA = 1 << 2,
    }
}
