using Engine.ComDriver;
using Engine.ComDriver.HEAO;
using Engine.Common;
using Engine.Core.TaskSchedule;
using Engine.Data;
using Engine.Data.DBFAC;
using Engine.Mod;
using Engine.MVVM.Messaging;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Runtime.CompilerServices;

namespace Engine.Core
{
    /// <summary>
    /// 后端访问器 - 实例、定义
    /// </summary>
    public partial class Accessor: DynamicEntityBase
    {
        private IDBFactory<ServerNode> _DB = DbFactory.CPU;
        public static readonly string PosItemMessageChanged = SystemDefault.UUID;

        private static Accessor _Current;
        public static Accessor Current
        {
            get
            {
                if (_Current == null)
                    _Current = new Accessor();
                return _Current;
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public Accessor() { }

        /// <summary>
        /// 拷贝构造函数
        /// </summary>
        private Accessor(IDBFactory<ServerNode> DataSource)
        {
            _DB = DataSource;
        }
        
        ///// <summary>
        ///// 新的连接上克隆一个新对象，
        ///// 为了并发访问提高效率
        ///// </summary>
        ///// <param name="DataSource"></param>
        ///// <returns></returns>
        //public Accessor CloneInstance(IDBFactory<ServerNode> DataSource)
        //{
        //    return new Accessor(DataSource);
        //}
    }

    /// <summary>
    /// 后端访问器 - 控制器，协议表
    /// </summary>
    public partial class Accessor
    {
        /// <summary>
        /// 获取视图
        /// </summary>
        /// <param name="PosKey"></param>
        /// <param name="ItemKey"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public List<ModelDataView> ReadDataViewBySymbol(string PosKey, string ItemKey)
        {
            List<ModelDataView> LstModelView = new List<ModelDataView>();
            string strLogicExpress = ReadPosItem(PosKey, ItemKey, "LogicExpress");
            if (string.IsNullOrEmpty(strLogicExpress))
                return LstModelView;

            List<string> LstFuncExpress = strLogicExpress.MySplit("|");
            foreach (string exp in LstFuncExpress)
            {
                string strFuncName = exp.MidString("", "(").Trim();
                string strFuncParam = exp.MidString("(", ")").Trim();
                List<string> ArrFuncParam = strFuncParam.MySplit(",");
                CallResult getCallResult = new CallResult() { Success = false };
                if (strFuncName == "GetView" && ArrFuncParam.Count == 2)
                {
                    List<string> LstRelatedDriver = ArrFuncParam[0].MySplit("&");
                    foreach (string relDriver in LstRelatedDriver)
                    {
                        getCallResult = ReadDataViewByDriver(relDriver, PosKey, ArrFuncParam[1]);
                        if (getCallResult.Success)
                        {
                            DataTable getTable = getCallResult.Result.ToMyDataTable();
                            if (getTable != null)
                            {
                                List<ModelDataView> lstdataView = ColumnDef.ToEntityList<ModelDataView>(getTable);
                                LstModelView.AddRange(lstdataView);
                            }
                        }
                    }
                }
            }
            return LstModelView;
        }

        /// <summary>
        /// 获取协议行记录集
        /// </summary>
        /// <param name="DriverName"></param>
        /// <param name="RelatedPos">关联工位，多个工位用&分割</param>
        /// <param name="DataUnitType">多项式用&连接 Status Alarm Command Data Temp</param>
        /// <returns>DataTable</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public CallResult ReadDataViewByDriver(string DriverName, string RelatedPos, string DataUnitType)
        {
            ModelDriverItem drv = ReadDriverItem(DriverName);
            List<string> LstRelatedPos = RelatedPos.MySplit("&");
            List<string> LstDataUnitType = DataUnitType.MySplit("&");
            string strRelatedPos = string.Empty;
            string strDataUnitType = string.Empty;
            foreach (string item in LstRelatedPos)
            {
                if (!string.IsNullOrEmpty(strRelatedPos))
                    strRelatedPos += ",";
                strRelatedPos += $"'{item}'";
            }
            foreach (string item in LstDataUnitType)
            {
                if (!string.IsNullOrEmpty(strDataUnitType))
                    strDataUnitType += ",";
                strDataUnitType += $"'{item}'";
            }
            string strWhere = $"DriverToken = '{drv.DriverToken}' and Comment not in('') and Comment is not null";
            if (!string.IsNullOrEmpty(strDataUnitType))
                strWhere += $" and DataUnitType in({strDataUnitType})";
            if (!string.IsNullOrEmpty(RelatedPos))
                strWhere += $" and RelatedGroup in({strRelatedPos})";
            strWhere += " order by OrderID asc";

            string strComLibKey = drv.ComLibTable.ToMyString().ToLower();
            if (strComLibKey.Contains("driver_heaointer"))
            {
                ModelComHeao modelHeao = new ModelComHeao()
                {
                    DriverToken = strWhere.MarkExpress().MarkWhere()
                };
                return _DB.ExcuteQuery<ModelComHeao>(modelHeao);
            }
            else if (strComLibKey.Contains("driver_heaointer"))
            {
                ModelComPLC modelPLC = new ModelComPLC()
                {
                    DriverToken = strWhere.MarkExpress().MarkWhere()
                };
                return _DB.ExcuteQuery<ModelComPLC>(modelPLC);
            }
            else if (strComLibKey.Contains("driver_instr"))
            {
                ModelComInstr modelInstr = new ModelComInstr()
                {
                    DriverToken = strWhere.MarkExpress().MarkWhere()
                };
                return _DB.ExcuteQuery<ModelComInstr>(modelInstr);
            }
            //switch (drv.Protocol)
            //{
            //    case "HeaoInter":
            //        ModelComHeao modelHeao = new ModelComHeao()
            //        {
            //            DriverToken = strWhere.MarkExpress().MarkWhere()
            //        };
            //        return _DB.ExcuteQuery<ModelComHeao>(modelHeao);

            //    case "S71200":
            //    case "S71500":
            //    case "S7200SMART":
            //    case "S7300":
            //    case "S7400":
            //        ModelComPLC modelPLC = new ModelComPLC()
            //        {
            //            DriverToken = strWhere.MarkExpress().MarkWhere()
            //        };
            //        return _DB.ExcuteQuery<ModelComPLC>(modelPLC);

            //}
            return new CallResult() { Success = false };
        }

        /// <summary>
        /// 获取指定控制器
        /// </summary>
        /// <param name="DriverName"></param>
        /// <returns></returns>
        public ModelDriverItem ReadDriverItem(string DriverName)
        {
            ModelDriverItem modelDrvItem = new ModelDriverItem()
            {
                Domain = SystemDefault.AppName,
                Name = DriverName
            };
            CallResult _result = _DB.ExcuteQuery<ModelDriverItem>(modelDrvItem);
            if (_result.Success)
            {
                DataTable dt = _result.Result.ToMyDataTable();
                if (dt.Rows.Count > 0)
                    modelDrvItem = ColumnDef.ToEntity<ModelDriverItem>(dt.Rows[0]);
            }
            return modelDrvItem;
        }

        /// <summary>
        /// 读取工位参数
        /// </summary>
        /// <param name="PosItem">工位项</param>
        /// <returns></returns>
        public string ReadPosItem(string PosItem)
        {
            string strPosKey = PosItem.MidString("", ".", EndStringSearchMode.FromTail);
            string strPosItem = PosItem.MidString(strPosKey + ".", "");
            return ReadPosItem(strPosKey, strPosItem);
        }

        /// <summary>
        /// 读取工位参数
        /// </summary>
        /// <param name="PosName">工位名称</param>
        /// <param name="ItemName">项名称</param>
        /// <param name="ParamKey">参数名称</param>
        /// <returns></returns>
        public string ReadPosItem(string PosName, string ItemName, string ParamKey = "CurrentValue")
        {
            ModelSystemSymbol TrySymbol = new ModelSystemSymbol() { GroupName = PosName, Name = ItemName };
            CallResult _result = _DB.ExcuteQuery<ModelSystemSymbol>(TrySymbol);
            if (_result.Success)
            {
                DataTable dt = _result.Result.ToMyDataTable();
                if (dt.Rows.Count > 0)
                    return dt.Rows[0][ParamKey].ToMyString();
            }
            return string.Empty;
        }

        /// <summary>
        /// 读取工作域报警
        /// </summary>
        /// <param name="WorkDomain"></param>
        /// <returns></returns>
        public bool ReadAlarmState(string WorkDomain = "")
        {
            ModelSystemSymbol systemgroup = new ModelSystemSymbol()
            {
                WorkDomain = WorkDomain,
                Name = "POS_STATE",
                CurrentValue = "ALARM"
            };
            CallResult _result = _DB.ExcuteQuery<ModelSystemSymbol>(systemgroup);
            if (_result.Success)
            {
                DataTable dt = _result.Result.ToMyDataTable();
                return dt.Rows.Count > 0;
            }
            return false;
        }

        /// <summary>
        /// 读取分组列表
        /// </summary>
        /// <param name="PosName"></param>
        /// <param name="ItemType">默认：Name</param>
        /// <returns></returns>
        public List<string> ReadPosItemList(string PosName, string ItemType = "Name")
        {
            ModelSystemSymbol TrySymbol = new ModelSystemSymbol() { GroupName = PosName };
            List<string> PosItems = new List<string>();
            CallResult _result = _DB.ExcuteQuery<ModelSystemSymbol>(TrySymbol);
            if (_result.Success)
            {
                DataTable dt = _result.Result.ToMyDataTable();
                foreach (DataRow row in dt.Rows)
                    PosItems.Add(row[ItemType].ToMyString());
            }
            return PosItems;
        }

        /// <summary>
        /// 读取符号列表
        /// </summary>
        /// <param name="GroupName"></param>
        /// <param name="Name"></param>
        /// <param name="DataType"></param>
        /// <returns></returns>
        public List<ModelSystemSymbol> ReadSymbolList(string GroupName,string Name,string DataType)
        {
            List<ModelSystemSymbol> LstSymbol = new List<ModelSystemSymbol>();
            ModelSystemSymbol TrySymbol = new ModelSystemSymbol()
            {
                GroupName = GroupName,
                Name = Name,
                DataType = DataType
            };
            CallResult _result = _DB.ExcuteQuery<ModelSystemSymbol>(TrySymbol);
            if (_result.Success)
            {
                DataTable dt = _result.Result.ToMyDataTable();
                LstSymbol = ColumnDef.ToEntityList<ModelSystemSymbol>(dt);
            }
            return LstSymbol;
        }

        /// <summary>
        /// 读取符号项
        /// </summary>
        /// <param name="GroupName"></param>
        /// <param name="Name"></param>
        /// <param name="DataType"></param>
        /// <returns></returns>
        public ModelSystemSymbol ReadSymbolItem(string GroupName, string Name, string DataType)
        {
            ModelSystemSymbol TrySymbol = new ModelSystemSymbol()
            {
                GroupName = GroupName,
                Name = Name,
                DataType = DataType
            };
            CallResult _result = _DB.ExcuteQuery<ModelSystemSymbol>(TrySymbol);
            if (_result.Success)
            {
                DataTable dt = _result.Result.ToMyDataTable();
                if (dt.Rows.Count > 0)
                    TrySymbol = ColumnDef.ToEntity<ModelSystemSymbol>(dt.Rows[0]);
            }
            return TrySymbol;
        }

        /// <summary>
        /// 写入工位参数
        /// </summary>
        /// <param name="PosName"></param>
        /// <param name="ItemName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool WritePosItem(string PosName, string ItemName, string value)
        {
            ModelSystemSymbol TrySymbol = new ModelSystemSymbol()
            {
                GroupName = PosName.MarkWhere(),
                Name = ItemName.MarkWhere(),
                CurrentValue = value.ValueAttachMark()
            };
            CallResult _result = _DB.ExcuteUpdate<ModelSystemSymbol>(TrySymbol);
            if (PosName.Contains(".") && ItemName == "VIEW_SI")
            {
                Messenger.Default.Send<string[]>(new string[] { PosName, string.IsNullOrEmpty(value) ? "Remove" : "Appand" }, PosItemMessageChanged);
            }
            return _result.Success;
        }


        /// <summary>
        /// 写入工位参数
        /// </summary>
        /// <param name="PosItem"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool WritePosItem(string PosItem, string value)
        {
            string strPosKey = PosItem.MidString("", ".", EndStringSearchMode.FromTail);
            string strPosItem = PosItem.MidString(strPosKey + ".", "");
            return WritePosItem(strPosKey, strPosItem, value);
        }

        /// <summary>
        /// 读取通讯变量
        /// </summary>
        /// <param name="DriverName">控制器名称</param>
        /// <param name="DataName">数据项名称</param>
        /// <returns></returns>
        public ModelComInstr ReadCommVarible(string DriverName,string DataName)
        {
            ModelDriverItem modDriver = ReadDriverItem(DriverName);
            string strComLibTable = modDriver.ComLibTable.ToMyString();
            ModelComInstr modCom = default(ModelComInstr);
            DataTable dt = new DataTable();
            if (strComLibTable.Contains("driver_heaointer"))
            {
                ModelComHeao com = new ModelComHeao()
                {
                    DriverToken = modDriver.DriverToken,
                    DataName = DataName
                };
                 dt =_DB.ExcuteQuery<ModelComHeao>(com).Result.ToMyDataTable();
            }
            else if (strComLibTable.Contains("driver_plc"))
            {
                ModelComPLC com = new ModelComPLC()
                {
                    DriverToken = modDriver.DriverToken,
                    DataName = DataName
                };
                 dt = _DB.ExcuteQuery<ModelComPLC>(com).Result.ToMyDataTable();
            }
            else if (strComLibTable.Contains("driver_instr"))
            {
                ModelComInstr com = new ModelComInstr()
                {
                    DriverToken = modDriver.DriverToken,
                    DataName = DataName
                };
                 dt = _DB.ExcuteQuery<ModelComInstr>(com).Result.ToMyDataTable();
            }
            if (dt.Rows.Count > 0)
                modCom = ColumnDef.ToEntity<ModelComInstr>(dt.Rows[0]);
            return modCom;
        }

        /// <summary>
        /// 读取通讯变量
        /// </summary>
        /// <param name="DriverName">控制器名称</param>
        /// <param name="LstDataName">数据项列表</param>
        /// <returns></returns>
        public List<ModelComInstr> ReadCommVaribleList(string DriverName, List<string> LstDataName)
        {
            ModelDriverItem modDriver = ReadDriverItem(DriverName);
            string strComLibTable = modDriver.ComLibTable.ToMyString();
            List<ModelComInstr> LstCom = default(List<ModelComInstr>);
            DataTable dt = new DataTable();
            string strWhereDataName = LstDataName.ToMyString(",", true, "'", "'");
            if (strComLibTable.Contains("driver_heaointer"))
            {
                ModelComHeao com = new ModelComHeao()
                {
                    DriverToken = modDriver.DriverToken,
                    DataName = string.Format(" DataName in ({0})",
                            strWhereDataName).MarkExpress().MarkWhere() 
                };
                dt = _DB.ExcuteQuery<ModelComHeao>(com).Result.ToMyDataTable();
            }
            else if (strComLibTable.Contains("driver_plc"))
            {
                ModelComPLC com = new ModelComPLC()
                {
                    DriverToken = modDriver.DriverToken,
                    DataName = string.Format(" DataName in ({0})",
                            strWhereDataName).MarkExpress().MarkWhere()
                };
                dt = _DB.ExcuteQuery<ModelComPLC>(com).Result.ToMyDataTable();
            }
            else if (strComLibTable.Contains("driver_instr"))
            {
                ModelComInstr com = new ModelComInstr()
                {
                    DriverToken = modDriver.DriverToken,
                    DataName = string.Format(" DataName in ({0})",
                            strWhereDataName).MarkExpress().MarkWhere()
                };
                dt = _DB.ExcuteQuery<ModelComInstr>(com).Result.ToMyDataTable();
            }
            if (dt.Rows.Count > 0)
                LstCom = ColumnDef.ToEntityList<ModelComInstr>(dt);
            return LstCom;
        }

        /// <summary>
        /// 向符号代理写入请求
        /// </summary>
        /// <param name="PosName">目标工位</param>
        /// <param name="CmdItem">工位项</param>
        /// <param name="SetValue">设定值</param>
        /// <param name="IsTaskCommand">是否任务命令</param>
        /// <param name="TaskCmdName">任务名称</param>
        /// <returns></returns>
        public bool WriteCommand(string PosName, string CmdItem, string SetValue = "", bool IsTaskCommand = false, string TaskCmdName = "")
        {
            ModelSystemSymbol TrySymbol = new ModelSystemSymbol()
            {
                GroupName = PosName,
                Name = CmdItem,
                DataType = "Command"
            };
            if (!string.IsNullOrEmpty(SetValue))
            {
                TrySymbol.GroupName = PosName.MarkWhere();
                TrySymbol.Name = CmdItem.MarkWhere();
                TrySymbol.DataType = "Command".MarkWhere();
                TrySymbol.SetValue = SetValue;
                CallResult _resSureCmd = _DB.ExcuteUpdate<ModelSystemSymbol>(TrySymbol);
                if (_resSureCmd.Fail)
                    return false;
            }
            CallResult _resGetCmd = _DB.ExcuteQuery<ModelSystemSymbol>(TrySymbol);
            if (_resGetCmd.Fail)
                return false;
            DataTable dt = _resGetCmd.Result.ToMyDataTable();
            if (dt.Rows.Count > 0)
            {
                ModelSystemSymbol symbol = ColumnDef.ToEntity<ModelSystemSymbol>(dt.Rows[0]);
                return WriteCommand(symbol, IsTaskCommand, TaskCmdName);
            }
            else
                return false;
        }

        /// <summary>
        /// 通过目标符号代理向目标控制器写入指令
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="IsTaskCommand">是否任务命令</param>
        /// <param name="TaskCmdName">任务名称</param>
        /// <returns></returns>
        public bool WriteCommand(ModelSystemSymbol symbol, bool IsTaskCommand = false,string TaskCmdName = "")
        {
            bool WriteSuccess = false;
            if (symbol.DataType != "Command")
                return false;
            string strRelatedDriverName = symbol.RelatedDriver.ToMyString();
            string strRelatedVariable = symbol.GroupName.ToMyString();
            string strSetValue = symbol.SetValue.ToMyString();
            strRelatedVariable += string.Format(".{0}", symbol.Name.ToMyString());
            WriteSuccess = WriteCommand(strRelatedDriverName, strRelatedVariable, strSetValue, 0, IsTaskCommand, TaskCmdName);
            if (!WriteSuccess)
                return false;
            //解析指令逻辑表达式及设定值表达式
            string strLogicExpress = symbol.LogicExpress.ToMyString();
            List<string> LstFuncExpress = strLogicExpress.MySplit("|");
            foreach (string exp in LstFuncExpress)
            {
                string strFuncName = exp.MidString("", "(");
                string strFuncParam = exp.MidString("(", ")");
                List<string> ArrFuncParam = strFuncParam.MySplit(",");

                if (strFuncName == "WriteCmd" && ArrFuncParam.Count == 2)
                {
                    WriteSuccess = WriteCommand(ArrFuncParam[0], ArrFuncParam[1], strSetValue, 1, IsTaskCommand, TaskCmdName);
                }
            }
            return WriteSuccess;
        }

        /// <summary>
        /// 根据数据关联变量写命令
        /// </summary>
        /// <param name="DriverName"></param>
        /// <param name="RelatedVariable"></param>
        /// <param name="SetValue"></param>
        /// <returns></returns>
        public bool WriteCommandByRelatedVariable(string DriverName, string RelatedVariable, string SetValue = "",
            bool IsTaskCommand = false, string TaskCmdName = "")
        {
            return WriteCommand(DriverName, RelatedVariable, SetValue, 0, IsTaskCommand, TaskCmdName);
        }

        /// <summary>
        /// 根据数据名称写命令
        /// </summary>
        /// <param name="DriverName"></param>
        /// <param name="DataName"></param>
        /// <param name="SetValue"></param>
        /// <param name="IsTaskCommand"></param>
        /// <param name="TaskCmdName"></param>
        /// <returns></returns>
        public bool WriteCommandByDataName(string DriverName, string DataName, string SetValue = "", 
            bool IsTaskCommand = false, string TaskCmdName = "")
        {
            return WriteCommand(DriverName, DataName, SetValue, 1, IsTaskCommand, TaskCmdName);
        }

        /// <summary>
        /// 根据数据ID写命令
        /// </summary>
        /// <param name="DriverName"></param>
        /// <param name="Token"></param>
        /// <param name="SetValue"></param>
        /// <returns></returns>
        public bool WriteCommandByCmdToken(string DriverName, string Token, string SetValue = "",
            bool IsTaskCommand = false, string TaskCmdName = "")
        {
            return WriteCommand(DriverName, Token, SetValue, 2, IsTaskCommand, TaskCmdName);
        }

        /// <summary>
        /// 向目标控制器写入指令
        /// </summary>
        /// <param name="DriverName">关联控制器</param>
        /// <param name="RelatedCmdKey">关联命令Key</param>
        /// <param name="SetValue">设定值</param>
        /// <param name="WriteByRelatedVariableOrDataName">0:关联变量  1:数据名称 2: Token</param>
        /// <returns></returns>
        private bool WriteCommand(string DriverName, string RelatedCmdKey, string SetValue, int WriteBy = 0,
            bool IsTaskCommand = false, string TaskCmdName = "")
        {
            bool WriteResult = false;
            if (string.IsNullOrEmpty(DriverName) || string.IsNullOrEmpty(RelatedCmdKey))
                return WriteResult;
            //读取控制器信息 - 获取指令写入入口
            ModelDriverItem modelDrvItem = ReadDriverItem(DriverName);
            string strDataValue = string.Empty;
            string strDestDbTable = string.Empty;
            if (modelDrvItem != null)
                strDestDbTable = modelDrvItem.ComLibTable;
            if (string.IsNullOrEmpty(strDestDbTable))
                return WriteResult;

            //将有效之写入命令接口
            if (strDestDbTable.Contains("driver_heaointer") && !string.IsNullOrEmpty(SetValue))
            {
                string strDecLE = (SetValue.Length / 2 + 1).ToString();
                strDataValue = string.Format("DataValue = '{0}',DataLen='{1}',", SetValue, strDecLE);
            }
            else if (strDestDbTable.Contains("driver_plc") && !string.IsNullOrEmpty(SetValue))
            {
                strDataValue = string.Format("DataWrite = '{0}',", SetValue);
            }
            else if (strDestDbTable.Contains("driver_instr") && !string.IsNullOrEmpty(SetValue))
            {
                strDataValue = string.Format("DataValue = '{0}',", SetValue);
            }
            string strWhere = string.Empty;
            string strAwaitState = "Sending";
            if (IsTaskCommand)
                strAwaitState = "TaskSending";
            if (WriteBy == 0)
                strWhere = string.Format(" where RelatedVariable = '{0}' and DriverToken='{1}'", RelatedCmdKey, modelDrvItem.DriverToken.ToMyString());
            else if (WriteBy == 1)
                strWhere = string.Format(" where DataName = '{0}' and DriverToken='{1}'", RelatedCmdKey, modelDrvItem.DriverToken.ToMyString());
            else if (WriteBy == 2)
                strWhere = string.Format(" where Token = '{0}' and DriverToken='{1}'", RelatedCmdKey, modelDrvItem.DriverToken.ToMyString());
            string strSql = string.Format("update {0} set {1} AwaitState='{2}',AwaitName='{3}',AwaitTime='{4}' {5}",
                strDestDbTable, strDataValue, strAwaitState, TaskCmdName, SystemDefault.StringTimeNow, strWhere);
            return _DB.ExcuteSQL(strSql).Success;
        }

        /// <summary>
        /// TaskDriver任务驱动项
        /// 任务执行后，通讯器反馈任务指令更新
        /// </summary>
        /// <param name="DriverName"></param>
        /// <param name="TaskName"></param>
        /// <param name="ValueSet"></param>
        /// <returns></returns>
        public bool WriteTaskDriverValue(string DriverName,string TaskName,string ValueSet = "")
        {
            ModelSystemSymbol model = new ModelSystemSymbol()
            {
                DataType = "TaskDriver".MarkWhere(),
                RelatedDriver = DriverName.MarkWhere(),
                Name = TaskName.MarkWhere(),
                CurrentValue = string.IsNullOrEmpty(ValueSet) ? SystemDefault.StringEmpty : ValueSet
            };
            return _DB.ExcuteUpdate<ModelSystemSymbol>(model).Success;
        }
    }

    /// <summary>
    /// 后端访问器 - Symbol符号
    /// </summary>
    public partial class Accessor
    {
        /// <summary>
        /// 获取符号组对象
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public ModelSystemGroup GetSymbolGroup(string groupName)
        {
            ModelSystemGroup modelGroup = new ModelSystemGroup() { MarkKey = SystemGroupType.Symbol.ToString(), Name = groupName };
            CallResult _result = _DB.ExcuteQuery<ModelSystemGroup>(modelGroup);
            if (_result.Success)
            {
                DataTable dt = _result.Result.ToMyDataTable();
                if (dt.Rows.Count > 0)
                    modelGroup = ColumnDef.ToEntity<ModelSystemGroup>(dt.Rows[0]);
            }
            return modelGroup;
        }
    }

    /// <summary>
    /// 后端访问器 - Sheet表格
    /// </summary>
    public partial class Accessor
    {
        /// <summary>
        /// 获取Sheet组对象
        /// </summary>
        /// <param name="SheetName"></param>
        /// <returns></returns>
        public ModelSystemGroup GetSheetGroup(string SheetName)
        {
            ModelSystemGroup modelGroup = new ModelSystemGroup()
            {
                MarkKey = SystemGroupType.DataSheet.ToString(),
                Name = SheetName
            };
            CallResult _result = _DB.ExcuteQuery<ModelSystemGroup>(modelGroup);
            if (_result.Success)
            {
                DataTable dt = _result.Result.ToMyDataTable();
                if (dt.Rows.Count > 0)
                    modelGroup = ColumnDef.ToEntity<ModelSystemGroup>(dt.Rows[0]);
            }
            return modelGroup;
        }

        /// <summary>
        /// 获取Sheet列集合
        /// </summary>
        /// <param name="SheetName"></param>
        /// <returns></returns>
        public List<ModelSheetColumn> GetSheetColumns(string SheetName)
        {
            ModelSystemGroup modelGroup = GetSheetGroup(SheetName);
            List<ModelSheetColumn> SheetColumns = new List<ModelSheetColumn>();
            if (string.IsNullOrEmpty(modelGroup.Token))
                return SheetColumns;
            ModelSheetColumn modelSheet = new ModelSheetColumn() { GroupToken = modelGroup.Token };
            CallResult _result = _DB.ExcuteQuery<ModelSheetColumn>(modelSheet);
            if (_result.Success)
            {
                DataTable dt = _result.Result.ToMyDataTable();
                SheetColumns = ColumnDef.ToEntityList<ModelSheetColumn>(dt);
            }
            return SheetColumns;
        }

        /// <summary>
        /// 更新Sheet表列参数
        /// </summary>
        /// <param name="SheetName"></param>
        /// <param name="SheetColumns"></param>
        /// <returns></returns>
        public bool UpdateSheetColumns(string SheetName,List<ModelSheetColumn> SheetColumns)
        {
            if (SheetColumns == null)
                return false;
            ModelSystemGroup modelGroup = GetSheetGroup(SheetName);
            for (int i = 0; i < SheetColumns.Count; i++)
            { 
                SheetColumns[i].Token = modelGroup.Token.MarkExpress();
                SheetColumns[i].ColHeader = SheetColumns[i].ColHeader.MarkExpress();
            }
            return _DB.ExcuteUpdate<ModelSheetColumn>(SheetColumns).Success;
        }
    }

    /// <summary>
    /// 后端访问器 - 任务管理、工位及子工位管理
    /// </summary>
    public partial class Accessor
    {
        /// <summary>
        /// 获取工位列表
        /// </summary>
        /// <returns></returns>
        public List<ModelSystemGroup> GetPosItems()
        {
            List<ModelSystemGroup> LstPosGroup = new List<ModelSystemGroup>();
            ModelSystemGroup modelGroup = new ModelSystemGroup() {
                MarkKey = string.Format(" MarkKey = '{0}' order by OrderID asc", SystemGroupType.Symbol.ToString())
                .MarkExpress().MarkWhere()
            };
            CallResult _result = _DB.ExcuteQuery<ModelSystemGroup>(modelGroup);
            if (_result.Success)
            {
                DataTable dt = _result.Result.ToMyDataTable();
                if (dt.Rows.Count > 0)
                    LstPosGroup = ColumnDef.ToEntityList<ModelSystemGroup>(dt);
            }
            return LstPosGroup;
        }

        /// <summary>
        /// 获取子工位项
        /// </summary>
        /// <param name="strSampleID"></param>
        /// <returns></returns>
        public ModelSubPos GetSubPosItem(ModelSubPos modelWhere)
        {
            return GetSubPosItem<ModelSubPos>(modelWhere);
        }

        /// <summary>
        /// 获取子工位某项
        /// </summary>
        /// <param name="strSampleID"></param>
        /// <returns></returns>
        public ModelTSubPos GetSubPosItem<ModelTSubPos>(ModelTSubPos modelWhere)
            where ModelTSubPos : new()
        {
            ModelTSubPos model = default(ModelTSubPos);
            DataTable dt = _DB.ExcuteQuery<ModelTSubPos>(modelWhere).Result.ToMyDataTable();
            if (dt.Rows.Count > 0)
                model = ColumnDef.ToEntity<ModelTSubPos>(dt.Rows[0]);
            return model;
        }

        /// <summary>
        /// 获取子工位列表
        /// </summary>
        /// <typeparam name="ModelTSubPos"></typeparam>
        /// <param name="modelWhere"></param>
        /// <returns></returns>
        public List<ModelTSubPos> GetSubPosList<ModelTSubPos>(ModelTSubPos modelWhere)
            where ModelTSubPos : new()
        {
            List<ModelTSubPos> LstModel = default(List<ModelTSubPos>);
            DataTable dt = _DB.ExcuteQuery<ModelTSubPos>(modelWhere).Result.ToMyDataTable();
            LstModel = ColumnDef.ToEntityList<ModelTSubPos>(dt);
            return LstModel;
        }

        /// <summary>
        /// 获取子工位可视列表
        /// </summary>
        /// <typeparam name="ModelTSubPos"></typeparam>
        /// <param name="modelWhere"></param>
        /// <returns></returns>
        public ObservableCollection<ModelTSubPos> GetSubPosObserver<ModelTSubPos>(ModelTSubPos modelWhere)
            where ModelTSubPos : new()
        {
            ObservableCollection<ModelTSubPos> LstModel = default(ObservableCollection<ModelTSubPos>);
            DataTable dt = _DB.ExcuteQuery<ModelTSubPos>(modelWhere).Result.ToMyDataTable();
            LstModel = ColumnDef.ToEntityObserver<ModelTSubPos>(dt);
            return LstModel;
        }

        ///// <summary>
        ///// 模式化管理子工位
        ///// </summary>
        ///// <param name="PosGroupName"></param>
        ///// <param name="LstModel"></param>
        ///// <returns></returns>
        //public bool UpdateSubPosItem<ModelTSubPos>(string PosGroupName, List<ModelTSubPos> LstModel = null)
        //    where ModelTSubPos : new()
        //{
        //    try
        //    {
        //        if (LstModel != null)
        //            _DB.ExcuteUpdate<ModelTSubPos>(LstModel);

        //        ModelSubPos model = new ModelSubPos()
        //        {
        //            SampleID = string.Format("GroupKey='{0}' order by OrderID asc", PosGroupName)
        //            .MarkExpress().MarkWhere()
        //        };
        //        ModelTSubPos modelT = new ModelTSubPos();
        //        model.MapperToModel<ModelSubPos, ModelTSubPos>(ref modelT);
        //        List<ModelTSubPos> LstSubStation = GetSubPosList<ModelTSubPos>(modelT);
        //        ModelSubPos ObjectEmptyPos = null;       //空工位: 可放置
        //        ModelSubPos ObjectWaitSamInPos = null;   //待入样: 可装样
        //        ModelSubPos ObjectSampledPos = null;     //已入样: 入样队列第一个
        //        ModelSubPos ObjectTimeOutPos = null;     //已到时: 首个超时样，待时间管理
        //        ModelSubPos ModelTemp = new ModelSubPos();
        //        //空工位 待入样 已到时 已入样 已禁用
        //        foreach (var item in LstSubStation)
        //        {
        //            item.MapperToModel<ModelTSubPos, ModelSubPos>(ref ModelTemp);
        //            string PosKey = ModelTemp.PosKey.ToMyString();
        //            if (ObjectEmptyPos == null && string.IsNullOrEmpty(ModelTemp.SampleID) && PosKey == "空工位")
        //                ModelTemp.MapperToModel<ModelSubPos, ModelSubPos>(ref ObjectEmptyPos);
        //            else if (ObjectWaitSamInPos == null && string.IsNullOrEmpty(ModelTemp.SampleID) && PosKey == "待入样")
        //                ModelTemp.MapperToModel<ModelSubPos, ModelSubPos>(ref ObjectWaitSamInPos);
        //            else if (ObjectSampledPos == null && !string.IsNullOrEmpty(ModelTemp.SampleID) && PosKey == "已入样")
        //                ModelTemp.MapperToModel<ModelSubPos, ModelSubPos>(ref ObjectSampledPos);
        //            else if (ObjectTimeOutPos == null && !string.IsNullOrEmpty(ModelTemp.SampleID) && PosKey == "已到时")
        //                ModelTemp.MapperToModel<ModelSubPos, ModelSubPos>(ref ObjectTimeOutPos);
        //        }
        //        WritePosItem(model.GroupKey, "ObjectEmptyPos", ObjectEmptyPos != null ? ObjectEmptyPos.PosID.ToMyString() : "");
        //        WritePosItem(model.GroupKey, "ObjectWaitSamInPos", ObjectWaitSamInPos != null ? ObjectWaitSamInPos.PosID.ToMyString() : "");
        //        WritePosItem(model.GroupKey, "ObjectSampledPos", ObjectSampledPos != null ? ObjectSampledPos.PosID.ToMyString() : "");
        //        WritePosItem(model.GroupKey, "ObjectTimeOutPos", ObjectTimeOutPos != null ? ObjectTimeOutPos.PosID.ToMyString() : "");
        //    }
        //    catch (Exception ex)
        //    {
        //        return false;
        //    }
        //    return true;
        //}

        /// <summary>
        /// 解除工位任务
        /// </summary>
        /// <param name="PosName"></param>
        /// <returns></returns>
        public CallResult RemovePosTask(string PosName)
        {
            List<ModelSystemSymbol> model = new List<ModelSystemSymbol>()
            {
                new ModelSystemSymbol(){ GroupName = PosName.MarkWhere(),
                CurrentValue = SystemDefault.StringEmpty,
                Name = " Name in('VIEW_SI','VIEW_SI_TEMP','TASK_CMD','STEP_KEY')".MarkExpress().MarkWhere() },

                new ModelSystemSymbol(){ GroupName = PosName.MarkWhere(),
                CurrentValue = TaskStateDef.NORMAL,
                Name = "TASK_STATE".MarkWhere() },

            };
            return _DB.ExcuteUpdate<ModelSystemSymbol>(model);
        }

        /// <summary>
        /// 解除关联工位任务
        /// </summary>
        /// <param name="PosName"></param>
        /// <returns></returns>
        public CallResult RemoveRelatedPosTask(string PosName)
        {
            string strViewSI = ReadPosItem(PosName, "VIEW_SI");
            List<string> LstRelatedPosName = new List<string>();
            if (!string.IsNullOrEmpty(strViewSI))
            {
                ModelSystemSymbol model = new ModelSystemSymbol() { Name = "VIEW_SI", CurrentValue = strViewSI };
                DataTable dt = _DB.ExcuteQuery(model).Result.ToMyDataTable();
                foreach (DataRow row in dt.Rows)
                    LstRelatedPosName.AppandList(row["GroupName"].ToMyString());
            }
            string strGroupNameWhere = LstRelatedPosName.ToMyString(",", true, "'", "'");
            if (!string.IsNullOrEmpty(strGroupNameWhere))
            {
                List<ModelSystemSymbol> LstModel = new List<ModelSystemSymbol>()
                {
                    new ModelSystemSymbol(){
                    GroupName = "GroupName in (" + strGroupNameWhere + ")".MarkExpress().MarkWhere(),
                    CurrentValue = SystemDefault.StringEmpty,
                    Name = " Name in('VIEW_SI','VIEW_SI_TEMP','TASK_CMD','STEP_KEY','PP_KEY')".MarkExpress().MarkWhere() },

                    new ModelSystemSymbol(){
                    GroupName = "GroupName in (" + strGroupNameWhere + ")".MarkExpress().MarkWhere(),
                    CurrentValue = TaskStateDef.NORMAL,
                    Name = "TASK_STATE".MarkWhere() },
                };
                return _DB.ExcuteUpdate<ModelSystemSymbol>(LstModel);
            }
            return new CallResult();
        }

        /// <summary>
        /// 解除关联的计划任务
        /// </summary>
        /// <param name="SampleLabel"></param>
        /// <returns></returns>
        public CallResult RemoveScheduleTask(string SampleLabel)
        {
            if (string.IsNullOrEmpty(SampleLabel))
                return new CallResult();
            string strSampleID = SampleLabel.MidString("", SystemDefault.LinkSign);
            string strSampleLabelKey = string.Empty;
            if (!string.IsNullOrEmpty(strSampleID))
                strSampleLabelKey = string.Format(" or msg_visual like '{0}%'", strSampleID + SystemDefault.LinkSign);
            string strSql = string.Format("update core_schedule_menu set state='' where msg_visual='{0}' {1};", strSampleID, strSampleLabelKey);
            return _DB.ExcuteSQL(strSql);
        }

        /// <summary>
        /// 解除任务
        /// </summary>
        /// <param name="SampleLabel"></param>
        /// <param name="PosName"></param>
        /// <returns></returns>
        public CallResult RemoveThisTaskChain(string SampleLabel, string PosName)
        {
            if (!string.IsNullOrEmpty(PosName))
                RemovePosTask(PosName);
            return RemoveScheduleTask(SampleLabel);
        }

        /// <summary>
        /// 解除关联任务
        /// </summary>
        /// <param name="SampleLabel"></param>
        /// <param name="PosName"></param>
        /// <returns></returns>
        public CallResult RemoveRelatedTaskChain(string SampleLabel, string PosName = "")
        {
            if (!string.IsNullOrEmpty(PosName))
                RemoveRelatedPosTask(PosName);
            return RemoveScheduleTask(SampleLabel);
        }
    }
}
