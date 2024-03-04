
namespace Engine.ComDriver
{
    /// <summary>
    /// 错误代码
    /// </summary>
    public enum ErrorCode
    {
        /// <summary>
        /// 正常
        /// </summary>
        NoError = 0,

        /// <summary>
        /// CPU类型错误
        /// </summary>
        WrongCPU_Type = 1,

        /// <summary>
        /// 配置参数错误
        /// </summary>
        WrongConfigParam = 2,

        /// <summary>
        /// 连接发生错误
        /// </summary>
        ConnectionError = 3,

        /// <summary>
        /// 连接已忽略
        /// </summary>
        ConnectionIgnored = 4,

        /// <summary>
        /// IP地址无效
        /// </summary>
        IPAddressNotAvailable = 5,

        /// <summary>
        /// 变量格式错误
        /// </summary>
        WrongVarFormat = 10,

        /// <summary>
        /// 接收字节数发生错误
        /// </summary>
        WrongNumberReceivedBytes = 11,

        /// <summary>
        /// 发送数据时发生错误
        /// </summary>
        SendData = 20,

        /// <summary>
        /// 读取数据时发生错误
        /// </summary>
        ReadData = 30,

        /// <summary>
        /// 写入数据时发生错误
        /// </summary>
        WriteData = 50
    }
}
