using Engine.Common;
using Engine.Core;
using Engine.Data.DBFAC;
using Engine.Mod;
using System;
using System.Collections.Generic;
using System.Data;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Engine.ComDriver
{
    /// <summary>
    /// 
    /// </summary>
    public class ComMain : ComMain<ServerNode>
    {
        
    }

    /// <summary>
    /// 
    /// </summary>
    public class ComMain<TConNode> where TConNode : new()
    {
        #region 内部变量
        public static Dictionary<string, IComRuleNetCom<byte[]>> _ComRuleNetHex = new Dictionary<string, IComRuleNetCom<byte[]>>();
        public static Dictionary<string, IComRuleNetCom<string>> _ComRuleNetAsc = new Dictionary<string, IComRuleNetCom<string>>();
        public static Dictionary<string, IComRuleSerialCom<byte[]>> _ComRuleSerialHex = new Dictionary<string, IComRuleSerialCom<byte[]>>();
        public static Dictionary<string, IComRuleSerialCom<string>> _ComRuleSerialAsc = new Dictionary<string, IComRuleSerialCom<string>>();
        public static Dictionary<string, IComDriverNetCom> _ComNetPLC = new Dictionary<string, IComDriverNetCom>();
        public static Dictionary<string, IComDriverSerialCom> _ComSerialPLC = new Dictionary<string, IComDriverSerialCom>();
        public static event Action<object, ModelDriverItem> ComDriverAdded;
        private static IDBFactory<TConNode> _CommDB = null;
        public static List<ModelComLink> ComLinkList = new List<ModelComLink>();

        /// <summary>
        /// 通讯管理器工作中
        /// </summary>
        public bool ComListWorking { get; set; }

        /// <summary>
        /// 驱动成功加载
        /// </summary>
        public bool DriverLoadSuccess { get; set; }
        #endregion

        #region 当前工程通讯实例
        private static ComMain<TConNode> _Current;
        public static ComMain<TConNode> Current
        {
            get
            {
                if (_Current == null)
                {
                    _Current = new ComMain<TConNode>();
                    try
                    {
                        _Current.LoadDriver();
                    }
                    catch (Exception)
                    {

                    }
                }
                return _Current;
            }
        }
        #endregion

        #region 公共方法
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="ConNode"></param>
        public ComMain(TConNode ConNode = default(TConNode))
        {
            if (ConNode == null)
                _CommDB = DbFactory.Current.GetConn<TConNode>(DbConKey.Task);
        }

        /// <summary>
        /// 从数据库加载通讯设备列表
        /// </summary>
        /// <param name="driverItem"></param>
        /// <returns></returns>
        public bool LoadDriver(ModelDriverItem driverItem = null)
        {
            if (driverItem == null)
                driverItem = new ModelDriverItem() { Enabled = "1", Domain = SystemDefault.AppName };
            string strDriverNameList = Project.Current.StartUP["DriverNameList"].ToMyString();
            if (!string.IsNullOrEmpty(strDriverNameList))
                driverItem.Name = $"DriverName in ({strDriverNameList.MySplit("|").ToMyString(",", true, "'", "'")})".MarkExpress();
            bool LoadSuccess = true;
            CallResult _result = _CommDB.ExcuteQuery<ModelDriverItem>(driverItem);
            if (_result.Fail)
            {
                throw new Exception("获取控制器列表遇到错误：" + _result.Result.ToMyString());
            }
            _ComRuleNetHex.Clear();
            _ComRuleNetAsc.Clear();
            _ComRuleSerialHex.Clear();
            _ComRuleSerialAsc.Clear();
            _ComNetPLC.Clear();
            _ComSerialPLC.Clear();
            DataTable DrvTable = _result.Result.ToMyDataTable();
            foreach (DataRow row in DrvTable.Rows)
            {
                driverItem = ColumnDef.ToEntity<ModelDriverItem>(row);
                if (driverItem.ComLink == "TCP/IP")
                {
                    DriverItem<NetworkCommParam> DrvItem = ColumnDef.ToEntity<DriverItem<NetworkCommParam>>(row);
                    NetworkCommParam ComParam = ColumnDef.ToEntity<NetworkCommParam>(row);
                    DrvItem.ComParam = ComParam;
                    if (DrvItem.CharType.ToUpper() == "ASCII")
                    {
                        IComRuleNetCom<string> ComRule = DriverCommon.CreateInstance<string,TConNode>(DrvItem);
                        _ComRuleNetAsc.AppandDict(DrvItem.DriverName, ComRule);
                        if (ComRule == null) LoadSuccess = false;
                    }
                    else if(DrvItem.CharType.ToUpper() == "HEX")
                    {
                        IComRuleNetCom<byte[]> ComRule = DriverCommon.CreateInstance<byte[],TConNode>(DrvItem);
                        _ComRuleNetHex.AppandDict(DrvItem.DriverName, ComRule);
                        if (ComRule == null) LoadSuccess = false;
                    }
                }
                else if (driverItem.ComLink == "SerialCom")
                {
                    DriverItem<SerialCommParam> DrvItem = ColumnDef.ToEntity<DriverItem<SerialCommParam>>(row);
                    SerialCommParam ComParam = ColumnDef.ToEntity<SerialCommParam>(row);
                    DrvItem.ComParam = ComParam;
                    if (DrvItem.CharType.ToUpper() == "ASCII")
                    {
                        IComRuleSerialCom<string> ComRule = DriverCommon.CreateInstance<string,TConNode>(DrvItem);
                        _ComRuleSerialAsc.AppandDict(DrvItem.DriverName, ComRule);
                        if (ComRule == null) LoadSuccess = false;
                    }
                    else if (DrvItem.CharType.ToUpper() == "HEX")
                    {
                        IComRuleSerialCom<byte[]> ComRule = DriverCommon.CreateInstance<byte[],TConNode>(DrvItem);
                        _ComRuleSerialHex.AppandDict(DrvItem.DriverName, ComRule);
                        if (ComRule == null) LoadSuccess = false;
                    }
                }
                else if (driverItem.ComLink == "PLC-TCP")
                {
                    DriverItem<NetworkCommParam> DrvItem = ColumnDef.ToEntity<DriverItem<NetworkCommParam>>(row);
                    NetworkCommParam ComParam = ColumnDef.ToEntity<NetworkCommParam>(row);
                    DrvItem.ComParam = ComParam;
                    if (DrvItem.CharType.ToUpper() == "ASCII")
                    {
                        
                    }
                    else if (DrvItem.CharType.ToUpper() == "HEX")
                    {
                        IComDriverNetCom ComPLC = DriverCommon.CreateInstance<TConNode>(DrvItem);
                        _ComNetPLC.AppandDict(DrvItem.DriverName, ComPLC);
                        if (ComPLC == null) LoadSuccess = false;
                    }
                }
                else if (driverItem.ComLink == "PLC-COM")
                {
                    DriverItem<SerialCommParam> DrvItem = ColumnDef.ToEntity<DriverItem<SerialCommParam>>(row);
                    SerialCommParam ComParam = ColumnDef.ToEntity<SerialCommParam>(row);
                    DrvItem.ComParam = ComParam;
                    if (DrvItem.CharType.ToUpper() == "ASCII")
                    {
         
                    }
                    else if (DrvItem.CharType.ToUpper() == "HEX")
                    {
                        IComDriverSerialCom ComPLC = DriverCommon.CreateInstance<TConNode>(DrvItem);
                        _ComSerialPLC.AppandDict(DrvItem.DriverName, ComPLC);
                        if (ComPLC == null) LoadSuccess = false;
                    }
                }
                ComDriverAdded?.Invoke(this, driverItem);
            }
            DriverLoadSuccess = LoadSuccess;
            return LoadSuccess;
        }

        /// <summary>
        /// 加载控制器 - 网络通讯类
        /// </summary>
        /// <param name="driverItem"></param>
        /// <returns></returns>
        public bool LoadDriver(DriverItem<NetworkCommParam> driverItem)
        {
            bool LoadSuccess = false;
            if (driverItem.ComLink == "TCP/IP")
            {
                DriverItem<NetworkCommParam> NetDrvItem = driverItem;
                if (driverItem.CharType.ToUpper() == "ASCII")
                {
                    IComRuleNetCom<string> ComRule = DriverCommon.CreateInstance<string, TConNode>(driverItem);
                    _ComRuleNetAsc.AppandDict(driverItem.DriverName, ComRule);
                    if (ComRule == null) LoadSuccess = false;
                }
                else if (driverItem.CharType.ToUpper() == "HEX")
                {
                    IComRuleNetCom<byte[]> ComRule = DriverCommon.CreateInstance<byte[], TConNode>(driverItem);
                    _ComRuleNetHex.AppandDict(driverItem.DriverName, ComRule);
                    if (ComRule == null) LoadSuccess = false;
                }
            }
            else if (driverItem.ComLink == "PLC-TCP")
            {
                if (driverItem.CharType.ToUpper() == "ASCII")
                {

                }
                else if (driverItem.CharType.ToUpper() == "HEX")
                {
                    IComDriverNetCom ComPLC = DriverCommon.CreateInstance<TConNode>(driverItem);
                    _ComNetPLC.AppandDict(driverItem.DriverName, ComPLC);
                    if (ComPLC == null) LoadSuccess = false;
                }
            }
            return LoadSuccess;
        }

        /// <summary>
        /// 加载控制器 - 串口通讯类
        /// </summary>
        /// <param name="driverItem"></param>
        /// <returns></returns>
        public bool LoadDriver(DriverItem<SerialCommParam> driverItem)
        {
            bool LoadSuccess = false;
            if (driverItem.ComLink == "SerialCom")
            {
                if (driverItem.CharType.ToUpper() == "ASCII")
                {
                    IComRuleSerialCom<string> ComRule = DriverCommon.CreateInstance<string, TConNode>(driverItem);
                    _ComRuleSerialAsc.AppandDict(driverItem.DriverName, ComRule);
                    if (ComRule == null) LoadSuccess = false;
                }
                else if (driverItem.CharType.ToUpper() == "HEX")
                {
                    IComRuleSerialCom<byte[]> ComRule = DriverCommon.CreateInstance<byte[], TConNode>(driverItem);
                    _ComRuleSerialHex.AppandDict(driverItem.DriverName, ComRule);
                    if (ComRule == null) LoadSuccess = false;
                }
            }
            else if (driverItem.ComLink == "PLC-COM")
            {
                if (driverItem.CharType.ToUpper() == "ASCII")
                {

                }
                else if (driverItem.CharType.ToUpper() == "HEX")
                {
                    IComDriverSerialCom ComPLC = DriverCommon.CreateInstance<TConNode>(driverItem);
                    _ComSerialPLC.AppandDict(driverItem.DriverName, ComPLC);
                    if (ComPLC == null) LoadSuccess = false;
                }
            }
            return LoadSuccess;
        }

        /// <summary>
        /// 启动通讯工作
        /// </summary>
        public bool CommStart()
        {
            if (ComListWorking)
            {
                string strError = "通讯已启动，请勿重复启动！";
                sCommon.MyMsgBox(strError, MsgType.Warning);
                return false;
            }
            ComListWorking = true;
            Task.Factory.StartNew(() =>
            {
                foreach (IComRuleNetCom<byte[]> comRule in _ComRuleNetHex.Values)
                {
                    Task.Factory.StartNew(() =>
                    {
                        if (comRule.ComState == SocketState.Nothing)
                        {
                            //comRule.SendMsg += ComRule_SendMsg; 
                            comRule.Open();
                            //ThreadPool.QueueUserWorkItem(Queue_Send<NetworkCommParam>, comRule.DriverItem);
                        }
                    });
                }

                foreach (IComRuleNetCom<string> comRule in _ComRuleNetAsc.Values)
                {
                    Task.Factory.StartNew(() =>
                    {
                        if (comRule.ComState == SocketState.Nothing)
                        {
                            //comRule.SendMsg += ComRule_SendMsg;
                            comRule.Open();
                            //ThreadPool.QueueUserWorkItem(Queue_Send<NetworkCommParam>, comRule.DriverItem);
                        }
                    });
                }

                foreach (IComRuleSerialCom<byte[]> comRule in _ComRuleSerialHex.Values)
                {
                    Task.Factory.StartNew(() =>
                    {
                        if (comRule.ComState == SerialState.Nothing)
                        {
                            //comRule.SendMsg += ComRule_SendMsg; ;
                            comRule.Open();
                            //ThreadPool.QueueUserWorkItem(Queue_Send<SerialCommParam>, comRule.DriverItem);
                        }
                    });
                }

                foreach (IComRuleSerialCom<string> comRule in _ComRuleSerialAsc.Values)
                {
                    Task.Factory.StartNew(() =>
                    {
                        if (comRule.ComState == SerialState.Nothing)
                        {
                            //comRule.SendMsg += ComRule_SendMsg; 
                            comRule.Open();
                            //ThreadPool.QueueUserWorkItem(Queue_Send<SerialCommParam>, comRule.DriverItem);
                        }
                    });
                }

                foreach (IComDriverNetCom comRule in _ComNetPLC.Values)
                {
                    Task.Factory.StartNew(() =>
                    {
                        if (!comRule.IsConnected)
                        {
                            //comRule.SendMsg += ComRule_SendMsg; 
                            comRule.ComStart();
                            //ThreadPool.QueueUserWorkItem(Queue_Send<SerialCommParam>, comRule.DriverItem);
                        }
                    });
                }
                foreach (IComDriverSerialCom comRule in _ComSerialPLC.Values)
                {
                    Task.Factory.StartNew(() =>
                    {
                        if (!comRule.IsConnected)
                        {
                            //comRule.SendMsg += ComRule_SendMsg; 
                            comRule.ComStart();
                            //ThreadPool.QueueUserWorkItem(Queue_Send<SerialCommParam>, comRule.DriverItem);
                        }
                    });
                }
            });
            return ComListWorking;
        }

        /// <summary>
        /// 通讯列表状态侦听
        /// </summary>
        public void StartCommListListener()
        {
            if (ComLinkList == null)
                return;
            Task.Run(()=>
            {
                while (true)
                {
                    foreach (ModelComLink link in ComLinkList)
                    {
                        try
                        {
                            using (TcpClient client = new TcpClient())
                            {
                                client.BeginConnect(link.ComIP, link.ComPort, Connect_Callback, link);
                                Thread.Sleep(500);
                            }
                        }
                        catch (SocketException ex)
                        {
                            if (ex.ErrorCode == 10061)
                            {
                                SetPortStatus(link, "端口占用");
                            }
                            else
                            {
                                SetPortStatus(link, "端口服务未开启");
                            }
                        }
                    }
                }
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ar"></param>
        private void Connect_Callback(IAsyncResult ar)
        {
            ModelComLink link = (ModelComLink)ar.AsyncState;
            try
            {
                using (TcpClient client = (TcpClient)ar.AsyncState)
                {
                    client.EndConnect(ar);
                    SetPortStatus(link, "连接");
                }
            }
            catch
            {
                SetPortStatus(link, "端口服务未开启");
            }
        }

        /// <summary>
        /// 设置状态
        /// </summary>
        /// <param name="link"></param>
        /// <param name="status"></param>
        private void SetPortStatus(ModelComLink link, string status)
        {
            if (ComLinkList != null)
            {
                ComLinkList.MySelectFirst(x => x == link).ServerStatus = status;
            }
        }

        /// <summary>
        /// 通讯停止
        /// </summary>
        public void CommStop()
        {
            if (!ComListWorking)
                return;
            Logger.Default.Write( LOG_TYPE.MESS,"关闭所有设备通讯");
            ComListWorking = false;
            foreach (var sn in _ComRuleNetHex.Values)
                sn.Close();
            foreach (var sn in _ComRuleNetAsc.Values)
                sn.Close();
            foreach (var sn in _ComRuleSerialHex.Values)
                sn.Close();
            foreach (var sn in _ComRuleSerialAsc.Values)
                sn.Close();
            foreach (var sn in _ComRuleSerialAsc.Values)
                sn.Close();
            foreach (var sn in _ComNetPLC.Values)
                sn.ComStop();
            foreach (var sn in _ComSerialPLC.Values)
                sn.ComStop();
        }
        #endregion
    }
}

