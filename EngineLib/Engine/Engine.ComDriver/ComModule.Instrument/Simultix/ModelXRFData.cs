using Engine.Data;
using Engine.Data.DBFAC;

namespace Engine.ComDriver.Simultix
{
    /// <summary>
    /// 理学文件结果
    /// </summary>
    [Table(Name = "data_result_spectrum", Comments = "理学荧光数据结果表", Token ="202301131328")]
    public class ModelXRFData : EntityEF
    {
        [Column(Name = "ID", PK = true, AI = true, Comments = "索引")]
        public string ID { get; set; }

        [DictField(Name = "Pos", Comments = "位置")]
        public string Pos { get; set; }

        [DictField(Name = "JobName", Comments = "任务类型 ex:常规分析")]
        public string JobName { get; set; }

        [DictField(Name = "AnaProgram", Comments = "分析方法 ST GLZ")]
        public string AnaProgram { get; set; }

        [DictField(Name = "AnaCode", Comments = "分析代码 ST GLZ")]
        public string AnaCode { get; set; }

        [DictField(Name = "SampleName", Comments = "样品名称")]
        public string SampleName { get; set; }

        [DictField(Name = "AnaCountNum", Comments = "分析次数")]
        public string AnaCountNum { get; set; }

        [DictField(Name = "Calculate", Comments = "计算")]
        public string Calculate { get; set; }

        [DictField(Name = "Print", Comments = "打印  缺省")]
        public string Print { get; set; }

        [DictField(Name = "Send", Comments = "发送 No 异常")]
        public string Send { get; set; }

        [DictField(Name = "AnaEndTime", Comments = "分析数据时间")]
        public string AnaEndTime { get; set; }

        [DictField(Name = "RecTime", Comments = "记录时间")]
        public string RecTime { get; set; } = SystemDefault.StringTimeNow;

        [DictField(Name = "ResultState", Comments = "分析结果状态")]
        public string ResultState { get; set; }

        [DictField(Name = "ResultErrorText", Comments = "结果错误内容")]
        public string ResultErrorText { get; set; }

        public ModelXRFData(string tableName = "")
        {
            TableName = tableName;
        }
    }
}
