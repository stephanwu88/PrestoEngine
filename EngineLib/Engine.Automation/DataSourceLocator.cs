using Engine.Common;
using Engine.WpfBase;

namespace Engine.Automation.Sparker
{
    class DataSourceLocator
    {
        public DataSourceLocator()
        {
            //ServiceRegistry.Instance.Register<ViewModelAnaPgm>(()=> GetAnaPgmViewModel());
            ServiceRegistry.Instance.Register<ViewModelAnaPgm>();
            ServiceRegistry.Instance.Register<ViewModelElemBase>();
            ServiceRegistry.Instance.Register<ViewModelRelatedSteel>();
        }

        /// <summary>
        /// 分析方法管理页
        /// </summary>
        public ViewModelAnaPgm AnaPgmViewModel => ServiceRegistry.Instance.GetInstance<ViewModelAnaPgm>();

        ///// <summary>
        ///// 分析方法管理页
        ///// </summary>
        //public ViewModelAnaPgm GetAnaPgmViewModel()
        //{
        //    string InsName = SparkHelper.Current?.BaseIns.InsName;
        //    ViewModelAnaPgm vm = new ViewModelAnaPgm(InsName);
        //    return vm;
        //}

        /// <summary>
        /// 分析方法 元素拓展
        /// </summary>
        public ViewModelElemBase ElemBaseViewModel
        {
            get
            {
                string key = AnaPgmViewModel.SelectedAnaPgm.Token;
                _ElemBaseViewModel = ServiceRegistry.Instance.GetInstance<ViewModelElemBase>(key);

                if (AnaPgmViewModel != null && _ElemBaseViewModel != null)
                    _ElemBaseViewModel.AnaPgm = AnaPgmViewModel.SelectedAnaPgm;

                return _ElemBaseViewModel;
            }
            private set
            {
                _ElemBaseViewModel = value;
            }
        }
        private ViewModelElemBase _ElemBaseViewModel;

        /// <summary>
        /// 分析方法 仪器分析元素库
        /// </summary>
        public ViewModelElemBase InsElemBaseLibViewModel
        {
            get
            {
                string key = SparkHelper.Current?.BaseIns.InsName;
                if (key.IsEmpty()) return null;
                _InsElemBaseLibViewModel = ServiceRegistry.Instance.GetInstance<ViewModelElemBase>(key);

                if (_InsElemBaseLibViewModel != null)
                    _InsElemBaseLibViewModel.AnaPgm = new ModelSpecPgm() { Token = key };

                return _InsElemBaseLibViewModel;
            }
            private set
            {
                _InsElemBaseLibViewModel = value;
            }
        }
        private ViewModelElemBase _InsElemBaseLibViewModel;

        /// <summary>
        /// 钢种关联分析曲线-牌号
        /// </summary>
        public ViewModelRelatedSteel RelatedSteelViewModel => ServiceRegistry.Instance.GetInstance<ViewModelRelatedSteel>();
    }
}
