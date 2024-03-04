
using Engine.Data.DBFAC;

namespace Engine.Core.TaskSchedule
{
    /// <summary>
    /// 计划任务 - 常规选项卡
    /// </summary>
    [Table(Name ="core_schedule_menu",Comments = "任务主表")]
    public class NormalCard
    {
        [Column(Name = "id", AI = true, PK = true, Comments = "索引")]
        public string ID { get; set; }
       
        [Column(Name = "name", Comments = "计划任务项名称")]
        public string Name { get; set; }
      
        [Column(Name = "creator", Comments = "计划任务创建者")]
        public string Creator { get; set; }
        
        [Column(Name = "description", Comments = "计划任务详情描述")]
        public string Comment { get; set; }
    }
}
