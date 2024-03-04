using System.Collections.Generic;
using System.Linq;

namespace Engine.ComDriver
{
    /// <summary>
    /// 电文字段结构
    /// </summary>
    public class TelegramField
    {
        /// <summary>
        /// 字段类型（必须 or 可选）MUST OPTION
        /// </summary>
        public string FieldType = "MUST";
        /// <summary>
        /// 字段引用符号
        /// Ref 时字段双引号 ""
        /// </summary>
        public string FiledMark;
        /// <summary>
        /// 字段名称
        /// </summary>
        public string FieldName;
        /// <summary>
        /// 字段值
        /// </summary>
        public string FieldVal;
    }

    /// <summary>
    /// 电文结构
    /// </summary>
    public class Telegram
    {
        /// <summary>
        /// 电文功能代码
        /// </summary>
        public string MsgType;

        /// <summary>
        /// 电文参数字典
        /// </summary>
        public Dictionary<string, TelegramField> Fields = new Dictionary<string, TelegramField>();

        public void Add(TelegramField field)
        {
            if (Fields.ContainsKey(field.FieldName))
                Fields[field.FieldName] = field;
            else
                Fields.Add(field.FieldName, field);
        }

        public void Add(List<TelegramField> LstFields)
        {
            foreach (var field in LstFields)
            {
                if (Fields.ContainsKey(field.FieldName))
                    Fields[field.FieldName] = field;
                else
                    Fields.Add(field.FieldName, field);
            }
        }

        public string GetFieldString(int id)
        {
            TelegramField[] FieldVal = Fields.Values.ToArray();
            if (FieldVal.Length > id)
                return FieldVal[id].FieldVal;
            else
                return string.Empty;
        }
        public int FieldCount => Fields.Count;
    }
}
