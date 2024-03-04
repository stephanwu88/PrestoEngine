using Engine.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using Engine.Data.DBFAC;
using Engine.Core;
using Engine.Data;
using System.Data;
using Engine.Mod;
using System.Threading.Tasks;
using System.Xml;

namespace Engine.ComDriver.ARL
{
    /// <summary>
    /// 指令请求枚举
    /// </summary>
    public enum Enum_REQ
    {
        [Description("ARE_YOU_THERE@")]
        Are_you_there = 1,

        [Description("CENTRAL_FILING_REQUESTED@")]
        Central_Filing_Requested = 2,

        [Description("DELETE_REGISTERED_PS@")]
        Delete_Registered_PS = 3,

        [Description("DISABLE_SCT_SESSION@")]
        Disable_SCT_Session = 4,

        [Description("DISCONNECT@")]
        Disconnect = 5,

        [Description("ENABLE_SCT_SESSION@")]
        Enable_SCT_Session = 6,

        [Description("EXTERN_ANALYSIS_RESULT@")]
        External_Analysis_Result = 7,

        [Description("GET_SCT_SESSION@")]
        Get_SCT_Session = 8,

        [Description("GET_SMS_MODE@")]
        Get_SMS_Mode = 9,

        [Description("I_AM_HERE@")]
        I_am_here = 10,

        [Description("MESSAGE_ERROR@")]
        Message_Error = 11,

        [Description("PREPARATION_MAHCINE_REQUESTED@")]
        Preparation_Machine_Requested = 12,

        [Description("PREPARATION_MAHCINE_STATUS@")]
        Preparation_Machine_Status = 13,

        [Description("PROCESS_CS@")]
        Process_Control_Samples = 14,

        [Description("PROCESS_PS@")]
        Process_Production_Sample = 15,

        [Description("PROCESS_STANDARDIZATION@")]
        Process_Standardisation = 16,

        [Description("PROCESS_TS@")]
        Process_Type_Standard_Samples = 17,

        [Description("REGISTER_PS@")]
        Register_Production_Sample = 18,

        [Description("SAMPLE_PREPARED@")]
        Sample_Prepared = 19,

        [Description("SET_CLOCK@")]
        Set_Clock = 20
    }

    /// <summary>
    /// 指令反馈枚举
    /// </summary>
    public enum Enum_ACK
    {
        [Description("ARE_YOU_THERE@")]
        Are_you_there = 1,

        [Description("ASK_FOR_PREPARATION_MACHINE@")]
        Ask_for_PrepMachine = 2,

        [Description("CS_PROCESS_FINISHED@")]
        Control_Sample_Process_Finished = 3,

        [Description("CS_PROCESS_STARTED@")]
        Control_Sample_Process_Started = 4,

        [Description("DISCONNECT@")]
        Disconnect = 5,

        [Description("FILE_SAMPLE@")]
        File_Sample = 6,

        [Description("FREE_CENTRAL_FILING@")]
        Free_Central_Filing = 7,

        [Description("FREE_PREPARATION_MACHINE@")]
        Free_PrepMachine = 8,

        [Description("I_AM_HERE@")]
        I_am_here = 9,

        [Description("LOG_MESSAGE@")]
        Log_Message = 10,

        [Description("MESSAGE_ERROR@")]
        Message_Error = 11,

        [Description("PREPARE_SAMPLE@")]
        Prepare_Sample = 12,

        [Description("PS_PROCESS_FINISHED@")]
        Production_Sample_Process_Finished = 13,

        [Description("PS_PROCESS_STARTED@")]
        Production_Sample_Process_Started = 14,

        [Description("PS_REGISTERED@")]
        Production_Sample_Registered = 15,

        [Description("PUNCHED_PS_DESTINATION@")]
        Punched_Production_Sample_Destination = 16,

        [Description("REGISTERED_PS_DELETED@")]
        Registered_Production_Sample_Deleted = 17,

        [Description("REQUEST_CENTRAL_FILING@")]
        Request_Central_Filing = 18,

        [Description("REQUEST_PREPARATION_MACHINE@")]
        Request_PrepMachine = 19,

        [Description("SAMPLE_GRIPPED@")]
        Sample_Gripped = 20,

        [Description("SCT_SESSION@")]
        SCT_Session = 21,

        [Description("SCT_SESSION_DISABLED@")]
        SCT_Session_Disabled = 22,

        [Description("SMS_MODE@")]
        SMS_Mode = 23,

        [Description("STANDARDIZATION_PROCESS_FINISHED@")]
        SCT_Process_Finished = 24,

        [Description("STANDARDIZATION_PROCESS_STARTED@")]
        SCT_Process_Started = 25,

        [Description("TS_PROCESS_FINISHED@")]
        TS_Process_Finished = 26,

        [Description("TS_PROCESS_STARTED@")]
        TS_Process_Started = 27
    }

    /// <summary>
    /// 请求字段
    /// </summary>
    public class ReqField
    {
        public string _Request__SID__ = string.Empty;

