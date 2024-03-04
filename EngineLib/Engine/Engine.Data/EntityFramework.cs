using Engine.Common;
using Engine.Data.DBFAC;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;

namespace Engine.Data
{
    /// <summary>
    /// EntityFrameworkCore框架 - 网络数据库操作
    /// </summary>
    public static partial class EFCore
    {
        /// <summary>
        /// 表查询方法 - DataTable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="SourceData"></param>
        /// <param name="serverNode"></param>
        /// <returns></returns>
        public static DataTable ExcuteQuery<T>(this T SourceData, ServerNode serverNode)
        {
            DataTable dt = DbFactory.Current[serverNode].ExcuteQuery<T>(SourceData).Result.ToMyDataTable();
            return dt;
        }

        /// <summary>
        /// 实体插入到表方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="SourceData"></param>
        /// <param name="serverNode"></param>
        /// <returns></returns>
        public static CallResult ExcuteInsert<T>(this T SourceData, ServerNode serverNode)
        {
            CallResult ret = DbFactory.Current[serverNode].ExcuteInsert<T>(SourceData);
            return ret;
        }

        /// <summary>
        /// 实体集合插入到表方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="LstSourceData"></param>
        /// <param name="serverNode"></param>
        /// <returns></returns>
        public static CallResult ExcuteInsert<T>(this List<T> LstSourceData, ServerNode serverNode)
        {
            CallResult ret = DbFactory.Current[serverNode].ExcuteInsert<T>(LstSourceData);
            return ret;
        }

        /// <summary>
        /// 实体集合插入到表方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="LstSourceData"></param>
        /// <param name="serverNode"></param>
        /// <returns></returns>
        public static CallResult ExcuteInsert<T>(this ObservableCollection<T> LstSourceData, ServerNode serverNode)
        {
            CallResult ret = DbFactory.Current[serverNode].ExcuteInsert<T>(LstSourceData);
            return ret;
        }

        /// <summary>
        /// 实体更新到表方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="SourceData"></param>
        /// <param name="serverNode"></param>
        /// <param name="WhereSection">Where片段特别指定则不取模板</param>
        /// <param name="SafeMode">安全模式</param>
        /// <param name="DefaultValueSet"></param>
        /// <param name="DefaultWriteValue"></param>
        /// <returns></returns>
        public static CallResult ExcuteUpdate<T>(this T SourceData, ServerNode serverNode, string WhereSection = "$", bool SafeMode = true, string DefaultWriteValue = null)
        {
            CallResult ret = DbFactory.Current[serverNode].ExcuteUpdate<T>(SourceData,WhereSection,SafeMode,DefaultWriteValue);
            return ret;
        }

        /// <summary>
        /// 实体集合更新到表方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="LstSourceData"></param>
        /// <param name="serverNode"></param>
        /// <param name="WhereSection">Where片段特别指定则不取模板</param>
        /// <param name="SafeMode">安全模式</param>
        /// <param name="DefaultValueSet"></param>
        /// <param name="DefaultWriteValue"></param>
        /// <returns></returns>
        public static CallResult ExcuteUpdate<T>(this List<T> LstSourceData, ServerNode serverNode, string WhereSection = "$", bool SafeMode = true, string DefaultWriteValue = null)
        {
            CallResult ret = DbFactory.Current[serverNode].ExcuteUpdate<T>(LstSourceData, WhereSection, SafeMode, DefaultWriteValue);
            return ret;
        }

        /// <summary>
        /// 实体集合更新到表方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="LstSourceData"></param>
        /// <param name="serverNode"></param>
        /// <param name="WhereSection">Where片段特别指定则不取模板</param>
        /// <param name="SafeMode">安全模式</param>
        /// <param name="DefaultValueSet"></param>
        /// <param name="DefaultWriteValue"></param>
        /// <returns></returns>
        public static CallResult ExcuteUpdate<T>(this ObservableCollection<T> LstSourceData, ServerNode serverNode, string WhereSection = "$", bool SafeMode = true, string DefaultWriteValue = null)
        {
            CallResult ret = DbFactory.Current[serverNode].ExcuteUpdate<T>(LstSourceData, WhereSection, SafeMode, DefaultWriteValue);
            return ret;
        }

        /// <summary>
        /// 表记录删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="SourceData"></param>
        /// <param name="serverNode"></param>
        /// <returns></returns>
        public static CallResult ExcuteDelete<T>(this T SourceData, ServerNode serverNode)
        {
            CallResult ret = DbFactory.Current[serverNode].ExcuteDelete<T>(SourceData);
            return ret;
        }
    }

