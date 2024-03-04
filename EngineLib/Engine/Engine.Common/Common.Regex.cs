using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Engine.Common
{
    /// <summary>
    /// 正则表达式处理
    /// </summary>
    public static partial class sCommon
    {
        /// <summary>
        /// 源字符串中获取格式化字符串
        /// </summary>
        /// <param name="Source"></param>
        /// <param name="Pattern">"@(.*?)@"</param>
        /// <returns></returns>
        public static List<string> RegexMatchedList(this string Source, string Pattern, bool RemoveRepeat = false)
        {
            List<string> FormattedStrings = new List<string>();
            try
            {
                Regex regex = new Regex(Pattern);
                MatchCollection matches = regex.Matches(Source);
                foreach (Match match in matches)
                {
                    if (RemoveRepeat && FormattedStrings.Contains(match.Value))
                        continue;
                    FormattedStrings.Add(match.Value);
                }

            }
            catch (Exception)
            {

            }
            return FormattedStrings;
        }

        /// <summary>
        /// 源字符串中获取正则格式化的内文
        /// </summary>
        /// <param name="Source"></param>
        /// <param name="Pattern">"@(.*?)@"</param>
        /// <returns></returns>
        public static List<string> RegexMatchedInnerText(this string Source, string Pattern, bool RemoveRepeat = true)
        {
            List<string> FormattedContents = new List<string>();
            try
            {
                Regex regex = new Regex(Pattern);
                MatchCollection matches = regex.Matches(Source);
                foreach (Match match in matches)
                {
                    if (RemoveRepeat && FormattedContents.Contains(match.Groups[1].Value))
                        continue;
                    FormattedContents.Add(match.Groups[1].Value);
                }

            }
            catch (Exception)
            {

            }
            return FormattedContents;
        }

        /// <summary>
        /// 源字符串中包含格式化字符串
        /// </summary>
        /// <param name="Source"></param>
        /// <param name="Pattern"></param>
        /// <returns></returns>
        public static bool IsRegexMatched(this string Source, string Pattern)
        {
            Regex regex = new Regex(Pattern);
            Match match = regex.Match(Source);
            return match?.Success == true;
        }

        /// <summary>
        /// 源字符串中格式化字符串的数量
        /// </summary>
        /// <param name="Source"></param>
        /// <param name="Pattern"></param>
        /// <returns></returns>
        public static int RegexMatchedCount(this string Source, string Pattern)
        {
            Regex regex = new Regex(Pattern);
            MatchCollection matches = regex.Matches(Source);
            return matches.Count;
        }

        /// <summary>
        /// 正则替换
        /// </summary>
        /// <param name="Source">元字符串</param>
        /// <param name="Pattern">Ex： @"\[.*?\]"  格式[x]</param>
        /// <param name="ReplacePattern"><$&></param>
        /// <returns></returns>
        public static string RegexReplace(this string Source, string Pattern, string ReplacePattern)
        {
            try
            {
                string result = Regex.Replace(Source, Pattern, ReplacePattern);
                return result;
            }
            catch (Exception ex)
            {
                return Source;
            }
        }

        /// <summary>
        /// 是否函数表达式
        /// </summary>
        /// <param name="Source"></param>
        /// <returns></returns>
        public static bool IsFunction(this string Source)
        {
            string pattern = @"^\w+\((\w+(,\s*\w+)*)?\)$";
            return Source.IsRegexMatched(pattern);
        }

        /// <summary>
        /// 验证手机号
        /// </summary>
        /// <param name="Source"></param>
        /// <returns></returns>
        public static bool IsPhoneNumber(this string Source)
        {
            string pattern = @"^1[3456789]\d{9}$";
            return Source.IsRegexMatched(pattern);
        }

        /// <summary>
        /// 验证身份证
        /// </summary>
        /// <param name="Source"></param>
        /// <returns></returns>
        public static bool IsPersonalIDCard(this string Source)
        {
            string pattern = @"^\d{17}[\dXx]$";
            return Source.IsRegexMatched(pattern);
        }

        /// <summary>
        /// 验证文件路径
        /// </summary>
        /// <param name="Source"></param>
        /// <returns></returns>
        public static bool IsFilePath(this string Source)
        {
            string pattern = @"^(?<fpath>([a-zA-Z]:\\)([\s\.\-\w]+\\)*)(?<fname>[\w]+.[\w]+)";
            return Source.IsRegexMatched(pattern);
        }
    }
}
