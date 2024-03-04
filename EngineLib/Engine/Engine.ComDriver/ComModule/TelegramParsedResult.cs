
namespace Engine.ComDriver
{
    /// <summary>
    /// 通讯电文解析结果
    /// 每次电文交互后，按规则解析，描述解析后情况
    /// </summary>
    public class TelegramParsedResult
    {
        /// <summary>
        ///指令名称
        /// </summary>
        public string CommandName { get; set; }
        /// <summary>
        ///指令码
        /// </summary>
        public string CommandCode { get; set; }
        /// <summary>
        /// 指令内容
        /// </summary>
        public string CommandText { get; set; }
        /// <summary>
        /// 指令响应错误
        /// </summary>
        public bool CommandError { get; set; }
        /// <summary>
        /// 状态码
        /// </summary>
        public string StatusCode { get; set; }
        /// <summary>
        /// 状态描述
        /// </summary>
        public string StatusText { get; set; }
        /// <summary>
        /// 状态有异常
        /// </summary>
        public bool StatusError { get; set; }
        /// <summary>
        /// 错误码
        /// </summary>
        public string ErrorCode { get; set; }
        /// <summary>
        /// 错误描述
        /// </summary>
        public string ErrorText { get; set; }
        /// <summary>
        /// 发生错误
        /// </summary>
        public bool ErrorOccurs { get; set; }
        /// <summary>
        /// 错误等级 ERROR WARN
        /// </summary>
        public string ErrorLevel { get; set; }
    }
}