    /// <summary>
    /// EntityFrameworkCore框架 - 本地数据库操作
    /// </summary>
    public static partial class EFCore
    {
        /// <summary>
        /// 表查询方法 - DataTable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="SourceData"></param>
        /// <param name="localSource"></param>
        /// <returns></returns>
        public static DataTable ExcuteQuery<T>(this T SourceData, LocalSource localSource)
        {
            DataTable dt = DbFactory.Current[localSource].ExcuteQuery<T>(SourceData).Result.ToMyDataTable();
            return dt;
        }

        /// <summary>
        /// 实体插入到表方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="SourceData"></param>
        /// <param name="localSource"></param>
        /// <returns></returns>
        public static CallResult ExcuteInsert<T>(this T SourceData, LocalSource localSource)
        {
            CallResult ret = DbFactory.Current[localSource].ExcuteInsert<T>(SourceData);
            return ret;
        }

        /// <summary>
        /// 实体集合插入到表方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="LstSourceData"></param>
        /// <param name="localSource"></param>
        /// <returns></returns>
        public static CallResult ExcuteInsert<T>(this List<T> LstSourceData, LocalSource localSource)
        {
            CallResult ret = DbFactory.Current[localSource].ExcuteInsert<T>(LstSourceData);
            return ret;
        }

        /// <summary>
        /// 实体集合插入到表方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="LstSourceData"></param>
        /// <param name="localSource"></param>
        /// <returns></returns>
        public static CallResult ExcuteInsert<T>(this ObservableCollection<T> LstSourceData, LocalSource localSource)
        {
            CallResult ret = DbFactory.Current[localSource].ExcuteInsert<T>(LstSourceData);
            return ret;
        }

        /// <summary>
        /// 实体更新到表方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="SourceData"></param>
        /// <param name="localSource"></param>
        /// <param name="WhereSection">Where片段特别指定则不取模板</param>
        /// <param name="SafeMode">安全模式</param>
        /// <param name="DefaultValueSet"></param>
        /// <param name="DefaultWriteValue"></param>
        /// <returns></returns>
        public static CallResult ExcuteUpdate<T>(this T SourceData, LocalSource localSource, string WhereSection = "$", bool SafeMode = true, string DefaultWriteValue = null)
        {
            CallResult ret = DbFactory.Current[localSource].ExcuteUpdate<T>(SourceData, WhereSection, SafeMode, DefaultWriteValue);
            return ret;
        }

        /// <summary>
        /// 实体集合更新到表方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="LstSourceData"></param>
        /// <param name="localSource"></param>
        /// <param name="WhereSection">Where片段特别指定则不取模板</param>
        /// <param name="SafeMode">安全模式</param>
        /// <param name="DefaultValueSet"></param>
        /// <param name="DefaultWriteValue"></param>
        /// <returns></returns>
        public static CallResult ExcuteUpdate<T>(this List<T> LstSourceData, LocalSource localSource, string WhereSection = "$", bool SafeMode = true, string DefaultWriteValue = null)
        {
            CallResult ret = DbFactory.Current[localSource].ExcuteUpdate<T>(LstSourceData, WhereSection, SafeMode, DefaultWriteValue);
            return ret;
        }

        /// <summary>
        /// 实体集合更新到表方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="LstSourceData"></param>
        /// <param name="localSource"></param>
        /// <param name="WhereSection">Where片段特别指定则不取模板</param>
        /// <param name="SafeMode">安全模式</param>
        /// <param name="DefaultValueSet"></param>
        /// <param name="DefaultWriteValue"></param>
        /// <returns></returns>
        public static CallResult ExcuteUpdate<T>(this ObservableCollection<T> LstSourceData, LocalSource localSource, string WhereSection = "$", bool SafeMode = true, string DefaultWriteValue = null)
        {
            CallResult ret = DbFactory.Current[localSource].ExcuteUpdate<T>(LstSourceData, WhereSection, SafeMode, DefaultWriteValue);
            return ret;
        }

        /// <summary>
        /// 表记录删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="SourceData"></param>
        /// <param name="localSource"></param>
        /// <returns></returns>
        public static CallResult ExcuteDelete<T>(this T SourceData, LocalSource localSource)
        {
            CallResult ret = DbFactory.Current[localSource].ExcuteDelete<T>(SourceData);
            return ret;
        }
    }
}
