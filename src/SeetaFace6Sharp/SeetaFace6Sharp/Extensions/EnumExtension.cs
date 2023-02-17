using System;
using System.ComponentModel;
using System.Reflection;

namespace SeetaFace6Sharp
{
    /// <summary>
    /// Enum 扩展
    /// </summary>
    internal static class EnumExtension
    {
        /// <summary>
        /// 返回枚举类型的 <see cref="DescriptionAttribute.Description"/>，没有则返回枚举值名称。
        /// </summary>
        /// <param name="enumValue"></param>
        /// <returns></returns>
        public static string GetDescription(this Enum enumValue)
        {
            string value = enumValue.ToString();
            FieldInfo field = enumValue.GetType().GetField(value);
            DescriptionAttribute descriptionAttribute = field.GetCustomAttribute<DescriptionAttribute>(false);
            return descriptionAttribute?.Description ?? value;
        }
    }
}