        //Central Filing Requested
        //Status=
        //S Successful
        //F Failed
        public string _Central_Filing_Requested__Status__ = "S";
        public string _Central_Filing_Requested__Reason__ = string.Empty;
        public string _Central_Filing_Requested__PrepMachine__ = string.Empty;
        //Delete Registered PS
        public string _Delete_Registered_PS__SID__ = string.Empty;
        //Disable/Enable SCT session
        //Get SCT Session
        //Get SMS Mode
        public string _Disable_Sct_Session__Instrument__ = string.Empty;
        //Enable SCT session
        public string _Enable_SCT_Session__Instrument__ = string.Empty;
        //Extern Analysis Result
        //Status=
        //G for Good
        //R for Reproducibility problem
        //Q for out of Quality
        public string _Extern_Ana_Result__Status__="G";
        //Get_SCT_Session
        public string _Get_SCT_Session__Instrument__ = string.Empty;
        //Get_SMS_Mode
        public string _Get_SMS_MODE__Instrument__ = string.Empty;
        //Message Error
        //Type=
        //P Protocol(unexpected message)
        //S Syntax(parsing error)
        //U Unknown message
        //O Other(invalid keyword, invalid values…)
        public string _MsgErr__Type__="P";  
        public string _MsgErr__Info__ = string.Empty;
        //Preparation Machine Requested
        //Status=
        //S Successful allocated to SMS
        //F Failed
        public string _PrepMachineRequested__PrepMachine__ = string.Empty;
        public string _PrepMachineRequested__Status__ = "S";
        public string _PrepMachineRequested__Reason__ = string.Empty;
        //Preparation Machine Status
        //Status=
        //A Available and allocated to SMS
        //N Not Avaialable
        public string _PrepMachineStatus__PrepMachine__ = string.Empty;
        public string _PrepMachineStatus__Status__ = "A";
        public string _PrepMachineStatus__Reason__ = string.Empty;
        //Process CS
        public string _ProcessCS__Instrument__ = string.Empty;
        public string _ProcessCS__SID__ = string.Empty;
        public string _ProcessCS__ProgramNumber__ = string.Empty;
        //Process PS
        public string _ProcessPS__SID__ = string.Empty;
        //Process standardisation
        public string _ProcessSTD__Instrument__ = string.Empty;
        public string _ProcessSTD__Programs__ = string.Empty;
        //Prcess TS
        public string _ProcessTS__SID__ = string.Empty;
        public string _ProcessTS__Instrument__ = string.Empty;
        public string _ProcessTS__ProgramNumber__ = string.Empty;
        //Register PS
        public string _RegisterPS__RegScheme__ = string.Empty;
        public string _RegisterPS__Fields__ = string.Empty;
        public string _RegisterPS__StartNow__ = "Y";
        public string _RegisterPS__Robot__ = string.Empty;
        //Sample Prepared
        //Type= P : PS    S : SCT
        //Status= S ： Successful the sample is ready to Analyse     
        //        B ： Bad Sample
        //        N ： No Sample
        public string _SamplePrepared__PrepMachine__ = string.Empty;
        public string _SamplePrepared__SID__ = string.Empty;
        public string _SamplePrepared__Type__ = "P";
        public string _SamplePrepared__Status__ = "S";
        public string _SamplePrepared__Reason__ = string.Empty;
        //Set Clock
        //YYYY MM DD hh mm ss
        public string _Set_Clock__Year__ = string.Empty;
        public string _Set_Clock__Month__ = string.Empty;
        public string _Set_Clock__Day__ = string.Empty;
        public string _Set_Clock__Hours__ = string.Empty;
        public string _Set_Clock__Minutes__ = string.Empty;
        public string _Set_Clock__Seconds__ = string.Empty;
    }

    /// <summary>
    /// 响应字段
    /// </summary>
    public class AckField
    {
        //CS Process Finished
        //Postion = Name of final position of the sample.Returned only if using the extended Host Computer Protocol
        //Status = 
        //S Successful
        //F Failed(ex: not reproducible, …)
        //A Aborted(problem occurred)
        //O Out of Control
        public string _CS_ProcessFinished__Instrument__ = string.Empty;
        public string _CS_ProcessFinished__SID__ = string.Empty;
        public string _CS_ProcessFinished__Position__ = string.Empty;
        public string _CS_ProcessFinished__Status__ = string.Empty;
        public string _CS_ProcessFinished__Reason__ = string.Empty;
        //CS Process Started
        //Status= 
        //S SuccessFul
        //F Failed
        //I Invalid sample
        //E SCT session enabled
        //Z Standardisation already avtive
        //B SMS Busy
        public string _CS_ProcessStarted__Instrument__ = string.Empty;
        public string _CS_ProcessStarted__SID__ = string.Empty;
        public string _CS_ProcessStarted__Status__ = string.Empty;
        public string _CS_ProcessStarted__Reason__ = string.Empty;
        //File Sample
        //Type=
        //P PS
        //S SCT
        public string _FileSample__SID__ = string.Empty;
        public string _FileSample__Type__ = string.Empty;
        //Free Preparation Machine
        public string _FreePrepMachine__PrepMachine__ = string.Empty;
        //Log Message
        public string _LogMsg__Text__ = string.Empty;
        //Message Error
        //Type= 
        //P Protocol(unexpected message)
        //S Syntax(parsing error)
        //U Unknown message
        //O Other(invalid keyword, invalid values…)
        public string _MsgError__Type__ = string.Empty;
        public string _MsgError__info__ = string.Empty;
        //Prepare Sample
        //Type = P: PS S: SCT
        public string _PrepareSample__PrepMachine__ = string.Empty;
        public string _PrepareSample__SID__ = string.Empty;
        public string _PrepareSample__Type__ = string.Empty;
        public string _PrepareSample__ProgramNumber__ = string.Empty;
        //PS Process finished
        //Position = = Name of final position of the sample. Returned only if using the extended Host Computer Protocol
        //Status = 
        //S Successful
        //F Failed
        //A Aborted
        //N Not Analysed
        public string _PS_ProcessFinished__SID__ = string.Empty;
        public string _PS_ProcessFinished__Position__ = string.Empty;
        public string _PS_ProcessFinished__Status__ = string.Empty;
        public string _PS_ProcessFinished__Reason__ = string.Empty;
        //PS Process Started
        //Status= 
        //S SuccessFul
        //F Failed
        //I Invalid sample
        //E SCT session enabled
        //Z Standardisation already avtive
        //B SMS Busy
        public string _PS_ProcessStarted__SID__ = string.Empty;
        public string _PS_ProcessStarted__Status__ = string.Empty;
        public string _PS_ProcessStarted__Reason__ = string.Empty;
        //PS Registered
        //Status =
        //S Successful
        //F Failed
        public string _PS_Registered__Status__ = string.Empty;
        public string _PS_Registered__SID__ = string.Empty;
        public string _PS_Registered__Reason__ = string.Empty;
        public string _PS_Registered__Signature__ = string.Empty;
        //Punched PS Destination
        public string _Punched_PS_Destion__Carousel__ = string.Empty;
        public string _Punched_PS_Destion__Pot__ = string.Empty;
        public string _Punched_PS_Destion__SID__ = string.Empty;
        //Registerd PS Deleted
        //Status=
        //S Successful
        //F Failed
        //I Invalid Sample
        public string _Registered_PS_Deleted__Status__ = string.Empty;
        public string _Registered_PS_Deleted__Reason__ = string.Empty;
        public string _Registered_PS_Deleted__SID__ = string.Empty;
        //Request Prepare Machine
        public string _Request_PrepMachine__SampleNumber__ = string.Empty;
        //Sample Gripped
        public string _SampleGripped__SID__ = string.Empty;
        //SCT Session
        //Status=
        //E Enabled
        //D Disabled
        public string _SCT_Session_Instrument__ = string.Empty;
        public string _SCT_Session_Status__ = string.Empty;
        //SCT Session Disabled
        public string _SCT_Session_Diabled__Instrument__ = string.Empty;
        //SMS Mode
        public string _SMS_MODE__Mode__ = string.Empty;            // A: Automatic M: Manual S: Switching  T:Test
        public string _SMS_MODE__Registration__ = string.Empty;    // E: Enabled D:Disabled
        public string _SMS_MODE__SCTProcessin__ = string.Empty;    // Y: Yes N:NO
        public string _SMS_MODE__HEAO_Status__ = string.Empty;     // A: Automatic M:Maunal R:Ready B:Busy F:Fault
        public string _SMS_MODE__HEAO_SamplerPos__ = string.Empty; // 1: 1# 2:2#
        public string _SMS_MODE__InstrumentMode__ = string.Empty;  // A: Automatic M: Manual S: Switching
        public string _SMS_MODE__PrepMode__ = string.Empty;        // A: Automatic M: Manual S: Switching
        //STANDARDIZATION_PROCESS_FINISHED
        public string _STANDARDIZATION_PROCESS_FINISHED__Instrument__ = string.Empty;
        public string _STANDARDIZATION_PROCESS_FINISHED__Status__ = string.Empty;      //S: Successful F:Failed T:Out of Tolerance
        public string _STANDARDIZATION_PROCESS_FINISHED__Reason__ = string.Empty;
        //STANDARDIZATION_PROCESS_STARTED
        public string _STANDARDIZATION_PROCESS_STARTED__Instrument__ = string.Empty;
        //S:Successful F:Failed I：Invalid Program   E:SCT Session enabled   Z:Standardisation already active    B:SMS Busy
        public string _STANDARDIZATION_PROCESS_STARTED__Status__ = string.Empty;       
        public string _STANDARDIZATION_PROCESS_STARTED__Reason__ = string.Empty;
        //TS_PROCESS_FINISHED
        public string _TS_PROCESS_FINISHED__Instrument__ = string.Empty;
        public string _TS_PROCESS_FINISHED__SID__ = string.Empty;
        public string _TS_PROCESS_FINISHED__Postion__ = string.Empty;
        public string _TS_PROCESS_FINISHED__Status__ = string.Empty;   //S:Successful F:Failed A：Aborted O:Out of Tolerance
        public string _TS_PROCESS_FINISHED__Reason__ = string.Empty;
        //TS_PROCESS_STARTED
        public string _TS_PROCESS_STARTED__Instrument__ = string.Empty;
        public string _TS_PROCESS_STARTED__SID__ = string.Empty;
        //S:Successful F:Failed I：Invalid sample   E:SCT Session enabled   Z:Standardisation already active    B:SMS Busy
        public string _TS_PROCESS_STARTED__Status__ = string.Empty;    
        public string _TS_PROCESS_STARTED__Reason__ = string.Empty;
    }

