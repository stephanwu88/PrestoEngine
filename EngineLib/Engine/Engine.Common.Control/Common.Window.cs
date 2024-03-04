using System;
using System.Windows;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Controls;
using Engine.Data.DBFAC;
using System.Data;

namespace Engine.Common
{
    /// <summary>
    /// 待处理废弃
    /// </summary>
    public static partial class sCommon
    {
        /// <summary>
        /// 查找已打开窗口
        /// </summary>
        /// <param name="WinName"></param>
        /// <returns></returns>
        public static Window MatchOpenedWindow(this string WinName)
        {
            WinName = WinName.Replace(".", "_");
            Window win = Application.Current.Windows.OfType<Window>().Where(x => x.Name == WinName && !string.IsNullOrEmpty(x.Name)).FirstOrDefault();
            if (win != null)
            {
                win.Activate();
                win.WindowState = WindowState.Normal;
            }
            return win;
        }
        /// <summary>
        /// 打开窗口
        /// </summary>
        /// <param name="win">窗口对象</param>
        /// <param name="WinKey">窗口名称</param>
        /// <returns></returns>
        public static bool OpenWindow(this Window win, string WinKey)
        {
            if (win == null || string.IsNullOrEmpty(WinKey))
                return false;
            Window winMatch = Application.Current.Windows.OfType<Window>().FirstOrDefault(x => x.Name == WinKey);
            if (winMatch == null)
            {
                win.Name = WinKey.Replace(".", "_");
                win.Show();
            }
            else
            {
                winMatch.Activate();
                winMatch.WindowState = WindowState.Normal;
                return true;
            }
            return false;
        }
    }

    /// <summary>
    /// 常用系统功能 - 窗口应用
    /// </summary>
    public static partial class sCommon
    {
        private static List<Window> _windows = new List<Window>();

        /// <summary>
        /// 打开单实例窗口
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="DataContext"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static T OpenWindow<T>(object DataContext, params object[] args)
        {
            return OpenWindow<T>(DataContext, null, args);
        }

        /// <summary>
        /// 打开单实例窗口
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Owner"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static T OpenWindow<T>(object DataContext, Window Owner, params object[] args)
        {
            Type type = typeof(T);
            Window win = type.OpenWindow(Owner, args);
            if (win != null && DataContext != null)
                win.DataContext = DataContext;
            return (T)(object)win;
        }

        /// <summary>
        /// 打开单实例窗口
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="args"></param>
        /// <returns></returns>
        public static T OpenWindow<T>(params object[] args)
        {
            return OpenWindow<T>(null, args);
        }

        /// <summary>
        /// 打开单实例窗口
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Owner"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static T OpenWindow<T>(Window Owner, params object[] args)
        {
            Type type = typeof(T);
            Window win = type.OpenWindow(Owner, args);
            return (T)(object)win;
        }

        /// <summary>
        /// 打开实例窗口
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="WinToken"></param>
        /// <param name="DelayCloseSecond"></param>
        /// <param name="Owner"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static T OpenWindow<T>(string WinToken, int DelayCloseSecond, Window Owner = null, params object[] args)
        {
            Type type = typeof(T);
            Window win = type.OpenWindow(Owner, WinToken, DelayCloseSecond, args);
            return (T)(object)win;
        }

        /// <summary>
        /// 打开单实例窗口
        /// </summary>
        /// <param name="windowType"></param>
        /// <param name="Owner"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static Window OpenWindow(this Type windowType, Window Owner = null, params object[] args)
        {
            return OpenWindow(windowType, Owner, "", 0, args);
        }

