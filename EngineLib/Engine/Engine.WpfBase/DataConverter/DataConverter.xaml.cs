using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Engine.Common;

namespace Engine.WpfBase
{
    #region 自动化工位状态图
    /// <summary>
    /// 自动化工位状态图
    /// </summary>
    public class StateImageConvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string strValue = value.ToMyString();
            ImageSource img = null;
            switch (strValue)
            {
                case "NORMAL":
                    img = new BitmapImage(new Uri("pack://application:,,,/Engine;component/Image/设备空闲.png"));
                    break;
                case "RUN":
                    img = new BitmapImage(new Uri("pack://application:,,,/Engine;component/Image/设备运行.png"));
                    break;
                case "ALARM":
                    img = new BitmapImage(new Uri("pack://application:,,,/Engine;component/Image/设备报警.png"));
                    break;
                case "WAIT":
                    img = new BitmapImage(new Uri("pack://application:,,,/Engine;component/Image/设备等待.png"));
                    break;
                case "DONE":
                    img = new BitmapImage(new Uri("pack://application:,,,/Engine;component/Image/设备完成.png"));
                    break;
                case "OFFLINE":
                default:
                    img = new BitmapImage(new Uri("pack://application:,,,/Engine;component/Image/设备脱机.png"));
                    break;

            }
            return img;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
    #endregion

    #region 自动化工位状态图
    /// <summary>
    /// 自动化工位状态图
    /// </summary>
    public class MultiStateImageConvert : IMultiValueConverter
    {
        public object Convert(object[] value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string strNetState = string.Empty;
            string strPosState = string.Empty;
            string strTaskLock = string.Empty;
            if (value.Length >= 2)
            {
                strNetState = value[0].ToMyString();
                strPosState = value[1].ToMyString();
            }
            if (value.Length >= 3)
                strTaskLock = value[2].ToMyString();
            ImageSource img = null;
            if (strNetState != "1" && strNetState != "Connected")
            {
                img = new BitmapImage(new Uri("pack://application:,,,/Engine;component/Assets/Image/网络断开.png"));
                return img;
            }
            else if (strTaskLock == "1")
            {
                img = new BitmapImage(new Uri("pack://application:,,,/Engine;component/Assets/Image/设备锁定.png"));
                return img;
            }
            switch (strPosState)
            {
                case "NORMAL":
                    img = new BitmapImage(new Uri("pack://application:,,,/Engine;component/Assets/Image/设备空闲.png"));
                    break;
                case "RUN":
                    img = new BitmapImage(new Uri("pack://application:,,,/Engine;component/Assets/Image/设备运行.png"));
                    break;
                case "ALARM":
                    img = new BitmapImage(new Uri("pack://application:,,,/Engine;component/Assets/Image/设备报警.png"));
                    break;
                case "WAIT":
                    img = new BitmapImage(new Uri("pack://application:,,,/Engine;component/Assets/Image/设备等待.png"));
                    break;
                case "DONE":
                    img = new BitmapImage(new Uri("pack://application:,,,/Engine;component/Assets/Image/设备完成.png"));
                    break;
                case "OFFLINE":
                    img = new BitmapImage(new Uri("pack://application:,,,/Engine;component/Assets/Image/设备脱机.png"));
                    break;
                default:
                    img = new BitmapImage(new Uri("pack://application:,,,/Engine;component/Assets/Image/设备空闲.png"));
                    break;
            }
            return img;
        }

