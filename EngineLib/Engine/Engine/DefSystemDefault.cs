using Engine.ComDriver;
using Engine.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;

namespace Engine
{
    /// <summary>
    /// 系统默认值
    /// </summary>
    public static class SystemDefault
    {
        /// <summary>
        /// 页面是否处于设计模式
        /// </summary>
        public static bool IsInDesignMode = DesignerProperties.GetIsInDesignMode(new DependencyObject());
        /// <summary>
        /// 屏幕宽度
        /// </summary>
        public static double ScreenWidth = System.Windows.SystemParameters.PrimaryScreenWidth;
        /// <summary>
        /// 屏幕高度
        /// </summary>
        public static double ScreenHeight= System.Windows.SystemParameters.PrimaryScreenHeight;
        /// <summary>
        /// double无效值
        /// 1.7 * Power(10,-308) double.MinValue
        /// </summary>
        public const double InValidDouble = 1.7E-308;
        /// <summary>
        /// int无效值
        /// Int32.MinValue
        /// </summary>
        public const int InValidInt = -2147483648; 
        /// <summary>
        /// 口令加密关键字
        /// </summary>
        public const string EnCryptWord = "[p$]";
        /// <summary>
        /// 数据库字段字符串值空表示
        /// </summary>
        public const string StringEmpty = "*EMP#";
        /// <summary>
        /// 数据库连接口令
        /// </summary>
        public const string DbConPwd = "178876839@qq.com";
        /// <summary>
        /// 未配置情况下默认日志路径
        /// </summary>
        public static string LogPath = Environment.CurrentDirectory + @"\Log";
        /// <summary>
        /// 日志txt文件名
        /// </summary>
        public static string LogFileName = "Log";
        /// <summary>
        /// 系统自定义连接符 &  样号后缀连接符
        /// </summary>
        public static string LinkSign = "&";
        /// <summary>
        /// 系统自定义换元符 @
        /// </summary>
        public static string ReplaceSign = "@";
        /// <summary>
        /// 系统自定义显示连接符 -
        /// </summary>
        public static string ViewLinkSign = "-";
        /// <summary>
        /// 系统自定义显示分割符 -
        /// </summary>
        public static string ViewSplitSign = " ";
        /// <summary>
        /// 系统自定义分割符 |
        /// </summary>
        public static string SplitSign = "|";
        /// <summary>
        /// 
        /// </summary>
        public static ByteOrder16 SiemensByteOrder16 = ByteOrder16.AB;
        /// <summary>
        /// 
        /// </summary>
        public static ByteOrder32 SiemensByteOrder32 = ByteOrder32.ABCD;
        /// <summary>
        /// 默认日期时间格式
        /// </summary>
        public static string DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
        /// <summary>
        /// 默认日期格式
        /// </summary>
        public static string DateFormat = "yyyy-MM-dd";
        /// <summary>
        /// 默认时间格式
        /// </summary>
        public static string TimeFormat = "HH:mm:ss";

        /// <summary>
        /// 运行程序名称，软件进程主键
        /// </summary>
        public static string AppName
        {
            get
            {
                if (string.IsNullOrEmpty(_AppName))
                    _AppName = MyAssembly.Default.AssemblyNameWithoutExtension;
                    //_AppName = Assembly.GetExecutingAssembly().GetName().Name.ToMyString();
                return _AppName;
            }
            set
            {
                if (_AppName != value)
                    _AppName = value;
            }
        }
        private static string _AppName;

        /// <summary>
        /// 默认命名空间设置
        /// </summary>
        public static string AppNameSpace { get; } = "App";

        /// <summary>
        /// 系统当前时间
        /// </summary>
        public static string StringTimeNow
        {
            get => DateTime.Now.ToMyString(DateTimeFormat);
        }

        /// <summary>
        /// 系统当前时间 - 时分秒
        /// </summary>
        public static string StringHmsNow
        {
            get => DateTime.Now.ToMyString(TimeFormat);
        }

        /// <summary>
        /// 当日
        /// </summary>
        public static DateTime Today => DateTime.Today;

        /// <summary>
        /// 昨日
        /// </summary>
        public static DateTime Yesterday => DateTime.Today.AddDays(-1);

        /// <summary>
        /// 明日
        /// </summary>
        public static DateTime Tomorrow => DateTime.Today.AddDays(1);

        /// <summary>
        /// 系统默认UUID
        /// </summary>
        public static string UUID => Guid.NewGuid().ToString("N");
        /// <summary>
        /// 系统默认数字类型UUID
        /// </summary>
        public static string NumberUUID => DateTime.Now.ToString("yyyyMMddHHmmssfffff");
        /// <summary>
        /// 回车换行符
        /// </summary>
        public static string Crlf=> Convert.ToChar(0x0D).ToString() + Convert.ToChar(0x0A).ToString();
        /// <summary>
        /// 回车符
        /// </summary>
        public static string Cr => Convert.ToChar(0x0D).ToString();
        /// <summary>
        /// 空格符
        /// </summary>
        public static string Space => " ";
    }

    /// <summary>
    /// 系统定义
    /// </summary>
    public static class SystemDef
    {
        /// <summary>
        /// 符号数据类型
        /// </summary>
        public static readonly List<string> LstSymbolDataType = new List<string>()
        {
            "string","number","DataView","Command","AlarmView","SubPosView"
        };
        /// <summary>
        /// 符号数据类型 - 集合
        /// </summary>
        private static readonly List<string> LstGatherDataType = new List<string>()
        {
            "DataView","Command","AlarmView","SubPosView"
        };
        /// <summary>
        /// 是否集合数据类型
        /// 集合
        /// </summary>
        /// <param name="SymbolDataType"></param>
        /// <returns></returns>
        public static bool IfGatherData(this string SymbolDataType)
        {
            return LstGatherDataType.Contains(SymbolDataType);
        }
        //协议数据库变量类型
        public static List<string> ComProtocolDataUnitType = new List<string>()
        {
            "Status","Alarm","Command","Path"
        };
        //和澳通讯协议变量地址类型
        public static List<string> HeaoComProtocolAddrType = new List<string>()
        {
            "Bool","byte[ ]","String"
        };
    }

    /// <summary>
    /// 任务状态定义
    /// </summary>
    public class TaskStateDef
    {
        public static string NORMAL = "NORMAL";
        public static string RUN = "RUN";
        public static string DONE = "DONE";
    }

    /// <summary>
    /// 工位状态定义
    /// </summary>
    public class PosStateDef
    {
        public static string NORMAL = "NORMAL";
        public static string RUN = "RUN";
        public static string ALARM = "ALARM";
        public static string WAIT = "WAIT";
        public static string DONE = "DONE";
        public static string OFFLINE = "OFFLINE";
    }

    /// <summary>
    /// 工位样品状态定义
    /// </summary>
    public class PosKey
    {
        public static string Empty = "空工位";
        public static string WaitSample = "待入样";
        public static string Sampled = "已入样";
        public static string SampledBeFull = "已装满";
        public static string SampleFilling = "正在装料";
        public static string Timed = "已到时";
        public static string Disabled = "已禁用";
        public static string Assigned = "待出样";
    }

    /// <summary>
    /// 编辑模式
    /// </summary>
    public enum EditMode
    {
        /// <summary>
        /// 新建
        /// </summary>
        AddNew,
        /// <summary>
        /// 修改
        /// </summary>
        Modify,
        /// <summary>
        /// 批量修改
        /// </summary>
        BatchModify,
    }

    /// <summary>
    /// IP地址类型
    /// </summary>
    public enum IP_TYPE
    {
        IPV4,
        IPV6,
        IP_Any
    }

    
}
