using Engine.ComDriver.Types;
using Engine.Common;
using Engine.Core;
using Engine.Data.DBFAC;
using Engine.Mod;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Engine.ComDriver.Siemens
{
    /// <summary>
    /// 西门子S7系列PLC直读
    /// </summary>
    public class sComS7PLC : sComS7PLC<ServerNode>
    {
        /// <summary>
        /// 拷贝构造函数
        /// </summary>
        /// <param name="DrvItem"></param>
        public sComS7PLC(DriverItem<NetworkCommParam> DrvItem) : base(DrvItem)
        {

        }
    }

    /// <summary>
    /// 西门子S7系列PLC直读
    /// </summary>
    public class sComS7PLC<TConNode> : ComS7 where TConNode : new()
    {
        #region 内部变量
        /// <summary>
        /// 数据库连接
        /// </summary>
        //private IDBFactory<ServerNode> _DB = DbFactory.CPU.CloneInstance("Com");
        private IDBFactory<TConNode> _DB = DbFactory.Current.GetConn<TConNode>(DbConKey.Task);
        #endregion

        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="Drv"></param>
        public sComS7PLC(DriverItem<NetworkCommParam> Drv) : base(Drv)
        {
            
        }
        #endregion

        #region 公共方法
        /// <summary>
        /// 配置PLC
        /// </summary>
        /// <param name="Drv"></param>
        /// <returns></returns>
        public sealed override bool Config(DriverItem<NetworkCommParam> Drv)
        {
            base.Config(Drv);
            StateChanged += Com_StateChanged;
            LoadCommVaribleList();
            return true;
        }

        /// <summary>
        /// 根据数据名称通讯写变量
        /// </summary>
        /// <param name="DataName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool WriteByDataName(string DataName, object value)
        {
            ModelComPLC model = _CommList.MySelectFirst(x => x.DataName == DataName);
            if (model?.DataAddr == null)
                return false;
            ErrorCode err = Write(model.DataAddr, value);
            return err == ErrorCode.NoError;
        }

        /// <summary>
        /// 网络状态变化
        /// </summary>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        private void Com_StateChanged(object arg1, bool arg2)
        {
            if (_DB == null)
                return;
            ModelComLink comLink = new ModelComLink()
            {
                AppName = SystemDefault.AppName.MarkWhere(),
                DriverName = _DriverItem.DriverName.MarkWhere(),
                ComLink = "PLC-TCP".MarkWhere(),
                LinkState = mClient.Connected ? "1" : "0"
            };
            _DB.ExcuteUpdate<ModelComLink>(comLink);
        }

        /// <summary>
        /// 自动分段所有通讯变量
        /// </summary>
        protected sealed override void AutoSetDataComRange()
        {
            base.AutoSetDataComRange();
            string strSql = string.Empty;
            List<ModelComPLC> ListUpdate = new List<ModelComPLC>();
            foreach (ModelComPLC model in _CommList)
            {
                ListUpdate.Add(new ModelComPLC()
                {
                    ID = model.ID.MarkWhere(),
                    StartByteOfRange = model.StartByteOfRange
                });
            }
            if (ListUpdate.Count > 0 && _DB != null)
                _DB.ExcuteUpdate<ModelComPLC>(ListUpdate);
        }

        /// <summary>
        /// 通讯接收到数据 - 更新至数据库
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        protected sealed override void ComDataReceived()
        {
            if (_CommList == null || _DB == null || !_DriverItem.ServerUpdEn.IsEquals("1"))
                return;
            if (_CommList.Count == 0)
                return;
            Dictionary<string, ModelComPLC> Dic = _CommList.ToDictionary(x => x.ID);
            List<ModelComPLC> LstModelUpdate = new List<ModelComPLC>();
            List<string> LstRelatedExpress = new List<string>();
            foreach (ModelComPLC com in Dic.Values)
            {
                LstModelUpdate.Add(new ModelComPLC()
                {
                    DataValue = string.IsNullOrEmpty(com.DataValue) ? SystemDefault.StringEmpty : com.DataValue,
                    DriverToken = com.DriverToken.MarkWhere(),
                    ID = com.ID.MarkWhere()
                });
                //解析关联变量
                string strRelatedVariable = com.RelatedVariable.ToMyString();
                string strAddrType = com.AddrType.ToMyString().ToUpper();
                string strDataValue = com.DataValue;
                if (!string.IsNullOrEmpty(strRelatedVariable))
                {
                    if (strRelatedVariable.Contains("WriteTableRow("))
                    {
                        if (strRelatedVariable.Contains("{0}"))
                        {
                            strRelatedVariable = string.Format(strRelatedVariable, strDataValue);
                            LstRelatedExpress.Add(strRelatedVariable);
                        }
                        else if ((strDataValue == "1" || strDataValue.ToUpper() == "TRUE") && strAddrType == "BOOL")
                        {
                            LstRelatedExpress.Add(strRelatedVariable);
                        }
                    }
                    else
                    {
                        if (!strRelatedVariable.Contains("="))
                        {
                            strRelatedVariable = string.Format(strRelatedVariable + "={0}", strDataValue);
                            LstRelatedExpress.Add(strRelatedVariable);
                        }
                        else if ((strDataValue == "1" || strDataValue.ToUpper() == "TRUE") && strAddrType == "BOOL")
                        {
                            LstRelatedExpress.Add(strRelatedVariable);
                        }
                    }
                }
            }
            if (LstModelUpdate.Count > 0)
            {
                if (LstRelatedExpress.Count > 0)
                    _DB.ExcuteMacroCommand(LstRelatedExpress);
                _DB.ExcuteUpdate<ModelComPLC>(LstModelUpdate);
            }
        }

        /// <summary>
        /// 数据发送队列
        /// </summary>
        /// <param name="state"></param>
        protected override void QueueSendWorker(object state)
        {
            while (ComWorking)
            {
                try
                {
                    if (IsConnected && _DB != null)
                    {
                        //lock (Writelocker)
                        {
                            ModelComPLC model = new ModelComPLC()
                            {
                                AwaitState = string.Format(" DriverToken = '{0}' and AwaitState in ('Sending','TaskSending') " +
                                    "order by AwaitTime asc", _DriverItem.DriverToken).MarkExpress().MarkWhere()
                            };

                            DataTable dt = _DB.ExcuteQuery<ModelComPLC>(model).Result.ToMyDataTable();
                            List<ModelComPLC> ListVarNode = ColumnDef.ToEntityList<ModelComPLC>(dt);
                            List<ModelComPLC> ListUpdate = new List<ModelComPLC>();
                            foreach (ModelComPLC node in ListVarNode)
                            {
                                string strVarID = node.ID.ToString();
                                string strVar = node.DataAddr.ToMyString();
                                string strValType = node.AddrType.ToMyString();
                                string strVal = node.DataWrite.ToMyString();
                                object ObjVal = 0;
                                string strDataType = node.AddrType.ToMyString().ToUpper();
                                if (strDataType == "REAL")
                                    ObjVal = strVal.ToMyDouble().ToInt(SystemDefault.SiemensByteOrder32);
                                else if (strDataType == "BOOL")
                                    ObjVal = (strVal == "True" || strVal == "1") ? 1 : 0;
                                else if (strDataType.Contains("STRING"))
                                    ObjVal = strVal;
                                else
                                    ObjVal = strVal.ToMyInt();
                                if (base.Write(strVar, ObjVal) == ErrorCode.NoError)
                                {
                                    string strDatalabel = string.Empty;
                                    if (!string.IsNullOrEmpty(node.Comment))
                                        strDatalabel = node.Comment;
                                    else if (!string.IsNullOrEmpty(node.DataName))
                                        strDatalabel = node.DataName;
                                    else
                                        strDatalabel = node.DataAddr.ToMyString();
                                    string Message = string.Format("【SND】【{0}】 写入数据【{1}={2}】",
                                                DriverItem.DriverName, strDatalabel, strVal);
                                    Logger.CommBody.Write(LOG_TYPE.MESS, Message);
                                }
                                model = new ModelComPLC()
                                {
                                    ID = node.ID.MarkWhere(),
                                    AwaitState = "Sended",
                                };
                                ListUpdate.Add(model);
                            }
                            if (ListUpdate.Count > 0)
                                _DB.ExcuteUpdate<ModelComPLC>(ListUpdate);
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogError(string.Format("[{0}] Error:{1}", _DriverItem.ComParam.ComIP, ex.Message));
                }
                Thread.Sleep(DriverItem.ComParam.CycleTime);
            }
        }

        #endregion

        #region 内部方法
        /// <summary>
        /// 设置通讯变量列表
        /// </summary>
        private void LoadCommVaribleList()
        {
            if (_DB == null)
                return;
            string strColumnName = ColumnAttribute.Column<ModelComPLC>("DriverToken").Name.ToMyString();
            ModelComPLC model = new ModelComPLC()
            {
                DriverToken = string.Format("{0}='{1}' order by ID asc", strColumnName, _DriverItem.DriverToken)
                .MarkExpress().MarkWhere()
            };
            DataTable dt = _DB.ExcuteQuery<ModelComPLC>(model).Result.ToMyDataTable();
            ObservableCollection<ModelComPLC> ListVarNode = ColumnDef.ToEntityObserver<ModelComPLC>(dt);
            if (ListVarNode != null)
            {
                _CommList = ListVarNode;
            }
        }
        #endregion
    }
}
