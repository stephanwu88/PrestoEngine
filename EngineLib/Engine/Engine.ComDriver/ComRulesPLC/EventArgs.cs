
namespace Engine.ComDriver
{
    /// <summary>
    /// 事件参数
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EventArgs<T>
    {
        /// <summary>
        /// 事件关键
        /// </summary>
        public T Key { get; set; }
        /// <summary>
        /// 事件信息
        /// </summary>
        public string Message { get; set; }
    }

    /// <summary>
    /// 事件结果
    /// </summary>
    public enum Result
    {
        Success,
        Fail
    }

    /// <summary>
    /// 读取错误
    /// </summary>
    public enum Error
    {
        //连接时发生错误
        ConnectError,
        //变量越界
        VarListOutOfRange,  
        //程序错误
        ProgError,
    }

    /// <summary>
    /// 通讯事件状态
    /// </summary>
    public enum EventType
    {
        //通讯已连接
        ComBuilded,
        //通讯关闭
        ComClosed,
        //数采开启
        ComReadStarted,
        //数采停止
        ComReadStoped,
        //数据写入
        DataWrited
    }
}