        /// <summary>
        /// 打开窗口
        /// </summary>
        /// <param name="windowType"></param>
        /// <param name="Owner"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static Window OpenWindow(this Type windowType, Window Owner, string WinToken, int DelayCloseSecond = 0, params object[] args)
        {
            try
            {
                Window window = default(Window);
                if (windowType.BaseType == typeof(UserControl))
                {
                    if (string.IsNullOrEmpty(WinToken))
                        window = _windows.FirstOrDefault(x => x.Content.GetType() == windowType);
                    else
                        window = _windows.FirstOrDefault(x => x.Content.GetType() == windowType && x.Tag.ToMyString() == WinToken);
                }
                else if (windowType.BaseType == typeof(Window))
                {
                    if (string.IsNullOrEmpty(WinToken))
                        window = _windows.FirstOrDefault(x => x.GetType() == windowType);
                    else
                        window = _windows.FirstOrDefault(x => x.GetType() == windowType && x.Tag.ToMyString() == WinToken);
                }
                if (window == null)
                {
                    if (windowType.BaseType == typeof(UserControl))
                    {
                        UserControl user = (UserControl)Activator.CreateInstance(windowType, args);
                        window = new Window() { Content = user, Title = WinToken };
                    }
                    else
                        window = (Window)Activator.CreateInstance(windowType, args);
                    if (window.Tag == null)
                        window.Tag = WinToken;
                    window.Closed += Window_Closed;
                    if (Owner == null)
                        Owner = Application.Current.MainWindow;
                    window.Owner = Owner;
                    if (window.WindowStartupLocation != WindowStartupLocation.Manual)
                    {
                        if (window.Owner == null)
                            window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                        else
                            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                    }
                    _windows.Add(window);
                }
                window.Activate();
                if (!window.IsVisible)
                {
                    SetWindow(ref window);
                    window.Show();
                    window.Unloaded += (s, e) =>
                      {
                          SaveOrUpdateWindow(window);
                      };
                }
                else
                    window.Focus();
                if (window.WindowState == WindowState.Minimized)
                    window.WindowState = WindowState.Normal;
                if (DelayCloseSecond > 0)
                {
                    Task.Delay(TimeSpan.FromSeconds(DelayCloseSecond)).ContinueWith((task, obj) =>
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            (window as Window).Close();
                        });
                    }, window);
                }
                return window;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// 关闭窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void Window_Closed(object sender, EventArgs e)
        {
            Window window = (Window)sender;
            window.Closed -= Window_Closed;
            if (_windows.Contains(window))
            {
                _windows.Remove(window);
                window = null;
            }
        }

        /// <summary>
        /// 关闭窗口
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void CloseWindow<T>()
        {
            Window winMatch = default(Window);
            foreach (Window item in _windows)
            {
                Type srcType = typeof(T);
                Type objType = default(Type);
                if (srcType.BaseType == typeof(UserControl))
                    objType = item.Content.GetType();
                else
                    objType = item.GetType();
                if (objType == srcType)
                {
                    winMatch = item;
                    break;
                }
            }
            if (winMatch != null) winMatch.Close();
        }

        /// <summary>
        /// 检查窗口是否存实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="WinToken"></param>
        /// <returns></returns>
        public static bool IsWindowOpened<T>(string WinToken = "")
        {
            Type windowType = typeof(T);
            return windowType.IsWindowOpened(WinToken);
        }

        /// <summary>
        /// 检查窗口是否存实例
        /// </summary>
        /// <param name="windowType"></param>
        /// <param name="WinToken"></param>
        /// <returns></returns>
        public static bool IsWindowOpened(this Type windowType, string WinToken = "")
        {
            Window window = null;
            if (windowType.BaseType == typeof(UserControl))
            {
                if (string.IsNullOrEmpty(WinToken))
                    window = _windows.FirstOrDefault(x => x.Content.GetType() == windowType);
                else
                    window = _windows.FirstOrDefault(x => x.Content.GetType() == windowType && x.Tag.ToMyString() == WinToken);
            }
            else if (windowType.BaseType == typeof(Window))
            {
                if (string.IsNullOrEmpty(WinToken))
                    window = _windows.FirstOrDefault(x => x.GetType() == windowType);
                else
                    window = _windows.FirstOrDefault(x => x.GetType() == windowType && x.Tag.ToMyString() == WinToken);
            }
            return window != null;
        }

        /// <summary>
        /// 窗口处于屏幕偏置左侧
        /// </summary>
        /// <param name="win"></param>
        /// <returns></returns>
        public static bool WindowAtScreenLeft(this Window win)
        {
            return win == null ? false : win.Left < SystemDefault.ScreenWidth / 2;
        }

