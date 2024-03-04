using Engine.Data.DBFAC;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Reflection;

namespace Engine.Common
{
    /// <summary>
    /// 特性模板类
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Enum | AttributeTargets.Property | AttributeTargets.Class)]
    public class ModelAttribute : AttributeBase
    {
        private ModelDef _Model = new ModelDef();

        public ModelAttribute()
        {

        }
        /// <summary>
        /// 名称
        /// </summary>
        public override string Name { get => _Model.Name; set { _Model.Name = value; } }
        /// <summary>
        /// 数据类型
        /// </summary>
        public string DataType { get => _Model.DataType; set { _Model.DataType = value; } }
        /// <summary>
        /// 数据格式
        /// </summary>
        public string DataFormat { get => _Model.DataFormat; set { _Model.DataFormat = value; } }
        /// <summary>
        /// 描述
        /// </summary>
        public string Comments { get => _Model.Comments; set { _Model.Comments = value; } }

        /// <summary> 
        /// 获取属性值的自定义特性
        /// </summary> 
        /// <param name="value"></param> 
        /// <returns></returns> 
        public static ModelDef Model<T>(string PropertyName)
        {
            // 属性Error和Item来自接口IDataErrorInfo
            if (PropertyName == "Error" || PropertyName == "Item")
                return null;
            PropertyInfo pi = typeof(T).GetProperty(PropertyName);
            ModelAttribute[] attributes = (ModelAttribute[])pi.GetCustomAttributes(typeof(ModelAttribute), true);
            if (attributes.Length > 0)
            {
                foreach (var attr in attributes)
                {
                    //if (attr.GetType() == typeof(ColumnAttribute))
                    //if (attr is ColumnAttribute)
                    return attr._Model;
                }
            }
            return null;
        }
    }

    /// <summary>
    /// 模板定义
    /// </summary>
    public class ModelDef
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 数据类型
        /// </summary>
        public string DataType { get; set; }
        /// <summary>
        /// 数据格式
        /// </summary>
        public string DataFormat { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Comments { get; set; }

        /// <summary>
        /// 获取列定义结构
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="PropName"></param>
        /// <returns></returns>
        public static ModelDef GetObjectModelDef<T>(string PropName)
        {
            ModelDef modelDef = ModelAttribute.Model<T>(PropName);
            if (modelDef == null)
                modelDef = new ModelDef();
            return modelDef;
        }

        /// <summary>
        /// DataRow - T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="row"></param>
        /// <returns></returns>
        public static T ToEntity<T>(DataRow row) where T : new()
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
                object value = row[MatchedColumeName];
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
                    foreach (DataRow row in dt.Rows)
                        list.Add(ToEntity<T>(row));
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
                    foreach (DataRow row in dt.Rows)
                        list.Add(ToEntity<T>(row));
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


    /// <summary>
    /// 自动序号
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class AutoIndexAttribute : ModelAttribute
    {

    }
}
