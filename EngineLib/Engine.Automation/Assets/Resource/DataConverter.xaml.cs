using Engine.Automation.Sparker;
using Engine.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace App.Assets.Resource
{
    /// <summary>
    /// 标样台位置矩阵
    /// </summary>
    public class StdShelfPosMatrix : IValueConverter
    { 
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //P2-05  2行05位
            string iPos = string.Empty;
            string strPosMatrix = value.ToMyString();
            string strParameter = parameter.ToMyString();
            if (strParameter == "Row")
                iPos = strPosMatrix.MidString("P","-").ToMyInt().ToString();
            else if (strParameter == "Col")
                iPos= strPosMatrix.MidString("-", "").ToMyInt().ToString();
            return iPos;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ProbenMainFullNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            List<string> LstSampleFullName = new List<string>();
            if (value is List<ModelProbenMain> LstProbenMain)
            {
                foreach (var item in LstProbenMain)
                {
                    string strFullName = item.Type.ToMyString();
                    if (!string.IsNullOrEmpty(strFullName))
                        strFullName += "： ";
                    strFullName += item.Name.ToMyString();
                    LstSampleFullName.Add(strFullName);
                }
            }
            return LstSampleFullName;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }

    /// <summary>
    /// 牌号使用的控样列表
    /// </summary>
    public class LstMaterialElemToProbenName : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string strRelatedProben = string.Empty;
            List<string> LstProbenName = new List<string>();
            if (value is List<ModelMaterialFull> LstMaterialElem)
            {
                foreach (var item in LstMaterialElem)
                {
                    LstProbenName.AppandList(item.T1_Name);
                    LstProbenName.AppandList(item.T2_Name);
                }
            }
            strRelatedProben = LstProbenName.ToMyString(", ");
            return strRelatedProben;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
