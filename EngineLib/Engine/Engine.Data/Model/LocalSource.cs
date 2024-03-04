
namespace Engine.Data.DBFAC
{
    /// <summary>
    /// 本地数据源
    /// </summary>
    public class LocalSource
    {
        /// <summary>
        /// 连接名称
        /// </summary>
        public string SourceName { get; set; } = string.Empty;
        /// <summary>
        /// 数据源文件
        /// </summary>
        public string SourceFile { get; set; } = string.Empty;
        /// <summary>
        /// 默认:读取 1:写入 2:混合
        /// </summary>
        public string FileMode { get; set; } = string.Empty;
        /// <summary>
        /// 连接密码
        /// </summary>
        public string Password { get; set; } = string.Empty;
        /// <summary>
        /// 系统自带,用以工厂类动态创建不同类型实例
        /// </summary>
        public string ProviderName { get; set; } = "Engine.Data.MSACCESS";
        /// <summary>
        /// 驱动程序集
        /// 格式: 程序集名称 | 驱动类库 | 附属信息..
        /// ex: Engine.Data.MSSQL | Engine.Data.MSSQL.DBMSSQL
        /// </summary>
        public string Provider { get; set; } = string.Empty;
    }
}
