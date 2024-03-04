using Engine.Common;
using Engine.Data.DBFAC;

namespace Engine.Mod
{
    public abstract class ModelLogger
    {
        [Column(Name = "ID", AI = true, PK = true, Comments = "索引")]
        public string ID { get; set; }

        [Column(Name = "MsgType", Comments = "消息类型  消息  命令  报警")]
        public string MsgType { get; set; }

        [Column(Name = "MsgDir", Comments = "任务方向  发送 - 接收")]
        public string MsgDir { get; set; }

        [Column(Name = "MsgObject", Comments = "消息对象")]
        public string MsgObject { get; set; }

        [Column(Name = "MsgText", Comments = "消息内容")]
        public string MsgText { get; set; }

        [Column(Name = "RecTime", Comments = "记录时间")]
        public string RecTime { get; set; }
    }

    [Table(Name = "process_log", Comments = "任务流程记录表")]
    public class ModelTaskLogger : ModelLogger
    {
        [AutoIndex()]
        public string IDX { get; set; }

        [Column(Name = "SampleID", Comments = "样品编码")]
        public string SampleID { get; set; }

        [Column(Name = "SampleLabel", Comments = "样品标签")]
        public string SampleLabel { get; set; }

        [Column(Name = "Custom1", Comments = "自定义1")]
        public string Custom1 { get; set; }

        [Column(Name = "Custom2", Comments = "自定义2")]
        public string Custom2 { get; set; }
    }
}