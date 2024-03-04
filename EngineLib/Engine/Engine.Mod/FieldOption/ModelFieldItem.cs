using Engine.Common;
using Engine.Data.DBFAC;
using System;
using System.Collections.Generic;

namespace Engine.Mod
{
    [AttributeUsage(AttributeTargets.Property)]
    public class AutoRuleAttribute : ModelAttribute
    {

    }

    [Table(Name = "def_field_item", Comments = "字段基类")]
    public class FieldBase : ValidateError<FieldBase>
    {
        [Column(Name = "ID", PK = true, AI = true, Comments = "索引")]
        public string ID { get; set; }

        [Column(Name = "GroupName", Comments = "分组名称")]
        public virtual string GroupName { get; set; }

        [Column(Name = "FieldCode", Comments = "字段代码 - 英文")]
        public virtual string FieldCode { get; set; }

        [Column(Name = "HeaderView", Comments = "字段代码 - 中文")]
        public virtual string HeaderView { get; set; }

        [Column(Name = "FieldValue", Comments = "设定值")]
        public virtual string FieldValue { get; set; }

        [Column(Name = "Unit", Comments = "单位")]
        public virtual string Unit { get; set; }

        [Column(Name = "Comment", Comments = "字段描述")]
        public virtual string Comment { get; set; }

        [Column(Name = "MarkKey", Comments = "工程应用分类标记")]
        public virtual string MarkKey { get; set; }

        [Column(Name = "OrderID", Comments = "排序号")]
        public virtual string OrderID { get; set; }
    }

    [Table(Name = "def_field_item", Comments = "字段项")]
    public class ModelFieldItem : FieldBase
    {
        [Column(Name = "Token", Comments = "字段主键")]
        public string Token { get; set; }

        [Column(Name = "GroupToken", Comments = "分组索引")]
        public string GroupToken { get; set; }


        [Column(Name = "RelatedPos", Comments = "字段关联到工位")]
        public string RelatedPos { get; set; }

        [AutoRule(Name = "SetFldHeader", Comments = "设置项标题")]
        [Column(Name = "HeaderView", Comments = "列显示中文标题")]
        public override string HeaderView { get; set; }

        [Column(Name = "Options", Comments = "字段选项值")]
        public string Options
        {
            get => _Options;
            set
            {
                if (_Options != value)
                {
                    _Options = value;
                    FieldOptionList = _Options.MySplit(",");
                }
            }
        }
        private string _Options;

        [AutoRule(Name = "DefaultValue", Comments = "ComboBox默认值")]
        [Column(Name = "DefaultValue", Comments = "字段选项默认值")]
        public string DefaultValue { get; set; }
      
        [Column(Name = "OptionsCode", Comments = "字段代码")]
        public string OptionsCode
        {
            get => _OptionsCode;
            set
            {
                if (_OptionsCode != value)
                {
                    _OptionsCode = value;
                    FieldCodeList = _OptionsCode.MySplit(",");
                }
            }
        }
        private string _OptionsCode;

        [AutoRule(Name = "SetFldUnit", Comments = "单位")]
        [Column(Name = "Unit", Comments = "单位")]
        public override string Unit { get; set; }

        [AutoRule(Name = "IsEditable", Comments = "")]
        [Column(Name = "IsEditable", Comments = "允许编辑 不允许编辑说明该字段智能从Fields中选择")]
        public int IsEditable { get; set; } = SystemDefault.InValidInt;

        [Column(Name = "IsActive", Comments = "启用")]
        public int IsActive { get; set; } = SystemDefault.InValidInt;

        [Column(Name = "CustomMark1", Comments = "标记1")]
        public string CustomMark1 { get; set; }

        [Column(Name = "CustomMark2", Comments = "标记2")]
        public string CustomMark2 { get; set; }

        [Column(Name = "CustomMark3", Comments = "标记3")]
        public string CustomMark3 { get; set; }

        [AutoRule(Name = "FieldOptionList", Comments = "字段选项列表，用于ItemSource绑定")]
        public List<string> FieldOptionList { get; set; }

        [AutoRule(Name = "FieldCodeList", Comments = "字段选项列表，用于程序代码绑定")]
        public List<string> FieldCodeList { get; set; }
    }
}
