using Engine.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Reflection;

namespace Engine.Data.DBFAC
{
    /// <summary>
    /// 数据表名称 
    /// 格式：[Table(Name="")]
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Enum | AttributeTargets.Property | AttributeTargets.Class)]
    public class TableAttribute : AttributeBase
    {
        private TableDef _Table = new TableDef();
        private static Dictionary<string, TableDef> DicTable;


        public TableAttribute()
        {
            
        }

        /// <summary>
        /// 表名称
        /// </summary>
        public override string Name { get => _Table.Name; set { _Table.Name = value; } }

        /// <summary>
        /// 视图表名称
        /// </summary>
        public string ViewName { get => _Table.ViewName; set { _Table.ViewName = value; } }

        /// <summary>
        /// 表描述
        /// </summary>
        public string Comments { get => _Table.Comments; set { _Table.Comments = value; } }

        /// <summary>
        /// 表索引
        /// </summary>
        public string Token { get => _Table.Token; set { _Table.Token = value; } }
        

        /// <summary> 
        /// 获取属性值的自定义特性
        /// </summary> 
        /// <param name="value"></param> 
        /// <returns></returns> 
        public static TableDef Table<T>()
        {
            TableAttribute[] attributes = (TableAttribute[])typeof(T).GetCustomAttributes(typeof(TableAttribute), true);
            if (attributes.Length > 0)
            {
                foreach (var attr in attributes)
                {
                    //if (attr.GetType() == typeof(TableAttribute))
                    //if (attr is TableAttribute)
                    string strToken = attr._Table.Token;
                    if (string.IsNullOrEmpty(strToken) || DicTable == null)
                    {
                        return attr._Table;
                    }
                    else
                    {
                        lock (DicTable)
                        {
                            if (DicTable.MyContains(strToken))
                                return DicTable[strToken];
                            else
                                return attr._Table;
                        }
                    }
                }
            }
            return null;
        }

        /// <summary> 
        /// 获取属性值的自定义特性
        /// </summary> 
        /// <param name="value"></param> 
        /// <returns></returns> 
        public static TableDef Table<T>(string PropertyName)
        {
            // 属性Error和Item来自接口IDataErrorInfo
            if (PropertyName == "Error" || PropertyName == "Item")
                return null;
            PropertyInfo pi = typeof(T).GetProperty(PropertyName);
            TableAttribute[] attributes = (TableAttribute[])pi.GetCustomAttributes(typeof(TableAttribute), true);
            if (attributes.Length > 0)
            {
                foreach (var attr in attributes)
                {
                    //if (attr.GetType() == typeof(ColumnAttribute))
                    //if (attr is ColumnAttribute)
                    return attr._Table;
                }
            }
            return null;
        }

        /// <summary>
        /// 修改表名
        /// </summary>
        /// <param name="strTableName"></param>
        public static void WriteTableName<T>(string strTableName)
        {
            //TableAttribute[] attributes = (TableAttribute[])typeof(T).GetCustomAttributes(typeof(TableAttribute), true);
            //if (attributes == null)
            //    return;
            //if (attributes.Length > 0)
            //    attributes[0].Name = strTableName;
            TableDef tableDef = Table<T>();
            tableDef.Name = strTableName;
            if (!string.IsNullOrEmpty(tableDef.Token) && tableDef != null)
            {
                if (DicTable == null)
                    DicTable = new Dictionary<string, TableDef>();
                lock (DicTable)
                    DicTable.AppandDict(tableDef.Token, tableDef);
            }
        }
    }

    /// <summary>
    /// 表列定义
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Enum | AttributeTargets.Property | AttributeTargets.Class)]
    public class ColumnAttribute : AttributeBase
    {
        private ColumnDef _Column = new ColumnDef();

        public ColumnAttribute()
        {

        }
        /// <summary>
        /// 列名称
        /// </summary>
        public override string Name { get => _Column.Name; set { _Column.Name = value; } }
        /// <summary>
        /// 数据类型
        /// </summary>
        public string DataType { get => _Column.DataType; set { _Column.DataType = value; } }
        /// <summary>
        /// 数据显示格式
        /// </summary>
        public string DataFormat { get => _Column.DataFormat; set { _Column.DataFormat = value; } }
        /// <summary>
        /// 主键
        /// </summary>
        public bool PK { get => _Column.PK; set { _Column.PK = value; } }
        /// <summary>
        /// 不为空
        /// </summary>
        public bool NN { get => _Column.NN; set { _Column.NN = value; } }
        /// <summary>
        /// 自动增长列
        /// </summary>
        public bool AI { get => _Column.AI; set { _Column.AI = value; } }
        /// <summary>
        /// 只读
        /// </summary>
        public bool ReadOnly { get => _Column.ReadOnly; set { _Column.ReadOnly = value; } }
        /// <summary>
        /// 默认值
        /// </summary>
        public string DefaultExpress { get => _Column.DefaultExpress; set { _Column.DefaultExpress = value; } }
        /// <summary>
        /// 列描述
        /// </summary>
        public string Comments { get => _Column.Comments; set { _Column.Comments = value; } }

        /// <summary> 
        /// 获取属性值的自定义特性
        /// </summary> 
        /// <param name="value"></param> 
        /// <returns></returns> 
        public static ColumnDef Column<T>(string PropertyName)
        {
            // 属性Error和Item来自接口IDataErrorInfo
            if (PropertyName == "Error" || PropertyName == "Item")
                return null;
            PropertyInfo pi = typeof(T).GetProperty(PropertyName);
            ColumnAttribute[] attributes = (ColumnAttribute[])pi.GetCustomAttributes(typeof(ColumnAttribute), true);
            if (attributes.Length > 0)
            {
                foreach (var attr in attributes)
                {
                    //if (attr.GetType() == typeof(ColumnAttribute))
                    //if (attr is ColumnAttribute)
                    return attr._Column;
                }
            }
            return null;
        }
    }

    /// <summary>
    /// 标记字典字段
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Enum | AttributeTargets.Property | AttributeTargets.Class)]
    public class DictFieldAttribute : AttributeBase
    {
        /// <summary>
        /// 字典值转换器
        /// </summary>
        public string DicValueConvert { get; set; }
        /// <summary>
        /// 说明
        /// </summary>
        public string Comments { get; set; }
    }

    /// <summary>
    /// 数据库表定义
    /// </summary>
    public class TableDef
    {
        /// <summary>
        /// 表名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 视图表名称
        /// </summary>
        public string ViewName { get; set; }
        /// <summary>
        /// 表描述
        /// </summary>
        public string Comments { get; set; }
        /// <summary>
        /// 主键
        /// </summary>
        public string Token { get; set; }
    }

    /// <summary>
    /// 数据库表列定义
    /// </summary>
    public class ColumnDef
    {
        /// <summary>
        /// 列名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 数据类型
        /// </summary>
        public string DataType { get; set; }
        /// <summary>
        /// 数据显示格式
        /// </summary>
        public string DataFormat { get; set; }
        /// <summary>
        /// 主键
        /// </summary>
        public bool PK { get; set; }
        /// <summary>
        /// 不为空
        /// </summary>
        public bool NN { get; set; }
        /// <summary>
        /// 自动增长列
        /// </summary>
        public bool AI { get; set; }
        /// <summary>
        /// 只读
        /// </summary>
        public bool ReadOnly { get; set; }
        /// <summary>
        /// 默认值
        /// </summary>
        public string DefaultExpress { get; set; }
        /// <summary>
        /// 列描述
        /// </summary>
        public string Comments { get; set; }

        /// <summary>
        /// 获取列定义结构
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="PropName"></param>
        /// <returns></returns>
        public static ColumnDef GetObjectColumnDef<T>(string PropName)
        {
            ColumnDef colDef = ColumnAttribute.Column<T>(PropName);
            if (colDef == null)
                colDef = new ColumnDef();
            return colDef;
        }

        /// <summary>
        /// DataRowView - T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="rowView"></param>
        /// <param name="rowIndex"></param>
        /// <returns></returns>
        public static T ToEntity<T>(DataRowView rowView, int rowIndex = -1) where T : new()
        {
            if (rowView == null)
                return default(T);
            DataRow row = rowView.Row;
            return ToEntity<T>(row, rowIndex);
        }

        /// <summary>
        /// DataRow - T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="row"></param>
        /// <returns></returns>
        public static T ToEntity<T>(DataRow row, int rowIndex = -1) where T : new()
        {
            if (row == null)
                return default(T);
            T model = new T();
            PropertyInfo[] propertys = model.GetType().GetProperties();
            DataColumnCollection Columns = row.Table.Columns;
            foreach (PropertyInfo pi in propertys)
            {
                string columnName = pi.Name;
                string defColumnName = ColumnDef.GetObjectColumnDef<T>(pi.Name).Name.ToMyString();
                string MatchedColumeName = string.Empty;
                if (Columns.Contains(defColumnName))
                    MatchedColumeName = defColumnName;
                else if (Columns.Contains(columnName))
                    MatchedColumeName = columnName;

                object value = null;
                if (string.IsNullOrEmpty(MatchedColumeName))
                {
                    //自动序号列
                    object[] AutoIndexAttrs = pi.GetCustomAttributes(typeof(AutoIndexAttribute), true);
                    if (AutoIndexAttrs == null || rowIndex <= 0)
                        continue;
                    if (AutoIndexAttrs.Length == 0)
                        continue;
                    value = rowIndex.ToString();
                }
                else
                {
                    if (Columns.Contains(MatchedColumeName))
                    {
                        if (Columns[MatchedColumeName].DataType == typeof(DateTime))
                            value = row[MatchedColumeName].ToMyDateTimeStr();
                        else
                            value = row[MatchedColumeName];
                    }
                }
                if (value is DBNull)
                    continue;
                pi.SetPropValue<T>(ref model, value);
            }
            return model;
        }

        /// <summary>
        /// DataTable - List T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static List<T> ToEntityList<T>(DataTable dt) where T : new()
        {
            List<T> list = new List<T>();
            if (dt != null)
            {
                if (dt.Rows.Count > 0)
                {
                    int rowIndex = 1;
                    foreach (DataRow row in dt.Rows)
                    {
                        list.Add(ToEntity<T>(row, rowIndex));
                        rowIndex++;
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// DataTable - ObservableCollection T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static ObservableCollection<T> ToEntityObserver<T>(DataTable dt) where T : new()
        {
            ObservableCollection<T> list = new ObservableCollection<T>();
            if (dt != null)
            {
                if (dt.Rows.Count > 0)
                {
                    int rowIndex = 1;
                    foreach (DataRow row in dt.Rows)
                    {
                        list.Add(ToEntity<T>(row, rowIndex));
                        rowIndex++;
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// DataRow - T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="row"></param>
        /// <returns></returns>
        public static T ToEntityEmpty<T>(DataRow row) where T : new()
        {
            if (row == null)
                return default(T);
            T model = new T();
            PropertyInfo[] propertys = model.GetType().GetProperties();
            DataColumnCollection Columns = row.Table.Columns;
            foreach (PropertyInfo pi in propertys)
            {
                string columnName = pi.Name;
                string defColumnName = ColumnDef.GetObjectColumnDef<T>(pi.Name).Name.ToMyString();
                string MatchedColumeName = string.Empty;
                if (Columns.Contains(defColumnName))
                    MatchedColumeName = defColumnName;
                else if (Columns.Contains(columnName))
                    MatchedColumeName = columnName;
                if (string.IsNullOrEmpty(MatchedColumeName))
                    continue;
                object value = SystemDefault.StringEmpty;
                if (value is DBNull)
                    continue;
                pi.SetPropValue<T>(ref model, value);
            }
            return model;
        }
    }
}
