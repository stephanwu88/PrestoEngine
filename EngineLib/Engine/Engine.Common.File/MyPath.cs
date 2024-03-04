using System;

namespace Engine.Files
{
    public class MyPath
    {
        /// <summary>
        /// 数据库操作日志
        /// </summary>
        public static string DbLog = Environment.CurrentDirectory + @"\DataBase";
        /// <summary>
        /// 调试跟踪日志
        /// </summary>
        public static string TraceLog = Environment.CurrentDirectory + @"\Trace";
        /// <summary>
        /// 运行日志
        /// </summary>
        public static string RunLog = Environment.CurrentDirectory + @"\Log";
        /// <summary>
        /// 操作日志
        /// </summary>
        public static string OpLog = Environment.CurrentDirectory + @"\Operate";
        /// <summary>
        /// 升级文件目录
        /// </summary>
        public static string UpgradePath = Environment.CurrentDirectory + @"\Upgrate";
    }
}
