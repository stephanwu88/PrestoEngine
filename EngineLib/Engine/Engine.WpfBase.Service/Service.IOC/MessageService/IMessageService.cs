using System;
using System.Threading.Tasks;
using System.Windows;

namespace Engine.WpfBase
{
    /// <summary>
    /// 消息服务接口
    /// </summary>
    public interface IMessageService
    {
        /// <summary>
        /// 默认模糊效果
        /// </summary>
        /// <param name="isuse"></param>
        void BeginDefaultBlurEffect(bool isuse);
        bool IsOpened();
        Task<T> ShowDialog<T>(UIElement element, Action<object, RoutedEventArgs> action = null);
        Task ShowPercentProgress(Action<IPercentProgress> action, Action closeAction = null);
        Task<T> ShowPercentProgress<T>(Func<IPercentProgress, T> action, Action closeAction = null);
        Task<R> ShowProgress<T, R>(Func<T, R> action, Action closeAction = null) where T : new();
        Task<bool> ShowResult(string message);
        Task ShowResult(string message, Action<object, RoutedEventArgs> action);
        Task ShowStringProgress(Action<IStringProgress> action, Action closeAction = null);
        Task<T> ShowStringProgress<T>(Func<IStringProgress, T> action, Action closeAction = null);
        Task ShowSumit(string message, bool isLog = true);
        Task ShowWaitter(Action action, Action closeAction = null);
        Task<T> ShowWaitter<T>(Func<T> action);
    }

    public class Messager : ServiceInstance<IMessageService>
    {

    }

    public interface IPercentProgress
    {
        int Value { set; }
    }

    public interface IStringProgress
    {
        string MessageStr { set; }
    }
}
