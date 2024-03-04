
using Engine.Data.DBFAC;

namespace Engine.Core.TaskSchedule
{
    /// <summary>
    /// 任务项
    /// </summary>
    [Table(Name = "core_schedule_items", Comments = "任务项表")]
    public class TaskItem : Process
    {
        [Column(Name = "msg_visual", Comments = "信息流可视")]
        public string msg_visual { get; set; }

        [Column(Name = "msg_stream", Comments = "信息流全部")]
        public string msg_stream { get; set; }
    }
}
