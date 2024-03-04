using Engine.Common;
using Engine.Data;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Engine.MVVM
{
    public class ViewFrameBase : NotifyObject
    {
        /// <summary>
        /// 连续添加
        /// </summary>
        public bool ContAddMode
        {
            get { return _ContAddMode; }
            set { _ContAddMode = value; RaisePropertyChanged(); }
        }
        private bool _ContAddMode = false;

        /// <summary>
        /// 编辑模式
        /// </summary>
        public EditMode editMode
        {
            get { return _editMode; }
            set
            {
                _editMode = value;
                RaisePropertyChanged();
            }
        }
        private EditMode _editMode = EditMode.AddNew;


        /// <summary>
        /// 窗口标题
        /// </summary>
        public string WinTitle
        {
            get { return _WinTitle; }
            set { _WinTitle = value; RaisePropertyChanged(); }
        }
        private string _WinTitle = "";

        /// <summary>
        /// 提交事件
        /// </summary>
        public delegate void DelegateActionCommit(object sender, object data);
        public DelegateActionCommit ActionCommit { get; set; }

        /// <summary>
        /// 菜单集合
        /// </summary>
        public ObservableCollection<PrsMenuItem> MenuItemList { get; set; } =
          new ObservableCollection<PrsMenuItem>();

        /// <summary>
        /// 窗口对象
        /// </summary>
        public object MainSubWin { get => _MainSubWin; set { _MainSubWin = value; RaisePropertyChanged(); } }
        private object _MainSubWin;

        /// <summary>
        /// 窗口页ID
        /// </summary>
        public string SelectedMainPageID { get => _SelectedMainPageID; set { _SelectedMainPageID = value; RaisePropertyChanged(); } }
        private string _SelectedMainPageID;
        
        /// <summary>
        /// 窗口页名称指定
        /// </summary>
        public string SelectedPage { get => _SelectedPage; set { _SelectedPage = value; RaisePropertyChanged(); } }
        private string _SelectedPage;

        /// <summary>
        /// 主菜单解析
        /// </summary>
        public virtual ICommand MenuCommand
        {
            get => new MyCommand((CommandParameter) =>
            {
                if (CommandParameter is PrsMenuItem mi)
                {
                    if (mi.SubMenu != null)
                    {

                    }
                    else if (mi.Page is UserControl)
                    {
                        MainSubWin = mi.Page as UserControl;
                        SelectedMainPageID = mi.PageID;
                    }
                    else if (mi.Page != null)
                        (mi.Page as System.Type).OpenWindow(Application.Current.MainWindow);
                    if (mi.Command != null)
                    {
                        mi.Command.Execute(null);
                    }
                }
            });
        }

        /// <summary>
        /// 最小化
        /// </summary>
        public ICommand CommandMinimizeWindow
        {
            get => new MyCommand((CommandParameter) =>
            {
                //SystemCommands.MinimizeWindow(Application.Current.MainWindow);
                SystemCommands.MinimizeWindow(CommandParameter as Window);
            });
        }

        /// <summary>
        /// 最大化
        /// </summary>
        public ICommand CommandMaximizeWindow
        {
            get => new MyCommand((CommandParameter) =>
            {
                Window win = CommandParameter as Window;
                if (win.WindowState == WindowState.Maximized)
                {
                    SystemCommands.RestoreWindow(win);
                    CommandMaxContent = "&#xe695;".UnicodeToString();
                }
                else
                {
                    SystemCommands.MaximizeWindow(win);
                    CommandMaxContent = "&#xe62b;".UnicodeToString();
                }
            });
        }

        /// <summary>
        /// 最大化
        /// </summary>
        public string CommandMaxContent { get => _CommandMaxContent; set { _CommandMaxContent = value; RaisePropertyChanged(); } }
        private string _CommandMaxContent = "&#xe695;".UnicodeToString();

        /// <summary>
        /// 关闭窗口
        /// </summary>
        public ICommand CommandCloseWindow
        {
            get => new MyCommand((CurrentWindow) =>
            {
                if (CurrentWindow == null) return;
                if(CurrentWindow is Window win)
                    win.Close();
            });
        }

        /// <summary>
        /// 退出应用
        /// </summary>
        public ICommand CommandExitEnvironment
        {
            get => new MyCommand((o) =>
            {
                MessageBoxResult result = sCommon.MyMsgBox("您是否确定退出运行环境？", MsgType.Question);
                if (result == MessageBoxResult.Yes)
                {
                    sCommon.ExitEnvironment();
                }
            });
        }
    }
}