    /// <summary>
    /// 会话区
    /// </summary>
    public class SessionMemory : NotifyObject
    {
        //状态字
        public string State_Normal { get; set; } = "0";
        public string State_Run { get; set; } = "0";
        public string State_Alarm { get; set; } = "0";
        public string State_Wait { get; set; } = "0";
        public string State_Done { get; set; } = "0";

        //自动化样品传输系统在线
        public string Status_Online { get; set; } = "0";            //0,1
        //自动化样品传输系统离线
        public string Status_Offline { get; set; } = "1";           //0,1

        //SMS模式    
        //A: Automatic 自动模式  M: Manual  手动模式  
        //S: Switching 切换中    T:Test     测试模式
        //:离线

        public string SMS_MODE { get => _SMS_MODE; set { _SMS_MODE = value; RaisePropertyChanged("SMS_MODE"); } } 
        private string _SMS_MODE = "离线";
        
        public string SMS_MODE_AUTO { get => _SMS_MODE_AUTO; set { _SMS_MODE_AUTO = value; RaisePropertyChanged("SMS_MODE_AUTO"); } }
        private string _SMS_MODE_AUTO = "0";
        public string SMS_MODE_MANUAL { get => _SMS_MODE_MANUAL; set { _SMS_MODE_MANUAL = value; RaisePropertyChanged("SMS_MODE_MANUAL"); } }
        private string _SMS_MODE_MANUAL = "0";
        
        public string SMS_MODE_TEST { get => _SMS_MODE_TEST; set { _SMS_MODE_TEST = value; RaisePropertyChanged("SMS_MODE_TEST"); } }
        private string _SMS_MODE_TEST = "0";
        
        public string SMS_MODE_SWITCH { get => _SMS_MODE_SWITCH; set { _SMS_MODE_SWITCH = value; RaisePropertyChanged("SMS_MODE_SWITCH"); } }
        private string _SMS_MODE_SWITCH = "0";
        
        public string SMS_MODE_OFFLINE { get => _SMS_MODE_OFFLINE; set { _SMS_MODE_OFFLINE = value; RaisePropertyChanged("SMS_MODE_OFFLINE"); } }
        private string _SMS_MODE_OFFLINE = "0";

        //通讯握手
        public string ComShake { get => _ComShake; set { _ComShake = value; RaisePropertyChanged("ComShake"); } }
        private string _ComShake = "0";

        //能否注册      
        public string CanRegister { get => _CanRegister; set { _CanRegister = value; RaisePropertyChanged("CanRegister"); } }
        private string _CanRegister = "0";

        //禁止注册      
        public string CannotRegister { get => _CannotRegister; set { _CannotRegister = value; RaisePropertyChanged("CannotRegister"); } }
        private string _CannotRegister = "0";

        //正在标准化    
        public string SCTProcessin { get => _SCTProcessin; set { _SCTProcessin = value; RaisePropertyChanged("SCTProcessin"); } }
        private string _SCTProcessin = "0";

        //注册结果      1: 成功 0:失败
        public string RegisterResult { get; set; } = string.Empty;
        //注册结果描述文本
        public string RegisterResultText { get; set; } = string.Empty;

        //通讯指令超时未响应
        public string Error_MeetOverTime { get; set; } = string.Empty;

        //流程错误信息
        public string Proc_Error
        {
            get => _Proc_Error;
            set
            {
                _Proc_Error = value;
                RaisePropertyChanged("Proc_Error");
            }
        }
        private string _Proc_Error;

        //测量流程步骤
        public string Proc_MeasureStep
        {
            get
            {
                if (string.IsNullOrEmpty(_Proc_MeasureStep))
                    _Proc_MeasureStep = "分析任务待命";
                return _Proc_MeasureStep;
            }
            set
            {
                _Proc_MeasureStep = value;
                RaisePropertyChanged("Proc_MeasureStep");
                Accessor.Current.WritePosItem("", "Proc_MeasureStep", value);
            }
        }
        private string _Proc_MeasureStep;
    }

    /// <summary>
    /// ARL HostComputer客户端协议
    /// </summary>
    public class sComSMS : sComSMS<ServerNode>
    {
        public sComSMS()
        {

        }

