﻿using Engine.Data;
using Engine.MVVM;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;

namespace Engine.WpfBase
{

    /// <summary> Mvc模式所有ViewModel的基类 </summary>
    public class MvcViewModelBase : NotifyObject, IMvcViewModelBase
    {
        private ILinkActionBase _selectLink;
        /// <summary> 说明  </summary>
        public ILinkActionBase SelectLink
        {
            get { return _selectLink; }
            set
            {
                _selectLink = value;
                RaisePropertyChanged("SelectLink");
            }
        }

        /// <summary> 跳转到Link页面 </summary>
        public MyCommand<ILinkActionBase> GoToLinkCommand => new Lazy<MyCommand<ILinkActionBase>>(() =>
                new MyCommand<ILinkActionBase>(GoToLink, CanGoToLink)).Value;

        [MethodImpl(MethodImplOptions.Synchronized)]
        private void GoToLink(ILinkActionBase args)
        {
            args.Controller = args.Controller ?? this.GetController();

            this.SelectLink = args;
        }

        private bool CanGoToLink(ILinkActionBase args)
        {
            return true;
        }

        /// <summary> 跳转到Action页面 </summary>
        public MyCommand<string> GoToActionCommand => new Lazy<MyCommand<string>>(() => new MyCommand<string>(GoToAction, CanGoToAction)).Value;

        private void GoToAction(string args)
        {
            this.GoToLink(args);
        }

        private bool CanGoToAction(string args)
        {
            return true;
        }

        /// <summary> 执行异步操作操作 </summary>
        public MyCommand<string> DoActionCommand => new Lazy<MyCommand<string>>(() => new MyCommand<string>(DoAction, CanDoAction)).Value;

        private async void DoAction(string args)
        {
            string controller = this.GetController();

            string action = args;

            await Task.Run(() =>
              {
                  var linkAction = new LinkActionBase() { Controller = controller, Action = action };

                  return linkAction.DoActionResult();
              });
        }

        private bool CanDoAction(string args)
        {
            return true;
        }


        [MethodImpl(MethodImplOptions.Synchronized)]
        protected void GoToLink(string controller, string action)
        {
            ILinkActionBase link = new LinkActionBase();
            link.Controller = controller;
            link.Action = action;
            this.SelectLink = link;
        }

        /// <summary> 获取控制器名称 </summary>
        public string GetController()
        {
            var results = this.GetType().GetCustomAttributes(typeof(ViewModelAttribute), true);

            return (results?.FirstOrDefault() as ViewModelAttribute)?.Name;
        }

        /// <summary> 获取当前LinkAction </summary>
        public ILinkActionBase GetLinkAction(string action)
        {
            ILinkActionBase link = new LinkActionBase();
            link.Controller = this.GetController();
            link.Action = action;
            link.DisplayName = action;
            return link;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        protected void GoToLink(string action)
        {
            this.SelectLink = GetLinkAction(action);
        }

        public MyCommand<string> LoadedCommand => new Lazy<MyCommand<string>>(() => new MyCommand<string>(Loaded, CanLoaded)).Value;

        protected virtual void Loaded(string args)
        {
            if (!string.IsNullOrEmpty(args))
            {
                this.GoToLink(args);
            }
        }

        protected virtual bool CanLoaded(string args)
        {
            return true;
        }

        private ObservableCollection<ILinkActionBase> _navigation = new ObservableCollection<ILinkActionBase>();
        /// <summary> 导航索引  </summary>
        public ObservableCollection<ILinkActionBase> Navigation
        {
            get { return _navigation; }
            set
            {
                _navigation = value;
                RaisePropertyChanged("Navigation");
            }
        }

        /// <summary> 异步运行 </summary>
        public void RunAsync(Action action)
        {
            Task.Run(action);
        }

        /// <summary> 使用主线程运行 </summary>
        public void Invoke(Action action)
        {
            Application.Current.Dispatcher.Invoke(action);
        }

        ///// <summary> 验证实体模型是否可用 </summary>
        //protected bool ModelState(object obj, out string message)
        //{
        //    var result = ObjectPropertyFactory.ModelState(obj, out List<string> errors);

        //    message = errors.FirstOrDefault();

        //    return result;
        //}
    }

    /// <summary> 带有封装好集合实体的基类 </summary>
    public class MvcEntityViewModelBase<M> : MvcViewModelBase, IMvcEntityViewModelBase<M> where M : new()
    {
        private ObservableCollection<M> _collection = new ObservableCollection<M>();
        /// <summary> 实体集合  </summary>
        public ObservableCollection<M> Collection
        {
            get { return _collection; }
            set
            {
                _collection = value;
                RaisePropertyChanged("Collection");
            }
        }

        private M _addItem = new M();
        /// <summary> 要添加的实体  </summary>
        public M AddItem
        {
            get { return _addItem; }
            set
            {
                _addItem = value;
                RaisePropertyChanged("AddItem");
            }
        }

        private M _selectedItem;
        /// <summary> 选中的实体  </summary>
        public M SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                RaisePropertyChanged("SelectedItem");
            }
        }
    }

    /// <summary> 带有依赖注入Respository的基类 </summary>
    public class MvcViewModelBase<R, M> : MvcEntityViewModelBase<M> where M : new()
    {
        public R Respository { get; set; } = ServiceRegistry.Instance.GetInstance<R>();
    }

    /// <summary> 带有依赖注入Respository的基类 </summary>
    public class MvcViewModelBase<R1, R2, M> : MvcViewModelBase<R1, M> where M : new()
    {
        public R2 Service1 { get; set; } = ServiceRegistry.Instance.GetInstance<R2>();
    }

    /// <summary> 带有依赖注入Respository的基类 </summary>
    public class MvcViewModelBase<R1, R2, R3, M> : MvcViewModelBase<R1, R2, M> where M : new()
    {
        public R3 Service2 { get; set; } = ServiceRegistry.Instance.GetInstance<R3>();
    }

    /// <summary> 带有依赖注入Respository的基类 </summary>
    public class MvcViewModelBase<R1, R2, R3, R4, M> : MvcViewModelBase<R1, R2, R3, M> where M : new()
    {
        public R4 Service3 { get; set; } = ServiceRegistry.Instance.GetInstance<R4>();
    }

    /// <summary> 带有依赖注入Respository的基类 </summary>
    public class MvcViewModelBase<R1, R2, R3, R4, R5, M> : MvcViewModelBase<R1, R2, R3, R4, M> where M : new()
    {
        public R5 Service4 { get; set; } = ServiceRegistry.Instance.GetInstance<R5>();
    }
}
