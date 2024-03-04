using Engine.Common;
using System.Collections.Generic;
using System.Reflection;

namespace Engine.Data.DBFAC
{
    /// <summary>
    /// 字段标记处理类
    /// </summary>
    public static class FieldMark
    {
        //[Where=][Express]DataType in('t1','t2')[@Express]
        //{ "ExpressStartMark","[Express]"},
        //{ "ExpressEndMark","[@Express]"},
        private static Dictionary<string, string> DicMark = new Dictionary<string, string>()
        {
            { "WhereMark","[@Where]"},
            { "ExpressMark","[@Express]"},
            { "OrderByMark","[@OrderBy]"},
        };

        /// <summary>
        /// 标记字段为表达式格式
        /// [Express]sdfg[@OrderBy]asc[@Va]sdf[@Express]
        /// </summary>
        /// <param name="Field"></param>
        /// <returns></returns>
        public static string MarkExpress(this string Field)
        {
            //return string.Format("{0}{1}{2}", DicMark["ExpressStartMark"], Field, DicMark["ExpressEndMark"]);
            return string.Format("{0}{1}", DicMark["ExpressMark"], Field);
        }

        /// <summary>
        /// 标记字段为Where段
        /// </summary>
        /// <param name="Field"></param>
        /// <returns></returns>
        public static string MarkWhere(this string Field)
        {
            if (Field.CheckIfWhere()) return Field;
            return string.Format("{0}{1}", DicMark["WhereMark"],Field);
        }

        /// <summary>
        /// 标记字段为OrderBy段
        /// </summary>
        /// <param name="Field"></param>
        /// <returns></returns>
        public static string MarkOrderBy(this string Field)
        {
            return string.Format("{0}{1}", DicMark["OrderByMark"], Field);
        }

        /// <summary>
        /// 解析表达式内容
        /// </summary>
        /// <param name="Express"></param>
        /// <returns></returns>
        public static string ParseExpress(this string Express)
        {
            //string ret = Express;
            //string strStartMark = DicMark["ExpressStartMark"];
            //string strEndMak = DicMark["ExpressEndMark"];
            //if (Express.Contains(strStartMark) && Express.Contains(strEndMak))
            //    ret = Express.Replace(strStartMark, "").Replace(strEndMak, "") ;
            //return ret;
            if (!Express.CheckIfExpress()) return Express;
            return Express.MidString(DicMark["ExpressMark"], "[", EndStringSearchMode.FromHeadAndToEndWhenUnMatch);
        }

        /// <summary>
        /// 解析Where段
        /// </summary>
        /// <param name="Where"></param>
        /// <returns></returns>
        public static string ParseWhere(this string Where)
        {
            //string ret = Where;
            //string strWhereMark = DicMark["WhereMark"];
            //if (Where.Contains(strWhereMark))
            //    ret = Where.Replace(strWhereMark, "");
            //return ret;
            return Where.MidString(DicMark["WhereMark"], "[", EndStringSearchMode.FromHeadAndToEndWhenUnMatch);
        }

        /// <summary>
        /// 解析OrderBy段
        /// </summary>
        /// <param name="Where"></param>
        /// <returns></returns>
        public static string ParseOrderBy(this string OrderBy)
        {
            return OrderBy.MidString(DicMark["OrderByMark"], "[", EndStringSearchMode.FromHeadAndToEndWhenUnMatch);
        }

        /// <summary>
        /// 还原标记前内容
        /// </summary>
        /// <param name="FieldWithMark"></param>
        /// <returns></returns>
        public static string GoOriginal(this string FieldWithMark)
        {
            string ret = FieldWithMark;
            foreach (string mark in DicMark.Values)
            {
                ret = ret.Replace(mark,"");
            }
            return ret;
        }

        /// <summary>
        /// 检查是否被标记
        /// </summary>
        /// <param name="CheckField"></param>
        /// <returns></returns>
        public static bool CheckIfMarked(this string CheckField)
        {
            bool ret = false;
            foreach (string mark in DicMark.Values)
            {
                if (CheckField.Contains(mark))
                {
                    ret = true;
                    break;
                }
            }
            return ret;
        }

        /// <summary>
        /// 检查是否被表达式
        /// </summary>
        /// <param name="CheckField"></param>
        /// <returns></returns>
        public static bool CheckIfExpress(this string CheckField)
        {
            return CheckField.Contains(DicMark["ExpressMark"]);
        }

        /// <summary>
        /// 检查是否排序字段
        /// </summary>
        /// <param name="CheckField"></param>
        /// <returns></returns>
        public static bool CheckIfOrderBy(this string CheckField)
        {
            return CheckField.Contains(DicMark["OrderByMark"]);
        }

        /// <summary>
        /// 检查是否Where段
        /// </summary>
        /// <param name="CheckField"></param>
        /// <returns></returns>
        public static bool CheckIfWhere(this string CheckField)
        {
            return CheckField.Contains(DicMark["WhereMark"]);
        }

        /// <summary>
        /// 字段写入值前过滤操作
        /// </summary>
        /// <param name="FieldValue"></param>
        /// <returns></returns>
        public static string ValueFilter(this string FieldValue)
        {
            if (FieldValue == SystemDefault.StringEmpty)
                return "";
            return FieldValue;
        }

        /// <summary>
        /// 字段写入值前标记操作
        /// </summary>
        /// <param name="FieldValue"></param>
        /// <returns></returns>
        public static string ValueAttachMark(this string FieldValue)
        {
            if (string.IsNullOrEmpty(FieldValue) || string.IsNullOrWhiteSpace(FieldValue))
                return SystemDefault.StringEmpty;
            return FieldValue;
        }

        /// <summary>
        /// 实体写入值前标记操作
        /// </summary>
        /// <param name="FieldValue"></param>
        /// <returns></returns>
        public static void ValueAttachMark<T>(this T Model)
        {
            if (Model == null)
                return;
            var Type = typeof(T);
            foreach (PropertyInfo pi in Type.GetProperties())
            {
                if (pi.Name == "Item" || pi.Name == "Error" || pi.Name=="ID")
                    continue;
                string value = string.Empty;
                if (pi.PropertyType == typeof(string))
                {
                    value = pi.GetValue(Model, null).ToMyString();
                    pi.SetValue(Model, value.ValueAttachMark(), null);
                }
            }
        }
    }
}
