using Engine.Data.DBFAC;

namespace Engine.Automation.OBLF
{
    [Table(Name = "import_material", Comments = "牌号导入接口表")]
    public class ModelImportMaterial
    {
        [Column(Name = "ID", AI = true, PK = true, Comments = "索引")]
        public string ID { get; set; }

        [Column(Name = "Material", Comments = "牌号名称")]
        public string Material { get; set; }

        [Column(Name = "Comment", Comments = "备注")]
        public string Comment { get; set; }

        [Column(Name = "MaterialToken", Comments = "牌号主键")]
        public string MaterialToken { get; set; }

        [Column(Name = "Element", Comments = "元素名称")]
        public string Element { get; set; }

        [Column(Name = "TolMethod", Comments = "偏差修正方法")]
        public string TolMethod { get; set; }

        [Column(Name = "LimitMin", Comments = "下偏差")]
        public string LimitMin { get; set; }

        [Column(Name = "LimitMax", Comments = "上偏差")]
        public string LimitMax { get; set; }

        [Column(Name = "T1_Name", Comments = "1.控样 名称")]
        public string T1_Name { get; set; }

        [Column(Name = "T2_Name", Comments = "2.控样 名称")]
        public string T2_Name { get; set; }

        [Column(Name = "CS_Name", Comments = "检查样 名称")]
        public string CS_Name { get; set; }

        [Column(Name = "CS_Tol", Comments = "检查样 偏差")]
        public string CS_Tol { get; set; }
    }
}
