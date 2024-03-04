using Engine.Data.DBFAC;
using Engine.Mod;
using Engine.Common;
using System.Collections.Generic;
using System.Reflection;
using System.ComponentModel;
using System.Data;

namespace Engine.Data
{
    /// <summary>
    /// 数据状态
    /// </summary>
    public enum DataStatus
    {
        [Description("缺省")]
        Default,
        [Description("创建")]
        CreateNew,
        [Description("修正数据")]
        DataCorrection,
        [Description("数据超限")]
        DataOverRange,
        [Description("审核未通过")]
        FailPass,
        [Description("审核通过")]
        AuditPass,
        [Description("禁止上传")]
        DISABLE,
        [Description("已上传")]
        Uploaded,
        [Description("上传失败")]
        UploadFail,
    }

    /// <summary>
    /// 拓展字段
    /// </summary>
    [Table(Name = "", Comments = "")]
    public abstract class EntityEF
    {
        [Table()]
        public string TableName { get; set; } = "";

        public List<ModelSheetColumn> FieldBook;

        //来自源数据（字段）的字典 - 转换前
        public Dictionary<string, object> DicSrcField { get; set; }
            = new Dictionary<string, object>();

        [Column(Comments = "转换后的指令-目标（字段）字典 转换后")]
        public Dictionary<string, object> DicSqlField { get; set; }
            = new Dictionary<string, object>();

        /// <summary>
        /// 添加内部属性到数据源字典
        /// </summary>
        /// <returns></returns>
        public bool AppandSourceField()
        {
            PropertyInfo[] properties = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo pi in properties)
            {
                // 属性Error和Item来自接口IDataErrorInfo，无需进行验证
                if (pi.Name == "Error" || pi.Name == "Item")
                    continue;
                DictFieldAttribute[] attributes = (DictFieldAttribute[])pi.GetCustomAttributes(typeof(DictFieldAttribute), true);
                object PropValue = this.GetPropValue(pi);
                if (attributes == null)
                    continue;
                if (attributes.Length > 0 && !string.IsNullOrEmpty(PropValue.ToMyString()))
                {
                    DicSrcField.AppandDict(attributes[0].Name, PropValue);
                }
            }
            return true;
        }

        /// <summary>
        /// 添加实体到数据源字典
        /// </summary>
        /// <returns></returns>
        public bool AppandSourceField<T>(T Model)
        {
            PropertyInfo[] properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo pi in properties)
            {
                // 属性Error和Item来自接口IDataErrorInfo，无需进行验证
                if (pi.Name == "Error" || pi.Name == "Item")
                    continue;
                DictFieldAttribute[] attributes = (DictFieldAttribute[])pi.GetCustomAttributes(typeof(DictFieldAttribute), true);
                object PropValue = Model.GetPropValue(pi);
                if (attributes == null)
                    continue;
                if (attributes.Length > 0)
                    DicSrcField.AppandDict(attributes[0].Name, PropValue);
            }
            return true;
        }

        /// <summary>
        /// 添加字段到数据源字典
        /// </summary>
        /// <param name="strItem"></param>
        /// <param name="strVal"></param>
        public void AppandSourceField(string strItem, object strVal)
        {
            DicSrcField.AppandDict(strItem, strVal);
        }

        /// <summary>
        /// 添加数据行到数据源字典
        /// </summary>
        /// <param name="row"></param>
        public void AppandSourceField(DataRow row)
        {
            if (row == null)
                return;
            foreach (DataColumn col in row.Table.Columns)
                DicSrcField.AppandDict(col.ColumnName, row[col.ColumnName]);
        }

        /// <summary>
        /// 添加数据行到数据源字典
        /// </summary>
        /// <param name="row"></param>
        public void AppandSqlField(DataRow row)
        {
            if (row == null)
                return;
            foreach (DataColumn col in row.Table.Columns)
                DicSqlField.AppandDict(col.ColumnName, row[col.ColumnName]);
        }

        /// <summary>
        /// 添加字典到数据源字典
        /// </summary>
        /// <param name="SourceDic"></param>
        public void AppandSourceField(Dictionary<string, object> SourceDic)
        {
            if (SourceDic == null)
                return;
            foreach (var item in SourceDic)
                DicSrcField.AppandDict(item.Key, item.Value);
        }

        /// <summary>
        /// 添加字典到数据源字典
        /// </summary>
        /// <param name="SourceDic"></param>
        public void AppandSqlField(Dictionary<string, object> SourceDic)
        {
            if (SourceDic == null)
                return;
            foreach (var item in SourceDic)
                DicSqlField.AppandDict(item.Key, item.Value);
        }

        /// <summary>
        /// 转换成数据库字段字典
        /// </summary>
        /// <param name="DicCodeBook"></param>
        /// <param name="DataCorrection">数据修正开关</param>
        /// <returns></returns>
        public void ConvertToSqlDict(Dictionary<string, ModelSheetColumn> DicCodeBook, bool DataCorrection = false)
        {
            AppandSourceField();
            DicSqlField.Clear();
            foreach (string item in DicSrcField.Keys)
            {
                ModelSheetColumn modCol = DicCodeBook.DictFieldValue(item);
                if (modCol == null)
                    continue;
                string ColName = modCol.ColName;
                object ColValue = DicSrcField[item];
                if (modCol.ColType == "datetime" && string.IsNullOrEmpty(ColValue.ToMyString()))
                    continue;
                //修正数据
                if (DataCorrection) ColValue = ColValue.DataCorrection(modCol);
                //附加数据
                DicSqlField.AppandDict(ColName, ColValue);
            }
        }

        /// <summary>
        /// 转换成数据库字段字典
        /// </summary>
        /// <param name="DicCodeBook"></param>
        /// <param name="DataCorrection">数据修正开关</param>
        /// <param name="LstOutRange">超限数据项列表</param>
        /// <returns></returns>
        public void ConvertToSqlDict(Dictionary<string, ModelSheetColumn> DicCodeBook, bool DataCorrection, out List<ModelSheetColumn> LstOutRange)
        {
            AppandSourceField();
            DicSqlField.Clear();
            LstOutRange = new List<ModelSheetColumn>();
            foreach (string item in DicSrcField.Keys)
            {
                ModelSheetColumn modCol = DicCodeBook.DictFieldValue(item);
                if (modCol == null)
                    continue;
                string ColName = modCol.ColName;
                object ColValue = DicSrcField[item];
                if (modCol.ColType == "datetime" && string.IsNullOrEmpty(ColValue.ToMyString()))
                    continue;
                //修正数据
                if (DataCorrection) ColValue = ColValue.DataCorrection(modCol);
                //附加数据
                DicSqlField.AppandDict(ColName, ColValue);
                //上下限判定
                bool IsSuccess = ColValue.ValidateDataRange(modCol, out bool ULmt, out bool DLmt);
                modCol.CurrentValue = ColValue.ToMyString();
                if(!IsSuccess)
                    LstOutRange.Add(modCol);
            }
        }

        /// <summary>
        /// 转换成数据库字段字典
        /// </summary>
        /// <param name="DicCodeBook"></param>
        /// <returns></returns>
        public void ConvertToSrcDict(Dictionary<string, ModelSheetColumn> DicCodeBook)
        {
            foreach (string item in DicSqlField.Keys)
            {
                ModelSheetColumn modCol = DicCodeBook.DictFieldValue(item);
                if (modCol == null)
                    continue;
                string ColName = modCol.ColBind;
                object ColValue = DicSqlField[item];
                DicSrcField.AppandDict(ColName, ColValue);
            }
        }
    }
}
