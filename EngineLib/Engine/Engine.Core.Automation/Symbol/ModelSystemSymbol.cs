using Engine.Common;
using Engine.Data.DBFAC;
using System.Collections.Generic;

namespace Engine.Core
{
    /// <summary>
    /// 系统变量表
    /// </summary>
    [Table(Name = "sys_symbol", ViewName = "sys_symbol_view", Comments = "变量表")]
    public class ModelSystemSymbol : ValidateError<ModelSystemSymbol>
    {
        [Column(Name = "ID", PK = true, AI = true, Comments = "索引")]
        public string ID { get; set; }

        [Column(Name = "GroupToken", Comments = "变量组ID")]
        public string GroupToken { get; set; }

        [Column(Name = "Token", Comments = "变量ID")]
        public string Token { get; set; }

        [Column(Name = "WorkDomain", ReadOnly = true, Comments = "变量组名称")]
        public string WorkDomain { get; set; }

        [Column(Name = "GroupName", Comments = "变量组名称")]
        public string GroupName { get; set; }

        [Column(Name = "Name", Comments = "变量名称")]
        [StringRule(MinLen = 1, MaxLen = 60, ErrorMessage = "请输入正确的变量名称")]
        public string Name { get; set; }

        [Column(Name = "DataType", Comments = "变量类型")]
        public string DataType { get; set; }

        [Column(Name = "DataFormat", Comments = "变量格式")]
        public string DataFormat { get; set; }

        [Column(Name = "CurrentValue", Comments = "当前值")]
        public string CurrentValue { get; set; }

        [Column(Name = "DefaultValue", Comments = "初始值")]
        public string DefaultValue { get; set; }

        [Column(Name = "MinValue", Comments = "最小值")]
        public string MinValue { get; set; }

        [Column(Name = "MaxValue", Comments = "最大值")]
        public string MaxValue { get; set; }

        [Column(Name = "LogicExpress", Comments = "逻辑表达式")]
        public string LogicExpress { get; set; }

        [Column(Name = "RelatedDriver", Comments = "关联控制器")]
        public string RelatedDriver { get; set; }

        [Column(Name = "UnitSign", Comments = "单位")]
        public string UnitSign { get; set; }

        [Column(Name = "Comment", Comments = "描述")]
        public string Comment { get; set; }

        [Column(Name = "IsView", Comments = "可视化")]
        public string IsView { get; set; }

        [Column(Name = "CustomMark", Comments = "自定义标记")]
        public string CustomMark { get; set; }

        [Column(Name = "OrderID", Comments = "排序号")]
        public string OrderID { get; set; }

        [Column(Name = "ViewValue", Comments = "显示值")]
        public string ViewValue { get; set; }

        [Column(Name = "SetValue", Comments = "设定值")]
        public string SetValue { get; set; }
        /// <summary>
        /// 计算值
        /// </summary>
        /// <returns></returns>
        public void ExcuteCalculate()
        {
            switch (DataType)
            {
                case "State":
                case "Alarm":
                case "Temp":
                    if (!string.IsNullOrEmpty(DefaultValue) && string.IsNullOrEmpty(SetValue))
                        CurrentValue = DefaultValue;
                    else if(!string.IsNullOrEmpty(SetValue))
                        CurrentValue = SetValue;
                    break;
                case "Command":
                    break;
                case "Number":
                    break;
            }
        }
    }

    /// <summary>
    /// 系统变量表 - 视图
    /// </summary>
    public class ViewSystemSymbol: ModelSystemSymbol
    {
        public ViewSystemSymbol()
        {
            if (DataTypeItems == null)
            {
                DataTypeItems = SystemDef.LstSymbolDataType;
                //DataTypeItems.Add("State");
                //DataTypeItems.Add("Alarm");
                //DataTypeItems.Add("Number");
                //DataTypeItems.Add("Command");
                //DataTypeItems.Add("Path[ ]");
                //DataTypeItems.Add("Temp");
                //"string","DataView","Command","AlarmView","SubPosView"
            }

            DataType = DataTypeItems[0];
        }

        public List<string> DataTypeItems { get; set; }

    }
}
