using Engine.Data;
using System;
using System.Net;
using System.Net.Sockets;

namespace Engine.ComDriver
{
    /// <summary>
    /// 网络通讯驱动
    /// </summary>
    public abstract class sComNetDevice : NotifyObject, IDisposable, 
        IComDriver<Socket, NetworkCommParam>
    {
        #region 属性
        /// <summary>
        /// 通讯参数
        /// </summary>
        public DriverItem<NetworkCommParam> DriverItem { get => _DriverItem; }
        protected DriverItem<NetworkCommParam> _DriverItem = new DriverItem<NetworkCommParam>();
        /// <summary>
        /// 通讯循环工作开关
        /// </summary>
        public bool ComWorking { get; protected set; }
        /// <summary>
        /// 与PLC通信的_mSocket客户端
        /// </summary>
        public Socket mClient { get; set; }
        /// <summary>
        /// 返回PLC是否可以建立连接
        /// </summary>
        public bool IsAvailable
        {
            get
            {
                using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
                {
                    return Connect(socket) == ErrorCode.NoError;
                }
            }
        }
        /// <summary>
        /// 是否处于连接状态
        /// </summary>
        public bool IsConnected
        {
            get
            {
                try
                {
                    if (mClient == null)
                        return false;
                    //return !((mClient.Poll(5000, SelectMode.SelectRead) && (mClient.Available == 0)) || !mClient.Connected);
                    return mClient.Connected;
                }
                catch
                {
                    return false;
                }
            }
        }
        /// <summary>
        /// 暂存错误内容
        /// </summary>
        public string LastErrorString { get; set; }
        /// <summary>
        /// 暂存错误代码
        /// </summary>
        public ErrorCode LastErrorCode { get; set; }
        #endregion

        #region 事件
        /// <summary>
        /// 消息通知事件
        /// </summary>
        public event Action<object, EventArgs<EventType>> ComNote_Rised;
        /// <summary>
        /// 错误捕捉事件
        /// </summary>
        public event Action<object, EventArgs<Error>> ComError;
        /// <summary>
        /// 网络状态变化
        /// </summary>
        public event Action<object, bool> StateChanged;
        #endregion

        #region 公共方法
        /// <summary>
        /// 配置驱动参数
        /// </summary>
        /// <param name="DrvItem"></param>
        /// <returns></returns>
        public virtual bool Config(DriverItem<NetworkCommParam> DrvItem)
        {
            if (DrvItem == null)
            {
                LastErrorCode = ErrorCode.WrongConfigParam;
                LastErrorString = "配置信息错误";
                return false;
            }
            _DriverItem = DrvItem;
            if (_DriverItem.ComParam.CycleTime <= 5)
                _DriverItem.ComParam.CycleTime = 100;
            return true;
        }

        #endregion

        #region 接口实现
        /// <summary>
        /// 调用函数 -- 连接PLC
        /// </summary>
        public virtual ErrorCode Open()
        {
            if (IsConnected)
                return ErrorCode.ConnectionIgnored;
            mClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            mClient.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 1000);
            mClient.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, 500);
            //mClient.ReceiveTimeout = 2000;//2000ms无数据接收则超时
            //mClient.SendTimeout = 500;
            return Connect(mClient);
        }
        /// <summary>
        /// 调用函数 -- 断开PLC连接
        /// </summary>
        public void Close()
        {
            try
            {
                if (mClient != null)
                {
                    //当设置通讯同步模式接收时超时Connected状态自动false,
                    //这时需要关闭连接，否则会造成多重连接问题
                    //if (mClient.Connected)
                    mClient.Shutdown(SocketShutdown.Both);
                    mClient.Disconnect(true);
                    mClient.Close();
                    string strMessage = string.Format("已从{0}:{1}断开连接", _DriverItem.ComParam.ComIP, _DriverItem.ComParam.ComPort);
                    EventRise_ComNote(EventType.ComClosed, strMessage);
                    EventRise_ComStateChanged(mClient.Connected);
                }
            }
            catch (Exception)
            {

            }
        }
        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            Close();
        }
        /// <summary>
        /// 清除错误信息
        /// </summary>
        public void ClearLastError()
        {
            LastErrorCode = ErrorCode.NoError;
            LastErrorString = string.Empty;
        }
        #endregion

        #region 内部方法
        /// <summary>
        /// 事件通知
        /// </summary>
        /// <param name="EvtType"></param>
        /// <param name="strMessage"></param>
        protected void EventRise_ComNote(EventType EvtType, string strMessage)
        {
            if (ComNote_Rised != null)
                ComNote_Rised(this, new EventArgs<EventType>() { Key = EvtType, Message = strMessage });
        }

        /// <summary>
        /// 错误通知
        /// </summary>
        /// <param name="err"></param>
        /// <param name="strMessage"></param>
        protected void EventRise_Error(Error err, string strMessage)
        {
            if (ComError != null)
                ComError(this, new EventArgs<Error>() { Key = err, Message = strMessage });
        }

        /// <summary>
        /// 事件通知
        /// </summary>
        /// <param name="EvtType"></param>
        /// <param name="strMessage"></param>
        protected void EventRise_ComStateChanged(bool IsConnected)
        {
            if (StateChanged != null)
                StateChanged(this, IsConnected);
        }
        /// <summary>
        /// 连接PLC
        /// </summary>
        /// <param name="socket">微软原始连接信息</param>
        /// <returns></returns>
        protected ErrorCode Connect(Socket socket)
        {
            try
            {
                IPEndPoint server = new IPEndPoint(IPAddress.Parse(_DriverItem.ComParam.ComIP), _DriverItem.ComParam.ComPort);
                socket.Connect(server);
                EventRise_ComNote(EventType.ComBuilded, string.Format("连接至{0}:{1}成功",
                    _DriverItem.ComParam.ComIP, _DriverItem.ComParam.ComPort));
                EventRise_ComStateChanged(socket.Connected);
                return ErrorCode.NoError;
            }
            catch (SocketException sex)
            {
                if (sex.SocketErrorCode == SocketError.AddressNotAvailable)
                {
                    LastErrorCode = ErrorCode.IPAddressNotAvailable;
                }
                else
                {
                    LastErrorCode = ErrorCode.ConnectionError;
                }

                LastErrorString = sex.Message;
            }
            catch (Exception ex)
            {
                LastErrorCode = ErrorCode.ConnectionError;
                LastErrorString = ex.Message;
            }
            EventRise_Error(Error.ConnectError, string.Format("连接至{0}:{1}失败,原因:【{2}】",
                DriverItem.ComParam.ComIP, DriverItem.ComParam.ComPort, LastErrorString));
            EventRise_ComStateChanged(socket.Connected);
            return LastErrorCode;
        }
        #endregion
    }
}