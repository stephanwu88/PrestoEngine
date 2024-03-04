using System.Net.Sockets;

namespace Engine.ComDriver
{
    /// <summary>
    /// 网络型设备合并接口
    /// </summary>
    public interface IComDriverNetCom : IComDriver<Socket, NetworkCommParam>, IComPlcWorker
    {

    }

    /// <summary>
    /// 网络型设备合并接口
    /// </summary>
    /// <typeparam name="TPlcDataType">软元件类型 I Q M DB</typeparam>
    /// <typeparam name="TVarType">数据变量类型 Bool Byte DWord DInt Int Real Timer Count</typeparam>
    public interface IComDriverNetCom<TPlcDataType, TVarType> :
        IComDriver<Socket, NetworkCommParam>,
        IComPlcWorker<TPlcDataType, TVarType>
    {

    }
}
