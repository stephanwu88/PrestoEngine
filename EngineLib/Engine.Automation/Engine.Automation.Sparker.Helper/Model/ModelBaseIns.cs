using Engine.Common;
using Engine.Data.DBFAC;

namespace Engine.Automation.Sparker
{
    [Table(Name = "ana_base_ins", Comments = "仪器名称表")]
    public class ModelBaseIns
    {
        [Column(Name = "ID", AI = true, PK = true, Comments = "索引")]
        public string ID { get; set; }

        [Column(Name = "InsName", Comments = "仪器名称  OBLF1")]
        [StringRule(MinLen = 1, MaxLen = 20, ErrorMessage = "仪器名称不正确,字符长1-40")]
        public string InsName { get; set; }

        [Column(Name = "BindAppName", Comments = "助手的App名称 负责更新的App")]
        [StringRule(MinLen = 1, MaxLen = 30, ErrorMessage = "绑定管理终端名称不正确,长度1-30")]
        public string BindAppName { get; set; }

        [Column(Name = "Provider", Comments = "类库提供者")]
        [StringRule(MinLen = 1, MaxLen = 40, ErrorMessage = "未指定仪器程序数据提供类")]
        public string Provider { get; set; }

        [Column(Name = "Comment", Comments = "描述")]
        [StringRule(MinLen = 0, MaxLen = 40, ErrorMessage = "描述内容文字超限")]
        public string Comment { get; set; }
    }
}