        /// <summary>
        /// 窗口处于屏幕偏置右侧
        /// </summary>
        /// <param name="win"></param>
        /// <returns></returns>
        public static bool WindowAtScreenRight(this Window win)
        {
            return win == null ? false : win.Left >= SystemDefault.ScreenWidth / 2;
        }

        /// <summary>
        /// 设置依赖窗口位置
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="window"></param>
        public static void SetWindowPos<T>(this T window)
        {
            Window win = window as Window;
            if (Application.Current.MainWindow.WindowAtScreenRight())
            {
                win.Left = Application.Current.MainWindow.Left - win.Width + 12;
                win.Top = Application.Current.MainWindow.Top;
                if (win.Left < 0)
                {
                    Application.Current.MainWindow.Left += 0 - win.Left;
                    win.Left = 0;
                }
            }
            else
            {
                win.Left = Application.Current.MainWindow.Left + Application.Current.MainWindow.Width - 12;
                win.Top = Application.Current.MainWindow.Top;
                if (win.Left + win.Width > SystemDefault.ScreenWidth)
                {
                    Application.Current.MainWindow.Left -= win.Left + win.Width - SystemDefault.ScreenWidth;
                    win.Left = Application.Current.MainWindow.Left + Application.Current.MainWindow.Width - 12;
                }
            }
            win.Height = Application.Current.MainWindow.ActualHeight;
        }

        /// <summary>
        /// 判断两个点是否满足拖动的条件
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <returns></returns>
        public static bool IsAbleToDrag(Point point1, Point point2)
        {
            var flag = false;
            var offsetX = Math.Abs(point1.X - point2.X);
            var offsetY = Math.Abs(point1.Y - point2.Y);

            if (offsetX > SystemParameters.MinimumHorizontalDragDistance ||
                offsetY > SystemParameters.MinimumVerticalDragDistance)
            {
                flag = true;
            }

            return flag;
        }
    }

    /// <summary>
    /// 窗口信息持久化
    /// </summary>
    public partial class sCommon
    {
        /// <summary>
        /// 保存或更新窗口
        /// </summary>
        /// <param name="window"></param>
        public static void SaveOrUpdateWindow(Window window)
        {
            try
            {
                ModelWindow win = new ModelWindow()
                {
                    WindowType = window.GetType().FullName,
                    WindowKey = window.Tag.ToMyString(),
                    Left = window.Left,
                    Top = window.Top,
                    Height = window.Height,
                    Width = window.Width
                };
                CallResult result = DbFactory.Default.ExcuteQuery(
                    new ModelWindow()
                    {
                        WindowType = win.WindowType,
                        WindowKey = win.WindowKey
                    });
                if (result.Success && result.Result.ToMyDataTable().Rows.Count > 0)
                {
                    win.WindowType = win.WindowType.MarkWhere();
                    win.WindowKey = win.WindowKey.MarkWhere();
                    DbFactory.Default?.ExcuteUpdate(win);
                }
                else
                    DbFactory.Default?.ExcuteInsert(win);
            }
            catch (Exception)
            {

            }
        }

        /// <summary>
        /// 设置窗口
        /// </summary>
        public static void SetWindow(ref Window window)
        {
            try
            {
                if (window == null)
                    return;
                ModelWindow modWin = new ModelWindow()
                {
                    WindowType = window.GetType().FullName,
                    WindowKey = window.Tag.ToMyString()
                };
                DataTable dt = DbFactory.Default?.ExcuteQuery(modWin).Result.ToMyDataTable();
                if (dt == null)
                    return;
                if (dt.Rows.Count > 0)
                {
                    modWin = ColumnDef.ToEntity<ModelWindow>(dt.Rows[0]);
                    if (modWin.Left > 0) window.Left = modWin.Left;
                    if (modWin.Top > 0) window.Top = modWin.Top;
                    if (modWin.Height > 0) window.Height = modWin.Height;
                    if (modWin.Width > 0) window.Width = modWin.Width;
                }
            }
            catch (Exception)
            {

            }
        }
    }
}
