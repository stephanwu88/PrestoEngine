
using Engine.Data.DBFAC;

namespace Engine.Core.TaskSchedule
{
    /// <summary>
    /// 计划任务 - 流程选项卡
    /// </summary>
    [Table(Name = "core_schedule_items", Comments = "任务项表")]
    public class Process
    {
        [Column(Name = "id", AI = true, PK = true, Comments = "索引")]
        public string id { get; set; }

        [Column(Name = "Token", Comments = "流程标识")]
        public string Token { get; set; }

        [Column(Name = "ScheduleToken", Comments = "计划标识")]
        public string ScheduleToken { get; set; }

        [Column(Name = "schedule_id", Comments = "父索引")]
        public string schedule_id { get; set; }

        [Column(Name = "task_enable", Comments = "任务使能")]
        public string task_enable { get; set; }
      
        [Column(Name = "task_type", Comments = "任务类型")]
        public string task_type { get; set; }
       
        [Column(Name = "task_detail", Comments = "任务详情")]
        public string task_detail { get; set; }

        [Column(Name = "task_state", Comments = "任务状态")]
        public string task_state { get; set; }
       
        [Column(Name = "task_pos", Comments = "任务关联工位")]
        public string task_pos { get; set; }
       
        [Column(Name = "task_id", Comments = "任务号")]
        public string task_id { get; set; }
     
        [Column(Name = "task_in", Comments = "任务进状态")]
        public string task_in { get; set; }
       
        [Column(Name = "task_out", Comments = "任务出状态")]
        public string task_out { get; set; }
      
        [Column(Name = "task_item", Comments = "任务项名称")]
        public string task_item { get; set; }
      
        [Column(Name = "task_des", Comments = "任务项描述")]
        public string task_des { get; set; }
       
        [Column(Name = "task_set_val", Comments = "任务设定值")]
        public string task_set_val { get; set; }
    
        [Column(Name = "task_order", Comments = "任务排序号")]
        public string task_order { get; set; }
       
        [Column(Name = "msg_from", Comments = "信息流源")]
        public string msg_from { get; set; }
        
        [Column(Name = "msg_to", Comments = "信息流目标")]
        public string msg_to { get; set; }
       
        [Column(Name = "msg_mode", Comments = "KillSource or KeepCopy")]
        public string msg_mode { get; set; }

        [Column(Name = "msg_tag", Comments = "Moved 信息流已传递")]
        public string msg_tag { get; set; }

        public string tag;
    }

}
