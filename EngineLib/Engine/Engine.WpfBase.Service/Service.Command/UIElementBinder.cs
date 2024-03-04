using System;
using System.Windows;
using System.Windows.Input;

namespace Engine.WpfBase
{
    public static class UIElementBinderExtention
    {
        #region BindCommand

        /// <summary>
        /// 绑定命令和命令事件到宿主UI
        /// </summary>
        public static void BindCommand(this UIElement @ui, ICommand com, Action<object, ExecutedRoutedEventArgs> call)
        {
            var bind = new CommandBinding(com);
            bind.Executed += new ExecutedRoutedEventHandler(call);
            @ui.CommandBindings.Add(bind);
        }

        /// <summary>
        /// 绑定RelayCommand命令到宿主UI
        /// </summary>
        public static void BindCommand(this UIElement @ui, ICommand com)
        {
            var bind = new CommandBinding(com);
            @ui.CommandBindings.Add(bind);
        }

        #endregion
   
        public static void Visible(this UIElement @ui)
        {
            @ui.Visibility=Visibility.Visible;
        }

        public static void VisibilityWith(this UIElement @ui,bool from)
        {
            if(from)
            {
                @ui.Visible();
            }else
            {
                ui.Collapsed();
            }
        }

        public static void Hidden(this UIElement @ui)
        {
            @ui.Visibility = Visibility.Hidden;
        }

        public static void Collapsed(this UIElement @ui)
        {
            @ui.Visibility = Visibility.Collapsed;
        }
    }
}
