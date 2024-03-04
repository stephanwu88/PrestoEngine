using Engine.Common;
using Engine.Data.DBFAC;

namespace Engine.Mod
{
    [Table(Name = "def_field_group")]
    public class ModelFieldGroup : ValidateError<ModelFieldGroup>
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
}
