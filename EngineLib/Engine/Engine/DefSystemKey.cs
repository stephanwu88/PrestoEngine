namespace Engine
{
    /// <summary>
    /// 数据库连接Key
    /// </summary>
    public partial class DbConKey
    {
        /// <summary>
        /// 本地化配置连接
        /// </summary>
        public const string SYS = "nd_sys";
        /// <summary>
        /// 流程任务处理连接
        /// </summary>
        public const string Task = "nd_work";
        /// <summary>
        /// 本地工艺数据连接
        /// </summary>
        public const string Data = "nd_data";
        /// <summary>
        /// 上位机信息化委托连接
        /// </summary>
        public const string LimsDelegate = "nd_lims_delegate";
        /// <summary>
        /// 上位机信息化数据连接
        /// </summary>
        public const string LimsData = "nd_lims_data";
    }

    /// <summary>
    /// 系统主键
    /// </summary>
    public partial class LogPathKey
    {
        /// <summary>
        /// 
        /// </summary>
        public const string Default = "Trace";
        /// <summary>
        /// 
        /// </summary>
        public const string CommBody = "CommBody";
        /// <summary>
        /// 
        /// </summary>
        public const string Task = "Task";
        /// <summary>
        /// 
        /// </summary>
        public const string Error = "Error";
        /// <summary>
        /// 
        /// </summary>
        public const string Data = "Data";
        /// <summary>
        /// 
        /// </summary>
        public const string Database = "Database";
    }
}
