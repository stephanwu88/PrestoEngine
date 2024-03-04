using Engine.Common;
using Engine.Data;
using Engine.Data.DBFAC;
using Engine.Mod;
using Engine.MVVM.Messaging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Threading;

namespace Engine.ComDriver.OBLF
{
    /// <summary>
    /// 指令请求枚举
    /// </summary>
    public enum Enum_REQ
    {
        [Description("@MESSUNG")]
        Request_PS = 1,

        [Description("@MESSUNG")]
        Request_CS = 2,

        [Description("@TYPPROBE")]
        Request_TS = 3,

        [Description("@REKALIBRATION")]
        Request_RS = 4,

        [Description("@PROBE_BEREIT")]
        StartExciter = 5,

        [Description("@MITTELN")]
        ManuallyAverage = 6,

        [Description("@REINIGUNG")]
        CleanPole = 7,

        [Description("@INIT")]
        Initialize = 8,

        [Description("@STATUS_ANFORDERUNG")]
        RequestStatus = 9,
    }

    /// <summary>
    /// 指令反馈枚举
    /// </summary>
    public enum Enum_ACK
    {
        [Description("@STATUS_MELDUNG")]
        SpecStatus = 1
    }

    /// <summary>
    /// 请求字段
    /// </summary>
    public class ReqField
    {
        //Sample identification code 1, max. 25 characters 
        public string _Request_PS__ID1__ { get; set; } = string.Empty;
        public string _Request_PS__ID2__ { get; set; } = string.Empty;
        public string _Request_PS__ID3__ { get; set; } = string.Empty;
        public string _Request_PS__ID4__ { get; set; } = string.Empty;
        public string _Request_PS__ID5__ { get; set; } = string.Empty;
        public string _Request_PS__ID6__ { get; set; } = string.Empty;
        //OBLFwin task number 1-999 任务号
        public string _Request_PS__AUFGABE__ { get; set; } = string.Empty;
        //"Fe", "Cu, "Al" 基体
        public string _Request_PS__MATRIX__ { get; set; } = string.Empty;
        //The analytical program, e.g. "Fe1", "Cu99", "Al1000" 分析程序
        public string _Request_PS__PROGRAMM__ { get; set; } = string.Empty;
        //As option for the Matrix/Program setting a 
        //program group can be defined, e.g. “Nickel”
        public string _Request_PS__PGMGRP__ { get; set; } = string.Empty;
        //„Repro elements“. This is a list of elements where 
        //the reproducibility will be checked. 
        public string _Request_PS__REPROELM__ { get; set; } = string.Empty;
        public string _Request_PS__BELICHTUNG__ { get; set; } = string.Empty;
        //Setting of the minimum number of reproducible 
        //single measurements for the average calculation.
        //Option.As standard the task settings will be used
        public string _Request_PS__MITANZ__ { get; set; } = string.Empty;
        //Maximum number of single measurements for 
        //the average calculation.If more measurements are
        //required, the spectrometer will stop the sequence
        //Option.As standard the task settings will be used.
        public string _Request_PS__MAXANZ__ { get; set; } = string.Empty;
        //Will show only elements which are checked for reproducibility.Option.
        public string _Request_PS__NURREPRO__ { get; set; } = string.Empty;
        //The type standardisation sample will be used 
        public string _Request_PS__TKALSET__ { get; set; } = string.Empty;

        //注册控样名称
        public string _Request_TS__Proben__ { get; set; } = string.Empty;
        //Number of the analytical task, 0-999
        public string _Request_TS__AUFGABE__ { get; set; } = string.Empty;

        //Name of Recal Sample 标样名称
        public string _Request_RS__PROBE__ { get; set; } = string.Empty;
        //最大25个字符
        public string _Request_RS__ID2__ { get; set; } = string.Empty;
        public string _Request_RS__ID3__ { get; set; } = string.Empty;
        public string _Request_RS__ID4__ { get; set; } = string.Empty;
        public string _Request_RS__ID5__ { get; set; } = string.Empty;
        public string _Request_RS__ID6__ { get; set; } = string.Empty;
        //Number of the analytical task, 0-999
        public string _Request_RS__AUFGABE__ { get; set; } = string.Empty;
        //基体  e.g. "Fe", "Cu, "Al" 
        public string _Request_RS__MATRIX__ { get; set; } = string.Empty;
        //分析程序
        public string _Request_RS__PROGRAMM__ { get; set; } = string.Empty;
        //A list of elements which shall be recalibrated 
        //(Option, as standard all elements are recalibrated)
        public string _Request_RS__ELEMENTE__ { get; set; } = string.Empty;
        //No reprofiling will be done
        public string _Request_RS__REPROFIL__ { get; set; } = string.Empty;
        //The resulting data of the first measurement will be ignored
        public string _Request_RS__OHNEERSTE__ { get; set; } = string.Empty;
        //Minimum number of reproducable measurements for an averaging.
        //Option.As standard the task settings will be used.
        public string _Request_RS__MITANZ__ { get; set; } = string.Empty;
        //Maximum number of measurements for an averaging. 
        //If an average can not be done, the measurement will be aborted.
        //Option.As standard the task settings will be used
        public string _Request_RS__MAXANZ__ { get; set; } = string.Empty;
        //Will show only these channels where a repro-limit is defined. (Option)
        public string _Request_RS__NURREPRO__ { get; set; } = string.Empty;

        //The single measurements which shall be used for averaging, e.g. "@NUMMER=1,3,4"
        public string _ManuallyAverage__NUMMER__ { get; set; } = string.Empty;

        //The format is "DD.MM.YYYY hh:mm:ss"
        public string _RequestStatus_TimeStamp { get; set; } = string.Empty;
    }

    /// <summary>
    /// 响应字段
    /// </summary>
    public class AckField
    {
        //FEHLER: 有错误  WARNUNG: 有警告   BEREIT 系统就绪
        //AKTIV: 测量激活，样已在  WARTEN: 激活测量，未来样
        //OFFLINE: 已离线 - 自己添加的，由助手返回
        public string _SpecStatus_GERAET__ { get; set; } = string.Empty;
    }

    /// <summary>
    /// 样品类型
    /// </summary>
    public enum SampleType
    {
        Default,    //缺省   
        PS,         //生产样
        TS,         //类标样
        CS,         //检查样
        RS          //完全标准化样
    }

    /// <summary>
    /// 会话区
    /// </summary>
    public class SessionMemory : NotifyObject
    {
        public string State_Normal { get; set; } = "0";
        public string State_Run { get; set; } = "0";
        public string State_Alarm { get; set; } = "0";
        public string State_Wait { get; set; } = "0";
        public string State_Done { get; set; } = "0";
        public string State_Offline { get; set; } = "0";
        public string State_Online { get; set; } = "0";
        public string ComShake { get; set; } = "0";
        public string IsSparking { get; set; } = "0";
        //注册通过
        public string SampleAllowed { get; set; } = "0";
        //移点信号
        public string InvokePotMove { get; set; } = "0";
        //分析结束信号
        public string InvokeAnaFinish { get; set; } = "0";
        //光室温度
        public string Data_CurrentTemp { get; set; }
        public string Proc_Error { get; set; }
        public string Proc_Warn { get; set; }
        public string Proc_MeasureStep { get; set; }
    }

    /// <summary>
    /// Oblf Spectro Protocol
    /// </summary>
    public class sComOblfSpectro : sComOblfSpectro<ServerNode>
    {
        /// <summary>
        /// 拷贝构造函数
        /// </summary>
        /// <param name="DrvItem"></param>
        public sComOblfSpectro(DriverItem<NetworkCommParam> DrvItem) : base(DrvItem)
        {
            
        }
    }

