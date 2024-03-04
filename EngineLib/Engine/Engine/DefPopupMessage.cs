using Engine.Common;
using System.Windows;

namespace Engine
{
    /// <summary>
    /// 弹窗结构
    /// </summary>
    public class PopupMessage
    {
        public MsgType MsgType { get; set; }
        public string Caption { get; set; }

        public MessageBoxButton ButtonType { get; set; }

        public MessageBoxImage icon { get; set; }

        public string MessageText { get; set; }

    }
}
