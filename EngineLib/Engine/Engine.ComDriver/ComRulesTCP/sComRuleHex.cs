using Engine.Common;
using Engine.Mod;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Engine.ComDriver
{
    public class sComRuleHex : IComRuleNetCom<byte[]>
    {
        #region 成员变量
        /// <summary>
        /// 连接端对象-服务器
        /// </summary>
        private sSocket _ComServer = new sSocket();
         /// <summary>
        /// 连接端对象-客户端
        /// </summary>
        private sSocket _Com = new sSocket();
        public sSocket ComObject { get => _Com; }
        /// <summary>
        /// 数据发送事件
        /// </summary>
        public event Action<IComRule<byte[], sSocket, NetworkCommParam>, byte[]> SendMsg;
        /// <summary>
        /// 数据接收事件
        /// </summary>
        public event Action<IComRule<byte[], sSocket, NetworkCommParam>, byte[]> RcvMsg;
        /// <summary>
        /// 发生错误事件
        /// </summary>
        public event Action<IComRule<byte[], sSocket, NetworkCommParam>, ComError> Error;
        /// <summary>
        /// 数据接收缓冲区
        /// </summary>
        protected List<byte> _BufferRcv = new List<byte>();

        protected byte[] _RcvData_Last = new byte[32];
        /// <summary>
        /// 数据接收的生产队列
        /// </summary>
        protected Queue<byte[]> _QueueRcv;
        /// <summary>
        /// 缓冲缓冲区大小
        /// </summary>
        protected int Max_Buffer_Size = 2048;
        /// <summary>
        /// 自动解析接收队列模式
        /// </summary>
        private bool QueueAutoParsedMode = false;
        #endregion

        #region 属性
        /// <summary>
        /// 通讯连接名称
        /// </summary>
        public string ComName { get; set; } = string.Empty;
        /// <summary>
        /// 本设备名称
        /// </summary>
        public string LocalEquipName { get; set; } = string.Empty;
        /// <summary>
        /// 连接目标名称
        /// </summary>
        public string RemoteBossName { get; set; } = string.Empty;
        /// <summary>
        /// 访问本地客户名称
        /// </summary>
        public string PartnerVisitorName { get; set; } = string.Empty;
        /// <summary>
        /// 通讯驱动信息
        /// </summary>
        public DriverItem<NetworkCommParam> DriverItem { get; protected set; }
                = new DriverItem<NetworkCommParam>();
        /// <summary>
        /// 通讯循环工作开关
        /// </summary>
        public bool ComWorking { get; set; }
        /// <summary>
        /// 通讯报文头
        /// </summary>
        public byte[] SnHead { get; set; }
        /// <summary>
        /// 报文结束符
        /// </summary>
        public byte byEnd { get; set; }
        /// <summary>
        /// 网络连接状态
        /// </summary>
        public SocketState ComState => ComObject.State;

        /// <summary>
        /// 工作模式
        /// </summary>
        public SocketMode ComWorkMode { get; set; }

        /// <summary>
        /// 通讯IP
        /// </summary>
        public string ComIP { get; private set; }
        /// <summary>
        /// 通讯端口
        /// </summary>
        public int ComPort { get; private set; }
        #endregion

        #region 成员函数
        /// <summary>
        /// 构造函数
        /// </summary>
        public sComRuleHex()
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="DrvItem"></param>
        public sComRuleHex(DriverItem<NetworkCommParam> DrvItem)
        {
            Config(DrvItem);
        }

        /// <summary>
        /// 通讯配置
        /// </summary>
        /// <param name="DrvItem"></param>
        /// <returns></returns>
        public bool Config(DriverItem<NetworkCommParam> DrvItem)
        {
            if (ComWorking)
                return false;
            if (string.IsNullOrEmpty(DrvItem?.ComParam?.ComIP))
            {
                if (!string.IsNullOrEmpty(ComIP))
                    DrvItem.ComParam.ComIP = ComIP;
                if (ComPort > 0)
                    DrvItem.ComParam.ComPort = ComPort;
            }
            else
            {
                DriverItem = DrvItem;
                ComIP = DrvItem.ComParam.ComIP;
                ComPort = DrvItem.ComParam.ComPort;
                LocalEquipName = DrvItem.DriverName;
                QueueAutoParsedMode = true;
            }
            if (DriverItem.ComParam.CycleTime <= 5)
                DriverItem.ComParam.CycleTime = 100;
            switch (DrvItem.ComParam.WorkMode)
            {
                case "Client":
                    Config(ComIP, ComPort, SocketMode.Client);
                    break;
                case "Server":
                    Config(ComIP, ComPort, SocketMode.Listen);
                    break;
            }
            return true;
        }

        /// <summary>
        /// 通讯配置
        /// </summary>
        /// <param name="strComIP">通讯IP ex:127.0.0.1</param>
        /// <param name="iComPort">通讯端口 ex:2000</param>
        /// <param name="ConMode">工作模式</param>
        /// <returns></returns>
        public bool Config(string strComIP, int iComPort, SocketMode ConMode)
        {
            if (ComWorking)
                return false;
            ComIP = strComIP;
            ComPort = iComPort;
            ComWorkMode = ConMode;
            if (ComWorkMode == SocketMode.Listen)
            {
                _ComServer = new sSocket();
                _ComServer.Config(ComPort);
                _Com = _ComServer.AddClient("");
                //_Com.Config("", 0, SocketMode.ListenClient);
                _Com.ComDataReceived += _Com_DataRecvived;
                _Com.StateChanged += _Com_StateChanged;
            }
            else
            {
                _Com = new sSocket();
                _Com.Config(strComIP, ComPort);
                _Com.ComDataReceived += _Com_DataRecvived;
                _Com.StateChanged += _Com_StateChanged;
            }
            _QueueRcv = new Queue<byte[]>();
            //ThreadPool.QueueUserWorkItem(QueueWorker);
            return true;
        }

        /// <summary>
        /// 网络状态变化
        /// </summary>
        /// <param name="obj"></param>
        protected virtual void _Com_StateChanged(sSocket sender, SocketState State)
        {

        }

        /// <summary>
        /// 建立连接
        /// </summary>
        public bool Open()
        {
            bool OpenSuccess = false;
            if (ComWorking)
                return OpenSuccess;
            Config(DriverItem);
            if (ComWorkMode == SocketMode.Listen)
                OpenSuccess = _ComServer.Open();
            else
                OpenSuccess = ComObject.Open();
            _QueueRcv = new Queue<byte[]>();
            ComWorking = true;
            if(QueueAutoParsedMode)
                ThreadPool.QueueUserWorkItem(QueueRcvWorker, DriverItem);
            if (DriverItem.ServerCmdEn.IsEquals("1"))
                ThreadPool.QueueUserWorkItem(QueueSendWorker, DriverItem);
            return OpenSuccess;
        }
        /// <summary>
        /// 断开连接
        /// </summary>
        public void Close()
        {
            ComWorking = false;
            _ComServer.Close();
            ComObject.Close();
        }

        /// <summary>
        /// 发送报文
        /// </summary>
        /// <param name="bySend"></param>
        /// <param name="LogSend">是否记录发送日志</param>
        /// <returns></returns>
        public bool Send(byte[] bySend, bool LogSend = true)
        {
            bool SendSuccess = ComObject.Write(bySend);
            if (SendSuccess)
                RiseSendMsgEvent(bySend, LogSend);
            return SendSuccess;
        }

        /// <summary>
        /// 数据接收生产者
        /// </summary>
        /// <param name="sender">通讯对象</param>
        /// <param name="byData">接收数据</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        protected virtual void _Com_DataRecvived(sSocket sender, byte[] byData)
        {
            try
            {
                if (_QueueRcv != null)
                {
                    lock (_QueueRcv)
                    {
                        if (_QueueRcv.Count > Max_Buffer_Size)
                        {
                            _QueueRcv.Clear();
                            RiseErrorEvent(ComError.RcvBuffer_OverFlow);
                        }
                        else
                        {
                            RiseRcvMsgEvent(byData);
                            if (!_QueueRcv.Contains(byData))
                                _QueueRcv.Enqueue(byData);
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
        protected virtual void QueueRcvWorker(object state)
        {
            while (ComWorking)
            {
                try
                {
                    lock (_QueueRcv)
                    {
                        if (_QueueRcv.Count > 0)
                        {
                            byte[] byGetData = _QueueRcv.Dequeue();
                        }
                    }
                }
                catch (Exception ex)
                {
                    string ErrMsg = string.Format("【QueueWorker】【{0}】【{1}:{2}】\r\n\t{3}", 
                        DriverItem.DriverName, ComIP, ComPort, ex.Message);
                    Logger.Error.Write(LOG_TYPE.ERROR, ErrMsg);
                }
                Thread.Sleep(100);
            }
        }

        /// <summary>
        /// 数据发送队列
        /// </summary>
        /// <param name="state"></param>
        protected virtual void QueueSendWorker(object state)
        {
           
        }

        /// <summary>
        /// 触发通讯错误事件
        /// </summary>
        protected void RiseErrorEvent(ComError comError)
        {
            if (Error != null)
                Error(this, comError);
            string strErrorMessage = string.Format("【{0}】【{1}:{2}】{3}\r\n",
                    DriverItem.DriverName, ComIP, ComPort, comError.ToString());
            Logger.CommBody.Write(LOG_TYPE.ERROR, strErrorMessage);
        }

        /// <summary>
        /// 触发数据发送事件
        /// </summary>
        /// <param name="bySend"></param>
        /// <param name="LogSend"></param>
        protected void RiseSendMsgEvent(byte[] bySend, bool LogSend = true)
        {
            if (SendMsg != null)
                SendMsg(this, bySend);
            if (LogSend)
                AddCommLog(bySend, "SND");
        }

        /// <summary>
        /// 触发数据接收事件
        /// </summary>
        protected void RiseRcvMsgEvent(byte[] byRcv)
        {
            if (RcvMsg != null)
                RcvMsg(this, byRcv);
            AddCommLog(byRcv,"RCV");
        }

        /// <summary>
        /// 添加日志记录
        /// </summary>
        /// <param name="byData">byte[]</param>
        /// <param name="DataType">SND or RCV</param>
        protected virtual void AddCommLog(byte[] byData, string DataType)
        {
            string HexString = byData.ByteArrayToHexString();
            string strContent = string.Format("【{0}】 【{1}】  {2}", DataType, DriverItem.DriverName.ToMyString(), HexString);
            Logger.CommBody.Write(LOG_TYPE.MESS, strContent);
        }
        #endregion
    }
}
