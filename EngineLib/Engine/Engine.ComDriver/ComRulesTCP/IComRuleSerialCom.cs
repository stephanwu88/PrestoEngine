
using System.IO.Ports;

namespace Engine.ComDriver
{
    /// <summary>
    /// 串行通讯接口规约
    /// </summary>
    /// <typeparam name="TBufData"></typeparam>
    public interface IComRuleSerialCom<TBufData> : IComRule<TBufData, sSerialPort, SerialCommParam> 
    {
        /// <summary>
        /// 通讯状态
        /// </summary>
        SerialState ComState { get; }
        /// <summary>
        /// 工作模式
        /// </summary>
        WorkMode ComWorkMode { get; }

        /// <summary>
        /// 通讯配置
        /// </summary>
        /// <param name="ComPort">通讯端口 ex:COM3</param>
        /// <param name="DataFormat">数据格式 ex:9600,n,8,1</param>
        /// <param name="ConMode">工作模式 Master Slave</param>
        /// <returns></returns>
        bool Config(string ComPort, string DataFormat, string ConMode);
    }
}
