
namespace Engine.Mod
{
    /// <summary>
    /// 日志管理器
    /// </summary>
    public class Logger
    {
        /// <summary>
        /// 私有化
        /// </summary>
        private Logger() { }

        /// <summary>
        /// 文件日志 - 调试跟踪日志
        /// </summary>
        public static FileLogger Default => FileLogger.Default;

        /// <summary>
        /// 文件日志 - 通讯控制器
        /// </summary>
        public static FileLogger CommBody => FileLogger.CommBody;

        /// <summary>
        /// 文件日志 - 任务管理层
        /// </summary>
        public static FileLogger Task => FileLogger.Task;

        /// <summary>
        /// 文件日志 - 错误日志
        /// </summary>
        public static FileLogger Error => FileLogger.Error;

        /// <summary>
        /// 文件日志 - 数据日志
        /// </summary>
        public static FileLogger Data => FileLogger.Data;

        /// <summary>
        /// 文件日志 - 数据库操作日志
        /// </summary>
        public static FileLogger Database => FileLogger.Database;
    }
}
