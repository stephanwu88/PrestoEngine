using Engine.Data;
using System;
using System.IO.Ports;

namespace Engine.ComDriver
{
    /// <summary>
    /// 串口通讯驱动
    /// </summary>
    public abstract class sComSerialDevice : NotifyObject, IDisposable, 
        IComDriver<SerialPort, SerialCommParam>
    {
        #region 属性
        /// <summary>
        /// 通讯参数
        /// </summary>
        public DriverItem<SerialCommParam> DriverItem { get; set; }
        /// <summary>
        /// 通讯循环工作开关
        /// </summary>
        public bool ComWorking { get; }
        /// <summary>
        /// 与PLC通信的客户端对象
        /// </summary>
        public SerialPort mClient { get; set; }
        /// <summary>
        /// 返回PLC是否可以建立连接
        /// </summary>
        public bool IsAvailable { get; }

        /// <summary>
        /// 是否处于连接状态
        /// </summary>
        public bool IsConnected { get; }

        /// <summary>
        /// Contains the last error registered when executing a function
        /// </summary>
        public string LastErrorString { get; set; }

        /// <summary>
        /// Contains the last error code registered when executing a function
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
        /// 连接状态变化
        /// </summary>
        public event Action<object, bool> StateChanged;
        #endregion

        #region 方法
        /// <summary>
        /// 通讯参数配置
        /// </summary>
        /// <param name="DrvItem"></param>
        /// <returns></returns>
        public bool Config(DriverItem<SerialCommParam> DrvItem) { return true; }
        /// <summary>
        /// 调用函数 -- 连接PLC
        /// </summary>
        public ErrorCode Open() { return ErrorCode.NoError; }
        /// <summary>
        /// 调用函数 -- 断开PLC连接
        /// </summary>
        public void Close() { }
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
        public void ClearLastError() { }
        #endregion
    }
}
