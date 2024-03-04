using Engine.Data.DBFAC;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Engine.Common
{
    /// <summary>
    /// 数据类型转换
    /// 枚举、字符串、数据
    /// </summary>
    public static partial class sCommon
    {
        /// <summary>
        ///  枚举字段名称字符串转枚举对象
        /// </summary>
        /// <typeparam name="T">枚举</typeparam>
        /// <param name="EnumName">字符串</param>
        /// <returns>转换的枚举对象</returns>
        public static T ToEnum<T>(this string EnumName) where T : struct
        {
            T Obj;
            if (Enum.TryParse(EnumName, true, out Obj))
            {
                //if (Enum.IsDefined(typeof(T), Obj) | Obj.ToMyString().Contains(","))
                //{
                
                //}
            }
            return Obj;
        }

        /// <summary>
        /// 枚举字段名称字符串转枚举值
        /// </summary>
        /// <typeparam name="T">枚举类型</typeparam>
        /// <param name="EnumName">枚举字段名称</param>
        /// <returns> SystemDefault.InValidInt 转换失败</returns>
        public static int ToEnumVal<T>(this string EnumName)
        {
            try
            {
                object obj = Enum.Parse(typeof(T), EnumName, true);
                if (obj != null)
                    return (int)obj;
            }
            catch (Exception )
            {
            }
            return SystemDefault.InValidInt;
        }

        /// <summary>
        /// 枚举字段名称字符串转枚举值
        /// </summary>
        /// <typeparam name="T">枚举类型</typeparam>
        /// <param name="EnumName">枚举字段名称</param>
        /// <returns> 失败：转换为空 </returns>
        public static string ToEnumStrVal<T>(this string EnumName)
        {
            int iVal = EnumName.ToEnumVal<T>();
            if (iVal == SystemDefault.InValidInt)
                return string.Empty;
            return iVal.ToString();
        }

        /// <summary>
        /// 枚举字段对象转枚举值
        /// </summary>
        /// <param name="EnumObj"></param>
        /// <returns>-1: 转换失败</returns>
        public static int ToEnumVal(this Enum EnumObj)
        {
            try
            {
                string EnumName = EnumObj.ToString();
                Type type = EnumObj.GetType();
                object obj = Enum.Parse(type, EnumName, true);
                if (obj != null)
                    return (int)obj;
            }
            catch (Exception )
            {
            }
            return SystemDefault.InValidInt;
        }

        /// <summary>
        /// 枚举字段对象转枚举值字符串
        /// </summary>
        /// <param name="EnumObj"></param>
        /// <returns>失败：转换为空</returns>
        public static string ToEnumStrVal(this Enum EnumObj)
        {
            int iVal = EnumObj.ToEnumVal();
            if (iVal == SystemDefault.InValidInt)
                return string.Empty;
            return iVal.ToString();
        }

        /// <summary>
        /// 字节数组转字符串
        /// </summary>
        /// <param name="byArray"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string ToMyString(this byte[] byArray, Encoding encoding)
        {
            if (encoding == null)
                encoding = Encoding.Default;
            if (byArray == null)
                return string.Empty;
            return encoding.GetString(byArray);
        }

        /// <summary>
        /// 转化为字符串，优点：null型变量字符转换自动排除异常
        /// </summary>
        /// <param name="obj">转换对象</param>
        /// <param name="DefaultStringIfNull">Null值返回字符串</param>
        /// <returns></returns>
        public static string ToMyString(this object obj,string DefaultStringIfNull)
        {
            if (obj == null)
                return DefaultStringIfNull;
            if (string.IsNullOrEmpty(obj.ToString()))
                return DefaultStringIfNull;
            return obj.ToString(); 
        }

        /// <summary>
        /// 转化为字符串，优点：null型变量字符转换自动排除异常
        /// </summary>
        /// <param name="obj">转换对象</param>
        /// <param name="DefaultStringIfNull">Null值返回字符串</param>
        /// <returns></returns>
        public static string ToMyString(this object obj)
        {
            if (obj == null)
                return string.Empty;
            return obj.ToString();
        }

        /// <summary>
        /// 转化为定长分割字符串
        /// ex: abcdefg -> abc-def-g
        /// </summary>
        /// <param name="strSource"></param>
        /// <param name="PartCharNum"></param>
        /// <param name="SplitStr"></param>
        /// <returns></returns>
        public static string ToMyString(this string strSource,int PartCharNum, string SplitStr = "-")
        {
            if (string.IsNullOrEmpty(strSource))
                return string.Empty;
            string strResult = string.Empty;
            for (int i = 0; i < strSource.Length; i = i + PartCharNum)
            {
                if (strResult.Length > 0)
                    strResult += SplitStr;
                strResult += strSource.MidString(i, PartCharNum);
            }
            return strResult;
        }

        /// <summary>
        /// 时间格式化
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string ToMyString(this DateTime dateTime, string format)
        {
            string formattedDateTime = format;
            // 年
            if (format.Contains("yyyy"))
            {
                formattedDateTime = formattedDateTime.Replace("yyyy", dateTime.Year.ToString("0000"));
            }
            else if (format.Contains("yy"))
            {
                formattedDateTime = formattedDateTime.Replace("yy", (dateTime.Year % 100).ToString("00"));
            }
            // 月
            if (format.Contains("MM"))
            {
                formattedDateTime = formattedDateTime.Replace("MM", dateTime.Month.ToString("00"));
            }
            else if (format.Contains("M"))
            {
                formattedDateTime = formattedDateTime.Replace("M", dateTime.Month.ToString());
            }
            // 日
            if (format.Contains("dd"))
            {
                formattedDateTime = formattedDateTime.Replace("dd", dateTime.Day.ToString("00"));
            }
            else if (format.Contains("d"))
            {
                formattedDateTime = formattedDateTime.Replace("d", dateTime.Day.ToString());
            }
            // 时
            if (format.Contains("HH"))
            {
                formattedDateTime = formattedDateTime.Replace("HH", dateTime.Hour.ToString("00"));
            }
            else if (format.Contains("H"))
            {
                formattedDateTime = formattedDateTime.Replace("H", dateTime.Hour.ToString());
            }
            else if (format.Contains("hh"))
            {
                formattedDateTime = formattedDateTime.Replace("hh", dateTime.Hour.ToString("00"));
            }
            else if (format.Contains("h"))
            {
                int hour12Format = dateTime.Hour > 12 ? dateTime.Hour - 12 : dateTime.Hour;
                formattedDateTime = formattedDateTime.Replace("h", hour12Format.ToString());
            }
            // 分
            if (format.Contains("mm"))
            {
                formattedDateTime = formattedDateTime.Replace("mm", dateTime.Minute.ToString("00"));
            }
            else if (format.Contains("m"))
            {
                formattedDateTime = formattedDateTime.Replace("m", dateTime.Minute.ToString());
            }
            // 秒
            if (format.Contains("ss"))
            {
                formattedDateTime = formattedDateTime.Replace("ss", dateTime.Second.ToString("00"));
            }
            else if (format.Contains("s"))
            {
                formattedDateTime = formattedDateTime.Replace("s", dateTime.Second.ToString());
            }
            return formattedDateTime;
        }

        /// <summary>
        /// 时间格式化
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string ToMyStringOfDataTime(this string strDateTime, string format)
        {
            string FormattedDateTimeString = string.Empty;
            if (DateTime.TryParse(strDateTime, out DateTime dt))
            {
                FormattedDateTimeString = dt.ToMyString(format);
            }
            return FormattedDateTimeString;
        }

        /// <summary>
        /// 列表中各个元素合并成一个字符串
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="LstSource"></param>
        /// <param name="FieldSplit">分割子串</param>
        /// <param name="RemoveEmpty">是否移除空元素</param>
        /// <param name="FieldPrev">字段拼接附加前缀</param>
        /// <param name="FieldTail">字段拼接附加后缀</param>
        /// <returns></returns>
        public static string ToMyString<T>(this List<T> LstSource, string FieldSplit,
            bool RemoveEmpty = true, string FieldPrev = "", string FieldTail = "")
        {
            try
            {
                if (LstSource == null)
                    return string.Empty;
                string strRet = string.Empty;
                foreach (T item in LstSource)
                {
                    string strItem = item.ToMyString();
                    if (RemoveEmpty && string.IsNullOrEmpty(strItem))
                        continue;
                    if (!string.IsNullOrEmpty(strRet))
                        strRet += FieldSplit;
                    strRet += string.Format("{0}{1}{2}", FieldPrev, strItem, FieldTail);
                }
                return strRet;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 转半角的函数(DBC case)
        /// 全角空格为12288，半角空格为32
        /// 其他字符半角(33-126)与全角(65281-65374)的对应关系是：均相差65248
        /// </summary>
        /// <param name="input">/任意字符串</param>
        /// <returns>半角字符串</returns>
        public static string ToDBC(this string input)
        {
            try
            {
                if (string.IsNullOrEmpty(input))
                    return string.Empty;
                char[] c = input.ToCharArray();
                for (int i = 0; i < c.Length; i++)
                {
                    if (c[i] == 12288)
                    {
                        c[i] = (char)32;
                        continue;
                    }
                    if (c[i] > 65280 && c[i] < 65375)
                        c[i] = (char)(c[i] - 65248);
                }
                return new string(c);
            }
            catch (Exception )
            {
                return input;
            }
           
        }

        /// <summary>
        /// Unicode 字体处理
        /// </summary>
        /// <param name="unicodeStr"></param>
        /// <returns></returns>
        public static string UnicodeToString(this string unicodeStr)
        {
            string outStr = "";
            if (!string.IsNullOrEmpty(unicodeStr))
            {
                string[] strlist = unicodeStr.Replace("&#", "").Replace(";", "").Split('x');
                try
                {
                    for (int i = 1; i < strlist.Length; i++)
                    {
                        outStr += (char)int.Parse(strlist[i], System.Globalization.NumberStyles.HexNumber);
                    }
                }
                catch (Exception ex)
                {
                    outStr = ex.Message;
                }
            }
            return outStr;
        }

        /// <summary>
        /// 获取字符串的字节数
        /// Unicode字符： 2个字节
        /// ASCII字符：   1个字节
        /// </summary>
        /// <param name="Source"></param>
        /// <returns></returns>
        public static int GetByteCnt(this string Source)
        {
            if (string.IsNullOrEmpty(Source)) return 0;
            byte[] byArr = Encoding.Default.GetBytes(Source);
            if (byArr == null) return 0;
            return byArr.Length;
        }

        /// <summary>
        /// 数据库字段写入前格式化
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToMyFieldString(this object obj)
        {
            string strFieldVal = obj.ToMyString();//.Replace("'", "''");
            return strFieldVal;
        }

        /// <summary>
        /// 转换为Byte
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static byte ToMyByte(this object obj)
        {
            return ToMyByte(obj.ToMyString());
        }

        /// <summary>
        /// 转换为Byte
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static byte ToMyByte(this string str)
        {
            byte ret = 0;
            byte.TryParse(str, out ret);
            return ret;
        }

        /// <summary>
        /// 转换为整型数据
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static int ToMyInt(this string str)
        {
            int ret = 0;
            int.TryParse(str, out ret);
            return ret;
        }

        /// <summary>
        /// 递增函数
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static string MyInc(this object model)
        {
            int i = model.ToMyInt();
            return (i++).ToString();
        }

        /// <summary>
        /// 转换为整型数据
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static int ToMyInt(this object obj)
        {
            if (obj is bool bObj)
                return bObj ? 1 : 0;
            int ret = 0;
            string str = obj.ToMyString();
            int.TryParse(str, out ret);
            return ret;
        }

        /// <summary>
        /// 转换为整型数据
        /// </summary>
        /// <param name="str"></param>
        /// <param name="MinVal"></param>
        /// <param name="MaxVal"></param>
        /// <returns></returns>
        public static int ToMyInt(this object obj, int MinVal, int MaxVal)
        {
            int ret = 0;
            int Val = 0;
            bool bRet = int.TryParse(obj.ToMyString(), out Val);
            if (!bRet)
                return MinVal;
            if (Val < MinVal)
                ret = MinVal;
            else if (Val > MaxVal)
                ret = MaxVal;
            else
                ret = Val;
            return ret;
        }

        /// <summary>
        /// 强制时间处理返回预期
        /// </summary>
        /// <param name="strTime">时间字符串</param>
        /// <param name="strFormat">格式化</param>
        /// <returns></returns>
        public static string ToMyDateTimeStr(this string strTime, string strFormat)
        {
            DateTime dateTime = default(DateTime);
            DateTime.TryParse(strTime, out dateTime);
            string strDateTime = dateTime.ToString(strFormat, DateTimeFormatInfo.InvariantInfo);
            if (strDateTime.Contains("0001"))
                strDateTime = string.Empty;
            return strDateTime;
        }

        /// <summary>
        /// 强制时间处理返回预期
        /// </summary>
        /// <param name="strTime">时间字符串</param>
        /// <param name="strFormat">格式化</param>
        /// <returns></returns>
        public static string ToMyDateTimeStr(this DateTime dateTime, string strFormat)
        {
            string strDateTime = dateTime.ToString(strFormat, DateTimeFormatInfo.InvariantInfo);
            if (strDateTime.Contains("0001"))
                strDateTime = string.Empty;
            return strDateTime;
        }

        /// <summary>
        /// 强制时间处理返回预期
        /// 返回系统默认格式
        /// </summary>
        /// <param name="strTime"></param>
        /// <returns></returns>
        public static string ToMyDateTimeStr(this string strTime)
        {
            return strTime.ToMyDateTimeStr(SystemDefault.DateTimeFormat);
        }

        /// <summary>
        /// 系统时间格式字符串
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static string ToMyDateTimeStr(this DateTime dateTime)
        {
            return dateTime.ToMyDateTimeStr(SystemDefault.DateTimeFormat);
        }

        /// <summary>
        /// 系统时间格式字符串
        /// </summary>
        /// <param name="objDateTime"></param>
        /// <returns></returns>
        public static string ToMyDateTimeStr(this object objDateTime)
        {
            string strDateTime = string.Empty;
            try
            {
                strDateTime = objDateTime.ToString();
            }
            catch (Exception )
            {
                
            }
            return strDateTime.ToMyDateTimeStr();
        }

        /// <summary>
        /// 时间格式转换
        /// </summary>
        /// <param name="strDateTime"></param>
        /// <param name="strFormat"></param>
        /// <returns>DateTime格式对象</returns>
        public static DateTime ToMyDateTime(this string strDateTime, string strFormat)
        {
            try
            {
                string strFormatDateTime= DateTime.Parse(strDateTime).ToString(strFormat);
                DateTime dateTime = DateTime.Parse(strFormatDateTime);
                return dateTime;
            }
            catch (Exception )
            {
                return default(DateTime);
            }
        }

        /// <summary>
        /// 时间格式转换
        /// 返回系统默认格式
        /// </summary>
        /// <param name="strDateTime"></param>
        /// <returns></returns>
        public static DateTime ToMyDateTime(this string strDateTime)
        {
            return strDateTime.ToMyDateTime(SystemDefault.DateTimeFormat);
        }

        /// <summary>
        /// 时间格式转换
        /// </summary>
        /// <param name="value">时间对象</param>
        /// <param name="strFormat"></param>
        /// <returns> DateTime格式对象 </returns>
        public static DateTime ToMyDateTime(this object value, string strFormat)
        {
            try
            {
                string strDateTime = value.ToString();
                return strDateTime.ToMyDateTime(strFormat);
            }
            catch (Exception )
            {
                return default(DateTime);
            }
        }

        /// <summary>
        /// 时间格式转换
        /// 返回系统默认格式
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static DateTime ToMyDateTime(this object value)
        {
            return value.ToMyDateTime(SystemDefault.DateTimeFormat);
        }

        /// <summary>
        /// 大写字母转化为数字
        /// </summary>
        /// <param name="Obj"></param>
        /// <returns></returns>
        public static string ToMyNumberByEnStr(this object Obj)
        {
            string strEn = Obj.ToString();
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < strEn.Length; i++)
            {
                if (strEn[i] >= 'A' && strEn[i] <= 'Z')
                    sb.Append((Char)strEn[i] - 'A' + 1);
            }
            return sb.ToString();
        }

        /// <summary>
        /// 转化为开关量
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool? ToMyBool(this object obj)
        {
            bool? ret = default(bool?);
            try
            {
                string strTryBool = obj.ToMyString().ToUpper();
                string[] ArrSourceTrue = new string[] { "1", "启用", "已启用", "真", "Y", "YES", "OK", "√", "TRUE" };
                string[] ArrSourceFalse = new string[] { "0", "未启用", "已禁用", "假", "N", "NO", "NG", "×", "FALSE","" };
                if (Array.IndexOf<string>(ArrSourceTrue, strTryBool) != -1)
                    return true;
                else if (Array.IndexOf<string>(ArrSourceFalse, strTryBool) != -1)
                    return false;
            }
            catch (Exception ) { }
            return ret;
        }

        /// <summary>
        /// 转化为开关量
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool ToMyBoolean(this object obj)
        {
            bool ret = false;
            try
            {
                string strTryBool = obj.ToMyString().ToUpper();
                string[] ArrSourceTrue = new string[] { "1", "启用", "已启用", "真", "Y", "YES", "OK", "√", "TRUE" };
                if (Array.IndexOf<string>(ArrSourceTrue, strTryBool) != -1)
                    return true;
            }
            catch (Exception) { }
            return ret;
        }

        /// <summary>
        /// 转换浮点数据
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static double ToMyDouble(this object obj)
        {
            return obj.ToMyDouble(0.00);
        }

        /// <summary>
        /// 转换浮点数据
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static double ToMyDouble(this object obj, double DefaultInValid)
        {
            double ret = 0;
            string str = obj.ToMyString();
            if (double.TryParse(str, out ret))
                return ret;
            else
                return DefaultInValid;
        }

        /// <summary>
        /// 转换浮点数据
        /// 带上下限
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="dDLmt"></param>
        /// <param name="dULmt"></param>
        /// <returns></returns>
        public static double ToMyDouble(this object obj, double? dDLmt = null, double? dULmt = null)
        {
            double dVal = obj.ToMyDouble();
            if (dVal < dDLmt)
                return (double)dDLmt;
            else if (dVal > dULmt)
                return (double)dULmt;
            return dVal;
        }

        /// <summary>
        /// 功能开关开启
        /// </summary>
        /// <param name="Source"></param>
        /// <returns></returns>
        public static bool IsFunctionOn(this object Source)
        {
            string strSource = Source.ToMyString().ToUpper();
            return strSource == "ON" || strSource == "1" || strSource == "TRUE";
        }
    }

    /// <summary>
    /// 数据格式转换
    /// </summary>
    public static partial class sCommon
    {
        /// <summary>
        /// 二进制字符串->32位整型
        /// Converts a binary string to Int32 value
        /// </summary>
        /// <param name="txt"></param>
        /// <returns></returns>
        public static int BinStringToInt32(this string txt)
        {
            int cnt = 0;
            int ret = 0;

            for (cnt = txt.Length - 1; cnt >= 0; cnt += -1)
            {
                if (int.Parse(txt.Substring(cnt, 1)) == 1)
                {
                    ret += (int)(Math.Pow(2, (txt.Length - 1 - cnt)));
                }
            }
            return ret;
        }

        /// <summary>
        /// 8位二进制字符串->字节
        /// Converts a binary string to a byte. Can return null.
        /// </summary>
        /// <param name="txt"></param>
        /// <returns></returns>
        public static byte? BinStringToByte(this string txt)
        {
            int cnt = 0;
            int ret = 0;

            if (txt.Length == 8)
            {
                for (cnt = 7; cnt >= 0; cnt += -1)
                {
                    if (int.Parse(txt.Substring(cnt, 1)) == 1)
                    {
                        ret += (int)(Math.Pow(2, (txt.Length - 1 - cnt)));
                    }
                }
                return (byte)ret;
            }
            return null;
        }

        /// <summary>
        /// 数值转为二进制字符串
        /// Converts the value to a binary string
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ValToBinString(this object value)
        {
            int cnt = 0;
            int cnt2 = 0;
            int x = 0;
            string txt = "";
            long longValue = 0;

            try
            {
                if (value.GetType().Name.IndexOf("[]") < 0)
                {
                    // ist nur ein Wert
                    switch (value.GetType().Name)
                    {
                        case "Byte":
                            x = 7;
                            longValue = (long)((byte)value);
                            break;
                        case "Int16":
                            x = 15;
                            longValue = (long)((Int16)value);
                            break;
                        case "Int32":
                            x = 31;
                            longValue = (long)((Int32)value);
                            break;
                        case "Int64":
                            x = 63;
                            longValue = (long)((Int64)value);
                            break;
                        default:
                            throw new Exception();
                    }

                    for (cnt = x; cnt >= 0; cnt += -1)
                    {
                        if (((Int64)longValue & (Int64)Math.Pow(2, cnt)) > 0)
                            txt += "1";
                        else
                            txt += "0";
                    }
                }
                else
                {
                    // ist ein Array
                    switch (value.GetType().Name)
                    {
                        case "Byte[]":
                            x = 7;
                            byte[] ByteArr = (byte[])value;
                            for (cnt2 = 0; cnt2 <= ByteArr.Length - 1; cnt2++)
                            {
                                for (cnt = x; cnt >= 0; cnt += -1)
                                    if ((ByteArr[cnt2] & (byte)Math.Pow(2, cnt)) > 0) txt += "1"; else txt += "0";
                            }
                            break;
                        case "Int16[]":
                            x = 15;
                            Int16[] Int16Arr = (Int16[])value;
                            for (cnt2 = 0; cnt2 <= Int16Arr.Length - 1; cnt2++)
                            {
                                for (cnt = x; cnt >= 0; cnt += -1)
                                    if ((Int16Arr[cnt2] & (byte)Math.Pow(2, cnt)) > 0) txt += "1"; else txt += "0";
                            }
                            break;
                        case "Int32[]":
                            x = 31;
                            Int32[] Int32Arr = (Int32[])value;
                            for (cnt2 = 0; cnt2 <= Int32Arr.Length - 1; cnt2++)
                            {
                                for (cnt = x; cnt >= 0; cnt += -1)
                                    if ((Int32Arr[cnt2] & (byte)Math.Pow(2, cnt)) > 0) txt += "1"; else txt += "0";
                            }
                            break;
                        case "Int64[]":
                            x = 63;
                            byte[] Int64Arr = (byte[])value;
                            for (cnt2 = 0; cnt2 <= Int64Arr.Length - 1; cnt2++)
                            {
                                for (cnt = x; cnt >= 0; cnt += -1)
                                    if ((Int64Arr[cnt2] & (byte)Math.Pow(2, cnt)) > 0) txt += "1"; else txt += "0";
                            }
                            break;
                        default:
                            throw new Exception();
                    }
                }
                return txt;
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// 字节中索引位状态
        /// Example: DB1.DBX0.5 -> var bytes = ReadBytes(DB1.DBW0); bool bit = bytes[0].SelectBit(5); 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="bitPosition"></param>
        /// <returns></returns>
        public static bool SelectBit(this byte data, int bitPosition)
        {
            int mask = 1 << bitPosition;
            int result = data & mask;
            return result != 0;
        }

        /// <summary>
        /// Converts from ushort value to short value; it's used to retrieve negative values from words
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static short ConvertToShort(this ushort input)
        {
            short output;
            output = short.Parse(input.ToString("X"), NumberStyles.HexNumber);
            return output;
        }

        /// <summary>
        /// Converts from short value to ushort value; it's used to pass negative values to DWs
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static ushort ConvertToUshort(this short input)
        {
            ushort output;
            output = ushort.Parse(input.ToString("X"), NumberStyles.HexNumber);
            return output;
        }

        /// <summary>
        /// Converts from UInt32 value to Int32 value; it's used to retrieve negative values from DBDs
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static Int32 ConvertToInt(this uint input)
        {
            int output;
            output = int.Parse(input.ToString("X"), NumberStyles.HexNumber);
            return output;
        }

        /// <summary>
        /// Converts from Int32 value to UInt32 value; it's used to pass negative values to DBDs
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static UInt32 ConvertToUInt(this int input)
        {
            uint output;
            output = uint.Parse(input.ToString("X"), NumberStyles.HexNumber);
            return output;
        }

        /// <summary>
        /// byte[] List<byte> string 转换位ASCII编码
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToASCII(this object obj)
        {
            string ret = string.Empty;
            if (obj is byte[])
            {
                ret = Encoding.ASCII.GetString(obj as byte[]);
            }
            else if (obj is List<byte>)
            {
                ret = Encoding.ASCII.GetString((obj as List<byte>).ToArray());
            }
            else if (obj is string)
            {
                List<byte> by = new List<byte>();
                string[] strG = obj.ToMyString().Split(',');
                if (strG.Length > 0)
                {
                    foreach (string c in strG)
                    {
                        if (c.Trim() == "0" || c.Trim().Length == 0)
                            continue;
                        by.Add(byte.Parse(c));
                    }
                }
                ret = Encoding.ASCII.GetString(by.ToArray());
            }
            return ret;
        }

        /// <summary>
        /// 16进制字符串转byte数组  - 未编写
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private static byte[] HexStringToByteArray(string s)
        {
            List<Byte> ret = new List<byte>();

            return ret.ToArray();
        }

        /// <summary>
        /// 转换字符串内的中文
        /// </summary>
        /// <param name="strSource"></param>
        /// <returns></returns>
        private static string ConvertChinese(string strSource)
        {
            StringBuilder s = new StringBuilder();
            foreach (short c in strSource.ToCharArray())
            {
                if (c <= 0 || c >= 127)
                {
                    s.Append(c.ToString("X4"));
                }
                else
                {
                    s.Append((char)c);
                }
            }
            return s.ToString();
        }

        /// <summary>
        /// 字符串过滤中文
        /// </summary>
        /// <param name="strSource"></param>
        /// <returns></returns>
        private static string FilterChinese(string strSource)
        {
            StringBuilder s = new StringBuilder();
            foreach (short c in strSource.ToCharArray())
            {
                if (c > 0 && c < 127)
                {
                    s.Append((char)c);
                }
            }
            return s.ToString();
        }

        /// <summary>
        /// byte[] -> Hex格式空格分割字符串
        /// 0x12 0x01 0x02 0x02 -> "12 01 02 02"
        /// </summary>
        /// <param name="byData"></param>
        /// <returns></returns>
        public static string ByteArrayToHexString(this byte[] byData, string Format = "{0:X2}", string SplitStr = "")
        {
            if (byData == null)
                return string.Empty;
            string strHex = "";
            for (int i = 0; i < byData.Length; i++)
            {
                if (strHex.Length > 0 && SplitStr.Length > 0)
                    strHex += SplitStr;
                strHex += string.Format(Format, byData[i]);
            }
            return strHex.Trim();
        }

        /// <summary>
        /// 10进制整型值转16进制字符串
        /// 10 -> "0A"
        /// </summary>
        /// <param name="dec"></param>
        /// <param name="formatLen">格式化长度,不足左侧补0</param>
        /// <returns></returns>
        public static string DecToHex(this int dec, int formatLen = 2)
        {
            return dec.ToString($"X{formatLen}");
            //or: return Convert.ToString(dec,16);
        }

        /// <summary>
        /// 10进制整型值转16进制字符串
        /// </summary>
        /// <param name="dec"></param>
        /// <param name="Format">格式化</param>
        /// <returns></returns>
        public static string DecToHex(this int dec, string Format = "X2")
        {
            return dec.ToString(Format);
        }

        /// <summary>
        /// 10进制字符值转16进制字符串
        /// </summary>
        /// <param name="Format">格式</param>
        /// <returns></returns>
        public static string DecToHex(this string DecString, string Format = "X2")
        {
            int val = DecString.ToMyInt();
            //X: 转换后为大写 x:转换后为小写  数字:代表不足位数时左侧自动补零
            return val.ToString(Format);
            //or: return Convert.ToString(dec,16) //转换后默认为小写 ex: 1,239,988 -> 0x12ebb4
        }

        /// <summary>
        /// 十六进制数据转十进制
        /// </summary>
        /// <param name="HexString"></param>
        /// <returns></returns>
        public static int HexToInt(this string HexString)
        {
            return HexString.HexToInt(out string Error);
        }

        /// <summary>
        /// 十六进制数据转十进制
        /// </summary>
        /// <param name="HexString">十六进制值</param>
        /// <param name="Error">错误信息</param>
        /// <returns></returns>
        public static int HexToInt(this string HexString,out string Error)
        {
            try
            {
                Error = string.Empty;
                return Convert.ToInt32(HexString, 16);
            }
            catch (Exception ex)
            {
                Error = ex.Message;
                return -1;
            }
        }

        /// <summary>
        /// ASCII字符串转换为10进制数组
        /// </summary>
        /// <param name="charSymbol"></param>
        /// <param name="CharNum"></param>
        /// <returns></returns>
        public static int HexCharToInt(this char charSymbol)
        {
            int iData = Convert.ToInt32(charSymbol.ToString(), 16);
            return iData;
        }

        /// <summary>
        /// ASCII字符串 转 十六进制字符串
        /// ABCD -> "10111213"
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToHexString(this object obj)
        {
            string HexStrViewSI = string.Empty;
            char[] cViewSI = obj.ToMyString().ToCharArray();
            foreach (char s in cViewSI)
            {
                // Get the integral value of the character.
                int value = Convert.ToInt32(s);
                // Convert the decimal value to a hexadecimal value in string form.
                HexStrViewSI += String.Format("{0:X2}", value);
            }
            return HexStrViewSI;
        }

        /// <summary>
        /// 字符串转16进制字符数组  "21"-- 0x21
        /// </summary>
        /// <param name="hex"></param>
        /// <returns></returns>
        public static byte[] StringToHexByte(this string SourceString)
        {
            return StringToHexByte(SourceString, false);
        }

        /// <summary>
        /// 字符串转16进制字符数组 "21"-- 0x21
        /// </summary>
        /// <param name="str"></param>
        /// <param name="isFilterChinese">是否过滤掉中文字符</param>
        /// <returns></returns>
        public static byte[] StringToHexByte(this string SourceString, bool isFilterChinese)
        {
            string hex = isFilterChinese ? FilterChinese(SourceString) : ConvertChinese(SourceString);

            //清除所有空格
            hex = hex.Replace(" ", "");
            ////若字符个数为奇数，末尾补一个0
            //hex += hex.Length % 2 != 0 ? "0" : "";
            //若字符个数为奇数，头部补一个0
            hex = (hex.Length % 2 != 0 ? "0" : "") + hex;

            byte[] result = new byte[hex.Length / 2];
            for (int i = 0, c = result.Length; i < c; i++)
            {
                result[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
            }
            return result;
        }
    }

    /// <summary>
    /// 泛型、集合数据转换
    /// </summary>
    public static partial class sCommon
    {
        /// <summary>
        /// 将对象转换成表格
        /// </summary>
        /// <param name="ObjSource"></param>
        /// <returns></returns>
        public static DataTable ToMyDataTable(this object ObjSource)
        {
            DataTable dt = new DataTable();
            try
            {
                if (ObjSource is DataTable)
                    dt = ObjSource as DataTable;
                else if (ObjSource is DataView)
                    dt = (ObjSource as DataView).Table;
            }
            catch (Exception)
            {

            }
            return dt;
        }


        /// <summary>
        /// 获得数据项
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ObjSource"></param>
        /// <returns></returns>
        public static T ToMyDataItem<T>(this object ObjSource) where T : new()
        {
            T item = default(T);
            try
            {
                DataTable dt = new DataTable();
                if (ObjSource is DataTable)
                    dt = ObjSource as DataTable;
                else if (ObjSource is DataView)
                    dt = (ObjSource as DataView).Table;
                if (dt.Rows.Count > 0)
                    item = ColumnDef.ToEntity<T>(dt.Rows[0]);
            }
            catch (Exception)
            {

            }
            return item;
        }

        /// <summary>
        /// List转换成字典-主键去重
        /// </summary>
        /// <typeparam name="TElement"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="source"></param>
        /// <param name="keyGetter"></param>
        /// <param name="valueGetter"></param>
        /// <returns></returns>
        public static Dictionary<TKey, TValue> ToMyDictionary<TElement, TKey, TValue>(
            this IEnumerable<TElement> source,
            Func<TElement, TKey> keyGetter,
            Func<TElement, TValue> valueGetter)
        {
            Dictionary<TKey, TValue> dict = new Dictionary<TKey, TValue>();
            if (source == null) return dict;
            foreach (var e in source)
            {
                var key = keyGetter(e);
                if (valueGetter(e) == null || key == null)
                    continue;
                if (dict.ContainsKey(key))
                    continue;
                dict.Add(key, valueGetter(e));
            }
            return dict;
        }

        /// <summary>
        /// 获取排序字典，自定义比较器
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="Dictionary"></param>
        /// <param name="Comparer"></param>
        /// <returns></returns>
        public static IDictionary<TKey, TValue> ToMySortedDictionary<TKey, TValue>(this IDictionary<TKey, TValue> SourceDictionary, IComparer<TKey> Comparer)
        {
            if (SourceDictionary == null) return null;
            if (Comparer == null) return SourceDictionary;
            List<TKey> lstKey = new List<TKey>();
            foreach (var key in SourceDictionary.Keys)
                lstKey.Add(key);
            lstKey.Sort(Comparer);
            IDictionary<TKey, TValue> NewDic = new Dictionary<TKey, TValue>();
            foreach (TKey key in lstKey)
            {
                if (SourceDictionary.ContainsKey(key))
                    NewDic.Add(key, SourceDictionary.DictFieldValue(key));
            }
            return NewDic;
        }

        /// <summary>
        /// 表行转换成字典
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public static Dictionary<string, string> ToMyDictionary(this DataRow row)
        {
            Dictionary<string, string> Dic = new Dictionary<string, string>();
            if (row == null)
                return Dic;
            foreach (DataColumn col in row.Table.Columns)
            {
                string ColName = col.ColumnName.ToMyString();
                string RowData = row[ColName].ToMyString();
                Dic.AppandDict(ColName, RowData);
            }
            return Dic;
        }

        /// <summary>
        /// 表转换成字典
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public static List<Dictionary<string, string>> ToMyGenericDictionary(this DataTable table)
        {
            List<Dictionary<string, string>> LstDic = new List<Dictionary<string, string>>();
            if (table == null)
                return LstDic;
            foreach (DataRow row in table.Rows)
            {
                Dictionary<string, string> Dic = new Dictionary<string, string>();
                foreach (DataColumn col in table.Columns)
                {
                    string ColName = col.ColumnName.ToMyString();
                    string RowData = row[ColName].ToMyString();
                    Dic.AppandDict(ColName, RowData);
                }
                LstDic.Add(Dic);
            }
            return LstDic;
        }

        /// <summary>
        /// ObjSource转List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="SelectedIList"></param>
        /// <returns></returns>
        public static List<T> ToMyList<T>(this object ObjSource) where T : new()
        {
            List<T> MyList = new List<T>();
            try
            {
                if (ObjSource == null)
                    return MyList;
                else if (ObjSource is IList select)
                    MyList = select.Cast<T>().ToList();
                else if (ObjSource is IEnumerable enumner)
                    MyList = enumner.Cast<T>().ToList();
                else if (ObjSource is ICollection collection)
                    MyList = collection.Cast<T>().ToList();
                else if (ObjSource is ObservableCollection<T> observe)
                    MyList = observe.ToList();
                else if (ObjSource is DataTable table)
                    MyList = ColumnDef.ToEntityList<T>(table);
                else if (ObjSource is DataView dv)
                {
                    DataTable dt = dv?.Table;
                    if (dt != null)
                        MyList = ColumnDef.ToEntityList<T>(dt);
                }
            }
            catch (Exception ex)
            {

            }
            return MyList;
        }

        /// <summary>
        /// DataView转List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataView"></param>
        /// <returns></returns>
        public static List<T> ToMyList<T>(this DataView dataView) where T : new()
        {
            DataTable dt = dataView.ToMyDataTable();
            List<T> lst = dt.ToMyList<T>();
            return lst;
        }

        /// <summary>
        /// DataTable转List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static List<T> ToMyList<T>(this DataTable dt) where T : new()
        {
            List<T> list = new List<T>();
            if (dt != null)
            {
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                        list.Add(ToEntity<T>(dr));
                }
            }
            return list;
        }
    }

    /// <summary>
    /// 泛型、集合数据转换
    /// </summary>
    public static partial class sCommon
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static List<IDictionary<string, object>> ToData(IDataReader reader)
        {
            var metaDataList = new List<IDictionary<string, object>>();

            while (reader.Read())
            {
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    dynamic fieldMetaData = new ExpandoObject();
                    var columnName = reader.GetName(i);
                    var value = reader[i];
                    var dotNetType = reader.GetFieldType(i);
                    var sqlType = reader.GetDataTypeName(i);
                    //var specificType = reader.GetProviderSpecificFieldType(i);
                    fieldMetaData.columnName = columnName;
                    fieldMetaData.value = value;
                    fieldMetaData.dotNetType = dotNetType;
                    fieldMetaData.sqlType = sqlType;
                    //fieldMetaData.specificType = specificType;
                    metaDataList.Add(fieldMetaData);
                }
            }
            return metaDataList;
        }

        /// <summary>
        /// 克隆一个实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Source"></param>
        /// <returns></returns>
        public static T ToMyEntity<T>(this object Obj)
        {
            try
            {
                if (Obj == null) return default(T);
                return (T)Obj;
            }
            catch (Exception ex)
            {
                return default(T);
            }
        }

        /// <summary>
        /// DataRow - T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="row"></param>
        /// <returns></returns>
        public static T ToEntity<T>(this DataRow row) where T : new()
        {
            if (row == null)
                return default(T);
            T data = new T();
            PropertyInfo[] propertys = data.GetType().GetProperties();
            DataColumnCollection Columns = row.Table.Columns;
            foreach (PropertyInfo pi in propertys)
            {
                if (!pi.CanWrite)
                    continue;
                string columnName = pi.Name;
                if (Columns.Contains(columnName))
                {
                    object value = row[columnName];
                    if (value is DBNull)
                        continue;
                    pi.SetPropValue<T>(ref data, value);
                }
            }
            return data;
        }

        /// <summary>
        /// DataReader - T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dr"></param>
        /// <returns></returns>
        private static T ToEntity<T>(this IDataReader dr) where T : new()
        {
            if (dr == null || dr.Read())
                return default(T);
            T t = new T();
            PropertyInfo[] propertys = t.GetType().GetProperties();
            List<string> fieldNameList = new List<string>();
            for (int i = 0; i < dr.FieldCount; i++)
            {
                fieldNameList.Add(dr.GetName(i));
            }
            foreach (PropertyInfo pi in propertys)
            {
                if (!pi.CanWrite)
                    continue;
                string strPropName = pi.Name;
                if (fieldNameList.Contains(strPropName))
                {
                    object value = dr[strPropName];
                    if (value is DBNull)
                        continue;
                    pi.SetPropValue<T>(ref t, value);
                }
            }
            return t;
        }

        /// <summary>
        /// DataTable - List T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static List<T> ToEntityList<T>(this DataTable dt) where T : new()
        {
            List<T> list = new List<T>();
            if (dt != null)
            {
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                        list.Add(ToEntity<T>(dr));
                }
            }
            return list;
        }

        /// <summary>
        /// DataReader - List T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dr"></param>
        /// <returns></returns>
        public static List<T> ToEntityList<T>(this IDataReader dr) where T : new()
        {
            List<T> list = new List<T>();
            if (dr != null)
            {
                while (dr.Read())
                {
                    list.Add(ToEntity<T>(dr));
                }
                dr.Close();
            }
            return list;
        }

        /// <summary>
        /// DataTable - ObservableCollection T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static ObservableCollection<T> ToEntityObserverCollection<T>(this DataTable dt) where T : new()
        {
            ObservableCollection<T> list = new ObservableCollection<T>();
            if (dt != null)
            {
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                        list.Add(ToEntity<T>(dr));
                }
            }
            return list;
        }

        /// <summary>
        /// DataReader - ObservableCollection T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dr"></param>
        /// <returns></returns>
        public static ObservableCollection<T> ToEntityObserverCollection<T>(this IDataReader dr) where T : new()
        {
            ObservableCollection<T> ObServerCol = new ObservableCollection<T>();
            if (dr != null)
            {
                while (dr.Read())
                {
                    ObServerCol.Add(ToEntity<T>(dr));
                }
                dr.Close();
            }
            return ObServerCol;
        }

        /// <summary>
        /// List T  - ObservableCollection T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static ObservableCollection<T> ToMyObservableCollection<T>(this List<T> source)
        {
            if (source == null)
                return new ObservableCollection<T>();
            ObservableCollection<T> to = new ObservableCollection<T>(source);
            //或者source.ForEach(p => to.Add(p));
            return to;
        }
    }

    /// <summary>
    /// 实体反射、集合方法 
    /// </summary>
    public static partial class sCommon
    {
        /// <summary>
        /// IDataReader - DataTable
        /// </summary>
        /// <param name="dr"></param>
        /// <returns></returns>
        public static DataTable GetDataTable(this IDataReader dr)
        {
            return dr.GetSchemaTable();
        }

        /// <summary>
        /// List T - DataTable
        /// </summary>
        /// <param name="modelList">实体类列表</param>
        /// <returns></returns>
        public static DataTable GetDataTable<T>(this List<T> modelList)
        {
            if (modelList == null || modelList.Count == 0)
            {
                return null;
            }
            DataTable dt = GetDataTable(modelList[0]);//创建表结构
            foreach (T model in modelList)
            {
                DataRow dataRow = dt.NewRow();
                foreach (PropertyInfo propertyInfo in typeof(T).GetProperties())
                {
                    dataRow[propertyInfo.Name] = propertyInfo.GetValue(model, null);
                }
                dt.Rows.Add(dataRow);
            }
            return dt;
        }

        /// <summary>
        /// 实体转换成数据表
        /// 调用示例：DataTable dt= GetDataTable(Entitylist.ToList());
        /// </summary>
        /// <param name="modelList">实体类列表</param>
        /// <returns></returns>
        public static DataTable GetDataTable<T>(this ObservableCollection<T> modelList)
        {
            if (modelList == null || modelList.Count == 0)
            {
                return null;
            }
            DataTable dt = GetDataTable(modelList[0]);
            foreach (T model in modelList)
            {
                DataRow dataRow = dt.NewRow();
                foreach (PropertyInfo propertyInfo in typeof(T).GetProperties())
                {
                    dataRow[propertyInfo.Name] = propertyInfo.GetValue(model, null);
                }
                dt.Rows.Add(dataRow);
            }
            return dt;
        }

        /// <summary>
        /// T - DataTabvle
        /// </summary>
        /// <param name="model">实体类</param>
        /// <returns></returns>
        private static DataTable GetDataTable<T>(this T model)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);
            foreach (PropertyInfo propertyInfo in typeof(T).GetProperties())
            {
                if (propertyInfo.Name != "CTimestamp")  //字段为oracle中的Timesstarmp类型
                {
                    dataTable.Columns.Add(new DataColumn(propertyInfo.Name, propertyInfo.PropertyType));
                }
                else
                {
                    dataTable.Columns.Add(new DataColumn(propertyInfo.Name, typeof(DateTime)));
                }
            }
            return dataTable;
        }

        /// <summary>
        /// 克隆一个实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Source"></param>
        /// <returns></returns>
        public static T ModelClone<T>(this T Source) where T : new()
        {
            T Obj = new T();
            Source.MapperToModel<T, T>(ref Obj);
            return Obj;
        }

        /// <summary>
        /// 克隆添加
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="lst"></param>
        /// <param name="Object"></param>
        public static void CloneAdd<T>(this List<T> lst, T Object) where T : new()
        {
            T Obj = ModelClone<T>(Object);
            lst.Add(Obj);
        }

        /// <summary>
        /// 反射实现两个类的对象之间相同属性的值的复制
        /// </summary>
        /// <typeparam name="S">数据源实体</typeparam>
        /// <typeparam name="D">返回的实体</typeparam>
        /// <param name="s">数据源实体</param>
        /// <param name="d">返回的实体</param>
        /// <returns></returns>
        public static CallResult MapperToModel<S, D>(this S s, ref D d) where D : new()
        {
            CallResult _result = new CallResult() { Success = false, Result = "" };
            try
            {
                var Types = s.GetType();
                var Typed = typeof(D);
                //if (d == null) d = new D();
                foreach (PropertyInfo sp in Types.GetProperties())
                {
                    //若该属性是索引 跳过本次循环
                    if (sp.GetIndexParameters().Length > 0)
                        continue;
                    foreach (PropertyInfo dp in Typed.GetProperties())
                    {
                        if (dp.Name == sp.Name && dp.PropertyType == sp.PropertyType
                            && dp.CanWrite && sp.CanRead)
                        {
                            dp.SetValue(d, sp.GetValue(s, null), null);
                        }
                    }
                }
                _result.Success = true;
            }
            catch (Exception ex)
            {
                _result.Success = false;
                _result.Result = ex.Message;
            }
            return _result;
        }

        /// <summary>
        /// 将列表S按属性名称复制并创建列表D
        /// </summary>
        /// <typeparam name="S"></typeparam>
        /// <typeparam name="D"></typeparam>
        /// <param name="Source"></param>
        /// <returns></returns>
        public static bool MapperToModel<S, D>(this List<S> Source, out List<D> Dest) where D : new()
        {
            List<D> LstDest = new List<D>();
            try
            {
                foreach (S item in Source)
                {
                    D d = new D();
                    item.MapperToModel(ref d);
                    LstDest.Add(d);
                }
                Dest = LstDest;
                return true;
            }
            catch (Exception ex)
            {
                Dest = LstDest;
                return false;
            }
        }

        /// <summary>
        /// 反射源实体 - 复制给目标实体列表
        /// </summary>
        /// <typeparam name="S">数据源实体</typeparam>
        /// <typeparam name="D">返回的实体</typeparam>
        /// <param name="Sourcre">数据源实体</param>
        /// <param name="LstDest">返回的实体</param>
        /// <param name="ContainIfEmpty">是否空值也覆盖 默认空值不传递</param>
        /// <returns></returns>
        public static CallResult MapperToModel<S, D>(this S Sourcre, ref List<D> LstDest, bool ContainIfEmpty = false) where D : new()
        {
            CallResult _result = new CallResult() { Success = false, Result = "" };
            try
            {
                var Types = Sourcre.GetType();
                var Typed = typeof(D);
                foreach (D dest in LstDest)
                {
                    foreach (PropertyInfo sp in Types.GetProperties())
                    {
                        //若该属性是索引 跳过本次循环
                        if (sp.GetIndexParameters().Length > 0)
                            continue;
                        foreach (PropertyInfo dp in Typed.GetProperties())
                        {
                            if (dp.Name == sp.Name && dp.PropertyType == sp.PropertyType
                                && dp.CanWrite && sp.CanRead)
                            {
                                object objSrc = sp.GetValue(Sourcre, null);
                                if (!ContainIfEmpty)
                                {
                                    if (sp.PropertyType == typeof(string))
                                    {
                                        string strSource = objSrc.ToMyString();
                                        if (string.IsNullOrEmpty(strSource))
                                            continue;
                                    }
                                    else if (objSrc == null)
                                        continue;
                                }
                                dp.SetValue(dest, objSrc, null);
                            }
                        }
                    }
                }
                _result.Success = true;
            }
            catch (Exception ex)
            {
                _result.Success = false;
                _result.Result = ex.Message;
            }
            return _result;
        }

        /// <summary>
        /// 属性等价赋值，并返回新的目标实体
        /// </summary>
        /// <typeparam name="S">数据源实体</typeparam>
        /// <typeparam name="D">返回的实体</typeparam>
        /// <param name="Source">数据源实体</param>
        /// <returns></returns>
        public static D MapperToModel<S, D>(this S Source) where D : new()
        {
            try
            {
                D Dest = new D();
                var Types = Source.GetType();
                var Typed = typeof(D);
                foreach (PropertyInfo sp in Types.GetProperties())
                {
                    //若该属性是索引 跳过本次循环
                    if (sp.GetIndexParameters().Length > 0)
                        continue;
                    foreach (PropertyInfo dp in Typed.GetProperties())
                    {
                        if (dp.Name == sp.Name && dp.PropertyType == sp.PropertyType &&
                            dp.CanWrite && sp.CanRead)
                        {
                            dp.SetValue(Dest, sp.GetValue(Source, null), null);
                        }
                    }
                }
                return Dest;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 反射实现两个类的对象之间相同属性的值的复制
        /// 两个实体，实体1的属性，在实体2的特性上
        /// </summary>
        /// <typeparam name="S">数据源实体</typeparam>
        /// <typeparam name="D">返回的实体</typeparam>
        /// <param name="s">数据源实体</param>
        /// <param name="d">返回的实体</param>
        /// <returns></returns>
        public static CallResult MapperToModelByAttr<S, D, TAttr>(this S s, ref D d) where TAttr : new()
        {
            CallResult _result = new CallResult() { Success = false, Result = "" };
            try
            {
                var Types = s.GetType();
                var Typed = typeof(D);
                foreach (PropertyInfo dp in Typed.GetProperties())
                {
                    object modelAttr = dp.Name.GetPropAttribute<D, TAttr>();
                    if (modelAttr == null)
                        continue;
                    string DestAttrName = (modelAttr as AttributeBase).Name;
                    foreach (PropertyInfo sp in Types.GetProperties())
                    {
                        //若该属性是索引 跳过本次循环
                        if (sp.GetIndexParameters().Length > 0)
                            continue;
                        if ((DestAttrName == sp.Name) && dp.PropertyType == sp.PropertyType
                            && dp.CanWrite && sp.CanRead)
                        {
                            dp.SetValue(d, sp.GetValue(s, null), null);
                        }
                    }
                }
                _result.Success = true;
            }
            catch (Exception ex)
            {
                _result.Success = false;
                _result.Result = ex.Message;
            }
            return _result;
        }

        /// <summary>
        /// 两个实体，具有相同特性的属性传递
        /// 属性不同，特性相同即可配对
        /// </summary>
        /// <typeparam name="S">数据源实体</typeparam>
        /// <typeparam name="D">返回的实体</typeparam>
        /// <param name="s">数据源实体</param>
        /// <param name="d">返回的实体</param>
        /// <returns></returns>
        public static CallResult MapperToModelByMatchedAttr<S, D, TAttr>(this S s, ref D d) where TAttr : new()
        {
            CallResult _result = new CallResult() { Success = false, Result = "" };
            try
            {
                var Types = s.GetType();
                var Typed = typeof(D);
                foreach (PropertyInfo dp in Typed.GetProperties())
                {
                    object modelAttr1 = dp.Name.GetPropAttribute<D, TAttr>();
                    if (modelAttr1 == null)
                        continue;
                    string DestAttrName1 = (modelAttr1 as AttributeBase).Name;
                    foreach (PropertyInfo sp in Types.GetProperties())
                    {
                        object modelAttr2 = sp.Name.GetPropAttribute<S, TAttr>();
                        if (modelAttr2 == null || sp.GetIndexParameters().Length > 0)
                            continue;
                        string DestAttrName2 = (modelAttr2 as AttributeBase).Name;
                        if (DestAttrName1 == DestAttrName2 && dp.PropertyType == sp.PropertyType
                            && dp.CanWrite && sp.CanRead)
                        {
                            dp.SetValue(d, sp.GetValue(s, null), null);
                        }
                    }
                }
                _result.Success = true;
            }
            catch (Exception ex)
            {
                _result.Success = false;
                _result.Result = ex.Message;
            }
            return _result;
        }

        /// <summary>
        /// 对象属性初始化
        /// </summary>
        /// <typeparam name="D"></typeparam>
        /// <param name="d"></param>
        /// <returns></returns>
        public static CallResult MapperEmptyToModel<D>(ref D d) where D : new()
        {
            CallResult _result = new CallResult() { Success = false, Result = "" };
            try
            {
                var Typed = typeof(D);
                foreach (PropertyInfo pi in Typed.GetProperties())
                {
                    if (!pi.CanWrite)
                        continue;
                    object objPropValue = null;
                    if (pi.PropertyType.FullName == typeof(Int32).FullName)
                        objPropValue = SystemDefault.InValidInt;
                    else if (pi.PropertyType.FullName == typeof(Boolean).FullName)
                        objPropValue = false;
                    else if (pi.PropertyType.FullName == typeof(String).FullName)
                        objPropValue = SystemDefault.StringEmpty;
                    else if (pi.PropertyType.FullName == typeof(double).FullName)
                        objPropValue = SystemDefault.InValidDouble;
                    pi.SetValue(d, objPropValue, null);
                }
                _result.Success = true;
            }
            catch (Exception ex)
            {
                _result.Success = false;
                _result.Result = ex.Message;
            }
            return _result;
        }

        /// <summary>
        /// 找出A列表中B列表以外的数据
        /// </summary>
        /// <typeparam name="A"></typeparam>
        /// <typeparam name="B"></typeparam>
        /// <typeparam name="TKeyA"></typeparam>
        /// <typeparam name="TKeyB"></typeparam>
        /// <param name="LstA"></param>
        /// <param name="LstB"></param>
        /// <param name="selectorA"></param>
        /// <param name="selectorB"></param>
        /// <returns></returns>
        public static List<A> WhereANotInB<A, B, TKeyA, TKeyB>(this List<A> LstA, List<B> LstB, Func<B, TKeyB> selectorB, Func<A, TKeyB> selectorA)
        {
            if (LstA == null) return null;
            if (LstB == null) return LstA;
            List<TKeyB> LstSrc = LstB.Select(selectorB).Distinct().ToList();
            List<A> LstSrc2 = LstA.Where(x => !LstSrc.Contains(selectorA(x))).ToList();
            return LstSrc2;
        }
    }
}
