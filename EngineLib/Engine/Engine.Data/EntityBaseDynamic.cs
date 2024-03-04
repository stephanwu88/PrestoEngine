using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using Microsoft.CSharp.RuntimeBinder;
using System.Runtime.CompilerServices;
using System;

namespace Engine.Data
{
    /// <summary>
    /// 动态属性类
    /// </summary>
    public class DynamicDictionary : DynamicObject
    {
        // 内部字典
        protected Dictionary<string, object> Properties = new Dictionary<string, object>();
        public int Count => Properties.Count;
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            string name = binder.Name;
            return Properties.TryGetValue(name, out result);
        }
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            if (!Properties.ContainsKey(binder.Name))
            {
                Properties.Add(binder.Name, value.ToString());
            }
            Properties[binder.Name] = value;
            return true;
        }
        public object GetMember(string propName)
        {
            CallSiteBinder binder = Binder.GetMember(CSharpBinderFlags.None, propName, this.GetType(), new List<CSharpArgumentInfo>{
                       CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None,null)});
            CallSite<Func<CallSite, object, object>> callsite =
                CallSite<Func<CallSite, object, object>>.Create(binder);
            return callsite.Target(callsite, this);
        }
        public void SetMember(string propName, object val)
        {
            CallSiteBinder binder = Binder.SetMember(CSharpBinderFlags.None, propName, this.GetType(), new List<CSharpArgumentInfo>{
                       CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None,null)});
            CallSite<Func<CallSite, object, object, object>> callsite =
                CallSite<Func<CallSite, object, object, object>>.Create(binder);
            callsite.Target(callsite, this, val);
        }
    }

    /// <summary>
    /// 动态属性带通知类
    /// </summary>
    public abstract class DynamicEntityBase : DynamicDictionary,INotifyPropertyChanged
    {
        public dynamic Props = new DynamicDictionary();
        //public dynamic Properties = new Dictionary<string, object>();
        //public dynamic Props = new ExpandoObject();
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// 属性发生改变时调用该方法发出通知
        /// </summary>
        /// <param name="propertyName">属性名称</param>
        public virtual void RaisePropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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
                        RaisePropertyChanged(propertyName);
                    }
                }
                else
                {
                    Properties.Add(propertyName, value);
                    RaisePropertyChanged(propertyName);
                }
            }
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

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyName"></param>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        protected virtual void SetAndNotifyIfChanged<T>(string propertyName, ref T oldValue, T newValue)
        {
            if (oldValue == null && newValue == null) return;
            if (oldValue != null && oldValue.Equals(newValue)) return;
            if (newValue != null && newValue.Equals(oldValue)) return;
            oldValue = newValue;
            RaisePropertyChanged(propertyName);
        }

        /// <summary>
        /// 属性键值对更新到字典，变化时通知
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        public void TrySetProperty(string propertyName, object value)
        {
            if (string.IsNullOrEmpty(propertyName))
                return;
            lock (Properties)
            {
                if (Properties.ContainsKey(propertyName))
                {
                    bool ret = object.Equals(Properties[propertyName], value);
                    //bool ret = object.ReferenceEquals(Properties[propertyName], value);
                    if (!ret)
                    {
                        Properties[propertyName] = value;
                        //Props.propertyName = value;
                        RaisePropertyChanged(propertyName);
                    }
                }
                else
                {
                    Properties.Add(propertyName, value);
                    //Props.propertyName = value;
                    RaisePropertyChanged(propertyName);
                }
            }
        }

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
        /// 索引器，可以用字符串访问变量属性
        /// </summary>
        /// <param name="_propertyName">属性名称</param>
        /// <returns>属性值，若传入的属性名称不存在，返回null</returns>
        public virtual object this[string _propertyName]
        {
            get
            {
                if (Properties.ContainsKey(_propertyName))
                    return Properties[_propertyName];
                else
                    return null;
            }
            set
            {
                if (Properties.ContainsKey(_propertyName))
                {
                    bool ret = object.Equals(Properties[_propertyName], value);
                    if (!ret)
                    {
                        Properties[_propertyName] = value;
                        //Props.propertyName = value;
                        RaisePropertyChanged(_propertyName);
                    }
                }
            }
        }
    }
}