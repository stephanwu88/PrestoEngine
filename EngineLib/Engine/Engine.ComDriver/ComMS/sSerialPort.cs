using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.ComDriver
{
    /// <summary>
    /// 工作状态
    /// </summary>
    public enum SerialState
    {
        Nothing = 0,
        Open = 1,
        Connecting = 2,
        Connected = 3,
        DisConnect = 4,
        Error = 5,
        Disable = 6
    }

    /// <summary>
    /// 工作模式
    /// </summary>
    public enum WorkMode
    {
        Master = 0,
        Slave = 1,
    }

    /// <summary>
    /// 串行操作类
    /// </summary>
    public class sSerialPort
    {

    }
}
