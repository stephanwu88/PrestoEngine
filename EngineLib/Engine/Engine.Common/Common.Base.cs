using System;
using System.Windows;
using System.ComponentModel;
using System.Reflection;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using Engine.Mod;
using System.Data;

namespace Engine.Common
{
    /// <summary>
    /// 系统消息类型
    /// </summary>
    public enum MsgType
    {
        [Description("错误")]
        Error,
        [Description("警告")]
        Warning,
        [Description("注意")]
        Exclamation,
        [Description("信息")]
        Infomation,
        [Description("问题")]
        Question,
        [Description("调试")]
        Debug,
        [Description("其他")]
        Other,
    }

    /// <summary>
    /// 结束字符串的匹配模式
    /// </summary>
    public enum EndStringSearchMode
    {
        /// <summary>
        /// 从头部开始查找
        /// </summary>
        FromHead,
        /// <summary>
        /// 从尾部开始查找
        /// </summary>
        FromTail,
        /// <summary>
        /// 从头部开始，未找到则至尾
        /// </summary>
        FromHeadAndToEndWhenUnMatch,
        /// <summary>
        /// 从尾部开始，未找到则至尾
        /// </summary>
        FromTailAndToEndWhenUnMatch
    }

    /// <summary>
    /// 字符串匹配模式
    /// 8421 
    /// </summary>
    [Flags]
    public enum StringMatchMode
    {
        /// <summary>
        /// 全字匹配
        /// </summary>
        WholeWord = 0,
        /// <summary>
        /// 忽略大小写
        /// </summary>
        IgnoreCase = 1,
        /// <summary>
        /// 忽略全角半角,统一成半角
        /// </summary>
        IgnoreDBC = 2
    }

    /// <summary>
    /// 搜索方向
    /// </summary>
    public enum MatchDirect
    {
        FromHead,   //从头部开始查找
        FromTail    //从尾部开始查找
    }

    /// <summary>
    /// 常用系统功能
    /// </summary>
    public static partial class sCommon
    {
        /// <summary>
        /// 验证程序使用者
        /// </summary>
        /// <returns></returns>
        public static bool ValidateAppCaller()
        {
            //获取执行App程序的FullName -- HeaoIPS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=52b73f80d00d732b
            string ExeFullName = Assembly.GetEntryAssembly().GetName().FullName;
            string ExePublicKey = Assembly.GetEntryAssembly().GetName().GetPublicKey().ByteArrayToHexString("{0:x2}");
            string ExePublicKeyToken = Assembly.GetEntryAssembly().GetName().GetPublicKeyToken().ByteArrayToHexString("{0:x2}");

            //获取Engine.dll文件  -- PublicKey
            string fileName = Assembly.GetCallingAssembly().Location;
            Assembly AssemblyInfo = Assembly.LoadFrom(fileName);
            string EngineFileFullName = AssemblyInfo.GetName().FullName;
            string EngineFilePublicKey = AssemblyInfo.GetName().GetPublicKey().ByteArrayToHexString("{0:x2}");
            string EngineFilePublicKeyToken = AssemblyInfo.GetName().GetPublicKeyToken().ByteArrayToHexString("{0:x2}");
            return ExePublicKeyToken == EngineFilePublicKeyToken;
        }
        /// <summary>
        /// 自定义弹出
        /// </summary>
        /// <param name="strContent"></param>
        /// <param name="msgType"></param>
        public static MessageBoxResult MyMsgBox(this string strContent, MsgType msgType = MsgType.Infomation)
        {
            //ValidateAppCaller();
            MessageBoxImage image = new MessageBoxImage();
            MessageBoxButton btn = new MessageBoxButton();
            MessageBoxResult ret = MessageBoxResult.None;
            switch (msgType)
            {
                case MsgType.Error:
                    image = MessageBoxImage.Error;
                    btn = MessageBoxButton.OK;
                    break;
                case MsgType.Warning:
                    image = MessageBoxImage.Warning;
                    btn = MessageBoxButton.OK;
                    break;
                case MsgType.Exclamation:
                    image = MessageBoxImage.Exclamation;
                    break;
                case MsgType.Infomation:
                    image = MessageBoxImage.Information;
                    btn = MessageBoxButton.OK;
                    break;
                case MsgType.Question:
                    image = MessageBoxImage.Question;
                    btn = MessageBoxButton.YesNo;
                    break;
            }
            if (strContent.Length > 0)
                ret = MessageBox.Show(strContent, CompanyName, btn, image);
            return ret;
        }
    }

