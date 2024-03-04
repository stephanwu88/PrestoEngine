using Engine.Data.DBFAC;

namespace Engine.ComDriver.Schneider
{
    /// <summary>
    /// 施耐德TM系列PLC网络通讯
    /// </summary>
    public class ComTMPLC : sComTMPLC
    {
        private IDBFactory<ServerNode> _DB = DbFactory.CPU;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="Drv"></param>
        public ComTMPLC(DriverItem<NetworkCommParam> Drv) : base(Drv)
        {

        }
    }
}
