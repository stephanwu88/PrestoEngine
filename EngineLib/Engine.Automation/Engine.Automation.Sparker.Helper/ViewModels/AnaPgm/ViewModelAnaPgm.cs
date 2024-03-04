using Engine.Common;
using Engine.MVVM;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace Engine.Automation.Sparker
{
    /// <summary>
    /// 仪器分析程序配置
    /// </summary>
    public class ViewModelAnaPgm : ViewFrameBase
    {
        private SparkHelper _SparkHelper;
        public ViewModelAnaPgm()
        {
            if (IsDesignMode)
                return;
            _SparkHelper = SparkHelper.Current;
            CommandUpdateAnaPgmView.Execute(null);
        }

        /// <summary>
        /// 分析程序列表实体
        /// </summary>
        public List<ModelSpecPgm> AnaPgmListView
        {
            get { return _AnaPgmListView; }
            set { _AnaPgmListView = value; RaisePropertyChanged(); }
        }
        private List<ModelSpecPgm> _AnaPgmListView = new List<ModelSpecPgm>();

        /// <summary>
        /// 分析程序选中项
        /// </summary>
        public ModelSpecPgm SelectedAnaPgm
        {
            get { return _SelectedAnaPgm; }
            set { _SelectedAnaPgm = value; RaisePropertyChanged(); }
        }
        private ModelSpecPgm _SelectedAnaPgm = null;
        

        /// <summary>
        /// 添加分析程序
        /// </summary>
        public ICommand CommandPgmAdd
        {
            get => new MyCommand((parameter) =>
            {
                if (typeof(winCreatePgm).IsWindowOpened())
                    return;
                ViewModelConfigPgm ViewModel = new ViewModelConfigPgm(EditMode.AddNew);
                ViewModel.ActionCommit += (s, e) =>
                {
                    if (e == null) return;
                    CallResult res = _SparkHelper.AddAnaPgm(e as ModelSpecPgm);
                    if (res.Success)
                    {
                        CommandUpdateAnaPgmView.Execute(null);
                    }
                    else
                        sCommon.MyMsgBox(res.Result.ToMyString(), MsgType.Error);

                };
                ViewModel.AnaPgm = new ModelSpecPgm() { InsName = _SparkHelper?.BaseIns.InsName };
                winCreatePgm win = sCommon.OpenWindow<winCreatePgm>(ViewModel);
            });
        }

        /// <summary>
        /// 编辑分析程序
        /// </summary>
        public ICommand CommandPgmEdit
        {
            get => new MyCommand((selectedPgm) =>
            {
                if (selectedPgm == null)
                {
                    sCommon.MyMsgBox("请选择编辑项！", MsgType.Warning);
                    return;
                }
                Window win = typeof(winCreatePgm).OpenWindow();
                if (win == null) return;
                ViewModelConfigPgm ViewModel = new ViewModelConfigPgm(EditMode.Modify)
                {
                    AnaPgm = selectedPgm as ModelSpecPgm
                };
                ViewModel.ActionCommit += (s, e) =>
                {
                    if (e == null) return;
                    CallResult res = _SparkHelper.UpdateAnaPgm(e as ModelSpecPgm);
                    if (res.Fail)
                        sCommon.MyMsgBox(res.Result.ToMyString(), MsgType.Error);
                    CommandUpdateAnaPgmView.Execute(null);
                };
                win.DataContext = ViewModel;
            });
        }

        /// <summary>
        /// 配置拓展元素
        /// </summary>
        public ICommand CommandExtendElemManage
        {
            get => new MyCommand((selectedPgm)=>
            {
                if (selectedPgm == null)
                {
                    sCommon.MyMsgBox("请选择需要配置的分析曲线！", MsgType.Warning);
                    return;
                }
                PageNavigater.Default.GetFrame("PageHomeFrame")?.NavigateToInstancePage(new PageAnaElemBase());   
            });
        }

        /// <summary>
        /// 分析方法偏差管理
        /// </summary>
        public ICommand CommandDeviationManage
        {
            get => new MyCommand((d) =>
            {

            });
        }

        /// <summary>
        /// 导入分析程序
        /// </summary>
        public ICommand CommandPgmImport
        {
            get => new MyCommand((parameter) =>
            {

            });
        }

        /// <summary>
        /// 导出分析程序
        /// </summary>
        public ICommand CommandPgmExport
        {
            get => new MyCommand((parameter) =>
            {

            });
        }

        /// <summary>
        /// 删除分析程序
        /// </summary>
        public ICommand CommandPgmDelete
        {
            get => new MyCommand((SelectedPgmList) =>
            {
                List<ModelSpecPgm> LstPgm = SelectedPgmList.ToMyList<ModelSpecPgm>();
                if (LstPgm.MyCount() == 0)
                {
                    sCommon.MyMsgBox("请选择需要删除项！", MsgType.Warning);
                    return;
                }
                string Message = "删除分析程序后,系统将会自动关联以下操作：\r\n\r\n";
                Message += $"1. 删除与该分析程序的关联牌号\r\n";
                Message += $"2. 解绑与该分析程序的关联控样\r\n\r\n";
                Message += "您是否确定需要删除当前选中的项?";
                MessageBoxResult ret = sCommon.MyMsgBox(Message, MsgType.Question);
                if (!ret.Equals(MessageBoxResult.Yes))
                    return;
                CallResult res = _SparkHelper?.DeleteAnaPgm(LstPgm);
                if (res.Success)
                    CommandUpdateAnaPgmView.Execute(null);
                else
                    sCommon.MyMsgBox(res.Result.ToMyString(), MsgType.Error);
            });
        }

        /// <summary>
        /// 查询分析程序 - 刷新
        /// </summary>
        public ICommand CommandUpdateAnaPgmView
        {
            get => new MyCommand((parameter) =>
            {
                AnaPgmListView = _SparkHelper.GetAnaPgmList();
            });
        }
    }
}