    /// <summary>
    /// 字符串操作 & 正则表达式 & 泛型集合
    /// </summary>
    public static partial class sCommon
    {
        /// <summary>
        /// 字符串是空的
        /// </summary>
        /// <param name="Source"></param>
        /// <returns></returns>
        public static bool IsEmpty(this string Source)
        {
            return string.IsNullOrEmpty(Source) || string.IsNullOrWhiteSpace(Source);
        }
        /// <summary>
        /// 字符串不是空的
        /// </summary>
        /// <param name="Source"></param>
        /// <returns></returns>
        public static bool IsNotEmpty(this string Source)
        {
            return !string.IsNullOrEmpty(Source) && !string.IsNullOrWhiteSpace(Source);
        }
        /// <summary>
        /// 字符串取子串  
        /// 头部： 不指定则默认第一个字符串开始，不存在则返回空
        /// 尾部： 不指定则至尾，指定不存在则根据模式返回空或者至尾
        /// </summary>
        /// <param name="source">源</param>
        /// <param name="StartString">开始子串</param>
        /// <param name="EndString">结束子串</param>
        /// <param name="EndStringDirect">结束字符串的搜索模式</param>
        /// <returns></returns>
        public static string MidString(this string source, string StartString, string EndString,
            EndStringSearchMode EndStringMode = EndStringSearchMode.FromHeadAndToEndWhenUnMatch, bool WithTrim = true)
        {
            if (string.IsNullOrEmpty(source))
                return string.Empty;
            string strResult = string.Empty;
            int iStartID = string.IsNullOrEmpty(StartString) ? 0 : source.IndexOf(StartString);
            int iStartWidth = StartString.Length;
            int iSubFrom = iStartID + iStartWidth;
            int iEndID = -1;
            if (string.IsNullOrEmpty(EndString) || (!source.Contains(EndString) && EndStringMode.ToString().Contains("ToEnd")))
                iEndID = source.Length;
            else if (EndStringMode.ToString().Contains("Tail"))
            {
                //iEndID = source.LastIndexOf(EndString, iSubFrom);
                iEndID = source.LastIndexOf(EndString);
                if (iEndID < iSubFrom) iEndID = -1;
            }
            else if (EndStringMode.ToString().Contains("Head"))
                iEndID = source.IndexOf(EndString, iSubFrom);
            if (iEndID == -1 && EndStringMode.ToString().Contains("ToEnd"))
                iEndID = source.Length;
            int iSubLen = iEndID - iSubFrom;
            if (iStartID >= 0 && iEndID >= 0 && iSubLen >= 0)
                strResult = source.MidString(iSubFrom, iSubLen);
            if (WithTrim) strResult = strResult.Trim();
            return strResult;
        }
        /// <summary>
        /// 字符串取子串
        /// </summary>
        /// <param name="source">源</param>
        /// <param name="startIndex">开始位置 从0开始  -1: 倒数第一位开始</param>
        /// <param name="len">长度 -1代表不指定，也就是取至尾部</param>
        /// <returns></returns>
        public static string MidString(this string source, int startIndex, int len = -1)
        {
            try
            {
                if (string.IsNullOrEmpty(source))
                    return string.Empty;
                int TotalLen = source.Length;
                //处理倒数位
                if (startIndex < 0)
                {
                    int temp = 0 - startIndex;
                    if (temp > TotalLen)
                    {
                        return string.Empty;
                    }
                    startIndex = TotalLen - temp;
                }
                if (TotalLen == 0 || startIndex < 0 || startIndex >= TotalLen)
                    return string.Empty;
                if ((len > 0 && (startIndex + len > TotalLen)) || len < 0)
                    return source.Substring(startIndex);
                return source.Substring(startIndex, len);
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
        /// <summary>
        /// 字符串取子串
        /// </summary>
        /// <param name="source">源</param>
        /// <param name="iFrom">开始索引，从0</param>
        /// <param name="StartString">起始子串</param>
        /// <param name="EndString">结束子串</param>
        /// <param name="EndStringMode">结束字符串的搜索模式</param>
        /// <returns></returns>
        public static string MidString(this string source, int iFrom, string StartString, string EndString,
            EndStringSearchMode EndStringMode = EndStringSearchMode.FromHeadAndToEndWhenUnMatch)
        {
            source = source.MidString(iFrom);
            return source.MidString(StartString, EndString, EndStringMode);
        }

        /// <summary>
        /// 获取子串的数量
        /// </summary>
        /// <param name="source"></param>
        /// <param name="subStr"></param>
        /// <returns></returns>
        public static int CountOfSub(this string source, string subStr)
        {
            if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(subStr))
                return -1;
            int iCount = Regex.Matches(source, subStr).Count;
            return iCount;
        }

        /// <summary>
        /// 定位字符串位置
        /// </summary>
        /// <param name="source"></param>
        /// <param name="subStr">目标子串</param>
        /// <param name="SelectID">第几个  从1开始</param>
        /// <param name="dir">前到后数  or  后往前数</param>
        /// <returns>返回那一个目标在子串中的序号  从1开始</returns>
        public static int OrderOfSub(this string source, string subStr, int SelectID = 1, MatchDirect dir = MatchDirect.FromHead)
        {
            if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(subStr))
                return -1;
            int OrderID = -1;
            try
            {
                MatchCollection matches = Regex.Matches(source, subStr);
                if (SelectID > matches.Count || SelectID < 1) return -1;
                if (dir == MatchDirect.FromHead)
                    OrderID = matches[SelectID - 1].Index + 1;
                else if (dir == MatchDirect.FromTail)
                    OrderID = matches[matches.Count - SelectID].Index + 1;
            }
            catch (Exception)
            {

            }
            return OrderID;
        }

        /// <summary>
        /// 定位字符串位置 -- 逆序
        /// </summary>
        /// <param name="source"></param>
        /// <param name="subStr">目标子串</param>
        /// <param name="SelectID">第几个  从1开始</param>
        /// <param name="dir">前到后数  or  后往前数</param>
        /// <returns>返回那一个目标在子串中的倒数序号  从1开始</returns>
        public static int InvertOrderOfSub(this string source, string subStr, int SelectID = 1, MatchDirect dir = MatchDirect.FromHead)
        {
            if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(subStr))
                return -1;
            int OrderID = -1;
            try
            {
                MatchCollection matches = Regex.Matches(source, subStr);
                if (SelectID > matches.Count || SelectID < 1) return -1;
                if (dir == MatchDirect.FromHead)
                    OrderID = source.Length - matches[SelectID - 1].Index;
                else if (dir == MatchDirect.FromTail)
                    OrderID = source.Length - matches[matches.Count - SelectID].Index;
            }
            catch (Exception)
            {

            }
            return OrderID;
        }

