using Engine.Data;
using Engine.Data.DBFAC;
using System.Collections.Generic;

namespace Engine.ComDriver
{
    [Table(Name = "data_result_spectrum", Comments = "光谱仪分析结果表", Token = "2023041100930")]
    public class ModelOESData : EntityEF
    {
        [Column(Name = "ID", PK = true, AI = true, Comments = "位置")]
        public string ID { get; set; }

        [DictField(Name = "Matrix", Comments = "基体")]
        public string Matrix { get; set; }

        [DictField(Name = "AnaPgmName", Comments = "分析曲线")]
        public string AnaPgmName { get; set; }

        [DictField(Name = "SampleID", Comments = "样品编码")]
        public string SampleID { get; set; }

        [DictField(Name = "SampleCategory", Comments = "样品大类 钢样/铁样")]
        public string SampleCategory { get; set; }

        [DictField(Name = "SampleType", Comments = "PS CS TS RS")]
        public string SampleType { get; set; }

        [DictField(Name = "BurnerID", Comments = "炉号/窑号")]
        public string BurnerID { get; set; }

        [DictField(Name = "SteelType", Comments = "钢种")]
        public string SteelType { get; set; }

        [DictField(Name = "MatrialGrade", Comments = "牌号")]
        public string MatrialGrade { get; set; }

        [DictField(Name = "StationID", Comments = "工位号")]
        public string StationID { get; set; }

        [DictField(Name = "OrderNum", Comments = "取样序号")]
        public string OrderNum { get; set; }

        [DictField(Name = "AnalyEquip", Comments = "分析设备")]
        public string AnalyEquip { get; set; }

        [DictField(Name = "DataGroup", Comments = "数据分组 SD AVG  单点 平均")]
        public string DataGroup { get; set; }

        [DictField(Name = "DataSource", Comments = "数据源")]
        public string DataSource { get; set; }

        [DictField(Name = "MsgFlag", Comments = "信息标记")]
        public string MsgFlag { get; set; }

        [DictField(Name = "MsgText", Comments = "数据信息")]
        public string MsgText { get; set; }

        [DictField(Name = "RecTime", Comments = "记录时间")]
        public string RecTime { get; set; }

        [DictField(Name = "AnaEndTime", Comments = "分析数据时间")]
        public string AnaEndTime { get; set; }

        [DictField(Name = "UploadTime", Comments = "上传时间")]
        public string UploadTime { get; set; }

        public ModelOESData()
        {

        }

        public ModelOESData(string tableName)
        {
            TableName = tableName;
        }
    }

    /// <summary>
    /// 分析样
    /// </summary>
    public class OblfAnaSample
    {
        public OblfSampleMain SampleMain { get; set; } = new OblfSampleMain();
        public List<OblfAnaElement> AnaElementList { get; set; } = new List<OblfAnaElement>();
    }

    /// <summary>
    /// 样品信息
    /// </summary>
    public class OblfSampleMain
    {
        [Column(Name = "ID", PK = true, AI = true, Comments = "位置")]
        public string ID { get; set; }

        [DictField(Name = "SampleID", Comments = "样品编码")]
        public string SampleID { get; set; }

        [DictField(Name = "SampleCategory", Comments = "样品大类 钢样/铁样")]
        public string SampleCategory { get; set; }

        [DictField(Name = "SampleType", Comments = "PS CS TS RS")]
        public string SampleType { get; set; }

        [DictField(Name = "BurnerID", Comments = "炉号/窑号")]
        public string BurnerID { get; set; }

        [DictField(Name = "SteelType", Comments = "钢种")]
        public string SteelType { get; set; }

        [DictField(Name = "MatrialGrade", Comments = "牌号")]
        public string MatrialGrade { get; set; }

        [DictField(Name = "StationID", Comments = "工位号")]
        public string StationID { get; set; }

        [DictField(Name = "AnalyEquip", Comments = "分析设备")]
        public string AnalyEquip { get; set; }

        [DictField(Name = "DataGroup", Comments = "数据分组 SD AVG  单点 平均")]
        public string DataGroup { get; set; }

        [DictField(Name = "DataSource", Comments = "数据源")]
        public string DataSource { get; set; }

        [DictField(Name = "MsgFlag", Comments = "信息标记")]
        public string MsgFlag { get; set; }

        [DictField(Name = "MsgText", Comments = "数据信息")]
        public string MsgText { get; set; }

        [DictField(Name = "RecTime", Comments = "记录时间")]
        public string RecTime { get; set; }

        [DictField(Name = "AnaEndTime", Comments = "分析数据时间")]
        public string AnaEndTime { get; set; }

        [DictField(Name = "UploadTime", Comments = "上传时间")]
        public string UploadTime { get; set; }
    }

    /// <summary>
    /// 样品分析元素结构
    /// </summary>
    public class OblfAnaElement
    {
        public string Name { get; set; }
        public string Sign { get; set; }
        public string Value { get; set; }
    }
}
