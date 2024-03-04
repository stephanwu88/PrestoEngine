
using Engine.Data.DBFAC;

namespace Engine.Core.TaskSchedule
{
    [Table(Name = "core_schedule_menu", Comments = "计划任务 - 摘要列表")]
    public class ScheduleContent
    {
        [Column(Name = "id", AI = true, PK = true, Comments = "索引")]
        public string ID { get; set; }

        [Column(Name = "Token", Comments = "计划标识")]
        public string Token { get; set; }

        [Column(Name = "enable", Comments = "使能状态")]
        public string Enable { get; set; }
       
        [Column(Name = "name", Comments = "任务名称")]
        public string Name { get; set; }
        
        [Column(Name = "state", Comments = "任务状态")]
        public string State { get; set; }
        
        [Column(Name = "trig", Comments = "触发器")]
        public string Trig { get; set; }
        
        [Column(Name = "active_time", Comments = "激活时间")]
        public string ActiveTime { get; set; }
        
        [Column(Name = "exe_time", Comments = "执行时间")]
        public string ExcuteTime { get; set; }
      
        [Column(Name = "exe_result", Comments = "执行结果")]
        public string ExcuteResult { get; set; }
        
        [Column(Name = "creator", Comments = "任务创建者")]
        public string Creator { get; set; }
        
        [Column(Name = "create_time", Comments = "任务创建时间")]
        public string CreateTime { get; set; }

        [Column(Name = "timeout_min", Comments = "任务超时时间")]
        public string TimeOut { get; set; }
        
        [Column(Name = "msg_stream", Comments = "任务信息")]
        public string MsgStream { get; set; }
        
        [Column(Name = "msg_visual", Comments = "显示信息")]
        public string MsgVisual { get; set; }
       
        [Column(Name = "description", Comments = "任务描述")]
        public string Comment { get; set; }
    }
}