    /// <summary>
    /// Oblf Spectro Protocol
    /// </summary>
    public class sComOblfSpectro<TConNode> : sComRuleAsc
        where TConNode:new()
    {
        public readonly string AnaDataReceived = SystemDefault.UUID;
        public readonly string AnaDataParseError = SystemDefault.UUID;
        private IDBFactory<TConNode> _DB = DbFactory.Current.GetConn<TConNode>("nd_work");
        public ReqField ReqField = new ReqField();
        public AckField AckField = new AckField();
        public SessionMemory Session { get; } = new SessionMemory();
        /// <summary>
        /// 协议变量列表
        /// </summary>
        List<ModelComInstr> ListComSymbol;
        string LastRcvStatusContent = string.Empty;
        List<string[]> LstFaultError = new List<string[]>();
        List<string[]> LstWarn = new List<string[]>();
        List<string> LstCustomIDFieldItem = new List<string>();
        protected Dictionary<string, ModelSheetColumn> DicResultConfig
            = new Dictionary<string, ModelSheetColumn>();

        /// <summary>
        /// 报文字典
        /// </summary>
        public Dictionary<Enum_REQ, Telegram> _REQTelegrams = new Dictionary<Enum_REQ, Telegram>();
        public Dictionary<Enum_ACK, Telegram> _ACKTelegrams = new Dictionary<Enum_ACK, Telegram>();

        /// <summary>
        /// 构造函数
        /// </summary>
        public sComOblfSpectro()
        {
            STX = "";
            EDX = "@ENDE\0";
            BulidMessageList();
        }

        /// <summary>
        /// 拷贝构造函数
        /// </summary>
        /// <param name="DrvItem"></param>
        public sComOblfSpectro(DriverItem<NetworkCommParam> DrvItem) : base(DrvItem)
        {
            STX = "";
            EDX = "@ENDE\0";
            foreach (Enum_REQ req in Enum.GetValues(typeof(Enum_REQ)))
                _REQTelegrams.Add(req, new Telegram() { MsgType = req.FetchDescription() });
            foreach (Enum_ACK ack in Enum.GetValues(typeof(Enum_ACK)))
                _ACKTelegrams.Add(ack, new Telegram() { MsgType = ack.FetchDescription() });
            UpdateTelegram();
            BulidMessageList();
            if (_DB != null)
                LoadProtocolTable();
        }

        /// <summary>
        /// 创建信息列表
        /// </summary>
        private void BulidMessageList()
        {
            LstFaultError.Clear();
            LstFaultError.Add(new string[] { "AUFGABE", "分析任务不存在" });
            LstFaultError.Add(new string[] { "DIAGNOSE", "自动分析程序出错" });
            LstFaultError.Add(new string[] { "KOMMANDO", "未知指令" });
            LstFaultError.Add(new string[] { "MITTELN", "数据平均出错" });
            LstFaultError.Add(new string[] { "PGMGRP", "分析程序组存在错误" });
            LstFaultError.Add(new string[] { "PROBE", "未知样品" });
            LstFaultError.Add(new string[] { "PROGRAMM", "未知分析程序" });
            LstFaultError.Add(new string[] { "REKAL", "未知样品" });
            LstFaultError.Add(new string[] { "SCHLECHTPROBE", "不良激发点" });
            LstFaultError.Add(new string[] { "TYPKAL", "类型标准化发生错误" });
            LstFaultError.Add(new string[] { "TYPKAL2", "类型标准化发生错误" });

            LstWarn.Clear();
            LstWarn.Add(new string[] { "ABBRUCH", "分析测量被中断" });
            LstWarn.Add(new string[] { "DRUCKER", "打印机无效" });
            LstWarn.Add(new string[] { "PROBENUEBERWACHUNG", "样品不良" });

            LstCustomIDFieldItem = new List<string>()
                    {
                        "","SampleID","MatrialGrade","","StationID","OrderNum","SampleType"
                    };
        }

        /// <summary>
        /// 转换信息
        /// </summary>
        /// <param name="MesKey"></param>
        /// <param name="MesGroup"></param>
        /// <returns></returns>
        private string ConvertMessage(string MesKey, string MesGroup = "Error")
        {
            List<string[]> LstMes = MesGroup == "Error" ? LstFaultError : LstWarn;
            foreach (string[] item in LstMes)
            {
                if (item.Length < 2) continue;
                if (item[0] == MesKey) return item[1];
            }
            return string.Empty;
        }

        /// <summary>
        /// 加载协议列表
        /// </summary>
        /// <param name="ListComData"></param>
        public void LoadProtocolTable(List<ModelComInstr> ListComData)
        {
            if (ListComData != null && ListComSymbol == null)
                return;
            ListComSymbol = ListComData;
        }

        /// <summary>
        /// 加载数据库协议表
        /// </summary>
        private void LoadProtocolTable()
        {
            if (ListComSymbol != null) return;
            ModelComInstr modelCom = new ModelComInstr()
            {
                DriverToken = DriverItem.DriverToken,
                DataUnitType = "DataUnitType in ('Status','Alarm')".MarkExpress().MarkWhere()
            };
            DataTable dt = _DB.ExcuteQuery<ModelComInstr>(modelCom).Result.ToMyDataTable();
            ListComSymbol = ColumnDef.ToEntityList<ModelComInstr>(dt);
            LoadToSessionMemory();
        }

        /// <summary>
        /// 加载到会话内存
        /// </summary>
        private void LoadToSessionMemory()
        {
            if (ListComSymbol == null) return;
            foreach (ModelComInstr mod in ListComSymbol)
            {
                if (string.IsNullOrEmpty(mod.DataValue))
                    continue;
                Session.SetPropValue(mod.DataName,mod.DataValue);
            }
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
        /// 更新发送报文内容
        /// </summary>
        private void UpdateTelegram()
        {
            //ReqField._RequestStatus_TimeStamp = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");
            #region 问询报文
            _REQTelegrams[Enum_REQ.Request_PS].Add(new List<TelegramField>() {
                new TelegramField() { FieldName = "ID1", FieldType = "MUST", FiledMark = "", FieldVal = ReqField._Request_PS__ID1__ },
                new TelegramField() { FieldName = "ID2", FieldType = "OPTION", FiledMark = "", FieldVal = ReqField._Request_PS__ID2__ },
                new TelegramField() { FieldName = "ID3", FieldType = "OPTION", FiledMark = "", FieldVal = ReqField._Request_PS__ID3__ },
                new TelegramField() { FieldName = "ID4", FieldType = "OPTION", FiledMark = "", FieldVal = ReqField._Request_PS__ID4__ },
                new TelegramField() { FieldName = "ID5", FieldType = "OPTION", FiledMark = "", FieldVal = ReqField._Request_PS__ID5__ },
                new TelegramField() { FieldName = "ID6", FieldType = "OPTION", FiledMark = "", FieldVal = ReqField._Request_PS__ID6__ },
                new TelegramField() { FieldName = "AUFGABE", FieldType = "OPTION", FiledMark = "", FieldVal = ReqField._Request_PS__AUFGABE__ },
                new TelegramField() { FieldName = "MATRIX", FieldType = "OPTION", FiledMark = "", FieldVal = ReqField._Request_PS__MATRIX__ },
                new TelegramField() { FieldName = "PROGRAMM", FieldType = "OPTION", FiledMark = "", FieldVal = ReqField._Request_PS__PROGRAMM__ },
                new TelegramField() { FieldName = "PGMGRP", FieldType = "OPTION", FiledMark = "", FieldVal = ReqField._Request_PS__PGMGRP__ },
                new TelegramField() { FieldName = "REPROELM", FieldType = "OPTION", FiledMark = "", FieldVal = ReqField._Request_PS__REPROELM__ },
                new TelegramField() { FieldName = "BELICHTUNG", FieldType = "OPTION", FiledMark = "", FieldVal = ReqField._Request_PS__BELICHTUNG__ },
                new TelegramField() { FieldName = "MITANZ", FieldType = "OPTION", FiledMark = "", FieldVal = ReqField._Request_PS__MITANZ__ },
                new TelegramField() { FieldName = "MAXANZ", FieldType = "OPTION", FiledMark = "", FieldVal = ReqField._Request_PS__MAXANZ__ },
                new TelegramField() { FieldName = "NURREPRO", FieldType = "OPTION", FiledMark = "", FieldVal = ReqField._Request_PS__NURREPRO__ },
                new TelegramField() { FieldName = "TKALSET", FieldType = "OPTION", FiledMark = "", FieldVal = ReqField._Request_PS__TKALSET__ },
            });

            _REQTelegrams[Enum_REQ.Request_TS].Add(new List<TelegramField>() {
                //@TYPPROBE@PROBE=xxxxxx@AUFGABE=xxx@END  110 or 3
                new TelegramField() { FieldName = "PROBE", FieldType = "MUST", FiledMark = "", FieldVal = ReqField._Request_TS__Proben__ },
                new TelegramField() { FieldName = "AUFGABE", FieldType = "MUST", FiledMark = "", FieldVal = ReqField._Request_TS__AUFGABE__ },
                //new TelegramField() { FieldName = "ID1", FieldType = "MUST", FiledMark = "", FieldVal = ReqField._Request_PS__ID1__ },
                //new TelegramField() { FieldName = "ID2", FieldType = "OPTION", FiledMark = "", FieldVal = ReqField._Request_PS__ID2__ },
                //new TelegramField() { FieldName = "ID3", FieldType = "OPTION", FiledMark = "", FieldVal = ReqField._Request_PS__ID3__ },
                //new TelegramField() { FieldName = "ID4", FieldType = "OPTION", FiledMark = "", FieldVal = ReqField._Request_PS__ID4__ },
                //new TelegramField() { FieldName = "ID5", FieldType = "OPTION", FiledMark = "", FieldVal = ReqField._Request_PS__ID5__ },
                //new TelegramField() { FieldName = "ID6", FieldType = "OPTION", FiledMark = "", FieldVal = ReqField._Request_PS__ID6__ },
                //new TelegramField() { FieldName = "AUFGABE", FieldType = "OPTION", FiledMark = "", FieldVal = ReqField._Request_PS__AUFGABE__ },
                //new TelegramField() { FieldName = "MATRIX", FieldType = "MUST", FiledMark = "", FieldVal = ReqField._Request_PS__MATRIX__ },
                //new TelegramField() { FieldName = "PROGRAMM", FieldType = "MUST", FiledMark = "", FieldVal = ReqField._Request_PS__PROGRAMM__ },
                //new TelegramField() { FieldName = "PGMGRP", FieldType = "OPTION", FiledMark = "", FieldVal = ReqField._Request_PS__PGMGRP__ },
                //new TelegramField() { FieldName = "REPROELM", FieldType = "OPTION", FiledMark = "", FieldVal = ReqField._Request_PS__REPROELM__ },
                //new TelegramField() { FieldName = "BELICHTUNG", FieldType = "OPTION", FiledMark = "", FieldVal = ReqField._Request_PS__BELICHTUNG__ },
                //new TelegramField() { FieldName = "MITANZ", FieldType = "OPTION", FiledMark = "", FieldVal = ReqField._Request_PS__MITANZ__ },
                //new TelegramField() { FieldName = "MAXANZ", FieldType = "OPTION", FiledMark = "", FieldVal = ReqField._Request_PS__MAXANZ__ },
                //new TelegramField() { FieldName = "NURREPRO", FieldType = "OPTION", FiledMark = "", FieldVal = ReqField._Request_PS__NURREPRO__ },
                //new TelegramField() { FieldName = "TKALSET", FieldType = "OPTION", FiledMark = "", FieldVal = ReqField._Request_PS__TKALSET__ },
            });

            _REQTelegrams[Enum_REQ.Request_CS].Add(new List<TelegramField>() {
                new TelegramField() { FieldName = "ID1", FieldType = "MUST", FiledMark = "", FieldVal = ReqField._Request_PS__ID1__ },
                new TelegramField() { FieldName = "ID2", FieldType = "OPTION", FiledMark = "", FieldVal = ReqField._Request_PS__ID2__ },
                new TelegramField() { FieldName = "ID3", FieldType = "OPTION", FiledMark = "", FieldVal = ReqField._Request_PS__ID3__ },
                new TelegramField() { FieldName = "ID4", FieldType = "OPTION", FiledMark = "", FieldVal = ReqField._Request_PS__ID4__ },
                new TelegramField() { FieldName = "ID5", FieldType = "OPTION", FiledMark = "", FieldVal = ReqField._Request_PS__ID5__ },
                new TelegramField() { FieldName = "ID6", FieldType = "OPTION", FiledMark = "", FieldVal = ReqField._Request_PS__ID6__ },
                new TelegramField() { FieldName = "AUFGABE", FieldType = "OPTION", FiledMark = "", FieldVal = ReqField._Request_PS__AUFGABE__ },
                new TelegramField() { FieldName = "MATRIX", FieldType = "MUST", FiledMark = "", FieldVal = ReqField._Request_PS__MATRIX__ },
                new TelegramField() { FieldName = "PROGRAMM", FieldType = "MUST", FiledMark = "", FieldVal = ReqField._Request_PS__PROGRAMM__ },
                new TelegramField() { FieldName = "PGMGRP", FieldType = "OPTION", FiledMark = "", FieldVal = ReqField._Request_PS__PGMGRP__ },
                new TelegramField() { FieldName = "REPROELM", FieldType = "OPTION", FiledMark = "", FieldVal = ReqField._Request_PS__REPROELM__ },
                new TelegramField() { FieldName = "BELICHTUNG", FieldType = "OPTION", FiledMark = "", FieldVal = ReqField._Request_PS__BELICHTUNG__ },
                new TelegramField() { FieldName = "MITANZ", FieldType = "OPTION", FiledMark = "", FieldVal = ReqField._Request_PS__MITANZ__ },
                new TelegramField() { FieldName = "MAXANZ", FieldType = "OPTION", FiledMark = "", FieldVal = ReqField._Request_PS__MAXANZ__ },
                new TelegramField() { FieldName = "NURREPRO", FieldType = "OPTION", FiledMark = "", FieldVal = ReqField._Request_PS__NURREPRO__ },
                new TelegramField() { FieldName = "TKALSET", FieldType = "OPTION", FiledMark = "", FieldVal = ReqField._Request_PS__TKALSET__ },
            });

            _REQTelegrams[Enum_REQ.Request_RS].Add(new List<TelegramField>() {
                new TelegramField() { FieldName = "PROBE", FieldType = "MUST", FiledMark = "", FieldVal = ReqField._Request_RS__PROBE__ },
                new TelegramField() { FieldName = "ID2", FieldType = "OPTION", FiledMark = "", FieldVal = ReqField._Request_RS__ID2__ },
                new TelegramField() { FieldName = "ID3", FieldType = "OPTION", FiledMark = "", FieldVal = ReqField._Request_RS__ID3__ },
                new TelegramField() { FieldName = "ID4", FieldType = "OPTION", FiledMark = "", FieldVal = ReqField._Request_RS__ID4__ },
                new TelegramField() { FieldName = "ID5", FieldType = "OPTION", FiledMark = "", FieldVal = ReqField._Request_RS__ID5__ },
                new TelegramField() { FieldName = "ID6", FieldType = "OPTION", FiledMark = "", FieldVal = ReqField._Request_RS__ID6__ },
                new TelegramField() { FieldName = "AUFGABE", FieldType = "OPTION", FiledMark = "", FieldVal = ReqField._Request_RS__AUFGABE__ },
                new TelegramField() { FieldName = "MATRIX", FieldType = "MUST", FiledMark = "", FieldVal = ReqField._Request_RS__MATRIX__ },
                new TelegramField() { FieldName = "PROGRAMM", FieldType = "OPTION", FiledMark = "", FieldVal = ReqField._Request_RS__PROGRAMM__ },
                new TelegramField() { FieldName = "ELEMENTE", FieldType = "OPTION", FiledMark = "", FieldVal = ReqField._Request_RS__ELEMENTE__ },
                new TelegramField() { FieldName = "REPROFIL", FieldType = "OPTION", FiledMark = "", FieldVal = ReqField._Request_RS__REPROFIL__ },
                new TelegramField() { FieldName = "OHNEERSTE", FieldType = "OPTION", FiledMark = "", FieldVal = ReqField._Request_RS__OHNEERSTE__ },
                new TelegramField() { FieldName = "MITANZ", FieldType = "OPTION", FiledMark = "", FieldVal = ReqField._Request_RS__MITANZ__ },
                new TelegramField() { FieldName = "MAXANZ", FieldType = "OPTION", FiledMark = "", FieldVal = ReqField._Request_RS__MAXANZ__ },
                new TelegramField() { FieldName = "NURREPRO", FieldType = "OPTION", FiledMark = "", FieldVal = ReqField._Request_RS__NURREPRO__ }
            });

            _REQTelegrams[Enum_REQ.StartExciter].Add(new List<TelegramField>() { });

            _REQTelegrams[Enum_REQ.ManuallyAverage].Add(new List<TelegramField>() {
                new TelegramField() { FieldName = "NUMMER", FieldType = "MUST", FiledMark = "", FieldVal = ReqField._ManuallyAverage__NUMMER__ }
            });

            _REQTelegrams[Enum_REQ.CleanPole].Add(new List<TelegramField>() { });

            _REQTelegrams[Enum_REQ.Initialize].Add(new List<TelegramField>() { });

            _REQTelegrams[Enum_REQ.RequestStatus].Add(new List<TelegramField>() {
                new TelegramField() { FieldName = "TIME_STAMP", FieldType = "OPTION", FiledMark = "", FieldVal = ReqField._RequestStatus_TimeStamp },
            });

            #endregion

            #region 响应报文
            _ACKTelegrams[Enum_ACK.SpecStatus].Add(new List<TelegramField>() {
                new TelegramField() { FieldName = "GERAET", FieldType = "OPTION", FiledMark = "", FieldVal = AckField._SpecStatus_GERAET__ },
            });
            #endregion
        }

        /// <summary>
        /// 请求仪器状态
        /// </summary>
        /// <returns></returns>
        public bool RequestStatus()
        {
            return RequestStatus(true);
        }

        /// <summary>
        /// 请求仪器状态
        /// </summary>
        /// <returns></returns>
        public bool RequestStatus(bool LogSend)
        {
            string str = string.Empty;
            if (LogSend) LastRcvStatusContent = string.Empty;
            return Request(Enum_REQ.RequestStatus, out str, LogSend);
        }

        /// <summary>
        /// 控样分析准备
        /// </summary>
        /// <param name="SampleID"></param>
        /// <param name="Matrix"></param>
        /// <param name="AnaProgram"></param>
        /// <returns></returns>
        public bool ReservePrepare(string SampleID,string Matrix = "FE", string AnaProgram = "FE 1")
        {
            string ErrorMsg = string.Empty;
            string strSend = string.Empty;
            ReqField._Request_PS__ID1__ = SampleID;
            ReqField._Request_PS__ID2__ = "";
            ReqField._Request_PS__ID3__ = "";
            ReqField._Request_PS__ID4__ = "";
            ReqField._Request_PS__ID5__ = "";
            ReqField._Request_PS__ID6__ = "";
            ReqField._Request_PS__AUFGABE__ = "";
            ReqField._Request_PS__MATRIX__ = Matrix;
            ReqField._Request_PS__PROGRAMM__ = AnaProgram;
            if (!string.IsNullOrEmpty(ErrorMsg)) return false;
            Session.InvokePotMove = "0";
            Session.InvokeAnaFinish = "0";
            Session.SampleAllowed = "0";
            TryParseBinder();
            return Request(Enum_REQ.Request_PS, out strSend);
        }

        /// <summary>
        /// 生产样注册 - 牌号控制分析
        /// </summary>
        /// <param name="SampleID">样品编码</param>
        /// <param name="MatrialGrade">材质牌号</param>
        /// <param name="BurnerID">炉号</param>
        /// <param name="Matrix">基体</param>
        /// <param name="AnaProgram">分析方法</param>
        public bool ReservePS(string SampleID, string MatrialGrade = "", string BurnerID = "",
            string StationID = "", string OrderNum = "",
            string Matrix = "FE", string AnaProgram = "FE 1")
        {
            //自定义: 
            //ID01： BurnerID 炉号  ID02:MaterialGrade 牌号 ID04: StationID 工位 ID05: OrderNum 取样序号
            //ID06： PS CS TS RS

            string ErrorMsg = string.Empty;
            string strSend = string.Empty;
            ReqField._Request_PS__ID1__ = SampleID;
            ReqField._Request_PS__ID2__ = MatrialGrade;
            //ReqField._Request_PS__ID3__ = BurnerID;
            ReqField._Request_PS__ID4__ = StationID;
            ReqField._Request_PS__ID5__ = OrderNum;
            ReqField._Request_PS__ID6__ = SampleType.PS.ToString();
            ReqField._Request_PS__AUFGABE__ = AnaJobType.牌号控制分析.ToEnumStrVal();
            ReqField._Request_PS__MATRIX__ = Matrix;
            ReqField._Request_PS__PROGRAMM__ = AnaProgram;
            if (!string.IsNullOrEmpty(ErrorMsg)) return false;
            Session.InvokePotMove = "0";
            Session.InvokeAnaFinish = "0";
            Session.SampleAllowed = "0";
            TryParseBinder();
            return Request(Enum_REQ.Request_PS, out strSend);
        }

        /// <summary>
        /// 控样注册
        /// </summary>
        /// <param name="SampleID">样品编码</param>
        public bool ReserveTS(string SampleID)
        {
            string ErrorMsg = string.Empty;
            string strSend = string.Empty;
            ReqField._Request_TS__Proben__ = SampleID;
            ReqField._Request_TS__AUFGABE__ = AnaJobType.类型标准化.ToEnumStrVal(); ;
            if (!string.IsNullOrEmpty(ErrorMsg)) return false;
            Session.InvokePotMove = "0";
            Session.InvokeAnaFinish = "0";
            Session.SampleAllowed = "0";
            TryParseBinder();
            return Request(Enum_REQ.Request_TS, out strSend);
        }

        /// <summary>
        /// 外循环控样注册
        /// </summary>
        /// <param name="SampleID">样品编码</param>
        /// <param name="Matrix">基体</param>
        /// <param name="AnaProgram">分析方法</param>
        public bool ReserveTPS(string SampleID, string Matrix = "FE", string AnaProgram = "FE 1")
        {
            string ErrorMsg = string.Empty;
            string strSend = string.Empty;
            ReqField._Request_PS__ID1__ = SampleID;
            ReqField._Request_PS__ID2__ = "";
            ReqField._Request_PS__ID3__ = "";
            ReqField._Request_PS__ID6__ = SampleType.TS.ToString();
            ReqField._Request_PS__AUFGABE__ = AnaJobType.类型标准化.ToEnumStrVal(); ;
            ReqField._Request_PS__MATRIX__ = "";
            ReqField._Request_PS__PROGRAMM__ = AnaProgram;
            if (!string.IsNullOrEmpty(ErrorMsg)) return false;
            Session.InvokePotMove = "0";
            Session.InvokeAnaFinish = "0";
            Session.SampleAllowed = "0";
            TryParseBinder();
            return Request(Enum_REQ.Request_PS, out strSend);
        }

        /// <summary>
        /// 检查样注册
        /// </summary>
        /// <param name="SampleID">样品编码</param>
        /// <param name="MatrialGrade">材质牌号</param>
        /// <param name="BurnerID">炉号</param>
        /// <param name="Matrix">基体</param>
        /// <param name="AnaProgram">分析方法</param>
        public bool ReserveCS(string SampleID, string MatrialGrade = "", string BurnerID = "", 
            string Matrix = "FE", string AnaProgram = "FE 1")
        {
            string ErrorMsg = string.Empty;
            string strSend = string.Empty;
            ReqField._Request_PS__ID1__ = SampleID;
            ReqField._Request_PS__ID2__ = MatrialGrade;
            ReqField._Request_PS__ID3__ = BurnerID;
            ReqField._Request_PS__ID6__ = SampleType.CS.ToString();
            ReqField._Request_PS__AUFGABE__ = AnaJobType.牌号控制分析.ToEnumStrVal(); ;
            ReqField._Request_PS__MATRIX__ = "";
            ReqField._Request_PS__PROGRAMM__ = AnaProgram;
            if (!string.IsNullOrEmpty(ErrorMsg)) return false;
            Session.InvokePotMove = "0";
            Session.InvokeAnaFinish = "0";
            Session.SampleAllowed = "0";
            TryParseBinder();
            return Request(Enum_REQ.Request_CS, out strSend);
        }

        /// <summary>
        /// 完全标准化样品注册
        /// </summary>
        /// <param name="SampleID">样品编码</param>
        /// <param name="Matrix">基体</param>
        /// <param name="AnaProgram">分析方法</param>
        public bool ReserveRS(string SampleID, string Matrix = "FE", string AnaProgram = "FE 1")
        {
            string ErrorMsg = string.Empty;
            string strSend = string.Empty;
            ReqField._Request_RS__PROBE__ = SampleID;
            ReqField._Request_PS__ID6__ = SampleType.RS.ToString();
            ReqField._Request_RS__MATRIX__ = Matrix;
            ReqField._Request_RS__PROGRAMM__ = AnaProgram;
            if (!string.IsNullOrEmpty(ErrorMsg)) return false;
            Session.InvokePotMove = "0";
            Session.InvokeAnaFinish = "0";
            Session.SampleAllowed = "0";
            TryParseBinder();
            return Request(Enum_REQ.Request_RS, out strSend);
        }

        /// <summary>
        /// 启动激发
        /// </summary>
        public bool StartExciter()
        {
            string ErrorMsg = string.Empty;
            string strSend = string.Empty;
            if (!string.IsNullOrEmpty(ErrorMsg)) return false;
            Session.InvokePotMove = "0";
            Session.IsSparking = "1";
            TryParseBinder();
            return Request(Enum_REQ.StartExciter, out strSend);
        }

        /// <summary>
        /// 清扫电极
        /// </summary>
        public bool CleanPole()
        {
            string ErrorMsg = string.Empty;
            string strSend = string.Empty;
            if (!string.IsNullOrEmpty(ErrorMsg)) return false;
            return Request(Enum_REQ.CleanPole, out strSend);
        }

        /// <summary>
        /// 初始化（强制结束）
        /// </summary>
        public bool Initialze()
        {
            string ErrorMsg = string.Empty;
            string strSend = string.Empty;
            if (!string.IsNullOrEmpty(ErrorMsg)) return false;
            return Request(Enum_REQ.Initialize, out strSend);
        }

        /// <summary>
        /// 手动平均
        /// </summary>
        /// <param name="LstAveragePots"></param>
        public bool ManualAverage(List<string> LstAveragePots)
        {
            string strAveragePots = LstAveragePots.ToMyString(",");
            string ErrorMsg = string.Empty;
            string strSend = string.Empty;
            ReqField._ManuallyAverage__NUMMER__ = strAveragePots;
            if (!string.IsNullOrEmpty(ErrorMsg)) return false;
            return Request(Enum_REQ.ManuallyAverage, out strSend);
        }

        /// <summary>
        /// 复位样品任务
        /// </summary>
        /// <returns></returns>
        public bool ResetSampleTask()
        {
            ClearProcStatus();
            return true;
        }

        /// <summary>
        /// 清除流程状态
        /// </summary>
        private void ClearProcStatus()
        {
            Session.InvokePotMove = "0";
            Session.InvokeAnaFinish = "0";
            Session.IsSparking = "0";
            Session.SampleAllowed = "0";
            Session.Proc_Error = string.Empty;
            RequestStatus(false);
        }

        /// <summary>
        /// 协议请求
        /// </summary>
        /// <param name="eREQ"></param>
        /// <param name="strTelegram"></param>
        public bool Request(Enum_REQ eREQ, out string strTelegram, bool LogSend = true)
        {
            string strMsg = string.Empty;
            UpdateTelegram();
            strMsg = _REQTelegrams[eREQ].MsgType;
            foreach (var fld in _REQTelegrams[eREQ].Fields.Values)
            {
                if (fld.FieldType == "OPTION" && string.IsNullOrEmpty(fld.FieldVal.ToMyString()))
                    continue;
                string strFldMarkLeft = fld.FiledMark == "Ref" ? "\"" : "";
                string strFldMarkRight = fld.FiledMark == "Ref" ? "\"" : "";
                strMsg += string.Format("@{0}={1}{2}{3}", fld.FieldName, strFldMarkLeft, fld.FieldVal, strFldMarkRight);
            }
            strTelegram = strMsg;
            return base.Send(strMsg, LogSend);
        }

        /// <summary>
        /// 协议回应
        /// </summary>
        public bool ACK(Enum_ACK eAck, out string strTelegram, bool LogSend = true)
        {
            string strMsg = string.Empty;
            UpdateTelegram();
            strMsg = _ACKTelegrams[eAck].MsgType;
            foreach (var fld in _ACKTelegrams[eAck].Fields.Values)
            {
                if (fld.FieldType == "OPTION" && string.IsNullOrEmpty(fld.FieldVal.ToMyString()))
                    continue;
                string strFldMarkLeft = fld.FiledMark == "Ref" ? "\"" : "";
                string strFldMarkRight = fld.FiledMark == "Ref" ? "\"" : "";
                strMsg += string.Format("@{0}={1}{2}{3}", fld.FieldName, strFldMarkLeft, fld.FieldVal, strFldMarkRight);
            }
            strTelegram = strMsg;
            return base.Send(strMsg, LogSend);
        }

        /// <summary>
        /// 数据消费者
        /// </summary>
        /// <param name="state"></param>
        protected override void QueueRcvWorker(object state)
        {
            DateTime dtStart = DateTime.Now;
            SocketState LastComState = SocketState.Nothing;
            while (ComWorking)
            {
                try
                {
                    lock (_QueueRcv)
                    {
                        if (_QueueRcv.Count > 0)
                        {
                            dtStart = DateTime.Now;
                            Session.State_Online = "1";
                            string strMessage = _QueueRcv.Dequeue();
                            ParseRecievedContent(strMessage);
                            TryParseBinder();
                        }
                        bool NetStateChanged = false;
                        if (ComState != SocketState.Connected)
                            AckField._SpecStatus_GERAET__ = "OFFLINE";
                        if (ComState != LastComState)
                        {
                            NetStateChanged = true;
                            dtStart = DateTime.Now;
                            LastComState = ComState;
                        }
                        if (AckField._SpecStatus_GERAET__ == "" || AckField._SpecStatus_GERAET__ == "OFFLINE")
                        {
                            DateTime dtNow = DateTime.Now;
                            double iTimeSpan = (dtNow - dtStart).TotalSeconds;
                            if (iTimeSpan > 5 || NetStateChanged)
                            {
                                dtStart = DateTime.Now;
                                RequestStatus(NetStateChanged);
                                TryParseBinder();
                            }
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
        /// 数据发送队列
        /// </summary>
        /// <param name="state"></param>
        protected override void QueueSendWorker(object state)
        {
            while (ComWorking)
            {
                #region 发送队列解析
                try
                {
                    if (ComState == SocketState.Connected && _DB != null)
                    {
                        //lock (CommandQueue)
                        {
                            ModelComInstr modelQueryCmd = new ModelComInstr()
                            {
                                AwaitState = string.Format(" DriverToken = '{0}' and DataUnitType in('Command') and " +
                                    "AwaitState in ('Sending','TaskSending') order by AwaitTime asc", DriverItem.DriverToken)
                                    .MarkExpress().MarkWhere()
                            };
                            CallResult _result = _DB.ExcuteQuery<ModelComInstr>(modelQueryCmd);
                            if (_result.Success)
                            {
                                DataTable dt = _result.Result.ToMyDataTable();
                                foreach (DataRow row in dt.Rows)
                                {
                                    modelQueryCmd = ColumnDef.ToEntity<ModelComInstr>(row);
                                    string strID = modelQueryCmd.ID.ToMyString();
                                    string strCommandValue = modelQueryCmd.DataValue;
                                    string strCmdComment = modelQueryCmd.Comment.ToMyString();
                                    if (string.IsNullOrEmpty(strCmdComment))
                                        strCmdComment = strCommandValue;

                                    string Message = string.Format("【Command】【{0}】{1}",
                                           modelQueryCmd.RelatedGroup.ToMyString(), strCmdComment);
                                    Logger.CommBody.Write(LOG_TYPE.MESS, Message);

                                    //bool SendSuccess = Send(modelQueryCmd.DataValue);
                                    CallResult result = ParseCommandInvoke(modelQueryCmd.RelatedVariable,
                                        modelQueryCmd.DataValue);
                                    if (result.Fail)
                                    {
                                        string Error = string.Format("【Command发送错误】【{0}】{1}:{2}",
                                           modelQueryCmd.RelatedGroup.ToMyString(), strCmdComment,
                                           result.Result.ToMyString());
                                        Logger.CommBody.Write(LOG_TYPE.ERROR, Error);
                                    }
                                    modelQueryCmd = new ModelComInstr()
                                    {
                                        ID = strID.MarkWhere(),
                                        AwaitState = "Sended",
                                        AwaitTime = SystemDefault.StringTimeNow
                                    };

                                    _DB.ExcuteUpdate<ModelComInstr>(modelQueryCmd);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    string ErrMsg = string.Format("【QueueWorker_SND】【{0}】【{1}:{2}】\r\n\t{3}",
                     DriverItem.DriverName, ComIP, ComPort, ex.Message);
                    Logger.Error.Write(LOG_TYPE.ERROR, ErrMsg);
                }
                #endregion
                Thread.Sleep(100);
            }
        }

        /// <summary>
        /// 添加日志记录
        /// </summary>
        /// <param name="strData"></param>
        /// <param name="DataType">SND or RCV</param>
        protected override void AddCommLog(string strData, string DataType)
        {
            if (strData.Contains("@STATUS_MELDUNG"))
            {
                string strSpecState = strData.MidString("@GERAET=", "@");
                if (strSpecState == LastRcvStatusContent)
                    return;
                LastRcvStatusContent = strSpecState;
            }
            string strContent = string.Format("【{0}】 【{1}】  {2}", DataType, DriverItem.DriverName.ToMyString(), strData);
            Logger.CommBody.Write(LOG_TYPE.MESS, strContent);
        }

        /// <summary>
        /// 解析接收报文
        /// </summary>
        /// <param name="strMessage"></param>
        /// <returns></returns>
        protected virtual string ParseRecievedContent(string strMessage)
        {
            string ErrorMessage = string.Empty;
            string strMessKey = strMessage.MidString("@", "@");
            switch (strMessKey)
            {
                case "MANUELL":             //光谱仪手动 @MANUELL@ENDE
                    AckField._SpecStatus_GERAET__ = "OFFLINE";
                    break;

                case "STATUS_MELDUNG":
                    //FEHLER: 有错误  WARNUNG: 有警告   BEREIT 系统就绪
                    //AKTIV: 测量激活，样已在  WARTEN: 激活测量，未来样
                    //OFFLINE: 已离线 - 自己添加的，由助手返回
                    AckField._SpecStatus_GERAET__ = strMessage.MidString("@GERAET=", "@");
                    break;

                case "GRENZEN":         //Response currently active limits for the system values. 
                    break;

                case "ZEITBEDARF":      //注册后返回光室温度 @ZEITBEDARF=13.0@ENDE
                    Session.Data_CurrentTemp = strMessage.MidString("@ZEITBEDARF=", "@");
                    break;

                case "PROBE_AUFLEGEN":  //激发前 -- 任务被允许
                    Session.InvokePotMove = "0";
                    Session.InvokeAnaFinish = "0";
                    Session.SampleAllowed = "1";
                    break;

                case "EINZELWERT":          //单点数据
                case "MITTELWERT":          //平均数据
                    #region 数据定义标记
                    //-----------------------PS
                    //ID1 SampleID  -- 炉号
                    //ID2 MatrialGrade
                    //ID3
                    //ID4 StationID
                    //ID5 OrderNum
                    //ID6 PS
                    //-----------------------TS
                    //ID1 SampleID
                    //ID6 TS
                    //-----------------------CS
                    //ID1 SampleID
                    //ID2 MatrialGrade
                    //ID3 BurnerID
                    //ID4 SteelType
                    //ID5 StationID
                    //ID6 CS
                    //-----------------------CS
                    //ID6 RS
                    #endregion
                    CallResult res = ParseResultData(strMessage);
                    if (res.Success)
                    {
                        ModelOESData data = new ModelOESData();
                        data = res.Result as ModelOESData;
                        Messenger.Default.Send(new GenericMessage<ModelOESData>(this, data), AnaDataReceived);
                    }
                    else
                    {
                        Messenger.Default.Send(new GenericMessage<string>(this, strMessage), AnaDataParseError);
                    }
                    Session.IsSparking = "0";
                    if (strMessKey == "EINZELWERT")
                    {
                        //Session.InvokePotMove = "1";
                        Session.InvokeAnaFinish = "0";
                    }
                    else if (strMessKey == "MITTELWERT")
                    {
                        Session.InvokePotMove = "0";
                        Session.InvokeAnaFinish = "1";
                    }
                    break;

                case "PROBE_VERSCHIEBEN":   //移点
                    Session.InvokePotMove = "1";
                    Session.InvokeAnaFinish = "0";
                    Session.IsSparking = "0";
                    break;

                case "PROBE_ENTFERNEN":     //结束
                    Session.InvokePotMove = "0";
                    Session.InvokeAnaFinish = "1";
                    Session.IsSparking = "0";
                    break;

                case "REPROFILIERUNG":      //完全标准化激发返回
                    break;

                case "FEHLER":          //错误信息
                    string strMesKey = strMessage.MidString("@TEXT=", "@");
                    Session.Proc_Error = ConvertMessage(strMesKey); 
                    break;
                case "WARNUNG":
                    strMesKey = strMessage.MidString("@TEXT=", "@");
                    Session.Proc_Warn = ConvertMessage(strMesKey);
                    break;
            }
            return ErrorMessage;
        }

        /// <summary>
        /// 解析执行指令
        /// </summary>
        /// <param name="CmdExp"></param>
        /// <param name="DataValue"></param>
        /// <returns></returns>
        private CallResult ParseCommandInvoke(string CmdExp, string DataValue)
        {
            CallResult ret = new CallResult() { Success = false, Result = "" };
            try
            {
                if (string.IsNullOrEmpty(CmdExp))
                {
                    ret.Result = "未关联到任务指令";
                    return ret;
                }
                //解析函数名称
                string FuncName = CmdExp.MidString("", "(").Trim();
                //解析函数参数
                string FuncParamField = CmdExp.MidString("(", ")", EndStringSearchMode.FromTailAndToEndWhenUnMatch);
                //参数元
                string[] ArrayDataValue = DataValue.ToMyString().MySplit(",", null, null, false, true).ToArray();
                //参数换元
                FuncParamField = string.Format(FuncParamField, ArrayDataValue);
                //参数字符串按，分割给参
                List<string> FuncParams = FuncParamField.MySplit(",", null, null, false, true);
                return this.SyncInvokeMethod(FuncName, FuncParams.ToArray());
            }
            catch (Exception ex)
            {
                ret.Result = ex.Message;
            }
            return ret;
        }

        /// <summary>
        /// 会话整理
        /// </summary>
        private void SessionMake()
        {
            //FEHLER: 有错误  WARNUNG: 有警告   BEREIT 系统就绪
            //AKTIV: 测量激活，样已在  WARTEN: 激活测量，未来样
            //OFFLINE: 已离线 - 自己添加的，由助手返回
            Session.State_Offline = (AckField._SpecStatus_GERAET__ == "OFFLINE" || AckField._SpecStatus_GERAET__ == "") ? "1" : "0";
            if (Session.State_Offline == "1")
                Session.State_Online = "0";
            else if (AckField._SpecStatus_GERAET__ != "OFFLINE" && !string.IsNullOrEmpty(AckField._SpecStatus_GERAET__))
                Session.State_Online = "1";
            Session.State_Alarm = ((!string.IsNullOrEmpty(Session.Proc_Error) || !string.IsNullOrEmpty(Session.Proc_Warn)) &&
                Session.State_Offline == "0") ? "1" : "0";
            if (Session.State_Offline == "1" || Session.State_Alarm == "1")
            {
                Session.State_Normal = "0";
                Session.State_Run = "0";
                Session.State_Done = "0";
            }
            else
            {
                Session.State_Done = Session.InvokeAnaFinish;
                Session.State_Run = Session.IsSparking == "1" && Session.InvokeAnaFinish == "0" ? "1" : "0";
                Session.State_Normal = Session.State_Done == "0" && Session.State_Run == "0" ? "1" : "0";
            }
            Session.ComShake = ComState == SocketState.Connected ? "1" : "0";
            Session.RaisePropertyNotify();
        }

        /// <summary>
        /// 解析绑定
        /// </summary>
        /// <param name="DataName"></param>
        /// <param name="strDataValue"></param>
        /// <returns></returns>
        protected bool TryParseBinder()
        {
            SessionMake();
            if (DriverItem.ServerUpdEn != "1") return true;
            List<ModelComInstr> LstModel = ListComSymbol.Where(x => x.DataUnitType != "Command").ToList();
            List<string> LstRelatedExpress = new List<string>();
            List<ModelComInstr> LstModelUpdate = new List<ModelComInstr>();
            foreach (ModelComInstr com in LstModel)
            {
                string strRelatedVariable = com.RelatedVariable;
                string strDataValue = Session.GetPropValue(com.DataName).ToMyString();
                //添加通讯值
                ModelComInstr modelUpdate = new ModelComInstr()
                {
                    ID = com.ID.MarkWhere(),
                    DriverToken = com.DriverToken.MarkWhere(),
                    DataValue = strDataValue.ValueAttachMark()
                };
                LstModelUpdate.Add(modelUpdate);
                //解析关联变量
                if (!string.IsNullOrEmpty(strRelatedVariable))
                {
                    List<string> LstRelatedVariable = strRelatedVariable.MySplit("|");
                    foreach (string itmRelated in LstRelatedVariable)
                    {
                        if (itmRelated.Contains("WriteTableRow("))
                        {
                            if (itmRelated.MatchParamCount() > 0)
                            {
                                string strCalcRelted = string.Format(itmRelated, strDataValue);
                                LstRelatedExpress.Add(strCalcRelted);
                            }
                            else if ((strDataValue.ToMyInt() == 1 || strDataValue.ToUpper() == "TRUE") &&
                                com.AddrType.ToMyString().ToUpper() == "BOOL")
                            {
                                LstRelatedExpress.Add(itmRelated);
                            }
                        }
                        else
                        {
                            if (!itmRelated.Contains("="))
                            {
                                string strCalcRelted = string.Format(itmRelated + "={0}", strDataValue);
                                LstRelatedExpress.Add(strCalcRelted);
                            }
                            else if ((strDataValue.ToMyInt() == 1 || strDataValue.ToUpper() == "TRUE") &&
                             com.AddrType.ToMyString().ToUpper() == "BOOL")
                            {
                                LstRelatedExpress.Add(itmRelated);
                            }
                        }
                    }
                }
            }
            if (LstModelUpdate.Count > 0 && _DB != null)
            {
                if (LstRelatedExpress.Count > 0)
                    _DB.ExcuteMacroCommand(LstRelatedExpress);
                CallResult _result = _DB.ExcuteUpdate<ModelComInstr>(LstModelUpdate);
            }
            return true;
        }

        /// <summary>
        /// 解析仪器数据
        /// </summary>
        /// <param name="strResultContent"></param>
        /// <returns></returns>
        public virtual CallResult ParseResultData(string strResultContent)
        {
            #region 数据模板
            //@EINZELWERT @DATUM = 30.05.2020@ZEIT = 18:41:57@MATRIX = FE@PROGRAMM = Recalibration@AUFGABE = 0@ID01 = F6@NUMMER = 002
            //@SOLL = OK
            //@ELEM01 = FeH@WERT01 = 159543  @EINH01 = D
            //@ELEM02 = Fe2@WERT02 = 68662   @EINH02 = D
            //@ELEM03 = CL@WERT03 = 1052    
            //@ELEM04 = CH@WERT04 = 1408.398
            //@ELEM05 = SiL@WERT05 = 51790.426
            //@ELEM06 = SiH@WERT06 = 27178.566
            //@ELEM07 = MnL@WERT07 = 58457.2811
            //@ELEM08 = MnH@WERT08 = 6629.561@ELEM09 = P@WERT09 = 3865.4156
            //@ELEM10 = S@WERT10 = 3058.1097@ELEM11 = CrL@WERT11 = 170242.819@ELEM12 = CrH@WERT12 = 52178.723
            //@ELEM13 = NiL@WERT13 = 119968.284@ELEM14 = NiH@WERT14 = 34758.03@ELEM15 = Mo@WERT15 = 59668.240
            //@ELEM16 = Cu@WERT16 = 15904.803@ELEM17 = Al@WERT17 = 6276.6317@ELEM18 = Ti@WERT18 = 5879.2927
            //@ELEM19 = V@WERT19 = 14875.9269@ELEM20 = NbL@WERT20 = 15566.9631@ELEM21 = NbH@WERT21 = 4262.487
            //@ELEM22 = W@WERT22 = 18824.706@ELEM23 = CoL@WERT23 = 13936.995@ELEM24 = B@WERT24 = 2078.6346
            //@ELEM25 = Ca@WERT25 = 2112.5966@ELEM26 = Sb@WERT26 = 3008.951@ELEM27 = As@WERT27 = 8134.484
            //@ELEM28 = Sn@WERT28 = 2267.960@ELEM29 = Bi@WERT29 = 1198.158@ELEM30 = N@WERT30 = 7620.8092
            //@ELEM31 = SPAN@WERT31 = 951@EINH31 = V
            //@ELEM32 = VAKU@WERT32 = +0.85@EINH32 = 1
            //@ELEM33 = TEMP@WERT33 = 32.0@EINH33 = C
            //@ELEM34 = PROF@WERT34 = 0@EINH34 = 1
            //@ELEM35 = ZAEH@WERT35 = 849@EINH35 = 1
            //@ELEM36 = ARGON@WERT36 = 591@EINH36 = l@ENDE

            //@MITTELWERT@DATUM=30.05.2020@ZEIT=18:41:57@MATRIX=FE@PROGRAMM=Recalibration@AUFGABE=0@ID01=F6
            //@ELEM01 =FeH@WERT01=159672  @EINH01=D
            //@ELEM02 =Fe2@WERT02=68415   @EINH02=D
            //@ELEM03 =CL@WERT03=1048    
            //@ELEM04 =CH@WERT04=1378.792@ELEM05=SiL@WERT05=51681.978@ELEM06=SiH@WERT06=27133.714@ELEM07=MnL@WERT07=58327.0448
            //@ELEM08 =MnH@WERT08=6614.374@ELEM09=P@WERT09=3837.1196@ELEM10=S@WERT10=3042.5092@ELEM11=CrL@WERT11=170255.952
            //@ELEM12 =CrH@WERT12=52241.896@ELEM13=NiL@WERT13=120039.367@ELEM14=NiH@WERT14=34773.88@ELEM15=Mo@WERT15=59872.136
            //@ELEM16 =Cu@WERT16=15882.157@ELEM17=Al@WERT17=6143.5594@ELEM18=Ti@WERT18=5562.4561@ELEM19=V@WERT19=14853.0135
            //@ELEM20 =NbL@WERT20=15485.4528@ELEM21=NbH@WERT21=4236.062@ELEM22=W@WERT22=18931.126@ELEM23=CoL@WERT23=13897.315
            //@ELEM24 =B@WERT24=2084.8738@ELEM25=Ca@WERT25=1930.2032@ELEM26=Sb@WERT26=3016.364@ELEM27=As@WERT27=8169.557
            //@ELEM28 =Sn@WERT28=2270.738@ELEM29=Bi@WERT29=1209.771@ELEM30=N@WERT30=7536.9332@ELEM31=SPAN@WERT31=951@EINH31=V
            //@ELEM32 =VAKU@WERT32=+0.85@EINH32=1@ELEM33=TEMP@WERT33=32.0@EINH33=C@ELEM34=PROF@WERT34=0@EINH34=1
            //@ELEM35 =ZAEH@WERT35=849@EINH35=1@ELEM36=ARGON@WERT36=591@EINH36=l@ENDE
            #endregion
            //ID1 SampleID  -- 炉号
            //ID2 MatrialGrade
            //ID3
            //ID4 StationID
            //ID5 OrderNum
            //ID6 PS
            CallResult res = new CallResult() { Success = false };
            ModelOESData ResultData = new ModelOESData();
            List<string> LstResultFields = strResultContent.MySplit("@");
            //验证分析数据报文标记
            if (!LstResultFields.Contains("EINZELWERT") && !LstResultFields.Contains("MITTELWERT"))
            {
                res.Result = "非数据报文无法继续解析！";
                return res;
            }
            //解析报文所有字段、值
            Dictionary<string, string> DicElemItem = new Dictionary<string, string>();
            Dictionary<string, string> DicElemVal = new Dictionary<string, string>();
            foreach (var item in LstResultFields)
            {
                if (!item.ToMyString().Contains("="))
                    continue;
                string strFieldItem = item.MidString("", "=").Trim();
                string strFieldVal = item.MidString("=", "").Trim();
                if (strFieldItem.StartsWith("EINH"))
                    continue;
                if (strFieldItem.StartsWith("ELEM"))
                {
                    string strElemID = strFieldItem.MidString("ELEM", "").Trim();
                    if (!DicElemItem.ContainsKey(strElemID)) DicElemItem.Add(strElemID, strFieldVal);
                }
                else if (strFieldItem.StartsWith("WERT"))
                {
                    string strElemID = strFieldItem.MidString("WERT", "").Trim();
                    //弃除符号标记 ~ + - 等
                    if (!strFieldVal.MidString(0, 1).IsNumeric())
                        strFieldVal = strFieldVal.MidString(1);
                    if (!DicElemVal.ContainsKey(strElemID)) DicElemVal.Add(strElemID, strFieldVal);
                }
                else if (strFieldItem.StartsWith("ID"))
                {
                    int iIDNum = strFieldItem.MidString("ID","").ToMyInt();
                    if (iIDNum > 0 && iIDNum <= 6)
                    {
                        ResultData.DicSqlField.AppandDict(LstCustomIDFieldItem[iIDNum], strFieldVal);
                    }
                }
                else
                {
                    ResultData.DicSrcField.AppandDict(strFieldItem, strFieldVal);
                    if (strFieldItem == "AUFGABE" && strFieldVal == "0")
                        ResultData.DicSqlField.AppandDict("SampleType", "RS");
                    else if (strFieldItem == "AUFGABE" && strFieldVal.MyContains("3,110",","))
                        ResultData.DicSqlField.AppandDict("SampleType", "TS");
                }
            }
            foreach (string strID in DicElemItem.Keys)
            {
                string strItem = string.Format("ELEM_{0}", DicElemItem[strID].ToMyString().ToUpper());
                if (ResultData.DicSrcField.MyContains(strItem) || !DicElemVal.MyContains(strID))
                    continue;
                string strItemVal = DicElemVal[strID];
                ResultData.DicSrcField.AppandDict(strItem, strItemVal);
            }
            //生产样的注册样号就是炉号
            if (ResultData.DicSqlField.DictFieldValue("SampleType").ToMyString() == "PS")
                ResultData.DicSqlField.AppandDict("BurnerID", ResultData.DicSqlField.DictFieldValue("SampleID").ToMyString());
            //附加逻辑标记字段 - 数据分组
            if (LstResultFields[0] == "EINZELWERT")
                ResultData.DicSrcField.AppandDict("DataGroup", "SD");
            else if (LstResultFields[0] == "MITTELWERT")
                ResultData.DicSrcField.AppandDict("DataGroup", "AVG");
            //附加逻辑标记字段 - 分析完成时间
            string strAnaEndTime = GetAnaDataReceiveTime(ResultData.DicSrcField).ToString("yyyy/MM/dd HH:mm:ss");
            ResultData.DicSrcField.AppandDict("RecTime", strAnaEndTime);
            ResultData.DicSrcField.AppandDict("AnaEndTime", strAnaEndTime);
            res.Result = ResultData;
            res.Success = true;
            return res;
        }

        /// <summary>
        /// 接收仪器数据时间
        /// </summary>
        /// <param name="DicResult">仪器结果字典</param>
        /// <returns></returns>
        private DateTime GetAnaDataReceiveTime(Dictionary<string, object> DicResult)
        {
            //30.05.2020 - 日.月.年
            //18:41:57 分析时间
            DateTime dtTime = DateTime.Now;
            string strAnaDate = DicResult.DictFieldValue("DATUM").ToMyString();
            string strAnaTime = DicResult.DictFieldValue("ZEIT").ToMyString();
            string strComDateTime = string.Empty;
            string[] strDate = strAnaDate.Split('.');
            if (strDate.Length >= 3)
                strComDateTime = string.Format("{0}/{1}/{2} {3}", strDate[2], strDate[1], strDate[0], strAnaTime);
            DateTime.TryParse(strComDateTime, out dtTime);
            return dtTime;
        }
    }
}
