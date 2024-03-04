using Engine.Data.DBFAC;

namespace Engine.Core.TaskSchedule
{
    /// <summary>
    /// 计划任务 - 门控选项卡
    /// </summary>
    [Table(Name = "core_schedule_items", Comments = "任务项表")]
    public class Gate
    {
        [Column(Name = "id", AI = true, PK = true, Comments = "索引")]
        public string id { get; set; }

        [Column(Name = "Token", Comments = "门控标识")]
        public string Token { get; set; }

        [Column(Name = "ScheduleToken", Comments = "计划标识")]
        public string ScheduleToken { get; set; }

        [Column(Name = "gate_enable", Comments = "门控使能")]
        public string gate_enable { get; set; }
        
        [Column(Name = "gate_type", Comments = "门控类型")]
        public string gate_type { get; set; }
      
        [Column(Name = "gate_detail", Comments = "门控详情")]
        public string gate_detail { get; set; }
         
        [Column(Name = "gate_state", Comments = "门控状态")]
        public string gate_state { get; set; }
        
        [Column(Name = "gate_pos", Comments = "关联工位")]
        public string gate_pos { get; set; }
        
        [Column(Name = "gate_item", Comments = "关联工位参数")]
        public string gate_item { get; set; }
        
        [Column(Name = "gate_object", Comments = "目标比较值")]
        public string gate_object { get; set; }
       
        [Column(Name = "gate_val", Comments = "当前采样值")]
        public string gate_val { get; set; }
        
        [Column(Name = "gate_sign", Comments = "逻辑比较符号")]
        public string gate_sign { get; set; }
       
        [Column(Name = "gate_order", Comments = "排序号")]
        public string gate_order { get; set; }
        
        /// <summary>
        /// 自定义信息
        /// </summary>
        public string tag { get; set; }
    }
}
