using Engine.Common;
using Engine.Data.DBFAC;

namespace Engine.Core
{
    /// <summary>
    /// 系统功能分组
    /// </summary>
    [Table(Name = "sys_group", Comments = "功能组管理表")]
    public class ModelSystemGroup : ValidateError<ModelSystemGroup>
    {
        [Column(Name = "ID", PK = true, AI = true, Comments = "索引")]
        public string ID { get; set; }

        [Column(Name = "Token", Comments = "变量组ID")]
        public string Token { get; set; }

        [Column(Name = "WorkDomain", Comments = "变量组工作域")]
        public string WorkDomain { get; set; }

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
    /// 系统符号组 - 视图
    /// </summary>
    public class ViewSystemGroup : ModelSystemGroup
    {

    }
}
