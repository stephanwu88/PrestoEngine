
using Engine.Data.DBFAC;

namespace Engine.Core
{
    /// <summary>
    /// 视图数据模型
    /// </summary>
    public class ModelDataView
    {
        [Column(Name = "Token", Comments = "")]
        public string Token { get; set; }

        [Column(Name = "DataName", Comments = "通讯数据项名称Key")]
        public string DataName { get; set; }

        [Column(Name = "DataAddr",Comments = "通讯变量地址")]
        public string DataAddr { get; set; }
     
        [Column(Name = "AddrType", Comments = "通讯变量类型 通讯变量表中的具体数据类型 ex:bit byte[] string")]
        public string AddrType { get; set; }
       
        [Column(Name = "DataUnitType", Comments = "数据定义类型 Status Alarm Command Data Temp")]
        public string DataUnitType { get; set; }
       
        [Column(Name = "DataValue", Comments = "数据值")]
        public string DataValue { get; set; }

        [Column(Name = "Comment", Comments = "说明")]
        public string Comment { get; set; }

    }
}
