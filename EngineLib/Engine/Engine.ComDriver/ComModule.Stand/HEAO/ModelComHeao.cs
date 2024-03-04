using Engine.Common;
using Engine.Data.DBFAC;
using System.Collections.Generic;
using System.Reflection;

namespace Engine.ComDriver.HEAO
{
    [Table(Name = "driver_heaointer", Comments = "和澳协议表")]
    public class ModelComHeao : ValidateError<ModelComHeao>
    {
        [Column(Name = "ID", AI = true, PK =true, Comments = "索引")]
        public string ID { get; set; }

        [Column(Name = "DriverToken", Comments = "控制器ID")]
        public string DriverToken { get; set; }

        [Column(Name = "Token", Comments = "ID")]
        public string Token { get; set; }

        [Column(Name = "ProtocolFC", Comments = "协议指令码")]
        public string ProtocolFC { get; set; }

        [Column(Name = "DataName", Comments = "显示名称")]
        public string DataName { get; set; }

        [Column(Name = "DataIndex", Comments = "数据字节地址索引")]
        public string DataIndex { get; set; }

        [Column(Name = "DataBitOffset", Comments = "数据位索引")]
        public string DataBitOffset { get; set; }

        [Column(Name = "DataLen", Comments = "数据长度")]
        [StringRule(MinLen = 1, MaxLen = 25, ErrorMessage = "未指定数据长度")]
        public string DataLen { get; set; }

        [Column(Name = "AddrType", Comments = "地址类型")]
        public string AddrType { get; set; }

        [Column(Name = "DataUnitType", Comments = "数据类型定义")]
        public string DataUnitType { get; set; }

        [Column(Name = "RelatedVariable", Comments = "关联变量")]
        [StringRule(MaxLen = 120, ErrorMessage = "关联变量长度超限")]
        public string RelatedVariable { get; set; }

        [Column(Name = "RelatedGroup", Comments = "关联分组")]
        public string RelatedGroup { get; set; }
        
        [Column(Name = "SymbolID", Comments = "关联变量ID")]
        public string SymbolID { get; set; }

        [Column(Name = "DataValue", Comments = "数据值")]
        public string DataValue { get; set; }

        [Column(Name = "AwaitName", Comments = "命令名称")]
        public string AwaitName { get; set; }

        [Column(Name = "AwaitState", Comments = "命令状态")]
        public string AwaitState { get; set; }

        [Column(Name = "AwaitTime", Comments = "命令时间")]
        public string AwaitTime { get; set; }

        [Column(Name = "Comment", Comments = "描述")]
        public string Comment { get; set; }

        [Column(Name = "OrderID", Comments = "排序号")]
        public string OrderID { get; set; }

        [Column(Name = "DataAddr", Comments = "通讯变量")]
        public virtual string DataAddr { get; set; }
    }

    public class ViewComHeao : ModelComHeao  
    {
        public ViewComHeao()
        {
            if (ProtocolFCItems == null)
            {
                ProtocolFCItems = new List<string>();
                ProtocolFCItems.Add("Status(03H)");
                ProtocolFCItems.Add("Alarm(04H)");
                ProtocolFCItems.Add("Cmd(05H)");
                ProtocolFCItems.Add("Config(06H)");
                ProtocolFCItems.Add("Path(07H)");
            }

            if (AddrTypeItems == null)
            {
                AddrTypeItems = new List<string>();
                AddrTypeItems.Add("Bool");
                AddrTypeItems.Add("byte[ ]");
            }

            if (DataUnitTypeItems == null)
            {
                DataUnitTypeItems = new List<string>();
                DataUnitTypeItems.Add("Status");
                DataUnitTypeItems.Add("Alarm");
                DataUnitTypeItems.Add("Command");
                DataUnitTypeItems.Add("Data");
                DataUnitTypeItems.Add("Temp");
            }

            DataAddr = "DU1.0";
            ProtocolFC = "Status(03H)";
            AddrType = "bit";
            DataUnitType = "Status";
        }

        private string _DataAddr;
        [Column(Name = "DataAddr", Comments = "通讯变量")]
        public override string DataAddr
        {
            get
            {
                return _DataAddr;                
            }
            set
            {
                _DataAddr = value;
                int index = _DataAddr.MidString("DU", ".").ToMyInt(1, 25);
                int offset = _DataAddr.MidString(".", "").ToMyInt();
                DataIndex = index.ToString();
                DataBitOffset = offset.ToString();
                _DataAddr = string.Format("DU{0}.{1}", DataIndex, DataBitOffset);
                RaisePropertyChanged("DataAddr");
            }
        }

        public static List<string> ProtocolFCItems { get; set; }
        public static List<string> AddrTypeItems { get; set; }
        public static List<string> DataUnitTypeItems { get; set; }

        /// <summary>
        /// 验证所有属性的规则
        /// </summary>
        /// <returns></returns>
        public override CallResult Validate()
        {
            CallResult _result = base.Validate();
            if (_result.Fail)
                return _result;
            //额外验证 - DataArr
            if (!DataAddr.StartSubString(2).Equals("DU"))
            {
                _result.Success = false;
                _result.Result = "数据地址格式错误\r\n";
                _result.Result += "格式：DUxx.x";
                _Error = _result.Result.ToMyString();
                return _result;
            }
            int DataIndex = DataAddr.MidString("DU", ".").ToMyInt();
            int DataBitOffset = DataAddr.MidString(".", "").ToMyInt();
            if (DataIndex <= 0 || DataIndex > 25)
            {
                _result.Success = false;
                _result.Result = "协议数据DU1-25";
            }
            else
            {
                if (AddrType == "bit" && DataBitOffset < 0 || DataBitOffset > 7)
                {
                    _result.Success = false;
                    _result.Result = "协议数据偏移0-7";
                }
                else if (AddrType == "byte[ ]" && DataAddr.Contains("."))
                {
                    _result.Success = false;
                    _result.Result = "数据地址含非法字符";
                }
            }
            if (_result.Fail)
                _Error = _result.Result.ToMyString();

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
