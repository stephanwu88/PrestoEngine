using Engine.MVVM;
using System.Collections.Generic;
using System.Windows.Input;

namespace Engine.Automation.Sparker
{
    /// <summary>
    /// 控样样品源管理 - 管理系统用到的控样及国家标准
    /// </summary>
    public class ViewModelProbenStd : ViewFrameBase
    {
        private SparkHelper _SparkHelper;
        public ViewModelProbenStd()
        {
            if (IsDesignMode)
                return;
            _SparkHelper = SparkHelper.Current;
            CommandUpdateProbenMainView.Execute(null);
        }

        /// <summary>
        /// 控样主信息列表
        /// </summary>
        public List<ModelProbenMain> LstProbenMain
        {
            get { return _LstProbenMain; }
            set { _LstProbenMain = value; RaisePropertyChanged(); }
        }
        private List<ModelProbenMain> _LstProbenMain = new List<ModelProbenMain>();


        /// <summary>
        /// 控样元素列表
        /// </summary>
        public List<ModelProbenElem> LstProbenElem
        {
            get { return _LstProbenElem; }
            set { _LstProbenElem = value; RaisePropertyChanged(); }
        }
        private List<ModelProbenElem> _LstProbenElem;

        /// <summary>
        /// 添加控样名称
        /// </summary>
        public ICommand CommandAddProbenMain
        {
            get => new MyCommand((param) =>
            {

            });
        }

        /// <summary>
        /// 修改控样名称
        /// </summary>
        public ICommand CommandEditProbenMain
        {
            get => new MyCommand((param) =>
            {

            });
        }

        /// <summary>
        /// 删除控样
        /// </summary>
        public ICommand CommandDeleteProben
        {
            get => new MyCommand((param) =>
            {

            });
        }

        /// <summary>
        /// 导入控样标准值
        /// </summary>
        public ICommand CommandImportProben
        {
            get => new MyCommand((param) =>
            {
                
            });
        }

        /// <summary>
        /// 更新控样列表主信息栏
        /// </summary>
        public ICommand CommandUpdateProbenMainView
        {
            get => new MyCommand((e) =>
            {
                LstProbenMain = _SparkHelper.GetGloableProbenMain();
            });
        }

        /// <summary>
        /// 更新元素列表显示
        /// </summary>
        public ICommand CommandUpdateProbenElemView
        {
            get => new MyCommand((probenMain) =>
            {
                if (probenMain == null) return;
                ModelProbenMain main = probenMain as ModelProbenMain;
                LstProbenElem = _SparkHelper.GetProbenElementStd(main.Name);
            });
        }

        public ICommand LstProbenMainSelectionChanged
        {
            get => new MyCommand((d) =>
            {
                if (d == null) return;
                //ModelProbenMain main = d as ModelProbenMain;
                CommandUpdateProbenElemView.Execute(d);
            });
        }
    }
}
