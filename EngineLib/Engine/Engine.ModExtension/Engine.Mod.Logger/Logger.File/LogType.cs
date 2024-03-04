using System.ComponentModel;

namespace Engine.Mod
{
    /// <summary>
    /// 日志类型
    /// </summary>
    public enum LOG_TYPE
    {
        [Description("DEBUG")]
        DEBUG = 1,
        [Description("MESS")]
        MESS = 2,
        [Description("WARN")]
        WARN = 3,
        [Description("ERROR")]
        ERROR = 4
    }
}
