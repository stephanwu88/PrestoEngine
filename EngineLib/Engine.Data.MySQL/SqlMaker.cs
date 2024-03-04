using Engine.Common;
using System.Collections.Generic;

namespace Engine.Data.MySQL
{
    /// <summary>
    /// SQL 生成器
    /// </summary>
    public static class SqlMaker
    {
        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public static string SqlQuery(this string table)
        {
            return string.Format("select * from {0};", table);
        }

        /// <summary>
        /// 条件查询
        /// </summary>
        /// <param name="table"></param>
        /// <param name="where"></param>
        /// <returns></returns>
        public static string SqlQuery(this string table, string where)
        {
            if (!string.IsNullOrEmpty(where))
                where = string.Format(" where {0}", where);
            return string.Format("select * from {0} {1};", table, where);
        }

        /// <summary>
        /// Insert SQL语句
        /// </summary>
        /// <returns></returns>
        public static string SqlInsert(this string table, string sectionInsert, string sectionValue)
        {
            if (string.IsNullOrEmpty(table) || string.IsNullOrEmpty(sectionInsert) ||
                string.IsNullOrEmpty(sectionValue))
                return string.Empty;
            string strSql = string.Format("insert into {0}({1}) values({2});", table, sectionInsert, sectionValue);
            return strSql;
        }

        /// <summary>
        /// Update SQL语句
        /// </summary>
        /// <returns></returns>
        public static string SqlUpdate(this string table, string sectionUpdate, string sectionWhere)
        {
            if (string.IsNullOrEmpty(table) || string.IsNullOrEmpty(sectionUpdate))
                return string.Empty;
            if (!string.IsNullOrEmpty(sectionWhere))
                sectionWhere = string.Format(" where {0}", sectionWhere);
            string strSql = string.Format("update {0} set {1} {2};", table, sectionUpdate, sectionWhere);
            return strSql;
        }

        /// <summary>
        /// Delete SQL语句
        /// </summary>
        /// <returns></returns>
        public static string SqlDelete(this string table, string sectionWhere)
        {
            if (string.IsNullOrEmpty(table))
                return string.Empty;
            if (!string.IsNullOrEmpty(sectionWhere))
                sectionWhere = string.Format(" where {0}", sectionWhere);
            string strSql = string.Format("delete from {0} {1};", table, sectionWhere);
            return strSql;
        }

        /// <summary>
        /// 创建事务语句集
        /// </summary>
        /// <param name="LstSql"></param>
        /// <returns></returns>
        public static string MakeSqlSet(this List<string> LstSql)
        {
            if (LstSql == null)
                return string.Empty;
            string strSql = string.Empty;
            foreach (var item in LstSql)
                strSql += item;
            return strSql;
        }

        /// <summary>
        /// 添加清表
        /// </summary>
        /// <param name="Source"></param>
        /// <param name="table"></param>
        /// <returns></returns>
        public static List<string> JoinTruncate(this string Source, string table)
        {
            string strTrancate = string.Format("truncate table {0};\r\n", table);
            return new List<string>() { strTrancate, Source };
        }

        /// <summary>
        /// SQL Where片段
        /// </summary>
        /// <param name="FieldExpress">[ID =1][Token='']</param>
        /// <returns></returns>
        public static string SectionWhere(this List<string> FieldExpress)
        {
            if (FieldExpress == null)
                return string.Empty;
            string strWhere = string.Empty;
            foreach (string item in FieldExpress)
            {
                string str = item.Replace("'", "");
                if (!string.IsNullOrEmpty(strWhere))
                    strWhere += " and ";
                strWhere += string.Format("{0}='{1}'", str.MidString("", "=").Trim(), str.MidString("=", "").Trim());
            }
            return strWhere;
        }

        /// <summary>
        /// SQL Update片段
        /// </summary>
        /// <param name="FieldExpress"></param>
        /// <returns></returns>
        public static string SectionUpdate(this List<string> FieldExpress)
        {
            if (FieldExpress == null)
                return string.Empty;
            string strUpdate = string.Empty;
            foreach (string item in FieldExpress)
            {
                string str = item.Replace("'", "");
                if (!string.IsNullOrEmpty(strUpdate))
                    strUpdate += ",";
                strUpdate += string.Format("{0}='{1}'", str.MidString("", "=").Trim(), str.MidString("=", "").Trim());
            }
            return strUpdate;
        }

        /// <summary>
        /// SQL Insert片段
        /// </summary>
        /// <returns>[Field][Value]</returns>
        public static string[] SectionInsert(this List<string> FieldExpress)
        {
            string[] ArrayField = new string[2];
            if (FieldExpress == null)
                return ArrayField;
            string strInsertField = string.Empty;
            string strInsertValue = string.Empty;
            foreach (string item in FieldExpress)
            {
                string str = item.Replace("'", "");
                if (!string.IsNullOrEmpty(strInsertField))
                {
                    strInsertField += ",";
                    strInsertValue += ",";
                }
                strInsertField += string.Format("{0}", str.MidString("", "=").Trim());
                strInsertValue += string.Format("{0}", str.MidString("=", "").Trim());
            }
            ArrayField[0] = strInsertField;
            ArrayField[1] = strInsertValue;
            return ArrayField;
        }
    }
}
