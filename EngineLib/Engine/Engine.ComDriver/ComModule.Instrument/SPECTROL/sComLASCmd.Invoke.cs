using Engine.Common;
using System.Collections.Generic;
using System.Xml;

namespace Engine.ComDriver.SPECTROL
{
    /*
        InvokeRequest("GetTypeStandardNames","")
        InvokeRequest("GetAnalysisResults","SampleResultsType={0}|SampleResultsValuesType={1}")

        InvokeRequest("AsyncStartExciter()")
        InvokeRequest("AsyncCleanPole()")
        InvokeRequest("AsyncReservePS({0},{1},{2})")
         */
    /// <summary>
    /// 主控委托调用
    /// </summary>
    /// <typeparam name="TConNode"></typeparam>
    public partial class sComLASCmd<TConNode>
    {
        /// <summary>
        /// 主控方包装指令 - 协议指令发送
        /// IPS->SparkWin
        /// </summary>
        /// <param name="CommandName"></param>
        /// <param name="CommandParam"></param>
        /// <returns></returns>
        public bool InvokeRequestWithParam(string CommandName, string CommandParam)
        {
            string VirtualCommandName = "InvokeTemplate";
            WriteCommandParam(CommandName, "InvokeParam", CommandParam);
            string strCommandXml = GetRenamedCommandXml(VirtualCommandName, $"{CommandName}@Invoke");
            List<string> lstField = strCommandXml.RegexMatchedList(RegexFieldText,true);
            foreach (string item in lstField)
            {
                strCommandXml = strCommandXml.MyReplace(item, ReqField[VirtualCommandName].GetString(item));
            }
            return Send(strCommandXml, true);
        }

        /// <summary>
        /// 主控方包装指令 - 委托方法发送
        /// IPS->SparkWin
        /// </summary>
        /// <param name="CommandExpress"></param>
        /// <returns></returns>
        public bool InvokeRequest(string CommandExpress)
        {
            string VirtualCommandName = "InvokeTemplate";
            WriteCommandParam(VirtualCommandName, "InvokeParam", CommandExpress);
            if (CommandExpress.ToLower().Contains("reset"))
            {
                Session.InvokeAnaFinish = "0";
                Session.InvokePotMove = "0";
            }
            string strCommandXml = GetRenamedCommandXml(VirtualCommandName, $"{VirtualCommandName}@Invoke");
            List<string> lstField = strCommandXml.RegexMatchedList(RegexFieldText, true);
            foreach (string item in lstField)
            {
                strCommandXml = strCommandXml.MyReplace(item, ReqField[VirtualCommandName].GetString(item));
            }
            return Send(strCommandXml, true);
        }

        /// <summary>
        /// 获取重命名指令XML模版
        /// </summary>
        /// <param name="CommandName"></param>
        /// <param name="ReName"></param>
        /// <returns></returns>
        public string GetRenamedCommandXml(string CommandName, string ReName)
        {
            XmlNode nd = CommandReqDoc.SelectSingleNode($"/AutomationCommandList/AutomationCommand[@Name='{CommandName}']");
            XmlDocument doc = nd.ImportToNewDoc();
            nd = doc.SelectSingleNode("//AutomationCommand");
            if (!string.IsNullOrEmpty(ReName))
                nd.Attributes["Name"].Value = ReName;
            return doc.ToMyString();
        }

        /// <summary>
        /// 主控表达式写参
        /// </summary>
        /// <param name="CommandName"></param>
        /// <param name="ParamExpress">格式：key1=val1 | key2=val2 </param>
        public void WriteCommandParam(string CommandName, string ParamExpress)
        {
            List<string> LstParam = ParamExpress.MySplit("|");
            foreach (string exp in LstParam)
            {
                string key = exp.MidString("", "=").Trim();
                string val = exp.MidString("", "=").Trim();
                WriteCommandParam(CommandName, key, val);
            }
        }
    }
}
