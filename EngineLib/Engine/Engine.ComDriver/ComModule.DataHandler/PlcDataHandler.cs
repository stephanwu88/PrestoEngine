
namespace Engine.ComDriver
{
    /// <summary>
    /// PLC数据处理器
    /// </summary>
    public class PlcDataHandler
    {
        public static PlcDataHandler Default
        {
            get {
                if (_Default == null)
                    _Default = new PlcDataHandler();
                return _Default;
            }
        }
        private static PlcDataHandler _Default;

      
    }

    internal class SiemensDataHandler
    {

    }
}
