using Engine.ComDriver;
using Engine.Common;
using Engine.Data.DBFAC;
using Engine.Files;
using Engine.Mod;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using System.Windows;

namespace Engine.Core
{
    /// <summary>
    /// 工程信息
    /// </summary>
    public class Project
    {
        #region 内部变量 - 当前工程
        private IniFile _CurrentPrjfile;
        /// <summary>
        /// 运行模式
        /// </summary>
        public string RunMode;

        #endregion

        #region 静态实例
        private static Project _Current = null;
        /// <summary>
        /// 当前应用工程
        /// </summary>
        [JsonIgnore()]
        public static Project Current
        {
            get
            {
                if (_Current == null)
                {
                    _Current = new Project();
                    CallResult ret = _Current.ParseProjectStartUp();
                    if (ret.Fail)
                        throw new Exception(ret.Result.ToMyString());
                }
                return _Current;
            }
        }
        #endregion

        #region 属性 
        /// <summary>
        /// 启动数据库类型
        /// </summary>
        public DataBaseType StartUpDbType { get; set; } = DataBaseType.None;
        /// <summary>
        /// 启动加载
        /// </summary>
        public ProjectStartUP StartUP { get; set; } = new ProjectStartUP();
        /// <summary>
        /// 常规设置
        /// </summary>
        public ProjectNormal Normal { get; set; } = new ProjectNormal();
        /// <summary>
        /// 网络数据库连接
        /// </summary>
        public List<ServerNode> NetDbServer { get; set; } = new List<ServerNode>();
        /// <summary>
        /// 本地文件连接
        /// </summary>
        public List<LocalSource> LocalDbSource { get; set; } = new List<LocalSource>();
        /// <summary>
        /// 日志配置项
        /// </summary>
        public List<LogItem> Logger { get; set; } = new List<LogItem>();

        /// <summary>
        /// 项目启动服务连接
        /// </summary>
        public IDBFactory<ServerNode> AppRunServer { get; private set; }
        public IDBFactory<LocalSource> AppRunLocal { get; private set; }
        /// <summary>
        /// 启动文件
        /// </summary>
        public IniFile StartUpIni { get => _CurrentPrjfile; }
        #endregion

        #region 内部方法
        /// <summary>
        /// 解析工程启动文件
        /// </summary>
        /// <returns></returns>
        private CallResult ParseProjectStartUp()
        {
            CallResult res = new CallResult();
            List<string> ConfigFile = new List<string>();
            #region 加载StartUp.ini文件
            string strCurIniFile = Directory.GetCurrentDirectory() + @"\StartUp.ini";
            if (!File.Exists(strCurIniFile))
                strCurIniFile = Directory.GetCurrentDirectory() + @"\Config\StartUp.ini";
            if (!File.Exists(strCurIniFile))
            {
                res.Success = false;
                res.Result = "未找到工程启动文件！";
                return res;
            }
            _CurrentPrjfile = new IniFile(strCurIniFile);
          
            #endregion

            #region 解析ini文件字段，获取主ServerNode
            string strAppName = _CurrentPrjfile.ReadValue("StartUP", "AppName").ToMyString();
            if (!string.IsNullOrEmpty(strAppName)) SystemDefault.AppName = strAppName;
            string strServerIP = _CurrentPrjfile.ReadValue("StartUP", "ServerIP").ToMyString();
            string strServerPort = _CurrentPrjfile.ReadValue("StartUP", "ServerPort").ToMyString();
            string strDatabase = _CurrentPrjfile.ReadValue("StartUP", "Database").ToMyString();
            string strPassword = _CurrentPrjfile.ReadValue("StartUP", "Password").ToMyString();
            string strProvider = _CurrentPrjfile.ReadValue("StartUP", "ProviderName").ToMyString().ToLower();
            RunMode = _CurrentPrjfile.ReadValue("StartUP", "RunMode").ToMyString();
            if (string.IsNullOrEmpty(RunMode))
                RunMode = "Visitor";
            if (string.IsNullOrEmpty(strProvider) || strProvider.Contains("mysql")) strProvider = "engine.data.mysql";
            else if (strProvider.Contains("mssql"))     strProvider = "engine.data.mssql";
            else if (strProvider.Contains("oracle"))    strProvider = "engine.data.oracle";
            else if (strProvider.Contains("msaccess"))  strProvider = "engine.data.msaccess";
            else if (strProvider.Contains("msexcel"))   strProvider = "engine.data.msexcel";
            else if (strProvider.Contains("sqlite"))    strProvider = "engine.data.sqlite";
            else
                strProvider = "";
            StartUpDbType = strProvider.GetDbType();
            if (StartUpDbType == DataBaseType.NetDb)
            {
                if (string.IsNullOrEmpty(strServerIP) || string.IsNullOrEmpty(strDatabase))
                {
                    res.Success = false;
                    res.Result = "启动时未找到完整的服务器连接配置信息！";
                    return res;
                }
                else
                {
                    ServerNode runServer = new ServerNode()
                    {
                        ServerName = SystemDefault.AppNameSpace,
                        Database = strDatabase,
                        ServerIP = strServerIP,
                        ProviderName = strProvider
                    };
                    if (!string.IsNullOrEmpty(strPassword))
                        runServer.Password = strPassword;
                    if (!string.IsNullOrEmpty(strServerPort))
                        runServer.ServerPort = strServerPort.ToMyInt();
                    return BootLoader(runServer);
                }
            }
            else if (StartUpDbType == DataBaseType.FileDb)
            {
                string StartUpDirectory = strCurIniFile.GetUpLevelPath(1);
                string strDbFile = StartUpDirectory + string.Format(@"\{0}", strDatabase);
                if (string.IsNullOrEmpty(strDatabase))
                {
                    res.Success = false;
                    res.Result = "启动时未找到工程数据文件连接信息！";
                    return res;
                }
                else
                {
                    LocalSource runServer = new LocalSource()
                    {
                        SourceName = SystemDefault.AppNameSpace,
                        SourceFile = strDbFile,
                        //Password = "presto",
                        ProviderName = strProvider
                    };
                    if (!string.IsNullOrEmpty(strPassword))
                        runServer.Password = strPassword;
                    return BootLoader(runServer);
                }
            }
            #endregion

            res.Success = false;
            res.Result = "未配置启动加载项！";
            return res;
        }

