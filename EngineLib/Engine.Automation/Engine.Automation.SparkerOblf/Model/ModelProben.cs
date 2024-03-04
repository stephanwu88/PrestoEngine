using Engine.Common;
using Engine.Data.DBFAC;

namespace Engine.Automation.OBLF
{
    [Table(Name = "local_proben_main", Comments = "控样列表 - 同步OBLF数据")]
    public class ModelLocalProbenMain
    {
        [Column(Name = "ID", AI = true, PK = true, Comments = "索引")]
        public string ID { get; set; }

        [AutoIndex()]
        public string IDX { get; set; }

        [Column(Name = "InsName", Comments = "仪器名称")]
        public string InsName { get; set; }

        [Column(Name = "ProbenID", Comments = "控样 - 项索引")]
        public string ProbenID { get; set; }

        [Column(Name = "ProbenNr", Comments = "控样关联序号")]
        public string ProbenNr { get; set; }

        [Column(Name = "Name", Comments = "控样名称")]
        public string Name { get; set; }

        [Column(Name = "ProbenType", Comments = "样品类型 T: 控样")]
        public string ProbenType { get; set; }

        [Column(Name = "PgmNr", Comments = "分析程序索引")]
        public string PgmNr { get; set; }

        public string HandFlag { get; set; }
    }

    [Table(Name = "local_proben_elem", Comments = "控样元素列表 - 同步OBLF数据")]
    public class ModelLocalProbenElem
    {
        [Column(Name = "ID", AI = true, PK = true, Comments = "索引")]
        public string ID { get; set; }

        [AutoIndex()]
        public string IDX { get; set; }

        [Column(Name = "InsName", Comments = "仪器名称")]
        public string InsName { get; set; }

        [Column(Name = "ProbenID", Comments = "控样 - 项索引")]
        public string ProbenID { get; set; }

        [Column(Name = "Element", Comments = "分析元素名称")]
        public string Element { get; set; }

        [Column(Name = "SetPoint", Comments = "标准值")]
        public string SetPoint { get; set; }

        [Column(Name = "StartValue", Comments = "首次值")]
        public string StartValue { get; set; }

        [Column(Name = "ActualValue", Comments = "实际值")]
        public string ActualValue { get; set; }

        [Column(Name = "MaxTolToStart", Comments = "首次偏差")]
        public string MaxTolToStart { get; set; }

        [Column(Name = "Additive", Comments = "修正方法  0:Dynamically 动态 1:Additive 加法 2:Multiplicative 乘法")]
        public string Additive { get; set; }
    }
}
