using Engine.Automation.Sparker;
using Engine.Data.DBFAC;

namespace Engine.Automation.OBLF
{
    [Table(Name = "local_spec_pgm", Comments = "仪器分析程序")]
    public class ModelLocalSpecPgm : ModelBasicSpecPgm
    {
        [Column(Name = "Install", Comments = "安装与否")]
        public int Install { get; set; } = SystemDefault.InValidInt;

        [Column(Name = "PgmNr", Comments = "OBLF数据库中的程序项的索引")]
        private string PgmNr { get; set; }
    }

}
