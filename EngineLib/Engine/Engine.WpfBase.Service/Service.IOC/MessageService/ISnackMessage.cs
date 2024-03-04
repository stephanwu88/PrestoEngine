using System;

namespace Engine.WpfBase
{
    public interface ISnackMessage
    {
        void Show(object message);
        void Show(object message, object actionContent, Action actionHandler);
        void Show<TArgument>(string message, object actionContent, Action<TArgument> actionHandler, TArgument actionArgument);
        void ShowTime(object message);
    }
}
