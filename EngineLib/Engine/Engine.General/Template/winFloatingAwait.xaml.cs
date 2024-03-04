using Engine.Common;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace Engine.Template
{
    /// <summary>
    /// winFloatingAwait.xaml 的交互逻辑
    /// </summary>
    public partial class winFloatingAwait : Window
    {
        Window _OwnerWindow = null;

        public delegate void FuncOpenMainPanel();
        public delegate void FuncOpenSetting();
        public delegate void FuncExit();

        /// <summary>
        /// 打开主窗口
        /// </summary>
        public FuncOpenMainPanel ActionOpenMainPanel { get; set; }
        /// <summary>
        /// 设置
        /// </summary>
        public FuncOpenSetting ActionOpenSetting { get; set; }
        /// <summary>
        /// 退出
        /// </summary>
        public FuncExit ActionExit { get; set; }

         /// <summary>
         /// 
         /// </summary>
        public winFloatingAwait()
        {
            InitializeComponent();
            LoadDefaultView("", "");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="IconText"></param>
        /// <param name="winTitle"></param>
        public winFloatingAwait(string IconText, string winTitle)
        {
            InitializeComponent();
            LoadDefaultView(IconText, winTitle);
            DependencyPropertyDescriptor ownerDescriptor = DependencyPropertyDescriptor.FromProperty(Window.VisibilityProperty, this.GetType());
            ownerDescriptor.AddValueChanged(this, VisibilityChanged);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void VisibilityChanged(object sender, EventArgs e)
        {
            if (Owner != null)
                _OwnerWindow = Owner;
            Owner = null;
        }
        
        /// <summary>
        /// 加载窗口样式
        /// </summary>
        /// <param name="IconText"></param>
        /// <param name="winTitle"></param>
        private void LoadDefaultView(string IconText, string winTitle)
        {
            this.Topmost = true;
            this.ShowInTaskbar = false;
            //设置位置
            this.WindowStartupLocation = WindowStartupLocation.Manual;
            double screenWidth = SystemDefault.ScreenWidth;
            double windowWidth = this.Width;
            this.Left = screenWidth - windowWidth - 50;
            this.Top = 50;
            //设置标题内容
            this.ToolTip = winTitle;
            this.Title = winTitle;
            _WinTitle.Text = winTitle;
            _IconText.Text = IconText;

            this.PreviewKeyDown += (s, e) =>
            {
                e.Handled = true;
            };

            this.MouseLeftButtonDown += (s, e) =>
            {
                this.DragMove();
            };

            this.MouseRightButtonDown += (s, e) =>
            {
                this.ContextMenu.IsOpen = true;
            };

            this.MouseDoubleClick += (s, e) =>
            {
                if (_OwnerWindow == null)
                    return;
                if (_OwnerWindow.WindowState == WindowState.Minimized ||
                _OwnerWindow.Visibility != Visibility.Visible)
                {
                    _OwnerWindow.Visibility = Visibility.Visible;
                    _OwnerWindow.WindowState = WindowState.Normal;
                }
                else
                {
                    _OwnerWindow.Visibility = Visibility.Collapsed;
                }
            };

            this.Loaded += (s, e) =>
            {
                this.ContextMenu = new ContextMenu();
                ContextMenu contextMenu = this.ContextMenu;
                contextMenu.Items.Clear(); // 清空原有菜单项

                MenuItem mi = new MenuItem();
                mi.Name = "OpenMainPanel";
                mi.Header = "打开主面板";
                //mi.Icon = "";
                mi.Click += MenuItem_Click;
                contextMenu.Items.Add(mi);

                mi = new MenuItem();
                mi.Name = "Setting";
                mi.Header = "设置";
                mi.Click += MenuItem_Click;
                contextMenu.Items.Add(mi);

                Separator separator = new Separator();
                contextMenu.Items.Add(separator);

                mi = new MenuItem();
                mi.Name = "Exit";
                mi.Header = "退出";
                mi.Click += MenuItem_Click;
                contextMenu.Items.Add(mi);
            };
        }

        /// <summary>
        /// 菜单响应
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            string strCmd = (sender as MenuItem).Name.ToMyString();
            switch (strCmd)
            {
                case "OpenMainPanel":
                    ActionOpenMainPanel?.Invoke();
                    break;
                case "Setting":
                    ActionOpenSetting?.Invoke();
                    break;
                case "Exit":
                    ActionExit?.Invoke();
                    break;
            }
        }
    }
}
