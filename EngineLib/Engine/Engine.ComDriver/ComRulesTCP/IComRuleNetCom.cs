
namespace Engine.ComDriver
{
    /// <summary>
    /// 网络通讯接口规约
    /// </summary>
    /// <typeparam name="TBufData"></typeparam>
    public interface IComRuleNetCom<TBufData> : IComRule<TBufData, sSocket, NetworkCommParam>
    {
        /// <summary>
        /// 通讯状态
        /// </summary>
        SocketState ComState { get; }
        /// <summary>
        /// 工作模式
        /// </summary>
        SocketMode ComWorkMode { get; }
    }
}
