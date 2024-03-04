using Engine.Common;
using Engine.Data.DBFAC;
using System.Collections.Generic;
using System.Reflection;

namespace Engine.ComDriver
{
    [Table(Name = "driver_main", ViewName = "driver_main_view", Comments = "控制器管理")]
    public class ModelDriverItem : ValidateError<ModelDriverItem>
    {
        [Column(Name = "ID", AI = true, PK = true, Comments = "索引")]
        public string ID { get; set; }

        [Column(Name = "DriverToken", Comments = "控制器ID")]
        public string DriverToken { get; set; }

        [Column(Name = "SnDomain", Comments = "控制域")]
        public string Domain { get; set; }

        [Column(Name = "DriverName", Comments = "控制器名称")]
        [StringRule(MinLen = 2, ErrorMessage = "请设定正确的控制器名称")]
        public string Name { get; set; }

        [Column(Name = "ComLink", Comments = "通讯链路 以太网/串口")]
        public string ComLink { get; set; }

        [Column(Name = "DriverPtl", Comments = "通讯协议")]
        public virtual string Protocol { get; set; }

        [Column(Name = "Provider", Comments = "通讯驱动集")]
        public string Provider { get; set; }

        [Column(Name = "CharType", Comments = "通讯数据格式")]
        public string CharType { get; set; }

        [Column(Name = "ComHeader", Comments = "设备码/报文起始码")]
        [StringRule(MinLen = 4, MaxLen = 4, ErrorMessage = "请设定正确的设备码")]
        public string ComHeader { get; set; }

        [Column(Name = "ComEDX", Comments = "报文结束码")]
        public string ComEDX { get; set; }

        [Column(Name = "ComIP", Comments = "IP地址")]
        [StringRule(MinLen = 7, MaxLen = 15, ErrorMessage = "请设定正确的IP地址")]
        public string ComIP { get; set; }

        [Column(Name= "SerialComFormat", Comments = "串口通讯数据格式")]
        public string SerialComFormat { get; set; }

        [Column(Name = "ComPort", Comments = "通讯端口")]
        public int ComPort { get; set; }

        [Column(Name = "WorkMode", Comments = "通讯模式")]
        public string WorkMode { get; set; }

        [Column(Name = "Comment", Comments = "备注")]
        [StringRule(MaxLen = 60, ErrorMessage = "备注字符超限")]
        public string Comment { get; set; }

        [Column(Name = "DriverEn", Comments = "启用状态")]
        public string Enabled { get; set; }

        [Column(Name = "ServerCmdEn", Comments = "开启服务器控制")]
        public string ServerCmdEn { get; set; }

        [Column(Name = "ServerUpdEn", Comments = "状态自动更新到服务器")]
        public string ServerUpdEn { get; set; }

        [Column(Name = "CycleTime", Comments = "扫描周期 ms")]
        public int CycleTime { get; set; }

        [Column(Name = "ComLibTable", Comments = "通讯数据表")]
        public string ComLibTable { get; set; }

        [Column(Name = "OrderID", Comments = "排序号")]
        public string OrderID { get; set; }

        public ModelDriverItem()
        {
            CycleTime = SystemDefault.InValidInt;
            ComPort = SystemDefault.InValidInt;
        }
    }

    /// <summary>
    /// 视图模式
    /// </summary>
    public class ViewDriverItem : ModelDriverItem
    {
        [Column(Name = "DriverPtl", Comments = "通讯协议")]
        public override string Protocol
        {
            get => _Protocol;
            set
            {
                _Protocol = value;
                if (_Protocol.ToMyString() != "HeaoInter" || ComHeader.ToMyString().Length < 4)
                    ComHeader = "0000";
                RaisePropertyChanged("ComHeader");
            }
        }
        private string _Protocol;

        public static List<string> DomainItems { get; set; }
        public static List<string> WorkModeItems { get; set; }
        public static List<string> ProtocolItems { get; set; }
        public static List<string> ComLinkItems { get; set; }

        public ViewDriverItem()
        {
            if (DomainItems == null)
            {
                DomainItems = new List<string>();
                DomainItems.Add("Lab");
            }
            if (ProtocolItems == null)
            {
                ProtocolItems = new List<string>();
                ProtocolItems.Add("HeaoInter");
                ProtocolItems.Add("SuperQ5");
                ProtocolItems.Add("KepSmart");
                ProtocolItems.Add("HeaoData");
                ProtocolItems.Add("LineWin");
                ProtocolItems.Add("S71200");
                ProtocolItems.Add("S7200SMART");
                ProtocolItems.Add("S71500");
            }
            if (WorkModeItems == null)
            {
                WorkModeItems = new List<string>();
                WorkModeItems.Add("Server");
                WorkModeItems.Add("Client");
            }
            if (ComLinkItems == null)
            {
                ComLinkItems = new List<string>();
                ComLinkItems.Add("TCP/IP");
                ComLinkItems.Add("SerialPort");
            }

            ComIP = "127.0.0.1";
            ComPort = 2000;
            WorkMode = "Server";
            Domain = "Lab";
            Protocol = "HeaoInter";
            ComLink = ComLinkItems[0];
            Enabled = "1";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ValidatePropList"></param>
        /// <returns></returns>
        public override CallResult Validate()
        {
            CallResult _result = new CallResult() { Success = true };
            if (ComLink == "TCP/IP")
            {
                if (Protocol == "HeaoInter")
                    _result = base.Validate(new List<string>() { "SerialComFormat" }, false);
                else
                    _result = base.Validate(new List<string>() { "SerialComFormat", "ComHeader" }, false);
            }
            else if (ComLink == "SerialPort")
                _result = base.Validate(new List<string>() { "ComIP" }, false);
            if (_result.Fail)
                return _result;
            //额外验证

            return _result;
        }

        /// <summary>
        /// 验证指定属性的关联规则
        /// </summary>
        /// <returns></returns>
        public override CallResult Validate(string PropName)
        {
            CallResult _result = base.Validate(PropName);
            if (_result.Fail)
                return _result;
            PropertyInfo pi = this.GetType().GetProperty(PropName);
            if (pi == null)
                return _result;
            object value = pi.GetValue(this, null);
            if (pi.Name == "验证属性")
            {
                //验证方法
            }
            return _result;
        }
    }
}
