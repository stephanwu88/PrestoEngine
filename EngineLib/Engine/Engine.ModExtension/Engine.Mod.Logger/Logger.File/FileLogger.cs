using Engine.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

namespace Engine.Mod
{
    /// <summary>
    /// 文件日志
    /// </summary>
    public class FileLogger
    {
        #region 成员变量
        private string _LogFileName;
        private string _LogPath = SystemDefault.LogPath;
        private static Dictionary<string, FileLogger> _DicLogger = new Dictionary<string, FileLogger>();
        public event Action<object, LOG_TYPE, string> LogWritten;
        #endregion

        #region 属性、静态实例
        /// <summary>
        /// 调试跟踪日志
        /// </summary>
        public static FileLogger Default => GetInstance(LogPathKey.Default);

        /// <summary>
        /// 通讯控制器
        /// </summary>
        public static FileLogger CommBody => GetInstance(LogPathKey.CommBody);

        /// <summary>
        /// 任务管理层
        /// </summary>
        public static FileLogger Task => GetInstance(LogPathKey.Task);

        /// <summary>
        /// 错误日志
        /// </summary>
        public static FileLogger Error => GetInstance(LogPathKey.Error);

        /// <summary>
        /// 数据日志
        /// </summary>
        public static FileLogger Data => GetInstance(LogPathKey.Data);

        /// <summary>
        /// 数据库操作日志
        /// </summary>
        public static FileLogger Database => GetInstance(LogPathKey.Database);

        /// <summary>
        /// 日志文件目录
        /// </summary>
        public string LogPath
        {
            get => _LogPath;
            set { _LogPath = value.FormatPath(); }
        }

        /// <summary>
        /// 日志文件名
        /// </summary>
        public string LogFileName
        {
            get => _LogFileName;
        }

        /// <summary>
        /// 实例索引
        /// </summary>
        /// <param name="LogPathKey"></param>
        /// <returns></returns>
        public FileLogger this[string LogPathKey]
        {
            get => GetInstance(LogPathKey);
        }
        #endregion

        #region 公共方法
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="LogPath"></param>
        public FileLogger(string LogPath = "")
        {
            LogPath = LogPath.FormatPath();
            if (string.IsNullOrEmpty(LogPath) || !Directory.Exists(LogPath))
                _LogPath = SystemDefault.LogPath;
            else
                _LogPath = LogPath;
        }

        /// <summary>
        /// 添加日志实例
        /// </summary>
        /// <param name="LogPathKey"></param>
        /// <param name="LogPath"></param>
        public static void Appand(string LogPathKey, string LogPath = "")
        {
            lock (_DicLogger)
            {
                if (string.IsNullOrEmpty(LogPath) || !Directory.Exists(LogPath.FormatPath()))
                    LogPath = SystemDefault.LogPath;
                if (!_DicLogger.ContainsKey(LogPathKey))
                    _DicLogger.Add(LogPathKey, new FileLogger(LogPath));
            }
        }

        /// <summary>
        /// 添加日志实例
        /// </summary>
        /// <param name="LogItem"></param>
        public static void Appand(LogItem LogItem)
        {
            lock (_DicLogger)
            {
                if (LogItem == null)
                    return;
                string logPath = LogItem.FilePath.ToMyString().FormatPath();
                string logType = LogItem.LogType.ToMyString();
                if (string.IsNullOrEmpty(logPath) || !Directory.Exists(logPath))
                    logPath = SystemDefault.LogPath;
                if (!_DicLogger.ContainsKey(logType))
                    _DicLogger.Add(logType, new FileLogger(logPath));
            }
        }

        /// <summary>
        /// 读取txt文件
        /// </summary>
        /// <param name="txtName"></param>
        /// <returns></returns>
        public string Read(string txtName)
        {
            _LogFileName = GetLogFileFullName(txtName);
            if (Directory.Exists(_LogPath))
                return new TxtFile(_LogFileName).ReadTxt();
            else
                return string.Empty;
        }

        /// <summary>
        /// 读取日志文件加载后时间开始的Max条以内的缓冲数据
        /// </summary>
        /// <param name="txtName">日志文件名</param>
        /// <param name="StartDT">画面加载时间，加载后的记录可显示</param>
        /// <param name="Max">默认显示最后200条，为0表示没有限制</param>
        /// <returns></returns>
        public List<string> Read(string txtName, DateTime StartDT, int Max = 200)
        {
            _LogFileName = GetLogFileFullName(txtName);
            if (Directory.Exists(_LogPath))
                return new TxtFile(_LogFileName).ReadTxtFromLast(StartDT, Max);
            else
                return null;
        }

        /// <summary>
        /// 日志记录
        /// </summary>
        /// <param name="LogType"></param>
        /// <param name="LogContent"></param>
        public void Write(LOG_TYPE LogType, string LogContent)
        {
            Write(SystemDefault.LogFileName, LogType, LogContent);
        }

        /// <summary>
        /// 日志记录
        /// </summary>
        /// <param name="txtName"></param>
        /// <param name="LogType"></param>
        /// <param name="LogContent"></param>
        /// <param name="ViewLog">True时通过LogWritten事件反馈内容</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Write(string txtName, LOG_TYPE LogType, string LogContent, bool ViewLog = true)
        {
            _LogFileName = GetLogFileFullName(txtName);
            if (Directory.Exists(_LogPath))
            {
                //new TxtFile(LogPath).WriteTxt("[" + DateTime.Now.ToString() + "] | " + "[" + Enum.GetName(typeof(LOG_TYPE), LogType) + "]\t" + " | " + LogContent + "\r\n");
                string strWriteContent = string.Format("[{0}] | [{1}]\t | {2}\r\n",
                           DateTime.Now.ToString("yyyy-M-d HH:mm:ss.fff"),
                           LogType.FetchDescription(), LogContent);
                new TxtFile(_LogFileName).WriteTxt(strWriteContent);
            }
            if (LogWritten != null && ViewLog)
                LogWritten(this, LogType, LogContent);
        }
        #endregion

        #region 内部方法
        /// <summary>
        /// 获取实例
        /// </summary>
        /// <param name="LogKey"></param>
        /// <returns></returns>
        private static FileLogger GetInstance(string LogKey)
        {
            lock (_DicLogger)
            {
                if (!_DicLogger.ContainsKey(LogKey))
                    _DicLogger.Add(LogKey, new FileLogger());
                return _DicLogger[LogKey];
            }
        }

        /// <summary>
        /// 获取日志文件目录全名
        /// </summary>
        /// <param name="logfName">日志文本文件名称</param>
        /// <returns></returns>
        private string GetLogFileFullName(string logfName)
        {
            string strLogPath = string.Empty;
            string txtName = logfName.ToLower().Contains(".txt") ? logfName.Remove(logfName.Length - 4, 4) : logfName;
            if (Directory.Exists(_LogPath))
                strLogPath = _LogPath + @"\" + txtName + ("_" + DateTime.Now.ToString("yy-MM-dd")) + ".txt";
            else
                strLogPath = Directory.GetCurrentDirectory() + @"\" + txtName + ("_" + DateTime.Now.ToString("yy-MM-dd")) + ".txt";
            return strLogPath;
        }
        #endregion
    }
}
