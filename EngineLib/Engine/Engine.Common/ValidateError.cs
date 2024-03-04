using Engine.Data;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace Engine.Common
{
    /// <summary>
    /// 实体验证抽象类，负责指定验证所有规则，被继承使用
    /// </summary>
    public abstract class ValidateError<T> : NotifyObject, IDataErrorInfo
    {
        protected string _Error;
        /// <summary>
        /// DataErrorInfo错误通道
        /// </summary>
        [JsonIgnore]
        public string Error { get => _Error; }

        /// <summary>
        /// 属性索引 
        /// 前端绑定的DataErrorInfo通过此处返回
        /// </summary>
        /// <param name="PropName"></param>
        /// <returns></returns>
        public string this[string PropName]
        {
            get => Validate(PropName).Result.ToMyString();
        }

        /// <summary>
        /// 验证对象
        /// </summary>
        /// <returns></returns>
        public virtual CallResult Validate()
        {
            return Validate(null, true);
        }

        /// <summary>
        /// 指定验证对象列表
        /// </summary>
        /// <param name="ValidatePropList">验证参与属性列表</param>
        /// <param name="IfContain">指定列表的属性参与验证</param>
        /// <returns></returns>
        public virtual CallResult Validate(List<string> ValidatePropList = null, bool IfContain = true)
        {
            CallResult _result = new CallResult() { Success = true };
            PropertyInfo[] properties = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo pi in properties)
            {
                // 属性Error和Item来自接口IDataErrorInfo，无需进行验证
                if (pi.Name == "Error" || pi.Name == "Item")
                    continue;
                if (ValidatePropList != null)
                {
                    if (IfContain && !ValidatePropList.Contains(pi.Name) || 
                        (!IfContain && ValidatePropList.Contains(pi.Name)))
                        continue;
                }
                _result = Validate(pi.Name);
                if (_result.Fail)
                    return _result;
            }
            return _result;
        }

        /// <summary>
        /// 验证指定属性的关联规则
        /// </summary>
        /// <returns></returns>
        public virtual CallResult Validate(string PropName)
        {
            CallResult _result = new CallResult() { Success = true };
            PropertyInfo pi = this.GetType().GetProperty(PropName);
            if (pi == null)
                return _result;
            // 属性Error和Item来自接口IDataErrorInfo，无需进行验证
            if (pi.Name == "Error" || pi.Name == "Item")
                return _result;
            object value = pi.GetValue(this, null);
            object[] attributes = pi.GetCustomAttributes(true);
            foreach (var attribute in attributes)
            {
                if (attribute is NumberRuleAttribute numberRule)
                {
                    if (!numberRule.IsValid(value))
                    {
                        _Error = numberRule.ErrorMessage;
                        _result.Result = _Error;
                        return _result;
                    }
                }
                else if (attribute is StringRuleAttribute stringRule)
                {
                    _result = StringRuleAttribute.Validate<T>(PropName, value);
                    if (_result.Fail)
                    {
                        _Error = _result.Result.ToMyString();
                        return _result;
                    }
                }
            }
            return _result;
        }
    }
}
