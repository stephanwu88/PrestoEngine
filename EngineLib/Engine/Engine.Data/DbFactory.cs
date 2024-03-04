using Engine.ComDriver;
using Engine.Common;
using Engine.Core;
using Engine.Mod;
using System;
using System.Collections.Generic;

namespace Engine.Data.DBFAC
{
    /// <summary>
    /// 数据库工厂集合
    /// </summary>
    /// <typeparam name="TConNode"></typeparam>
    public class DbFactory<TConNode> where TConNode : new()
    {
        public Dictionary<string, IDBFactory<TConNode>> DataBase = new Dictionary<string, IDBFactory<TConNode>>();

        /// <summary>
        /// 连接数
        /// </summary>
        public int ConnCount => DataBase.Count;

        /// <summary>
        /// 附加网络连接到工厂集合
        /// </summary>
        /// <param name="ConNode"></param>
        public void AppandDatabase(TConNode ConNode)
        {
            string strNodeName = GetConName(ConNode);
            if (string.IsNullOrEmpty(strNodeName)) return;
            if (DataBase.MyContains(strNodeName)) return;
            IDBFactory<TConNode> _DbFac = DbCommon.CreateInstance(ConNode);
            DataBase.AppandDict(strNodeName, _DbFac);            
        }

        /// <summary>
        /// 附加连接到工厂集合
        /// </summary>
        /// <param name="DbFac"></param>
        public void AppandDatabase(IDBFactory<TConNode> DbCon)
        {
            string strNodeName = GetConName(DbCon.ConNode);
            if (string.IsNullOrEmpty(strNodeName)) return;
            if (DataBase.MyContains(strNodeName)) return;
            DataBase.AppandDict(strNodeName, DbCon);
        }

        /// <summary>
        /// 附加专有连接到工厂
        /// </summary>
        /// <param name="DbKey"></param>
        /// <param name="ConNode"></param>
        public void AppandDatabase(string DbKey, TConNode ConNode)
        {
            if (string.IsNullOrEmpty(DbKey) || ConNode == null) return;
            if (!DataBase.MyContains(DbKey))
            {
                IDBFactory<TConNode> _DbFac = DbCommon.CreateInstance(ConNode);
                DataBase.AppandDict(DbKey, _DbFac);
            }
        }

        /// <summary>
        /// 附加专有连接到工厂
        /// </summary>
        /// <param name="DbKey"></param>
        /// <param name="DbFac"></param>
        public void AppandDatabase(string DbKey, IDBFactory<TConNode> DbFac)
        {
            if (string.IsNullOrEmpty(DbKey) || DbFac == null) return;
            if (DbFac.ConNode == null) return;
            AppandDatabase(DbKey, DbFac.ConNode);
        }

        /// <summary>
        /// 根据名称获取连接实例
        /// </summary>
        /// <param name="ConName"></param>
        /// <returns></returns>
        public IDBFactory<TConNode> GetConn(string ConName) => DataBase.DictFieldValue(ConName);

        /// <summary>
        /// 根据名称获取连接实例
        /// </summary>
        /// <param name="DbKey"></param>
        /// <returns></returns>
        public IDBFactory<TConNode> GetConn(TConNode ConNode)
        {
            string strNodeName = GetConName(ConNode);
            if (string.IsNullOrEmpty(strNodeName)) return null;
            if (!DataBase.MyContains(strNodeName))
                AppandDatabase(strNodeName, ConNode);
            return DataBase.DictFieldValue(strNodeName);
        }

        /// <summary>
        /// 根据名称获取连接实例
        /// </summary>
        /// <param name="DbKey"></param>
        /// <returns></returns>
        public IDBFactory<TConNode> GetConn(string ConKey, IDBFactory<TConNode> DbCon)
        {
            if(!DataBase.MyContains(ConKey))
               AppandDatabase(ConKey, DbCon);
            return DataBase.DictFieldValue(ConKey);
        }

        /// <summary>
        /// 根据名称获取连接实例
        /// </summary>
        /// <param name="DbKey"></param>
        /// <returns></returns>
        public IDBFactory<TConNode> GetConn(string ConKey, TConNode ConNode)
        {
            if (!DataBase.MyContains(ConKey))
                AppandDatabase(ConKey, ConNode);
            return DataBase.DictFieldValue(ConKey);
        }

