using Engine.Data.DBFAC;

namespace Engine.Automation.OBLF
{
    [Table(Name = "local_material_main", Comments = "材质主信息")]
    public class ModelLocalMaterialMain
    {
        [Column(Name = "ID", AI = true, PK = true, Comments = "索引")]
        public string ID { get; set; }

        [Column(Name = "InsName", Comments = "仪器名称")]
        public string InsName { get; set; }

        [Column(Name = "Material", Comments = "牌号代码")]
        public string Material { get; set; }

        [Column(Name = "Matrix", Comments = "基体索引")]
        public string Matrix { get; set; }

        [Column(Name = "PgmName", Comments = "程序名称")]
        public string PgmName { get; set; }

        public string HandFlag { get; set; }
    }

    [Table(Name = "local_material_elem", ViewName = "view_local_material_elem", Comments = "材质主信息")]
    public class ModelLocalMaterialElem
    {
        [Column(Name = "ID", AI = true, PK = true, Comments = "索引")]
        public string ID { get; set; }

        [Column(Name = "InsName", Comments = "仪器名称")]
        public string InsName { get; set; }

        [Column(Name = "Material", Comments = "牌号代码")]
        public string Material { get; set; }

        [Column(Name = "ElemCount", Comments = "关联控样数量")]
        public string ElemCount { get; set; }

        [Column(Name = "Element", Comments = "元素名称")]
        public string Element { get; set; }

        [Column(Name = "Type1stID", Comments = "1.控样ID")]
        public string Type1stID { get; set; }

        [Column(Name = "Type2stID", Comments = "2.控样ID")]
        public string Type2stID { get; set; }

        public string Type1st { get; set; }
        public string T1_SetPoint { get; set; }
        public string T1_StartValue { get; set; }
        public string T1_ActualValue{ get; set; }
        public string T1_MaxTolToStart { get; set; }
        public string T1_Additive { get; set; }
        public string Type2st { get; set; }
        public string T2_SetPoint { get; set; }
        public string T2_StartValue { get; set; }
        public string T2_ActualValue { get; set; }
        public string T2_MaxTolToStart { get; set; }
        public string T2_Additive { get; set; }
    }
}
