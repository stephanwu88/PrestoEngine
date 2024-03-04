namespace Engine.Mod
{
    /// <summary>
    /// 日志配置项
    /// </summary>
    public class LogItem
    {
        /// <summary>
        /// 日志名称
        /// </summary>
        public string LogType { get; set; }
        /// <summary>
        /// 日志文件路径
        /// </summary>
        public string FilePath { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Comment { get; set; }
    }
}
