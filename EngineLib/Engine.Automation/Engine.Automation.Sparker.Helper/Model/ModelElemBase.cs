using Engine.Data;
using Engine.Data.DBFAC;

namespace Engine.Automation.Sparker
{
    [Table(Name = "ana_spec_elembase", Comments = "分析方法基础元素表")]
    public class ModelElemBase : NotifyObject
    {
        [Column(Name = "ID", AI = true, PK = true, Comments = "索引")]
        public string ID { get; set; }

        [Column(Name = "PgmToken", Comments = "分析曲线主键")]
        public string PgmToken { get; set; }

        [Column(Name = "Element", Comments = "元素")]
        public string Element { get; set; }

        [Column(Name = "ElementType", Comments = "元素类型")]
        public string ElementType { get; set; }

        [Column(Name = "MassRangeU", Comments = "含量范围上限")]
        public string MassRangeU { get; set; }

        [Column(Name = "MassRangeD", Comments = "含量范围下限")]
        public string MassRangeD { get; set; }

        [Column(Name = "DecimalDigits", Comments = "修约位数")]
        public string DecimalDigits { get; set; }

        [Column(Name = "DataFormat", Comments = "数据显示格式")]
        public string DataFormat { get; set; }

        [Column(Name = "MassCalcuteExpress", Comments = "含量计算表达式")]
        public string MassCalcuteExpress { get; set; }

        [Column(Name = "Unit", Comments = "单位")]
        public string Unit { get; set; }

        [Column(Name = "OrderID", Comments = "排序号")]
        public string OrderID
        {
            get { return _OrderID; }
            set
            {
                if (!string.IsNullOrEmpty(_OrderID) && _OrderID != value)
                    IsModified = true;
                _OrderID = value;
                RaisePropertyChanged();
            }
        }
        private string _OrderID;

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
        /// 数据被选择
        /// </summary>
        public bool IsChecked
        {
            get { return _IsChecked; }
            set { _IsChecked = value; RaisePropertyChanged(); }
        }
        private bool _IsChecked;
    }
}
