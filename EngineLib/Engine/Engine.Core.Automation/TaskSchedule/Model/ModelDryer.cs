using Engine.Common;
using Engine.Data;
using Engine.Data.DBFAC;

namespace Engine.Core.TaskSchedule
{
    /// <summary>
    /// 烘箱数据模型
    /// </summary>
    [Table(Name = "location_dryer", Comments = "烘箱")]
    public class ModelDryer : EntityBase
    {
        [Column(Name = "ID", AI = true, PK = true, Comments = "索引")]
        public string ID { get => TryGet("ID").ToMyString(); set { TrySet("ID", value); } }

        [Column(Name = "Token", Comments = "")]
        public string Token { get => TryGet("Token").ToMyString(); set { TrySet("Token", value); } }

        [Column(Name = "GroupKey", Comments = "烘箱名称")]
        public string GroupKey { get => TryGet("GroupKey").ToMyString(); set { TrySet("GroupKey", value); } }

        [Column(Name = "PosID", Comments = "烘箱工位号")]
        public string PosID { get => TryGet("PosID").ToMyString(); set { TrySet("PosID", value); } }

        [Column(Name = "PosEN", Comments = "")]
        public string PosEN { get => TryGet("PosEN").ToMyString(); set { TrySet("PosEN", value); } }

        [Column(Name = "PosKey", Comments = "工位标记")]
        public string PosKey { get => TryGet("PosKey").ToMyString(); set { TrySet("PosKey", value); } }

        [Column(Name = "MarkKey", Comments = "")]
        public string MarkKey { get => TryGet("MarkKey").ToMyString(); set { TrySet("MarkKey", value); } }

        [Column(Name = "SampleID", Comments = "")]
        public string SampleID { get => TryGet("SampleID").ToMyString(); set { TrySet("SampleID", value); } }

        [Column(Name = "SampleName", Comments = "")]
        public string SampleName { get => TryGet("SampleName").ToMyString(); set { TrySet("SampleName", value); } }

        [Column(Name = "SampleType", Comments = "")]
        public virtual string SampleType { get => TryGet("SampleType").ToMyString(); set { TrySet("SampleType", value); } }

        [Column(Name = "SampleMsg", Comments = "")]
        public string SampleMsg { get => TryGet("SampleMsg").ToMyString(); set { TrySet("SampleMsg", value); } }

        [Column(Name = "PosState", Comments = "")]
        public string PosState { get => TryGet("PosState").ToMyString(); set { TrySet("PosState", value); } }

        [Column(Name = "PrepMode", Comments = "")]
        public virtual string PrepMode { get => TryGet("PrepMode").ToMyString(); set { TrySet("PrepMode", value); } }

        [Column(Name = "MaterLocation", Comments = "")]
        public virtual string MaterLocation { get => TryGet("MaterLocation").ToMyString(); set { TrySet("MaterLocation", value); } }

        [Column(Name = "InjectTime", Comments = "")]
        public string InjectTime { get => TryGet("InjectTime").ToMyString(); set { TrySet("InjectTime", value); } }

        [Column(Name = "DueTime", Comments = "")]
        public string DueTime { get => TryGet("DueTime").ToMyString(); set { TrySet("DueTime", value); } }

        [Column(Name = "RunTime_H", Comments = "")]
        public string RunTime_H { get => TryGet("RunTime_H").ToMyString(); set { TrySet("RunTime_H", value); UpdRunTime(); } }

        [Column(Name = "RunTime_M", Comments = "")]
        public string RunTime_M { get => TryGet("RunTime_M").ToMyString(); set { TrySet("RunTime_M", value); UpdRunTime(); } }

        [Column(Name = "RunTime_S", Comments = "")]
        public string RunTime_S { get => TryGet("RunTime_S").ToMyString(); set { TrySet("RunTime_S", value); UpdRunTime(); } }

        [Column(Name = "ObjectTime_H", Comments = "")]
        public string ObjectTime_H { get => TryGet("ObjectTime_H").ToMyString(); set { TrySet("ObjectTime_H", value); UpdObjectTime(); } }

        [Column(Name = "ObjectTime_M", Comments = "")]
        public string ObjectTime_M { get => TryGet("ObjectTime_M").ToMyString(); set { TrySet("ObjectTime_M", value); UpdObjectTime(); } }

        [Column(Name = "ObjectTime_S", Comments = "")]
        public string ObjectTime_S { get => TryGet("ObjectTime_S").ToMyString(); set { TrySet("ObjectTime_S", value); UpdObjectTime(); } }

        [Column(Name = "TareWeight", Comments = "")]
        public string TareWeight { get => TryGet("TareWeight").ToMyString(); set { TrySet("TareWeight", value); } }

        [Column(Name = "InitWeight", Comments = "")]
        public string InitWeight { get => TryGet("InitWeight").ToMyString(); set { TrySet("InitWeight", value); } }

        [Column(Name = "LastWeight", Comments = "")]
        public string LastWeight { get => TryGet("LastWeight").ToMyString(); set { TrySet("LastWeight", value); } }

        [Column(Name = "NoChangeTimes", Comments = "")]
        public string NoChangeTimes { get => TryGet("NoChangeTimes").ToMyString(); set { TrySet("NoChangeTimes", value); } }

        protected virtual void UpdRunTime()
        {

        }
        protected virtual void UpdObjectTime()
        {

        }
    }

    public class ViewDryer : ModelDryer
    {

        private string _PrepMode;
        public override string PrepMode
        {
            get => _PrepMode;
            set
            {
                if (value != _PrepMode)
                {
                    switch (value)
                    {
                        case "0":
                            _PrepMode = "复称模式";
                            break;
                        case "1":
                            _PrepMode = "首称模式";
                            break;
                        case "2":
                            _PrepMode = "预烘模式";
                            break;
                    }
                    RaisePropertyChanged("PrepMode");
                }
            }
        }

        private string _SampleType;
        public override string SampleType
        {
            get => _SampleType;
            set
            {
                if (value != _SampleType)
                {
                    switch (value)
                    {
                        case "CFY":
                            _SampleType = "成分样";
                            break;
                        case "SFY":
                            _SampleType = "水分样";
                            break;
                        case "LDY":
                            _SampleType = "粒度样";
                            break;
                        case "WASH":
                            _SampleType = "洗机样";
                            break;
                        case "CFY_WASH":
                            _SampleType = "成分料洗样";
                            break;
                    }
                    RaisePropertyChanged("SampleType");
                }
            }
        }

        public string RunTime { get; set; }
        public string ObjectTime { get; set; }

        protected override void UpdRunTime()
        {
            string strRunTime = string.Empty;
            if (!string.IsNullOrEmpty(RunTime_H))
                strRunTime += RunTime_H + "时";
            if (!string.IsNullOrEmpty(RunTime_M))
                strRunTime += RunTime_M + "分";
            if (!string.IsNullOrEmpty(RunTime_S))
                strRunTime += RunTime_S + "秒";
            RunTime = strRunTime;
            RaisePropertyChanged("RunTime");
        }
        protected override void UpdObjectTime()
        {
            string strObjectTime = string.Empty;
            if (!string.IsNullOrEmpty(ObjectTime_H))
                strObjectTime += ObjectTime_H + "时";
            if (!string.IsNullOrEmpty(ObjectTime_M))
                strObjectTime += ObjectTime_M + "分";
            if (!string.IsNullOrEmpty(ObjectTime_S))
                strObjectTime += ObjectTime_S + "秒";
            ObjectTime = strObjectTime;
            RaisePropertyChanged("ObjectTime");
        }
    }
         
}
