using System;
using System.Collections.Generic;
using System.Security.Permissions;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;
using System.ServiceProcess;
using System.IO;
using System.Windows.Threading;
using System.Windows;

namespace Engine.Common
{
    /// <summary>
    /// Windows系统服务
    /// </summary>
    public static partial class sCommon
    {
        /// <summary>
        /// 获取系统服务
        /// System.ServiceProcess.dll
        /// </summary>
        /// <param name="ServiceDisplayName">服务显示名称</param>
        /// <returns>-1:未找到服务  0:服务未运行  1:服务已运行</returns>
        public static int GetWinService(string ServiceDisplayName)
        {
            ServiceController[] service = ServiceController.GetServices();
            int iResult = -1;
            for (int i = 0; i < service.Length; i++)
            {
                if (service[i].DisplayName.ToUpper().Equals(ServiceDisplayName.ToUpper()))
                {
                    if (service[i].Status == ServiceControllerStatus.Running)
                        iResult = 1;
                    else
                        iResult = 0;
                    break;
                }
            }
            return iResult;
        }

        /// <summary>
        /// 获取系统服务
        /// System.ServiceProcess.dll
        /// </summary>
        /// <param name="ServiceDisplayName">服务显示名称</param>
        /// <returns>服务显示名称列表</returns>
        public static List<string> GetWinServiceDispNames(string ServiceDisplayName)
        {
            ServiceController[] service = ServiceController.GetServices();
            List<string> ServiceDispNames = new List<string>();
            for (int i = 0; i < service.Length; i++)
                ServiceDispNames.Add(service[i].DisplayName);
            return ServiceDispNames;
        }
    }

    /// <summary>
    /// Windows进程管理
    /// </summary>
    public static partial class sCommon
    {
        /// <summary>
        /// 启动进程
        /// </summary>
        /// <param name="ProcessFile"></param>
        /// <param name="ListOutput"></param>
        /// <param name="ListError"></param>
        /// <returns></returns>
        public static bool StartProcess(string ProcessFile, ref List<string> ListOutput, ref List<string> ListError)
        {
            try
            {
                //获得当前登录的Windows用户身份角色
                //WindowsIdentity identity = WindowsIdentity.GetCurrent();
                //WindowsPrincipal principal = new WindowsPrincipal(identity);
                //if (principal.IsInRole(WindowsBuiltInRole.Administrator))
                //{
                //    //管理员身份
                //}
                using (Process process = new Process())
                {
                    FileInfo file = new FileInfo(ProcessFile);
                    if (file.Directory != null)
                        process.StartInfo.WorkingDirectory = file.Directory.FullName;
                    process.StartInfo.FileName = ProcessFile;
                    process.StartInfo.RedirectStandardOutput = true;    //重定向IO流
                    process.StartInfo.RedirectStandardError = true;     //重定向错误流
                    process.StartInfo.UseShellExecute = false;          //是否指定操作系统外壳进程启动程序
                    process.StartInfo.CreateNoWindow = true;            //不显示dos命令行窗口
                    process.StartInfo.Verb = "runas";                   //管理员身份运行
                    process.Start();
                    process.WaitForExit();
                    ListOutput = StreamToList(process.StandardOutput);
                    ListError = StreamToList(process.StandardError);
                    process.Close();                                    //关闭进程
                }
                return ListError.Count == 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 启动进程
        /// </summary>
        /// <param name="ExeName"></param>
        /// <param name="ProcessFileName"></param>
        /// <returns></returns>
        public static Process StartProcess(string ExeName, string ProcessFileName)
        {
            Process ps = new Process();
            ps.StartInfo.FileName = ExeName;
            ps.StartInfo.Arguments = $"\"{ProcessFileName}\"";
            ps.StartInfo.UseShellExecute = false;
            ps.StartInfo.CreateNoWindow = true;
            ps.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            ps.StartInfo.Verb = "runas";                  
            ps.Start();
            return ps;
        }

        /// <summary>
        /// 取消进程
        /// </summary>
        /// <param name="ProcessName">进程名称</param>
        public static void KillProcess(string ProcessName)
        {
            try
            {
                Process ps = GetProcess(ProcessName);
                if (ps != null)
                {
                    ps.Kill();
                    ps.Close();
                }
            }
            catch (Exception )
            {

            }            
        }

        /// <summary>
        /// 根据进程名称从进程列表中获取进程对象
        /// </summary>
        /// <param name="ProcessName">进程名称</param>
        /// <returns></returns>
        public static Process GetProcess(string ProcessName)
        {
            Process ps = default(Process);
            Process[] psLst = Process.GetProcessesByName(ProcessName);
            if (psLst.Length > 0)
                ps = psLst[0];
            return ps;
        }

        /// <summary>
        /// 从进程列表中获取进程对象，若没有自动创建
        /// </summary>
        /// <param name="ProcessName">进程名称</param>
        /// <param name="StartFileName">创建进程文件</param>
        /// <returns></returns>
        public static Process GetProcessWithStartFile(string ProcessName, string ExeName, string StartFileName)
        {
            Process WorkProc = GetProcess(ProcessName);
            if (WorkProc == null)
                WorkProc = StartProcess(ExeName, StartFileName); 
            return WorkProc;
        }

        /// <summary>
        /// 退出应用环境
        /// </summary>
        public static void ExitEnvironment()
        {
            Environment.Exit(0);
            Application.Current.Shutdown(0);
        }
    }

    /// <summary>
    /// Windows内存管理
    /// </summary>
    public static partial class sCommon
    {
        /// <summary>
        /// 清除缓存
        /// </summary>
        /// <param name="sleepSpan">秒</param>
        public static void MemoryCollectWorker(int sleepSpan = 30)
        {
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    FlushMemory();
                    Thread.Sleep(TimeSpan.FromSeconds((double)sleepSpan));
                }
            });
        }

