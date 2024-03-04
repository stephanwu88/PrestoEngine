using Engine.Data.DBFAC;

namespace Engine.Automation.OBLF
{
    [Table(Name = "import_proben", Comments = "导入控样接口表")]
    public class ModelImportProben
    {
        [Column(Name = "ID", AI = true, PK = true, Comments = "索引")]
        public string ID { get; set; }

        [Column(Name = "MainToken", Comments = "控样索引")]
        public string MainToken { get; set; }

        [Column(Name = "ProbenToken", Comments = "控样分类索引")]
        public string ProbenToken { get; set; }

        [Column(Name = "PgmToken", Comments = "分析程序索引")]
        public string PgmToken { get; set; }

        [Column(Name = "InsName", Comments = "仪器名称")]
        public string InsName { get; set; }

        [Column(Name = "Name", Comments = "控样名称")]
        public string Name { get; set; }

        [Column(Name = "Type", Comments = "样品类型")]
        public string Type { get; set; }

        [Column(Name = "Comment", Comments = "样品描述")]
        public string Comment { get; set; }

        [Column(Name = "Element", Comments = "元素名称")]
        public string Element { get; set; }

        [Column(Name = "SetPoint", Comments = "标准值")]
        public string SetPoint { get; set; }

        [Column(Name = "StartValue", Comments = "首次值")]
        public string StartValue { get; set; }

        [Column(Name = "ActualValue", Comments = "分析值")]
        public string ActualValue { get; set; }

        [Column(Name = "TolStart", Comments = "首次偏差")]
        public string TolStart { get; set; }

        [Column(Name = "TolActual", Comments = "偏差")]
        public string TolActual { get; set; }

        [Column(Name = "AdditiveValue", Comments = "加法参数")]
        public string AdditiveValue { get; set; }

        [Column(Name = "MultiplitiveValue", Comments = "乘法参数")]
        public string MultiplitiveValue { get; set; }

        [Column(Name = "HandleKey", Comments = "处理标记 ImportAnaValue:表示需要导入控样分析值")]
        public string HandleKey { get; set; }
    }
}
