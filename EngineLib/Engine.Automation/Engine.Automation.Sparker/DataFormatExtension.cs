using Engine.Common;

namespace Engine.Automation.Sparker
{
    public static class DataFormatExtension
    {
        /// <summary>
        /// 默认修约格式化
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static string RoundToDefaultDecimalAndFormat(this double number)
        {
            return number.RoundToDecimalAndFormat(4, "0.0000");
        }
    }
}
 