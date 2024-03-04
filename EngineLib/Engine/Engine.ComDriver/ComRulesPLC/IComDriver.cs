using System;

namespace Engine.ComDriver
{
    /// <summary>
    /// 工业通信驱动规约
    /// </summary>
    /// <typeparam name="TClientObject">程序集对象方法泛型 sSocket sSerialPort</typeparam>
    /// <typeparam name="TCommParam">通讯参数泛型 NetworkCommParam SerialCommParam</typeparam>
    public interface IComDriver<TClientObject, TCommParam> where TCommParam : new()
    {
        #region 属性
        /// <summary>
        /// 通讯参数
        /// </summary>
        DriverItem<TCommParam> DriverItem { get; }
        /// <summary>
        /// 通讯循环工作开关
        /// </summary>
        bool ComWorking { get; }
        /// <summary>
        /// 与PLC通信的客户端对象
        /// </summary>
        TClientObject mClient { get; set; }
        /// <summary>
        /// 返回PLC是否可以建立连接
        /// </summary>
        bool IsAvailable { get; }

        /// <summary>
        /// 是否处于连接状态
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        /// Contains the last error registered when executing a function
        /// </summary>
        string LastErrorString { get; set; }

        /// <summary>
        /// Contains the last error code registered when executing a function
        /// </summary>
        ErrorCode LastErrorCode { get; set; }
        #endregion

        #region 事件
        /// <summary>
        /// 消息通知事件
        /// </summary>
        event Action<object, EventArgs<EventType>> ComNote_Rised;
        /// <summary>
        /// 错误捕捉事件
        /// </summary>
        event Action<object, EventArgs<Error>> ComError;
        /// <summary>
        /// 网络状态变化
        /// </summary>
        event Action<object, bool> StateChanged;
        #endregion

        #region 方法
        /// <summary>
        /// 通讯参数配置
        /// </summary>
        /// <param name="DrvItem"></param>
        /// <returns></returns>
        bool Config(DriverItem<TCommParam> DrvItem);
        /// <summary>
        /// 调用函数 -- 连接PLC
        /// </summary>
        ErrorCode Open();
        /// <summary>
        /// 调用函数 -- 断开PLC连接
        /// </summary>
        void Close();
        /// <summary>
        /// 清除错误信息
        /// </summary>
        void ClearLastError();
        #endregion
    }
}
