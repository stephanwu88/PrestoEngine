using Engine.Data;
using Engine.Data.DBFAC;

namespace Engine.Automation.Sparker
{
    /// <summary>
    /// 偏差设置基类
    /// </summary>
    public class ModelDeviationBase : NotifyObject
    {
        [Column(Name = "IsEnabled")]
        public string IsEnabled { get; set; }

        [Column(Name = "GamaMode")]
        public string GamaMode { get; set; }

        [Column(Name = "SD")]
        public string SD { get; set; }

        [Column(Name = "RSD")]
        public string RSD { get; set; }

        [Column(Name = "Gama1")]
        public string Gama1 { get; set; }

        [Column(Name = "Gama2")]
        public string Gama2 { get; set; }

        [Column(Name = "Gama3")]
        public string Gama3 { get; set; }

        [Column(Name = "A")]
        public string A { get; set; }

        [Column(Name = "B")]
        public string B { get; set; }

        [Column(Name = "GamaExpress")]
        public string GamaExpress { get; set; }

        [Column(Name = "DecimalDigits")]
        public string DecimalDigits { get; set; }

        [Column(Name = "DataFormat")]
        public string DataFormat { get; set; }
    }

    [Table(Name = "ana_spec_elembase", Comments = "分析曲线偏差设置")]
    public class ModelAnaDeviation : ModelDeviationBase
    {

        [Column(Name = "PgmToken")]
        public string PgmToken { get; set; }

        [Column(Name = "Element")]
        public string Element { get; set; }

        /// <summary>
        /// 数据已修改
        /// </summary>
        public bool IsModified
        {
            get { return _IsModified; }
            set { _IsModified = value; RaisePropertyChanged(); }
        }
        private bool _IsModified;

        /// <summary>
        /// 分析曲线对象
        /// </summary>
        public ModelSpecPgm AnaPgm { get; set; }
    }

    [Table(Name = "ana_material_elem", Comments = "牌号分析偏差设置")]
    public class ModelMaterialDeviation : ModelDeviationBase
    {
        [Column(Name = "MaterialToken")]
        public string MaterialToken { get; set; }

        [Column(Name = "Element")]
        public string Element { get; set; }

        [Column(Name = "Material",ReadOnly =true)]
        public string Material { get; set; }

        /// <summary>
        /// 数据已修改
        /// </summary>
        public bool IsModified
        {
            get { return _IsModified; }
            set { _IsModified = value; RaisePropertyChanged(); }
        }
        private bool _IsModified;

        /// <summary>
        /// 牌号对象
        /// </summary>
        public ModelMaterialMain MaterialMain { get; set; }
    }
}
