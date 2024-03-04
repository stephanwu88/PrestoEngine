using Engine.Data.DBFAC;

namespace Engine.MVVM
{
    [Table(Name = "app_table_invoke", ViewName = "table_invoke_view", Comments = "数据库表回调")]
    public class ModelTableInvoke
    {
        [Column(Name = "ID", AI = true, PK = true, Comments = "索引")]
        public string ID { get; set; }

        [Column(Name = "InvokeKey", Comments = "回调主键")]
        public string InvokeKey { get; set; }

        [Column(Name = "Schema", Comments = "数据库名称")]
        public string Schema { get; set; }

        [Column(Name = "TableName", Comments = "表名称")]
        public string TableName { get; set; }

        [Column(Name = "TableRows", Comments = "表行数")]
        public string TableRows { get; set; }

        [Column(Name = "TableRowsLimit", Comments = "表行限制数量")]
        public string TableRowsLimit { get; set; }

        [Column(Name = "CreateTime", Comments = "表创建时间")]
        public string CreateTime { get; set; }

        [Column(Name = "UpdateTime", Comments = "表更新时间")]
        public string UpdateTime { get; set; }

        [Column(Name = "ChangedToken", Comments = "")]
        public string ChangedToken { get; set; }

        [Column(Name = "Comment", Comments = "表说明")]
        public string Comment { get; set; }
    }
}
