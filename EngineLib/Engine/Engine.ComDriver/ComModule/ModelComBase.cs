using Engine.Common;
using Engine.Data.DBFAC;

namespace Engine.ComDriver
{
    /// <summary>
    /// 通讯变量基类模型
    /// </summary>
    [Table(Name ="driver_instr",Comments ="仪器仪表通讯")]
    public abstract class ModelComBase : ValidateError<ModelComBase>
    {
        [Column(Name = "ID", PK = true, AI = true, Comments = "索引")]
        public string ID { get; set; }

        [Column(Name = "DriverToken", Comments = "控制器ID")]
        public string DriverToken { get; set; }

        [Column(Name = "Token", Comments = "变量ID")]
        public string Token { get; set; }

        [Column(Name = "DataName", Comments = "数据名称")]
        public string DataName { get; set; }

        [Column(Name = "DataAddr", Comments = "变量地址符号")]
        public string DataAddr { get; set; }

        [Column(Name = "AddrType", Comments = "变量类型")]
        public string AddrType { get; set; }

        [Column(Name = "DataUnitType", Comments = "变量类型 Status Alarm Command")]
        public string DataUnitType { get; set; }

        [Column(Name = "DataValue", Comments = "数据值")]
        public string DataValue { get; set; }

        [Column(Name = "RelatedVariable", Comments = "关联变量")]
        public string RelatedVariable { get; set; }

        [Column(Name = "RelatedGroup", Comments = "关联工位")]
        public string RelatedGroup { get; set; }

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
    }

    /// <summary>
    /// 通讯可视模型
    /// </summary>
    [Table(Name = "driver_instr", Comments = "仪器仪表通讯")]
    public class ViewComSymbol : ModelComBase
    {

    }
}
