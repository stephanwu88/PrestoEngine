using Engine.Common;
using Engine.Data.DBFAC;

namespace Engine.Mod
{
    [Table(Name = "sys_alarm")]
    public class ModelAlarmItem:ValidateError<ModelAlarmItem>
    {
        [Column(Name = "ID", PK = true, AI = true, Comments = "索引")]
        public string ID { get; set; }

        [Column(Name = "GroupToken", Comments = "报警区ID")]
        public string GroupToken { get; set; }

        [Column(Name = "Token", Comments = "报警ID")]
        public string Token { get; set; }

        [Column(Name = "Name", Comments = "报警名称")]
        public string Name { get; set; }

        [Column(Name = "Config", Comments = "配置信息")]
        public string Config { get; set; }

        [Column(Name = "AlarmLevel", Comments = "报警级别")]
        public string AlarmLevel { get; set; }

        [Column(Name = "GroupName", Comments = "报警区名称")]
        public string GroupName { get; set; }

        [Column(Name = "Comment", Comments = "说明")]
        public string Comment { get; set; }

        [Column(Name = "SymbolToken", Comments = "数据库变量ID")]
        public string SymbolToken { get; set; }

        [Column(Name = "SymbolName", Comments = "数据库变量名称")]
        public string SymbolName { get; set; }
    }

    public class ViewAlarmItem : ModelAlarmItem
    {

    }
}