        /// <summary>
        /// Bootloader from serverNode
        /// </summary>
        /// <param name="runServer"></param>
        /// <returns></returns>
        private CallResult BootLoader(ServerNode runServer)
        {
            CallResult res = new CallResult();
            CallResult _ret;
            List<string> ConfigFile = new List<string>();

            #region 连接主ServerNode,同步数据库StartUP组
            ModelAppStartUp startup = new ModelAppStartUp()
            {
                AppName = SystemDefault.AppName,
                GroupName = "StartUP"
            };
            _ret = DbFactory.Current[runServer].ExcuteQuery<ModelAppStartUp>(startup);
            if (_ret.Fail)
            {
                res.Success = false;
                res.Result = string.Format("工程启动数据库接驳失败！\r\n{0}", _ret.Result.ToMyString());
                return res;
            }
            AppRunServer = DbFactory.Current[runServer];
            DataTable dt = _ret.Result.ToMyDataTable();
            if (dt.Rows.Count == 0)
            {
                res.Success = false;
                res.Result = "未找到应用程序启动加载项！";
                return res;
            }
            List<ModelAppStartUp> LstStartUpUpdate = new List<ModelAppStartUp>();
            string strPrjSource = string.Empty;
            //StartUP与ini文件设置同步
            foreach (DataRow row in dt.Rows)
            {
                string strKeyName = row["KeyName"].ToMyString();
                string strDataType = row["DataType"].ToMyString();
                string strConfigValue = row["ConfigValue"].ToMyString();
                string strKeyValue = _CurrentPrjfile.ReadValue("StartUP", strKeyName).ToMyString();
                if (strKeyName == "ProjectSource")
                    strPrjSource = strConfigValue;
                StartUP.AppandField(strKeyName, strConfigValue);
                if (string.IsNullOrEmpty(strKeyValue) || strConfigValue == strKeyValue)
                    continue;
                StartUP.AppandField(strKeyName, strKeyValue);
                if (strKeyName == "ProjectSource")
                    strPrjSource = strKeyValue;
                LstStartUpUpdate.Add(new ModelAppStartUp()
                {
                    GroupName = "StartUP".MarkWhere(),
                    KeyName = strKeyName.MarkWhere(),
                    ConfigValue = strKeyValue
                });
            }
            //更新到数据库
            if (LstStartUpUpdate.Count > 0)
            {
                CallResult ret = DbFactory.Current[runServer].ExcuteUpdate<ModelAppStartUp>(LstStartUpUpdate);
                if (ret.Fail)
                {
                    res.Success = false;
                    res.Result = "更新StartUP到数据库失败！";
                    return res;
                }
            }
            #endregion

            #region 解析.prj工程文件内配置 - 并同步到数据库对应组
            LstStartUpUpdate = new List<ModelAppStartUp>();
            string strProjectSource = strPrjSource.FormatPath();
            if (File.Exists(strProjectSource) && !string.IsNullOrEmpty(strProjectSource))
            {
                ConfigFile.Add(strProjectSource);
                string strProjectFile = File.ReadAllText(strProjectSource);
                _ret = strProjectFile.MyJsonDeserialize<Project>();
                if (_ret.Fail)
                {
                    res.Success = false;
                    res.Result = string.Format("工程文件【{0}】解析失败！\r\n{1}", strProjectSource, _ret.Result.ToMyString());
                    return res;
                }
                //更新ProjectFile Normal 到数据库 Normal
                Project proFull = _ret.Result as Project;
                ProjectNormal proNormal = proFull.Normal;
                if (proNormal != null)
                {
                    foreach (PropertyInfo pi in typeof(ProjectNormal).GetProperties())
                    {
                        string strName = pi.Name;
                        string strValue = proNormal.GetPropValue<ProjectNormal>(pi.Name).ToMyString();
                        if (string.IsNullOrEmpty(strValue))
                            continue;
                        LstStartUpUpdate.Add(new ModelAppStartUp()
                        {
                            GroupName = "Normal".MarkWhere(),
                            KeyName = strName.MarkWhere(),
                            ConfigValue = strValue
                        });
                    }
                }
                //更新ProjectFile NetDbServer 到数据库 NetDbServer组
                List<ServerNode> LstServer = proFull.NetDbServer;
                if (LstServer != null)
                {
                    foreach (ServerNode node in LstServer)
                    {
                        string strServerKey = node.ServerName.ToMyString();
                        if (!string.IsNullOrEmpty(strServerKey) && node != null)
                        {
                            LstStartUpUpdate.Add(new ModelAppStartUp()
                            {
                                GroupName = "NetDbServer".MarkWhere(),
                                KeyName = strServerKey.MarkWhere(),
                                ConfigValue = node.MyJsonSerialize().Result.ToMyString()
                            });
                        }
                    }
                }
                //更新ProjectFile LocalDbSource 到数据库 LocalDbSource组
                List<LocalSource> LstDS = proFull.LocalDbSource;
                if (LstDS != null)
                {
                    foreach (LocalSource node in LstDS)
                    {
                        string strSourceKey = node.SourceName.ToMyString();
                        if (!string.IsNullOrEmpty(strSourceKey) && node != null)
                        {
                            LstStartUpUpdate.Add(new ModelAppStartUp()
                            {
                                GroupName = "LocalDbSource".MarkWhere(),
                                KeyName = strSourceKey.MarkWhere(),
                                ConfigValue = node.MyJsonSerialize().Result.ToMyString()
                            });
                        }
                    }
                }
                //更新ProjectFile Logger 到数据库 Logger组
                List<LogItem> LstLdf = proFull.Logger;
                if (LstLdf != null)
                {
                    foreach (LogItem itm in LstLdf)
                    {
                        string strLogKey = itm.LogType.ToMyString();
                        if (!string.IsNullOrEmpty(strLogKey) && itm != null)
                        {
                            LstStartUpUpdate.Add(new ModelAppStartUp()
                            {
                                GroupName = "Logger".MarkWhere(),
                                KeyName = strLogKey.MarkWhere(),
                                ConfigValue = itm.MyJsonSerialize().Result.ToMyString(),
                                Remark = itm.Comment.ToMyString()
                            });
                        }
                    }
                }
            }
            //更新到数据库
            if (LstStartUpUpdate.Count > 0)
            {
                MessageBoxResult mes = sCommon.MyMsgBox("检测到工程配置文件更新，请确认?", MsgType.Question);
                if (mes == MessageBoxResult.No)
                {
                    res.Success = false;
                    res.Result = "用户决策不更新工程配置！";
                    return res;
                }
                CallResult ret = DbFactory.Current[runServer].ExcuteUpdate<ModelAppStartUp>(LstStartUpUpdate);
                if (ret.Fail)
                {
                    res.Success = false;
                    res.Result = "更新工程配置文件到数据库失败！";
                    return res;
                }
            }
            #endregion

            #region 解析.prj工程外部配置文件 - Normal组涉及的外部文件,按文件类型逐个解析并同步数据库
            startup = new ModelAppStartUp()
            {
                AppName = SystemDefault.AppName,
                GroupName = "Normal",
                DataType = "textfile"
            };

            LstStartUpUpdate = new List<ModelAppStartUp>();
            dt = DbFactory.Current[runServer].ExcuteQuery<ModelAppStartUp>(startup).Result.ToMyDataTable();
            foreach (DataRow row in dt.Rows)
            {
                string strFilePath = row["ConfigValue"].ToMyString().FormatPath();
                if (!File.Exists(strFilePath))
                    continue;
                ConfigFile.Add(strFilePath);
                string strFileContent = File.ReadAllText(strFilePath);
                if (string.IsNullOrEmpty(strFileContent))
                    continue;
                _ret = strFileContent.MyJsonDeserialize<Project>();
                if (_ret.Fail)
                {
                    res.Success = false;
                    res.Result = string.Format("文件解析【{0}】失败！\r\n{1}", strFilePath, _ret.Result.ToMyString());
                    return res;
                }
                Project PrjLoader = _ret.Result as Project;
                string strFileType = strFilePath.GetExtension();
                switch (strFileType)
                {
                    case ".nd":     //ServerNode文件 更新到 NetDbServer组
                        List<ServerNode> LstServer = PrjLoader.NetDbServer;
                        if (LstServer != null)
                        {
                            foreach (ServerNode node in LstServer)
                            {
                                string strServerKey = node.ServerName.ToMyString();
                                if (!string.IsNullOrEmpty(strServerKey) && node != null)
                                {
                                    LstStartUpUpdate.Add(new ModelAppStartUp()
                                    {
                                        KeyName = strServerKey.MarkWhere(),
                                        ConfigValue = node.MyJsonSerialize().Result.ToMyString()
                                    });
                                }
                            }
                        }
                        break;

                    case ".ls":     //LocalSource文件 到数据库 LocalDbSource组
                        List<LocalSource> LstDS = PrjLoader.LocalDbSource;
                        if (LstDS != null)
                        {
                            foreach (LocalSource node in LstDS)
                            {
                                string strSourceKey = node.SourceName.ToMyString();
                                if (!string.IsNullOrEmpty(strSourceKey) && node != null)
                                {
                                    LstStartUpUpdate.Add(new ModelAppStartUp()
                                    {
                                        KeyName = strSourceKey.MarkWhere(),
                                        ConfigValue = node.MyJsonSerialize().Result.ToMyString()
                                    });
                                }
                            }
                        }
                        break;

                    case ".lgf":    //日志配置文件  到数据库 Logger组
                        List<LogItem> LstLogPath = PrjLoader.Logger;
                        if (LstLogPath != null)
                        {
                            foreach (LogItem itm in LstLogPath)
                            {
                                string strLogKey = itm.LogType.ToMyString();
                                string strLogPath = itm.FilePath.ToMyString();
                                if (!string.IsNullOrEmpty(strLogKey))
                                {
                                    LstStartUpUpdate.Add(new ModelAppStartUp()
                                    {
                                        KeyName = strLogKey.MarkWhere(),
                                        ConfigValue = itm.MyJsonSerialize().Result.ToMyString()
                                    });
                                }
                            }
                        }
                        break;

                    case ".prj":   //工程文件
                    default:
                        continue;
                }
            }

            //更新到数据库
            if (LstStartUpUpdate.Count > 0)
            {
                MessageBoxResult mes = sCommon.MyMsgBox("检测到工程关联文件更新，请确认?", MsgType.Question);
                if (mes == MessageBoxResult.No)
                {
                    res.Success = false;
                    res.Result = "用户决策不更新工程关联配置！";
                    return res;
                }

                CallResult ret = DbFactory.Current[runServer].ExcuteUpdate<ModelAppStartUp>(LstStartUpUpdate);
                if (ret.Fail)
                {
                    res.Success = false;
                    res.Result = "更新工程外部配置文件到数据库失败！";
                    return res;
                }
            }
            #endregion

            #region 删除更新文件
            if (ConfigFile.Count > 0)
            {
                foreach (string file in ConfigFile)
                    File.Delete(file);
            }
            #endregion

            #region 初始化工程加载项
            List<ModelComLink> LstComLink = new List<ModelComLink>();
            startup = new ModelAppStartUp()
            {
                AppName = SystemDefault.AppName,
                DataType = "DataType in ('ServerNode','LocalSource','LogItem')".MarkExpress().MarkWhere()
            };
            _ret = DbFactory.Current[runServer].ExcuteQuery<ModelAppStartUp>(startup);
            if (_ret.Fail)
            {
                res.Success = false;
                res.Result = string.Format("工程资源加载失败！\r\n原因:{0}", _ret.Result.ToMyString());
                return res;
            }
            dt = _ret.Result.ToMyDataTable();
            foreach (DataRow row in dt.Rows)
            {
                ModelAppStartUp d = sCommon.ToEntity<ModelAppStartUp>(row);
                if (string.IsNullOrEmpty(d.KeyName) || string.IsNullOrEmpty(d.ConfigValue))
                    continue;

                switch (d.DataType.ToMyString())
                {
                    case "ServerNode":
                        _ret = d.ConfigValue.MyJsonDeserialize<ServerNode>();
                        if (_ret.Fail)
                        {
                            res.Success = false;
                            res.Result = string.Format("ServerNode【{0}】节点配置无效\r\n", d.KeyName.ToMyString());
                            return res;
                        }
                        ServerNode node = _ret.Result as ServerNode;
                        ModelComLink model = new ModelComLink();
                        node.MapperToModelByAttr<ServerNode, ModelComLink, ServerNodeAttribute>(ref model);
                        model.AppName = SystemDefault.AppName;
                        model.ComLink = "Database";
                        model.Comment = d.Remark;
                        model.LinkState = "0";
                        LstComLink.Add(model);
                        DbFactory.Current.AppandServerDatabase(node);
                        NetDbServer.Add(node);
                        break;
                    case "LocalSource":
                        _ret = d.ConfigValue.MyJsonDeserialize<LocalSource>();
                        if (_ret.Fail)
                        {
                            res.Success = false;
                            res.Result = string.Format("LocalSource【{0}】节点配置无效\r\n", d.KeyName.ToMyString());
                            return res;
                        }
                        LocalSource ds = _ret.Result as LocalSource;
                        DbFactory.Current.AppandSourceDatabase(ds);
                        LocalDbSource.Add(ds);
                        break;
                    case "LogItem":
                        _ret = d.ConfigValue.MyJsonDeserialize<LogItem>();
                        if (_ret.Fail)
                        {
                            res.Success = false;
                            res.Result = string.Format("LogItem【{0}】配置无效\r\n", d.KeyName.ToMyString());
                            return res;
                        }
                        LogItem itm = _ret.Result as LogItem;
                        Mod.FileLogger.Appand(itm);
                        Logger.Add(itm);
                        break;
                }
            }

            //创建ComLink通讯状态表
            try
            {
                if (RunMode == "MasterControl")
                {
                    ModelDriverItem modelDrvItem = new ModelDriverItem() { Domain = SystemDefault.AppName };
                    dt = DbFactory.Current[runServer].ExcuteQuery<ModelDriverItem>(modelDrvItem).Result.ToMyDataTable();
                    List<ModelDriverItem> LstModelDrvItem = ColumnDef.ToEntityList<ModelDriverItem>(dt);
                    foreach (ModelDriverItem item in LstModelDrvItem)
                    {
                        ModelComLink model = new ModelComLink();
                        item.MapperToModelByAttr<ModelDriverItem, ModelComLink, DriverItemAttribute>(ref model);
                        model.AppName = SystemDefault.AppName;
                        model.LinkState = "0";
                        LstComLink.Add(model);
                    }
                    //string tableNameComLnk = TableAttribute.Table<ModelComLink>().Name;
                    //DbFactory.Current[runServer].ExcuteSQL(string.Format("truncate table {0}", tableNameComLnk));
                    ModelComLink modLnk = new ModelComLink() { AppName = SystemDefault.AppName };
                    DbFactory.Current[runServer].ExcuteDelete(modLnk);
                    DbFactory.Current[runServer].ExcuteInsert<ModelComLink>(LstComLink);
                    ComMain<ServerNode>.ComLinkList = LstComLink;
                }
            }
            catch (Exception )
            {

            }

            //将最终的StartUP存储到内存表中
            startup = new ModelAppStartUp()
            {
                AppName = SystemDefault.AppName,
                GroupName = " GroupName in ('Normal')".MarkExpress().MarkWhere()
            };
            dt = DbFactory.Current[runServer].ExcuteQuery<ModelAppStartUp>(startup).Result.ToMyDataTable();
            foreach (DataRow row in dt.Rows)
            {
                string KeyName = row["KeyName"].ToMyString();
                string KeyValue = row["ConfigValue"].ToMyString();
                Normal.SetPropValue<ProjectNormal>(KeyName, KeyValue);
            }
            #endregion

            //返回结果 - 成功
            res.Success = true;
            res.Result = "";
            return res;
        }

