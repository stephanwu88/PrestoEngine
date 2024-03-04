
using Engine.Data.DBFAC;

namespace Engine.Core.TaskSchedule
{
    /// <summary>
    /// 计划任务 - 触发器选项卡
    /// </summary>
    [Table(Name ="core_schedule_items",Comments ="任务项表")]
    public class Trigger
    {
        [Column(Name = "id", AI = true, PK = true, Comments = "索引")]
        public string id { get; set; }

        [Column(Name = "Token", Comments = "触发器标识")]
        public string Token { get; set; }

        [Column(Name = "ScheduleToken", Comments = "计划标识")]
        public string ScheduleToken { get; set; }

        [Column(Name = "trig_enable", Comments = "触发器使能")]
        public string trig_enable { get; set; }

        [Column(Name = "trig_type", Comments = "触发器类型")]
        public string trig_type { get; set; }

        [Column(Name = "trig_detail", Comments = "触发器详情")]
        public string trig_detail { get; set; }

        [Column(Name = "trig_state", Comments = "触发器状态")]
        public string trig_state { get; set; }

        [Column(Name = "trig_pos", Comments = "触发器关联工位")]
        public string trig_pos { get; set; }

        [Column(Name = "trig_item", Comments = "触发器关联工位项目")]
        public string trig_item { get; set; }

        [Column(Name = "trig_sign", Comments = "触发器逻辑比较符")]
        public string trig_sign { get; set; }

        [Column(Name = "trig_object", Comments = "触发器逻辑比较目标值")]
        public string trig_object { get; set; }

        [Column(Name = "trig_val", Comments = "触发器逻辑比较采样值")]
        public string trig_val { get; set; }

        [Column(Name = "trig_order", Comments = "触发器排序号")]
        public string trig_order { get; set; }

        /// <summary>
        /// 自定义标记
        /// </summary>
        public string tag { get; set; }
    }
}
