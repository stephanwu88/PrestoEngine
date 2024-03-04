using Engine.Common;
using Engine.Data;
using Engine.Data.DBFAC;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace Engine.ComDriver.Valentin
{
    /* 配置示例
     LabelPrinter.prn配置文件
        ////打包机APL标签机////
        //File | A:\\standard\\apl002.prn
        AM[1] | 1427;2569;0;1;0;3;1;1;42
        BM[1] | {0}
        //AC[1]  | NAME = "ID_01"
        AM[2] | 1719;2569;0;1;0;3;1;1;42
        BM[2] | {1}
        //AC[2]  | NAME = "ID_02"
    */

    /// <summary>
    /// 会话区
    /// </summary>
    public class SessionMemory : NotifyObject
    {
        /// <summary>
        /// 指令流程步骤
        /// </summary>
        public string ProcStep
        {
            get
            {
                if (string.IsNullOrEmpty(_ProcStep))
                    _ProcStep = "打印就绪";
                return _ProcStep;
            }
            set
            {
                _ProcStep = value;
                RaisePropertyChanged("ProcStep");
            }
        }
        private string _ProcStep;
    }

    /// <summary>
    /// 德国进口标签机 Valentin APL100
    /// </summary>
    public partial class sComAPL : sComRuleAsc
    {
        private IDBFactory<ServerNode> _DB = DbFactory.CPU;
        public SessionMemory Session { get; } = new SessionMemory();

        //SOH Start of Heading (标题开始)
        private string SOH = Convert.ToChar(0x01).ToString();
        //End of Transmit Block (传输块结束)
        private string ETB = Convert.ToChar(0x17).ToString();

        /// <summary>
        /// 构造函数
        /// </summary>
        public sComAPL()
        {
            //设置带中文编码方式
            TextEncoding = UnicodeEncoding.UTF8;
            STX = SOH;
            EDX = ETB;
        }

        /// <summary>
        /// 拷贝构造函数
        /// </summary>
        /// <param name="DrvItem"></param>
        public sComAPL(DriverItem<NetworkCommParam> DrvItem) : base(DrvItem)
        {
            //设置带中文编码方式
            TextEncoding = UnicodeEncoding.UTF8;
            STX = SOH;
            EDX = ETB;
        }

        /// <summary>
        /// 网络状态变化
        /// </summary>
        /// <param name="obj"></param>
        protected sealed override void _Com_StateChanged(sSocket sender, SocketState State)
        {
            if (_DB == null)
                return;
            ModelComLink comLink = new ModelComLink()
            {
                AppName = SystemDefault.AppName.MarkWhere(),
                DriverName = DriverItem.DriverName.MarkWhere(),
                ComLink = "TCP/IP".MarkWhere(),
                LinkState = State == SocketState.Connected ? "1" : "0"
            };
            _DB.ExcuteUpdate<ModelComLink>(comLink);
        }

        /// <summary>
        /// 数据消费者
        /// </summary>
        /// <param name="state"></param>
        protected override void QueueRcvWorker(object state)
        {
            while (ComWorking)
            {
                try
                {
                    lock (_QueueRcv)
                    {
                        if (_QueueRcv.Count > 0)
                        {
                            string strMessage = _QueueRcv.Dequeue();
                            string strErrorMessage = ParseRecievedContent(strMessage);
                        }
                    }
                }
                catch (Exception )
                {

                }
                Thread.Sleep(DriverItem.ComParam.CycleTime);
            }
        }

        /// <summary>
        /// 解析接收报文
        /// </summary>
        /// <param name="strMessage"></param>
        /// <returns></returns>
        private string ParseRecievedContent(string strMessage)
        {
            return "";
        }
    }

    /// <summary>
    /// 接口实现
    /// </summary>
    public partial class sComAPL : IComPrinter
    {
        /// <summary>
        /// 设置打印标签内容 - 文件模板排版
        /// </summary>
        /// <param name="PrnFile">模板文件路径</param>
        /// <param name="FieldContent">字段内容</param>
        /// <returns></returns>
        public bool PrintLabel(string PrnFile, string[] FieldContent)
        {
            return PrintLabel(PrnFile, FieldContent, 1);
        }

        /// <summary>
        /// 设置打印标签内容 - 文件模板排版
        /// </summary>
        /// <param name="PrnFile">模板文件路径</param>
        /// <param name="FieldContent">字段内容</param>
        /// <param name="CopyCount">打印份数</param>
        /// <returns></returns>
        public bool PrintLabel(string PrnFile, string[] FieldContent,int CopyCount)
        {
            ////快分钢样标签机
            //File | A:\\standard\\apl002.prn
            //BM[1] | {0}
            //BM[2] | {1}

            //FGA---r0
            //FMB---rA:\\standard\\apl003.prn
            ////TEXT (1/100 mm)
            //□AM[1]1260; 2295; 0; 1; 0; 1; 3; 3; 17□
            //□BM[1]123456□
            //□AC[1]NAME = "ID_01"□
            ////TEXT (1/100 mm)
            //□AM[2]1794; 2444; 0; 1; 0; 1; 3; 3; 17□
            //□BM[2]ACDEFGH□
            //□AC[2]NAME = "ID_02"□
            //// PRINT
            //□FBC---r--------□

            //项目发送案例：FGA---r0FMB---rA:\standard\an45BM[1]1111BM[2] 硅锰BM[3]2023/02/02BM[4] C, SiBM[5]1111BM[6]16:39:08BM[7]FBC---r--------
            if (!File.Exists(PrnFile))
                throw new Exception(string.Format("标签打印机【{0}】模板文件未找到", DriverItem.DriverName.ToMyString()));
            string strFileContent = File.ReadAllText(PrnFile);
            if (strFileContent.MatchParamCount() != FieldContent.Length)
                throw new Exception(string.Format("标签打印机【{0}】内容设置错误", DriverItem.DriverName.ToMyString()));
            //格式化文件中全部字符串，给定转换数组一次完成转换
            strFileContent = string.Format(strFileContent, FieldContent);
            List<object> LstObj = new List<object>();
            List<string> LstLine = strFileContent.MySplit("\r\n");
            //组合拼接发送报文
            string strLabelField = string.Empty;
            //CF卡配置路径
            string strCardConfigFile = string.Empty;
            foreach (string item in LstLine)
            {
                string strLine = item.MidString("", "//");
                List<string> LstContent = strLine.MySplit("|");
                if (LstContent.Count < 2) continue;
                string strFieldName = strLine.MidString("", "|").Trim();
                string strFieldVal = strLine.MidString("|", "").Trim();
                if (strFieldName.ToLower() == "file")
                {
                    //卡内文件路径字符串
                    strCardConfigFile = strFieldVal;
                }
                else
                {
                    //内容字段
                    strLabelField += string.Format("{0}{1}{2}{3}",SOH,strFieldName,strFieldVal,ETB);
                }
            }
            if (!string.IsNullOrEmpty(strCardConfigFile))
            {
                strCardConfigFile = $"{SOH}FMB---r{strCardConfigFile}{ETB}";
            }
            string strLabelContent = $"{SOH}FGA---r0{ETB}";
            strLabelContent += strCardConfigFile;
            strLabelContent += strLabelField;
            strLabelContent += $"{SOH}FBC---r--------{ETB}";
            return Send(strLabelContent);
        }
    }
}
