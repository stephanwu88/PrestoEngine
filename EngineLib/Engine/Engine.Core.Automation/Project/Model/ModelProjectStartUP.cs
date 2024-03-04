using System.Collections.Generic;
namespace Engine.Core
{
    /// <summary>
    /// 工程启动组
    /// </summary>
    public class ProjectStartUP
    {
        /// <summary>
        /// StartUP字段，与ini文件StartUP组匹配
        /// </summary>
        private Dictionary<string, string> DicStartUP = new Dictionary<string, string>();

        /// <summary>
        /// 添加字段
        /// </summary>
        /// <param name="strKey"></param>
        /// <param name="strValue"></param>
        public void AppandField(string strKey,string strValue)
        {
            if (DicStartUP.ContainsKey(strKey))
                DicStartUP[strKey] = strValue;
            else
                DicStartUP.Add(strKey, strValue);
        }

        /// <summary>
        /// 字段索引器
        /// </summary>
        /// <param name="strKey"></param>
        /// <returns></returns>
        public object this[string strKey]
        {
            get
            {
                if (DicStartUP.ContainsKey(strKey))
                    return DicStartUP[strKey];
                return string.Empty;
            }
        }
    }
}
