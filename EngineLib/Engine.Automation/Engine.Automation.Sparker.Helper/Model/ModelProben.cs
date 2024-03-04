using Engine.Common;
using Engine.Data.DBFAC;
using System;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;

namespace Engine.Automation.Sparker
{
    [Table(Name = "ana_proben_main", Comments = "样本数据-控样列表")]
    public class ModelProbenMain
    {
        [Column(Name = "ID", AI = true, PK = true, Comments = "索引")]
        public string ID { get; set; }

        [Column(Name = "MainToken", Comments = "样品索引")]
        public virtual string MainToken { get; set; }

        [Column(Name = "ProbenToken", ReadOnly = true, Comments = "样品配置索引")]
        public string ProbenToken { get; set; }

        [Column(Name = "Name", Comments = "控样名称")]
        [StringRule(MinLen = 1, MaxLen = 40, ErrorMessage = "样本名称指定不正确,字符长度1-40")]
        public string Name { get; set; }

        [Column(Name = "Type", Comments = "控样类型名称")]
        public string Type { get; set; }

        [Column(Name = "Comment", Comments = "控样描述")]
        public string Comment { get; set; }
    }

    [Table(Name = "ana_proben_detail", ViewName = "view_ana_proben_relatedmain", Comments = "样本数据-控样详情列表")]
    public class ModelProbenDetail
    {
        [Column(Name = "ID", AI = true, PK = true, Comments = "索引")]
        public string ID { get; set; }

        [Column(Name = "Name", ReadOnly = true, Comments = "样品名称")]
        [StringRule(MinLen = 1, ErrorMessage = "样品名称长度无效")]
        public string Name { get; set; }

        [Column(Name = "Type", ReadOnly = true, Comments = "样品类型")]
        public string Type { get; set; }

        //使用类型 控样 标样 会被当做检查样使用
        public string UseType { get; set; } = string.Empty;

        [Column(Name = "Comment", ReadOnly = true, Comments = "备注")]
        public string Comment { get; set; }

        [Column(Name = "InsName", ReadOnly = true, Comments = "仪器名称")]
        public string InsName { get; set; }

        [Column(Name = "Matrix", ReadOnly = true, Comments = "基体")]
        public string Matrix { get; set; }

        [Column(Name = "PgmName", ReadOnly = true, Comments = "分析程序名称")]
        public string PgmName { get; set; }

        [Column(Name = "ProbenToken", Comments = "控样主键")]
        public string ProbenToken { get; set; }

        [Column(Name = "MainToken", Comments = "控样引用索引")]
        public string MainToken { get; set; }

        [Column(Name = "PgmToken", Comments = "仪器分析程序索引")]
        public string PgmToken { get; set; }

        [Column(Name = "ShelfPos", Comments = "标样台位置")]
        public string ShelfPos { get; set; }

        [Column(Name = "AnaCycleTime", Comments = "分析周期")]
        [NumberRule(0, 100, "分析周期设置无效")]
        public string AnaCycleTime { get; set; }

        [Column(Name = "SparkPosCount", Comments = "激发点数设定值")]
        [NumberRule(0, 100, "分析周期设置无效")]
        public string SparkPosCount { get; set; }

        [Column(Name = "CurrentSparkPots", Comments = "当前激发点数")]
        public string CurrentSparkPots { get; set; }

        [Column(Name = "LastAnaTime", Comments = "最后分析时间")]
        public string LastAnaTime { get; set; }

        [Column(Name = "CurrentState", Comments = "当前状态")]
        public string CurrentState { get; set; }
    }

    [Table(Name = "ana_proben_std", ViewName = "view_ana_proben_std", Comments = "控样元素标准值")]
    public class ModelProbenStd
    {
        [Column(Name = "ID", AI = true, PK = true, Comments = "索引")]
        public string ID { get; set; }

        [Column(Name = "MainToken", Comments = "控样索引")]
        public string MainToken { get; set; }

        [Column(Name = "Element", Comments = "元素名称")]
        [StringRule(MinLen = 1, MaxLen = 10, ErrorMessage = "元素名称指定不正确,字符长度1-10")]
        public string Element { get; set; }

        [Column(Name = "SetPoint", Comments = "标准值")]
        [Numberic()]
        public string SetPoint { get; set; }

        [Column(Name = "Name", ReadOnly = true, Comments = "样品名称")]
        public string Name { get; set; }

        [Column(Name = "Type", ReadOnly = true, Comments = "样品类型")]
        public string Type { get; set; }

