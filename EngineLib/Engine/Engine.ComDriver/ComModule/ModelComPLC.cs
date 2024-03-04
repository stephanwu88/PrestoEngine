using Engine.Common;
using Engine.Data.DBFAC;

namespace Engine.ComDriver
{
    [Table(Name = "driver_plc", Comments = "PLC协议表")]
    public class ModelComPLC : ValidateError<ModelComPLC>
    {
        [Column(Name = "ID", PK = true, AI = true, Comments = "索引")]
        public string ID { get; set; }

        [Column(Name = "DriverToken", Comments = "控制器ID")]
        public string DriverToken { get; set; }

        [Column(Name = "Token", Comments = "变量ID")]
        public string Token { get; set; }

        [Column(Name = "DataGroup", Comments = "数据分组")]
        public string DataGroup { get; set; }

        [Column(Name = "DataName", Comments = "数据名称")]
        public string DataName { get; set; }

        [Column(Name = "DataAddr", Comments = "变量地址符号")]
        [StringRule(MinLen = 3, ErrorMessage = "请输入正确变量地址")]
        public string DataAddr { get; set; }

        [Column(Name = "AddrType", Comments = "变量类型")]
        public string AddrType { get; set; }

        [Column(Name = "DataAccess", Comments = "访问权限")]
        public string DataAccess { get; set; }

        [Column(Name = "DataUnitType", Comments = "数据类型")]
        public string DataUnitType { get; set; }

        [Column(Name = "RelatedVariable", Comments = "关联变量")]
        public string RelatedVariable { get; set; }

        [Column(Name = "RelatedGroup", Comments = "关联工位")]
        public string RelatedGroup { get; set; }

        [Column(Name = "SymbolToken", Comments = "关联符号ID")]
        public string SymbolToken { get; set; }

        [Column(Name = "DataValue", Comments = "数据值")]
        public string DataValue { get; set; }

        [Column(Name = "DataWrite", Comments = "写入值")]
        public string DataWrite { get; set; }

        [Column(Name = "StartByteOfRange", Comments = "分区起始地址")]
        public string StartByteOfRange { get; set; }

        [Column(Name = "AwaitState", Comments = "命令状态")]
        public string AwaitState { get; set; }

        [Column(Name = "AwaitTime", Comments = "命令时间")]
        public string AwaitTime { get; set; }

        [Column(Name = "Comment", Comments = "说明")]
        public string Comment { get; set; }

        [Column(Name = "OrderID", Comments = "排序号")]
        public string OrderID { get; set; }
    }
}
