
using Engine.Common;
using Engine.Mod;
using System.Text;

namespace Engine.ComDriver.SPECTROL
{
    /// <summary>
    /// Spectrol LAS01 Event协议
    /// </summary>
    /// <typeparam name="TConNode"></typeparam>
    public partial class sComLASEvent : sComRuleAsc
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="DrvItem"></param>
        public sComLASEvent(DriverItem<NetworkCommParam> DrvItem) : base(DrvItem)
        {
            STX = "<?xml";
            EDX = "</AutomationEvent>";
            QueueWithEDX = true;
            SendWithSTX = false;
            SendWithEDX = false;
            TextEncoding = UnicodeEncoding.UTF8;
            Max_Buffer_Size = 1 * 1024 * 1024;  //1M
        }

        /// <summary>
        /// 添加日志记录
        /// </summary>
        /// <param name="strData"></param>
        /// <param name="DataType">SND or RCV</param>
        protected override void AddCommLog(string strData, string DataType)
        {
            if (strData.Contains("InstrumentStateChanged"))
                return;
            string strContent = string.Format("【{0}】 【{1}】  {2}", DataType, DriverItem.DriverName.ToMyString(), strData);
            Logger.CommBody.Write(LOG_TYPE.MESS, strContent);
        }
    }
}