        public object[] ConvertBack(object value, Type[] targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
    #endregion

    #region BoolToColorConverter 开关量状态颜色转换器
    /// <summary>
    /// 状态颜色转换器
    /// </summary>
    public class BoolToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string strValue = value.ToMyString();
            List<string> LstParam = parameter.ToMyString().MySplit("|");
            if (LstParam.MyCount() != 2)
                return null;
            string strBrush = strValue.ToMyBool() == true ? LstParam[0] : LstParam[1];
            SolidColorBrush scb = new SolidColorBrush((Color)ColorConverter.ConvertFromString(strBrush));
            return scb;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
    #endregion

    #region StateColorConvert 状态颜色转换器 默认灰色 无信号:淡天空蓝 有信号绿色
    /// <summary>
    /// 状态颜色转换器
    /// </summary>
    public class StateColorConvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string strValue = value.ToMyString();
            SolidColorBrush brush = null;
            switch (strValue)
            {
                case "0":
                case "False":
                case "FALSE":
                case "false":
                    brush = Brushes.LightSkyBlue;
                    break;
                case "1":
                case "True":
                case "TRUE":
                case "true":
                case "RUN":
                    brush = Brushes.Green;
                    break;

                case "NORMAL":
                    brush = Brushes.White;
                    break;
                case "ALARM":
                    brush = Brushes.Red;
                    break;
                case "WAIT":
                    brush = Brushes.Yellow;
                    break;
                case "DONE":
                    brush = Brushes.Blue;
                    break;
                case "OFFLINE":
                default:
                    brush = Brushes.Gray;
                    break;

            }
            return brush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            SolidColorBrush brush = value as SolidColorBrush;
            string strRet = string.Empty;
            if (brush == Brushes.Green)
                strRet = "1";
            else
                strRet = "0";
            return strRet;
        }
    }
    #endregion