        /// <summary>
        /// Bootloader from localsource
        /// </summary>
        /// <param name="runServer"></param>
        /// <returns></returns>
        private CallResult BootLoader(LocalSource runServer)
        {
            CallResult res = new CallResult();
            CallResult _ret;
            List<string> ConfigFile = new List<string>();

            #region 连接主LocalSource,同步数据库StartUP组
            ModelAppStartUp startup = new ModelAppStartUp()
            {
                AppName = SystemDefault.AppName,
                GroupName = "StartUP"
            };
            _ret = DbFactory.Current[runServer].ExcuteQuery<ModelAppStartUp>(startup);
            if (_ret.Fail)
            {
                res.Success = false;
                res.Result = string.Format("工程启动数据库接驳失败！\r\n{0}", _ret.Result.ToMyString());
                return res;
            }
            AppRunLocal = DbFactory.Current[runServer];
            DataTable dt = _ret.Result.ToMyDataTable();
            if (dt.Rows.Count == 0)
            {
                res.Success = false;
                res.Result = "未找到应用程序启动加载项！";
                return res;
            }
            List<ModelAppStartUp> LstStartUpUpdate = new List<ModelAppStartUp>();
            string strPrjSource = string.Empty;
            //StartUP与ini文件设置同步
            foreach (DataRow row in dt.Rows)
            {
                string strKeyName = row["KeyName"].ToMyString();
                string strDataType = row["DataType"].ToMyString();
                string strConfigValue = row["ConfigValue"].ToMyString();
                string strKeyValue = _CurrentPrjfile.ReadValue("StartUP", strKeyName).ToMyString();
                if (strKeyName == "ProjectSource")
                    strPrjSource = strConfigValue;
                StartUP.AppandField(strKeyName, strConfigValue);
                if (string.IsNullOrEmpty(strKeyValue) || strConfigValue == strKeyValue)
                    continue;
                StartUP.AppandField(strKeyName, strKeyValue);
                if (strKeyName == "ProjectSource")
                    strPrjSource = strKeyValue;
                LstStartUpUpdate.Add(new ModelAppStartUp()
                {
                    GroupName = "StartUP".MarkWhere(),
                    KeyName = strKeyName.MarkWhere(),
                    ConfigValue = strKeyValue
                });
            }
            //更新到数据库
            if (LstStartUpUpdate.Count > 0)
            {
                CallResult ret = DbFactory.Current[runServer].ExcuteUpdate<ModelAppStartUp>(LstStartUpUpdate);
                if (ret.Fail)
                {
                    res.Success = false;
                    res.Result = "更新StartUP到数据库失败！";
                    return res;
                }
            }
            #endregion

            #region 解析.prj工程文件内配置 - 并同步到数据库对应组
            LstStartUpUpdate = new List<ModelAppStartUp>();
            string strProjectSource = strPrjSource.FormatPath();
            if (File.Exists(strProjectSource) && !string.IsNullOrEmpty(strProjectSource))
            {
                ConfigFile.Add(strProjectSource);
                string strProjectFile = File.ReadAllText(strProjectSource);
                _ret = strProjectFile.MyJsonDeserialize<Project>();
                if (_ret.Fail)
                {
                    res.Success = false;
                    res.Result = string.Format("工程文件【{0}】解析失败！\r\n{1}", strProjectSource, _ret.Result.ToMyString());
                    return res;
                }
                //更新ProjectFile Normal 到数据库 Normal
                Project proFull = _ret.Result as Project;
                ProjectNormal proNormal = proFull.Normal;
                if (proNormal != null)
                {
                    foreach (PropertyInfo pi in typeof(ProjectNormal).GetProperties())
                    {
                        string strName = pi.Name;
                        string strValue = proNormal.GetPropValue<ProjectNormal>(pi.Name).ToMyString();
                        if (string.IsNullOrEmpty(strValue))
                            continue;
                        LstStartUpUpdate.Add(new ModelAppStartUp()
                        {
                            GroupName = "Normal".MarkWhere(),
                            KeyName = strName.MarkWhere(),
                            ConfigValue = strValue
                        });
                    }
                }
                //更新ProjectFile NetDbServer 到数据库 NetDbServer组
                List<ServerNode> LstServer = proFull.NetDbServer;
                if (LstServer != null)
                {
                    foreach (ServerNode node in LstServer)
                    {
                        string strServerKey = node.ServerName.ToMyString();
                        if (!string.IsNullOrEmpty(strServerKey) && node != null)
                        {
                            LstStartUpUpdate.Add(new ModelAppStartUp()
                            {
                                GroupName = "NetDbServer".MarkWhere(),
                                KeyName = strServerKey.MarkWhere(),
                                ConfigValue = node.MyJsonSerialize().Result.ToMyString()
                            });
                        }
                    }
                }
                //更新ProjectFile LocalDbSource 到数据库 LocalDbSource组
                List<LocalSource> LstDS = proFull.LocalDbSource;
                if (LstDS != null)
                {
                    foreach (LocalSource node in LstDS)
                    {
                        string strSourceKey = node.SourceName.ToMyString();
                        if (!string.IsNullOrEmpty(strSourceKey) && node != null)
                        {
                            LstStartUpUpdate.Add(new ModelAppStartUp()
                            {
                                GroupName = "LocalDbSource".MarkWhere(),
                                KeyName = strSourceKey.MarkWhere(),
                                ConfigValue = node.MyJsonSerialize().Result.ToMyString()
                            });
                        }
                    }
                }
                //更新ProjectFile Logger 到数据库 Logger组
                List<LogItem> LstLdf = proFull.Logger;
                if (LstLdf != null)
                {
                    foreach (LogItem itm in LstLdf)
                    {
                        string strLogKey = itm.LogType.ToMyString();
                        if (!string.IsNullOrEmpty(strLogKey) && itm != null)
                        {
                            LstStartUpUpdate.Add(new ModelAppStartUp()
                            {
                                GroupName = "Logger".MarkWhere(),
                                KeyName = strLogKey.MarkWhere(),
                                ConfigValue = itm.MyJsonSerialize().Result.ToMyString(),
                                Remark = itm.Comment.ToMyString()
                            });
                        }
                    }
                }
            }
            //更新到数据库
            if (LstStartUpUpdate.Count > 0)
            {
                MessageBoxResult mes = sCommon.MyMsgBox("检测到工程配置文件更新，请确认?", MsgType.Question);
                if (mes == MessageBoxResult.No)
                {
                    res.Success = false;
                    res.Result = "用户决策不更新工程配置！";
                    return res;
                }
                CallResult ret = DbFactory.Current[runServer].ExcuteUpdate<ModelAppStartUp>(LstStartUpUpdate);
                if (ret.Fail)
                {
                    res.Success = false;
                    res.Result = "更新工程配置文件到数据库失败！";
                    return res;
                }
            }
            #endregion

            #region 解析.prj工程外部配置文件 - Normal组涉及的外部文件,按文件类型逐个解析并同步数据库
            startup = new ModelAppStartUp()
            {
                AppName = SystemDefault.AppName,
                GroupName = "Normal",
                DataType = "textfile"
            };

            LstStartUpUpdate = new List<ModelAppStartUp>();
            dt = DbFactory.Current[runServer].ExcuteQuery<ModelAppStartUp>(startup).Result.ToMyDataTable();
            foreach (DataRow row in dt.Rows)
            {
                string strFilePath = row["ConfigValue"].ToMyString().FormatPath();
                if (!File.Exists(strFilePath))
                    continue;
                ConfigFile.Add(strFilePath);
                string strFileContent = File.ReadAllText(strFilePath);
                if (string.IsNullOrEmpty(strFileContent))
                    continue;
                _ret = strFileContent.MyJsonDeserialize<Project>();
                if (_ret.Fail)
                {
                    res.Success = false;
                    res.Result = string.Format("文件解析【{0}】失败！\r\n{1}", strFilePath, _ret.Result.ToMyString());
                    return res;
                }
                Project PrjLoader = _ret.Result as Project;
                string strFileType = strFilePath.GetExtension();
                switch (strFileType)
                {
                    case ".nd":     //ServerNode文件 更新到 NetDbServer组
                        List<ServerNode> LstServer = PrjLoader.NetDbServer;
                        if (LstServer != null)
                        {
                            foreach (ServerNode node in LstServer)
                            {
                                string strServerKey = node.ServerName.ToMyString();
                                if (!string.IsNullOrEmpty(strServerKey) && node != null)
                                {
                                    LstStartUpUpdate.Add(new ModelAppStartUp()
                                    {
                                        KeyName = strServerKey.MarkWhere(),
                                        ConfigValue = node.MyJsonSerialize().Result.ToMyString()
                                    });
                                }
                            }
                        }
                        break;

                    case ".ls":     //LocalSource文件 到数据库 LocalDbSource组
                        List<LocalSource> LstDS = PrjLoader.LocalDbSource;
                        if (LstDS != null)
                        {
                            foreach (LocalSource node in LstDS)
                            {
                                string strSourceKey = node.SourceName.ToMyString();
                                if (!string.IsNullOrEmpty(strSourceKey) && node != null)
                                {
                                    LstStartUpUpdate.Add(new ModelAppStartUp()
                                    {
                                        KeyName = strSourceKey.MarkWhere(),
                                        ConfigValue = node.MyJsonSerialize().Result.ToMyString()
                                    });
                                }
                            }
                        }
                        break;

                    case ".lgf":    //日志配置文件  到数据库 Logger组
                        List<LogItem> LstLogPath = PrjLoader.Logger;
                        if (LstLogPath != null)
                        {
                            foreach (LogItem itm in LstLogPath)
                            {
                                string strLogKey = itm.LogType.ToMyString();
                                string strLogPath = itm.FilePath.ToMyString();
                                if (!string.IsNullOrEmpty(strLogKey))
                                {
                                    LstStartUpUpdate.Add(new ModelAppStartUp()
                                    {
                                        KeyName = strLogKey.MarkWhere(),
                                        ConfigValue = itm.MyJsonSerialize().Result.ToMyString()
                                    });
                                }
                            }
                        }
                        break;

                    case ".prj":   //工程文件
                    default:
                        continue;
                }
            }

            //更新到数据库
            if (LstStartUpUpdate.Count > 0)
            {
                MessageBoxResult mes = sCommon.MyMsgBox("检测到工程关联文件更新，请确认?", MsgType.Question);
                if (mes == MessageBoxResult.No)
                {
                    res.Success = false;
                    res.Result = "用户决策不更新工程关联配置！";
                    return res;
                }

                CallResult ret = DbFactory.Current[runServer].ExcuteUpdate<ModelAppStartUp>(LstStartUpUpdate);
                if (ret.Fail)
                {
                    res.Success = false;
                    res.Result = "更新工程外部配置文件到数据库失败！";
                    return res;
                }
            }
            #endregion

            #region 删除更新文件
            if (ConfigFile.Count > 0)
            {
                foreach (string file in ConfigFile)
                    File.Delete(file);
            }
            #endregion

            #region 初始化工程加载项
            List<ModelComLink> LstComLink = new List<ModelComLink>();
            startup = new ModelAppStartUp()
            {
                AppName = SystemDefault.AppName,
                DataType = "DataType in ('ServerNode','LocalSource','LogItem')".MarkExpress().MarkWhere()
            };
            _ret = DbFactory.Current[runServer].ExcuteQuery<ModelAppStartUp>(startup);
            if (_ret.Fail)
            {
                res.Success = false;
                res.Result = string.Format("工程资源加载失败！\r\n原因:{0}", _ret.Result.ToMyString());
                return res;
            }
            dt = _ret.Result.ToMyDataTable();
            foreach (DataRow row in dt.Rows)
            {
                ModelAppStartUp d = sCommon.ToEntity<ModelAppStartUp>(row);
                if (string.IsNullOrEmpty(d.KeyName) || string.IsNullOrEmpty(d.ConfigValue))
                    continue;

                switch (d.DataType.ToMyString())
                {
                    case "ServerNode":
                        _ret = d.ConfigValue.MyJsonDeserialize<ServerNode>();
                        if (_ret.Fail)
                        {
                            res.Success = false;
                            res.Result = string.Format("ServerNode【{0}】节点配置无效\r\n", d.KeyName.ToMyString());
                            return res;
                        }
                        ServerNode node = _ret.Result as ServerNode;
                        ModelComLink model = new ModelComLink();
                        node.MapperToModelByAttr<ServerNode, ModelComLink, ServerNodeAttribute>(ref model);
                        model.AppName = SystemDefault.AppName;
                        model.ComLink = "Database";
                        model.Comment = d.Remark;
                        model.LinkState = "0";
                        LstComLink.Add(model);
                        DbFactory.Current.AppandServerDatabase(node);
                        NetDbServer.Add(node);
                        break;
                    case "LocalSource":
                        _ret = d.ConfigValue.MyJsonDeserialize<LocalSource>();
                        if (_ret.Fail)
                        {
                            res.Success = false;
                            res.Result = string.Format("LocalSource【{0}】节点配置无效\r\n", d.KeyName.ToMyString());
                            return res;
                        }
                        LocalSource ds = _ret.Result as LocalSource;
                        DbFactory.Current.AppandSourceDatabase(ds);
                        LocalDbSource.Add(ds);
                        break;
                    case "LogItem":
                        _ret = d.ConfigValue.MyJsonDeserialize<ServerNode>();
                        if (_ret.Fail)
                        {
                            res.Success = false;
                            res.Result = string.Format("LogItem【{0}】配置无效\r\n", d.KeyName.ToMyString());
                            return res;
                        }
                        LogItem itm = _ret.Result as LogItem;
                        Mod.FileLogger.Appand(itm);
                        Logger.Add(itm);
                        break;
                }
            }

            //创建ComLink通讯状态表
            try
            {
                if (RunMode == "MasterControl")
                {
                    ModelDriverItem modelDrvItem = new ModelDriverItem() { Domain = SystemDefault.AppName };
                    dt = DbFactory.Current[runServer].ExcuteQuery<ModelDriverItem>(modelDrvItem).Result.ToMyDataTable();
                    List<ModelDriverItem> LstModelDrvItem = ColumnDef.ToEntityList<ModelDriverItem>(dt);
                    foreach (ModelDriverItem item in LstModelDrvItem)
                    {
                        ModelComLink model = new ModelComLink();
                        item.MapperToModelByAttr<ModelDriverItem, ModelComLink, DriverItemAttribute>(ref model);
                        model.AppName = SystemDefault.AppName;
                        model.LinkState = "0";
                        LstComLink.Add(model);
                    }
                    //string tableNameComLnk = TableAttribute.Table<ModelComLink>().Name;
                    //DbFactory.Current[runServer].ExcuteSQL(string.Format("truncate table {0}", tableNameComLnk));
                    ModelComLink modLnk = new ModelComLink() { AppName = SystemDefault.AppName };
                    DbFactory.Current[runServer].ExcuteDelete(modLnk);
                    DbFactory.Current[runServer].ExcuteInsert<ModelComLink>(LstComLink);
                    ComMain<LocalSource>.ComLinkList = LstComLink;
                }
            }
            catch (Exception )
            {

            }

            //将最终的StartUP存储到内存表中
            startup = new ModelAppStartUp()
            {
                AppName = SystemDefault.AppName,
                GroupName = " GroupName in ('Normal')".MarkExpress().MarkWhere()
            };
            dt = DbFactory.Current[runServer].ExcuteQuery<ModelAppStartUp>(startup).Result.ToMyDataTable();
            foreach (DataRow row in dt.Rows)
            {
                string KeyName = row["KeyName"].ToMyString();
                string KeyValue = row["ConfigValue"].ToMyString();
                Normal.SetPropValue<ProjectNormal>(KeyName, KeyValue);
            }
            #endregion

            //返回结果 - 成功
            res.Success = true;
            res.Result = "";
            return res;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private CallResult BootLoader()
        {
            return null;
        }
        #endregion
    }
}
