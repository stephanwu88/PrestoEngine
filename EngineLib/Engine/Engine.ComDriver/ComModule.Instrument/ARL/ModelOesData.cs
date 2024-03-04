using Engine.Data;
using Engine.Data.DBFAC;

namespace Engine.ComDriver.ARL
{
    /// <summary>
    /// 分析文件结果
    /// </summary>
    [Table(Name = "data_result_spectrum", Comments = "ARL光谱仪数据结果表", Token = "202302211808")]
    public class ModelOESData : EntityEF
    {
        [Column(Name = "ID", PK = true, AI = true, Comments = "索引")]
        public string ID { get; set; }

        [DictField(Name = "SampleID", Comments = "样品便编码")]
        public string SampleID { get; set; }

        [DictField(Name = "SteelType", Comments = "钢种")]
        public string SteelType { get; set; }

        [DictField(Name = "AnaEndTime", Comments = "分析数据时间")]
        public string AnaEndTime { get; set; }

        [DictField(Name = "RecTime", Comments = "记录时间")]
        public string RecTime { get; set; } = SystemDefault.StringTimeNow;

        public ModelOESData(string tableName = "")
        {
            TableName = tableName;
        }
    }
}
