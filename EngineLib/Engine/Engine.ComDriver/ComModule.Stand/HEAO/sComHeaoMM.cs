using Engine.ComDriver.ARL;

namespace Engine.ComDriver.HEAO
{
    /// <summary>
    /// Heao铣床协议
    /// </summary>
    public class sComHeaoMM : sComARL_MM
    {
        public sComHeaoMM(DriverItem<NetworkCommParam> DrvItem) : base(DrvItem)
        {

        }
    }
}
