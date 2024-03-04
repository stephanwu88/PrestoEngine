using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Engine.MVVM
{
    /// <summary>
    /// 属性绑定
    /// </summary>
    public abstract class NotifyPropertyChangedBase : INotifyPropertyChanged
    {
        #region - MVVM -

        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }

    /// <summary> Mvvm绑定模型基类 </summary>
    public abstract class NotifyPropertyChanged : NotifyPropertyChangedBase
    {
        [Browsable(false)]
        [XmlIgnore]
        public MyCommand RelayCommand { get; set; }

        [Browsable(false)]
        [XmlIgnore]
        public MyCommand LoadedCommand { get; set; }

        [Browsable(false)]
        [XmlIgnore]
        public MyCommand CallMethodCommand { get; set; }

        protected virtual void RelayMethod(object obj)
        {

        }

        public NotifyPropertyChanged()
        {
            RelayCommand = new MyCommand(RelayMethod);

            LoadedCommand = new MyCommand(Loaded);

            CallMethodCommand = new MyCommand(CallMethod);

            RelayMethod("init");

            Init();


        }

        /// <summary> 初始化方法 </summary>
        protected virtual void Init()
        {

        }

        /// <summary> 加载方法 </summary>
        protected virtual void Loaded(object obj)
        {

        }

        /// <summary> 调用当前类指定方法 </summary>
        protected virtual void CallMethod(object obj)
        {
            string methodName = obj?.ToString();

            var method = this.GetType().GetMethod(methodName);

            if (method == null)
            {
                throw new ArgumentException("no found method :" + method);
            }

            method.Invoke(this, null);
        }
    }
}
