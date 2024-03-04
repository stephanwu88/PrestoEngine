using Engine.Access;
using Engine.Common;
using Engine.Data.DBFAC;
using Engine.Mod;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Runtime.CompilerServices;
using System.Reflection;
using System.Threading;
using Oracle.ManagedDataAccess.Client;

namespace Engine.Data.Oracle
{
    /// <summary>
    /// Oracle数据库类
    /// </summary>
    public class DBOracle : IDBFactory<ServerNode>
    {
        #region 内部成员
        private OracleConnection _OracleConn;
        private OracleCommand _OracleCommand;
        private OracleDataReader _OracleDataReader;
        private string _ConnString;
        private bool StateConnected;
        private bool RestartConnect;
        private ConnectionState _ConnStateTemp = ConnectionState.Broken;
        private ServerNode _ConNode;
        private string _LastError = "";
        /// <summary>
        /// 错误发生事件
        /// </summary>
        public event Action<object, string> ErrorHappend;
        /// <summary>
        /// 连接状态变化
        /// </summary>
        public event Action<object, ConnectionState> ConnectionStateChanged;
        /// <summary>
        /// 连接信息描述
        /// </summary>
        public ServerNode ConNode
        {
            get => _ConNode;
            set
            {
                _ConNode = value;
                CreateConnectString(_ConNode);
            }
        }
        /// <summary>
        /// 数据库连接对象
        /// </summary>
        public IDbConnection DbConn => CreateConnection();
        /// <summary>
        /// Command对象
        /// </summary>
        public IDbCommand DbCmd => CreateCommand();
        /// <summary>
        /// DataReader对象
        /// </summary>
        public IDataReader DbDataReader => _OracleDataReader;
        /// <summary>
        /// 连接状态
        /// </summary>
        public ConnectionState ConnState
        {
            get
            {
                if (_OracleConn != null)
                    return _OracleConn.State;
                else
                    return ConnectionState.Broken;
            }
        }
        /// <summary>
        /// 连接字符串
        /// </summary>
        public string ConnString
        {
            set { _ConnString = value; }
        }
        /// <summary>
        /// 错误信息
        /// </summary>
        public string LastError => _LastError;
        #endregion

