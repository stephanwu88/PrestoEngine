using Engine.Common;
using Engine.Data;
using Engine.Data.DBFAC;

namespace Engine.Automation.Sparker
{   
    [Table(Name = "ana_material_main", ViewName = "view_ana_material_main", Comments = "牌号主信息表")]
    public class ModelMaterialMain
    {
        [Column(Name = "ID", AI = true, PK = true, Comments = "索引")]
        public string ID { get; set; }

        [Column(Name = "Token", Comments = "主键")]
        public string Token { get; set; }

        [Column(Name = "PgmToken", Comments = "分析程序主键")]
        [StringRule(MinLen = 1, MaxLen = 40, ErrorMessage = "未指定分析程序")]
        public string PgmToken { get; set; }

        [Column(Name = "Material", Comments = "牌号名称")]
        [StringRule(MinLen = 1, MaxLen = 40, ErrorMessage = "牌号名称不正确,字符长1-40")]
        public string Material { get; set; }

        [Column(Name = "Comment", Comments = "备注")]
        public string Comment { get; set; }

        [Column(Name = "InsName", ReadOnly = true, Comments = "仪器名称")]
        public string InsName { get; set; }

        [Column(Name = "Matrix", ReadOnly = true, Comments = "基体名称")]
        public string Matrix { get; set; }

        [Column(Name = "PgmName", ReadOnly = true, Comments = "分析程序名称")]
        public string PgmName { get; set; }

    }

    [Table(Name = "ana_material_elem", ViewName = "view_ana_material_elem", Comments = "材质元素表")]
    public class ModelMaterialElem
    {
        [Column(Name = "ID", AI = true, PK = true, Comments = "索引")]
        public string ID { get; set; }

        [Column(Name = "Material", ReadOnly = true, Comments = "牌号名称")]
        public string Material { get; set; }

        [Column(Name = "MaterialToken", Comments = "牌号主键")]
        public string MaterialToken { get; set; }

        [Column(Name = "Element", Comments = "元素名称")]
        public string Element { get; set; }
        
        [Column(Name = "TolMethod", Comments = "偏差")]
        public string TolMethod { get; set; }

        [Column(Name ="LimitMin",Comments ="下偏差")]
        public string LimitMin { get; set; }

        [Column(Name = "LimitMax", Comments = "上偏差")]
        public string LimitMax { get; set; }

        [Column(Name = "TS1_Token", Comments = "1.控样 主键")]
        public string TS1_Token { get; set; }

        [Column(Name = "TS2_Token", Comments = "2.控样 主键")]
        public string TS2_Token { get; set; }

        [Column(Name = "CS_Token", Comments = "检查样 主键")]
        public string CS_Token { get; set; }

        [Column(Name = "CheckValue", Comments = "检查样 检查值")]
        public string CheckValue { get; set; }

        [Column(Name = "TolCheck", Comments = "检查样 偏差")]
        public string TolCheck { get; set; }

        [Column(Name = "InsName", ReadOnly = true, Comments = "仪器名称")]
        public string InsName { get; set; }

        [Column(Name = "Matrix", ReadOnly = true, Comments = "基体名称")]
        public string Matrix { get; set; }

        [Column(Name = "PgmName", ReadOnly = true, Comments = "分析程序名称")]
        public string PgmName { get; set; }
    }

    [Table(Name = "view_ana_material_full", Comments = "牌号全表")]
    public class ModelMaterialFull : NotifyObject
    {
        [Column(Name = "ID", AI = true, PK = true, Comments = "牌号主键")]
        public string ID { get; set; }

        [Column(Name = "Material", ReadOnly = true, Comments = "牌号主键")]
        public string Material { get; set; }

        [Column(Name = "Comment", ReadOnly = true, Comments = "元素名称")]
        public string Comment { get; set; }

        [Column(Name = "InsName", ReadOnly = true, Comments = "")]
        public string InsName { get; set; }

        [Column(Name = "PgmName", ReadOnly = true, Comments = "")]
        public string PgmName { get; set; }

        [Column(Name = "Matrix", ReadOnly = true, Comments = "")]
        public string Matrix { get; set; }

        [Column(Name = "Element", ReadOnly = true, Comments = "")]
        public string Element { get; set; }