        /// <summary>
        /// 拷贝构造函数
        /// </summary>
        /// <param name="DrvItem"></param>
        public sComSMS(DriverItem<NetworkCommParam> DrvItem) : base(DrvItem)
        {

        }
    }

    /// <summary>
    /// ARL HostComputer客户端协议
    /// </summary>
    public class sComSMS<TConNode> : sComRuleAsc
         where TConNode : new()
    {
        private IDBFactory<TConNode> _DB = DbFactory.Current.GetConn<TConNode>("nd_work");
        public ReqField ReqField = new ReqField();
        public AckField AckField = new AckField();        
        public SessionMemory Session { get; } = new SessionMemory();
        /// <summary>
        /// 协议变量列表
        /// </summary>
        private List<ModelComInstr> ListComSymbol;
        private string LastRcvStatusContent = string.Empty;
        static XmlDocument FieldMapDoc;

        /// <summary>
        /// 报文字典
        /// </summary>
        public Dictionary<Enum_REQ, Telegram> _REQTelegrams = new Dictionary<Enum_REQ, Telegram>();
        public Dictionary<Enum_ACK, Telegram> _ACKTelegrams = new Dictionary<Enum_ACK, Telegram>();

        public sComSMS()
        {
            EDX = "\0";
            foreach (Enum_REQ req in Enum.GetValues(typeof(Enum_REQ)))
                _REQTelegrams.Add(req, new Telegram() { MsgType = req.FetchDescription() });
            foreach (Enum_ACK ack in Enum.GetValues(typeof(Enum_ACK)))
                _ACKTelegrams.Add(ack, new Telegram() { MsgType = ack.FetchDescription() });
            UpdateTelegram();
            BulidMessageList();
        }

        public sComSMS(DriverItem<NetworkCommParam> DrvItem) : base(DrvItem)
        {
            EDX = "\0";
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
            FieldMapDoc = sCommon.LoadResourceXml("Engine.ComDriver/ComModule.Instrument/ARL.ComField.xml", out bool Success);
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
            #region 问询报文
            _REQTelegrams[Enum_REQ.Central_Filing_Requested].Add(new List<TelegramField>() {
                new TelegramField() { FieldName = "Status", FieldType = "MUST", FiledMark = "", FieldVal = ReqField._Central_Filing_Requested__Status__ },
                new TelegramField() { FieldName = "Reason", FieldType = "OPTION", FiledMark = "Ref", FieldVal = ReqField._Central_Filing_Requested__Reason__ },
                new TelegramField() { FieldName = "PrepMachine", FieldType = "OPTION", FiledMark = "Ref", FieldVal = ReqField._Central_Filing_Requested__PrepMachine__ }
            });

            //SMS3513_REQ_PAR._Delete_Registered_PS__SID__ = SMS3513_REQ_PAR._Request__SID__;
            _REQTelegrams[Enum_REQ.Delete_Registered_PS].Add(new List<TelegramField>() {
                new TelegramField() { FieldName = "Sample", FieldType = "MUST", FiledMark = "Ref", FieldVal = ReqField._Delete_Registered_PS__SID__ }
            });

            _REQTelegrams[Enum_REQ.Disable_SCT_Session].Add(new List<TelegramField>() {
                new TelegramField() { FieldName = "Instrument", FieldType = "OPTION", FiledMark = "Ref", FieldVal = ReqField._Disable_Sct_Session__Instrument__ }
            });

            _REQTelegrams[Enum_REQ.Enable_SCT_Session].Add(new List<TelegramField>() {
                new TelegramField() { FieldName = "Instrument", FieldType = "OPTION", FiledMark = "Ref", FieldVal = ReqField._Enable_SCT_Session__Instrument__ }
            });

            _REQTelegrams[Enum_REQ.External_Analysis_Result].Add(new List<TelegramField>() {
                new TelegramField() { FieldName = "Status", FieldType = "MUST", FiledMark = "", FieldVal = ReqField._Extern_Ana_Result__Status__ }
            });

            _REQTelegrams[Enum_REQ.Get_SCT_Session].Add(new List<TelegramField>() {
                new TelegramField() { FieldName = "Instrument", FieldType = "OPTION", FiledMark = "Ref", FieldVal = ReqField._Get_SCT_Session__Instrument__ }
            });

            _REQTelegrams[Enum_REQ.Get_SMS_Mode].Add(new List<TelegramField>() {
                new TelegramField() { FieldName = "Instrument", FieldType = "OPTION", FiledMark = "Ref", FieldVal = ReqField._Get_SMS_MODE__Instrument__ }
            });

            _REQTelegrams[Enum_REQ.Message_Error].Add(new List<TelegramField>() {
                new TelegramField() { FieldName = "Type", FieldType = "MUST", FiledMark = "", FieldVal = ReqField._MsgErr__Type__ },
                new TelegramField() { FieldName = "Info", FieldType = "MUST", FiledMark = "Ref", FieldVal = ReqField._MsgErr__Info__ }
            });

            _REQTelegrams[Enum_REQ.Preparation_Machine_Requested].Add(new List<TelegramField>() {
                new TelegramField() { FieldName = "PrepMachine", FieldType = "OPTION", FiledMark = "Ref", FieldVal = ReqField._PrepMachineRequested__PrepMachine__ },
                new TelegramField() { FieldName = "Status", FieldType = "MUST", FiledMark = "", FieldVal = ReqField._PrepMachineRequested__Status__ },
                new TelegramField() { FieldName = "Reason", FieldType = "OPTION", FiledMark = "Ref", FieldVal = ReqField._PrepMachineRequested__Reason__ },
            });

            _REQTelegrams[Enum_REQ.Preparation_Machine_Status].Add(new List<TelegramField>() {
                new TelegramField() { FieldName = "PrepMachine", FieldType = "OPTION", FiledMark = "Ref", FieldVal = ReqField._PrepMachineStatus__PrepMachine__ },
                new TelegramField() { FieldName = "Status", FieldType = "MUST", FiledMark = "", FieldVal = ReqField._PrepMachineStatus__Status__ },
                new TelegramField() { FieldName = "Reason", FieldType = "OPTION", FiledMark = "Ref", FieldVal = ReqField._PrepMachineStatus__Reason__ },
            });

            //SMS3513_REQ_PAR._ProcessCS__SID__ = SMS3513_REQ_PAR._Request__SID__;
            _REQTelegrams[Enum_REQ.Process_Control_Samples].Add(new List<TelegramField>() {
                new TelegramField() { FieldName = "Instrument", FieldType = "OPTION", FiledMark = "Ref", FieldVal = ReqField._ProcessCS__Instrument__ },
                new TelegramField() { FieldName = "Sample", FieldType = "MUST", FiledMark = "Ref", FieldVal = ReqField._ProcessCS__SID__ },
                new TelegramField() { FieldName = "ProgramNumber", FieldType = "OPTION", FiledMark = "", FieldVal = ReqField._ProcessCS__ProgramNumber__ },
            });

            //SMS3513_REQ_PAR._ProcessPS__SID__ = SMS3513_REQ_PAR._Request__SID__;
            _REQTelegrams[Enum_REQ.Process_Production_Sample].Add(new List<TelegramField>() {
                new TelegramField() { FieldName = "Sample", FieldType = "MUST", FiledMark = "Ref", FieldVal = ReqField._ProcessPS__SID__ }
            });

            _REQTelegrams[Enum_REQ.Process_Standardisation].Add(new List<TelegramField>() {
                new TelegramField() { FieldName = "Instrument", FieldType = "OPTION", FiledMark = "Ref", FieldVal = ReqField._ProcessSTD__Instrument__ },
                new TelegramField() { FieldName = "Programs", FieldType = "OPTION", FiledMark = "Ref", FieldVal = ReqField._ProcessSTD__Programs__ }
            });

            //SMS3513_REQ_PAR._ProcessTS__SID__ = SMS3513_REQ_PAR._Request__SID__;
            _REQTelegrams[Enum_REQ.Process_Type_Standard_Samples].Add(new List<TelegramField>() {
                new TelegramField() { FieldName = "Instrument", FieldType = "OPTION", FiledMark = "Ref", FieldVal = ReqField._ProcessTS__Instrument__ },
                new TelegramField() { FieldName = "Sample", FieldType = "MUST", FiledMark = "Ref", FieldVal = ReqField._ProcessTS__SID__ },
                new TelegramField() { FieldName = "ProgramNumber", FieldType = "OPTION", FiledMark = "", FieldVal = ReqField._ProcessTS__ProgramNumber__ }
            });

            //SMS3513_REQ_PAR._RegisterPS__Fields__ = SMS3513_REQ_PAR._Request__SID__;
            _REQTelegrams[Enum_REQ.Register_Production_Sample].Add(new List<TelegramField>() {
                new TelegramField() { FieldName = "RegScheme", FieldType = "OPTION", FiledMark = "Ref", FieldVal = ReqField._RegisterPS__RegScheme__ },
                new TelegramField() { FieldName = "Fields", FieldType = "MUST", FiledMark = "", FieldVal = ReqField._RegisterPS__Fields__ },
                new TelegramField() { FieldName = "StartNow", FieldType = "MUST", FiledMark = "", FieldVal = ReqField._RegisterPS__StartNow__ },
                //new TelegramField() { FieldName = "Robot", FieldType = "OPTION", FiledMark = "", FieldVal = ReqField._RegisterPS__Robot__ }
            });

            //SMS3513_REQ_PAR._SamplePrepared__SID__ = SMS3513_REQ_PAR._Request__SID__;
            _REQTelegrams[Enum_REQ.Sample_Prepared].Add(new List<TelegramField>() {
                new TelegramField() { FieldName = "PrepMachine", FieldType = "OPTION", FiledMark = "Ref", FieldVal = ReqField._SamplePrepared__PrepMachine__ },
                new TelegramField() { FieldName = "Sample", FieldType = "MUST", FiledMark = "Ref", FieldVal = ReqField._SamplePrepared__SID__ },
                new TelegramField() { FieldName = "Type", FieldType = "MUST", FiledMark = "", FieldVal = ReqField._SamplePrepared__Type__ },
                new TelegramField() { FieldName = "Status", FieldType = "MUST", FiledMark = "", FieldVal = ReqField._SamplePrepared__Status__ },
                new TelegramField() { FieldName = "Reason", FieldType = "OPTION", FiledMark = "Ref", FieldVal = ReqField._SamplePrepared__Reason__ }
            });

            ReqField._Set_Clock__Year__ = DateTime.Now.ToString("yyyy");
            ReqField._Set_Clock__Month__ = DateTime.Now.ToString("MM");
            ReqField._Set_Clock__Day__ = DateTime.Now.ToString("dd");
            ReqField._Set_Clock__Hours__ = DateTime.Now.ToString("hh");
            ReqField._Set_Clock__Minutes__ = DateTime.Now.ToString("mm");
            ReqField._Set_Clock__Seconds__ = DateTime.Now.ToString("ss");

            _REQTelegrams[Enum_REQ.Set_Clock].Add(new List<TelegramField>() {
                new TelegramField() { FieldName = "Year", FieldType = "OPTION", FiledMark = "Ref", FieldVal = ReqField._Set_Clock__Year__ },
                new TelegramField() { FieldName = "Month", FieldType = "OPTION", FiledMark = "Ref", FieldVal = ReqField._Set_Clock__Month__ },
                new TelegramField() { FieldName = "Day", FieldType = "OPTION", FiledMark = "Ref", FieldVal = ReqField._Set_Clock__Day__ },
                new TelegramField() { FieldName = "Hours", FieldType = "OPTION", FiledMark = "Ref", FieldVal = ReqField._Set_Clock__Hours__ },
                new TelegramField() { FieldName = "Minutes", FieldType = "OPTION", FiledMark = "Ref", FieldVal = ReqField._Set_Clock__Minutes__ },
                new TelegramField() { FieldName = "Seconds", FieldType = "OPTION", FiledMark = "Ref", FieldVal = ReqField._Set_Clock__Seconds__ }
            });
            #endregion

            #region 响应报文

            _ACKTelegrams[Enum_ACK.SMS_Mode].Add(new List<TelegramField>() {
                new TelegramField() { FieldName = "Mode", FieldType = "MUST", FiledMark = "", FieldVal = AckField._SMS_MODE__Mode__ },
                new TelegramField() { FieldName = "Registration", FieldType = "MUST", FiledMark = "", FieldVal = AckField._SMS_MODE__Registration__ },
                new TelegramField() { FieldName = "SCTProcessing", FieldType = "MUST", FiledMark = "", FieldVal = AckField._SMS_MODE__SCTProcessin__ },
                new TelegramField() { FieldName = "HEAO_Status", FieldType = "MUST", FiledMark = "", FieldVal = AckField._SMS_MODE__HEAO_Status__ },
                new TelegramField() { FieldName = "PREP_POS", FieldType = "MUST", FiledMark = "", FieldVal = AckField._SMS_MODE__HEAO_SamplerPos__ },
                new TelegramField() { FieldName = "InstrumentMode", FieldType = "OPTION", FiledMark = "", FieldVal = AckField._SMS_MODE__InstrumentMode__ },
                new TelegramField() { FieldName = "PreparaionMode", FieldType = "OPTION", FiledMark = "", FieldVal = AckField._SMS_MODE__PrepMode__ },
            });

            _ACKTelegrams[Enum_ACK.Production_Sample_Registered].Add(new List<TelegramField>() {
                new TelegramField() { FieldName = "Status", FieldType = "OPTION", FiledMark = "", FieldVal = AckField._PS_Registered__Status__ },
                new TelegramField() { FieldName = "Sample", FieldType = "OPTION", FiledMark = "Ref", FieldVal = AckField._PS_Registered__SID__ },
                new TelegramField() { FieldName = "Signature", FieldType = "OPTION", FiledMark = "Ref", FieldVal = AckField._PS_Registered__Signature__ },
            });

            _ACKTelegrams[Enum_ACK.Production_Sample_Process_Started].Add(new List<TelegramField>() {
                new TelegramField() { FieldName = "Sample", FieldType = "OPTION", FiledMark = "Ref", FieldVal = AckField._PS_ProcessStarted__SID__ },
                new TelegramField() { FieldName = "Status", FieldType = "OPTION", FiledMark = "", FieldVal = AckField._PS_ProcessStarted__Status__ },
            });
            #endregion
        }

        /// <summary>
        /// 请求仪器状态
        /// </summary>
        /// <returns></returns>
        public bool RequestStatus(bool LogSend = true)
        {
            string strTelegram = string.Empty;
            return Request(Enum_REQ.Get_SMS_Mode,out strTelegram, LogSend);
        }

        /// <summary>
        /// 注册样品信息
        /// </summary>
        /// <param name="SampleID"></param>
        /// <param name="SteelType"></param>
        /// <param name="_RegScheme">规则方法名 ex: SMS-1114-LG</param>
        /// <returns></returns>
        public bool ReserveSample(string SampleID, string SteelType, string InsSelection, string RegScheme)
        {
            ReqField._RegisterPS__RegScheme__ = RegScheme;
            //ReqField._RegisterPS__Fields__ = string.Format("\"{0}\",\"{1}\",\"\",\"\"",this._SampleID.Text,this._SampleID_SUB.Text);
            if (string.IsNullOrEmpty(InsSelection))
                ReqField._RegisterPS__Fields__ = string.Format("\"{0}\",\"{1}\"", SampleID, SteelType);
            else
                ReqField._RegisterPS__Fields__ = string.Format("\"{0}\",\"{1}\",\"{2}\"", SampleID, SteelType, InsSelection);
            ReqField._Request__SID__ = string.Format("{0}", SampleID);
            string strTelegram = string.Empty;
            return Request(Enum_REQ.Register_Production_Sample, out strTelegram);
        }

        /// <summary>
        /// 注册样品信息
        /// </summary>
        /// <param name="SampleID"></param>
        /// <param name="SteelType"></param>
        /// <param name="_RegScheme">规则方法名 ex: SMS-1114-LG</param>
        /// <returns></returns>
        public bool ReserveSample(string SampleID,string SteelType, string RegScheme)
        {
            ReqField._RegisterPS__RegScheme__ = RegScheme;
            //ReqField._RegisterPS__Fields__ = string.Format("\"{0}\",\"{1}\",\"\",\"\"",this._SampleID.Text,this._SampleID_SUB.Text);
            ReqField._RegisterPS__Fields__ = string.Format("\"{0}\",\"{1}\"", SampleID, SteelType);
            ReqField._Request__SID__ = string.Format("{0}", SampleID);
            string strTelegram = string.Empty;
            return Request(Enum_REQ.Register_Production_Sample, out strTelegram);
        }

        /// <summary>
        /// 启动样品任务
        /// </summary>
        /// <param name="SampleID">试样编号</param>
        /// <param name="SteelType">钢种</param>
        /// <param name="_RegScheme">注册模式</param>
        /// <returns></returns>
        public bool StartSampleTask(string SampleID, string SteelType, string InsSelection, string RegScheme)
        {
            if (Session.Proc_MeasureStep.ToMyString() != "分析任务待命")
                return false;
            Session.Proc_MeasureStep = "样品流程启动";
            Session.Proc_Error = string.Empty;
            Session.RegisterResult = string.Empty;
            DateTime dtStart = DateTime.Now;
            Task.Factory.StartNew(() =>
            {
                while (Session.Proc_MeasureStep.ToMyString() != "分析任务待命")
                {
                    switch (Session.Proc_MeasureStep)
                    {
                        case "样品流程启动":
                            RequestStatus();
                            Session.Proc_MeasureStep = "进样前确认状态";
                            break;

                        case "进样前确认状态":
                            if (AckField._SMS_MODE__Registration__ == "E" && AckField._SMS_MODE__SCTProcessin__ == "N" &&
                                    (AckField._SMS_MODE__Mode__ == "A" || AckField._SMS_MODE__Mode__ == "T"))
                            {
                                Session.RegisterResult = string.Empty;
                                ReserveSample(SampleID, SteelType, InsSelection, RegScheme);
                                dtStart = DateTime.Now;
                                Session.Proc_MeasureStep = "样品分析注册";
                            }
                            else
                            {
                                DateTime dtNow = DateTime.Now;
                                double iTimeSpan = (dtNow - dtStart).TotalSeconds;
                                if (iTimeSpan > 2)
                                {
                                    dtStart = DateTime.Now;
                                    Session.Proc_Error = "样品注册时光谱仪状态未就绪";
                                    Logger.Default["REG"].Write( LOG_TYPE.ERROR,string.Format("[{0}]    注册失败，{1}",SampleID, Session.Proc_Error));
                                    Session.Proc_MeasureStep = "分析任务待命";
                                }
                            }
                            break;

                        case "样品分析注册":
                            if (Session.RegisterResult == "成功")
                            {
                                Session.Proc_MeasureStep = "分析任务待命";
                                Logger.Default["REG"].Write(LOG_TYPE.MESS, string.Format("[{0}]    注册成功", SampleID));
                            }
                            else if (Session.RegisterResult == "失败")
                            {
                                Session.Proc_MeasureStep = "分析任务待命";
                                Logger.Default["REG"].Write(LOG_TYPE.MESS, string.Format("[{0}]    注册失败，{1}", SampleID, AckField._PS_Registered__Reason__));
                            }
                            else
                            {
                                DateTime dtNow = DateTime.Now;
                                double iTimeSpan = (dtNow - dtStart).TotalSeconds;
                                if (iTimeSpan > 2)
                                {
                                    dtStart = DateTime.Now;
                                    Session.Proc_Error = "注册指令超时未响应";
                                    Logger.Default["REG"].Write(LOG_TYPE.ERROR, string.Format("[{0}]    注册失败，{1}", SampleID, Session.Proc_Error));
                                    Session.Proc_MeasureStep = "分析任务待命";
                                }
                            }
                            break;
                    }
                    Thread.Sleep(100);
                }
            });
            return true;
        }

        /// <summary>
        /// 协议请求
        /// </summary>
        /// <param name="eREQ"></param>
        /// <param name="strTelegram"></param>
        public bool Request(Enum_REQ eREQ,out string strTelegram, bool LogSend = true)
        {
            string strMsg = string.Empty;
            if (eREQ == Enum_REQ.Get_SMS_Mode)
            {
                AckField._SMS_MODE__Mode__ = "";
                AckField._SMS_MODE__Registration__ = "";
                AckField._SMS_MODE__SCTProcessin__ = "";
                AckField._SMS_MODE__InstrumentMode__ = "";
                AckField._SMS_MODE__PrepMode__ = "";
            }
            UpdateTelegram();
            strMsg = _REQTelegrams[eREQ].MsgType;
            foreach (var fld in _REQTelegrams[eREQ].Fields.Values)
            {
                if (fld.FieldType == "OPTION" && string.IsNullOrEmpty(fld.FieldVal.ToMyString()))
                    continue;
                string strFldMarkLeft = fld.FiledMark=="Ref" ? "\"" : "";
                string strFldMarkRight = fld.FiledMark == "Ref" ? "\"" : "";
                strMsg += string.Format("{0}={1}{2}{3}@",fld.FieldName, strFldMarkLeft,fld.FieldVal, strFldMarkRight);
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
                strMsg += string.Format("{0}={1}{2}{3}@", fld.FieldName, strFldMarkLeft, fld.FieldVal, strFldMarkRight);
            }
            strTelegram = strMsg;
            return base.Send(strMsg,LogSend);
        }

        /// <summary>
        /// 数据消费者
        /// </summary>
        /// <param name="state"></param>
        protected override void QueueRcvWorker(object state)
        {
            DateTime dtStart = DateTime.Now;
            while (ComWorking)
            {
                try
                {
                    lock (_QueueRcv)
                    {
                        if (_QueueRcv.Count > 0)
                        {
                            dtStart = DateTime.Now;
                            string strMessage = _QueueRcv.Dequeue();
                            Session.Status_Online = "1";
                            Session.Status_Offline = "0";
                            TelegramParsedResult parseRes = ParseRecievedContent(strMessage);
                            AddRcvCommLog(parseRes?.CommandText.ToMyString());
                            AddRcvCommLog(parseRes?.StatusText.ToMyString());
                            AddRcvCommLog(parseRes?.ErrorText.ToMyString());
                            SessionMake(parseRes);
                            TryParseBinder();
                        }
                        else
                        {
                            DateTime dtNow = DateTime.Now;
                            double iTimeSpan = (dtNow - dtStart).TotalSeconds;
                            if (iTimeSpan > 5)
                            {
                                dtStart = DateTime.Now;
                                RequestStatus(false);
                                if (DriverItem.ServerUpdEn == "1")
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
                                CallResult result = ParseCommandInvoke(modelQueryCmd.RelatedVariable, modelQueryCmd.DataValue);
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
        /// 添加接收日志
        /// </summary>
        /// <param name="content"></param>
        private void AddRcvCommLog(string content)
        {
            if (string.IsNullOrEmpty(content))
                return;
            AddCommLog(content, "RCV");
        }

        /// <summary>
        /// 添加日志记录
        /// </summary>
        /// <param name="strData"></param>
        /// <param name="DataType">SND or RCV</param>
        protected override void AddCommLog(string strData, string DataType)
        {
            if (DataType == "RCV" && LastRcvStatusContent.Contains(strData))
                return;
            if (strData.EndsWith("\r\n"))
            {
                strData = strData.MidString("", "\r\n", EndStringSearchMode.FromTailAndToEndWhenUnMatch);
            }
            string strContent = string.Format("【{0}】 【{1}】  {2}", DataType, DriverItem.DriverName.ToMyString(), strData);
            Logger.CommBody.Write(LOG_TYPE.MESS, strContent);
        }

        /// <summary>
        /// 解析接收报文
        /// </summary>
        /// <param name="strMessage"></param>
        /// <returns></returns>
        protected virtual TelegramParsedResult ParseRecievedContent(string strMessage)
        {
            string ErrorMessage = string.Empty;
            string strMessKey = strMessage.MidString("", "@");
            TelegramParsedResult parsedRes = new TelegramParsedResult()
            {
                CommandName = strMessKey
            };
            switch (strMessKey)
            {
                case "SMS_MODE":
                    AckField._SMS_MODE__Mode__ = strMessage.MidString("Mode=", "@");
                    AckField._SMS_MODE__Registration__ = strMessage.MidString("Registration=", "@");
                    AckField._SMS_MODE__SCTProcessin__ = strMessage.MidString("SCTProcessing=", "@");
                    AckField._SMS_MODE__InstrumentMode__ = strMessage.MidString("InstrumentMode=", "@");
                    AckField._SMS_MODE__PrepMode__ = strMessage.MidString("PreparationMode=", "@");

                    //Session.SMS_MODE = ConvertMessage("STATUS_MODE", AckField._SMS_MODE__Mode__, ref parsedRes);
                    //Session.CannotRegister = ConvertMessage("STATUS_REGISTER", AckField._SMS_MODE__Registration__, ref parsedRes);
                    //Session.SMS_MODE = ConvertMessage("STATUS_MODE", AckField._SMS_MODE__InstrumentMode__, ref parsedRes);
                    //Session.SMS_MODE = ConvertMessage("STATUS_MODE", AckField._SMS_MODE__PrepMode__, ref parsedRes);

                    if (AckField._SMS_MODE__Mode__ == "A")
                        Session.SMS_MODE = "自动模式";
                    else if (AckField._SMS_MODE__Mode__ == "M")
                        Session.SMS_MODE = "手动模式";
                    else if (AckField._SMS_MODE__Mode__ == "S")
                        Session.SMS_MODE = "切换中";
                    else if (AckField._SMS_MODE__Mode__ == "T")
                        Session.SMS_MODE = "测试模式";
                    else
                        Session.SMS_MODE = "已离线";
                    Session.SMS_MODE_AUTO = Session.SMS_MODE == "自动模式" ? "1" : "0";
                    Session.SMS_MODE_MANUAL = Session.SMS_MODE == "手动模式" ? "1" : "0";
                    Session.SMS_MODE_TEST = Session.SMS_MODE == "测试模式" ? "1" : "0";
                    Session.SMS_MODE_SWITCH = Session.SMS_MODE == "切换中" ? "1" : "0";
                    Session.SMS_MODE_OFFLINE = Session.SMS_MODE != "自动模式" ? "1" : "0";
                    Session.CanRegister = AckField._SMS_MODE__Registration__ == "E" ? "1" : "0";
                    Session.CannotRegister = AckField._SMS_MODE__Registration__ == "E" ? "0" : "1";
                    Session.SCTProcessin = AckField._SMS_MODE__SCTProcessin__ == "Y" ? "1" : "0";
                    break;

                case "PS_REGISTERED":
                    AckField._PS_Registered__Status__ = strMessage.MidString("Status=", "@");
                    AckField._PS_Registered__SID__ = strMessage.MidString("Sample=", "@").Replace("\"", "");
                    AckField._PS_Registered__Reason__ = strMessage.MidString("Reason=", "@").Replace("\"", ""); ;
                    AckField._PS_Registered__Signature__ = strMessage.MidString("Signature=", "@").Replace("\"", "");
                    if (AckField._PS_Registered__Status__ == "S")
                        Session.RegisterResultText = string.Format("[{0}]{1}",AckField._PS_Registered__SID__, "注册成功");
                    else
                        Session.RegisterResultText = string.Format("[{0}]{1}{2}",AckField._PS_Registered__SID__, "注册失败", AckField._PS_Registered__Reason__);
                    Session.RegisterResult = AckField._PS_Registered__Status__ == "S" ? "成功" : "失败";
                    ConvertMessage("PS_REGISTERED_COMMAND", AckField._PS_Registered__Status__, ref parsedRes);
                    break;

                case "PS_PROCESS_STARTED":
                    string strProcessStatus = strMessage.MidString("Status=", "@");
                    if (strProcessStatus != "S")
                    {

                    }
                    ConvertMessage("PS_PROCESS_STARTED_COMMAND", AckField._PS_Registered__Status__, ref parsedRes);
                    break;
            }
            return parsedRes;
        }

        /// <summary>
        /// 转换信息
        /// </summary>
        /// <param name="MesKey"></param>
        /// <param name="Code"></param>
        /// <param name="MessageType"></param>
        /// <returns></returns>
        private string ConvertMessage(string MesKey, string Code, ref TelegramParsedResult parsedResult)
        {
            if (FieldMapDoc == null)
                return string.Empty;
            XmlNodeList nodes = FieldMapDoc.SelectNodes($"FieldMappings/FieldMapping[@Name='{MesKey}']/Field");
            foreach (XmlNode node in nodes)
            {
                string strCode = node.Attributes["Code"]?.Value;
                string strText = node.Attributes["Text"]?.Value;
                string strResCodeType = node.Attributes["Type"]?.Value;
                if (strCode == Code)
                {
                    FillParsedResult(node, ref parsedResult);
                    return strText;
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// 将解析目标XmlNode信息填充到解析结果实体
        /// </summary>
        /// <param name="node"></param>
        /// <param name="parsedResult"></param>
        private void FillParsedResult(XmlNode node, ref TelegramParsedResult parsedResult)
        {
            if (parsedResult == null)
                return;
            string strCode = node.Attributes["Code"]?.Value;
            string strText = node.Attributes["Text"]?.Value;
            string strResCodeType = node.Attributes["Type"]?.Value;
            XmlNode pNode = node.ParentNode;
            string strFieldType = pNode.Attributes["Type"]?.Value;
            if (strFieldType == "COMMAND")
            {
                parsedResult.CommandCode = $"{parsedResult?.CommandName.ToMyString()}:{strCode}";
                if(!string.IsNullOrEmpty(parsedResult.CommandText))
                    parsedResult.CommandText += ",";
                parsedResult.CommandText += strText;
                parsedResult.CommandError = IsErrorOrWarning(strResCodeType);
                parsedResult.ErrorLevel = strResCodeType;
            }
            else if (strFieldType == "STATUS")
            {
                parsedResult.StatusCode = strCode;
                if (!string.IsNullOrEmpty(parsedResult.StatusText))
                    parsedResult.StatusText += ",";
                parsedResult.StatusText += strText;
                parsedResult.StatusError = IsErrorOrWarning(strResCodeType);
                parsedResult.ErrorLevel = strResCodeType;
            }
            else if (IsErrorOrWarning(strFieldType))
            {
                parsedResult.ErrorCode = strCode;
                if (!string.IsNullOrEmpty(parsedResult.ErrorText))
                    parsedResult.ErrorText += ",";
                parsedResult.ErrorText += strText;
                parsedResult.ErrorOccurs = IsErrorOrWarning(strResCodeType);
                parsedResult.ErrorLevel = strResCodeType;
            }
        }

        /// <summary>
        /// 信息类型
        /// </summary>
        /// <param name="MessageType"></param>
        /// <returns></returns>
        public bool IsErrorOrWarning(string MessageType)
        {
            return MessageType.MyContains("ERROR,WARN", ",");
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
        /// <param name="parsedRes"></param>
        private void SessionMake(TelegramParsedResult parsedRes = null)
        {
            Session.Status_Offline = Session.SMS_MODE_OFFLINE;
            if (Session.Status_Offline == "1")
                Session.Status_Online = "0";
            else
            {
                Session.State_Normal = (Session.SMS_MODE_AUTO == "1" && Session.CanRegister == "1") ? "1" : "0";
            }
            //else if (Session.StatusCode_Ins != "OFFLINE" && !string.IsNullOrEmpty(Session.StatusCode_Ins))
            //    Session.Status_Online = "1";
            //Session.State_Alarm = ((!string.IsNullOrEmpty(Session.Proc_Error) || !string.IsNullOrEmpty(Session.Proc_Warn)) &&
            //    Session.Status_Offline == "0") ? "1" : "0";
            //if (Session.Status_Offline == "1" || Session.State_Alarm == "1")
            //{
            //    Session.State_Normal = "0";
            //    Session.State_Run = "0";
            //    Session.State_Done = "0";
            //}
            //else
            //{
            //    //Session.State_Done = Session.InvokeAnaFinish;
            //    //Session.State_Run = Session.IsSparking == "1" && Session.InvokeAnaFinish == "0" ? "1" : "0";
            //    Session.State_Normal = Session.StatusCode_Ins.MyContains("01,02", ",") && Session.StatusCode_Ins_Error == "00" ? "1" : "0";
            //}
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
    }
}