        #region 公共方法
        /// <summary>
        /// 构造函数
        /// </summary>
        public DBOracle()
        {
            if (_OracleConn == null)
                _OracleConn = new OracleConnection();
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="node">服务器信息</param>
        public DBOracle(ServerNode serverNode = null)
        {
            if (_OracleConn == null)
            {
                _OracleConn = new OracleConnection();
                _LastError = string.Empty;
            }
            CreateConnectString(serverNode);
        }
        /// <summary>
        /// 创建连接对象
        /// </summary>
        /// <returns></returns>
        public IDbConnection CreateConnection()
        {
            try
            {
                if (_OracleConn.State != ConnectionState.Open)
                {
                    if (_OracleConn.ConnectionString != _ConnString)
                        _OracleConn.ConnectionString = _ConnString;
                    _OracleConn.Open();
                }
                ChangeState(_OracleConn.State);
                //if (!StateConnected)
                //    ThreadPool.QueueUserWorkItem(WatchConnection, null);
                return _OracleConn;
            }
            catch (OracleException ex)
            {
                if (_LastError != ex.Message)
                {
                    _OracleConn.Close();
                    _LastError = ex.Message;
                    Logger.Error.Write(LOG_TYPE.ERROR, string.Format("【Oracle Connect Fail】 {0}", ex.Message));
                }
                ChangeState(_OracleConn.State);
                //if (!StateConnected)
                //    ThreadPool.QueueUserWorkItem(WatchConnection, null);
                return null;
            }
        }

        /// <summary>
        /// 保持长连接
        /// </summary>
        /// <param name="state"></param>
        private void WatchConnection(object state)
        {
            if (_OracleConn.State == ConnectionState.Open)
            {
                StateConnected = true;
                RestartConnect = false;
            }
            else
            {
                StateConnected = false;
                RestartConnect = true;
            }
            while (StateConnected)
            {
                if (_OracleConn.State == ConnectionState.Closed)
                {
                    StateConnected = false;
                    RestartConnect = true;
                }
                Thread.Sleep(1000);
            }
            if (RestartConnect && !StateConnected)
            {
                Thread.Sleep(1000);
                RestartConnect = false;
                CreateConnection();
            }
        }

        /// <summary>
        /// 关闭连接
        /// </summary>
        public void CloseConnection()
        {
            if (_OracleConn.State != ConnectionState.Closed)
            {
                _OracleConn.Close();
                _OracleConn.Dispose();
                OracleConnection.ClearPool(_OracleConn);
            }
            ChangeState(_OracleConn.State);
        }

        /// <summary>
        /// 创建Command对象
        /// </summary>
        /// <returns></returns>
        public IDbCommand CreateCommand()
        {
            CreateConnection();
            if (_OracleCommand == null)
                _OracleCommand = _OracleConn.CreateCommand();
            return _OracleCommand;
        }

        /// <summary>
        /// 创建DataReader对象
        /// </summary>
        /// <param name="strCmd"></param>
        /// <returns></returns>
        public IDataReader CreateDataReader(string strSql)
        {
            CreateCommand();
            OracleDataReader dr = null;
            if (_OracleCommand != null)
            {
                _OracleCommand.CommandText = strSql;
                dr = _OracleCommand.ExecuteReader();
            }
            return dr;
        }

        /// <summary>
        /// 检索
        /// </summary>
        /// <param name="strSql"></param>
        /// <returns></returns>
        public IDataReader Query(string strSql)
        {
            CallResult ret = CmdBySql("select", strSql);
            return ret.Success ? _OracleDataReader : null;
        }

        /// <summary>
        /// 执行Sql段
        /// </summary>
        /// <param name="strSql"></param>
        /// <returns></returns>
        public CallResult ExcuteSQL(string strSql)
        {
            string str = strSql.ToLower();
            if (!str.Contains("begin") && !str.Contains("end"))
                strSql = string.Format("begin {0} end;", strSql);
            return CmdBySql("exe", strSql);
        }

        /// <summary>
        /// 宏指令执行
        /// WriteTableRow(SRC=[localtion_dryer],IF=[GroupKey=MoisDryer & PosID=16],THEN=[MaterLocation={0}])
        /// </summary>
        /// <param name="LstMacrolExpress"></param>
        /// <returns></returns>
        public CallResult ExcuteMacroCommand(List<string> LstMacrolExpress)
        {
            string strSql = string.Empty;
            foreach (string exp in LstMacrolExpress)
            {
                strSql += ParseLogicExpress(exp);
            }
            return CmdBySql("exe", strSql);
        }

        /// <summary>
        /// 宏指令查询
        /// ReadTableRow(SRC=[location_group],IF=[GroupKey=RockSieve & PosID=1],RET=[MarkKey])
        /// </summary>
        /// <param name="MacrolQueryExpress"></param>
        /// <returns></returns>
        public CallResult ExcuteMacroQuery(string MacrolQueryExpress)
        {
            string strSql = string.Empty;
            CallResult ret = new CallResult();
            strSql += ParseLogicExpress(MacrolQueryExpress);
            string strRetField = MacrolQueryExpress.MidString("RET=[", "]", EndStringSearchMode.FromTail).Trim();
            ret = CmdBySql("select", strSql);
            if (ret.Fail)
                return ret;
            DataTable dt = ret.Result.ToMyDataTable();
            if (dt.Rows.Count == 0)
            {
                ret.Result = "";
                return ret;
            }
            if (!dt.Columns.Contains(strRetField) && strRetField.IsNotEmpty())
            {
                ret.Success = false;
                ret.Result = string.Format("RET字段【{0}】不存在", strRetField);
                return ret;
            }
            if (strRetField.IsEmpty())
                ret.Result = dt.Rows[0];
            else
                ret.Result = dt.Rows[0][strRetField].ToMyString();
            return ret;
        }

        /// <summary>
        /// 获取数据表
        /// </summary>
        /// <param name="strSql"></param>
        /// <returns></returns>
        public CallResult ExcuteQuery(string strSql)
        {
            CallResult ret = CmdBySql("select", strSql);
            return ret;
        }

        /// <summary>
        /// 获取指定对象的数据表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="SourceData"></param>
        /// <param name="OrderBy"></param>
        /// <returns></returns>
        public CallResult ExcuteQuery<T>(T SourceData, string OrderBy = "")
        {
            CallResult ret = new CallResult() { Success = false, Result = new DataTable() };
            if (SourceData != null)
            {
                string strSql = SqlQuery<T>(SourceData,OrderBy);
                if (!string.IsNullOrEmpty(strSql))
                    ret = ExcuteQuery(strSql);
            }
            return ret;
        }

        /// <summary>
        /// 实体插入到表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="SourceData"></param>
        /// <param name="InsertMode"></param>
        /// <returns></returns>
        public CallResult ExcuteInsert<T>(T SourceData, InsertMode InsertMode = InsertMode.None)
        {
            CallResult ret = new CallResult() { Success = false };
            if (SourceData == null) { ret.Result = "实体参数为NULL"; return ret; }
            CallResult SqlReq = SqlInsert<T>(SourceData, InsertMode);
            if (SqlReq.Fail) { ret.Result = SqlReq.Result; return ret; }
            return CmdBySql("exe", SqlReq.Result.ToMyString());
        }

        /// <summary>
        /// 实体集合插入到表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="LstSourceData"></param>
        /// <param name="InsertMode"></param>
        /// <returns></returns>
        public CallResult ExcuteInsert<T>(List<T> LstSourceData, InsertMode InsertMode = InsertMode.None)
        {
            CallResult ret = new CallResult() { Success = false };
            if (LstSourceData == null) { ret.Result = "实体参数为NULL"; return ret; }
            string strSql = string.Empty;
            foreach (T data in LstSourceData)
            {
                CallResult SqlReq = SqlInsert<T>(data, InsertMode);
                if (SqlReq.Fail)
                {
                    ret.Result = SqlReq.Result;
                    return ret;
                }
                strSql += SqlReq.Result.ToMyString();
            }
            return CmdBySql("exe", strSql);
        }

        /// <summary>
        /// 实体集合插入到表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="LstSourceData"></param>
        /// <param name="InsertMode"></param>
        /// <returns></returns>
        public CallResult ExcuteInsert<T>(ObservableCollection<T> LstSourceData, InsertMode InsertMode = InsertMode.None)
        {
            CallResult ret = new CallResult() { Success = false };
            if (LstSourceData == null) { ret.Result = "实体参数为NULL"; return ret; }
            string strSql = string.Empty;
            foreach (T data in LstSourceData)
            {
                CallResult SqlReq = SqlInsert<T>(data, InsertMode);
                if (SqlReq.Fail)
                {
                    ret.Result = SqlReq.Result;
                    return ret;
                }
                strSql += SqlReq.Result.ToMyString();
            }
            return CmdBySql("exe", strSql);
        }

        /// <summary>
        /// 实体更新到表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="SourceData"></param>
        /// <param name="WhereSection">Where片段特别指定则不取模板</param>
        /// <param name="SafeMode">安全模式</param>
        /// <param name="DefaultWriteValue"></param>
        /// <returns></returns>
        public CallResult ExcuteUpdate<T>(T SourceData, string WhereSection = "$", bool SafeMode = true, string DefaultWriteValue = null)
        {
            CallResult ret = new CallResult() { Success = false, Result = "" };
            if (SourceData == null) { ret.Result = "实体参数为NULL"; return ret; }
            CallResult SqlReq = SqlUpdate<T>(SourceData, WhereSection, SafeMode, DefaultWriteValue);
            if (SqlReq.Fail) { ret.Result = SqlReq.Result; return ret; }
            return CmdBySql("exe", SqlReq.Result.ToMyString());
        }

        /// <summary>
        /// 实体集合更新到表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="LstSourceData"></param>
        /// <param name="WhereSection">Where片段特别指定则不取模板</param>
        /// <param name="SafeMode">安全模式</param>
        /// <param name="DefaultWriteValue"></param>
        /// <returns></returns>
        public CallResult ExcuteUpdate<T>(List<T> LstSourceData, string WhereSection = "$", bool SafeMode = true, string DefaultWriteValue = null)
        {
            CallResult ret = new CallResult() { Success = false, Result = "" };
            if (LstSourceData == null) { ret.Result = "实体参数为NULL"; return ret; }
            string strSql = string.Empty;
            foreach (T data in LstSourceData)
            {
                CallResult SqlReq = SqlUpdate<T>(data, WhereSection, SafeMode, DefaultWriteValue);
                if (SqlReq.Fail)
                {
                    ret.Result = SqlReq.Result;
                    return ret;
                }
                strSql += SqlReq.Result.ToMyString();
            }
            return CmdBySql("exe", strSql);
        }

        /// <summary>
        /// 实体集合更新到表方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="LstSourceData"></param>
        /// <param name="WhereSection">Where片段特别指定则不取模板</param>
        /// <param name="SafeMode">安全模式</param>
        /// <param name="DefaultWriteValue"></param>
        /// <returns></returns>
        public CallResult ExcuteUpdate<T>(ObservableCollection<T> LstSourceData, string WhereSection = "$", bool SafeMode = true, string DefaultWriteValue = null)
        {
            CallResult ret = new CallResult() { Success = false, Result = "" };
            if (LstSourceData == null) { ret.Result = "实体参数为NULL"; return ret; }
            string strSql = string.Empty;
            foreach (T data in LstSourceData)
            {
                CallResult SqlReq = SqlUpdate<T>(data, WhereSection, SafeMode, DefaultWriteValue);
                if (SqlReq.Fail)
                {
                    ret.Result = SqlReq.Result;
                    return ret;
                }
                strSql += SqlReq.Result.ToMyString();
            }
            return CmdBySql("exe", strSql);
        }

        /// <summary>
        /// 更新到表方法
        /// </summary>
        /// <param name="Table">表名</param>
        /// <param name="SectionUpdate">表达式</param>
        /// <param name="SectionWhere">表达式</param>
        /// <returns></returns>
        public CallResult ExcuteUpdate(string Table, List<string> SectionUpdate, List<string> SectionWhere)
        {
            string strUpdate = SqlMaker.SectionUpdate(SectionUpdate);
            string strWhere = SqlMaker.SectionWhere(SectionWhere);
            string strSql = SqlMaker.SqlUpdate(Table, strUpdate, strWhere);
            return CmdBySql("exe", strSql);
        }

        /// <summary>
        /// 表记录删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="SourceData"></param>
        /// <param name="SafeMode"></param>
        /// <returns></returns>
        public CallResult ExcuteDelete<T>(T SourceData, bool SafeMode = true)
        {
            CallResult ret = new CallResult() { Success = false, Result = "" };
            if (SourceData == null) { ret.Result = "实体参数为NULL"; return ret; }
            CallResult SqlReq = SqlDelete<T>(SourceData, SafeMode);
            if (SqlReq.Fail) { ret.Result = SqlReq.Result; return ret; }
            return CmdBySql("exe", SqlReq.Result.ToMyString());
        }

        /// <summary>
        /// 表记录集删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="LstSourceData"></param>
        /// <param name="SafeMode"></param>
        /// <returns></returns>
        public CallResult ExcuteDelete<T>(List<T> LstSourceData, bool SafeMode = true)
        {
            CallResult ret = new CallResult() { Success = false, Result = "" };
            if (LstSourceData == null) { ret.Result = "实体参数为NULL"; return ret; }
            string strSql = string.Empty;
            foreach (T data in LstSourceData)
            {
                CallResult SqlReq = SqlDelete<T>(data, SafeMode);
                if (SqlReq.Fail)
                {
                    ret.Result = SqlReq.Result;
                    return ret;
                }
                strSql += SqlReq.Result.ToMyString();
            }
            return CmdBySql("exe", strSql);
        }

        /// <summary>
        /// 表记录集删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="LstSourceData"></param>
        /// <param name="SafeMode"></param>
        /// <returns></returns>
        public CallResult ExcuteDelete<T>(ObservableCollection<T> LstSourceData, bool SafeMode = true)
        {
            CallResult ret = new CallResult() { Success = false, Result = "" };
            if (LstSourceData == null) { ret.Result = "实体参数为NULL"; return ret; }
            string strSql = string.Empty;
            foreach (T data in LstSourceData)
            {
                CallResult SqlReq = SqlDelete<T>(data, SafeMode);
                if (SqlReq.Fail)
                {
                    ret.Result = SqlReq.Result;
                    return ret;
                }
                strSql += SqlReq.Result.ToMyString();
            }
            return CmdBySql("exe", strSql);
        }

        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="strProcName"></param>
        /// <returns></returns>
        public CallResult ExcuteProcedure(string strProcName)
        {
            return CmdBySql("exe", strProcName, CommandType.StoredProcedure);
        }

        /// <summary>
        /// 获取数据库所有表名
        /// </summary>
        /// <returns></returns>
        public List<string> GetTablesName()
        {
            List<string> TablesName = new List<string>();
            return TablesName;
        }

        /// <summary>
        /// 表查询方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="OrderBy"></param>
        /// <returns></returns>
        public string SqlQuery<T>(T data, string OrderBy = "")
        {
            Type ObjType = typeof(T);
            //Type ObjType = data.GetType();
            string strSql = string.Empty;
            string strWhere = string.Empty;
            string strTableName = string.Empty;
            TableDef tableDef = TableAttribute.Table<T>();
            if (tableDef == null)
                return string.Empty;
            strTableName = string.IsNullOrEmpty(tableDef.ViewName) ? tableDef.Name : tableDef.ViewName;
            foreach (PropertyInfo pi in ObjType.GetProperties())
            {
                object piValue = pi.GetPropValue(data);
                tableDef = TableAttribute.Table<T>(pi.Name);
                if (tableDef != null && !string.IsNullOrEmpty(piValue.ToMyString()))
                    strTableName = piValue.ToMyString();
                ColumnDef colDef = ColumnAttribute.Column<T>(pi.Name);
                if (colDef == null)
                    continue;
                Type piType = pi.PropertyType;
                if (piType.IsGenericType && piType.GetGenericTypeDefinition() == typeof(Dictionary<,>))
                {
                    //字典集合
                    Type keyType = piType.GetGenericArguments()[0];   //键的类型
                    Type valueType = piType.GetGenericArguments()[1]; //值的类型
                    if (keyType != typeof(string))
                        continue;
                    if (piValue is Dictionary<string, string> strDic)
                        AppandWhereSection(strDic, ref strWhere);
                    else if (piValue is Dictionary<string, int> intDic)
                        AppandWhereSection(intDic, ref strWhere);
                    else if (piValue is Dictionary<string, double> doubleDic)
                        AppandWhereSection(doubleDic, ref strWhere);
                    else if (piValue is Dictionary<string, float> floatDic)
                        AppandWhereSection(floatDic, ref strWhere);
                    else if (piValue is Dictionary<string, object> objDic)
                        AppandWhereSection(objDic, ref strWhere);
                }
                else
                {
                    string strDbFieldContent = string.Empty;
                    string strDbFieldVal = string.Empty;
                    bool ret = GetFieldValue<T>(data, pi, ref colDef, ref strDbFieldContent, ref strDbFieldVal);
                    if (!ret || colDef == null || string.IsNullOrEmpty(strDbFieldVal))
                        continue;
                    AppandWhereSection(colDef, strDbFieldContent, ref strWhere);
                }
            }
            if (!string.IsNullOrEmpty(strWhere))
                strWhere = string.Format(" where {0}", strWhere);
            if (!string.IsNullOrEmpty(strTableName))
            {
                string strOrderBy = string.Empty;
                if (!strWhere.MyContains("Order|by", "|", StringMatchMode.IgnoreCase) && !string.IsNullOrEmpty(OrderBy))
                {
                    strOrderBy = $" order by {OrderBy}";
                }
                //此处注意，单条sql语句，结尾不能用; ,会报错，ORA-00933: SQL 命令未正确结束
                strSql = string.Format("select * from {0} {1} {2}", strTableName, strWhere, strOrderBy);
            }
            return strSql;
        }

        /// <summary>
        /// 表插入方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="InsertMode"></param>
        /// <returns></returns>
        public CallResult SqlInsert<T>(T data, InsertMode InsertMode = InsertMode.None)
        {
            CallResult Res = new CallResult() { Success = false, Result = "" };
            Type ObjType = typeof(T);
            //Type ObjType = data.GetType();
            string strInsertField = string.Empty;
            string strInsertValue = string.Empty;
            string strTableName = string.Empty;
            TableDef tableDef = TableAttribute.Table<T>();
            if (tableDef == null) { Res.Result = "未解析到表名称"; return Res; }
            strTableName = tableDef.Name;
            foreach (PropertyInfo pi in ObjType.GetProperties())
            {
                object piValue = pi.GetPropValue(data);
                tableDef = TableAttribute.Table<T>(pi.Name);
                if (tableDef != null && !string.IsNullOrEmpty(piValue.ToMyString()))
                    strTableName = piValue.ToMyString();
                ColumnDef colDef = ColumnAttribute.Column<T>(pi.Name);
                if (colDef == null)
                    continue;
                Type piType = pi.PropertyType;
                if (piType.IsGenericType && piType.GetGenericTypeDefinition() == typeof(Dictionary<,>))
                {
                    //字典集合
                    Type keyType = piType.GetGenericArguments()[0];   //键的类型
                    Type valueType = piType.GetGenericArguments()[1]; //值的类型
                    if (keyType != typeof(string))
                        continue;
                    if (piValue is Dictionary<string, string> strDic)
                        AppandInsertSection(strDic, ref strInsertField, ref strInsertValue);
                    else if (piValue is Dictionary<string, int> intDic)
                        AppandInsertSection(intDic, ref strInsertField, ref strInsertValue);
                    else if (piValue is Dictionary<string, double> doubleDic)
                        AppandInsertSection(doubleDic, ref strInsertField, ref strInsertValue);
                    else if (piValue is Dictionary<string, float> floatDic)
                        AppandInsertSection(floatDic, ref strInsertField, ref strInsertValue);
                    else if (piValue is Dictionary<string, object> objDic)
                        AppandInsertSection(objDic, ref strInsertField, ref strInsertValue);
                }
                else
                {
                    string strDbFieldContent = string.Empty;
                    string strDbFieldVal = string.Empty;
                    bool ret = GetFieldValue<T>(data, pi, ref colDef, ref strDbFieldContent, ref strDbFieldVal);
                    if (!ret || colDef == null || string.IsNullOrEmpty(strDbFieldVal))
                        continue;
                    if (colDef.AI || colDef.ReadOnly)
                        continue;
                    AppandInsertSection(colDef, strDbFieldContent, ref strInsertField, ref strInsertValue);
                }
            }
            if (!string.IsNullOrEmpty(strTableName) && !string.IsNullOrEmpty(strInsertField) && !string.IsNullOrEmpty(strInsertValue))
            {
                Res.Success = true;
                Res.Result = string.Format("insert into {0}({1}) values({2});", strTableName, strInsertField, strInsertValue);
            }
            return Res;
        }

        /// <summary>
        /// 表插入实体列表 - SQL字符串 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="LstData"></param>
        /// <param name="InsertMode"></param>
        /// <returns></returns>
        public CallResult SqlInsert<T>(List<T> LstData, InsertMode InsertMode = InsertMode.None)
        {
            CallResult Res = new CallResult() { Success = false, Result = "" };
            if (LstData == null) { Res.Result = "实体参数为NULL"; return Res; }
            string strSql = string.Empty;
            foreach (T data in LstData)
            {
                CallResult SqlReq = SqlInsert<T>(data, InsertMode);
                if (SqlReq.Fail)
                {
                    Res.Result = SqlReq.Result;
                    return Res;
                }
                strSql += SqlReq.Result.ToMyString();
            }
            Res.Success = true;
            Res.Result = strSql;
            return Res;
        }

        /// <summary>
        /// 表更新方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="WhereSection"></param>
        /// <param name="SafeMode">安全模式</param>
        /// <param name="DefaultValueSet"></param>
        /// <param name="DefaultWriteValue"></param>
        /// <returns></returns>
        public CallResult SqlUpdate<T>(T data, string WhereSection = "$", bool SafeMode = true, string DefaultWriteValue = null)
        {
            CallResult Res = new CallResult() { Success = false, Result = "" };
            Type ObjType = typeof(T);
            //Type ObjType = data.GetType();
            string strUpdate = string.Empty;
            string strWhere = string.Empty;
            string strTableName = string.Empty;
            TableDef tableDef = TableAttribute.Table<T>();
            if (tableDef == null) { Res.Result = "未解析到表名称"; return Res; }
            strTableName = tableDef.Name;
            foreach (PropertyInfo pi in ObjType.GetProperties())
            {
                object piValue = pi.GetPropValue(data);
                tableDef = TableAttribute.Table<T>(pi.Name);
                if (tableDef != null && !string.IsNullOrEmpty(piValue.ToMyString()))
                    strTableName = piValue.ToMyString();
                ColumnDef colDef = ColumnAttribute.Column<T>(pi.Name);
                if (colDef == null)
                    continue;
                Type piType = pi.PropertyType;
                if (piType.IsGenericType && piType.GetGenericTypeDefinition() == typeof(Dictionary<,>))
                {
                    //字典集合
                    Type keyType = piType.GetGenericArguments()[0];   //键的类型
                    Type valueType = piType.GetGenericArguments()[1]; //值的类型
                    if (keyType != typeof(string))
                        continue;
                    if (piValue is Dictionary<string, string> strDic)
                        AppandUpdateSection(strDic, ref strUpdate, ref strWhere);
                    else if (piValue is Dictionary<string, int> intDic)
                        AppandUpdateSection(intDic, ref strUpdate, ref strWhere);
                    else if (piValue is Dictionary<string, double> doubleDic)
                        AppandUpdateSection(doubleDic, ref strUpdate, ref strWhere);
                    else if (piValue is Dictionary<string, float> floatDic)
                        AppandUpdateSection(floatDic, ref strUpdate, ref strWhere);
                    else if (piValue is Dictionary<string, object> objDic)
                        AppandUpdateSection(objDic, ref strUpdate, ref strWhere);
                }
                else
                {
                    string strDbFieldContent = string.Empty;
                    string strDbFieldVal = string.Empty;
                    bool ret = GetFieldValue<T>(data, pi, ref colDef, ref strDbFieldContent, ref strDbFieldVal);
                    if (!ret || colDef == null || string.IsNullOrEmpty(strDbFieldVal))
                        continue;
                    if (colDef.ReadOnly)
                        continue;
                    AppandUpdateSection(colDef, strDbFieldContent, ref strUpdate);
                    AppandWhereSection(colDef, strDbFieldContent, ref strWhere, true);
                }
            }
            if (WhereSection == "$" && !string.IsNullOrEmpty(strWhere))
                strWhere = string.Format(" where {0}", strWhere);
            else if (WhereSection != "$")
                strWhere = string.Format(" where {0}", WhereSection.ToLower().MidString("where", ""));
            if (string.IsNullOrEmpty(strWhere) && SafeMode) { Res.Result = "安全模式下阻止更新"; return Res; }
            else if (string.IsNullOrEmpty(strUpdate)) { Res.Result = "未解析到更新内容"; return Res; }
            if (!string.IsNullOrEmpty(strTableName) && !string.IsNullOrEmpty(strUpdate) && ((!string.IsNullOrEmpty(strWhere) && SafeMode) || !SafeMode))
            {
                Res.Success = true;
                Res.Result = string.Format("update {0} set {1} {2};", strTableName, strUpdate, strWhere);
            }
            return Res;
        }

        /// <summary>
        /// 表更新实体列表 - SQL字符串 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="LstData"></param>
        /// <param name="WhereSection"></param>
        /// <param name="SafeMode"></param>
        /// <param name="DefaultWriteValue"></param>
        /// <returns></returns>
        public CallResult SqlUpdate<T>(List<T> LstData, string WhereSection = "$", bool SafeMode = true, string DefaultWriteValue = null)
        {
            CallResult Res = new CallResult() { Success = false, Result = "" };
            if (LstData == null) { Res.Result = "实体参数为NULL"; return Res; }
            string strSql = string.Empty;
            foreach (T data in LstData)
            {
                CallResult SqlReq = SqlUpdate<T>(data, WhereSection, SafeMode, DefaultWriteValue);
                if (SqlReq.Fail)
                {
                    Res.Result = SqlReq.Result;
                    return Res;
                }
                strSql += SqlReq.Result.ToMyString();
            }
            Res.Success = true;
            Res.Result = strSql;
            return Res;
        }

        /// <summary>
        /// 表记录行删除方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="SafeMode">安全模式</param>
        /// <returns></returns>
        public CallResult SqlDelete<T>(T data, bool SafeMode = true)
        {
            CallResult Res = new CallResult() { Success = false, Result = "" };
            Type ObjType = typeof(T);
            //Type ObjType = data.GetType();
            string strWhere = string.Empty;
            string strTableName = string.Empty;
            TableDef tableDef = TableAttribute.Table<T>();
            if (tableDef == null) { Res.Result = "未解析到表名称"; return Res; }
            strTableName = tableDef.Name;
            foreach (PropertyInfo pi in ObjType.GetProperties())
            {
                object piValue = pi.GetPropValue(data);
                tableDef = TableAttribute.Table<T>(pi.Name);
                if (tableDef != null && !string.IsNullOrEmpty(piValue.ToMyString()))
                    strTableName = piValue.ToMyString();
                ColumnDef colDef = ColumnAttribute.Column<T>(pi.Name);
                if (colDef == null)
                    continue;
                Type piType = pi.PropertyType;
                if (piType.IsGenericType && piType.GetGenericTypeDefinition() == typeof(Dictionary<,>))
                {
                    //字典集合
                    Type keyType = piType.GetGenericArguments()[0];   //键的类型
                    Type valueType = piType.GetGenericArguments()[1]; //值的类型
                    if (keyType != typeof(string))
                        continue;
                    if (piValue is Dictionary<string, string> strDic)
                        AppandWhereSection(strDic, ref strWhere);
                    else if (piValue is Dictionary<string, int> intDic)
                        AppandWhereSection(intDic, ref strWhere);
                    else if (piValue is Dictionary<string, double> doubleDic)
                        AppandWhereSection(doubleDic, ref strWhere);
                    else if (piValue is Dictionary<string, float> floatDic)
                        AppandWhereSection(floatDic, ref strWhere);
                    else if (piValue is Dictionary<string, object> objDic)
                        AppandWhereSection(objDic, ref strWhere);
                }
                else
                {
                    string strDbFieldContent = string.Empty;
                    string strDbFieldVal = string.Empty;
                    bool ret = GetFieldValue<T>(data, pi, ref colDef, ref strDbFieldContent, ref strDbFieldVal);
                    if (!ret || colDef == null || string.IsNullOrEmpty(strDbFieldVal))
                        continue;
                    if (colDef.ReadOnly)
                        continue;
                    AppandWhereSection(colDef, strDbFieldContent, ref strWhere);
                }
            }
            if (!string.IsNullOrEmpty(strWhere))
                strWhere = string.Format(" where {0}", strWhere);
            if (string.IsNullOrEmpty(strWhere) && SafeMode) { Res.Result = "安全模式下阻止删除"; return Res; }
            if (!string.IsNullOrEmpty(strTableName) && ((!string.IsNullOrEmpty(strWhere) && SafeMode) || !SafeMode))
            {
                Res.Success = true;
                Res.Result = string.Format("delete from {0} {1};", strTableName, strWhere);
            }
            return Res;
        }

        /// <summary>
        /// 表行删除实体列表 - SQL字符串 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="LstData"></param>
        /// <param name="SafeMode"></param>
        /// <returns></returns>
        public CallResult SqlDelete<T>(List<T> LstData, bool SafeMode = true)
        {
            CallResult Res = new CallResult() { Success = false, Result = "" };
            if (LstData == null) { Res.Result = "实体参数为NULL"; return Res; }
            string strSql = string.Empty;
            foreach (T data in LstData)
            {
                CallResult SqlReq = SqlDelete<T>(data, SafeMode);
                if (SqlReq.Fail)
                {
                    Res.Result = SqlReq.Result;
                    return Res;
                }
                strSql += SqlReq.Result.ToMyString();
            }
            Res.Success = true;
            Res.Result = strSql;
            return Res;
        }

        /// <summary>
        /// 数据库事务
        /// </summary>
        /// <param name="ListSql"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public CallResult CmdByTran(List<string> ListSql)
        {
            CallResult ret = new CallResult() { Success = false };
            if (ListSql == null || ListSql.Count == 0)
            {
                ret.Success = false;
                ret.Result = "SQL事务为空";
                return ret;
            }
            lock (_OracleConn)
            {
                CreateCommand();
                OracleTransaction tran = _OracleConn.BeginTransaction();
                _OracleCommand.Transaction = tran;
                try
                {
                    int count = 0;
                    foreach (string strsql in ListSql)
                    {
                        if (strsql.Trim().Length > 0)
                        {
                            _OracleCommand.CommandText = strsql;
                            count += _OracleCommand.ExecuteNonQuery();
                        }
                    }
                    tran.Commit();
                    ret.Success = true;
                    ret.Result = count;
                    return ret;
                }
                catch (Exception ex)
                {
                    ret.Success = false;
                    ret.Result = ex.Message;
                    if (_LastError != ex.Message)
                    {
                        Logger.Error.Write(LOG_TYPE.ERROR, string.Format("【Oracle TranError】 {0}", ex.ToString()));
                        _LastError = ex.Message;
                    }
                    tran.Rollback();
                    return ret;
                }
            }
        }

        #endregion

        #region 内部方法 - 原始操作
        /// <summary>
        /// 设置连接字符串
        /// </summary>
        /// <param name="serverNode"></param>
        private void CreateConnectString(ServerNode serverNode)
        {
            if (serverNode == null)
            {
                serverNode = new ServerNode()
                {
                    ServerName = "",
                    ServerIP = "127.0.0.1",
                    ServerPort = 1521,
                    UserID = "root",
                    Password = "178876839@qq.com"
                };
            }
            if (string.IsNullOrEmpty(serverNode.ServerIP))
                serverNode.ServerIP = "127.0.0.1";
            if (serverNode.ServerPort == 0)
                serverNode.ServerPort = 1521;
            if (string.IsNullOrEmpty(serverNode.UserID))
                serverNode.UserID = "root";
            if (DESCryption.Default.IfWithEncryptMark(serverNode.Password))
                serverNode.Password = DESCryption.Default.DecodeWithMark(serverNode.Password);
            _ConNode = serverNode;
            _ConnString = string.Format(
                "User ID={0};Password={1};Data Source=(DESCRIPTION = (ADDRESS_LIST= (ADDRESS = (PROTOCOL = TCP)(HOST = {2})(PORT = {3}))) (CONNECT_DATA = (SERVICE_NAME = {4})))",
                serverNode.UserID,
                serverNode.Password,
                serverNode.ServerIP,
                serverNode.ServerPort,
                serverNode.Database
               );
        }

        /// <summary>
        /// 使用前检查DataReader状态
        /// </summary>
        private void CheckDataReaderState()
        {
            if (_OracleDataReader != null)
            {
                if (!_OracleDataReader.IsClosed)
                    _OracleDataReader.Close();
            }
        }

        /// <summary>
        /// 连接状态发生变化
        /// </summary>
        /// <param name="_newState">新的状态</param>
        private void ChangeState(ConnectionState _newState)
        {
            if (ConnectionStateChanged != null)
            {
                if (!_ConnStateTemp.Equals(_newState))
                {
                    _ConnStateTemp = _newState;
                    ConnectionStateChanged(this, _newState);
                }
            }
            else
            {
                _ConnStateTemp = ConnectionState.Broken;
            }
        }

        /// <summary>
        ///  Sql操作接口
        /// </summary>
        /// <param name="method">select,insert,update,delete</param>
        /// <param name="strSql"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private CallResult CmdBySql(string method, string strSql, CommandType type = CommandType.Text)
        {
            lock (_OracleConn)
            {
                CallResult ret = new CallResult() { Success = false };
                if (string.IsNullOrEmpty(strSql))
                {
                    ret.Success = false;
                    ret.Result = string.Format("执行【{0}】方法时SQL语句为空", method);
                    return ret;
                }
                try
                {
                    CreateCommand();
                    _OracleCommand.CommandType = type;
                    CheckDataReaderState();
                    switch (method.ToUpper())
                    {
                        case "SELECT":
                            _OracleCommand.CommandText = strSql;
                            _OracleDataReader = _OracleCommand.ExecuteReader();
                            DataTable dt = new DataTable();
                            dt.Load(_OracleDataReader);
                            ret.Result = dt;
                            ret.Success = true;
                            break;
                        case "EXE":
                            _OracleCommand.CommandText = string.Format("begin\r\n {0}\r\n end;", strSql);
                            ret.Result = _OracleCommand.ExecuteNonQuery();
                            ret.Success = true;
                            break;
                        default:
                            ret.Result = _OracleCommand.ExecuteNonQuery();
                            ret.Success = true;
                            break;
                    }
                    return ret;
                }
                catch (Exception ex)
                {
                    ret.Result = ex.Message;
                    ret.Success = false;
                    RaiseError(ex.Message);
                    if (_LastError != ex.Message)
                    {
                        Logger.Error.Write(LOG_TYPE.ERROR, string.Format("【Oracle CmdError】\r\n ErrMsg:\t\t{0} \r\nSqlStatement:\t{1}", ex.Message, strSql));
                        _LastError = ex.Message;
                    }
                }
                return ret;
            }
        }

        /// <summary>
        /// 异常发送
        /// </summary>
        private void RaiseError(string strErrMsg)
        {
            if (ErrorHappend != null)
                ErrorHappend(this, strErrMsg);
        }
        #endregion

        #region 内部方法 - 实体操作
        /// <summary>
        /// 取字段关联内容
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="pi"></param>
        /// <param name="colDef"></param>
        /// <param name="strDbFiledContent"></param>
        /// <param name="strDbFieldVal"></param>
        /// <returns></returns>
        private static bool GetFieldValue<T>(T data, PropertyInfo pi,
            ref ColumnDef colDef, ref string strDbFiledContent, ref string strDbFieldVal)
        {
            strDbFiledContent = pi.GetValue(data, null).ToMyFieldString();
            strDbFieldVal = strDbFiledContent.GoOriginal();
            if (pi.PropertyType.FullName == typeof(double).FullName || pi.PropertyType.FullName == typeof(float).FullName)
            {
                double dVal = strDbFieldVal.ToMyDouble();
                if (dVal != SystemDefault.InValidDouble)
                {
                    if (string.IsNullOrEmpty(colDef.DataFormat))
                        strDbFieldVal = dVal.ToString("0.00");
                    else
                        strDbFieldVal = dVal.ToString(colDef.DataFormat);
                }
                else
                {
                    strDbFieldVal = string.Empty;
                }
            }
            else if (pi.PropertyType.FullName == typeof(int).FullName)
            {
                int iVal = strDbFieldVal.ToMyInt();
                if (iVal != SystemDefault.InValidInt)
                    strDbFieldVal = iVal.ToString();
                else
                    strDbFieldVal = string.Empty;
            }
            return true;
        }

        /// <summary>
        /// 拼接条件字段
        /// </summary>
        /// <param name="FieldName"></param>
        /// <param name="FieldContent"></param>
        /// <param name="Where"></param>
        private static void AppandWhereSection(ColumnDef colDef, string FieldContent, ref string Where, bool IsUpdateMode = false)
        {
            bool IsWhereFieldDef = IsUpdateMode ? (colDef.PK || FieldContent.CheckIfWhere()) : true;
            if (!string.IsNullOrEmpty(FieldContent) && IsWhereFieldDef)
            {
                if (Where.Length > 0)
                    Where += " and ";
                if (FieldContent.CheckIfExpress())
                    Where += FieldContent.GoOriginal();
                else
                    Where += string.Format("{0}='{1}'", colDef.Name, FieldContent.GoOriginal());
            }
        }

        /// <summary>
        /// 拼接条件字段
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="SourceDic"></param>
        /// <param name="Where"></param>
        private static void AppandWhereSection<TKey, TValue>(Dictionary<TKey, TValue> SourceDic, ref string Where)
        {
            if (SourceDic == null)
                return;
            foreach (TKey k in SourceDic.Keys)
            {
                if (k is string)
                {
                    TValue value = SourceDic[k];
                    string FieldContent = value.ToMyString();
                    string FieldVal = FieldContent.GoOriginal();
                    if (!string.IsNullOrEmpty(FieldContent) && FieldContent.CheckIfWhere())
                    {
                        if (Where.Length > 0)
                            Where += " and ";
                        if (FieldContent.CheckIfExpress())
                            Where += FieldContent.GoOriginal();
                        else
                            Where += string.Format("{0}='{1}'", k, FieldContent.GoOriginal());
                    }
                }
            }
        }

        /// <summary>
        /// 拼接Insert段
        /// </summary>
        /// <param name="colDef"></param>
        /// <param name="FieldContent"></param>
        /// <param name="strInsert"></param>
        /// <param name="strValue"></param>
        private static void AppandInsertSection(ColumnDef colDef, string FieldContent, ref string strInsert, ref string strValue)
        {
            string FieldVal = FieldContent.GoOriginal();
            if (string.IsNullOrEmpty(FieldVal) && colDef.NN)
                FieldVal = colDef.DefaultExpress;
            if (!string.IsNullOrEmpty(FieldVal))
            {
                if (strInsert.Length > 0)
                    strInsert += ",";
                strInsert += colDef.Name;

                if (strValue.Length > 0)
                    strValue += ",";
                if (FieldContent.CheckIfExpress())
                    strValue += string.Format("{0}", FieldVal);
                else
                    strValue += string.Format("'{0}'", FieldVal.ValueFilter());
            }
        }

        /// <summary>
        /// 拼接Insert段集合
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="SourceDic"></param>
        /// <param name="strInsert"></param>
        /// <param name="strValue"></param>
        private static void AppandInsertSection<TKey, TValue>(Dictionary<TKey, TValue> SourceDic, ref string strInsert, ref string strValue)
        {
            if (SourceDic == null)
                return;
            foreach (TKey k in SourceDic.Keys)
            {
                if (k is string)
                {
                    TValue value = SourceDic[k];
                    if (strInsert.Length > 0)
                        strInsert += ",";
                    strInsert += string.Format("{0}", k);

                    if (strValue.Length > 0)
                        strValue += ",";
                    if (typeof(TValue) == typeof(double) || typeof(TValue) == typeof(float) || typeof(TValue) == typeof(int))
                        strValue += string.Format("{0}", value.ToMyString());
                    else if (value.ToMyString().CheckIfExpress())
                        strValue += string.Format("{0}", value.ToMyString().ParseExpress());
                    else
                        strValue += string.Format("'{0}'", value.ToMyString().ValueFilter());
                }
            }
        }

        /// <summary>
        /// 拼接Update表达式
        /// </summary>
        /// <param name="colDef"></param>
        /// <param name="FieldContent"></param>
        /// <param name="strUpdateExpress"></param>
        private static void AppandUpdateSection(ColumnDef colDef, string FieldContent, ref string strUpdateExpress)
        {
            //拼接更新字段
            string strFieldUpdateValue = string.Empty;
            string FieldVal = FieldContent.GoOriginal();
            if (!colDef.PK && !FieldContent.CheckIfWhere() && FieldVal != null)
                strFieldUpdateValue = FieldVal;
            else if (!colDef.PK && !FieldContent.CheckIfWhere() && colDef.NN)
                strFieldUpdateValue = colDef.DefaultExpress;
            if (string.IsNullOrEmpty(strFieldUpdateValue))
                return;
            if (strUpdateExpress.Length > 0)
                strUpdateExpress += ",";
            if (FieldContent.CheckIfExpress())
                strUpdateExpress += string.Format("{0}={1}", colDef.Name, strFieldUpdateValue);
            else
                strUpdateExpress += string.Format("{0}='{1}'", colDef.Name, strFieldUpdateValue.ValueFilter());
        }

        /// <summary>
        /// 字典字段拼接到Update或Where段落
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="SourceDic"></param>
        /// <param name="strUpdateExpress"></param>
        /// <param name="strWhere"></param>
        private static void AppandUpdateSection<TKey, TValue>(Dictionary<TKey, TValue> SourceDic, ref string strUpdateExpress, ref string strWhere)
        {
            if (SourceDic == null)
                return;
            bool IsWhereField = false;
            foreach (TKey k in SourceDic.Keys)
            {
                if (k is string)
                {
                    TValue value = SourceDic[k];
                    string FieldContent = value.ToMyString();
                    string FieldVal = FieldContent.GoOriginal();
                    if (FieldContent.CheckIfWhere() && FieldVal != null)
                        IsWhereField = true;
                    if (IsWhereField)
                    {
                        if (strWhere.Length > 0)
                            strWhere += " and ";
                        if (FieldContent.CheckIfExpress())
                            strWhere += FieldVal;
                        else
                            strWhere += string.Format("{0}='{1}'", k, FieldVal);
                    }
                    else
                    {
                        if (strUpdateExpress.Length > 0)
                            strUpdateExpress += ",";
                        if (typeof(TValue) == typeof(double) || typeof(TValue) == typeof(float) || typeof(TValue) == typeof(int))
                            strUpdateExpress += string.Format("{0}={1}", k, value.ToMyString());
                        else
                            strUpdateExpress += string.Format("{0}='{1}'", k, value.ToMyString().ValueFilter());
                    }
                }
            }
        }
        #endregion

        #region 内部方法 - 关联表达式
        /// <summary>
        /// 解析逻辑表达式
        /// WriteTableRow(SRC=[localtion_dryer],IF=[GroupKey=MoisDryer & PosID=16],THEN=[MaterLocation={0}])
        /// WriteTableRow(SRC=[sys_symbol],IF=[GroupName=MoisDryer & Name=POS_STATE],THEN=[CurrentValue=NORMAL])
        /// </summary>
        /// <param name="LogicExpress"></param>
        /// <returns></returns>
        private string ParseLogicExpress(string LogicExpress)
        {
            if (LogicExpress.Contains("WriteTableRow("))
            {
                return SqlWriteTableRow(LogicExpress);
            }
            else if (LogicExpress.Contains("ReadTableRow("))
            {
                return SqlReadTableRow(LogicExpress);
            }
            else if (!string.IsNullOrEmpty(LogicExpress))
            {
                string strBulidLogicExpress = BuildWritePosItem(LogicExpress);
                return SqlWriteTableRow(strBulidLogicExpress);
            }
            return string.Empty;
        }

        /// <summary>
        /// 写表表达式
        /// WriteTableRow(SRC=[localtion_dryer],IF=[GroupKey=MoisDryer & PosID=16],THEN=[MaterLocation={0}])
        /// </summary>
        /// <param name="LogicExpress"></param>
        /// <returns></returns>
        private static string SqlWriteTableRow(string LogicExpress)
        {
            string ExpressContent = LogicExpress.MidString("WriteTableRow(", ")", EndStringSearchMode.FromTail);
            if (string.IsNullOrEmpty(ExpressContent))
                return string.Empty;
            try
            {
                string FieldSrc = ExpressContent.MidString("SRC=[", "]").Trim();
                List<string> FieldIF = ExpressContent.MidString("IF=[", "]").MySplit("&");
                List<string> FieldThen = ExpressContent.MidString("THEN=[", "]").MySplit("&");
                string strUpdate = FieldThen.SectionUpdate();
                string strWhere = FieldIF.SectionWhere();
                string strSql = SqlMaker.SqlUpdate(FieldSrc, strUpdate, strWhere);
                return strSql;
            }
            catch (Exception ex)
            {

            }
            return string.Empty;
        }

        /// <summary>
        /// 读表表达式
        /// ReadTableRow(SRC=[location_group],IF=[GroupKey=RockSieve & PosID=1],RET=[MarkKey])
        /// </summary>
        /// <param name="LogicExpress"></param>
        /// <returns></returns>
        private static string SqlReadTableRow(string LogicExpress)
        {
            string ExpressContent = LogicExpress.MidString("ReadTableRow(", ")", EndStringSearchMode.FromTail);
            if (string.IsNullOrEmpty(ExpressContent))
                return string.Empty;
            try
            {
                string FieldSrc = ExpressContent.MidString("SRC=[", "]");
                List<string> FieldIF = ExpressContent.MidString("IF=[", "]").MySplit("&");
                string strWhere = FieldIF.SectionWhere();
                string strSql = SqlMaker.SqlQuery(FieldSrc, strWhere);
                return strSql;
            }
            catch (Exception ex)
            {

            }
            return string.Empty;
        }

        /// <summary>
        /// 写工位指令转为逻辑表达式形式
        /// </summary>
        /// <param name="">GrinderGroup.Grinder1.CmdReset=value</param>
        /// <returns></returns>
        private static string BuildWritePosItem(string PosExpress)
        {
            PosItem posItem = PosExpress.ParsePosItem();
            string strRelatedPos = posItem.Name;
            string strRelatedItem = posItem.Item;
            string strRelatedValue = posItem.Value;
            string strSetValue = string.Empty;
            return string.Format("WriteTableRow(SRC=[sys_symbol],IF=[GroupName={0} & Name={1}],THEN=[CurrentValue={2}])",
                strRelatedPos, strRelatedItem, strRelatedValue);
        }
        #endregion
    }
}
