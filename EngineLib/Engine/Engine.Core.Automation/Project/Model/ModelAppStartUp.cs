using Engine.Data.DBFAC;

namespace Engine.Core
{
    /// <summary>
    /// 工程 - 启动运行
    /// </summary>
    [Table(Name = "app_startup")]
    public class ModelAppStartUp
    {
        [Column(Name = "ID", PK = true, AI = true)]
        public string ID { get; set; }

        [Column(Name = "Token", Comments = "标识码")]
        public string Token { get; set; }

        [Column(Name = "AppName", Comments = "应用程序名称")]
        public string AppName { get; set; } = SystemDefault.AppName;

        [Column(Name = "GroupName", Comments = "分组名称")]
        public string GroupName { get; set; }

        [Column(Name = "KeyName",PK = true, Comments = "键名称")]
        public string KeyName { get; set; }

        [Column(Name = "DataType", Comments = "类型名称")]
        public string DataType { get; set; }

        [Column(Name = "ConfigValue", Comments = "配置内容")]
        public string ConfigValue { get; set; }

        [Column(Name = "Remark", Comments = "描述")]
        public string Remark { get; set; }

        /// <summary>
        /// 获取列定义
        /// </summary>
        /// <param name="PropName"></param>
        /// <returns></returns>
        public static ColumnDef GetColumneDef(string PropName)
        {
            ColumnDef colDef = ColumnAttribute.Column<ModelAppStartUp>(PropName);
            if (colDef == null)
                colDef = new ColumnDef();
            return colDef;
        }
    }
}
