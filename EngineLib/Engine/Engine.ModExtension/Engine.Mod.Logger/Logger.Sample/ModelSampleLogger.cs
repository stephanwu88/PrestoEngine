using Engine.Data.DBFAC;

namespace Engine.Mod
{
    [Table(Name = "process_record", Comments = "样品流转记录表")]
    public class ModelSampleRecord
    {
        [Column(Name = "ID", AI = true, PK = true, Comments = "索引")]
        public string ID { get; set; }

        [Column(Name = "SampleID", Comments = "样品号")]
        public string SampleID { get; set; }

        [Column(Name = "SampleLabel", Comments = "样品标签")]
        public string SampleLabel { get; set; }

        [Column(Name = "ProcKey", Comments = "流程名称")]
        public string ProcKey { get; set; }

        [Column(Name = "PosKey", Comments = "工位名称")]
        public string PosKey { get; set; }

        [Column(Name = "PosName", Comments = "工位名称中文")]
        public string PosName { get; set; }

        [Column(Name = "InjectTime", Comments = "注册时间")]
        public string InjectTime { get; set; }

        [Column(Name = "DueTime", Comments = "注销事件")]
        public string DueTime { get; set; }

        [Column(Name = "TimeLong", Comments = "时长 单位:秒")]
        public string TimeLong { get; set; }

        [Column(Name = "Custom1", Comments = "自定义信息1")]
        public string Custom1 { get; set; }

        [Column(Name = "Custom2", Comments = "自定义信息2")]
        public string Custom2 { get; set; }

        [Column(Name = "Custom3", Comments = "自定义信息3")]
        public string Custom3 { get; set; }

        [Column(Name = "Custom4", Comments = "自定义信息4")]
        public string Custom4 { get; set; }

        [Column(Name = "Custom5", Comments = "自定义信息5")]
        public string Custom5 { get; set; }
    }
}