        [Column(Name = "TolMethod", ReadOnly = true, Comments = "")]
        public string TolMethod { get; set; }

        [Column(Name = "LimitMin", ReadOnly = true, Comments = "")]
        [Numberic()]
        public string LimitMin
        {
            get => _LimitMin;
            set
            {
                if (_LimitMin != value)
                {
                    _LimitMin = GetValue(value, _LimitMin);
                    RaisePropertyChanged();
                    if (_LimitMin.IsNumeric() && _LimitMinSetted)
                        IsModified = true;
                }
                _LimitMinSetted = true;
            }
        }
        private string _LimitMin;
        private bool _LimitMinSetted = false;

        [Column(Name = "LimitMax", ReadOnly = true, Comments = "")]
        [Numberic()]
        public string LimitMax
        {
            get => _LimitMax;
            set
            {
                if (_LimitMax != value)
                {
                    _LimitMax = GetValue(value, _LimitMax);
                    RaisePropertyChanged();
                    if (_LimitMax.IsNumeric() && _LimitMaxSetted)
                        IsModified = true;
                }
                _LimitMaxSetted = true;
            }
        }
        private string _LimitMax;
        private bool _LimitMaxSetted = false;

        [Column(Name = "T1_Name", ReadOnly = true, Comments = "")]
        public string T1_Name { get; set; }

        [Column(Name = "T1_SetPoint", ReadOnly = true, Comments = "")]
        public string T1_SetPoint { get=> _T1_SetPoint; set { _T1_SetPoint = GetValue(value, _T1_SetPoint);RaisePropertyChanged(); } }
        private string _T1_SetPoint;

        [Column(Name = "T1_StartValue", ReadOnly = true, Comments = "")]
        public string T1_StartValue { get => _T1_StartValue; set { _T1_StartValue = GetValue(value, _T1_StartValue); RaisePropertyChanged(); } }
        public string _T1_StartValue;

        [Column(Name = "T1_ActualValue", ReadOnly = true, Comments = "")]
        public string T1_ActualValue { get => _T1_ActualValue; set { _T1_ActualValue = GetValue(value, _T1_ActualValue); RaisePropertyChanged(); } }
        public string _T1_ActualValue;

        [Column(Name = "T1_TolStart", ReadOnly = true, Comments = "")]
        public string T1_TolStart { get => _T1_TolStart; set { _T1_TolStart = GetValue(value, _T1_TolStart); RaisePropertyChanged(); } }
        public string _T1_TolStart { get; set; }

        [Column(Name = "T1_TolActual", ReadOnly = true, Comments = "")]
        public string T1_TolActual { get => _T1_TolActual; set { _T1_TolActual = GetValue(value, _T1_TolActual); RaisePropertyChanged(); } }
        public string _T1_TolActual { get; set; }

        [Column(Name = "T1_AdditiveValue", ReadOnly = true, Comments = "")]
        public string T1_AdditiveValue { get => _T1_AdditiveValue; set { _T1_AdditiveValue = GetValue(value, _T1_AdditiveValue); RaisePropertyChanged(); } }
        public string _T1_AdditiveValue { get; set; }

        [Column(Name = "T1_MultiplitiveValue", ReadOnly = true, Comments = "")]
        public string T1_MultiplitiveValue { get => _T1_MultiplitiveValue; set { _T1_MultiplitiveValue = GetValue(value, _T1_MultiplitiveValue); RaisePropertyChanged(); } }
        public string _T1_MultiplitiveValue { get; set; }

        [Column(Name = "T1_CurrentState", ReadOnly = true, Comments = "")]
        public string T1_CurrentState { get; set; }

        [Column(Name = "T2_Name", ReadOnly = true, Comments = "")]
        public string T2_Name { get; set; }

        [Column(Name = "T2_SetPoint", ReadOnly = true, Comments = "")]
        public string T2_SetPoint { get => _T2_SetPoint; set { _T2_SetPoint = GetValue(value, _T2_SetPoint); RaisePropertyChanged(); } }
        public string _T2_SetPoint { get; set; }

        [Column(Name = "T2_StartValue", ReadOnly = true, Comments = "")]
        public string T2_StartValue { get => _T2_StartValue; set { _T2_StartValue = GetValue(value, _T2_StartValue); RaisePropertyChanged(); } }
        public string _T2_StartValue { get; set; }