        [Column(Name = "Comment", ReadOnly = true, Comments = "描述")]
        public string Comment { get; set; }
    }

    [Table(Name = "ana_proben_elem",ViewName = "view_ana_proben_full", Comments = "控样元素列表")]
    public class ModelProbenElem : ValidateError<ModelProbenElem>
    {
        [Column(Name = "ID", AI = true, PK = true, Comments = "索引")]
        public string ID { get; set; }

        [Column(Name = "Name", ReadOnly = true, Comments = "控样名称")]
        public string Name { get; set; }

        [Column(Name = "Type", ReadOnly = true, Comments = "样品类型")]
        public string Type { get; set; }

        [Column(Name = "Comment", ReadOnly = true, Comments = "样品描述")]
        public string Comment { get; set; }

        [Column(Name = "InsName", ReadOnly = true, Comments = "仪器名称")]
        public string InsName { get; set; }

        [Column(Name = "Matrix", ReadOnly = true, Comments = "基体")]
        public string Matrix { get; set; }

        [Column(Name = "PgmName", ReadOnly = true, Comments = "分析程序")]
        public string PgmName { get; set; }

        [Column(Name = "MainToken", Comments = "控样索引")]
        public string MainToken { get; set; }

        [Column(Name = "ProbenToken", Comments = "控样分类索引")]
        public string ProbenToken { get; set; }

        [Column(Name = "PgmToken", Comments = "分析程序索引")]
        public string PgmToken { get; set; }

        [Column(Name = "Element", Comments = "元素名称")]
        public string Element { get; set; }

        [Column(Name = "SetPoint", Comments = "标准值")]
        [Numberic()]
        public string SetPoint
        {
            get => _SetPoint;
            set
            {
                if (_SetPoint != value)
                {
                    _SetPoint = GetValue(value,_SetPoint);
                    RaisePropertyChanged();
                    UpdateTolCalculate();
                    if (_SetPoint.IsNumeric() && _SetPointSetted)
                        IsModified = true;
                }
                _SetPointSetted = true;
            }
        }
        private string _SetPoint = string.Empty;
        private bool _SetPointSetted = false;

        [Column(Name = "StartValue", Comments = "首次值")]
        public string StartValue
        {
            get => _StartValue;
            set
            {
                if (_StartValue != value)
                {
                    _StartValue = GetValue(value, _StartValue);
                    RaisePropertyChanged();
                    UpdateTolCalculate();
                    if (_StartValue.IsNumeric() && _StartValueSetted)
                        IsModified = true;
                }
                _StartValueSetted = true;
            }
        }
        private string _StartValue;
        private bool _StartValueSetted = false;

        [Column(Name = "ActualValue", Comments = "校正分析值")]
        [Numberic()]
        public string ActualValue
        {
            get => _ActualValue;
            set
            {
                if (_ActualValue != value)
                {
                    _ActualValue = GetValue(value, _ActualValue);
                    RaisePropertyChanged();
                    UpdateTolCalculate();
                    if (_ActualValue.IsNumeric() && _ActualValueSetted)
                        IsModified = true;
                }
                _ActualValueSetted = true;
            }
        }
        private string _ActualValue;
        private bool _ActualValueSetted = false;

        //检查分析值
        [Numberic()]
        public string CheckValue
        {
            get => _CheckValue;
            set
            {
                if (_CheckValue != value)
                {
                    _CheckValue = GetValue(value, _CheckValue);
                    RaisePropertyChanged();
                    UpdateTolCalculate();
                }
            }
        }
        private string _CheckValue;

        [Column(Name = "TolStart", Comments = "首次偏差")]
        [Numberic()]
        public string TolStart
        {
            get => _TolStart;
            set { _TolStart = GetValue(value, _TolStart); RaisePropertyChanged(); }
        }
        private string _TolStart;

        [Column(Name = "TolActual", Comments = "校正偏差")]
        [Numberic()]
        public string TolActual
        {
            get => _TolActual;
            set { _TolActual = GetValue(value, _TolActual); RaisePropertyChanged(); }
        }
        private string _TolActual;

        [Column(Name = "AdditiveValue", Comments = "加法参数")]
        [Numberic()]
        public string AdditiveValue
        {
            get => _AdditiveValue;
            set { _AdditiveValue = GetValue(value, _AdditiveValue); RaisePropertyChanged(); }
        }
        private string _AdditiveValue;

