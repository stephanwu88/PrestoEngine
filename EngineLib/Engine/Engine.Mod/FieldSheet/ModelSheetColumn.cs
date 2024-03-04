using Engine.Common;
using Engine.Data.DBFAC;
using System.Collections.Generic;

namespace Engine.Mod
{
    /// <summary>
    /// 表格列配置
    /// </summary>
    [Table(Name = "sys_sheet_column", Comments = "报表列配置")]
    public class ModelSheetColumn : ValidateError<ModelSheetColumn>
    {
        [Column(Name = "ID", PK = true, AI = true, Comments = "索引")]
        public string ID { get; set; }

        [Column(Name = "Token", Comments = "列ID")]
        public string Token { get; set; }

        [Column(Name = "IsActive", Comments = "启用")]
        public int IsActive { get; set; }

        [Column(Name = "IsEditable", Comments = "列编辑")]
        public int IsEditable { get; set; }

        [Column(Name = "IsFrozen", Comments = "列冻结")]
        public int IsFrozen { get; set; }

        [Column(Name = "GroupToken", Comments = "分组ID")]
        public string GroupToken { get; set; }

        [Column(Name = "GroupName", Comments = "分组名称")]
        public string GroupName { get; set; }

        [Column(Name = "ColVisible", Comments = "列显示")]
        public int ColVisible { get; set; }

        [Column(Name = "ColName", Comments = "列名称")]
        [StringRule(MinLen = 1, MaxLen = 60, ErrorMessage = "请输入正确的列名称")]
        public string ColName { get; set; }

        [Column(Name = "ColHeader", Comments = "列中文标题")]
        [StringRule(MinLen = 1, MaxLen = 60, ErrorMessage = "请输入正确的列标题")]
        public string ColHeader { get; set; }

        [Column(Name = "ColDesc", Comments = "列备注")]
        public string ColDesc { get; set; }

        [Column(Name = "ColMark", Comments = "列标记 辅助字段")]
        public string ColMark { get; set; }

        [Column(Name = "ColBind", Comments = "列绑定")]
        public string ColBind { get; set; }

        [Column(Name = "ColType", Comments = "列类型  datetime varchar(45) number")]
        public string ColType { get; set; }

        [Column(Name = "ColStyle", Comments = "列显示样式")]
        public string ColStyle { get; set; }

        [Column(Name = "ColWidth", Comments = "列宽")]
        [StringRule(MinLen = 1, MaxLen = 3, ErrorMessage = "请输入正确的列宽")]
        public string ColWidth { get; set; }

        [Column(Name = "DataType", Comments = "数据类型 datetime string number ")]
        [StringRule(MinLen = 1, MaxLen = 20, ErrorMessage = "请输入正确的数据类型")]
        public string DataType { get; set; }

        [Column(Name = "DataFormat", Comments = "数据格式")]
        public string DataFormat { get; set; }

        [Column(Name = "DecimalDigits", Comments = "小数位数")]
        public string DecimalDigits { get; set; }

        [Column(Name = "Unit", Comments = "单位")]
        public string Unit { get; set; }

        //当前值
        public string CurrentValue { get; set; }

        [Column(Name = "DefaultValue", Comments = "默认值")]
        public string DefaultValue { get; set; }

        [Column(Name = "CorrectionMode", Comments = "数据修正模式")]
        public string CorrectionMode { get; set; }

        [Column(Name = "CorrectionScale", Comments = "数据修正系数")]
        public string CorrectionScale { get; set; }

        [Column(Name = "CorrectionOffset", Comments = "数据修正偏移")]
        public string CorrectionOffset { get; set; }

        [Column(Name = "EnableCorrection", Comments = "数据修正开关")]
        public string EnableCorrection { get; set; }

        [Column(Name = "EnableRangeComp", Comments = "范围控制开关")]
        public string EnableRangeComp { get; set; }

        [Column(Name = "MaxValue", Comments = "最大值")]
        public string MaxValue { get; set; }

        [Column(Name = "MinValue", Comments = "最小值")]
        public string MinValue { get; set; }

        [Column(Name = "OrderID", Comments = "排序号")]
        public string OrderID { get; set; }

        public ModelSheetColumn()
        {
            ColVisible = SystemDefault.InValidInt;
            IsActive = SystemDefault.InValidInt;
            IsEditable = SystemDefault.InValidInt;
            IsFrozen = SystemDefault.InValidInt;
        }
    }

    /// <summary>
    /// 报表列 - 视图
    /// </summary>
    public class ViewSheetColumn : ModelSheetColumn
    {
        public ViewSheetColumn()
        {
            if (ColumnStyleItems == null)
            {
                ColumnStyleItems = new List<string>();
                ColumnStyleItems.Add("Text");
                ColumnStyleItems.Add("CheckBox");
                ColumnStyleItems.Add("TextBlock");
            }

            if (DataTypeItems ==null)
            {
                DataTypeItems = new List<string>();
                DataTypeItems.Add("string");
                DataTypeItems.Add("int");
                DataTypeItems.Add("double");
            }

            ColStyle = ColumnStyleItems[0];
            DataType = DataTypeItems[0];
            ColWidth = "80";
        }

        public List<string> ColumnStyleItems { get; set; }
        public List<string> DataTypeItems { get; set; }

    }
}
