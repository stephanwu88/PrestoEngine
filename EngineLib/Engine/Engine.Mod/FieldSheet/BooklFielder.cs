using Engine.Common;
using Engine.Data.DBFAC;
using Engine.Files;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace Engine.Mod
{
    /// <summary>
    /// 译码本主键
    /// </summary>
    public enum DicNameBy
    {
        ColName,
        ColBind
    }

    /// <summary>
    /// 字段管理器 - 表格列字段
    /// </summary>
    public class BookFielder
    {
        public Dictionary<string, List<ModelSheetColumn>> CodeBooks = 
            new Dictionary<string, List<ModelSheetColumn>>();
        private ExcelEIO _Excel = new ExcelEIO();

        private BookFielder() { }

        private static readonly BookFielder _Instance = new BookFielder();
        public static BookFielder Instance => _Instance;

        /// <summary>
        /// 转换得到Sql语句
        /// </summary>
        /// <param name="DictName">字段对应表名</param>
        /// <param name="TableName">数据库表名</param>
        /// <param name="WhereDict">条件字典</param>
        /// <param name="LimitCount">读取条数</param>
        /// <returns></returns>
        public string ConvertQuerySQL(string DictName, string TableName,Dictionary<string,string> WhereDict,int LimitCount=1)
        {
            string strSql = string.Empty;
            string strSel = string.Empty;
            string strWhere = string.Empty;
            string strOrderBy = string.Empty;
            string strLimit = string.Empty;
            List<ModelSheetColumn> LstColumns = CodeBooks.DictFieldValue(DictName);
            if (LstColumns == null)
                return strSql;
            foreach (ModelSheetColumn modCol in LstColumns)
            {
                if (modCol.ColMark.ToMyString() == "[C]" || string.IsNullOrEmpty(modCol.ColBind) 
                    || string.IsNullOrEmpty(modCol.ColName))
                    continue;
                if (!string.IsNullOrEmpty(strSel)) strSel += ",";
                strSel += string.Format("{0} as '{1}'",modCol.ColName,modCol.ColBind);

                if (WhereDict.MyContains(modCol.ColBind))
                {
                    string strColWhereValue = WhereDict.DictFieldValue(modCol.ColBind);

                    if (strColWhereValue.CheckIfOrderBy())
                    {
                        string strOrderDirect = string.Empty;
                        string strOrderByField = strColWhereValue.ParseOrderBy();
                        if (strOrderByField.ToLower().MyContains("asc"))
                            strOrderDirect = "asc";
                        else if (strOrderByField.ToLower().MyContains("desc"))
                            strOrderDirect = "desc";
                        if (!string.IsNullOrEmpty(strOrderDirect))
                        {
                            if (!string.IsNullOrEmpty(strOrderBy))
                                strOrderBy += ",";
                            strOrderBy += string.Format(" {0} {1} ", modCol.ColName, strOrderDirect);
                        }            
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(strWhere))
                            strWhere += " and ";
                        if (strColWhereValue.CheckIfExpress())
                            strWhere += string.Format("({0} {1})", modCol.ColName, strColWhereValue.ParseExpress());
                        else
                            strWhere += string.Format("{0}='{1}'", modCol.ColName, strColWhereValue);
                    }
                }
            }
            if (!string.IsNullOrEmpty(strWhere))
                strWhere = string.Format(" where {0}", strWhere);
            if (!string.IsNullOrEmpty(strOrderBy))
                strOrderBy = string.Format(" order by {0}",strOrderBy);
            if (LimitCount > 0)
                strLimit = " limit " + LimitCount.ToMyString();
            if (!string.IsNullOrEmpty(strSel))
                strSql = string.Format("select {0} from {1} {2} {3} {4}", strSel, TableName, strWhere, strOrderBy, strLimit);
            return strSql;
        }

        /// <summary>
        /// 附加接口表
        /// </summary>
        /// <param name="LstCodeBook"></param>
        /// <param name="strFileName"></param>
        /// <returns></returns>
        public bool AppandCodeBook(List<string> LstCodeBook, string strFileName = "$\\Locales\\DataSheet.xls")
        {
            try
            {
                foreach (string item in LstCodeBook)
                {
                    List<ModelSheetColumn> Sheet = GetSheet(item, strFileName);
                    if (Sheet.MyCount() <= 0) return false;
                    CodeBooks.AppandDict(item, Sheet);
                }
                return true;
            }
            catch (Exception ex)
            {
                sCommon.MyMsgBox(ex.Message, MsgType.Error);
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="SheetName"></param>
        /// <param name="ExcelFileName"></param>
        /// <returns></returns>
        private List<ModelSheetColumn> GetSheet(string SheetName, string ExcelFileName = "$\\Locales\\DataSheet.xls")
        {
            string strFileName = ExcelFileName.Replace("$", Directory.GetCurrentDirectory());
            if (string.IsNullOrEmpty(strFileName) || !File.Exists(strFileName))
                throw new Exception("未找到列配置文件!");
            if (_Excel == null)
                _Excel = new ExcelEIO();
            LocalSource ds = new LocalSource() { SourceFile = strFileName, ProviderName = "engine.data.msexcel" };
            DataTable dt = _Excel.GetDataTable(ds, SheetName).ToMyDataTable();
            List<ModelSheetColumn> Sheet = sCommon.ToEntityList<ModelSheetColumn>(dt);
            return Sheet;
        }

        /// <summary>
        /// 属性拾取
        /// </summary>
        /// <param name="bookName"></param>
        /// <returns></returns>
        public Dictionary<string,ModelSheetColumn> this[string bookName, DicNameBy DictNameBY]
        {
            get
            {
                lock (CodeBooks)
                {
                    if (DictNameBY == DicNameBy.ColName)
                        return CodeBooks.DictFieldValue(bookName).ToMyDictionary(x => x.ColName, x => x);
                    else if (DictNameBY == DicNameBy.ColBind)
                        return CodeBooks.DictFieldValue(bookName).ToMyDictionary(x => x.ColBind, x => x);
                    else
                        return null;
                }
            }   
        }

        /// <summary>
        /// 属性拾取
        /// </summary>
        /// <param name="bookName"></param>
        /// <returns></returns>
        public List<ModelSheetColumn> this[string bookName]
        {
            get
            {
                lock (CodeBooks)
                    return CodeBooks.DictFieldValue(bookName);
            }
        }
    }
}
