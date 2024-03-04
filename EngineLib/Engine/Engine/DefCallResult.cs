
namespace Engine
{
    /// <summary>
    /// 调用返回结果
    /// </summary>
    public class CallResult
    {
        private bool _Success = false;
        private object _Result = string.Empty;
        /// <summary>
        /// 结果成功
        /// </summary>
        public bool Success { get => _Success; set { _Success = value; } }
        /// <summary>
        /// 结果失败
        /// </summary>
        public bool Fail { get => !_Success; set { _Success = !value; } }
        /// <summary>
        /// 结果内容
        /// </summary>
        public object Result { get => _Result; set { _Result = value; } }
    }
}