        /// <summary>
        /// 获取连接名称
        /// </summary>
        /// <param name="conNode"></param>
        /// <returns></returns>
        public static string GetConName(TConNode conNode)
        {
            string strNodeName = string.Empty;
            if (conNode == null) return strNodeName;
            if (typeof(TConNode) == typeof(ServerNode))
            {
                ServerNode serverNode = new ServerNode();
                conNode.MapperToModel(ref serverNode);
                strNodeName = serverNode.ServerName;
                if (string.IsNullOrEmpty(strNodeName))
                    strNodeName = serverNode.MyJsonSerialize().Result.ToMyString();
            }
            else if (typeof(TConNode) == typeof(LocalSource))
            {
                LocalSource lsNode = new LocalSource();
                conNode.MapperToModel(ref lsNode);
                strNodeName = lsNode.SourceName;
                if (string.IsNullOrEmpty(strNodeName))
                    strNodeName = lsNode.MyJsonSerialize().Result.ToMyString();
            }
            return strNodeName;
        }

        /// <summary>
        /// 属性拾取连接对象
        /// </summary>
        /// <param name="ConNode"></param>
        /// <returns></returns>
        public IDBFactory<TConNode> this[TConNode ConNode] => GetConn(ConNode);

        /// <summary>
        /// 属性拾取连接对象
        /// </summary>
        /// <param name="ConName"></param>
        /// <returns></returns>
        public IDBFactory<TConNode> this[string ConName] => GetConn(ConName);

        /// <summary>
        /// 属性拾取连接对象
        /// </summary>
        /// <param name="ConName"></param>
        /// <param name="SourceConn"></param>
        /// <returns></returns>
        public IDBFactory<TConNode> this[string ConName, IDBFactory<TConNode> SourceConn] => GetConn(ConName,SourceConn);

        /// <summary>
        /// 属性拾取连接对象
        /// </summary>
        /// <param name="ConName"></param>
        /// <param name="ConNode"></param>
        /// <returns></returns>
        public IDBFactory<TConNode> this[string ConName, TConNode ConNode] => GetConn(ConName,ConNode);
    }

    /// <summary>
    /// 数据库工厂集合
    /// </summary>
    public class DbFactory
    {
        public static DbFactory<ServerNode> Net = new DbFactory<ServerNode>();
        public static DbFactory<LocalSource> Local = new DbFactory<LocalSource>();
        private static Dictionary<string, DbFactory> DicDbFactory = new Dictionary<string, DbFactory>();
        private static DbFactory _Current;

        /// <summary>
        /// 获取指定应用相关的数据库工厂集
        /// </summary>
        /// <param name="LogKey"></param>
        /// <returns></returns>
        private static DbFactory GetInstance(string DbFactoryKey)
        {
            lock (DicDbFactory)
            {
                if (!DicDbFactory.ContainsKey(DbFactoryKey))
                    DicDbFactory.Add(DbFactoryKey, new DbFactory());
                return DicDbFactory[DbFactoryKey];
            }
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            try
            {
                ModelComLink comLink = new ModelComLink() { AppName = SystemDefault.AppName.MarkWhere(), LinkState = "0" };
                if (Project.Current.StartUpDbType == DataBaseType.NetDb)
                    Current.GetConn<ServerNode>(SystemDefault.AppNameSpace).ExcuteUpdate(comLink);
                else if (Project.Current.StartUpDbType == DataBaseType.FileDb)
                    Current.GetConn<LocalSource>(SystemDefault.AppNameSpace).ExcuteUpdate(comLink);
                Logger.Default.Write(LOG_TYPE.MESS, "关闭所有数据库连接");
            }
            catch (Exception )
            {
                Logger.Default.Write(LOG_TYPE.MESS, "关闭所有数据库连接");
            }
        }

        /// <summary>
        /// 本地化引用
        /// </summary>
        public static DbFactory Current
        {
            get
            {
                if (_Current == null)
                    _Current = new DbFactory();
                return _Current;
            }
        }
        /// <summary>
        /// 运维连接库
        /// </summary>
        public static IDBFactory<ServerNode> Default { get => Net.DataBase.DictFieldValue(DbConKey.SYS); }

        /// <summary>
        /// 任务服务器
        /// </summary>
        public static IDBFactory<ServerNode> CPU => Net.DataBase.DictFieldValue(DbConKey.Task);

