using System;
using System.Collections.Generic;
using System.Reflection;

namespace Engine.Common
{
    /// <summary>
    /// 特性基类
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Enum | AttributeTargets.Property | AttributeTargets.Class)]
    public class BasicAttribute : Attribute
    {
        #region 内部变量
        public static readonly BasicAttribute Default = new BasicAttribute();
        private List<object> basic;
        #endregion

        #region 公共方法
        public BasicAttribute() : this(null)
        {

        }

        public BasicAttribute(List<object> basic)
        {
            this.basic = basic;
        }

        public override bool Equals(object obj)
        {
            if (obj == this)
            {
                return true;
            }
            BasicAttribute attribute = obj as BasicAttribute;
            return ((attribute != null) && (attribute.Basic == this.Basic));
        }

        public override int GetHashCode()
        {
            return this.Basic.GetHashCode();
        }

        public override bool IsDefaultAttribute()
        {
            return this.Equals(Default);
        }
        #endregion

        #region 属性
        public virtual List<object> Basic
        {
            get
            {
                return this.BasicValue;
            }
        }
        protected List<object> BasicValue
        {
            get
            {
                return this.basic;
            }
            set
            {
                this.basic = value;
            }
        }
        #endregion

        #region 公共方法
        /// <summary> 
        /// 获取枚举的自定义特性
        /// </summary> 
        /// <param name="value"></param> 
        /// <returns></returns> 
        public static List<object> GetEnumCustomAttributeContent(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());
            BasicAttribute[] attributes = (BasicAttribute[])fi.GetCustomAttributes(typeof(BasicAttribute), false);
            return (attributes.Length > 0) ? attributes[0].Basic : null;
        }
        /// <summary>
        /// 获取类上的自定义特性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static List<object> GetClassCustomAttributeContent<T>(T data)
        {
            BasicAttribute[] attributes = (BasicAttribute[])typeof(T).GetCustomAttributes(typeof(BasicAttribute), true);
            return (attributes.Length > 0) ? attributes[0].Basic : null;
        }

        /// <summary> 
        /// 获取属性值的自定义特性
        /// </summary> 
        /// <param name="value"></param> 
        /// <returns></returns> 
        public static List<object> GetPropCustomAttributeContent<T>(string PropertyName)
        {
            PropertyInfo pi = typeof(T).GetProperty(PropertyName);
            BasicAttribute[] attributes = (BasicAttribute[])pi.GetCustomAttributes(typeof(BasicAttribute), true);
            return (attributes.Length > 0) ? attributes[0].Basic : null;
        }
        #endregion
    }
}
