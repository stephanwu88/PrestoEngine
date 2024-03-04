using Engine.Common;
using Engine.Data.DBFAC;
using Engine.Mod;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Engine.ComDriver.ARL
{
    /// <summary>
    /// ARL光谱仪数据传输协议
    /// </summary>
    public class sComSMSData: sComRuleAsc
    {
        private IDBFactory<ServerNode> _DB = DbFactory.CPU;

        public sComSMSData()
        {
            EDX = "";
        }

        public sComSMSData(DriverItem<NetworkCommParam> DrvItem) : base(DrvItem)
        {
            EDX = "";
        }

        /// <summary>
        /// 网络状态变化
        /// </summary>
        /// <param name="obj"></param>
        protected sealed override void _Com_StateChanged(sSocket sender, SocketState State)
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
        }

        /// <summary>
        /// 数据消费者
        /// </summary>
        /// <param name="state"></param>
        protected override void QueueRcvWorker(object state)
        {
            while (ComWorking)
            {
                try
                {
                    lock (_QueueRcv)
                    {
                        if (_QueueRcv.Count > 0)
                        {
                            string strMessage = _QueueRcv.Dequeue();
                            string strErrorMessage = ParseRecievedContent(strMessage);
                            if (!string.IsNullOrEmpty(strErrorMessage))
                            {
                                strErrorMessage = string.Format("ARL数据解析错误：{0}", strErrorMessage);
                                Logger.Error.Write(LOG_TYPE.ERROR, strErrorMessage);
                            }
                        }
                    }
                }
                catch (Exception )
                {

                }
                Thread.Sleep(DriverItem.ComParam.CycleTime);
            }
        }

        /// <summary>
        /// 解析接收报文
        /// </summary>
        /// <param name="strMessage"></param>
        /// <returns></returns>
        protected virtual string ParseRecievedContent(string strMessage)
        {
            string ErrorMessage = string.Empty;
            //L1238355665A1,SPHC-1R,,,,,FELASTSOL,,,,,
            //N,0.0025,C,0.6669,Si,0.0045,Mn,0.0787,P,0.0664,S,0.0410,As,0.0042,Al,0.8867,Al_sol,0.8517,V,0.0012,
            //Ti,0.0011,Ca,0.0025,Nb,0.0019,Ni,0.0086,Cr,0.0419,Cu,0.0356,B,0.0004,Mo,0.0007,Sb,0.0023,W,0.0049,
            //Sn,0.0049,Ceq,0.6917,Pb,0.0008,Co,0.0035,Fe%,98.1390,
            ModelOESData modelData = new ModelOESData();
            List<string> LstField = strMessage.MySplit(",");
            if (LstField.MyCount() < 3) return "解析数据长度错误";

            modelData.SampleID = LstField[0];
            modelData.SteelType = LstField[1];
            modelData.DicSrcField.AppandDict("RecTime", SystemDefault.StringTimeNow);
            modelData.DicSrcField.AppandDict("ZHUAN_FLAG", "0");
            modelData.DicSrcField.AppandDict("ZHI_FLAG", "0");
            try
            {
                for (int i = LstField.MyCount() - 1; i > 2; i = i - 2)
                {
                    if (LstField[i].IsNumeric() && !LstField[i - 1].IsNumeric())
                    {
                        string strElemName = string.Format("ELEM_{0}", LstField[i - 1].ToUpper().MyReplace("%", ""));
                        string strElemVal = LstField[i];
                        modelData.DicSrcField.AppandDict(strElemName, strElemVal);
                    }
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            //modelData.TableName = "data_result_sms";
            //modelData.DicSrcField.AppandDict("SampleCategory", "钢样");
            Dictionary<string, ModelSheetColumn> DicCode = FieldManager.Instance["TB_DataResult", DicNameBy.ColBind];
            if (DicCode != null && DbFactory.Data != null)
            {
                modelData.AppandSourceField();
                modelData.ConvertToSqlDict(DicCode);
                CallResult result = DbFactory.Data.ExcuteInsert(modelData);
                if (result.Fail)
                    ErrorMessage = result.Result.ToMyString();
            }
            return ErrorMessage;
        }
    }
}
