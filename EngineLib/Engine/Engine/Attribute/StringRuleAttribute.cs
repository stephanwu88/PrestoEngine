using System;
using System.Reflection;

namespace Engine.Common
{
    /// <summary>
    /// StringRule
    /// </summary>
    public class StringRuleAttribute : Attribute
    {
        private StringRuleDef _StringRule = new StringRuleDef();

        public StringRuleAttribute()
        {
         
        }

        /// <summary>
        /// 最小长度
        /// </summary>
        public int MinLen { get => _StringRule.MinLen; set { _StringRule.MinLen = value; } }

        /// <summary>
        /// 最小长度
        /// </summary>
        public int MaxLen { get => _StringRule.MaxLen; set { _StringRule.MaxLen = value; } }

        /// <summary>
        /// 包含字符串
        /// </summary>
        public string ContainString { get => _StringRule.ContainString; set { _StringRule.ContainString = value; } }

        /// <summary>
        /// 起始字符匹配
        /// </summary>
        public string StartWith { get => _StringRule.StartWith; set { _StringRule.StartWith = value; } }

        /// <summary>
        /// 结束字符匹配
        /// </summary>
        public string EndWith { get => _StringRule.EndWith; set { _StringRule.EndWith = value; } }

        /// <summary>
        /// 默认错误信息
        /// </summary>
        public string ErrorMessage { get => _StringRule.ErrorMessage; set { _StringRule.ErrorMessage = value; } }

        /// <summary>
        /// 获取属性值的自定义特性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="PropertyName"></param>
        /// <returns></returns>
        public static StringRuleDef Rule<T>(string PropertyName)
        {
            PropertyInfo pi = typeof(T).GetProperty(PropertyName);
            StringRuleAttribute[] attributes = (StringRuleAttribute[])pi.GetCustomAttributes(typeof(StringRuleAttribute), true);
            if (attributes.Length > 0)
                return attributes[0]._StringRule;
            return null;
        }

        /// <summary>
        /// 验证所有规则设定
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="PropertyName"></param>
        /// <param name="PropValue"></param>
        /// <returns></returns>
        public static CallResult Validate<T>(string PropertyName,object PropValue)
        {
            CallResult _res = new CallResult() { Success = true, Result = "" };
            PropertyInfo pi = typeof(T).GetProperty(PropertyName);
            StringRuleAttribute[] attributes = (StringRuleAttribute[])pi.GetCustomAttributes(typeof(StringRuleAttribute), true);
            if (attributes.Length > 0)
            {
                StringRuleAttribute attr = attributes[0];

                _res = attr.ValidateStringLen(PropValue);
                if (_res.Fail)
                    return _res;
                _res = attr.ValidateStringContain(PropValue);
                if (_res.Fail)
                    return _res;
                _res = attr.ValidateStringStart(PropValue);
                if (_res.Fail)
                    return _res;
                _res = attr.ValidateStringEnd(PropValue);
                return _res;
            }
            return _res;
        }

        /// <summary>
        /// 验证字符串长度
        /// </summary>
        /// <param name="PropValue"></param>
        /// <returns></returns>
        public CallResult ValidateStringLen(object PropValue)
        {
            CallResult _res = new CallResult() { Success = true, Result = "" };
            int length = PropValue.ToMyString().Length;
            bool IsTooMax = length > MaxLen && MaxLen != 0;
            bool IsTooMin = length < MinLen && MinLen != 0;
            if (IsTooMax || IsTooMin)
            {
                _res.Result = string.Format("{0}\r\n", ErrorMessage);
                string strMinLen = string.Empty;
                string strMaxLen = string.Empty;
                if (MinLen > 0) strMinLen = MinLen.ToMyString();
                if (MaxLen > 0) strMaxLen = MaxLen.ToMyString();
                _res.Result += string.Format("长度:{0}-{1}", strMinLen, strMaxLen);
                _res.Success = false;
                return _res;
            }
            return _res;
        }

        /// <summary>
        /// 验证是否包含字符串
        /// </summary>
        /// <param name="PropValue"></param>
        /// <param name="Compare"></param>
        /// <returns></returns>
        public CallResult ValidateStringContain(object PropValue,string Compare="")
        {
            CallResult _res = new CallResult() { Success = true, Result = "" };
            string strPropValue = PropValue.ToMyString();
            string strCompare = string.Empty;
            if (string.IsNullOrEmpty(Compare))
                strCompare = Compare;
            else
                strCompare = ContainString;          
            if (!strPropValue.Contains(strCompare) && !string.IsNullOrEmpty(strCompare))
            {
                _res.Result = string.Format("{0}\r\n", ErrorMessage);
                _res.Result = string.Format("未包含字符:{0}", strCompare);
                _res.Success = false;
                return _res;
            }
            return _res;
        }

        /// <summary>
        /// 验证字符串起始
        /// </summary>
        /// <param name="PropValue"></param>
        /// <param name="Start"></param>
        /// <returns></returns>
        public CallResult ValidateStringStart(object PropValue, string Start = "")
        {
            CallResult _res = new CallResult() { Success = true, Result = "" };
            string strPropValue = PropValue.ToMyString();
            string strStart = string.Empty;
            if (string.IsNullOrEmpty(Start))
                strStart = Start;
            else
                strStart = StartWith;
            if (!strPropValue.Contains(strStart) && !string.IsNullOrEmpty(strStart))
            {
                _res.Result = string.Format("{0}\r\n", ErrorMessage);
                _res.Result = string.Format("起始:{0}", strStart);
                _res.Success = false;
                return _res;
            }
            return _res;
        }

        /// <summary>
        /// 验证字符串起始
        /// </summary>
        /// <param name="PropValue"></param>
        /// <param name="Start"></param>
        /// <returns></returns>
        public CallResult ValidateStringEnd(object PropValue, string End = "")
        {
            CallResult _res = new CallResult() { Success = true, Result = "" };
            string strPropValue = PropValue.ToMyString();
            string strEnd = string.Empty;
            if (string.IsNullOrEmpty(End))
                strEnd = End;
            else
                strEnd = StartWith;
            if (!strPropValue.Contains(strEnd) && !string.IsNullOrEmpty(strEnd))
            {
                _res.Result = string.Format("{0}\r\n", ErrorMessage);
                _res.Result = string.Format("结尾:{0}", strEnd);
                _res.Success = false;
                return _res;
            }
            return _res;
        }
    }


    /// <summary>
    /// 字符串规则定义
    /// </summary>
    public class StringRuleDef
    {
        /// <summary>
        /// 最小长度
        /// </summary>
        public int MinLen { get; set; }
        /// <summary>
        /// 最大长度
        /// </summary>
        public int MaxLen { get; set; }
        /// <summary>
        /// 包含字符串
        /// </summary>
        public string ContainString { get; set; }
        /// <summary>
        /// 起始字符匹配
        /// </summary>
        public string StartWith { get; set; }
        /// <summary>
        /// 结束字符匹配
        /// </summary>
        public string EndWith { get; set; }
        /// <summary>
        /// 默认错误信息
        /// </summary>
        public string ErrorMessage { get; set; }
    }
}
