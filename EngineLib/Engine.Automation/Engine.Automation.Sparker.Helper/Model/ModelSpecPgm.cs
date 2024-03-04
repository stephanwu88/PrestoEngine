using Engine.Common;
using Engine.Data.DBFAC;

namespace Engine.Automation.Sparker
{
    /// <summary>
    /// 仪器分析程序
    /// </summary>
    public abstract class ModelBasicSpecPgm : ValidateError<ModelBasicSpecPgm>
    {
        [Column(Name = "ID", AI = true, PK = true, Comments = "索引")]
        public string ID { get; set; }

        [Column(Name = "InsName", Comments = "仪器名称")]
        public string InsName { get; set; }

        [Column(Name = "Matrix", Comments = "基体")]
        [StringRule(MinLen = 1, MaxLen = 10, ErrorMessage = "长度1-10")]
        public string Matrix { get; set; }

        [Column(Name = "PgmName", Comments = "分析程序名称")]
        [StringRule(MinLen = 1, MaxLen = 40, ErrorMessage = "长度1-40")]
        public string PgmName { get; set; }

        [Column(Name = "PgmNameLang", Comments = "分析程序名称-中文")]
        [StringRule(MinLen = 0, MaxLen = 100, ErrorMessage = "长度0-100")]
        public string PgmNameLang { get; set; }
    }

    [Table(Name = "ana_spec_pgm", Comments = "仪器分析程序")]
    public class ModelSpecPgm : ModelBasicSpecPgm
    {
        [Column(Name = "Token", Comments = "程序索引")]
        public string Token { get; set; }

        [Column(Name = "OrderID", Comments = "排序号")]
        public string OrderID { get; set; }

        public override string ToString()
        {
            string strPgmNameLang = string.Empty;
            //if (!string.IsNullOrEmpty(PgmNameLang))
            //    strPgmNameLang = $": {PgmNameLang}";
            return $"{PgmName}{strPgmNameLang}";
        }
    }
}
