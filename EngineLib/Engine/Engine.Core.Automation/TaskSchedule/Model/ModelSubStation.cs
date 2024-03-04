using Engine.Data;
using Engine.Data.DBFAC;

namespace Engine.Core.TaskSchedule
{
    /// <summary>
    /// 子工位数据模型
    /// </summary>
    [Table(Name = "location_group", Comments = "子工位")]
    public class ModelSubPos : NotifyObject
    {
        [Column(Name = "ID", PK = true, AI = true, Comments = "索引")]
        public string ID { get; set; }

        [Column(Name = "Token", Comments = "工位标识")]
        public string Token { get; set; }

        [Column(Name = "ItemType", Comments = "项类型  Static: 静态  Dynamic: 动态")]
        public string ItemType { get; set; }

        [Column(Name = "GroupKey", Comments = "工位标识")]
        public string GroupKey { get; set; }

        [Column(Name = "PosType", Comments = "工位类型 CounterAndTimer:计数时间型 CounterOrTimer:计数或时间型 Timer:定时型  Logical:逻辑型")]
        public string PosType { get; set; }

        [Column(Name = "PosID", Comments = "工位号")]
        public string PosID { get; set; }

        [Column(Name = "PosKey", Comments = "工位编码")]
        public virtual string PosKey { get; set; }

        [Column(Name = "MarkKey", Comments = "工位标记")]
        public string MarkKey { get; set; }

        [Column(Name = "AllowedCount", Comments = "允许存储数量")]
        public string AllowedCount { get; set; }

        [Column(Name = "SavedCount", Comments = "已存储数量")]
        public string SavedCount { get; set; }

        [Column(Name = "PosSensor", Comments = "工位检测")]
        public string PosSensor { get; set; }

        [Column(Name = "PosState", Comments = "工位状态")]
        public string PosState { get; set; }

        [Column(Name = "ProcKey", Comments = "流程关键")]
        public string ProcKey { get; set; }

        [Column(Name = "SampleLabel", Comments = "样品任务标签")]
        public string SampleLabel { get; set; }

        [Column(Name = "SampleID", Comments = "样品编码")]
        public string SampleID{ get; set; }

        [Column(Name = "SampleName", Comments = "样品名称")]
        public string SampleName { get; set; }

        [Column(Name = "SampleType", Comments = "样品类型")]
        public string SampleType { get; set; }

        [Column(Name = "SampleMsg", Comments = "样品信息")]
        public string SampleMsg { get; set; }

        [Column(Name = "InjectTime", Comments = "注入时间")]
        public string InjectTime { get; set; }

        [Column(Name = "DueTime", Comments = "到期时间")]
        public string DueTime { get; set; }

        [Column(Name = "Custom1", Comments = "自定义1")]
        public string Custom1 { get; set; }

        [Column(Name = "Custom2", Comments = "自定义2")]
        public string Custom2 { get; set; }

        [Column(Name = "Custom3", Comments = "自定义3")]
        public string Custom3 { get; set; }
    }
}
