namespace SeetaFace6Sharp
{
    /// <summary>
    /// 预测器
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BasePredictor<T> : BaseSeetaFace6<T> where T : BaseConfig
    {
        /// <summary>
        /// 初始化预测器构造器
        /// </summary>
        /// <param name="config"></param>
        public BasePredictor(T config) : base(config) { }
    }
}