        /// <summary>
        /// 回收内存
        /// </summary>
        /// <returns></returns>
        public static bool FlushMemory()
        {
            try
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
                if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                    SetProcessWorkingSetSize(Process.GetCurrentProcess().Handle, -1, -1);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 注： Application.DoEvents()方法是在Winfrom 当中的，但是在WPF里面是没有Application.DoEvents();方法的，
        /// </summary>
        [SecurityPermissionAttribute(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public static void DoEvents()
        {
            DispatcherFrame frame = new DispatcherFrame();
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(ExitFrames), frame);
            try { Dispatcher.PushFrame(frame); }
            catch (InvalidOperationException) { }
        }
        private static object ExitFrames(object frame)
        {
            ((DispatcherFrame)frame).Continue = false;
            return null;
        }
    }

    /// <summary>
    /// 用户交互 句柄操纵
    /// </summary>
    public static partial class sCommon
    {
        /// <summary>
        /// 获取对象坐标尺寸
        /// </summary>
        /// <param name="awin"></param>
        /// <param name="Margin"></param>
        /// <param name="Width"></param>
        /// <param name="Height"></param>
        /// <returns></returns>
        public static bool GetObjectRect(IntPtr hwd, out Thickness Margin, out int Width, out int Height)
        {
            Margin = new Thickness();
            Width = 0;
            Height = 0;
            try
            {
                RECT rc = new RECT();
                GetWindowRect(hwd, ref rc);
                Width = rc.Right - rc.Left;     //窗口的宽度
                Height = rc.Bottom - rc.Top;    //窗口的高度
                Margin.Left = rc.Left.ToMyDouble();
                Margin.Top = rc.Top.ToMyDouble();
                Margin.Right = rc.Right.ToMyDouble();
                Margin.Bottom = rc.Bottom.ToMyDouble();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 获取窗口标题
        /// </summary>
        /// <returns></returns>
        public static string GetWindowTitle(this IntPtr hwd)
        {
            try
            {
                string winTitle = string.Empty;
                GetWindowText(hwd, winTitle, 256);
                return winTitle;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 获取任务栏句柄
        /// </summary>
        /// <returns></returns>
        public static IntPtr GetTaskBarHwnd()
        {
            IntPtr hwd = FindWindowEx(IntPtr.Zero, IntPtr.Zero, "Shell_TrayWnd", null);
            return hwd;
        }

        /// <summary>
        /// 递归搜索目标句柄
        /// </summary>
        /// <param name="hwndRoot">根句柄</param>
        /// <param name="DeepString">窗口深度 从1开始 ex: 1.2.4</param>
        /// <returns></returns>
        public static IntPtr SearchObjectHandle(this IntPtr hwndRoot, string DeepString)
        {
            List<string> lstSource = DeepString.MySplit(".");
            List<int> lstDeep = new List<int>();
            foreach (var item in lstSource)
            {
                lstDeep.Add(item.ToMyInt());
            }
            if (lstDeep.Count == 0)
                return IntPtr.Zero;
            return SearchObjectHandle(hwndRoot, lstDeep.ToArray());
        }

        /// <summary>
        /// 递归搜索目标句柄
        /// </summary>
        /// <param name="hwndRoot">根句柄</param>
        /// <param name="ArrDeep">窗口深度 从1开始，章节表达式 ex:  1 2 4</param>
        /// <returns></returns>
        public static IntPtr SearchObjectHandle(this IntPtr hwndRoot, int[] ArrayDeep)
        {
            if (hwndRoot == IntPtr.Zero)
                return IntPtr.Zero;
            IntPtr hwndTemp = IntPtr.Zero;
            for (int i = 0; i < ArrayDeep.MyCount(); i++)
            {
                int iDeepCount = ArrayDeep[i];
                if (iDeepCount <= 0) return hwndTemp;
                IntPtr hwndSubWin = i == 0 ? hwndRoot : hwndTemp;
                for (int j = 0; j < iDeepCount; j++)
                {
                    if (j == 0)
                        hwndTemp = FindWindowEx(hwndSubWin, IntPtr.Zero, null, null);
                    else
                        hwndTemp = FindWindowEx(hwndSubWin, hwndTemp, null, null);
                    if (hwndTemp == IntPtr.Zero)
                        return hwndTemp;
                }
            }
            return hwndTemp;
        }

        /// <summary>
        /// 检索子节点中名称匹配的
        /// </summary>
        /// <param name="hwndRoot"></param>
        /// <param name="WinTitle"></param>
        /// <param name="LimitDeep"></param>
        /// <returns></returns>
        public static IntPtr SearchChild(this IntPtr hwndRoot, string WinTitle, int LimitDeep = -1)
        {
            if (hwndRoot == IntPtr.Zero)
                return IntPtr.Zero;
            IntPtr hwndTemp = IntPtr.Zero;
            IntPtr hwndSubWin = hwndRoot;
            if (LimitDeep > 0)
            {
                for (int j = 0; j < LimitDeep; j++)
                {
                    if (j == 0)
                        hwndTemp = FindWindowEx(hwndSubWin, IntPtr.Zero, null, null);
                    else
                        hwndTemp = FindWindowEx(hwndSubWin, hwndTemp, null, null);
                    if (hwndTemp == IntPtr.Zero)
                        return hwndTemp;
                    if (hwndTemp.GetWindowTitle() == WinTitle)
                        return hwndTemp;
                }
            }
            else
            {
                int j = 0;
                do
                {
                    if (j == 0)
                        hwndTemp = FindWindowEx(hwndSubWin, IntPtr.Zero, null, null);
                    else
                        hwndTemp = FindWindowEx(hwndSubWin, hwndTemp, null, null);
                    if (hwndTemp == IntPtr.Zero)
                        return hwndTemp;
                    if (hwndTemp.GetWindowTitle() == WinTitle)
                        return hwndTemp;
                    j++;
                } while (hwndTemp != IntPtr.Zero);
            }
            return IntPtr.Zero;
        }

        /// <summary>
        /// 检索子节点中名称匹配的
        /// </summary>
        /// <param name="hwndRoot"></param>
        /// <param name="ObjChild">目标子句柄</param>
        /// <param name="LimitDeep"></param>
        /// <returns></returns>
        public static IntPtr SearchChild(this IntPtr hwndRoot, IntPtr ObjChild, int LimitDeep = -1)
        {
            if (hwndRoot == IntPtr.Zero)
                return IntPtr.Zero;
            IntPtr hwndTemp = IntPtr.Zero;
            IntPtr hwndSubWin = hwndRoot;
            if (LimitDeep > 0)
            {
                for (int j = 0; j < LimitDeep; j++)
                {
                    if (j == 0)
                        hwndTemp = FindWindowEx(hwndSubWin, IntPtr.Zero, null, null);
                    else
                        hwndTemp = FindWindowEx(hwndSubWin, hwndTemp, null, null);
                    if (hwndTemp == IntPtr.Zero)
                        return hwndTemp;
                    if (hwndTemp == ObjChild)
                        return hwndTemp;
                }
            }
            else
            {
                int j = 0;
                do
                {
                    if (j == 0)
                        hwndTemp = FindWindowEx(hwndSubWin, IntPtr.Zero, null, null);
                    else
                        hwndTemp = FindWindowEx(hwndSubWin, hwndTemp, null, null);
                    if (hwndTemp == IntPtr.Zero)
                        return hwndTemp;
                    if (hwndTemp == ObjChild)
                        return hwndTemp;
                    j++;
                } while (hwndTemp != IntPtr.Zero);
            }
            return IntPtr.Zero;
        }
      
        /// <summary>
        /// 隐藏任务栏
        /// </summary>
        /// <param name="hwd"></param>
        public static void HideTaskBar()
        {
            GetTaskBarHwnd().HideWindow();
        }

        /// <summary>
        /// 显示任务栏
        /// </summary>
        /// <param name="hwd"></param>
        public static void ShowTaskBar()
        {
            GetTaskBarHwnd().ShowWindow(1);
        }

        /// <summary>
        /// 挂载到父句柄并设置窗口位置
        /// </summary>
        /// <param name="hwd"></param>
        /// <param name="Parent"></param>
        /// <param name="Left"></param>
        /// <param name="Top"></param>
        /// <param name="Width"></param>
        /// <param name="Height"></param>
        public static void MountToParent(this IntPtr hwd, IntPtr Parent, int Left = 0, int Top = 0, int Width = -1, int Height = -1)
        {
            try
            {
                if (Width < 0 || Height < 0)
                {
                    bool Success = GetObjectRect(Parent, out Thickness thick, out int iWidth, out int iHeight);
                    if (Success)
                    {
                        if (Width < 0 && iWidth > 0) Width = iWidth;
                        if (Height < 0 && iHeight > 0) Height = iHeight;
                    }
                }
                //hwd.SetWindowPos(IntPtr.Zero, Left, Top, Width, Height, SWP_NOMOVE | SWP_NOSIZE | SWP_NOZORDER | SWP_FRAMECHANGED);
                hwd.SetWindowPos(IntPtr.Zero, Left, Top, Width, Height, SWP_NOZORDER);
                IntPtr FindPoint = Parent.SearchChild(hwd);
                if (FindPoint == IntPtr.Zero)
                {
                    hwd.SetParent(Parent);
                    hwd.WindowMaxmized();
                }
            }
            catch (Exception)
            {

            }
        }
    }

    /// <summary>
    /// 用户交互 - 窗口操作
    /// </summary>
    public partial class sCommon
    {
        /// <summary>
        /// 控制窗口程序
        /// 0-关闭窗口 1-正常大小显示 2最小化窗口 3-最大化窗口
        /// </summary>
        /// <param name="hwnd">窗口句柄</param>
        /// <param name="nCmdShow">0-关闭窗口 1-正常大小显示 2最小化窗口 3-最大化窗口</param>
        /// <returns></returns>
        public static int DisplayWindow(this IntPtr hwnd, int nCmdShow)
        {
            return ShowWindow(hwnd, nCmdShow);
        }

        /// <summary>
        /// 查找弹窗,可选自动处理按钮
        /// </summary>
        /// <param name="PopWinPtr">弹窗句柄</param>
        /// <param name="AutoClickBtn">自动点击按钮(根据名称) 确定|Sure</param>
        /// <returns></returns>
        public static bool SearchPopWindow(out IntPtr PopWinPtr, string AutoClickButton = "确定")
        {
            PopWinPtr = FindWindow("#32770", null);
            if (PopWinPtr != IntPtr.Zero && !string.IsNullOrEmpty(AutoClickButton))
            {
                List<string> LstBtnName = AutoClickButton.MySplit("|");
                foreach (string btnName in LstBtnName)
                {
                    IntPtr hwdSureBtn = FindWindowEx(PopWinPtr, IntPtr.Zero, "Button", btnName);
                    hwdSureBtn.SendMessage_Click();
                }
            }
            return PopWinPtr != IntPtr.Zero;
        }

        /// <summary>
        /// 最大化窗口
        /// </summary>
        /// <param name="hwd"></param>
        public static void WindowMaxmized(this IntPtr hwd)
        {
            if (hwd == IntPtr.Zero)
                return;
            hwd.ShowWindow(3);
        }

        /// <summary>
        /// 最小化窗口
        /// </summary>
        /// <param name="hwd"></param>
        public static void WindowMinimized(this IntPtr hwd)
        {
            if (hwd == IntPtr.Zero)
                return;
            hwd.ShowWindow(2);
        }

        /// <summary>
        /// 隐藏窗口
        /// </summary>
        /// <param name="hwd"></param>
        public static void HideWindow(this IntPtr hwd)
        {
            if (hwd == IntPtr.Zero)
                return;
            hwd.ShowWindow(0);
        }

        /// <summary>
        /// 显示窗口
        /// </summary>
        /// <param name="hwd"></param>
        public static void ShowWindow(this IntPtr hwd)
        {
            if (hwd == IntPtr.Zero)
                return;
            hwd.ShowWindow(1);
        }

        /// <summary>
        /// 禁止窗口命令操作 - 禁止调整大小，禁止移动
        /// </summary>
        /// <param name="win"></param>
        public static void DisableWindowSysCommand(this Window win)
        {
            win.SourceInitialized += (s, e) =>
            {
                System.Windows.Interop.HwndSource source = System.Windows.Interop.HwndSource.FromHwnd(new System.Windows.Interop.WindowInteropHelper(win).Handle);
                source.AddHook(new System.Windows.Interop.HwndSourceHook(WndProc));
            };

            IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
            {
                if (msg == WM_NCLBUTTONDBLCLK)
                {
                    handled = true;
                }
                else if (msg == WM_SYSCOMMAND && (int)wParam == SC_MOVE)
                {
                    handled = true;
                }
                return IntPtr.Zero;
            }
        }

        /// <summary>
        /// 禁用窗口关闭按钮
        /// </summary>
        /// <param name="hwd">窗口句柄</param>
        public static void DisableCloseButton(this IntPtr hwd)
        {
            if (hwd == IntPtr.Zero)
                return;
            //找控制菜单
            IntPtr CLOSE_MENU = GetSystemMenu(hwd, IntPtr.Zero);
            int SC_CLOSE = 0xF060;
            //禁用关闭按钮
            RemoveMenu(CLOSE_MENU, SC_CLOSE, 0x0);
        }

        /// <summary>
        /// 除去窗体边框，包括标题栏
        /// </summary>
        public static void RemoveWindowBorder(this IntPtr hwd)
        {
            if (hwd == IntPtr.Zero)
                return;
            Int32 wndStyle = GetWindowLong(hwd, GWL_STYLE);
            wndStyle &= ~WS_BORDER;
            wndStyle &= ~WS_THICKFRAME;
            SetWindowLong(hwd, GWL_STYLE, wndStyle);
        }
    }

    /// <summary>
    /// 用户交互  外挂消息封装
    /// </summary>
    public static partial class sCommon
    {
        /// <summary>
        /// 发送点击消息
        /// </summary>
        /// <param name="hwndBtn"></param>
        public static void SendMessage_Click(this IntPtr hwndBtn)
        {
            if (hwndBtn == IntPtr.Zero)
                return;
            SendMessage(hwndBtn, WM_BM_CLICK, 0, 0);
        }
    }
}
