using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Engine.ComDriver
{
    /// <summary>
    /// 工作状态
    /// </summary>
    public enum SocketState
    {
        Nothing = 0,
        Listen = 1,
        Connecting = 2,
        Connected = 3,
        DisConnect = 4,
        Error = 5,
        Disable = 6
    }

    /// <summary>
    /// 工作模式
    /// </summary>
    public enum SocketMode
    {
        Listen = 0,
        Client = 1,
        ListenClient = 2
    }

    public class sSocket
    {
        #region 数据成员

        /// <summary>
        /// 初始连接状态
        /// </summary>
        private SocketState _ConnState = SocketState.Nothing;
        /// <summary>
        /// 初始工作模式
        /// </summary>
        public SocketMode _WorkMode = SocketMode.ListenClient;
        /// <summary>
        /// 连接端口号
        /// </summary>
        public int SocketPort;
        /// <summary>
        /// 连接IP
        /// </summary>
        public string SocketIP;

        /// <summary>
        /// 用于ListenClient模式时的Server连接对象
        /// </summary>
        private sSocket _ServerConn;
        /// <summary>
        /// 主连接
        /// </summary>
        private Socket _Conn;
        /// <summary>
        /// 用于Listen模式时客户端列表，若IP设置为空，则不验证客户端是否匹配，将接受所有客户端连接
        /// </summary>
        private List<sSocket> _ClientConns = new List<sSocket>();

        /// <summary>
        /// 接收数据的真实大小（字节）
        /// </summary>
        private int _RcvSize;
        /// <summary>
        /// 接收数据的最大缓冲区（字节）
        /// </summary>
        private const int _RcvBufferSize = 0x800;

        /// <summary>
        /// 最后一次接收的数据
        /// </summary>
        private byte[] _RcvData;

        /// <summary>
        /// 通讯变量数据列表，由用户自定义
        /// </summary>
        public Dictionary<string, string> TagList = new Dictionary<string, string>();

        public bool IsOpen = true;

        /// <summary>
        /// 定义数据接收事件
        /// </summary>
        public event Action<sSocket, byte[]> ComDataReceived;
        /// <summary>
        /// 定义状态变化事件
        /// </summary>
        public event Action<sSocket,SocketState> StateChanged;

        #endregion

        #region 内部过程
        /// <summary>
        /// 连接状态发生变化
        /// </summary>
        /// <param name="_newState">新的状态</param>
        public void ChangeState(SocketState _newState)
        {
            _ConnState = _newState;
            if (StateChanged != null) StateChanged(this, _newState);
        }

        /// <summary>
        /// 开始监听
        /// </summary>
        /// <param name="port"></param>
        /// <returns></returns>
        public bool Listen(int port)
        {
            try
            {
                SocketPort = port;

                if (_Conn != null)
                    _Conn.Close();
                _Conn = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _Conn.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Linger, true);
                IPEndPoint ipe = new IPEndPoint(IPAddress.Any, SocketPort);

                _Conn.Bind(ipe);
                _Conn.Listen(1);
                ChangeState(SocketState.Listen);

                ThreadPool.QueueUserWorkItem(AcceptConn, null);
                return true;
            }
            catch (Exception )
            {
                ChangeState(SocketState.Error);
                return false;
            }
        }

        /// <summary>
        /// 请求连接
        /// </summary>
        /// <param name="obj"></param>
        public void AcceptConn(Object obj)
        {
            while (_ConnState == SocketState.Listen && IsOpen)
            {
                try
                {
                    Socket clientSck = _Conn.Accept();
                    IPEndPoint ipe = (IPEndPoint)clientSck.RemoteEndPoint;
                    string ip = ipe.Address.ToString();

                    bool bAccept = false;
                    for (int i = 0; i < _ClientConns.Count; i++)
                    {
                        string ipTemp = _ClientConns[i].SocketIP;
                        if (ipTemp == ip || ipTemp == string.Empty || ipTemp == null)
                        {
                            if (_ClientConns[i].State != SocketState.Connected && _ClientConns[i].State != SocketState.Disable)
                            {
                                _ClientConns[i].ServerObject = this;
                                _ClientConns[i].msSocket = clientSck;
                                _ClientConns[i].Open();
                                bAccept = true;
                                break;
                            }
                        }
                        if (!bAccept)
                        {
                            clientSck.Shutdown(SocketShutdown.Both);
                            clientSck.Close();
                        }
                    }
                }
                catch (Exception ex) { }
            }
        }

        /// <summary>
        /// 开始连接
        /// </summary>
        /// <param name="ip">Server IP</param>
        /// <param name="port">Server Port</param>
        /// <returns></returns>
        public bool Connect(string ip, int port)
        {
            try
            {
                SocketIP = ip;
                SocketPort = port;
                ChangeState(SocketState.Connecting);
                _Conn = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _Conn.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Linger, true);
                ThreadPool.QueueUserWorkItem(RequestConn, null);
                return true;
            }
            catch (Exception )
            {
                ChangeState(SocketState.Error);
                return false;
            }
        }

        /// <summary>
        /// 请求连接
        /// </summary>
        /// <param name="state"></param>
        public void RequestConn(object state)
        {
            while (_ConnState == SocketState.Connecting && IsOpen)
            {
                try
                {
                    _Conn.Connect(SocketIP, SocketPort);
                    ChangeState(SocketState.Connected);
                    ThreadPool.QueueUserWorkItem(doReceive, _Conn);
                    break;
                }
                catch (Exception )
                {
                    Thread.Sleep(100);
                }
            }
        }

        /// <summary>
        /// 数据接收
        /// </summary>
        /// <param name="state"></param>
        public void doReceive(object state)
        {
            Socket conn = (Socket)state;
            while (_ConnState == SocketState.Connected && IsOpen)
            {
                try
                {
                    byte[] rcvBuffer = new byte[_RcvBufferSize];

                    _RcvSize = conn.Receive(rcvBuffer);
                    if (_RcvSize <= 0)
                    {
                        ChangeState(SocketState.DisConnect);

                        conn.Shutdown(SocketShutdown.Both);
                        conn.Close();

                        if (_WorkMode == SocketMode.Client)
                            Connect(SocketIP, SocketPort);
                        break;
                    }
                    else
                    {
                        if (ComDataReceived != null)
                        {
                            _RcvData = new byte[_RcvSize];
                            Buffer.BlockCopy(rcvBuffer, 0, _RcvData, 0, _RcvSize);
                            ComDataReceived(this, _RcvData);
                        }
                    }
                }
                catch (Exception ex)
                {
                    ChangeState(SocketState.DisConnect);

                    try
                    {
                        conn.Shutdown(SocketShutdown.Both);
                        conn.Close();
                    }
                    catch (Exception )
                    {

                    }

                    if (_WorkMode == SocketMode.Client)
                        Connect(SocketIP, SocketPort);
                    break;
                }
            }
        }

        /// <summary>
        /// 仅在Listen模式下使用，添加用于验证的客户端连接
        /// </summary>
        /// <param name="ip">验证IP，若设置为空则不作验证，将接受所有请求</param>
        /// <returns></returns>
        public sSocket AddClient(string ip)
        {
            foreach (sSocket s in _ClientConns)
            {
                if (s.SocketIP == ip)
                    return null;
            }
            sSocket newSocket = new sSocket();
            newSocket.Config(ip, SocketMode.ListenClient);
            _ClientConns.Add(newSocket);
            return newSocket;
        }

        public void AddClient(sSocket client)
        {
            if (_ClientConns.IndexOf(client) == -1)
            {
                client._WorkMode = SocketMode.ListenClient;
                _ClientConns.Add(client);
            }
        }
        #endregion

        #region 接口函数
        /// <summary>
        /// 配置，用于Listen模式
        /// </summary>
        /// <param name="port">监听端口号</param>
        public void Config(int port)
        {
            Config("", port, SocketMode.Listen);
        }
        /// <summary>
        /// 配置，用于Client模式
        /// </summary>
        /// <param name="ip">服务器IP</param>
        /// <param name="port">服务器端口号</param>
        public void Config(string ip, int port)
        {
            Config(ip, port, SocketMode.Client);
        }
        /// <summary>
        /// 配置，用于ListenClient模式
        /// </summary>
        /// <param name="ip">验证IP，若为空则不验证，将接受所有连接</param>
        /// <param name="mode">工作模式</param>
        public void Config(string ip, SocketMode mode)
        {
            Config(ip, 0, mode);
        }
        /// <summary>
        /// 配置函数
        /// </summary>
        /// <param name="ip">IP地址</param>
        /// <param name="port">端口号</param>
        /// <param name="mode">工作模式</param>
        public void Config(string ip, int port, SocketMode mode)
        {
            SocketIP = ip;
            SocketPort = port;
            _WorkMode = mode;
        }

        /// <summary>
        /// 开始工作
        /// </summary>
        /// <returns></returns>
        public bool Open()
        {
            try
            {
                IsOpen = true;
                switch (_WorkMode)
                {
                    case SocketMode.Listen:
                        if (_ConnState == SocketState.Listen)
                            return false;
                        Listen(SocketPort);
                        break;
                    case SocketMode.Client:
                        if (_ConnState == SocketState.Connected || _ConnState == SocketState.Connecting)
                            return false;
                        Connect(SocketIP, SocketPort);
                        break;
                    case SocketMode.ListenClient:
                        if (_ConnState == SocketState.Connected)
                            ThreadPool.QueueUserWorkItem(doReceive, _Conn);
                        else if (_ConnState == SocketState.Disable)
                            ChangeState(SocketState.DisConnect);
                        else
                            return false;
                        break;
                    default:
                        return false;
                }
                return true;
            }
            catch (Exception )
            {
                return false;
            }
        }

        /// <summary>
        /// 停止工作
        /// </summary>
        /// <returns></returns>
        public bool Close()
        {
            try
            {
                IsOpen = false;
                switch (_WorkMode)
                {
                    case SocketMode.Listen:
                        if (_ConnState == SocketState.Listen)
                        {
                            ChangeState(SocketState.Nothing);
                            _Conn.Close();
                        }
                        else
                            return false;
                        break;
                    case SocketMode.Client:
                        if (_ConnState == SocketState.Connected || _ConnState == SocketState.Connecting)
                        {
                            ChangeState(SocketState.DisConnect);
                            _Conn.Close();
                        }
                        else
                            return false;
                        break;
                    case SocketMode.ListenClient:
                        if (_ConnState == SocketState.Connected)
                        {
                            ChangeState(SocketState.Disable);
                            Thread.Sleep(30);
                            _Conn.Shutdown(SocketShutdown.Both);
                            _Conn.Close();
                        }
                        else
                            return false;
                        break;
                    default:
                        return false;
                }
                return true;
            }
            catch (Exception )
            {
                return false;
            }
        }

        /// <summary>
        /// 发送字符数据
        /// </summary>
        /// <param name="sendData"></param>
        public bool Write(string sendData)
        {
            byte[] bySend = ASCIIEncoding.Default.GetBytes(sendData);
            return Write(bySend, bySend.Length);
        }

        /// <summary>
        /// 发送字符数据
        /// </summary>
        /// <param name="sendData"></param>
        /// <param name="TextEncoding">ASCIIEncoding.Default or UnicodeEncoding.UTF8 or other</param>
        /// <returns></returns>
        public bool Write(string sendData, Encoding TextEncoding)
        {
            byte[] bySend = TextEncoding.GetBytes(sendData);
            return Write(bySend, bySend.Length);
        }

        /// <summary>
        /// 发送字节数据
        /// </summary>
        /// <param name="bySend"></param>
        public bool Write(byte[] bySend)
        {
            return Write(bySend, bySend.Length);
        }

        /// <summary>
        /// 发送字节数组并指定长度
        /// </summary>
        /// <param name="bySend">字节数组</param>
        /// <param name="size">数据长度</param>
        public bool Write(byte[] bySend, int size)
        {
            if (_ConnState != SocketState.Connected)
                return false;
            return _Conn.Send(bySend, size, SocketFlags.None) > 0;
        }


        #endregion

        #region 属性接口
        /// <summary>
        /// 微软原始连接属性
        /// </summary>
        public Socket msSocket
        {
            get => _Conn;
            set
            {
                _Conn = value;
                if (_Conn.Connected)
                    ChangeState(SocketState.Connected);
            }
        }
        /// <summary>
        /// 用于ListenClient模式，Server监听引用
        /// </summary>
        public sSocket ServerObject
        {
            get => _ServerConn;
            set
            {
                _ServerConn = value;
            }

        }

        /// <summary>
        /// 工作模式
        /// </summary>
        public SocketMode WorkMode
        {
            get => _WorkMode;
        }

        /// <summary>
        /// 连接状态属性
        /// </summary>
        public SocketState State
        {
            get => _ConnState;
        }
        /// <summary>
        /// 最后接受到的数据
        /// </summary>
        public byte[] ReadData
        {
            get => _RcvData;
        }

        /// <summary>
        /// 用于Listen模式时的客户端列表，若IP设置为空，则不验证客户端是否匹配，接受所有客户端连接
        /// </summary>
        public List<sSocket> ClientList
        {
            get => _ClientConns;
            set
            {
                _ClientConns = value;
            }
        }
        #endregion 
    }


}
