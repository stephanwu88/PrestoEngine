using System.Collections.Generic;

namespace Engine.Common
{
    /// <summary>
    /// 工位项定义
    /// </summary>
    public class PosItem
    {
        public string Name = "";
        public string Item = "";
        public string Value = "";
    }
    /// <summary>
    /// Symbol 系统符号处理模块相关
    /// </summary>
    public static partial class ParseConverter
    {
        /// <summary>
        /// 解析表达式获取字段元素
        /// </summary>
        /// <param name="strExpress">ex:CrusherGroup.Crusher1.STEP_KEY = Step1 </param>
        /// <returns></returns>
        public static PosItem ParsePosItem(this string strExpress)
        {
            PosItem posItem = new PosItem();
            string strItem = strExpress.MidString("", "=").Trim();
            string strSetValue = strExpress.MidString("=", "").Trim();
            List<string> ObjectItemPart = strItem.MySplit(".");
            if (ObjectItemPart.Count != 2 && ObjectItemPart.Count != 3)
                return posItem;
            if (ObjectItemPart.Count == 2)
            {
                posItem.Name = ObjectItemPart[0];
                posItem.Item = ObjectItemPart[1];
            }
            else if (ObjectItemPart.Count == 3)
            {
                posItem.Name = ObjectItemPart[0] + "." + ObjectItemPart[1];
                posItem.Item = ObjectItemPart[2];
            }
            posItem.Value = strSetValue;
            return posItem;
        }
    }
}
