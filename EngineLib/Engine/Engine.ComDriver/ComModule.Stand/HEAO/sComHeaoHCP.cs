using Engine.Common;
using Engine.Core;
using Engine.Data.DBFAC;
using Engine.Mod;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Engine.ComDriver.HEAO
{
    /// <summary>
    /// Heao Host Computer Protocol
    /// </summary>
    public sealed class sComHeaoHCP : sComRuleHex
    {
        /// <summary>
        /// 协议变量列表
        /// </summary>
        private List<ModelComHeao> ListComHeao;
        /// <summary>
        /// 正在发送通讯队列指令列表
        /// </summary>
        private List<ModelComHeao> CommandQueue = new List<ModelComHeao>();
        private IDBFactory<ServerNode> _DB = DbFactory.CPU?.CloneInstance("Com");

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="DrvItem"></param>
        public sComHeaoHCP(DriverItem<NetworkCommParam> DrvItem) : base(DrvItem)
        {
            byte[] byArrSnHead = DrvItem.ComHeader.ToMyString().StringToHexByte();
            if (byArrSnHead.Length != 2)
                throw new Exception(string.Format("控制器【{0}】构造失败", DrvItem.DriverName));
            SnHead = byArrSnHead;
            byEnd = 0xA6;

            //SystemTask.Task.Factory.StartNew(
            //()=> 
            if (_DB != null)
                LoadProtocolTable();
            //);
        }

        /// <summary>
        /// 加载协议列表
        /// </summary>
        /// <param name="ListComData"></param>
        public void LoadProtocolTable(List<ModelComHeao> ListComData)
        {
            if (ListComData != null && ListComHeao == null)
                return;
            ListComHeao = ListComData;
        }

        /// <summary>
        /// 加载数据库协议表
        /// </summary>
        private void LoadProtocolTable()
        {
            if (ListComHeao != null)
            {
                ListComHeao = new List<ModelComHeao>();
                return;
            }
            ModelComHeao modelComHeao = new ModelComHeao()
            {
                DriverToken = DriverItem.DriverToken,
                ProtocolFC = "ProtocolFC in ('Status(03H)','Alarm(04H)')".MarkExpress().MarkWhere()
            };
            DataTable dt = _DB.ExcuteQuery<ModelComHeao>(modelComHeao).Result.ToMyDataTable();
            ListComHeao = ColumnDef.ToEntityList<ModelComHeao>(dt);

            modelComHeao = new ModelComHeao() { DriverToken = DriverItem.DriverToken, AwaitState = "Sended" };
            dt = _DB.ExcuteQuery<ModelComHeao>(modelComHeao).Result.ToMyDataTable();
            foreach (DataRow row in dt.Rows)
            {
                ModelComHeao model = ColumnDef.ToEntity<ModelComHeao>(row);
                if (!string.IsNullOrEmpty(model.AwaitName))
                    CommandQueueAdd(model);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="modelCmd"></param>
        private void CommandQueueAdd(ModelComHeao modelCmd)
        {
            if (CommandQueue == null) CommandQueue = new List<ModelComHeao>();
            if (CommandQueue.Exists(x => x.Token == modelCmd.Token))
            {
                ModelComHeao cmd = CommandQueue.FirstOrDefault(x => x.Token == modelCmd.Token);
                cmd.DataValue = modelCmd.DataValue;
                cmd.AwaitName = modelCmd.AwaitName;
            }
            else
            {
                modelCmd.AwaitState = "Sended";
                lock (CommandQueue)
                    CommandQueue.Add(modelCmd);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="modelCmd"></param>
        private ModelComHeao CommandQueueRemove(byte byFC, string CmdValueString)
        {
            ModelComHeao modelDel = CommandQueue.FirstOrDefault(x => x.DataValue == CmdValueString &&
            x.ProtocolFC.MidString("(", "H").ToMyInt() == byFC);
            if (modelDel == null)
                return null;
            lock (CommandQueue)
                CommandQueue.Remove(modelDel);
            return modelDel;
        }

        /// <summary>
        /// 更新任务状态
        /// </summary>
        /// <param name="comModel"></param>
        /// <returns></returns>
        private bool UpdateTaskDriverState(ModelComHeao comModel)
        {
            if (comModel == null)
                return false;
            if (string.IsNullOrEmpty(comModel.AwaitName))
                return false;
            ModelSystemSymbol model = new ModelSystemSymbol()
            {
                DataType = "TaskDriver".MarkWhere(),
                Name = comModel.AwaitName.MarkWhere(),
                RelatedDriver = DriverItem.DriverName.MarkWhere(),
                CurrentValue = comModel.DataValue
            };
            return _DB.ExcuteUpdate<ModelSystemSymbol>(model).Success;
        }

        /// <summary>
        /// 网络状态变化
        /// </summary>
        /// <param name="obj"></param>
        protected override void _Com_StateChanged(sSocket sender, SocketState State)
        {
            if (_DB == null)
                return;
            ModelComLink comLink = new ModelComLink()
            {
                AppName = SystemDefault.AppName.MarkWhere(),
                DriverName = DriverItem.DriverName.MarkWhere(),
                ComLink = "TCP/IP".MarkWhere(),
                LinkState = State == SocketState.Connected ? "1" : "0"
            };
            _DB.ExcuteUpdate<ModelComLink>(comLink);
            Array.Clear(_RcvData_Last, 0, _RcvData_Last.Length);
        }

        /// <summary>
        /// 接收报文解析
        /// </summary>
        /// <param name="sender">通讯对象</param>
        /// <param name="byData">接收数据</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        protected override void _Com_DataRecvived(sSocket sender, byte[] byData)
        {
            try
            {
                bool isRcv = false;
                if (byData.Length == 32)
                {
                    for (int i = 0; i < 32; i++)
                    {
                        if (_RcvData_Last[i] != byData[i])
                        {
                            isRcv = true;
                            break;
                        }
                    }
                    if (isRcv)
                    {
                        for (int i = 0; i < 32; i++)
                        {
                            _RcvData_Last[i] = byData[i];
                        }
                    }
                    else
                    {
                        return;
                    }
                }
                else
                    return;

                _BufferRcv.AddRange(byData);

                int id_end = 0;
                int id_start = 0;
                int id_cursor = 0;
                if (_BufferRcv.Count > Max_Buffer_Size)
                {
                    _BufferRcv.Clear();
                    RiseErrorEvent(ComError.RcvBuffer_OverFlow);
                }
                else
                {
                    while (id_end >= 0 && id_end <= 31)
                    {
                        //定位结束符
                        id_end = _BufferRcv.FindIndex(id_cursor, x => x == byEnd);
                        //验证长度有效
                        if (id_end >= 31)
                        {
                            //验证报文头部匹配
                            if (_BufferRcv[id_end - 31] == SnHead[0] && _BufferRcv[id_end - 30] == SnHead[1])
                            {
                                id_start = id_end - 31;
                                byte[] RcvData = _BufferRcv.GetRange(id_start, 32).ToArray();
                                RiseRcvMsgEvent(RcvData);
                                lock (_QueueRcv)
                                {
                                    if (!_QueueRcv.Contains(RcvData))
                                        _QueueRcv.Enqueue(RcvData);
                                }
                                _BufferRcv.RemoveRange(0, ++id_end);
                                //报文解析完成后，发现剩余报文不足一条，则清除
                                if (_BufferRcv.Count > 0 && _BufferRcv.Count < 32)
                                {
                                    _BufferRcv.Clear();
                                    RiseErrorEvent(ComError.RcvBuffer_DataErr);
                                }
                                id_cursor = 0;
                            }
                            else
                            {
                                id_cursor = id_end + 1;
                            }
                        }
                        else
                        {
                            //移动游标到非真结束符后移位开始
                            id_cursor = id_end + 1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = string.Format("【Com_DataRecvived】【{0}】【{1}:{2}】\r\n\t{3}", 
                    DriverItem.DriverName, ComIP, ComPort, ex.Message);
                Logger.Error.Write(LOG_TYPE.ERROR, ErrMsg);
            }
        }

        /// <summary>
        /// 数据消费者
        /// </summary>
        /// <param name="state"></param>
        protected override void QueueRcvWorker(object state)
        {
            while (ComWorking)
            {
                #region 状态接收队列解析
                try
                {
                    lock (_QueueRcv)
                    {
                        if (_QueueRcv.Count > 0)
                        {
                            byte[] byGetData = _QueueRcv.Dequeue();
                            List<ModelComHeao> LstModelUpdate = new List<ModelComHeao>();
                            List<string> LstRelatedExpress = new List<string>();
                            if (byGetData.Length == 32 && _DB != null && ListComHeao != null)
                            {
                                byte[] Status_DU = new byte[25];
                                byte byLe = byGetData[2];
                                byte byFC = byGetData[3];
                                Array.Copy(byGetData, 4, Status_DU, 0, Status_DU.Length);
                                List<ModelComHeao> ListComHeaoByFC = ListComHeao.Where(x =>
                                    x.ProtocolFC.MidString("(", "H").ToMyInt() == byFC).ToList();
                                if (ListComHeaoByFC.Count == 0 && byFC != 0x03 && byFC != 0x04)
                                {
                                    string CmdValueString = Status_DU.ByteArrayToHexString();
                                    CmdValueString = CmdValueString.MidString(0, (byLe - 1) * 2);
                                    //byLe字节，最后一个字节位0x10 表示任务完成
                                    if (CmdValueString.EndsWith("10") && CmdValueString.Length > 2)
                                    {
                                        CmdValueString = CmdValueString.Substring(0, CmdValueString.Length - 2);
                                        ModelComHeao model = CommandQueueRemove(byFC, CmdValueString);
                                        if (UpdateTaskDriverState(model))
                                        {
                                            Logger.Task.Write(LOG_TYPE.MESS, string.Format("任务完成【{0}】【{1}:\t {2}】",
                                                 model.AwaitName, DriverItem.DriverName, model.DataValue));
                                            LstModelUpdate.Add(new ModelComHeao()
                                            {
                                                ID = model.ID.MarkWhere(),
                                                AwaitName = SystemDefault.StringEmpty,
                                                AwaitState = "Finish",
                                                AwaitTime = SystemDefault.StringTimeNow
                                            });
                                        }
                                    }
                                }
                                foreach (ModelComHeao com in ListComHeaoByFC)
                                {
                                    int iDataIndex = com.DataIndex.ToMyInt();
                                    int iDataBitOffset = com.DataBitOffset.ToMyInt();
                                    int iDataLen = com.DataLen.ToMyInt();
                                    string strAddrType = com.AddrType.ToMyString().ToUpper();
                                    string strRelatedVariable = com.RelatedVariable.ToMyString();
                                    string strSymbolID = com.SymbolID.ToMyString();
                                    if (iDataIndex < 1) iDataIndex = 1;
                                    if (iDataLen < 1) iDataLen = 1;
                                    int iCursor = (iDataIndex - 1) * 8 + iDataBitOffset;

                                    ModelComHeao modelUpdate = new ModelComHeao()
                                    {
                                        ID = com.ID.MarkWhere(),
                                        DriverToken = com.DriverToken.MarkWhere()
                                    };
                                    if (strAddrType == "BOOL")
                                    {
                                        BitArray Bits = new BitArray(Status_DU);
                                        string value = Bits.Get(iCursor) ? "1" : "0";
                                        modelUpdate.DataValue = value;
                                    }
                                    else if (strAddrType == "BYTE[ ]")
                                    {
                                        string value = string.Empty;
                                        if (iDataLen > 0 && (iDataIndex + iDataLen) <= 25 + 1)
                                        {
                                            for (int i = iDataIndex - 1; i < (iDataIndex - 1 + iDataLen); i++)
                                            {
                                                if (!string.IsNullOrEmpty(value))
                                                    value += ",";
                                                value += Status_DU[i].ToMyString();
                                            }
                                        }
                                        modelUpdate.DataValue = value;
                                    }
                                    string strDataValue = modelUpdate.DataValue.ToMyString();
                                    if (!string.IsNullOrEmpty(strDataValue))
                                    {
                                        LstModelUpdate.Add(modelUpdate);
                                        //解析关联变量
                                        if (!string.IsNullOrEmpty(strRelatedVariable))
                                        {
                                            List<string> LstRelatedVariable = strRelatedVariable.MySplit("|");
                                            foreach (string itmRelated in LstRelatedVariable)
                                            {
                                                if (itmRelated.Contains("WriteTableRow("))
                                                {
                                                    if (itmRelated.MatchParamCount() > 0)
                                                    {
                                                        string strCalcRelted = string.Format(itmRelated, strDataValue);
                                                        LstRelatedExpress.Add(strCalcRelted);
                                                    }
                                                    else if ((strDataValue == "1" || strDataValue.ToUpper() == "TRUE") && strAddrType == "BOOL")
                                                    {
                                                        LstRelatedExpress.Add(itmRelated);
                                                    }
                                                }
                                                else
                                                {
                                                    if (!itmRelated.Contains("="))
                                                    {
                                                        string strCalcRelted = string.Format(itmRelated + "={0}", strDataValue);
                                                        LstRelatedExpress.Add(strCalcRelted);
                                                    }
                                                    else if ((strDataValue == "1" || strDataValue.ToUpper() == "TRUE") && strAddrType == "BOOL")
                                                    {
                                                        LstRelatedExpress.Add(itmRelated);
                                                    }
                                                    else if(strAddrType == "BYTE[ ]")
                                                    {
                                                        string strParsedItemRelated = MacroCommand.Default.ParseExpStatement(itmRelated, strDataValue);
                                                        if(!string.IsNullOrEmpty(strParsedItemRelated)) LstRelatedExpress.Add(strParsedItemRelated);
                                                    } 
                                                }
                                            }
                                        }
                                    }
                                }
                                if (LstModelUpdate.Count > 0 && _DB != null)
                                {
                                    if (LstRelatedExpress.Count > 0)
                                        _DB.ExcuteMacroCommand(LstRelatedExpress);
                                    CallResult _result = _DB.ExcuteUpdate<ModelComHeao>(LstModelUpdate);
                                    if (_result.Fail)
                                        Array.Clear(_RcvData_Last, 0, _RcvData_Last.Length);
                                }
                                else if (LstModelUpdate.Count > 0 && _DB == null)
                                {
                                    //更新到内存表
                                    //foreach (ModelComHeao item in ListComHeao)
                                    //{

                                    //}
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    string ErrMsg = string.Format("【QueueWorker_RCV】【{0}】【{1}:{2}】\r\n\t{3}",
                        DriverItem.DriverName, ComIP, ComPort, ex.Message);
                    Logger.Error.Write(LOG_TYPE.ERROR, ErrMsg);
                }
                #endregion
                Thread.Sleep(DriverItem.ComParam.CycleTime);
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
                #region 发送队列解析
                try
                {
                    if (ComState == SocketState.Connected && _DB != null)
                    {
                        //lock (CommandQueue)
                        {
                            ModelComHeao modelQueryCmd = new ModelComHeao()
                            {
                                AwaitState = string.Format(" DriverToken = '{0}' and ProtocolFC in('Cmd(05H)','Path(07H)') and " +
                                    "AwaitState in ('Sending','TaskSending') order by AwaitTime asc", DriverItem.DriverToken)
                                    .MarkExpress().MarkWhere()
                            };
                            CallResult _result = _DB.ExcuteQuery<ModelComHeao>(modelQueryCmd);
                            if (_result.Success)
                            {
                                DataTable dt = _result.Result.ToMyDataTable();
                                foreach (DataRow row in dt.Rows)
                                {
                                    modelQueryCmd = ColumnDef.ToEntity<ModelComHeao>(row);
                                    string strID = modelQueryCmd.ID.ToMyString();
                                    string strLE = modelQueryCmd.DataLen.ToMyString();
                                    string strFC = modelQueryCmd.ProtocolFC.ToMyString().MidString("(", "H");
                                    modelQueryCmd.DataValue = modelQueryCmd.DataValue.ToMyString().Trim();
                                    string strDU = modelQueryCmd.DataValue;
                                    string strAwaitState = modelQueryCmd.AwaitState.ToMyString();
                                    string strCmdComment = modelQueryCmd.Comment.ToMyString();
                                    if (string.IsNullOrEmpty(strCmdComment))
                                        strCmdComment = strDU;
                                    string Message = string.Format("【Command】【{0}】{1}",
                                           modelQueryCmd.RelatedGroup.ToMyString(), strCmdComment);
                                    Logger.CommBody.Write(LOG_TYPE.MESS, Message);
                                    bool SendSuccess = Send(strLE, strFC, strDU, strAwaitState == "TaskSending");
                                    if (!SendSuccess)
                                    {
                                        string Error = string.Format("【Command发送错误】【{0}】{1}",
                                           modelQueryCmd.RelatedGroup.ToMyString(), strCmdComment);
                                        Logger.CommBody.Write(LOG_TYPE.ERROR, Error);
                                    }
                                    else
                                    {
                                        if (!string.IsNullOrEmpty(modelQueryCmd.AwaitName))
                                            CommandQueueAdd(modelQueryCmd);
                                    }
                                    modelQueryCmd = new ModelComHeao()
                                    {
                                        ID = strID.MarkWhere(),
                                        AwaitState = "Sended",
                                        AwaitTime = SystemDefault.StringTimeNow
                                    };
                                    _DB.ExcuteUpdate<ModelComHeao>(modelQueryCmd);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    string ErrMsg = string.Format("【QueueWorker_SND】【{0}】【{1}:{2}】\r\n\t{3}",
                     DriverItem.DriverName, ComIP, ComPort, ex.Message);
                    Logger.Error.Write(LOG_TYPE.ERROR, ErrMsg);
                }
                #endregion
                Thread.Sleep(100);
            }
        }

        /// <summary>
        /// 和澳命令发送
        /// </summary>
        /// <param name="LE"></param>
        /// <param name="FC"></param>
        /// <param name="DU"></param>
        /// <returns></returns>
        public bool Send(string LE, string FC, string DU, bool IsTaskCommand = false)
        {
            List<byte> bySend = new List<byte>();
            bySend.AddRange(new byte[2] { SnHead[0], SnHead[1] });
            string HexLe = LE.DecToHex();
            bySend.AddRange(HexLe.StringToHexByte());
            bySend.AddRange(FC.StringToHexByte());
            byte[] byDU = DU.StringToHexByte();
            byte[] bySendDU = new byte[25];
            if (byDU.Length > 25)
                Array.Copy(byDU, bySendDU, 25);
            else
                Array.Copy(byDU, bySendDU, byDU.Length);
            if (IsTaskCommand)
                bySendDU[24] = 01;
            bySend.AddRange(bySendDU);
            bySend.AddRange(new byte[29 - bySend.Count]);
            bySend.AddRange(new byte[2] { 0x00, 0x00 });
            bySend.AddRange(new byte[1] { byEnd });
            bool SendSuccess = Send(bySend.ToArray());
            return SendSuccess;
        }
    }
}