        [Column(Name = "T2_ActualValue", ReadOnly = true, Comments = "")]
        public string T2_ActualValue { get => _T2_ActualValue; set { _T2_ActualValue = GetValue(value, _T2_ActualValue); RaisePropertyChanged(); } }
        public string _T2_ActualValue { get; set; }

        [Column(Name = "T2_TolStart", ReadOnly = true, Comments = "")]
        public string T2_TolStart { get => _T2_TolStart; set { _T2_TolStart = GetValue(value, _T2_TolStart); RaisePropertyChanged(); } }
        public string _T2_TolStart { get; set; }

        [Column(Name = "T2_TolActual", ReadOnly = true, Comments = "")]
        public string T2_TolActual { get => _T2_TolActual; set { _T2_TolActual = GetValue(value, _T2_TolActual); RaisePropertyChanged(); } }
        public string _T2_TolActual { get; set; }

        [Column(Name = "T2_AdditiveValue", ReadOnly = true, Comments = "")]
        public string T2_AdditiveValue { get => _T2_AdditiveValue; set { _T2_AdditiveValue = GetValue(value, _T2_AdditiveValue); RaisePropertyChanged(); } }
        public string _T2_AdditiveValue { get; set; }

        [Column(Name = "T2_MultiplitiveValue", ReadOnly = true, Comments = "")]
        public string T2_MultiplitiveValue { get => _T2_MultiplitiveValue; set { _T2_MultiplitiveValue = GetValue(value, _T2_MultiplitiveValue); RaisePropertyChanged(); } }
        public string _T2_MultiplitiveValue { get; set; }

        [Column(Name = "T1_ShelfPos", ReadOnly = true, Comments = "")]
        public string T1_ShelfPos { get; set; }

        [Column(Name = "T1_AnaCycleTime", ReadOnly = true, Comments = "")]
        public string T1_AnaCycleTime { get; set; }

        [Column(Name = "T1_SparkPotCount", ReadOnly = true, Comments = "")]
        public string T1_SparkPotCount { get; set; }

        [Column(Name = "T1_CurrentSparkPots", ReadOnly = true, Comments = "")]
        public string T1_CurrentSparkPots { get; set; }

        [Column(Name = "T1_LastAnaTime", ReadOnly = true, Comments = "")]
        public string T1_LastAnaTime { get; set; }

        [Column(Name = "T2_ShelfPos", ReadOnly = true, Comments = "")]
        public string T2_ShelfPos { get; set; }

        [Column(Name = "T2_AnaCycleTime", ReadOnly = true, Comments = "")]
        public string T2_AnaCycleTime { get; set; }

        [Column(Name = "T2_SparkPotCount", ReadOnly = true, Comments = "")]
        public string T2_SparkPotCount { get; set; }

        [Column(Name = "T2_CurrentSparkPots", ReadOnly = true, Comments = "")]
        public string T2_CurrentSparkPots { get; set; }

        [Column(Name = "T2_LastAnaTime", ReadOnly = true, Comments = "")]
        public string T2_LastAnaTime { get; set; }

        [Column(Name = "T2_CurrentState", ReadOnly = true, Comments = "")]
        public string T2_CurrentState { get; set; }

        [Column(Name = "CS_Name", ReadOnly = true, Comments = "")]
        public string CS_Name { get; set; }

        [Column(Name = "CheckValue", ReadOnly = true, Comments = "")]
        public string CheckValue { get; set; }

        [Column(Name = "TolCheck", ReadOnly = true, Comments = "")]
        public string TolCheck { get; set; }

        [Column(Name = "MaterialToken", ReadOnly = true, Comments = "")]
        public string MaterialToken { get; set; }

        [Column(Name = "TS1_Token", ReadOnly = true, Comments = "")]
        public string TS1_Token { get; set; }

        [Column(Name = "TS2_Token", ReadOnly = true, Comments = "")]
        public string TS2_Token { get; set; }

        [Column(Name = "CS_Token", ReadOnly = true, Comments = "")]
        public string CS_Token { get; set; }

        public bool IsModified { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private string GetValue(object NewValue, object OldValue, int decimalPlaces = 4, string DataFormat = "0.00##")
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
    }
}