    #region StateColorConvertInv 状态颜色转换器 默认灰色 无信号:淡天空蓝 有信号绿色
    /// <summary>
    /// 状态颜色转换器
    /// </summary>
    public class StateColorConvertInv : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string strValue = value.ToMyString();
            SolidColorBrush brush = null;
            switch (strValue)
            {
                case "0":
                case "False":
                    brush = Brushes.Green;
                    break;
                case "1":
                case "True":
                    brush = Brushes.LightSkyBlue;
                    break;
                default:
                    brush = Brushes.Gray;
                    break;

            }
            return brush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            SolidColorBrush brush = value as SolidColorBrush;
            string strRet = string.Empty;
            if (brush == Brushes.LightSkyBlue)
                strRet = "1";
            else
                strRet = "0";
            return strRet;
        }
    }
    #endregion

    #region StateBackColorConvert 状态颜色转换器 默认灰色 无信号:淡天空蓝 有信号绿色
    /// <summary>
    /// 状态颜色转换器
    /// </summary>
    public class StateBackColorConvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string strValue = value.ToMyString();
            SolidColorBrush brush = Brushes.White;
            switch (strValue)
            {
                case "1":
                case "True":
                case "RUN":
                    brush = Brushes.Green;
                    break;
            }
            return brush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
    #endregion

    #region StateTextColorConvert 状态颜色转换器 默认灰色 无信号:淡天空蓝 有信号绿色
    /// <summary>
    /// 状态颜色转换器
    /// </summary>
    public class StateTextColorConvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string strValue = value.ToMyString();
            SolidColorBrush brush = Brushes.Red;
            switch (strValue)
            {
                case "1":
                case "True":
                case "RUN":
                    brush = Brushes.White;
                    break;
            }
            return brush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
    #endregion

    #region AlarmColorConvert 报警颜色转换器 默认灰色 无信号:淡天空蓝 有信号：红色
    /// <summary>
    /// 报警颜色转换器
    /// </summary>
    public class AlarmColorConvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string strValue = value.ToMyString();
            SolidColorBrush brush = null;
            switch (strValue)
            {
                case "0":
                case "False":
                    brush = Brushes.LightSkyBlue;
                    break;
                case "1":
                case "True":
                    brush = Brushes.Red;
                    break;
                default:
                    brush = Brushes.Gray;
                    break;

            }
            return brush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            SolidColorBrush brush = value as SolidColorBrush;
            string strRet = string.Empty;
            if (brush == Brushes.Red)
                strRet = "1";
            else
                strRet = "0";
            return strRet;
        }
    }
    #endregion

    #region AlarmColorConvertInv 报警颜色转换器 默认灰色 无信号:淡天空蓝 有信号：红色
    /// <summary>
    /// 报警颜色转换器
    /// </summary>
    public class AlarmColorConvertInv : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string strValue = value.ToMyString();
            SolidColorBrush brush = null;
            switch (strValue)
            {
                case "0":
                case "False":
                    brush = Brushes.Red;
                    break;
                case "1":
                case "True":
                    brush = Brushes.LightSkyBlue;
                    break;
                default:
                    brush = Brushes.Gray;
                    break;

            }
            return brush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            SolidColorBrush brush = value as SolidColorBrush;
            string strRet = string.Empty;
            if (brush == Brushes.Red)
                strRet = "0";
            else
                strRet = "1";
            return strRet;
        }
    }
    #endregion

    #region NetColorConvert 网络状态颜色转换 默认灰色 无信号红色 有信号绿色
    /// <summary>
    /// 网络状态颜色转换
    /// </summary>
    public class NetColorConvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string strValue = value.ToMyString();
            SolidColorBrush brush = null;
            switch (strValue)
            {
                case "0":
                case "False":
                    brush = Brushes.Red;
                    break;
                case "1":
                case "True":
                case "RUN":
                    brush = Brushes.Green;
                    break;

                default:
                    brush = Brushes.Gray;
                    break;

            }
            return brush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            SolidColorBrush brush = value as SolidColorBrush;
            string strRet = string.Empty;
            if (brush == Brushes.Green)
                strRet = "1";
            else
                strRet = "0";
            return strRet;
        }
    }
    #endregion

    #region NetComputerConvert 网络状态图片转换
    /// <summary>
    /// 网络状态图片转换
    /// </summary>
    public class NetComputerConvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string strNetState = value.ToMyString();
            ImageSource img = new BitmapImage(new Uri("pack://application:,,,/Engine;component/Assets/Image/计算机网络断开.png"));
            if (strNetState == "1" || strNetState.ToLower() == "connected" || strNetState.ToLower() == "true")
                img = new BitmapImage(new Uri("pack://application:,,,/Engine;component/Assets/Image/计算机网络正常.png"));
            return img;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
    #endregion

    #region VisibleConvert
    public class VisibleConvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string strValue = value.ToMyString().ToUpper();
            Visibility CanVisual = Visibility.Collapsed;
            switch (strValue)
            {
                case "0":
                case "FALSE":
                    CanVisual = Visibility.Collapsed;
                    break;
                case "1":
                case "TRUE":
                case "RUN":
                case "ADDNEW":
                    CanVisual = Visibility.Visible;
                    break;
                default:
                    CanVisual = Visibility.Collapsed;
                    break;
            }
            return CanVisual;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
    #endregion

    #region VisibleConvertInv
    public class VisibleConvertInv : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string strValue = value.ToMyString();
            Visibility CanVisual = Visibility.Visible;
            switch (strValue)
            {

                case "0":
                case "False":
                    CanVisual = Visibility.Visible;
                    break;
                case "1":
                case "True":
                case "RUN":
                    CanVisual = Visibility.Hidden;
                    break;
                default:
                    CanVisual = Visibility.Visible;
                    break;

            }
            return CanVisual;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
    #endregion

    /// <summary>
    /// NullableToVisibilityConverter
    /// </summary>
    public class NullableToVisibilityConverter : IValueConverter
    {
        public Visibility NullValue { get; set; } = Visibility.Collapsed;
        public Visibility NotNullValue { get; set; } = Visibility.Visible;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null ? NullValue : NotNullValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }

    /// <summary>
    /// 字符串转换可见性
    /// ex: Converter={StaticResource StringToVisible}, ConverterParameter=DateTime[Today] //Today,Yesterday,Tomorrow
    /// ex: Converter={StaticResource StringToVisible}, ConverterParameter=BitArray[0]  // value = 1110010 
    /// ex: Converter={StaticResource StringToVisible}, ConverterParameter=Equals[已到时]
    /// ex: Converter={StaticResource StringToVisible}, ConverterParameter='Contains[已到时,已入样,待制样]'}  或者使用|分割也兼容
    /// ex: Converter={StaticResource StringToVisible}, ConverterParameter='NotContains[已到时,已入样,待制样]'}  或者使用|分割也兼容
    /// ex: Converter={StaticResource StringToVisible}, ConverterParameter='Len[>0]'}   // > >= < <=  =
    /// </summary>
    public class StringToVisible : IValueConverter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"> BitArray[0] Equals[0] Contains[ABC] NotContains[ABC] Len[>0] Len[=8]</param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Visibility vsb = Visibility.Collapsed;
            string strValue = value.ToMyString();
            if (parameter == null)
            {
                if (strValue.ToMyBool() == true)
                    vsb = Visibility.Visible;
            }
            else
            {
                string strParam = parameter.ToMyString();
                string strParamKey = strParam.MidString("","[").Trim();
                string strParamValue = strParam.MidString("[","]", EndStringSearchMode.FromTailAndToEndWhenUnMatch).Trim();
                switch (strParamKey)
                {
                    case "DateTime":
                        if (strParamValue == "Today" && strValue.IsToday() ||
                           strParamValue == "Yesterday" && strValue.IsToday() ||
                           strParamValue == "Tomorrow" && strValue.IsToday())
                        {
                            vsb = Visibility.Visible;
                        }
                        break;
                    case "BitArray":
                        int bitIndex = strParamValue.ToMyInt();
                        if (strValue.MidString(bitIndex, 1).ToMyBool() == true)
                            vsb = Visibility.Visible;
                        break;
                    case "Equals":
                        if (strParamValue == strValue)
                            vsb = Visibility.Visible;
                        break;
                    case "Contains":
                        if (strValue.MyContains(strParamValue,",") || strValue.MyContains(strParamValue, "|"))
                            vsb = Visibility.Visible;
                        break;
                    case "NotContains":
                        if (!strValue.MyContains(strParamValue, ",") && !strValue.MyContains(strParamValue, "|"))
                            vsb = Visibility.Visible;
                        break;
                    case "Len":
                        if (strParamValue.Contains("=") && strValue.MyLength() == strParam.MidString("=", "").ToMyInt())
                        {
                            vsb = Visibility.Visible;
                        }
                        else if (strParamValue.Contains(">=") && strValue.MyLength() >= strParam.MidString(">=", "").ToMyInt())
                        {
                            vsb = Visibility.Visible;
                        }
                        else if (strParamValue.Contains("<=") && strValue.MyLength() <= strParam.MidString("<=", "").ToMyInt())
                        {
                            vsb = Visibility.Visible;
                        }
                        else if (strParamValue.Contains(">") && strValue.MyLength() > strParam.MidString(">", "").ToMyInt())
                        {
                            vsb = Visibility.Visible;
                        }
                      
                        else if (strParamValue.Contains("<") && strValue.MyLength() < strParam.MidString("<", "").ToMyInt())
                        {
                            vsb = Visibility.Visible;
                        }
                        break;

                    default:
                        if (strParam == strValue)
                            vsb = Visibility.Visible;
                        break;
                }
            }
            return vsb;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    /// <summary>
    /// Int转换可见性
    /// </summary>
    public class IntToVisible : IValueConverter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int iVal = value.ToMyInt();
            return iVal > 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    /// <summary>
    /// 字符串转换颜色
    /// ex: Converter={StaticResource StringToBrush}, ConverterParameter='BitArray[7] | #FFF4F4F5 | #FFFF380C'  // value = 1110010 
    /// ex: Converter={StaticResource StringToBrush}, ConverterParameter='Equals[已到时] | #FFF4F4F5 | #FFFF380C'
    /// ex: Converter={StaticResource StringToBrush}, ConverterParameter='Contains[已到时,已入样,待制样] | #FFF4F4F5 | #FFFF380C'
    /// ex: Converter={StaticResource StringToBrush}, ConverterParameter='Len[>0] | #FFF4F4F5 | #FFFF380C'}   // > >= < <=  =
    /// </summary>
    public class StringToBrush : IValueConverter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"> BitArray[0] Equals[0] Contains[ABC]|#FFF0F0F0|#FFF0F0F0 </param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                SolidColorBrush scb = Brushes.Transparent;
                List<string> LstParam = parameter.ToMyString().MySplit("|");
                if (parameter == null || LstParam.MyCount() != 3)
                    return scb;
                string strValue = value.ToMyString();
                string bindColorString = LstParam[1];
                string defaultColorString = LstParam[2];
                string strParam1 = LstParam[0];
                scb = new SolidColorBrush((Color)ColorConverter.ConvertFromString(defaultColorString));
                string strParamKey = strParam1.MidString("", "[").Trim();
                string strParamValue = strParam1.MidString("[", "]", EndStringSearchMode.FromTailAndToEndWhenUnMatch).Trim();
                switch (strParamKey)
                {
                    case "BitArray":
                        int bitIndex = strParamValue.ToMyInt();
                        if (strValue.MidString(bitIndex, 1).ToMyBool() == true)
                           scb = new SolidColorBrush((Color)ColorConverter.ConvertFromString(bindColorString));
                        break;
                    case "Equals":
                        if (strParamValue == strValue)
                            scb = new SolidColorBrush((Color)ColorConverter.ConvertFromString(bindColorString));
                        break;
                    case "Contains":
                        if (strValue.MyContains(strParamValue,","))
                            scb = new SolidColorBrush((Color)ColorConverter.ConvertFromString(bindColorString));
                        break;
                    case "Len":
                        if (strParamValue.Contains("=") && strValue.MyLength() == strParamValue.MidString("=", "").ToMyInt())
                        {
                            scb = new SolidColorBrush((Color)ColorConverter.ConvertFromString(bindColorString));
                        }
                        else if (strParamValue.Contains(">=") && strValue.MyLength() >= strParamValue.MidString(">=", "").ToMyInt())
                        {
                            scb = new SolidColorBrush((Color)ColorConverter.ConvertFromString(bindColorString));
                        }
                        else if (strParamValue.Contains("<=") && strValue.MyLength() <= strParamValue.MidString("<=", "").ToMyInt())
                        {
                            scb = new SolidColorBrush((Color)ColorConverter.ConvertFromString(bindColorString));
                        }
                        else if (strParamValue.Contains(">") && strValue.MyLength() > strParamValue.MidString(">", "").ToMyInt())
                        {
                            scb = new SolidColorBrush((Color)ColorConverter.ConvertFromString(bindColorString));
                        }

                        else if (strParamValue.Contains("<") && strValue.MyLength() < strParamValue.MidString("<", "").ToMyInt())
                        {
                            scb = new SolidColorBrush((Color)ColorConverter.ConvertFromString(bindColorString));
                        }
                        break;
                }
                return scb;
            }
            catch (Exception)
            {
                return Brushes.Transparent;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    /// <summary>
    /// ex: Converter={StaticResource StringToChecked}, ConverterParameter=Equals[页面1]
    /// ex: Converter={StaticResource StringToChecked}, ConverterParameter=BitArray[00100]
    /// ex: Converter={StaticResource StringToChecked}, ConverterParameter=Contains[ABC]
    /// </summary>
    public class StringToChecked : IValueConverter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"> BitArray[0] Equals[0] Contains[ABC] </param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool? bChecked = default(bool?);
            string strValue = value.ToMyString();
            if (parameter == null)
            {
                bChecked = strValue.ToMyBool();
            }
            else
            {
                string strParam = parameter.ToMyString();
                string strParamKey = strParam.MidString("", "[").Trim();
                string strParamValue = strParam.MidString("[", "]", EndStringSearchMode.FromTailAndToEndWhenUnMatch).Trim();
                switch (strParamKey)
                {
                    case "BitArray":
                        int bitIndex = strParamValue.ToMyInt();
                        bChecked = strValue.MidString(bitIndex, 1).ToMyBool();
                        break;
                    case "Equals":
                        bChecked = strParamValue == strValue;
                        break;
                    case "Contains":
                        bChecked = strValue.MyContains(strParamValue, ",");
                        break;
                    default:
                        bChecked = strParam == strValue;
                        break;
                }
            }
            return bChecked;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string strParam = parameter.ToMyString();
            string strParamKey = strParam.MidString("", "[").Trim();
            string strParamValue = strParam.MidString("[", "]", EndStringSearchMode.FromTailAndToEndWhenUnMatch).Trim();
            if (value.ToMyBool() == true && strParamKey == "Equals")
                return strParamValue;
            return string.Empty;
        }
    }

    /// <summary>
    /// 字符串转换可见性
    /// </summary>
    public class StringToUnChecked : IValueConverter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"> BitArray[0] Equals[0] Contains[ABC] </param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool? bChecked = default(bool?);
            string strValue = value.ToMyString();
            if (parameter == null)
            {
                bChecked = strValue.ToMyBool();
            }
            else
            {
                string strParam = parameter.ToMyString();
                string strParamKey = strParam.MidString("", "[").Trim();
                string strParamValue = strParam.MidString("[", "]", EndStringSearchMode.FromTailAndToEndWhenUnMatch).Trim();
                switch (strParamKey)
                {
                    case "BitArray":
                        int bitIndex = strParamValue.ToMyInt();
                        bChecked = strValue.MidString(bitIndex, 1).ToMyBool();
                        break;
                    case "Equals":
                        bChecked = strParamValue == strValue;
                        break;
                    case "Contains":
                        bChecked = strValue.MyContains(strParamValue, ",");
                        break;
                    default:
                        bChecked = strParam == strValue;
                        break;
                }
            }
            if (bChecked != null)
                bChecked = !bChecked;
            return bChecked;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }

    /// <summary>
    /// 字符串条件判别转换器
    /// ex:  Converter={StaticResource StringIFS}},ConverterParameter='SelByLen[:,]'                           //Len>0 返回: 否则返回空
    /// ex:  Converter={StaticResource StringIFS}},ConverterParameter='IFS[Cond1|Val1|Cond2|Val2|Cond3|Val3]'  //条件|值 ...
    /// </summary>
    public class StringIFS : IValueConverter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter">SelByLen[:,]</param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                string strValue = value.ToMyString();
                string strParam = parameter.ToMyString();
                string strParamKey = strParam.MidString("", "[").Trim();
                string strParamValue = strParam.MidString("[", "]", EndStringSearchMode.FromTailAndToEndWhenUnMatch,false);
                string strRet = string.Empty;
                switch (strParamKey)
                {
                    case "SelByLen":
                        string[] LstSel = strParamValue.Split(',');
                        if(LstSel.MyCount()==2)
                            strRet = string.IsNullOrEmpty(strValue) ? LstSel[1] : LstSel[0];
                        break;
                    case "IFS":
                        LstSel = strParamValue.Split('|');
                        for (int i = 0; i < LstSel.Length; i = i + 2)
                        {
                            List<string> LstIf = LstSel[i].MySplit("|");
                            if (LstIf.MyCount() == 0)
                                return LstSel[i + 1];
                            if (LstIf.MyContains(strValue) && !string.IsNullOrEmpty(strValue))
                                return LstSel[i + 1];
                        }
                        break;
                }
                return strRet;
            }
            catch (Exception )
            {
                return string.Empty;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }

    /// <summary>
    /// 格式化字符串转换器
    /// ex:  Converter={StaticResource StringFormat}},ConverterParameter='{0}个'  
    /// </summary>
    public class StringFormat : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                string strValue = string.Empty;
                if (value is EditMode edm)
                {
                    strValue = edm == EditMode.AddNew ? "添加" : "修改";
                }
                else
                {
                     strValue= value.ToMyString();
                }
                string strParam = parameter.ToMyString().Trim();
                if (string.IsNullOrEmpty(strValue))
                    return string.Empty;
                return string.Format(strParam, strValue);
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }

    /// <summary>
    /// 格式化字符串转换器
    /// ex:  Converter={StaticResource StringFormat}},ConverterParameter='{0}个'  
    /// </summary>
    public class MultiStringFormat : IMultiValueConverter
    {
        /*
         <TextBlock.Text>
             <MultiBinding Converter={StaticResource MultiStringFormat}},ConverterParameter='{0}{1}个'>
                 <Binding Path="FirstName" />
                 <Binding Path="LastName" />
             </MultiBinding>
         </TextBlock.Text>
        */
        public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
        {
            string strParam = parameter.ToMyString().Trim();
            if (strParam.MatchParamCount() != value.Length)
                return string.Empty;
            return string.Format(strParam, value);
        }

        public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class IntToBoolArrayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (!(value.ToMyInt() is int intValue) || !(parameter.ToMyInt() is int bitIndex))
                    return false;
                // 将整数值转换为二进制并填充前导零
                var binaryString = System.Convert.ToString(intValue, 2).PadLeft(32, '0');
                // 获取指定二进制位索引的布尔值
                var bitValue = binaryString.ElementAtOrDefault(31 - bitIndex) == '1';
                return bitValue;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (!(value is bool isChecked) || !(parameter.ToMyInt() is int bitIndex))
                    return Binding.DoNothing;
                // 获取当前整数值
                //if (!(targetType == typeof(int)))
                //    return Binding.DoNothing;
                var intValue = value.ToMyInt();
                // 将指定二进制位设置为对应的布尔值
                var binaryString = intValue.ToMyString().PadLeft(32, '0').ToCharArray();
                binaryString[31 - bitIndex] = isChecked ? '1' : '0';
                intValue = System.Convert.ToInt32(new string(binaryString), 2);
                return intValue;
            }
            catch (Exception)
            {
                return Binding.DoNothing;
            }
        }
    }

    /// <summary>
    /// Int转Bool 索引数组，按位返回
    /// 1 - 1000 
    /// 2 - 0100
    /// 3 - 0010
    /// 4 - 0001
    /// </summary>
    public class IntToIndexConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (!(value.ToMyInt() is int intValue) || !(parameter.ToMyInt() is int bitIndex))
                    return false;
                // 将整数值转换为二进制并填充前导零
                int iValue = value.ToMyInt();
                string strBits = "1";
                if (iValue >= 1 && iValue <= 32)
                    strBits = strBits.PadRight(32 - iValue + 1, '0').PadLeft(32,'0');
                if (strBits.Length != 32 || bitIndex<1 || bitIndex>32) return false;
                // 获取指定二进制位索引的布尔值
                var bitValue = strBits.MidString(bitIndex-1,1) == "1";
                return bitValue;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (!(value is bool isChecked) || !(parameter.ToMyInt() is int bitIndex))
                    return Binding.DoNothing;
                int iValue = parameter.ToMyInt();
                if(iValue<1 || iValue>32)
                    return Binding.DoNothing;
                return iValue;
            }
            catch (Exception)
            {
                return Binding.DoNothing;
            }
        }
    }

    /// <summary>
    /// 值控制GridLength 为* 自适应屏
    /// </summary>
    public class IntToGridLengthStar : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (!(value.ToMyInt() is int intValue) || !(parameter.ToMyInt() is int bitIndex))
                    return false;
                // 将整数值转换为二进制并填充前导零
                int iValue = value.ToMyInt();
                string strBits = "1";
                if (iValue >= 1 && iValue <= 32)
                    strBits = strBits.PadRight(32 - iValue + 1, '0').PadLeft(32, '0');
                if (strBits.Length != 32 || bitIndex < 1 || bitIndex > 32) return false;
                // 获取指定二进制位索引的布尔值
                var bitValue = strBits.MidString(bitIndex - 1, 1) == "1";
                return bitValue ? new GridLength(1, GridUnitType.Star) : GridLength.Auto;
            }
            catch (Exception)
            {
                return GridLength.Auto;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }

    /// <summary>
    /// 页面切片器选中转换器
    /// [当前页][设置页号]  相等返回 true 
    /// </summary>
    public class PageSlicerSelectedConverter : IMultiValueConverter
    {
        public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
        {
            bool IsChecked = false;
            if (value.Length == 2)
            {
                string CurrentPageID = value[0].ToMyString();
                string ObjectPageID = value[1].ToMyString();
                IsChecked = CurrentPageID == ObjectPageID && !string.IsNullOrEmpty(ObjectPageID);
            }
            return IsChecked;
        }

        public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    /*
       <Grid Grid.Row="1" Background="White" Visibility="{Binding IsChecked,ElementName=r2, Converter={StaticResource VisibleConvert}}">
                    <ContentControl ContentTemplate="{Binding IsChecked,ElementName=r2, Converter={StaticResource DataTemplateConverter},IsAsync=True,
                        ConverterParameter='Source=/Assets/Resource/DataTemplate.xaml,True=DataQueryTemplate'}"/>
       </Grid>
    */
    /// <summary>
    /// ex:  Converter={StaticResource DataTemplateConverter}},ConverterParameter='Source=xxx.xaml,key1=val1,key2=val2'  //条件|值 ...
    /// </summary>
    public class DataTemplateConverter : IValueConverter
    {
        private string LastDataTemplate = null;
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                List<string> LstExp = parameter.ToMyString().MySplit(",");
                string strSource = parameter.ToMyString().MidString("Source=", ",").Trim();
                if (string.IsNullOrEmpty(strSource)) return null;
                string strValue = value.ToMyString();
                ResourceDictionary resourceDict = new ResourceDictionary();
                resourceDict.Source = new Uri(strSource, UriKind.RelativeOrAbsolute);
                foreach (var item in LstExp)
                {
                    string strKey = item.MidString("", "=").Trim();
                    string strVal = item.MidString("=", "").Trim();
                    if (strKey.Contains("Source"))
                        continue;
                    if (strKey == strValue)
                    {
                        if (strVal != LastDataTemplate)
                        {
                            LastDataTemplate = strVal;
                            return resourceDict[strVal] as DataTemplate;
                        }
                        else
                            return Binding.DoNothing;
                    }
                }
                return Binding.DoNothing;
            }
            catch (Exception)
            {
                return Binding.DoNothing;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 字符串转资源
    /// </summary>
    public class StringToResource : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                string strResourceKey = value.ToMyString();
                object objResource = Application.Current.FindResource(strResourceKey);
                return objResource;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 显示样号转换器
    /// </summary>
    public class ViewSIConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string strViewLabel = value.ToMyString();
            return strViewLabel.MidString("", SystemDefault.ViewSplitSign, EndStringSearchMode.FromTailAndToEndWhenUnMatch);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class DoubleF1 : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double dVal = value.ToMyDouble();
            return dVal.ToString("F1");
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }

    public class DoubleF2 : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double dVal = value.ToMyDouble();
            return dVal.ToString("F2");
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }

    public class DoubleF3 : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double dVal = value.ToMyDouble();
            return dVal.ToString("F3");
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }

    public class OpModeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string strValue = value.ToMyString();
            string strOpMode = string.Empty;
            switch (strValue)
            {
                case "0":
                    strOpMode = "自动模式";
                    break;
                case "1":
                    strOpMode = "手动模式";
                    break;
                default:
                    strOpMode = "自动模式";
                    break;
            }
            return strOpMode;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }

    public class CheckEnabled : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool IsChecked = value.ToMyBool() == true;
            return IsChecked;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool IsChecked = value.ToMyBool() == true;
            return IsChecked ? 1 : 0;
        }
    }

    #region 内容可见性
    public class ContentVisibleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string strValue = value.ToMyString();
            return strValue.Length > 0 ? Visibility.Visible : Visibility.Hidden;
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
    #endregion

    #region 状态可见性
    public class StateVisibleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string strValue = value.ToMyString();
            Visibility CanVisible = Visibility.Hidden;
            switch (strValue)
            {
                case "True":
                case "1":
                    CanVisible = Visibility.Visible;
                    break;
                case "False":
                case "0":
                    CanVisible = Visibility.Hidden;
                    break;
            }
            return CanVisible;
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }

    public class StateVisibleConverterInv : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string strValue = value.ToMyString();
            Visibility CanVisible = Visibility.Visible;
            switch (strValue)
            {
                case "True":
                case "1":
                    CanVisible = Visibility.Hidden;
                    break;
                case "False":
                case "0":
                    CanVisible = Visibility.Visible;
                    break;
            }
            return CanVisible;
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
    #endregion

    #region 机械手程序指针格式化
    public class RobotPPConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string strValue = value.ToMyString();
            return string.Format("机械手锁定【{0}】任务",strValue);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }

    public class SamplerPPConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string strValue = value.ToMyString();
            return string.Format("进样传输锁定【{0}】任务", strValue);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
    #endregion


    //0   皮带待命中
    //1..2    自动回零中
    //3   2#接料准备
    //4   1#接料准备
    //11.15   视觉检测
    //16..39  样品传输中
    //40..49  坏样弃样
    //50  皮带工作结束
    public class StepConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int iStep = value.ToMyInt();
            string strStepDesc = string.Empty;
            if (iStep == 0)
                strStepDesc = "皮带待命中";
            else if (iStep >= 1 && iStep <= 2)
                strStepDesc = "自动回零中";
            else if (iStep >= 3 && iStep <= 3)
                strStepDesc = "2#接料准备";
            else if (iStep >= 4 && iStep <= 4)
                strStepDesc = "1#接料准备";
            else if (iStep >= 11 && iStep <= 15)
                strStepDesc = "视觉检测";
            else if (iStep >= 16 && iStep <= 39)
                strStepDesc = "样品传输中";
            else if (iStep >= 40 && iStep <= 49)
                strStepDesc = "坏样弃样";
            else if (iStep >= 50 && iStep <= 50)
                strStepDesc = "皮带工作结束";
            else
                strStepDesc = "";
            return strStepDesc;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
}
