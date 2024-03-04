
namespace Engine.Data.DBFAC
{
    /// <summary>
    /// 服务器节点信息
    /// </summary>
    public class ServerNode
    {
        /// <summary>
        /// 服务器IP
        /// </summary>
        public string ServerIP { get; set; } = string.Empty;
        /// <summary>
        /// 服务器名称
        /// </summary>
        public string ServerName { get; set; } = string.Empty;
        /// <summary>
        /// 服务器端口
        /// </summary>
        public int ServerPort { get; set; }
        /// <summary>
        /// 数据库名称
        /// </summary>
        public string Database { get; set; } = string.Empty;
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserID { get; set; } = string.Empty;
        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; } = "jMdOFpYyzmp047UKo7i3iw==";
        /// <summary>
        /// 系统自带,用以工厂类动态创建不同类型实例
        /// </summary>
        public string ProviderName { get; set; } = "Engine.Data.MySQL";
        /// <summary>
        /// 驱动程序集
        /// 格式: 程序集名称 | 驱动类库 | 附属信息..
        /// ex: Engine.Data.MSSQL | Engine.Data.MSSQL.DBMSSQL
        /// </summary>
        public string Provider { get; set; } = string.Empty;
    }
}
