using Engine.ComDriver;
using Engine.Common;
using Engine.Core;
using Engine.Mod;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Engine.Data.DBFAC
{
    /// <summary>
    /// 数据库类型
    /// </summary>
    public enum DataBaseType
    {
        None,
        NetDb,
        FileDb
    }

    /// <summary>
    /// 数据库常用方法
    /// </summary>
    public static class DbCommon
    {
        /// <summary>
        /// 克隆一个数据库连接
        /// </summary>
        /// <param name="CloneSource"></param>
        /// <param name="CloneKey"></param>
        /// <returns></returns>
        public static IDBFactory<TConNode> CloneInstance<TConNode>(this IDBFactory<TConNode> CloneSource, string CloneKey)
            where TConNode : new()
        {
            if (CloneSource == null)
                return null;
            return DbFactory.Current.CloneConn(CloneKey, CloneSource);
        }

        /// <summary>
        /// 克隆一个数据库连接
        /// </summary>
        /// <param name="CloneSource"></param>
        /// <param name="CloneKey"></param>
        /// <returns></returns>
        public static IDBFactory<ServerNode> CloneInstance(this IDBFactory<ServerNode> CloneSource, string CloneKey)
        {
            return DbFactory.Current[CloneKey, CloneSource];
        }

        /// <summary>
        /// 克隆一个数据库连接
        /// </summary>
        /// <param name="CloneSource"></param>
        /// <param name="CloneKey"></param>
        /// <returns></returns>
        public static IDBFactory<LocalSource> CloneInstance(this IDBFactory<LocalSource> CloneSource, string CloneKey)
        {
            return DbFactory.Current[CloneKey, CloneSource];
        }

        /// <summary>
        /// 创建数据库实例,根据Provider提供不同数据库
        /// </summary>
        /// <param name="serverNode">依据ProviderName创建</param>
        /// <returns></returns>
        public static IDBFactory<TConNode> CreateInstance<TConNode>(TConNode conNode)
            where TConNode:new()
        {
            if (conNode == null) return null;
            if (typeof(TConNode) == typeof(ServerNode))
            {
                ServerNode serverNode = new ServerNode();
                conNode.MapperToModel(ref serverNode);
                return (IDBFactory<TConNode>)CreateInstance(serverNode);
            }
            else if (typeof(TConNode) == typeof(LocalSource))
            {
                LocalSource lsNode = new LocalSource();
                conNode.MapperToModel(ref lsNode);
                return (IDBFactory<TConNode>)CreateInstance(lsNode);
            }
            return null;
        }

        /// <summary>
        /// 创建数据库实例,根据Provider提供不同数据库
        /// </summary>
        /// <param name="serverNode">依据ProviderName创建</param>
        /// <returns></returns>
        public static IDBFactory<ServerNode> CreateInstance(ServerNode serverNode)
        {
            try
            {
                IDBFactory<ServerNode> _DbFac = null;
                switch (serverNode.ProviderName.ToLower())
                {
                    case "engine.data.mysql":
                        _DbFac = CreateInstance("Engine.Data.MySQL", "Engine.Data.MySQL.DBMySQL", serverNode);
                        break;
                    case "engine.data.mssql":
                        _DbFac = CreateInstance("Engine", "Engine.Data.MSSQL.DBMSSQL", serverNode);
                        break;
                    case "engine.data.oracle":
                        _DbFac = CreateInstance("Engine.Data.Oracle", "Engine.Data.Oracle.DBOracle", serverNode);
                        break;
                    default:
                        if (!string.IsNullOrEmpty(serverNode.Provider))
                        {
                            List<string> providerField = serverNode.Provider.MySplit("|");
                            if (providerField.Count >= 2)
                                _DbFac = CreateInstance(providerField[0], providerField[1], serverNode);
                        }
                        break;
                }
                if (!string.IsNullOrEmpty(serverNode.ServerName) && _DbFac != null)
                {
                    if (serverNode.ServerName != SystemDefault.AppNameSpace)
                    {
                        _DbFac.ConnectionStateChanged += _DbFac_ConnectionStateChanged;
                        DbFactory.Net.DataBase.AppandDict(serverNode.ServerName, _DbFac);
                    }
                }
                //DbFactory.Current.AppandServerDatabase(serverNode);
                return _DbFac;
            }
            catch (Exception ex)
            {
                sCommon.MyMsgBox(string.Format("数据库【{0}】实例化失败！目标服务器：{1}\r\n\r\n错误信息：\r\n\r\n{2}",
                    serverNode.Database, serverNode.ServerIP, ex.Message), MsgType.Error);
                return null;
            }
        }

        /// <summary>
        /// 连接状态变更
        /// </summary>
        /// <param name="DbObj">数据库连接对象</param>
        /// <param name="ConState">更新状态</param>
        private static void _DbFac_ConnectionStateChanged(object DbObj, ConnectionState ConState)
        {
            if (DbObj == null) return;
            string ConName = string.Empty;
            ModelComLink comLink = new ModelComLink()
            {
                AppName = SystemDefault.AppName.MarkWhere(),
                ComLink = "Database".MarkWhere(),
                LinkState = ConState == ConnectionState.Open ? "1" : "0"
            };
            if (DbObj is IDBFactory<ServerNode> ServerCon)
            {
                ConName = ServerCon.ConNode.ServerName.ToMyString();
                comLink.DriverName = ConName.MarkWhere();
                WriteNetState(comLink);
                //ServerCon.ExcuteUpdate<ModelComLink>(comLink);
            }

            else if (DbObj is IDBFactory<LocalSource> LocalCon)
            {
                ConName = LocalCon.ConNode.SourceName.ToMyString();
                comLink.DriverName = ConName.MarkWhere();
                WriteNetState(comLink);
                //LocalCon.ExcuteUpdate<ModelComLink>(comLink);
            }
            else
                return;
            Logger.CommBody.Write(comLink.LinkState == "1" ? LOG_TYPE.MESS : LOG_TYPE.WARN, string.Format("【{0}】数据库连接{1}",
                ConName, comLink.LinkState == "1" ? "打开" : "关闭"));
        }

        private static void WriteNetState(ModelComLink comLink)
        {
            if (Project.Current.AppRunServer != null)
                Project.Current.AppRunServer.ExcuteUpdate<ModelComLink>(comLink);
            else if (Project.Current.AppRunLocal != null)
                Project.Current.AppRunLocal.ExcuteUpdate<ModelComLink>(comLink);
        }

        /// <summary>
        /// 创建数据库实例,根据Provider提供不同数据库
        /// </summary>
        /// <param name="serverNode">依据ProviderName创建</param>
        /// <returns></returns>
        public static IDBFactory<LocalSource> CreateInstance(LocalSource localSource)
        {
            try
            {
                IDBFactory<LocalSource> _DbFac = null;
                switch (localSource.ProviderName.ToLower())
                {
                    case "engine.data.msaccess":
                        _DbFac = CreateInstance("Engine", "Engine.Data.MSACCESS.DBMSACCESS", localSource);
                        break;
                    case "engine.data.msexcel":
                        _DbFac = CreateInstance("Engine", "Engine.Data.MSEXCEL.DBMSEXCEL", localSource);
                        break;
                    case "engine.data.sqlite":
                        _DbFac = CreateInstance("Engine.Data.SQLite", "Engine.Data.SQLite.DBSQLite", localSource);
                        break;
                    default:
                        if (!string.IsNullOrEmpty(localSource.Provider))
                        {
                            List<string> providerField = localSource.Provider.MySplit("|");
                            if (providerField.Count >= 2)
                                _DbFac = CreateInstance(providerField[0], providerField[1], localSource);
                        }
                        break;
                }
                if (!string.IsNullOrEmpty(localSource.SourceName) && _DbFac != null)
                {
                    if (localSource.SourceName != SystemDefault.AppNameSpace)
                    {
                        _DbFac.ConnectionStateChanged += _DbFac_ConnectionStateChanged;
                        DbFactory.Local.DataBase.AppandDict(localSource.SourceName, _DbFac);
                    }
                }
                return _DbFac;
            }
            catch (Exception err)
            {
                sCommon.MyMsgBox(string.Format("数据源【{0}】实例化失败！", localSource.SourceFile), MsgType.Error);
                sCommon.MyMsgBox(string.Format("错误信息：\r\n\r\n{0}", err.Message), MsgType.Error);
                return null;
            }
        }

        /// <summary>
        /// 创建数据库实例
        /// </summary>
        /// <param name="assemString">程序集名称</param>
        /// <param name="typeName">类型名</param>
        /// <param name="serverNodeOrLocalSource">网络数据库：服务器节点信息  本地数据库：数据源</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        private static IDBFactory<T> CreateInstance<T>(string assemString, string typeName, T serverNodeOrLocalSource) where T : new()
        {
            try
            {
                Assembly assembly = Assembly.Load(assemString);
                Type type = assembly.GetType(typeName);
                object obj = Activator.CreateInstance(type, serverNodeOrLocalSource);
                IDBFactory<T> dBHelper = obj as IDBFactory<T>;
                return dBHelper;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 创建数据库实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assemString">程序集名称</param>
        /// <param name="typeName">类型名</param>
        /// <returns></returns>
        public static IDBFactory<T> CreateInstance<T>(string assemString, string typeName) where T : new()
        {
            try
            {
                Assembly assembly = Assembly.Load(assemString);
                Type type = assembly.GetType(typeName);
                object obj = Activator.CreateInstance(type);
                IDBFactory<T> dBHelper = obj as IDBFactory<T>;
                return dBHelper;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static List<string> LstNetDb = new List<string>() {
            "engine.data.mysql", "engine.data.mssql", "engine.data.oracle" };
        private static List<string> LstFileDb = new List<string>() {
            "engine.data.msaccess", "engine.data.msexcel", "engine.data.sqlite" };

        /// <summary>
        /// 解析连接数据类型
        /// </summary>
        /// <param name="strProvider"></param>
        /// <returns></returns>
        public static DataBaseType GetDbType(this string strProvider)
        {
            DataBaseType dbType = DataBaseType.None;
            string strLowerProvider = strProvider.ToLower().Trim();
            if (LstNetDb.Contains(strLowerProvider))
                dbType = DataBaseType.NetDb;
            else if (LstFileDb.Contains(strLowerProvider))
                dbType = DataBaseType.FileDb;
            return dbType;
        }
    }
}
