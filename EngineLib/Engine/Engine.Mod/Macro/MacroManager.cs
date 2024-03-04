using Engine.Common;
using System.Collections.Generic;

namespace Engine.Mod
{
    /// <summary>
    /// 宏指令管理器
    /// </summary>
    public static class MacroManager
    {
        /// <summary>
        /// 表达式解析装载到字典
        /// </summary>
        /// <param name="SourceDict"></param>
        /// <param name="Express"></param>
        public static void ParseExpress(this Dictionary<string,object> SourceDict, string Express)
        {
            string FieldName = Express.MidString("", "=");
            string FieldValue = Express.MidString("=", "(");
            if (!string.IsNullOrEmpty(FieldName) || string.IsNullOrEmpty(FieldValue))
                return;
            //常量赋值表达式
            if (!Express.Contains("("))
            {
                SourceDict.AppandDict(FieldName, FieldValue);
                return;
            }
            //函数赋值表达式
            string FuncName = Express.MidString("=", "(").Trim().ToLower();
            string FuncParam = Express.MidString("(", ")", EndStringSearchMode.FromTail);
            switch (FuncName)
            {
                case "now":     //字段赋值为当前时间
                    SourceDict.AppandDict(FieldName, SystemDefault.StringTimeNow);
                    break;
            }

        }
        
    }
}
