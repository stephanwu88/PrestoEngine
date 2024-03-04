using Engine.Common;
using Engine.Mod;
using Engine.MVVM.Messaging;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace Engine.ComDriver
{
    public class sComRuleAsc : IComRuleNetCom<string>
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
        public event Action<IComRule<string, sSocket, NetworkCommParam>, string> SendMsg;
        /// <summary>
        /// 数据接收事件
        /// </summary>
        public event Action<IComRule<string, sSocket, NetworkCommParam>, string> RcvMsg;
        /// <summary>
        /// 发生错误事件
        /// </summary>
        public event Action<IComRule<string, sSocket, NetworkCommParam>, ComError> Error;
        /// <summary>
        /// 数据接收缓冲区
        /// </summary>
        protected string _BufferRcv = string.Empty;
        /// <summary>
        /// 数据接收的生产队列
        /// </summary>
        public Queue<string> _QueueRcv;
        /// <summary>
        /// 缓冲缓冲区大小
        /// </summary>
        protected int Max_Buffer_Size = 2048;
        /// <summary>
        /// 自动解析接收队列模式
        /// </summary>
        public bool QueueAutoParsedMode = false;
        /// <summary>
        /// 电文接收队列携带起始字符
        /// </summary>
        public bool QueueWithSTX = true;
        /// <summary>
        /// 电文接收队列携带结束字符
        /// </summary>
        public bool QueueWithEDX = false;
        /// <summary>
        /// 电文发送时自动带起始字符
        /// </summary>
        public bool SendWithSTX = true;
        /// <summary>
        /// 电文发送时自动带结束字符
        /// </summary>
        public bool SendWithEDX = true;
        #endregion

        #region 属性
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
        /// 报文起始符
        /// </summary>
        public string STX { get; set; } = string.Empty;
        /// <summary>
        /// 报文结束符
        /// </summary>
        public string EDX { get; set; } = string.Empty;
        /// <summary>
        /// 网络连接状态
        /// </summary>
        public SocketState ComState => _Com.State;
        /// <summary>
        /// 通讯连接名称
        /// </summary>
        public string ComName { get; set; } = string.Empty;
        /// <summary>
        /// 工作模式
        /// </summary>
        public SocketMode ComWorkMode { get; set; }
        /// <summary>
        /// IP地址
        /// </summary>
        public string ComIP { get; private set; }
        /// <summary>
        /// 网络端口
        /// </summary>
        public int ComPort { get; private set; }
        /// <summary>
        /// 通讯报文编码方式
        /// </summary>
        protected Encoding TextEncoding { get; set; } = Encoding.Default;
        #endregion

        #region 成员函数
        /// <summary>
        /// 默认构造函数
        /// </summary>
        public sComRuleAsc()
        {

        }

        /// <summary>
        /// 重载构造函数
        /// </summary>
        /// <param name="DrvItem"></param>
        public sComRuleAsc(DriverItem<NetworkCommParam> DrvItem)
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
            if (!string.IsNullOrEmpty(DrvItem.ComEDX))
                EDX = DrvItem.ComEDX;

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
            }
            else
            {
                _Com = new sSocket();
                _Com.Config(strComIP, ComPort);
            }
            if (QueueAutoParsedMode)
            {
                _Com.ComDataReceived += _Com_DataRecvived;
                _Com.StateChanged += _Com_StateChanged;
            }
            _QueueRcv = new Queue<string>();
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
                OpenSuccess = _Com.Open();
            _QueueRcv = new Queue<string>();
            ComWorking = true;
            if (QueueAutoParsedMode)
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
            _ComServer.Close();
            _Com.Close();
        }

        /// <summary>
        /// 发送报文
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="LogSend">是否记录发送日志</param>
        /// <returns></returns>
        public bool Send(string msg, bool LogSend = true)
        {
            //return Send(msg, ASCIIEncoding.Default);
            return Send(msg, TextEncoding, LogSend);
        }

        /// <summary>
        /// 发送报文
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="TxtEncoding"></param>
        /// <param name="LogSend">是否记录发送日志</param>
        /// <returns></returns>
        public bool Send(string msg, Encoding TxtEncoding, bool LogSend = true)
        {
            string strSTX = SendWithSTX ? STX : string.Empty;
            string strEDX = SendWithEDX ? EDX : string.Empty;
            string strSend = $"{strSTX}{msg}{strEDX}";
            bool SendSuccess = _Com.Write(strSend, TxtEncoding);
            if (SendSuccess)
                RiseSendMsgEvent(strSend, LogSend);
            return SendSuccess;
        }

        /// <summary>
        /// 数据接收生产者
        /// </summary>
        /// <param name="sender">数据对象源</param>
        /// <param name="byData">数据内容</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        protected virtual void _Com_DataRecvived(sSocket sender, byte[] byData)
        {
            try
            {
                //填入缓冲区
                string strRcv = TextEncoding.GetString(byData);
                //缓冲区溢出
                if (_BufferRcv.Length > Max_Buffer_Size)
                {
                    _BufferRcv = string.Empty;
                    RiseErrorEvent(ComError.RcvBuffer_OverFlow);
                }
                else
                {
                    if (string.IsNullOrEmpty(EDX))
                    {
                        lock (_QueueRcv)
                        {
                            if (!_QueueRcv.Contains(strRcv))
                                _QueueRcv.Enqueue(strRcv);
                        }
                        RiseRcvMsgEvent(strRcv);
                        return;
                    }
                    _BufferRcv += strRcv;
                    int endIndex = 0;
                    while (endIndex >= 0)
                    {
                        endIndex = _BufferRcv.IndexOf(EDX);
                        if (endIndex >= 0)
                        {
                            endIndex += EDX.Length;
                            int startIndex = 0;
                            if (!string.IsNullOrEmpty(STX))
                                startIndex = _BufferRcv.IndexOf(STX, 0, endIndex);
                            string item = _BufferRcv.Substring(startIndex, endIndex - startIndex);
                            _BufferRcv = _BufferRcv.Substring(endIndex);
                            if (!string.IsNullOrEmpty(item))
                            {
                                if (!QueueWithEDX)
                                    item = item.Substring(0, item.Length - EDX.Length);
                                RiseRcvMsgEvent(item);
                                lock (_QueueRcv)
                                {
                                    if (!_QueueRcv.Contains(item))
                                        _QueueRcv.Enqueue(item);
                                }
                            }
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
                            string strMessage = _QueueRcv.Dequeue();
                            Messenger.Default.Send<string>(strMessage, $"{DriverItem?.DriverPtl.ToMyString()}:{ComIP?.ToMyString()}");
                            string strMsgType = string.Empty;
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
        /// <param name="strMsg"></param>
        /// <param name="LogSend"></param>
        protected void RiseSendMsgEvent(string strMsg, bool LogSend = true)
        {
            if (SendMsg != null)
                SendMsg(this, strMsg);
            if (LogSend)
                AddCommLog(strMsg, "SND");
        }

        /// <summary>
        /// 触发数据接收事件
        /// </summary>
        protected void RiseRcvMsgEvent(string strMsg)
        {
            if (RcvMsg != null)
                RcvMsg(this, strMsg);
            AddCommLog(strMsg, "RCV");
        }

        /// <summary>
        /// 添加日志记录
        /// </summary>
        /// <param name="strData"></param>
        /// <param name="DataType">SND or RCV</param>
        protected virtual void AddCommLog(string strData, string DataType)
        {
            string strContent = string.Format("【{0}】 【{1}】  {2}", DataType, DriverItem.DriverName.ToMyString(), strData);
            Logger.CommBody.Write(LOG_TYPE.MESS, strContent);
        }
        #endregion
    }
}
