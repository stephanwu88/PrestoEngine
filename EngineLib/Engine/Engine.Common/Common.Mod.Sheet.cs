using Engine.Mod;
using System.Collections.Generic;

namespace Engine.Common
{
    /// <summary>
    /// 关于表格列配置
    /// </summary>
    public partial class sCommon
    {
        /// <summary>
        /// 验证数据范围
        /// </summary>
        /// <param name="DataValue"></param>
        /// <param name="modCol"></param>
        /// <param name="OutULmt"></param>
        /// <param name="OutDLmt"></param>
        /// <returns>true:验证通过</returns>
        public static bool ValidateDataRange(this object DataValue, ModelSheetColumn modCol,
            out bool OutULmt, out bool OutDLmt)
        {
            OutULmt = false;
            OutDLmt = false;
            bool? EnableRangeComp = modCol.EnableRangeComp.ToMyBool();
            if (EnableRangeComp == true && DataValue.IsNumeric())
            {
                double dColValue = DataValue.ToMyDouble();
                double dMaxValue = modCol.MaxValue.ToMyDouble();
                double dMinValue = modCol.MinValue.ToMyDouble();
                if (modCol.MaxValue.IsNumeric() && dColValue >= dMaxValue)
                {
                    OutULmt = true;
                }
                if (modCol.MinValue.IsNumeric() && dColValue <= dMinValue)
                {
                    OutDLmt = true;
                }
            }
            return !OutULmt && !OutDLmt;
        }

        /// <summary>
        /// 数据修正
        /// </summary>
        /// <param name="DataValue"></param>
        /// <param name="modCol"></param>
        /// <returns></returns>
        public static object DataCorrection(this object DataValue, ModelSheetColumn modCol)
        {
            bool? EnableDataCorrection = modCol.EnableCorrection.ToMyBool();
            object ObjColValue = DataValue;
            if (EnableDataCorrection == true && ObjColValue.IsNumeric())
            {
                double dColValue = ObjColValue.ToMyDouble();
                double dCorretionScale = modCol.CorrectionScale.ToMyDouble();
                double dCorretionOffset = modCol.CorrectionOffset.ToMyDouble();
                if (modCol.CorrectionMode.Contains("乘法") && modCol.CorrectionScale.IsNumeric() && dCorretionScale != 0)
                    dColValue *= dCorretionScale;
                if (modCol.CorrectionMode.Contains("加法") && modCol.CorrectionOffset.IsNumeric())
                    dColValue += dCorretionOffset;
                ObjColValue = dColValue;
            }
            return ObjColValue;
        }

        /// <summary>
        /// 转换数据超范围信息字符串
        /// </summary>
        /// <param name="LstOutRange"></param>
        /// <returns></returns>
        public static string ToMyOutRangeString(this List<ModelSheetColumn> LstOutRange)
        {
            string strMessage = string.Empty;
            if (LstOutRange != null)
            {
                foreach (var item in LstOutRange)
                {
                    strMessage += $"数据[{item.ColBind}]={item.CurrentValue}, 超越范围[{item.MinValue}]-[{item.MaxValue}]\r\n";
                }
            }
            return strMessage;
        }
    }
}