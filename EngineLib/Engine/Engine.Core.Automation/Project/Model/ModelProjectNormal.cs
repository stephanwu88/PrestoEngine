
namespace Engine.Core
{
    /// <summary>
    /// 工程常规设置
    /// </summary>
    public class ProjectNormal
    {
        /// <summary>
        /// 工程名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 工程标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 工程描述
        /// </summary>
        public string Comment { get; set; }
        /// <summary>
        /// 工作数据库 - 配置文件
        /// </summary>
        public string WorkDB { get; set; }
        /// <summary>
        /// 系统数据库 - 配置文件
        /// </summary>
        public string SystemDB { get; set; }
        /// <summary>
        /// 系统日志 - 配置文件
        /// </summary>
        public string WorkLog { get; set; }
    }
}
