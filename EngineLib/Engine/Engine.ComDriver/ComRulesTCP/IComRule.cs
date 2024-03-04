using System;

namespace Engine.ComDriver
{
    /// <summary>
    /// 通讯错误
    /// </summary>
    public enum ComError
    {
        /// <summary>
        /// 通讯连接正常
        /// </summary>
        Normal,
        /// <summary>
        /// 通讯接收区溢出
        /// </summary>
        RcvBuffer_OverFlow,
        /// <summary>
        /// 通讯数据验证发生错误
        /// </summary>
        RcvBuffer_DataErr,
        /// <summary>
        /// 通讯发送超时
        /// </summary>
        Send_Timeout,
    }

    /// <summary>
    /// 通讯规则管理，检查数据完整性
    /// </summary>
    /// <typeparam name="TBufData">通讯缓冲区数据格式 byte[] or string</typeparam>
    /// <typeparam name="TClientObject">程序集对象方法泛型 sSocket sSerialPort</typeparam>
    /// <typeparam name="TCommParam">通讯参数泛型 NetworkCommParam SerialCommParam</typeparam>
    public interface IComRule<TBufData, TClientObject, TCommParam> where TCommParam : new()
    {
        #region 属性
        /// <summary>
        /// 通讯连接名称 
        /// </summary>
        string ComName { get; set; }
        /// <summary>
        /// 本设备名称
        /// </summary>
        string LocalEquipName { get; set; }
        /// <summary>
        /// 连接目标名称
        /// </summary>
        string RemoteBossName { get; set; }
        /// <summary>
        /// 访问本地客户名称
        /// </summary>
        string PartnerVisitorName { get; set; }
        /// <summary>
        /// 驱动配置
        /// </summary>
        DriverItem<TCommParam> DriverItem { get; }
        /// <summary>
        /// 通讯循环工作开关
        /// </summary>
        bool ComWorking { get; set; }
        /// <summary>
        /// 通讯对象
        /// </summary>
        TClientObject ComObject { get; }
        #endregion

        #region 方法
        /// <summary>
        /// 启动通讯
        /// </summary>
        bool Open();
        /// <summary>
        /// 停止通讯
        /// </summary>
        void Close();
        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="SendData">数据</param>
        /// <param name="LogSend">是否记录发送日志</param>
        /// <returns></returns>
        bool Send(TBufData SendData, bool LogSend = true);
        #endregion

        #region 事件
        /// <summary>
        /// 数据发送事件
        /// </summary>
        event Action<IComRule<TBufData, TClientObject, TCommParam>, TBufData> SendMsg;
        /// <summary>
        /// 数据接收事件
        /// </summary>
        event Action<IComRule<TBufData, TClientObject, TCommParam>, TBufData> RcvMsg;
        /// <summary>
        /// 发送错误事件
        /// </summary>
        event Action<IComRule<TBufData, TClientObject, TCommParam>, ComError> Error;
        #endregion
    }
}
