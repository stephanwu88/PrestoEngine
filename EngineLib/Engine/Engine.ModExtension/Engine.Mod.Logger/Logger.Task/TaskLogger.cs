using System;
using System.Runtime.CompilerServices;
using Engine.Common;
using Engine.Data.DBFAC;

namespace Engine.Mod
{
    /// <summary>
    /// 任务日志
    /// </summary>
    public class TaskLogger
    {
        private IDBFactory<ServerNode> _DB = DbFactory.Task.CloneInstance("Logger");

        private TaskLogger() { }

        private static TaskLogger _Default;

        /// <summary>
        /// 静态实例
        /// </summary>
        public static TaskLogger Default
        {
            get
            {
                if (_Default == null)
                    _Default = new TaskLogger();
                return _Default;
            }
        }

        /// <summary>
        /// 任务记录
        /// </summary>
        /// <param name="LogType"></param>
        /// <param name="SampleLabel"></param>
        /// <param name="MsgObject"></param>
        /// <param name="MsgText"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public bool Write(LOG_TYPE LogType, string SampleLabel, string MsgObject, string MsgText,
            string Custom1="",string Custom2="")
        {
            try
            {
                if (string.IsNullOrEmpty(SampleLabel))
                    return false;
                ModelTaskLogger model = new ModelTaskLogger()
                {
                    SampleLabel = SampleLabel,
                    SampleID = SampleLabel.MidString("",SystemDefault.LinkSign),
                    MsgDir = "发送",
                    MsgType = LogType.FetchDescription(),
                    MsgObject = MsgObject,
                    MsgText = MsgText,
                    RecTime = SystemDefault.StringTimeNow,
                    Custom1 = string.IsNullOrEmpty(Custom1) ? SystemDefault.StringEmpty : Custom1,
                    Custom2 = string.IsNullOrEmpty(Custom2) ? SystemDefault.StringEmpty : Custom2,
                };
                return _DB.ExcuteInsert(model).Success;
            }
            catch (Exception )
            {
                return false;
            }
        }
    }
}