        /// <summary>
        /// 内核服务器
        /// </summary>
        public static IDBFactory<ServerNode> Task => Net.DataBase.DictFieldValue(DbConKey.Task);

        /// <summary>
        /// 数据服务器
        /// </summary>
        public static IDBFactory<ServerNode> Data => Net.DataBase.DictFieldValue(DbConKey.Data);

        /// <summary>
        /// 附加网络连接到工厂集合
        /// </summary>
        /// <param name="serverNode"></param>
        public void AppandServerDatabase(ServerNode serverNode)
        {
            Net.AppandDatabase(serverNode);
        }

        /// <summary>
        /// 附加文件连接到工厂集合
        /// </summary>
        /// <param name="localSource"></param>
        public void AppandSourceDatabase(LocalSource localSource)
        {
            Local.AppandDatabase(localSource);
        }

        /// <summary>
        /// 根据名称获取连接实例
        /// </summary>
        /// <typeparam name="TCon"></typeparam>
        /// <param name="ConName"></param>
        /// <returns></returns>
        public IDBFactory<TCon> GetConn<TCon>(string ConName)
            where TCon : new()
        {
            if (Net.DataBase.MyContains(ConName))
                return (IDBFactory<TCon>)Net.GetConn(ConName);
            else if (Local.DataBase.MyContains(ConName))
                return (IDBFactory<TCon>)Local.GetConn(ConName);
            return null;
        }

        /// <summary>
        /// 根据名称获取连接实例
        /// </summary>
        /// <typeparam name="TCon"></typeparam>
        /// <param name="ConName"></param>
        /// <returns></returns>
        public IDBFactory<TCon> CloneConn<TCon>(string ConName,IDBFactory<TCon> DbFac)
            where TCon : new()
        {
            if (typeof(TCon) == typeof(ServerNode))
            {
                if (Net.DataBase.MyContains(ConName))
                    return (IDBFactory<TCon>)Net.GetConn(ConName);
                else
                {
                    Net.AppandDatabase(ConName,(IDBFactory<ServerNode>)DbFac);
                    return (IDBFactory<TCon>)Net.DataBase.DictFieldValue(ConName);
                }
            }
            else if (typeof(TCon) == typeof(LocalSource))
            {
                if (Local.DataBase.MyContains(ConName))
                    return (IDBFactory<TCon>)Local.GetConn(ConName);
                else
                {
                    Local.AppandDatabase(ConName, (IDBFactory<LocalSource>)DbFac);
                    return (IDBFactory<TCon>)Local.DataBase.DictFieldValue(ConName);
                }
            }
            return null;
        }

        /// <summary>
        /// 属性拾取连接对象
        /// </summary>
        /// <param name="serverNode"></param>
        /// <returns></returns>
        public IDBFactory<ServerNode> this[ServerNode serverNode] => Net[serverNode];
        /// <summary>
        /// 属性拾取连接对象
        /// </summary>
        /// <param name="localSource"></param>
        /// <returns></returns>
        public IDBFactory<LocalSource> this[LocalSource localSource] => Local[localSource];

        /// <summary>
        /// 属性拾取连接对象
        /// </summary>
        /// <param name="DbKey"></param>
        /// <param name="DbCon"></param>
        /// <returns></returns>
        public IDBFactory<ServerNode> this[string DbKey, IDBFactory<ServerNode> DbCon = null] => Net[DbKey, DbCon];
        /// <summary>
        /// 属性拾取连接对象
        /// </summary>
        /// <param name="DbKey"></param>
        /// <param name="DbCon"></param>
        /// <returns></returns>
        public IDBFactory<LocalSource> this[string DbKey, IDBFactory<LocalSource> DbCon] => Local[DbKey, DbCon];
        /// <summary>
        /// 属性拾取连接对象
        /// </summary>
        /// <param name="DbKey"></param>
        /// <param name="serverNode"></param>
        /// <returns></returns>
        public IDBFactory<ServerNode> this[string DbKey, ServerNode serverNode] => Net[DbKey,serverNode];
        /// <summary>
        /// 属性拾取连接对象
        /// </summary>
        /// <param name="DbKey"></param>
        /// <param name="localSource"></param>
        /// <returns></returns>
        public IDBFactory<LocalSource> this[string DbKey, LocalSource localSource] => Local[DbKey, localSource];
    }
}