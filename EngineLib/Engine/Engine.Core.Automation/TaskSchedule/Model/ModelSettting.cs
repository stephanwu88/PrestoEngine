
using Engine.Data.DBFAC;

namespace Engine.Core.TaskSchedule
{
    /// <summary>
    /// 计划任务 - 设置选项卡
    /// </summary>
    [Table(Name = "core_schedule_items", Comments = "任务项表")]
    public class ParamSetting
    {
        [Column(Name = "id", AI = true, PK = true, Comments = "索引")]
        public string id { get; set; }

        [Column(Name = "timeout_min",Comments = "超时时间 携带单位")]
        public string TimeOut { get; set; }
    }
}
