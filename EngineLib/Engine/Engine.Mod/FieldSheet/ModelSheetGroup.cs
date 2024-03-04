using Engine.Common;
using Engine.Data.DBFAC;
using System.ComponentModel;

namespace Engine.Mod
{
    /// <summary>
    /// 系统分组类型枚举
    /// </summary>
    public enum SystemGroupType
    {
        [Description("表格组")]
        DataSheet,
        [Description("变量组")]
        Symbol,
        [Description("配方组")]
        Receipe,
        [Description("报警组")]
        Alarm,
        [Description("报表组")]
        Report,
        [Description("字段组")]
        Field,
    }

    /// <summary>
    /// 表格组 - 主信息
    /// </summary>
    [Table(Name = "sys_sheet_group", Comments = "表格组管理表")]
    public class ModelSheetGroup : ValidateError<ModelSheetGroup>
    {
        [Column(Name = "ID", PK = true, AI = true, Comments = "索引")]
        public string ID { get; set; }

        [Column(Name = "Token", Comments = "变量组ID")]
        public string Token { get; set; }

        [Column(Name = "Name", Comments = "变量组名称")]
        [StringRule(MinLen = 1, MaxLen = 50, ErrorMessage = "请输入正确的变量组名称")]
        public string Name { get; set; }

        [Column(Name = "Comment", Comments = "变量组描述")]
        [StringRule(MaxLen = 100, ErrorMessage = "变量组备注超限")]
        public string Comment { get; set; }

        [Column(Name = "MarkKey", Comments = "标记")]
        public string MarkKey { get; set; }

        [Column(Name = "OrderID", Comments = "排序号")]
        public string OrderID { get; set; }
    }

    /// <summary>
    /// 表格组 - 视图
    /// </summary>
    public class ViewSheetGroup : ModelSheetGroup
    {

    }
}
