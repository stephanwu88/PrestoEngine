using Engine.WpfBase;
using System.Windows;

namespace Engine.Interactivity
{
    /******************************************************************
     * 引用方式
     * 添加命名空间: 
     * xmlns:i="http://schemas.microsoft.com/expression/2010/interactivity"
     * xmlns:behavior="clr-namespace:Engine.Interactivity;assembly=Engine"
     * <i:Interaction.Behaviors>
     *   <behavior:WinAtWorkArea></behavior:WinAtWorkArea>
     * </i:Interaction.Behaviors>
     *******************************************************************/
    /// <summary>
    /// 窗口变化时底部不覆盖任务栏
    /// </summary>
    public class WinAtWorkArea : Behavior<Window>
    {
        #region Properties

        public Window AttachedWindow { get; set; }

        #endregion Properties

        #region Methods

        protected override void OnAttached()
        {
            base.OnAttached();
            AttachedWindow = AssociatedObject as Window;

            if (AttachedWindow == null)
            {
                return;
            }

            RegisterEvents();
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            UnRegisterEvents();
        }

        /// <summary>
        ///  注册事件
        /// </summary>
        private void RegisterEvents()
        {
            if (AttachedWindow != null)
            {
                AttachedWindow.StateChanged += AttachedWindow_StateChanged; 
            }
        }

     

        /// <summary>
        /// 注销事件
        /// </summary>
        private void UnRegisterEvents()
        {
            if (AttachedWindow != null)
            {
                AttachedWindow.StateChanged -= AttachedWindow_StateChanged;
            }
        }

        #endregion Methods

        #region EventHandlers
        private void AttachedWindow_StateChanged(object sender, System.EventArgs e)
        {
            Window win = sender as Window;
            if (win.WindowState == WindowState.Maximized)
            {
                win.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
                win.MaxWidth = SystemParameters.MaximizedPrimaryScreenWidth;
            }
        }
        #endregion EventHandlers
    }
}