        /// <summary>
        /// 字符串头部开始的子串
        /// </summary>
        /// <param name="source"></param>
        /// <param name="iStartLen">子串长度</param>
        /// <returns></returns>
        public static string StartSubString(this string source, int iStartLen)
        {
            if (source.Length >= iStartLen)
                return source.Substring(0, iStartLen);
            return string.Empty;
        }
        /// <summary>
        /// 字符串尾部结束的子串
        /// </summary>
        /// <param name="source"></param>
        /// <param name="iEndLen">子串长度</param>
        /// <returns></returns>
        public static string EndSubString(this string source, int iEndLen)
        {
            if (source.Length >= iEndLen)
                return source.Substring(source.Length - iEndLen, iEndLen);
            return string.Empty;
        }
        /// <summary>
        /// 字符串去除右侧匹配字符
        /// </summary>
        /// <param name="strSrc">源字符串</param>
        /// <param name="strTrim">要去除的字符串</param>
        /// <returns></returns>
        public static string TrimEnd(this string strSrc, string strTrim)
        {
            if (strSrc.Contains(strTrim) && !string.IsNullOrEmpty(strTrim))
            {
                string strTail = strSrc.Substring(strSrc.Length - strTrim.Length);
                if (strTail == strTrim)
                    return strSrc.Substring(0, strSrc.Length - strTrim.Length);
            }
            return strSrc;
        }
        /// <summary>
        /// 字符串中去除指定子串
        /// </summary>
        /// <param name="source">源</param>
        /// <param name="RemoveSubString">去除子串组</param>
        /// <returns></returns>
        public static string RemoveString(this string source, string[] RemoveSubString)
        {
            string ret = source;
            if (RemoveSubString.Length > 0)
            {
                foreach (string rem in RemoveSubString)
                {
                    ret = ret.Replace(rem, "");
                }
            }
            return ret;
        }
        /// <summary>
        /// 设置变量值
        /// 若设置变量有值则忽略
        /// </summary>
        /// <param name="source">变量员</param>
        /// <param name="value">设置值</param>
        /// <returns></returns>
        public static void TrySetValueTo(string source, object value)
        {
            if (string.IsNullOrEmpty(source) && value != null)
                source = value.ToMyString();
        }
        /// <summary>
        /// 字符串分割
        /// </summary>
        /// <param name="source">源字符串</param>
        /// <param name="SplitWith">分割符号</param>
        /// <param name="LstIgnore">需要忽略的字符串列表</param>
        /// <param name="LstRemoveSub">字符串中需要移除的子串列表</param>
        /// <param name="RemoveEmpty">是否要移除空</param>
        /// <param name="WithTrim">是否头尾去空</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static List<string> MySplit(this string source, string SplitWith,
            List<string> LstIgnore = null, List<string> LstRemoveSub = null,
            bool RemoveEmpty = true, bool WithTrim = true)
        {
            List<string> LstStr = new List<string>();
            if (string.IsNullOrWhiteSpace(source) || string.IsNullOrEmpty(source))
                return LstStr;
            List<string> LstSource = source.ToMyString().Split(new string[] { SplitWith }, StringSplitOptions.None).ToList();
            foreach (string item in LstSource)
            {
                if (RemoveEmpty && string.IsNullOrEmpty(item))
                    continue;
                if (LstIgnore.MyContains(item))
                    continue;
                string strAppand = item;
                if (LstRemoveSub != null)
                {
                    foreach (string sub in LstRemoveSub)
                        strAppand = strAppand.Replace(sub, "");
                }
                if (WithTrim)
                    strAppand = strAppand.Trim();
                LstStr.Add(strAppand);
            }
            return LstStr;
        }

        /// <summary>
        /// 字符串分割，
        /// 双引号包含为整字段
        /// </summary>
        /// <param name="Source"></param>
        /// <param name="SplitChar"></param>
        /// <returns></returns>
        public static List<string> MySplit(this string Source, char SplitChar, bool RemoveEmpty = true, bool WithTrim = true,
            List<string> LstIgnore = null, List<string> LstRemoveSub = null)
        {
            string pattern = string.Format(@"""(?:[^""]|"""")*""|[^{0}]+|(?<={0})(?={0}|$)", Convert.ToString(SplitChar));
            List<string> LstResult = new List<string>();
            List<string> matchedList = Source.RegexMatchedList(pattern, false);
            foreach (string str in matchedList)
            {
                if (RemoveEmpty && string.IsNullOrEmpty(str))
                    continue;
                if (LstIgnore.MyContains(str))
                    continue;
                string strAppand = str;
                strAppand = strAppand.Trim(new char[] { '\"' });
                if (LstRemoveSub != null)
                {
                    foreach (string sub in LstRemoveSub)
                        strAppand = strAppand.Replace(sub, "");
                }
                if (WithTrim)
                    strAppand = strAppand.Trim();
                LstResult.Add(strAppand);
            }
            return LstResult;
        }

        /// <summary>
        /// 字符串长度
        /// </summary>
        /// <param name="Source"></param>
        /// <returns></returns>
        public static int MyLength(this string Source)
        {
            if (string.IsNullOrEmpty(Source))
                return 0;
            return Source.Length;
        }

        /// <summary>
        /// 预处理格式字符串
        /// </summary>
        /// <param name="Source"></param>
        /// <param name="MatchMode">StringMatchMode.IgnoreCase|StringMatchMode.IgnoreDBC</param>
        /// <returns></returns>
        public static string MyFormatString(this string Source, StringMatchMode MatchMode)
        {
            try
            {
                string strSource = Source;
                if ((MatchMode & StringMatchMode.IgnoreCase) != 0)
                {
                    strSource = strSource.ToLower();
                }
                if ((MatchMode & StringMatchMode.IgnoreDBC) != 0)
                {
                    strSource = strSource.ToDBC();
                }
                return strSource;
            }
            catch (Exception)
            {
                return Source;
            }
        }

        /// <summary>
        /// 字符串起始检查
        /// </summary>
        /// <param name="Source"></param>
        /// <param name="StartStr"></param>
        /// <param name="SplitStr"></param>
        /// <param name="MatchMode"></param>
        /// <returns></returns>
        public static bool MyStartsWith(this string Source, string StartStr, string SplitStr,
            StringMatchMode MatchMode = StringMatchMode.WholeWord)
        {
            if (MatchMode > 0)
            {
                Source = Source.MyFormatString(MatchMode);
                StartStr = StartStr.MyFormatString(MatchMode);
            }
            if (string.IsNullOrEmpty(Source) || string.IsNullOrEmpty(StartStr))
                return false;
            if (string.IsNullOrEmpty(SplitStr))
                return Source.StartsWith(StartStr);
            List<string> LstStart = StartStr.MySplit(SplitStr);
            foreach (string item in LstStart)
            {
                bool IsContain = Source.StartsWith(item);
                if (IsContain) return true;
            }
            return false;
        }
        /// <summary>
        /// 字符串结束检查
        /// </summary>
        /// <param name="Source"></param>
        /// <param name="EndStr"></param>
        /// <param name="SplitStr"></param>
        /// <param name="MatchMode"></param>
        /// <returns></returns>
        public static bool MyEndsWith(this string Source, string EndStr, string SplitStr,
           StringMatchMode MatchMode = StringMatchMode.WholeWord)
        {
            if (MatchMode > 0)
            {
                Source = Source.MyFormatString(MatchMode);
                EndStr = EndStr.MyFormatString(MatchMode);
            }
            if (string.IsNullOrEmpty(Source) || string.IsNullOrEmpty(EndStr))
                return false;
            if (string.IsNullOrEmpty(SplitStr))
                return Source.EndsWith(EndStr);
            List<string> LstEnd = EndStr.MySplit(SplitStr);
            foreach (string item in LstEnd)
            {
                bool IsContain = Source.EndsWith(item);
                if (IsContain) return true;
            }
            return false;
        }

        /// <summary>
        /// 获取集合内的元素数量
        /// </summary>
        /// <param name="LstSource"></param>
        /// <returns></returns>
        public static int MyCount<T>(this IEnumerable<T> LstSource)
        {
            if (LstSource == null) return 0;
            return LstSource.Count();
        }
        /// <summary>
        /// 获取集合内的筛选后元素数量
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="LstSource"></param>
        /// <param name="Selector"></param>
        /// <returns></returns>
        public static int MyCount<T>(this IEnumerable<T> LstSource, Func<T, bool> Selector)
        {
            if (LstSource == null)
                return 0;
            return LstSource.ToList().Where(Selector).Count();
        }
        /// <summary>
        /// 数组长度
        /// </summary>
        /// <param name="LstSource"></param>
        /// <returns></returns>
        public static int MyCount<T>(this T[] ArrSource)
        {
            if (ArrSource == null) return 0;
            return ArrSource.Length;
        }

        /// <summary>
        /// 字符串包含检查
        /// </summary>
        /// <param name="Source"></param>
        /// <param name="ContainStr"></param>
        /// <param name="MatchMode"></param>
        /// <returns></returns>
        public static bool MyContains(this string Source, string ContainStr,
            StringMatchMode MatchMode = StringMatchMode.WholeWord)
        {
            if (MatchMode > 0)
            {
                Source = Source.MyFormatString(MatchMode);
                ContainStr = ContainStr.MyFormatString(MatchMode);
            }
            if (string.IsNullOrEmpty(Source) || string.IsNullOrEmpty(ContainStr))
                return false;
            return Source.Contains(ContainStr);
        }
        /// <summary>
        /// 字符串包含检查
        /// </summary>
        /// <param name="Source"></param>
        /// <param name="ContainStr"></param>
        /// <param name="SplitStr"></param>
        /// <param name="MatchMode"></param>
        /// <returns></returns>
        public static bool MyContains(this string Source, string ContainStr, string SplitStr,
            StringMatchMode MatchMode = StringMatchMode.WholeWord)
        {
            if (string.IsNullOrEmpty(Source)) return false;
            if (MatchMode > 0)
            {
                Source = Source.MyFormatString(MatchMode);
                ContainStr = ContainStr.MyFormatString(MatchMode);
            }
            if (string.IsNullOrEmpty(SplitStr))
                Source.MyContains(ContainStr, MatchMode);
            List<string> LstContain = ContainStr.MySplit(SplitStr);
            foreach (string item in LstContain)
            {
                bool IsContain = Source.MyContains(item, MatchMode);
                if (IsContain) return true;
            }
            return false;
        }
        /// <summary>
        /// 列表内容包含性检查
        /// </summary>
        /// <param name="LstSource"></param>
        /// <returns></returns>
        public static bool MyContains<T>(this IEnumerable<T> LstSource, T Elem)
        {
            if (LstSource == null)
                return false;
            return LstSource.Contains(Elem);
        }
        /// <summary>
        /// 列表内容包含性检查
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="LstSource"></param>
        /// <param name="Elem"></param>
        /// <returns></returns>
        public static bool MyContains<T>(this IEnumerable<T> LstSource, Func<T, bool> Selector)
        {
            if (LstSource == null)
                return false;
            return LstSource.FirstOrDefault(Selector) != null;
        }
        /// <summary>
        /// 列表内容包含性检查
        /// </summary>
        /// <param name="LstSource"></param>
        /// <returns></returns>
        public static bool MyContains<TKey, TValue>(this Dictionary<TKey, TValue> SourceDict, TKey Key)
        {
            if (SourceDict == null)
                return false;
            if (typeof(TKey) == typeof(string))
            {
                if (string.IsNullOrEmpty(Key.ToMyString()))
                    return false;
            }
            return SourceDict.ContainsKey(Key);
        }
        /// <summary>
        /// 字符串替换
        /// </summary>
        /// <param name="source"></param>
        /// <param name="strOld"></param>
        /// <param name="strNew"></param>
        /// <returns></returns>
        public static string MyReplace(this string source, string strOld, string strNew)
        {
            if (string.IsNullOrEmpty(source)) return string.Empty;
            if (strOld.Length > 0)
                return source.Replace(strOld, strNew.ToMyString());
            else
                return source;
        }
        /// <summary>
        /// 字符串替换
        /// </summary>
        /// <param name="source"></param>
        /// <param name="iStart"></param>
        /// <param name="strNew"></param>
        /// <returns></returns>
        public static string MyReplace(this string source, int iStart, string strNew)
        {
            if (string.IsNullOrEmpty(source)) return string.Empty;
            if (string.IsNullOrEmpty(strNew)) return source;
            string strPart1 = source.MidString(0, iStart);
            string strPart2 = source.MidString(iStart, strNew.Length);
            string strPart3 = source.MidString(iStart + strNew.Length);
            return string.Format("{0}{1}{2}", strPart1, strNew, strPart3);
        }
        /// <summary>
        /// 字符串替换
        /// </summary>
        /// <param name="source"></param>
        /// <param name="iStart"></param>
        /// <param name="iLen"></param>
        /// <param name="strNew"></param>
        /// <returns></returns>
        public static string MyReplace(this string source, int iStart, int iLen, string strNew)
        {
            if (string.IsNullOrEmpty(source)) return string.Empty;
            if (string.IsNullOrEmpty(strNew)) return source;
            int iLength = iLen >= strNew.Length ? strNew.Length : iLen;
            string strPart1 = source.MidString(0, iStart);
            string strPart2 = source.MidString(iStart, iLength);
            string strPart3 = source.MidString(iStart + iLength);
            return string.Format("{0}{1}{2}", strPart1, strNew.MidString(0, iLength), strPart3);
        }
        /// <summary>
        /// 字符串中查找可变量数量
        /// {0} {1} {2}
        /// </summary>
        /// <param name="source"></param>
        /// <param name="RemoveRepeatItem">是否去除重复项</param>
        /// <returns></returns>
        public static int MatchParamCount(this string source, bool RemoveRepeatItem = true)
        {
            MatchCollection mcol = Regex.Matches(source, @"\{\d{1}\}");
            List<string> Values = new List<string>();
            foreach (Match item in mcol)
            {
                if (!Values.Contains(item.Value) && RemoveRepeatItem)
                    Values.Add(item.Value);
            }
            return Values.Count;
        }

        /// <summary>
        /// 列表按筛选返回
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Source"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static IEnumerable<T> MySelect<T>(this IEnumerable<T> Source, Func<T, bool> Selector)
        {
            if (Source == null)
                return default(List<T>);
            return Source.Where(Selector);
        }
        /// <summary>
        /// 列表按筛选返回
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Source"></param>
        /// <param name="Selector"></param>
        /// <returns></returns>
        public static IEnumerable<T> MySelect<T>(this IEnumerable<T> Source, Func<T, bool> Selector, int iStart)
        {
            if (Source == null)
                return default(IEnumerable<T>);
            IEnumerable<T> LstSel = Source.Where(Selector).Skip(iStart);
            return LstSel;
        }
        /// <summary>
        /// 列表按筛选返回
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Source"></param>
        /// <param name="Selector"></param>
        /// <returns></returns>
        public static IEnumerable<T> MySelect<T>(this IEnumerable<T> Source, Func<T, bool> Selector, int iStart, int ilen)
        {
            if (Source == null)
                return default(IEnumerable<T>);
            IEnumerable<T> LstSel = Source.Where(Selector).Skip(iStart).Take(ilen);
            return LstSel;
        }
        /// <summary>
        /// 列表按筛选返回
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Source"></param>
        /// <param name="Selector"></param>
        /// <returns></returns>
        public static IEnumerable<T> MySelect<T>(this IEnumerable<T> Source, int iStart)
        {
            if (Source == null)
                return default(IEnumerable<T>);
            IEnumerable<T> LstSel = Source.Skip(iStart);
            return LstSel;
        }
        /// <summary>
        /// 列表按筛选返回
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Source"></param>
        /// <param name="Selector"></param>
        /// <returns></returns>
        public static IEnumerable<T> MySelect<T>(this IEnumerable<T> Source, int iStart, int ilen)
        {
            if (Source == null)
                return default(IEnumerable<T>);
            IEnumerable<T> LstSel = Source.Skip(iStart).Take(ilen);
            return LstSel;
        }
        /// <summary>
        /// 列表按筛选返回首个
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Source"></param>
        /// <param name="Selector"></param>
        /// <returns></returns>
        public static T MySelectFirst<T>(this IEnumerable<T> Source, Func<T, bool> Selector)
        {
            try
            {
                if (Source == null)
                    return default(T);
                return Source.FirstOrDefault(Selector);
            }
            catch (Exception)
            {
                return default(T);
            }
          
        }
        /// <summary>
        /// 选择指定项
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Source"></param>
        /// <returns></returns>
        public static T MySelectAny<T>(this IEnumerable<T> Source, int index)
        {
            if (Source.MyCount() < index + 1)
                return default(T);
            return Source.ToList()[index];
        }
        /// <summary>
        /// 选择指定项
        /// </summary>
        /// <param name="Source"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static string MySelectAny(this IEnumerable<string> Source, int index)
        {
            if (Source.MyCount() < index + 1)
                return string.Empty;
            return Source.ToList()[index];
        }
        /// <summary>
        /// 列表上调
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="FullList"></param>
        /// <param name="SelectedList"></param>
        public static void MoveListUp<T>(IList<T> FullList, IList<T> SelectedList)
        {
            try
            {
                lock (FullList)
                {
                    T FirstSelected = default(T);
                    T LastSelected = default(T);
                    int iSelectedCount = SelectedList.MyCount();
                    if (iSelectedCount > 0)
                    {
                        FirstSelected = SelectedList.MySelectAny(0);
                        LastSelected = SelectedList.MySelectAny(iSelectedCount - 1);
                    }
                    int iFirstSelectedID = FullList.SearchIndexOfItem<T>(FirstSelected);
                    if (iFirstSelectedID <= 0)
                        return;
                    T MoveItem = FullList[iFirstSelectedID - 1];
                    FullList.Remove(MoveItem);
                    int iInsertIndex = FullList.SearchIndexOfItem(LastSelected);
                    FullList.Insert(iInsertIndex + 1, MoveItem);
                }
            }
            catch (Exception)
            {

            }
        }
        /// <summary>
        /// 列表下调
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="FullList"></param>
        /// <param name="SelectedList"></param>
        public static void MoveListDn<T>(IList<T> FullList, IList<T> SelectedList)
        {
            try
            {
                lock (FullList)
                {
                    T FirstSelected = default(T);
                    T LastSelected = default(T);
                    int iSelectedCount = SelectedList.MyCount();
                    if (iSelectedCount > 0)
                    {
                        FirstSelected = SelectedList.MySelectAny(0);
                        LastSelected = SelectedList.MySelectAny(iSelectedCount - 1);
                    }
                    int iLastSelectedID = FullList.SearchIndexOfItem(LastSelected);
                    if (iLastSelectedID >= FullList.MyCount() - 1)
                        return;
                    T MoveItem = FullList[iLastSelectedID + 1];
                    FullList.Remove(MoveItem);
                    int iInsertIndex = FullList.SearchIndexOfItem(FirstSelected);
                    FullList.Insert(iInsertIndex, MoveItem);
                }
            }
            catch (Exception)
            {

            }
        }
        /// <summary>
        /// 定位指定项的索引
        /// </summary>
        /// <param name="ObjectItem"></param>
        /// <returns></returns>
        public static int SearchIndexOfItem<T>(this IList<T> FullList,T ObjectItem) 
        {
            int iMove = 0;
            foreach (var item in FullList)
            {
                if (item.Equals(ObjectItem))
                    break;
                iMove++;
            }
            return iMove;
        }
    }

    /// <summary>
    /// 数据验证判别  
    /// 实体操作
    /// </summary>
    public static partial class sCommon
    {
        /// <summary>
        /// 比较两个字节数组是否相同
        /// </summary>
        /// <param name="bt1">组1</param>
        /// <param name="bt2">组2</param>
        /// <returns></returns>
        public static bool CompareArray(byte[] bt1, byte[] bt2)
        {
            var len1 = bt1.Length;
            var len2 = bt2.Length;
            if (len1 != len2)
            {
                return false;
            }
            for (var i = 0; i < len1; i++)
            {
                if (bt1[i] != bt2[i])
                    return false;
            }
            return true;
        }
        /// <summary>
        /// 数据区间比较
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Source"></param>
        /// <param name="LimitD"></param>
        /// <param name="LimitU"></param>
        /// <returns></returns>
        public static bool IsBetween<T>(this T Source, T LimitD, T LimitU)
        {
            return Comparer<T>.Default.Compare(Source, LimitD) >= 0 && Comparer<T>.Default.Compare(Source, LimitU) <= 0;
        }
        /// <summary>
        /// 验证数值是否在范围内
        /// </summary>
        /// <param name="proben">样本数据</param>
        /// <param name="LimitD">下限</param>
        /// <param name="LimitU">上限</param>
        /// <returns></returns>
        public static bool IsInOfRange(this string proben, int LimitD, int LimitU)
        {
            int ProbenVal = 0;
            if (!int.TryParse(proben, out ProbenVal))
                return false;
            return ProbenVal >= LimitD && ProbenVal <= LimitU;
        }
        /// <summary>
        /// 验证数值是否在范围
        /// </summary>
        /// <param name="proben"></param>
        /// <param name="LimitD"></param>
        /// <param name="LimitU"></param>
        /// <returns></returns>
        public static bool IsInOfRange(this int proben, int LimitD, int LimitU)
        {
            return proben >= LimitD && proben <= LimitU;
        }
        /// <summary>
        /// 验证数值是否是数字
        /// </summary>
        /// <param name="proben">样本数据</param>
        /// <returns></returns>
        public static bool IsNumeric(this string proben)
        {
            if (string.IsNullOrEmpty(proben)) return false;
            return double.TryParse(proben, out double ProbenVal);
        }
        /// <summary>
        /// 验证数值是否是数字
        /// </summary>
        /// <param name="proben">样本数据</param>
        /// <returns></returns>
        public static bool IsNumeric(this object proben)
        {
            string strProben = proben.ToMyString();
            return strProben.IsNumeric();
        }
        /// <summary>
        /// 判定True
        /// </summary>
        /// <param name="Source"></param>
        /// <returns></returns>
        public static bool IsTrue(this object Source)
        {
            return Source.ToMyString().ToUpper().MyContains("1,TRUE",",");
        }
        /// <summary>
        /// 判定False
        /// </summary>
        /// <param name="Source"></param>
        /// <returns></returns>
        public static bool IsFalse(this object Source)
        {
            return Source.ToMyString().ToUpper().MyContains("0,FALSE",",");
        }
        /// <summary>
        /// 字符串模糊比较相等 忽略大小写，忽略全角半角
        /// </summary>
        /// <param name="Source"></param>
        /// <param name="Dest"></param>
        /// <returns></returns>
        public static bool IsEqualsFuzzy(this string Source, string Dest)
        {
            return Source.IsEquals(Dest, StringMatchMode.IgnoreCase|StringMatchMode.IgnoreDBC);
        }
        /// <summary>
        /// 字符串比较
        /// </summary>
        /// <param name="Source"></param>
        /// <param name="Dest"></param>
        /// <param name="MatchMode">StringMatchMode.IgnoreCase|StringMatchMode.IgnoreDBC</param>
        /// <param name="AutoTrim"></param>
        /// <returns></returns>
        public static bool IsEquals(this string Source, string Dest, StringMatchMode MatchMode = StringMatchMode.WholeWord,bool AutoTrim = true)
        {
            if (Source == null || Dest == null) return false;
            if (AutoTrim)
            {
                Source = Source.Trim();
                Dest = Dest.Trim();
            }
            if (MatchMode > 0)
            {
                Source = Source.MyFormatString(MatchMode);
                Dest = Dest.MyFormatString(MatchMode);
            }
            return Source == Dest;
        }
        /// <summary>
        /// 比较相等,如遇到实体，仅仅比较类型
        /// </summary>
        /// <param name="Source"></param>
        /// <param name="Dest"></param>
        /// <returns></returns>
        public static bool MyEquals(this object Source, object Dest)
        {
            if (Source == null || Dest ==null) return false;
            Type typeSrc = Source.GetType();
            if (typeSrc.IsClass)
                return typeSrc == Dest.GetType();
            return Source == Dest;
        }
        /// <summary>
        /// 用上下限约束变量
        /// </summary>
        /// <param name="proben"></param>
        /// <param name="LimitD"></param>
        /// <param name="LimitU"></param>
        /// <returns></returns>
        public static string ConstraintByRange(this string proben, string LimitD, string LimitU)
        {
            double dbLimitD = LimitD.ToMyDouble();
            double dbLimitU = LimitU.ToMyDouble();
            double dbProben = proben.ToMyDouble();
            bool EnLimitD = LimitD.Length > 0;
            bool EnLimitU = LimitU.Length > 0;
            if (EnLimitD && EnLimitU && dbLimitD > dbLimitU)
                return proben;
            if (EnLimitD && dbProben <= dbLimitD)
                return LimitD;
            else if (EnLimitU && dbProben >= dbLimitU)
                return LimitU;
            else
                return proben;
        }
        /// <summary>
        /// 比对两个实体的属性差异并列举
        /// </summary>
        /// <typeparam name="TModel">模型类</typeparam>
        /// <param name="ObjSource">源实体</param>
        /// <param name="ObjDest">目标实体</param>
        /// <returns></returns>
        public static string ObjectDiff<TModel>(this object ObjSource, object ObjDest)
        {
            try
            {
                string strRet = string.Empty;
                PropertyInfo[] mPi = typeof(TModel).GetProperties();
                for (int i = 0; i < mPi.Length; i++)
                {
                    PropertyInfo pi = mPi[i];
                    string strSource = pi.GetValue(ObjSource, null).ToString();
                    string strDest = pi.GetValue(ObjDest, null).ToString();
                    if (strSource != strDest)
                    {
                        strRet += string.Format("{0}:ObjSource={1},ObjDest={2}\r\n", pi.Name, strSource, strDest);
                    }
                }
                return strRet;
            }
            catch (Exception )
            {
                return "Error";
            }
        }
        /// <summary>
        /// 比较两个实体的属性值是否相等
        /// </summary>
        /// <typeparam name="TModel">模型类</typeparam>
        /// <param name="ObjSource">源实体</param>
        /// <param name="ObjDest">目标实体</param>
        /// <returns></returns>
        public static bool ObjectEquals<TModel>(this object ObjSource, object ObjDest)
        {
            try
            {
                PropertyInfo[] mPi = typeof(TModel).GetProperties();
                for (int i = 0; i < mPi.Length; i++)
                {
                    PropertyInfo pi = mPi[i];
                    string strSource = pi.GetValue(ObjSource, null).ToMyString();
                    string strDest = pi.GetValue(ObjDest, null).ToMyString();
                    if (strSource != strDest)
                        return false;
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        /// <summary>
        /// 类属性值拷贝
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="toobj"></param>
        /// <param name="fromobj"></param>
        /// <returns></returns>
        public static T CopyObjValue<T>(this T toobj, Object fromobj) where T : class
        {
            if (fromobj != null && toobj != null)
            {
                var otherobjPorps = fromobj.GetType().GetProperties();
                foreach (var formp in otherobjPorps)
                {
                    var top = toobj.GetType().GetProperty(formp.Name);
                    if (top != null)
                    {
                        try
                        {
                            top.SetValue(toobj, formp.GetValue(fromobj));
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }
                    }
                }
            }
            return toobj;
        }
        /// <summary>
        /// 从字典中获取键值
        /// </summary>
        /// <param name="SourceDic"></param>
        /// <param name="strKey"></param>
        /// <returns></returns>
        public static TValue DictFieldValue<TKey, TValue>(this IDictionary<TKey, TValue> SourceDic, TKey ObjKey)
        {
            if (SourceDic == null || !SourceDic.ContainsKey(ObjKey))
                return default(TValue);
            lock (SourceDic)
            {
                return SourceDic[ObjKey];
            }
        }
        /// <summary>
        /// 添加字典内容
        /// </summary>
        /// <param name="SourceDic">源字典</param>
        /// <param name="strKey">添加键</param>
        /// <param name="strValue">添加值</param>
        /// <returns>添加后的字典</returns>
        public static void AppandDict<TKey, TValue>(this Dictionary<TKey, TValue> SourceDic,
            TKey ObjKey, TValue ObjValue)
        {
            if (SourceDic == null)
                SourceDic = new Dictionary<TKey, TValue>();
            if (string.IsNullOrEmpty(ObjKey.ToMyString()))
                return;
            if (SourceDic.ContainsKey(ObjKey))
                SourceDic[ObjKey] = ObjValue;
            else 
            {
                lock (ObjKey)
                    SourceDic.Add(ObjKey, ObjValue);
            }
        }
        /// <summary>
        /// 添加列表内容
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="SourceList"></param>
        /// <param name="Value"></param>
        public static void AppandList<T>(this List<T> SourceList,T Value)
        {
            if (SourceList == null)
                SourceList = new List<T>();
            if (Value == null) return;
            if (!SourceList.Contains(Value))
                SourceList.Add(Value);
        }

        /// <summary>
        /// 列配置字典中转换字段
        /// </summary>
        /// <param name="KeyField"></param>
        /// <param name="ValueField"></param>
        /// <param name="DicCodeBook"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static string KeyToValue(this string KeyItem, string ValueField, Dictionary<string, ModelSheetColumn> DicCodeBook)
        {
            if (string.IsNullOrEmpty(KeyItem)) return string.Empty;
            foreach (string item in DicCodeBook.Keys)
            {
                if (item != KeyItem)
                    continue;
                ModelSheetColumn modCol = DicCodeBook.DictFieldValue(KeyItem);
                return modCol.GetPropValue(ValueField).ToMyString();
            }
            return string.Empty;
        }

        /// <summary>
        /// 列配置字典中转换字段
        /// </summary>
        /// <param name="KeyField"></param>
        /// <param name="ValueField"></param>
        /// <param name="DicCodeBook"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static string KeyToValue(this string KeyItem,string strKeyField, string ValueField, List<ModelSheetColumn> LstCodeBook)
        {
            if (string.IsNullOrEmpty(KeyItem)) return string.Empty;
            foreach (ModelSheetColumn modCol in LstCodeBook)
            {
                if (modCol.GetPropValue(strKeyField).ToMyString() != KeyItem)
                    continue;
                return modCol.GetPropValue(ValueField).ToMyString();
            }
            return string.Empty;
        }

        /// <summary>
        /// 列配置字典中转换字段
        /// </summary>
        /// <param name="KeyField"></param>
        /// <param name="DicCodeBook"></param>
        /// <returns></returns>
        public static string ColNameToBind(this string KeyItem, Dictionary<string, ModelSheetColumn> DicCodeBook)
        {
            return KeyToValue(KeyItem, "ColBind", DicCodeBook);
        }

        /// <summary>
        /// 列配置字典中转换字段
        /// </summary>
        /// <param name="KeyItem"></param>
        /// <param name="DicCodeBook"></param>
        /// <returns></returns>
        public static string ColBindToName(this string KeyItem, Dictionary<string, ModelSheetColumn> DicCodeBook)
        {
            return KeyToValue(KeyItem, "ColName", DicCodeBook);
        }

        /// <summary>
        /// 列配置字典中转换字段
        /// </summary>
        /// <param name="KeyItem"></param>
        /// <param name="LstCodeBook"></param>
        /// <returns></returns>
        public static string ColNameToBind(this string KeyItem, List<ModelSheetColumn> LstCodeBook)
        {
            return KeyToValue(KeyItem, "ColName", "ColBind", LstCodeBook);
        }

        /// <summary>
        /// 列配置字典中转换字段
        /// </summary>
        /// <param name="KeyItem"></param>
        /// <param name="LstCodeBook"></param>
        /// <returns></returns>
        public static string ColBindToName(this string KeyItem, List<ModelSheetColumn> LstCodeBook)
        {
            return KeyToValue(KeyItem, "ColBind", "ColName", LstCodeBook);
        }
    }

    /// <summary>
    /// 时间处理
    /// </summary>
    public static partial class sCommon
    {
        /// <summary>
        /// 比较两个时间大小
        /// 不给定日期时系统会自动取当天
        /// </summary>
        /// <param name="strTime1"></param>
        /// <param name="strTime2"></param>
        /// <returns>-2 -1 0 1</returns>
        public static int CompareTime(string strTime1, string strTime2)
        {
            int ret = -2;
            DateTime dt1 = new DateTime();
            DateTime dt2 = new DateTime();
            bool bTime1 = DateTime.TryParse(strTime1, out dt1);
            bool bTime2 = DateTime.TryParse(strTime2, out dt2);
            if (bTime1 && bTime2)
            {
                ret = DateTime.Compare(dt1, dt2);
            }
            return ret;
        }
        /// <summary>
        /// 追加小时
        /// </summary>
        /// <param name="strTime"></param>
        /// <param name="dHour"></param>
        /// <returns></returns>
        public static string AddTimeHour(this string strTime, double dHour)
        {
            DateTime dt = new DateTime();
            bool bTime1 = DateTime.TryParse(strTime, out dt);
            if (bTime1)
            {
                dt = dt.AddHours(dHour);
                return dt.ToMyDateTimeStr();
            }
            return string.Empty;
        }

        /// <summary>
        /// 时间间隔
        /// </summary>
        /// <param name="StartTime"></param>
        /// <param name="EndTime"></param>
        /// <returns></returns>
        public static TimeSpan TimeLong(this string StartTime, string EndTime)
        {
            DateTime dtStart = StartTime.ToMyDateTime();
            DateTime dtEndTime = EndTime.ToMyDateTime();
            TimeSpan ts = dtEndTime - dtStart;
            return ts;
        }

        /// <summary>
        /// 追加时间间隔
        /// </summary>
        /// <param name="strTime"></param>
        /// <param name="dHour"></param>
        /// <returns></returns>
        public static string AddTime(this string strTime, int dHour = 0, int dMin = 0, int dSec = 0)
        {
            DateTime dt = new DateTime();
            bool bTime1 = DateTime.TryParse(strTime, out dt);
            if (bTime1)
            {
                dt = dt.Add(new TimeSpan(dHour, dMin, dSec));
                return dt.ToMyDateTimeStr();
            }
            return string.Empty;
        }

        /// <summary>
        /// 判断是否到时
        /// </summary>
        /// <param name="ObjTime"></param>
        /// <returns></returns>
        public static bool IsTimeArrive(this string ObjTime)
        {
            if (string.IsNullOrEmpty(ObjTime))
                return false;
            return CompareTime(SystemDefault.StringTimeNow, ObjTime) >= 0;
        }

        /// <summary>
        /// 是否时间格式
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsTimeMate(this string str)
        {
            return DateTime.TryParse(str, out DateTime std);
        }

        /// <summary>
        /// 是否当日
        /// </summary>
        /// <param name="strTime"></param>
        /// <returns></returns>
        public static bool IsToday(this string strTime)
        {
            if (string.IsNullOrEmpty(strTime) || !strTime.IsTimeMate())
                return false;
            return CompareTime(SystemDefault.Today.ToMyDateTime("yyyy-MM-dd").ToMyString(),
                strTime.ToMyDateTime("yyyy-MM-dd").ToMyString()) == 0;
        }

        /// <summary>
        /// 是否昨日
        /// </summary>
        /// <param name="strTime"></param>
        /// <returns></returns>
        public static bool IsYesterday(this string strTime)
        {
            if (string.IsNullOrEmpty(strTime) || !strTime.IsTimeMate())
                return false;
            return CompareTime(SystemDefault.Yesterday.ToMyDateTime("yyyyMMdd").ToMyString(),
                strTime.ToMyDateTime("yyyyMMdd").ToMyString()) == 0;
        }

        /// <summary>
        /// 是否明天
        /// </summary>
        /// <param name="strTime"></param>
        /// <returns></returns>
        public static bool IsTomorrow(this string strTime)
        {
            if (string.IsNullOrEmpty(strTime) || !strTime.IsTimeMate())
                return false;
            return CompareTime(SystemDefault.Tomorrow.ToMyDateTime("yyyyMMdd").ToMyString(),
                strTime.ToMyDateTime("yyyyMMdd").ToMyString()) == 0;
        }
    }

    /// <summary>
    /// Provides extension methods for LinkedList.
    /// </summary>
    public static partial class sCommon
    {
        /// <summary>
        /// Finds the next node after the given node that contains the specified value.
        /// </summary>
        /// <typeparam name="T">The type of value in the linked list.</typeparam>
        /// <param name="list">The linked list.</param>
        /// <param name="node">The node after which to search for the value in the linked list, or <c>null</c> to search from the beginning.</param>
        /// <param name="value">The value to locate in the linked list.</param>
        /// <returns>The first node after the given node that contains the specified value, if found; otherwise, <c>null</c>.</returns>
        public static LinkedListNode<T> FindNext<T>(this LinkedList<T> list, LinkedListNode<T> node, T value)
        {
            if (list == null)
            {
                throw new ArgumentNullException("list");
            }

            if (node == null)
            {
                return list.Find(value);
            }

            if (list != node.List)
            {
                throw new ArgumentException("The list does not contain the given node.");
            }

            EqualityComparer<T> comparer = EqualityComparer<T>.Default;

            // Skip the given node.
            node = node.Next;
            while (node != null)
            {
                if (value != null)
                {
                    if (comparer.Equals(node.Value, value))
                    {
                        return node;
                    }
                }
                else if (node.Value == null)
                {
                    return node;
                }
                node = node.Next;
            }

            return null;
        }

        /// <summary>
        /// Finds the previous node before the given node that contains the specified value.
        /// </summary>
        /// <typeparam name="T">The type of value in the linked list.</typeparam>
        /// <param name="list">The linked list.</param>
        /// <param name="node">The node before which to search for the value in the linked list, or <c>null</c> to search from the end.</param>
        /// <param name="value">The value to locate in the linked list.</param>
        /// <returns>The first node before the given node that contains the specified value, if found; otherwise, <c>null</c>.</returns>
        public static LinkedListNode<T> FindPrevious<T>(this LinkedList<T> list, LinkedListNode<T> node, T value)
        {
            if (list == null)
            {
                throw new ArgumentNullException("list");
            }

            if (node == null)
            {
                return list.FindLast(value);
            }

            if (list != node.List)
            {
                throw new ArgumentException("The list does not contain the given node.");
            }

            EqualityComparer<T> comparer = EqualityComparer<T>.Default;

            // Skip the given node.
            node = node.Previous;
            while (node != null)
            {
                if (value != null)
                {
                    if (comparer.Equals(node.Value, value))
                    {
                        return node;
                    }
                }
                else if (node.Value == null)
                {
                    return node;
                }
                node = node.Previous;
            }

            return null;
        }
    }

    /// <summary>
    /// Provides extension methods for rects.
    /// </summary>
    public static partial class sCommon
    {
        /// <summary>
        /// Returns the center point of the <see cref="Rect"/>.
        /// </summary>
        /// <param name="rect">The rect to return the center point of.</param>
        /// <returns>The center <see cref="Point"/> of the <paramref name="rect"/>.</returns>
        public static Point GetCenter(this Rect rect)
        {
            return new Point(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);
        }

        /// <summary>
        /// Returns whether the <see cref="Rect"/> defines a real area in space.
        /// </summary>
        /// <param name="rect">The rect to test.</param>
        /// <returns><c>true</c> if rect defines an area or point in finite space, which is not the case for <see cref="Rect.Empty"/> or if any of the fields are <see cref="double.NaN"/>.</returns>
        public static bool IsDefined(this Rect rect)
        {
            return rect.Width >= 0.0
                && rect.Height >= 0.0
                && rect.Top < Double.PositiveInfinity
                && rect.Left < Double.PositiveInfinity
                && (rect.Top > Double.NegativeInfinity || rect.Height == Double.PositiveInfinity)
                && (rect.Left > Double.NegativeInfinity || rect.Width == Double.PositiveInfinity);
        }

        /// <summary>
        /// Indicates whether the specified rectangle intersects with the current rectangle, properly considering the empty rect and infinities.
        /// </summary>
        /// <param name="self">The current rectangle.</param>
        /// <param name="rect">The rectangle to check.</param>
        /// <returns><c>true</c> if the specified rectangle intersects with the current rectangle; otherwise, <c>false</c>.</returns>
        public static bool Intersects(this Rect self, Rect rect)
        {
            return (self.IsEmpty || rect.IsEmpty)
                || (self.Width == Double.PositiveInfinity || self.Right >= rect.Left)
                && (rect.Width == Double.PositiveInfinity || rect.Right >= self.Left)
                && (self.Height == Double.PositiveInfinity || self.Bottom >= rect.Top)
                && (rect.Height == Double.PositiveInfinity || rect.Bottom >= self.Top);
        }
    }

    /// <summary>
    /// 反射获取信息
    /// </summary>
    public static partial class sCommon
    {
        /// <summary>
        /// 获取程序集设定的公司名称
        /// </summary>
        /// <returns></returns>
        private static string GetCompanyName()
        {
            string fileName = System.Windows.Forms.Application.ExecutablePath;
            System.Diagnostics.FileVersionInfo VersionInfo = System.Diagnostics.FileVersionInfo.GetVersionInfo(fileName);
            return VersionInfo.CompanyName;
        }

        /// <summary>
        /// 获取程序集设定的公司名称
        /// </summary>
        public static string CompanyName
        {
            get
            {
                return GetCompanyName().Length > 0 ? GetCompanyName() : "南京普瑞斯通科技有限公司";
            }
        }
    }

    /// <summary>
    /// 从StatckFrame堆栈帧中获取信息
    /// </summary>
    public static partial class sCommon
    {
        /// <summary>
        /// 获取调用者信息
        /// </summary>
        /// <param name="CallerMethodName"></param>
        /// <param name="CallerClassName"></param>
        /// <returns></returns>
        public static bool GetCallerMessage(out string CallerMethodName,out string CallerClassName)
        {
            try
            {
                StackFrame frame = new StackFrame(1);
                MethodBase method = frame.GetMethod();
                CallerMethodName = method.Name;
                CallerClassName = method.DeclaringType.Name;
                return true;
            }
            catch (Exception)
            {
                CallerMethodName = string.Empty;
                CallerClassName = string.Empty;
                return false;
            }
        }

        public static void aa()
        {
            MethodBase method = MethodBase.GetCurrentMethod();
            string funcName = method.Name;
            ParameterInfo[] parameters = method.GetParameters();
            foreach (ParameterInfo pi in parameters)
            {
                string strName = pi.Name;
                Type type = pi.ParameterType;
            }
        }
    }
}
