using System;

namespace Engine.Common
{
    /// <summary>
    /// 特性基类
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Enum | AttributeTargets.Property | AttributeTargets.Class)]
    public abstract class AttributeBase : Attribute
    {
        public virtual string Name { get; set; }
        /// <summary>
        ///     验证失败时给出的消息
        /// </summary>
        public string ErrorMessage { get; protected set; }

        /// <summary>
        ///     是否满足验证规则，若满足返回true
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public virtual bool IsValid(object obj)
        {
            return true;
        }
    }
}
