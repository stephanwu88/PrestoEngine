using Engine.Data;
using Engine.Data.DBFAC;

namespace Engine.ComDriver
{
    [Table(Name = "data_result_spectrum", Comments = "布鲁克荧光仪分析结果表", Token = "2023091900754")]
    public class ModelXRFData : EntityEF
    {
        [Column(Name = "ID", PK = true, AI = true, Comments = "索引")]
        public string ID { get; set; }

        [DictField(Name = "SampleCategory", Comments = "样品大类")]
        public string SampleCategory { get; set; }

        [DictField(Name = "DataGroup", Comments = "数据组")]
        public string DataGroup { get; set; }

        [DictField(Name = "AnaProgram", Comments = "校准方法")]
        public string AnaProgram { get; set; }

        [DictField(Name = "SampleName", Comments = "样品名称")]
        public string SampleName { get; set; }

        [DictField(Name = "AnaMode", Comments = "分析模式  UN:例行分析 RE: 校准分析")]
        public string AnaMode { get; set; }

        [DictField(Name = "TotalAnaTime", Comments = "分析时长")]
        public string TotalAnaTime { get; set; }

        [DictField(Name = "AnaEndTime", Comments = "测量完成时间")]
        public string AnaEndTime { get; set; }

        [DictField(Name = "RecTime", Comments = "记录时间")]
        public string RecTime { get; set; } = SystemDefault.StringTimeNow;

        [DictField(Name = "ResultState", Comments = "分析结果状态")]
        public string ResultState { get; set; }

        [DictField(Name = "ResultErrorText", Comments = "结果错误内容")]
        public string ResultErrorText { get; set; }

        public ModelXRFData()
        {

        }

        public ModelXRFData(string tableName = "")
        {
            TableName = tableName;
        }
    }
}
