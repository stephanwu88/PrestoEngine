using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Runtime.CompilerServices;

namespace Engine.Data.DBFAC
{
    /// <summary>
    /// 数据插入模式
    /// </summary>
    public enum InsertMode
    {
        /// <summary>
        /// 直接插入
        /// </summary>
        None,
        /// <summary>
        /// 唯一性索引冲突时忽略插入
        /// </summary>
        IngoreInsert,
        /// <summary>
        /// 唯一性索引冲突时转为更新
        /// </summary>
        DuplicateKeyUpdate,
        /// <summary>
        /// 唯一性索引冲突时先删除后差润
        /// </summary>
        ReplaceInsert,
        /// <summary>
        /// 不存在数据项时插入
        /// </summary>
        InsertNotExists
    }

    /// <summary>
    /// 数据库工厂接口
    /// </summary>
    public interface IDBFactory<TConNode> where TConNode : new()
    {
        /// <summary>
        /// 错误发生事件
        /// </summary>
        event Action<object, string> ErrorHappend;
        /// <summary>
        /// 连接状态变化
        /// </summary>
        event Action<object, ConnectionState> ConnectionStateChanged;
        /// <summary>
        /// 连接信息描述
        /// </summary>
        TConNode ConNode { get; set; }
        /// <summary>
        /// 数据库连接对象
        /// </summary>
        IDbConnection DbConn { get; }
        /// <summary>
        /// Command对象
        /// </summary>
        IDbCommand DbCmd { get; }
        /// <summary>
        /// DataReader对象
        /// </summary>
        IDataReader DbDataReader { get; }
        /// <summary>
        /// 连接状态
        /// </summary>
        ConnectionState ConnState { get; }
        /// <summary>
        /// 连接字符串
        /// </summary>
        string ConnString { set; }
        /// <summary>
        /// 错误信息
        /// </summary>
        string LastError { get; }
        /// <summary>
        /// 创建连接对象
        /// </summary>
        /// <returns></returns>
        IDbConnection CreateConnection();
        /// <summary>
        /// 关闭连接
        /// </summary>
        void CloseConnection();
        /// <summary>
        /// 创建Command对象
        /// </summary>
        /// <returns></returns>
        IDbCommand CreateCommand();
        /// <summary>
        /// 创建DataReader对象
        /// </summary>
        /// <param name="strSql"></param>
        /// <returns></returns>
        IDataReader CreateDataReader(string strSql);
        /// <summary>
        /// 查询检索
        /// </summary>
        /// <param name="strSql"></param>
        /// <returns></returns>
        IDataReader Query(string strSql);
        /// <summary>
        /// 执行Sql段
        /// </summary>
        /// <param name="strSql"></param>
        /// <returns></returns>
        CallResult ExcuteSQL(string strSql);
        /// <summary>
        /// 宏指令执行
        /// WriteTableRow(SRC=[localtion_dryer],IF=[GroupKey=MoisDryer & PosID=16],THEN=[MaterLocation={0}])
        /// </summary>
        /// <param name="LstMacrolExpress"></param>
        /// <returns></returns>
        CallResult ExcuteMacroCommand(List<string> LstMacrolExpress);
        /// <summary>
        /// 宏指令查询
        /// ReadTableRow(SRC=[location_group],IF=[GroupKey=RockSieve & PosID=1],RET=[MarkKey])
        /// </summary>
        /// <param name="MacrolQueryExpress"></param>
        /// <returns></returns>
        CallResult ExcuteMacroQuery(string MacrolQueryExpress);
        /// <summary>
        /// 获取指定数据表
        /// </summary>
        /// <param name="strSql"></param>
        /// <returns></returns>
        CallResult ExcuteQuery(string strSql);
        /// <summary>
        /// 获取指定对象的数据表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="SourceData"></param>
        /// <param name="OrderBy"></param>
        /// <returns></returns>
        CallResult ExcuteQuery<T>(T SourceData, string OrderBy = "");
        /// <summary>
        /// 实体插入到表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="SourceData"></param>
        /// <param name="InsertMode"></param>
        /// <returns></returns>
        CallResult ExcuteInsert<T>(T SourceData, InsertMode InsertMode = InsertMode.None);
        /// <summary>
        /// 实体集合插入到表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="LstSourceData"></param>
        /// <param name="InsertMode"></param>
        /// <returns></returns>
        CallResult ExcuteInsert<T>(List<T> LstSourceData, InsertMode InsertMode = InsertMode.None);
        /// <summary>
        /// 实体集合插入到表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="LstSourceData"></param>
        /// <param name="InsertMode"></param>
        /// <returns></returns>
        CallResult ExcuteInsert<T>(ObservableCollection<T> LstSourceData, InsertMode InsertMode = InsertMode.None);
        /// <summary>
        /// 实体更新到表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="SourceData"></param>
        /// <param name="WhereSection">Where片段特别指定则不取模板</param>
        /// <param name="SafeMode">安全模式</param>
        /// <param name="DefaultWriteValue"></param>
        /// <returns></returns>
        CallResult ExcuteUpdate<T>(T SourceData, string WhereSection = "$", bool SafeMode = true, string DefaultWriteValue = null);
        /// <summary>
        /// 实体集合更新到表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="LstSourceData"></param>
        /// <param name="WhereSection">Where片段特别指定则不取模板</param>
        /// <param name="SafeMode">安全模式</param>
        /// <param name="DefaultWriteValue"></param>
        /// <returns></returns>
        CallResult ExcuteUpdate<T>(List<T> LstSourceData, string WhereSection = "$", bool SafeMode = true, string DefaultWriteValue = null);
        /// <summary>
        /// 实体集合更新到表方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="LstSourceData"></param>
        /// <param name="WhereSection">Where片段特别指定则不取模板</param>
        /// <param name="SafeMode">安全模式</param>
        /// <param name="DefaultWriteValue"></param>
        /// <returns></returns>
        CallResult ExcuteUpdate<T>(ObservableCollection<T> LstSourceData, string WhereSection = "$", bool SafeMode = true, string DefaultWriteValue = null);
        /// <summary>
        /// 更新到表方法
        /// </summary>
        /// <param name="Table">表名</param>
        /// <param name="SectionUpdate">表达式</param>
        /// <param name="SectionWhere">表达式</param>
        /// <returns></returns>
        CallResult ExcuteUpdate(string Table, List<string> SectionUpdate, List<string> SectionWhere);
        /// <summary>
        /// 表记录删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="SourceData"></param>
        /// <param name="SafeMode"></param>
        /// <returns></returns>
        CallResult ExcuteDelete<T>(T SourceData, bool SafeMode = true);
        /// <summary>
        /// 表记录集删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="LstSourceData"></param>
        /// <param name="SafeMode"></param>
        /// <returns></returns>
        CallResult ExcuteDelete<T>(List<T> LstSourceData, bool SafeMode = true);
        /// <summary>
        /// 表记录集删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="LstSourceData"></param>
        /// <param name="SafeMode"></param>
        /// <returns></returns>
        CallResult ExcuteDelete<T>(ObservableCollection<T> LstSourceData, bool SafeMode = true);
        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="strProcName">过程名称</param>
        /// <returns></returns>
        CallResult ExcuteProcedure(string strProcName);
        /// <summary>
        /// 获取数据库所有表名
        /// </summary>
        /// <returns></returns>
        List<string> GetTablesName();
        /// <summary>
        /// 表查询实体 - SQL字符串 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="OrderBy"></param>
        /// <returns></returns>
        string SqlQuery<T>(T data, string OrderBy = "");
        /// <summary>
        /// 表插入实体 - SQL字符串 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="InsertMode"></param>
        /// <returns></returns>
        CallResult SqlInsert<T>(T data, InsertMode InsertMode = InsertMode.None);
        /// <summary>
        /// 表插入实体列表 - SQL字符串 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="LstData"></param>
        /// <param name="InsertMode"></param>
        /// <returns></returns>
        CallResult SqlInsert<T>(List<T> LstData, InsertMode InsertMode = InsertMode.None);
        /// <summary>
        /// 表更新实体 - SQL字符串 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="WhereSection"></param>
        /// <param name="SafeMode"></param>
        /// <param name="DefaultWriteValue"></param>
        /// <returns></returns>
        CallResult SqlUpdate<T>(T data, string WhereSection = "$", bool SafeMode = true, string DefaultWriteValue = null);
        /// <summary>
        /// 表更新实体列表 - SQL字符串 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="LstData"></param>
        /// <param name="WhereSection"></param>
        /// <param name="SafeMode"></param>
        /// <param name="DefaultWriteValue"></param>
        /// <returns></returns>
        CallResult SqlUpdate<T>(List<T> LstData, string WhereSection = "$", bool SafeMode = true, string DefaultWriteValue = null);
        /// <summary>
        /// 表行删除实体 - SQL字符串 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="SafeMode"></param>
        /// <returns></returns>
        CallResult SqlDelete<T>(T data, bool SafeMode = true);
        /// <summary>
        /// 表行删除实体列表 - SQL字符串 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="LstData"></param>
        /// <param name="SafeMode"></param>
        /// <returns></returns>
        CallResult SqlDelete<T>(List<T> LstData, bool SafeMode = true);
        /// <summary>
        /// 数据库事务操作
        /// </summary>
        /// <param name="ListSql"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        CallResult CmdByTran(List<string> ListSql);
    }
}
