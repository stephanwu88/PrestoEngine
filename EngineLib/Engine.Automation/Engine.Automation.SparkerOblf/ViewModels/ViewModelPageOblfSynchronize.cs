using Engine.Automation.Sparker;
using Engine.Common;
using Engine.MVVM;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace Engine.Automation.OBLF
{
    /// <summary>
    /// Oblf数据同步
    /// </summary>
    public class ViewModelPageOblfSynchronize : ViewFrameBase
    {
        /// <summary>
        /// 仪器名称
        /// </summary>
        private string InsName { get; set; }
        private SparkHelperOblf _SparkHelper;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="insName"></param>
        public ViewModelPageOblfSynchronize(string insName)
        {
            if (!IsDesignMode)
            {
                InsName = insName;
                _SparkHelper = SparkHelper.Factory.DictFieldValue(InsName) as SparkHelperOblf;
                MenuCommandUpdateAnaPgm.Execute(null);
                MenuCommandUpdateProbenView.Execute(null);
                MenuCommandUpdateMaterialView.Execute(null);
            }
            ExpanderID = "1";
        }

        public string ExpanderID { get => _ExpanderID; set => RaisePropertyChanged(ref _ExpanderID, value); }

        private string _ExpanderID = "1";
        /// <summary>
        /// 仪器程序实体列表
        /// </summary>
        public List<ModelLocalSpecPgm> LstSpecPgm
        {
            get { return _LstSpecPgm; }
            set { _LstSpecPgm = value; RaisePropertyChanged(); }
        }
        private List<ModelLocalSpecPgm> _LstSpecPgm = new List<ModelLocalSpecPgm>();

        /// <summary>
        /// 控样列表视图
        /// </summary>
        public List<ModelLocalProbenMain> LstProbenMain
        {
            get { return _LstProbenMain; }
            set { _LstProbenMain = value; RaisePropertyChanged(); }
        }
        private List<ModelLocalProbenMain> _LstProbenMain = new List<ModelLocalProbenMain>();

        /// <summary>
        /// 控样对应的元素标定视图
        /// </summary>
        public List<ModelLocalProbenElem> LstProbenElem
        {
            get { return _LstProbenElem; }
            set { _LstProbenElem = value; RaisePropertyChanged(); }
        }
        private List<ModelLocalProbenElem> _LstProbenElem = new List<ModelLocalProbenElem>();

        /// <summary>
        /// 材质牌号列表视图
        /// </summary>
        public List<ModelLocalMaterialMain> LstMaterialMain
        {
            get { return _LstMaterialMain; }
            set { _LstMaterialMain = value; RaisePropertyChanged(); }
        }
        private List<ModelLocalMaterialMain> _LstMaterialMain = new List<ModelLocalMaterialMain>();

        /// <summary>
        /// 材质牌号关联元素列表视图
        /// </summary>
        public List<ModelLocalMaterialElem> LstMaterialElem
        {
            get { return _LstMaterialElem; }
            set { _LstMaterialElem = value; RaisePropertyChanged(); }
        }
        private List<ModelLocalMaterialElem> _LstMaterialElem = new List<ModelLocalMaterialElem>();

        /// <summary>
        /// 同步分析程序
        /// </summary>
        public ICommand MenuCommandSychronizeAnaPgm
        {
            get => new MyCommand((parameter) =>
            {
                MessageBoxResult ret = sCommon.MyMsgBox("该操作将OBLF仪器分析程序设置同步到本地,请确认?", MsgType.Question);
                if (ret == MessageBoxResult.No) return;
                string ErrorMessage = _SparkHelper.SynchronizeProgram();
                if (!string.IsNullOrEmpty(ErrorMessage))
                {
                    sCommon.MyMsgBox("同步发生错误! \r\n" + ErrorMessage, MsgType.Error);
                    return;
                }
                MenuCommandUpdateAnaPgm.Execute(null);
            });
        }

        /// <summary>
        /// 刷新本地仪器分析程序，通知到页面
        /// 从本地数据库获取同步后的仪器分析程序列表
        /// </summary>
        public ICommand MenuCommandUpdateAnaPgm
        {
            get => new MyCommand((parameter) =>
            {
                LstSpecPgm = _SparkHelper.GetLocalAnaPgmList();
            });
        }


        /// <summary>
        /// 同步Oblf仪器样本数据-控样列表以及控样对应的元素标定
        /// </summary>
        public ICommand MenuCommandSychronizeProbe
        {
            get => new MyCommand((parameter) =>
            {
                MessageBoxResult ret = sCommon.MyMsgBox("该操作将OBLF仪器控样数据同步到本地,请确认?", MsgType.Question);
                if (ret == MessageBoxResult.No) return;
                CallResult result = _SparkHelper.SynchronizeProbenAndElem();
                if (result.Fail)
                {
                    sCommon.MyMsgBox("同步发生错误! \r\n" + result.Result.ToMyString(), MsgType.Error);
                    return;
                }
                MenuCommandUpdateProbenView.Execute(null);
            });
        }

        /// <summary>
        /// 控样列表查询、更新视图
        /// </summary>
        public ICommand MenuCommandUpdateProbenView
        {
            get => new MyCommand((parameter) => {
                LstProbenMain = _SparkHelper.GetLocalProbenMain();
            });
        }

        /// <summary>
        /// 控样列表选择切换
        /// </summary>
        public ICommand ProbenListSelectionChanged
        {
            get => new MyCommand((selected) => {
                if (selected == null)
                    return;
                ModelLocalProbenMain mod = selected as ModelLocalProbenMain;
                MenuCommandUpdateProbenElemView.Execute(mod);
            });
        }

        /// <summary>
        /// 控样元素查询、更新视图
        /// </summary>
        public ICommand MenuCommandUpdateProbenElemView
        {
            get => new MyCommand((selectedProben) =>
            {

                if (selectedProben == null)
                    return;
                ModelLocalProbenMain proben = selectedProben as ModelLocalProbenMain;
                LstProbenElem = _SparkHelper.GetLocalProbenElem(proben.ProbenID);
            });
        }


        /// <summary>
        /// 同步牌号
        /// </summary>
        public ICommand MenuCommandSychronizeMaterial
        {
            get => new MyCommand((parameter) =>
            {
                MessageBoxResult ret = sCommon.MyMsgBox("该操作将OBLF仪器牌号数据同步到本地,请确认?", MsgType.Question);
                if (ret == MessageBoxResult.No) return;
                CallResult result = _SparkHelper.SynchronizeMaterial();
                if (result.Fail)
                {
                    sCommon.MyMsgBox("同步发生错误! \r\n" + result.Result.ToMyString(), MsgType.Error);
                    return;
                }
                MenuCommandUpdateMaterialView.Execute(null);
            });
        }

        /// <summary>
        /// 更新材质牌号列表视图
        /// </summary>
        public ICommand MenuCommandUpdateMaterialView
        {
            get => new MyCommand((parameter) => {
                LstMaterialMain = _SparkHelper.GetLocalMaterialMain();
            });
        }

        /// <summary>
        /// 材质牌号列表选择切换
        /// </summary>
        public ICommand MaterialMainSelectionChanged
        {
            get => new MyCommand((selected) => {
                if (selected == null)
                    return;
                ModelLocalMaterialMain mod = selected as ModelLocalMaterialMain;
                MenuCommandUpdateMaterialElemView.Execute(mod);
            });
        }

        /// <summary>
        /// 更新材质牌号列表视图
        /// </summary>
        public ICommand MenuCommandUpdateMaterialElemView
        {
            get => new MyCommand((selectedMaterial) => {
                if (selectedMaterial == null) return;
                ModelLocalMaterialMain main = selectedMaterial as ModelLocalMaterialMain;
                LstMaterialElem = _SparkHelper.GetLocalMaterialElem(main.Material);
            });
        }
    }
}