        [Column(Name = "MultiplitiveValue", Comments = "乘法参数")]
        [Numberic()]
        public string MultiplitiveValue
        {
            get => _MultiplitiveValue;
            set { _MultiplitiveValue = GetValue(value, _MultiplitiveValue); RaisePropertyChanged(); }
        }
        private string _MultiplitiveValue;

        //检查偏差
        [Numberic()]
        public string TolCheck
        {
            get => _TolCheck;
            set { _TolCheck = GetValue(value, _TolCheck); RaisePropertyChanged(); }
        }
        private string _TolCheck;

        public bool IsModified { get; set; }
        public bool IsChecked { get=>_IsChecked; set { _IsChecked = value;RaisePropertyChanged(); } }
        private bool _IsChecked;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private string GetValue(object NewValue, object OldValue, int decimalPlaces = 4, string DataFormat = "0.000##")
        {
            string strFormatedValue = string.Empty;
            if (NewValue.ToMyString() == SystemDefault.StringEmpty) return NewValue.ToMyString();
            string strValue = NewValue.ToMyString().GoOriginal();
            if (strValue.IsEmpty()) return string.Empty;
            if (strValue.IsNumeric())
                strFormatedValue = strValue.ToMyDouble().RoundToDecimalAndFormat(decimalPlaces, DataFormat);
            else if (!OldValue.IsNumeric() && !string.IsNullOrEmpty(OldValue.ToMyString()))
                strFormatedValue = "0".RoundToDecimalAndFormat(decimalPlaces, DataFormat);
            else
                strFormatedValue = OldValue.ToMyString();
            return strFormatedValue;
        }

        /// <summary>
        /// 更新首次偏差
        /// </summary>
        private void UpdateTolCalculate()
        {
            if (StartValue.IsNotEmpty() && SetPoint.IsNotEmpty())
            {
                //首次偏差
                double dTolStart = Calculate("A-B-C", SetPoint.ToMyDouble(), StartValue.ToMyDouble(), 0);
                if (dTolStart != SystemDefault.InValidDouble)
                    TolStart = dTolStart.RoundToDefaultDecimalAndFormat();
            }
            if (ActualValue.IsNotEmpty() && SetPoint.IsNotEmpty())
            {
                //实际偏差
                double dTolActual = Calculate("A-B-C", SetPoint.ToMyDouble(), ActualValue.ToMyDouble(), 0);
                if (dTolActual != SystemDefault.InValidDouble)
                    TolActual = dTolActual.RoundToDefaultDecimalAndFormat();
                //乘法系数
                double dMulScale = Calculate("A/B-C", SetPoint.ToMyDouble(), ActualValue.ToMyDouble(), 0);
                if (dMulScale == SystemDefault.InValidDouble)
                    dMulScale = 1.0;
                MultiplitiveValue = dMulScale.RoundToDefaultDecimalAndFormat();
                //加法偏移
                double dAddOff = Calculate("A-B-C", SetPoint.ToMyDouble(), ActualValue.ToMyDouble(), 0);
                if (dAddOff != SystemDefault.InValidDouble)
                    AdditiveValue = dAddOff.RoundToDefaultDecimalAndFormat();
            }
            if (CheckValue.IsNotEmpty() && SetPoint.IsNotEmpty())
            {
                //首次偏差
                double dTolCheck = Calculate("A-B-C", SetPoint.ToMyDouble(), CheckValue.ToMyDouble(), 0);
                if (dTolCheck != SystemDefault.InValidDouble)
                    TolCheck = dTolCheck.RoundToDefaultDecimalAndFormat();
            }
        }

        /// <summary>
        /// 计算公式
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <param name="C"></param>
        /// <returns></returns>
        private double Calculate(string expression, double A, double B, double C)
        {
            try
            {
                //ex: ??? =A *  lgm - B
                //创建参数
                ParameterExpression parameterA = Expression.Parameter(typeof(double), "A");
                ParameterExpression parameterB = Expression.Parameter(typeof(double), "B");
                ParameterExpression parameterC = Expression.Parameter(typeof(double), "C");
                //构建表达式树
                Expression body = DynamicExpressionParser.ParseLambda(new[] { parameterA, parameterB, parameterC }, typeof(double), expression).Body;
                //编译表达式树成委托函数
                Func<double, double, double, double> calculator = Expression.Lambda<Func<double, double, double, double>>(body, parameterA, parameterB, parameterC).Compile();
                //调用委托函数计算结果
                double result = calculator(A, B, C);
                return result;
            }
            catch (Exception)
            {
                return SystemDefault.InValidDouble;
            }
        }
    }
}
