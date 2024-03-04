using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;

namespace Engine.Data
{
    /// <summary>
    /// 实体属性变化通知类
    /// </summary>
    public abstract class NotifyObject : INotifyPropertyChanged
    {
        [JsonIgnore]
        public bool EnableRaiseChanged = true;

        /// <summary>
        /// 页面处于设计器模式
        /// </summary>
        [JsonIgnore]
        public bool IsDesignMode => DesignerProperties.GetIsInDesignMode(new DependencyObject());

        /// <summary>
        /// 
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
       
        /// <summary>
        /// 属性发生改变时调用该方法发出通知
        /// </summary>
        /// <param name="propertyName">属性名称</param>
        public virtual void RaisePropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (!EnableRaiseChanged)
                return;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="member"></param>
        /// <param name="val"></param>
        /// <param name="propertyName"></param>
        protected virtual void RaisePropertyChanged<T>(ref T member, T val, [CallerMemberName] string propertyName = null)
        {
            if (object.Equals(member, val))
                return;
            member = val;
            RaisePropertyChanged(propertyName);
        }

        protected virtual void SetAndNotifyIfChanged<T>(string propertyName, ref T oldValue, T newValue)
        {
            if (oldValue == null && newValue == null) return;
            if (oldValue != null && oldValue.Equals(newValue)) return;
            if (newValue != null && newValue.Equals(oldValue)) return;
            oldValue = newValue;
            RaisePropertyChanged(propertyName);
        }

        /// <summary>
        /// 反射所有属性并通知
        /// </summary>
        /// <returns></returns>
        public void RaisePropertyNotify()
        {
            PropertyInfo[] properties = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo pi in properties)
            {
                // 属性Error和Item来自接口IDataErrorInfo，无需进行验证
                if (pi.Name == "Error" || pi.Name == "Item")
                    continue;
                RaisePropertyChanged(pi.Name);
            }
        }
    }

    /// <summary>
    /// 实体基础类，强化属性通知类，属性索引器
    /// </summary>
    public abstract class EntityBase: NotifyObject
    {
        /// <summary>
        /// 内部属性字典库
        /// </summary>
        protected Dictionary<string, object> Properties = new Dictionary<string, object>();

        /// <summary>
        /// 属性字典元素数量
        /// </summary>
        public int Count => Properties.Count;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public object TryGet(string propertyName)
        {
            if (Properties.ContainsKey(propertyName))
                return Properties[propertyName];
            else
                return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        public void TrySet(string propertyName, object value)
        {
            RaisePropertyChanged(propertyName, value);
        }

        /// <summary>
        /// 属性键值对更新到字典，变化时通知
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        public void RaisePropertyChanged(string propertyName, object value)
        {
            lock (Properties)
            {
                if (Properties.ContainsKey(propertyName))
                {
                    bool ret = object.Equals(Properties[propertyName], value);
                    if (!ret)
                    {
                        Properties[propertyName] = value;
                        base.RaisePropertyChanged(propertyName);
                    }
                }
                else
                {
                    Properties.Add(propertyName, value);
                    base.RaisePropertyChanged(propertyName);
                }
            }
        }

        /// <summary>
        /// 索引器，可以用字符串访问变量属性
        /// </summary>
        /// <param name="_propertyName">属性名称</param>
        /// <returns>属性值，若传入的属性名称不存在，返回null</returns>
        public virtual object this[string _propertyName]
        {
            get
            {
                //从所有获取的属性值中找到传入的属性值
                var pi = this.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly).
                    FirstOrDefault(p => p.Name.Equals(_propertyName));
                if (null != pi && null != pi.GetMethod)
                {
                    return pi.GetValue(this);
                }
                else
                {
                    return null;
                }
            }
            set
            {
                var pi = this.GetType().GetProperties().FirstOrDefault(p => p.Name.Equals(_propertyName));
                if (null != pi && null != pi.SetMethod)
                {
                    if (pi.PropertyType.Equals(value.GetType()))
                    {
                        pi.SetValue(this, value);
                    }
                    else
                    {
                        pi.SetValue(this, Convert.ChangeType(value, pi.PropertyType));
                    }
                }
            }
        }
    }
}
