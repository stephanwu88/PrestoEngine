using Engine.Common;
using Engine.MVVM.Messaging;
using System;
using System.Collections.Generic;
using System.Timers;
using System.Linq;
using Engine.Mod;
using System.Threading.Tasks;

namespace Engine.ComDriver
{
    /// <summary>
    /// 通讯任务项
    /// </summary>
    public class ComItem
    {
        /// <summary>
        /// 关联工位 - 方便日志
        /// </summary>
        public string RelatedGroup = "";
        /// <summary>
        /// 通讯指令
        /// </summary>
        public string Command = "";
        public object[] Param;
        /// <summary>
        /// 监管开始时间
        /// </summary>
        public string StartTime = "";
        /// <summary>
        /// 超时时间 单位：S
        /// </summary>
        public double OverTime;
        /// <summary>
        /// 累计时间
        /// </summary>
        public double AccTime;
        /// <summary>
        /// 是否超时
        /// </summary>
        public bool IsOverTime;
        /// <summary>
        /// 重试计数
        /// </summary>
        public int RetryCount;
        /// <summary>
        /// 通讯交互结果
        /// </summary>
        public string Result { get; set; } = "";
        /// <summary>
        /// 通讯交互结果-信息
        /// </summary>
        public string ResultText { get; set; } = "";
    }

    /// <summary>
    /// 通讯任务管理
    /// </summary>
    public partial class ComTask
    {
        public Dictionary<string, ComTask> DicTask { get; private set; } = new Dictionary<string, ComTask>();
        private List<ComItem> LstCom = new List<ComItem>();
        private Timer timer;
        /// <summary>
        /// 通讯超时的重试次数
        /// </summary>
        public int RetryCount { get; set; } = 1;
        public bool IgnoreOverTime;
        public static readonly string ComNodeOverTime = SystemDefault.UUID;

        public ComTask()
        {
            timer = new Timer();
            timer.Interval = 1000;  //设置定时器间隔为1秒
            timer.Elapsed += (s, e) =>
            {
                foreach (ComItem item in LstCom)
                {
                    if (string.IsNullOrEmpty(item.StartTime) || item.OverTime == 0.00)
                        continue;
                    TimeSpan timeSinceModified = DateTime.Now - item.StartTime.ToMyDateTime();
                    if (timeSinceModified.TotalSeconds >= item.OverTime)
                    {
                        item.IsOverTime = true;
                        item.Result = "OverTime";
                        Messenger.Default.Send(item, ComNodeOverTime);
                    }
                }
                if (LstCom.Count == 0)
                    timer.Stop();
            };
        }

        /// <summary>
        /// 执行任务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="SourceObject"></param>
        public async Task<CallResult> StartTask<T>(T SourceObject)
        {
            CallResult FinalResult = new CallResult();
            if (LstCom.Count() > 0)
            {
                FinalResult.Success = true;
                FinalResult.Result = "Finished";
            }
            await Task.Run(() =>
            {
                while (LstCom.Count() > 0)
                {
                    ComItem comItem = LstCom.MySelectAny(0);
                    if (comItem != null)
                    {
                        if (string.IsNullOrEmpty(comItem?.Result))
                        {
                            CallResult result = sCommon.SyncInvokeMethod<T>(SourceObject, comItem.Command, comItem.Param);
                            if (result.Fail)
                            {
                                string Error = string.Format("【Command发送错误】【{0}】{1}:{2}",
                                                comItem.RelatedGroup.ToMyString(), comItem.Command.ToMyString(),
                                                result.Result.ToMyString());
                                Logger.Task.Write(LOG_TYPE.ERROR, Error);
                                comItem.ResultText = result.Result.ToMyString();
                                FinalResult.Result = result.Result;
                            }
                            else
                            {
                                comItem.Result = "Sended";
                                comItem.StartTime = SystemDefault.StringTimeNow;
                                string strSendSuffix = comItem.RetryCount > 0 ? $",第{comItem.RetryCount}次重试" : "";
                                string strError = $"【Command发送成功{strSendSuffix}】【{comItem.RelatedGroup.ToMyString()}】{comItem.Command.ToMyString()}";
                                Logger.Task.Write(LOG_TYPE.MESS, strError);
                            }
                        }
                        else
                        {
                            if (comItem?.Result == "Fail")
                            {
                                if (string.IsNullOrEmpty(comItem.ResultText)) comItem.ResultText = "通讯流程失败，原因未知";
                                FinalResult.Result = comItem.ResultText;
                                FinalResult.Success = false;
                                LstCom.Clear();
                                Logger.Task.Write(LOG_TYPE.ERROR, comItem.ResultText);
                            }
                            else if (comItem?.Result == "Success")
                            {
                                if (string.IsNullOrEmpty(comItem.ResultText)) comItem.ResultText = "通讯会话成功";
                                FinalResult.Result = comItem.ResultText;
                                LstCom.RemoveAt(0);
                                Logger.Task.Write(LOG_TYPE.MESS, comItem.ResultText);
                            }
                            else if (comItem?.Result == "OverTime")
                            {
                                comItem.RetryCount++;
                                if ((comItem.OverTime >= RetryCount && RetryCount > 0) || RetryCount <= 0)
                                {
                                    //超过重试次数，确定失败
                                    FinalResult.Success = false;
                                    FinalResult.Result = comItem.ResultText = "指令超时未响应";
                                    LstCom.Clear();
                                    Logger.Task.Write(LOG_TYPE.ERROR, comItem.ResultText);
                                }
                                else if (comItem.OverTime < RetryCount && RetryCount > 0)
                                {
                                    comItem.Result = "";
                                    comItem.ResultText = "超时重试";
                                }
                            }
                        }
                    }
                    System.Threading.Thread.Sleep(100);
                }
            });
            return FinalResult;
        }

        /// <summary>
        /// 添加任务
        /// </summary>
        /// <param name="com"></param>
        public void Put(ComItem com)
        {
            LstCom.AppandList(com);
            if (!timer.Enabled && com.OverTime > 0.0)
            {
                timer.Start(); // 启动定时器
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="comTask"></param>
        public void AddTo(string name, ComTask comTask)
        {
            DicTask.AppandDict(name,comTask);
        }

        /// <summary>
        /// 添加通讯结果
        /// </summary>
        /// <param name="CommandName"></param>
        /// <param name="Result"></param>
        /// <param name="ResultText"></param>
        public void AppandComResult(string CommandName,string Result,string ResultText)
        {
            ComItem com =  LstCom.MySelectFirst(x => x.Command == CommandName);
            if (com != null)
            {
                com.Result = Result;
                com.ResultText = ResultText;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public ComItem GetByCommand(string name)
        {
            return LstCom.MySelectFirst(x => x.Command == name);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ComItem GetFirstAwait()
        {
            return LstCom.MySelectAny(0);
        }

        /// <summary>
        /// 属性索引器
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public ComTask this[string name]
        {
            get
            {
                if (!DicTask.ContainsKey(name))
                    DicTask.Add(name, new ComTask());
                return DicTask.DictFieldValue(name);
            }
        }
    }
}
