using Engine.Common;
using System.Windows;
using System.Windows.Input;
using Engine.WpfBase;

namespace Engine.Interactivity
{
    /******************************************************************
     * 引用方式
     * 添加命名空间: 
     * xmlns:i="http://schemas.microsoft.com/expression/2010/interactivity"
     * xmlns:behavior="clr-namespace:Engine.Interactivity;assembly=Engine"
     * <i:Interaction.Behaviors>
     *   <behavior:WinDragMoveBehavior></behavior:WinDragMoveBehavior>
     * </i:Interaction.Behaviors>
     *******************************************************************/
    /// <summary>
    /// 拖拽窗口
    /// </summary>
    public class WinDragMoveBehavior: Behavior<Window>
    {
        #region Fields

        private Point _downPoint;

        #endregion Fields

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
                AttachedWindow.PreviewMouseLeftButtonDown += AttachedGrid_PreviewMouseLeftButtonDown;
                AttachedWindow.PreviewMouseMove += AttachedGridOnPreviewMouseMove;
                AttachedWindow.PreviewMouseLeftButtonUp += AttachedGridOnPreviewMouseLeftButtonUp;
            }
        }

        /// <summary>
        /// 注销事件
        /// </summary>
        private void UnRegisterEvents()
        {
            if (AttachedWindow != null)
            {
                AttachedWindow.MouseLeftButtonDown -= AttachedGrid_PreviewMouseLeftButtonDown;
                AttachedWindow.MouseMove -= AttachedGridOnPreviewMouseMove;
                AttachedWindow.MouseLeftButtonUp -= AttachedGridOnPreviewMouseLeftButtonUp;
            }
        }

        #endregion Methods

        #region EventHandlers

        private void AttachedGrid_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _downPoint = e.GetPosition(AttachedWindow);
        }

        private void AttachedGridOnPreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var mouseMovePoint = e.GetPosition(AttachedWindow);

                if (sCommon.IsAbleToDrag(mouseMovePoint, _downPoint))
                {
                    AttachedWindow.DragMove();
                }
            }
        }

        private void AttachedGridOnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var mouseMovePoint = e.GetPosition(AttachedWindow);

            if (sCommon.IsAbleToDrag(mouseMovePoint, _downPoint))
            {
                e.Handled = true;

                var element = Mouse.Captured;
                element?.ReleaseMouseCapture();
            }
        }

        #endregion EventHandlers
    }
}
