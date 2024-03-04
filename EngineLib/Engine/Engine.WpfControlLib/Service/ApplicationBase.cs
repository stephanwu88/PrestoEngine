using Engine.Common;
using Engine.Mod;
using System;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Engine.WpfBase
{
    /// <summary> Application基类 封装一些加载初始化和注入方法 </summary>
    public abstract partial class ApplicationBase : Application
    {
        /// <summary>
        /// 
        /// </summary>
        private Mutex mutex;

        /// <summary>
        /// 日志服务
        /// </summary>
        public ILogService ILogger => ServiceRegistry.Instance.GetInstance<ILogService>();

        /// <summary>
        /// 程序创建
        /// </summary>
        public IApplicationBuilder IApplicationBuilder => ServiceRegistry.Instance.GetInstance<IApplicationBuilder>();

        /// <summary>
        /// 服务集合
        /// </summary>
        public IServiceCollection IServiceCollection => ServiceRegistry.Instance.GetInstance<IServiceCollection>();


        /// <summary>
        /// 构造函数
        /// </summary>
        public ApplicationBase()
        {
            //异常监听
            InitCatchExcetion();

            //系统退出
            Application.Current.Exit += (s, e) =>
              {
                  sCommon.ExitEnvironment();
              };

            //依赖服务注册
            ServiceRegistry.Instance.Register<IServiceCollection, ServiceCollection>();
            ServiceRegistry.Instance.Register<IApplicationBuilder, ApplicationBuilder>();
            //配置服务
            ConfigureServices(IServiceCollection);
        }

        /// <summary>
        /// 重写启动过程
        /// </summary>
        /// <param name="e"></param>
        protected override void OnStartup(StartupEventArgs e)
        {
            //单实例检查 配置App环境
            CheckInstance();
            Configure(IApplicationBuilder);
            BootLoader(e);
            if (Application.Current.StartupUri == null)
                CreateMainWindow();
            base.OnStartup(e);
            //ILogger?.Info("系统启动");
        }

        /// <summary>
        /// 单实例运行
        /// </summary>
        public virtual void CheckInstance()
        {
            string appName = Assembly.GetExecutingAssembly().GetName().Name.ToString();
            mutex = new Mutex(true, appName, out bool IsNewCreated);
            if (!IsNewCreated)
            {
                if (MessageProxy.Windower == null)
                {
                    sCommon.MyMsgBox("系统已经运行，请勿重复启动！", MsgType.Exclamation);
                }
                else
                {
                    MessageProxy.Windower.ShowSumit("当前程序已经运行！", null, false, -1);
                }
                Environment.Exit(0);
            }
        }

        /// <summary>
        /// 应用程序配置
        /// </summary>
        /// <param name="app"></param>
        protected virtual void Configure(IApplicationBuilder app)
        {
            //线程设置
            ThreadPool.SetMaxThreads(1000, 1000);
            ThreadPool.SetMinThreads(200, 200);
            Thread.CurrentThread.CurrentCulture = (CultureInfo)Thread.CurrentThread.CurrentCulture.Clone();
            Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortDatePattern = SystemDefault.DateFormat;
            Thread.CurrentThread.CurrentCulture.DateTimeFormat.LongDatePattern = SystemDefault.DateFormat;
            Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortTimePattern = SystemDefault.TimeFormat;
            Thread.CurrentThread.CurrentCulture.DateTimeFormat.LongTimePattern = SystemDefault.TimeFormat;
            //启动内存回管理
            sCommon.MemoryCollectWorker(30);
        }

        /// <summary>
        /// 配置服务
        /// </summary>
        /// <param name="services"></param>
        protected virtual void ConfigureServices(IServiceCollection services)
        {
            //services.AddStyle();
        }

        /// <summary>
        /// 自定义启动过程
        /// </summary>
        /// <param name="e"></param>
        protected virtual void BootLoader(StartupEventArgs e)
        {   
            
        }

        //创建主窗体
        protected virtual void CreateMainWindow()
        {
            
        }
    }

    /// <summary>
    /// 异常捕捉监听
    /// </summary>
    public abstract partial class ApplicationBase
    {
        /// <summary>
        /// 初始化异常捕捉监听
        /// </summary>
        protected virtual void InitCatchExcetion()
        {
            base.DispatcherUnhandledException += App_DispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            //TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            //Application.Current.Dispatcher.Invoke(delegate
            //{
            //    if (MessageProxy.Windower == null)
            //    {
            //        MessageBox.Show(e.Exception?.ToString(), "系统异常");
            //    }
            //    else
            //    {
            //        MessageProxy.Windower?.ShowSumit(e.Exception?.ToString(), "系统异常", false, -1);
            //    }
            //});
            //e.Handled = true;
            //ILogger?.Error("系统异常");
            //ILogger?.Error(e.Exception);
            e.Handled = true;
            string strMsg = "应用程序遇到重大错误,该错误在此将被忽略\r\n若问题继续存在，请联系管理员\r\n" + e.Exception.Message;
            MessageBox.Show(strMsg, "意外操作", MessageBoxButton.OK, MessageBoxImage.Error);
            Logger.Error.Write(LOG_TYPE.ERROR, string.Format("UI线程发生重大错误:{0}", e.Exception.ToString()));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            //Exception ex = (Exception)e.ExceptionObject;
            //Application.Current.Dispatcher.Invoke(() => MessageBox.Show("当前应用程序遇到一些问题，该操作已经终止，请进行重试，若问题继续存在，请联系管理员", "意外的操作"));
            //ILogger?.Fatal("当前应用程序遇到一些问题，该操作已经终止，请进行重试，若问题继续存在，请联系管理员", "意外的操作");
            //ILogger?.Fatal(ex);
            string strErrMsg = string.Empty;
            if (e.ExceptionObject != null)
                strErrMsg = e.ExceptionObject.GetType().Name;
            string strMsg = "应用程序遇到重大错误,该错误在此将被忽略\r\n若问题继续存在，请联系管理员\r\n" + strErrMsg;
            MessageBox.Show(strMsg, "程序崩溃", MessageBoxButton.OK, MessageBoxImage.Error);
            Logger.Error.Write(LOG_TYPE.ERROR, string.Format("非UI线程异常导致程序崩溃:{0}", strErrMsg));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            foreach (Exception innerException in e.Exception.InnerExceptions)
            {
                sb.AppendLine($"异常类型：{innerException.GetType()}\r\n异常内容：{innerException.Message}\r\n来自：{innerException.Source}\r\n{innerException.StackTrace}");
            }
            e.SetObserved();
            MessageBox.Show(sb.ToString(), "系统任务异常", MessageBoxButton.OK, MessageBoxImage.Error);
            //ILogger?.Error("Task Exception");
            //ILogger?.Error(sb.ToString());
            //Application.Current.Dispatcher.Invoke(() => MessageProxy.Windower.ShowSumit(sb.ToString(), "系统任务异常", false, -1));
        }
    }
}
