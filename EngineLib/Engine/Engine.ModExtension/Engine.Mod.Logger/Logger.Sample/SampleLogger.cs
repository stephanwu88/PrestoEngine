using Engine.Common;
using Engine.Core;
using Engine.Core.TaskSchedule;
using Engine.Data.DBFAC;
using Engine.MVVM;
using Engine.MVVM.Messaging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Engine.Mod
{
    /// <summary>
    /// 样品记录
    /// </summary>
    public class SampleLogger
    {
        public static string RecordStarted = SystemDefault.UUID;
        public static string RecordFinished = SystemDefault.UUID;
        private IDBFactory<ServerNode> _DB = DbFactory.Task.CloneInstance("Logger");
        public List<ModelSampleRecord> LstSampleRecord = new List<ModelSampleRecord>();
        public static readonly SampleLogger Default = new SampleLogger();

        /// <summary>
        /// 构造函数
        /// </summary>
        private SampleLogger()
        {
            //加载近期的初始记录项
            LoadDefaultSet();
            //检测工位完成，通知 PosFinish
            Messenger.Default.Register<ModelSystemSymbol>(this, ViewModel.PosItemChanged, (PosSymbol) =>
            {
                if (PosSymbol ==null)  return;
                string PosKey = PosSymbol.GroupName;
                if (PosSymbol.Name == "POS_STATE" && PosSymbol.CurrentValue == "DONE")
                {
                    string strSampleLabel = Accessor.Current.ReadPosItem(PosKey,"VIEW_SI");
                    NotifyFinsh(strSampleLabel,PosKey,"PosFinish");
                }
            });
            //记录任务开始 结束  
            //开始时新增记录项
            //结束时通知 TaskFinish
            Messenger.Default.Register<Tuple<TaskItem, ModelSystemSymbol, string>>(this, TaskFactoryFrame.TaskDriverStarted, (TaskDriver) =>
            {
                if (TaskDriver.Item2 == null)
                    return;
                AddSampleRecord(TaskDriver.Item3.ToMyString(), TaskDriver.Item2.GroupName,TaskDriver.Item2.CustomMark);
            });
            Messenger.Default.Register<Tuple<ModelSystemSymbol, string>>(this, TaskFactoryFrame.TaskDriverFinished, (TaskDriver) =>
            {
                if (TaskDriver.Item1==null)
                    return;
                NotifyFinsh(TaskDriver.Item2.ToMyString(), TaskDriver.Item1.Name,"TaskFinish");
            });
        }

        /// <summary>
        /// 从记录表初始化加载最近需要的记录项
        /// </summary>
        private void LoadDefaultSet()
        {
            ModelSampleRecord mod = new ModelSampleRecord()
            {
                ID = "ID>0 and ifnull(length(InjectTime),0)>0 and ifnull(length(DueTime),0)=0 order by InjectTime desc limit 50".MarkExpress()
            };
            DataTable dt = _DB.ExcuteQuery(mod).Result.ToMyDataTable();
            LstSampleRecord = ColumnDef.ToEntityList<ModelSampleRecord>(dt);
        }

        /// <summary>
        /// 添加任务记录
        /// </summary>
        /// <param name="SampleLabel"></param>
        /// <param name="PosKey"></param>
        /// <param name="Custom1"></param>
        public void AddSampleRecord(string SampleLabel, string PosKey, string Custom1)
        {
            try
            {
                if (string.IsNullOrEmpty(SampleLabel) || string.IsNullOrEmpty(PosKey))
                    return;
                if (LstSampleRecord.Where(x => x.PosKey == PosKey && x.SampleLabel == SampleLabel).ToList().MyCount() > 0)
                    return;
                ModelSampleRecord Model = new ModelSampleRecord();
                Model.SampleLabel = SampleLabel;
                Model.SampleID = SampleLabel.MidString("", SystemDefault.LinkSign);
                Model.ProcKey = Custom1.MidString("(", ")");
                Model.PosKey = PosKey;
                Model.PosName = "";
                Model.InjectTime = SystemDefault.StringTimeNow;
                Model.Custom1 = Custom1;
                CallResult res = _DB.ExcuteInsert(Model);
                LstSampleRecord.Insert(0, Model);
                Messenger.Default.Send(Model, RecordStarted);
            }
            catch (Exception )
            {

            }
        }

        /// <summary>
        /// 通知制样完成  RecAndEndByTask  RecAndEndByPos
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        public void NotifyFinsh(string SampleLabel,string PosKey,string FinishKey)
        {
            try
            {
                List<ModelSampleRecord> LstUpdated = new List<ModelSampleRecord>();
                bool PosNotifyed = false;
                for (int i = 0; i < LstSampleRecord.Count; i++)
                {
                    string CustomKey = LstSampleRecord[i].Custom1.ToMyString();
                    if (LstSampleRecord[i].PosKey == PosKey)
                    {
                        bool FinishByTask = CustomKey.MyContains("RecAndEndByTask") && FinishKey == "TaskFinish";
                        bool FinishByPos = CustomKey.MyContains("RecAndEndByPos") && FinishKey == "PosFinish";
                        if (!PosNotifyed && LstSampleRecord[i].SampleLabel == SampleLabel &&(FinishByTask || FinishByPos))
                        {
                            string strDueTime = SystemDefault.StringTimeNow;
                            string strSampleLabel = LstSampleRecord[i].SampleLabel;
                            string strInjectTime = LstSampleRecord[i].InjectTime;
                            string TimeLong = strInjectTime.TimeLong(strDueTime).TotalSeconds.ToMyString();
                            LstUpdated.Add(new ModelSampleRecord()
                            {
                                SampleLabel = strSampleLabel.MarkWhere(),
                                PosKey = PosKey.MarkWhere(),
                                DueTime = strDueTime,
                                TimeLong = TimeLong
                            });
                            LstSampleRecord[i].DueTime = strDueTime;
                            LstSampleRecord[i].TimeLong = TimeLong;
                            Messenger.Default.Send(LstSampleRecord[i], RecordFinished);
                            PosNotifyed = true;
                        }
                        LstSampleRecord.Remove(LstSampleRecord[i]);
                    }
                }
                if (LstUpdated.Count > 0)
                    _DB.ExcuteUpdate(LstUpdated);

            }
            catch (Exception ex)
            {

            }
        }

        public void AddRecord()
        {

        }
    }
}
