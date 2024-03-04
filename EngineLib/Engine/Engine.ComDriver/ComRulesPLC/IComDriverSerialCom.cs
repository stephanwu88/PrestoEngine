namespace Engine.ComDriver
{
    /// <summary>
    /// 串口型设备合并接口
    /// </summary>
    public interface IComDriverSerialCom : IComDriver<sSerialPort, SerialCommParam>, IComPlcWorker
    {

    }

    /// <summary>
    /// 串口型设备合并接口
    /// </summary>
    /// <typeparam name="TPlcDataType">软元件类型 I Q M DB</typeparam>
    /// <typeparam name="TVarType">数据变量类型 Bool Byte DWord DInt Int Real Timer Count</typeparam>
    public interface IComDriverSerialCom<TPlcDataType, TVarType> : 
        IComDriver<sSerialPort, SerialCommParam>,
        IComPlcWorker<TPlcDataType, TVarType>
    {

    }
}
