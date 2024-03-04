using Engine.Common;
using Engine.Data.DBFAC;
using Engine.MVVM;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Engine.Automation.Sparker
{
    /// <summary>
    /// 控样管理
    /// </summary>
    public partial class ViewModelPageProben : ViewFrameBase
    {
        private List<string> LstPgmTextAwait = new List<string>();
        public Window Owner;
        private SparkHelper _SparkHelper;
        public ViewModelPageProben()
        {
            if (IsDesignMode)
                return;
            _SparkHelper = SparkHelper.Current;
            LoadDefaultSet();
            CommandUpdateProbenMainView.Execute(null);

        }

        #region 页面要素及窗口管理

        /// <summary>
        /// 加载页面配置
        /// </summary>
        private void LoadDefaultSet()
        {
            LstAnaPgm = _SparkHelper.GetAnaPgmList();
            if (LstAnaPgm.Count > 0) AnaPgm = LstAnaPgm[0];
        }

        /// <summary>
        /// 分析程序列表
        /// </summary>
        public List<ModelSpecPgm> LstAnaPgm
        {
            get { return _LstAnaPgm; }
            set { _LstAnaPgm = value; RaisePropertyChanged(); }
        }
        private List<ModelSpecPgm> _LstAnaPgm = new List<ModelSpecPgm>();

        /// <summary>
        /// 分析程序实体
        /// </summary>
        public ModelSpecPgm AnaPgm
        {
            get { return _AnaPgm; }
            set
            {
                _AnaPgm = value; RaisePropertyChanged();
                _SparkHelper.AnaPgm = _AnaPgm;
            }
        }
        private ModelSpecPgm _AnaPgm = new ModelSpecPgm();

        #endregion

        /// <summary>
        /// 控样主信息列表
        /// </summary>
        public List<ModelProbenDetail> LstProbenMain
        {
            get { return _LstProbenMain; }
            set { _LstProbenMain = value; RaisePropertyChanged(); }
        }
        private List<ModelProbenDetail> _LstProbenMain = new List<ModelProbenDetail>();

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
        /// 已选列表
        /// </summary>
        public List<ModelProbenElem> LstSelectedProbenElem
        {
            get { return _LstSelectedProbenElem; }
            set { _LstSelectedProbenElem = value; RaisePropertyChanged(); }
        }
        private List<ModelProbenElem> _LstSelectedProbenElem = new List<ModelProbenElem>();

        /// <summary>
        /// 关联牌号列表
        /// </summary>
        public List<ModelMaterialMain> LstRelatedMaterial
        {
            get { return _LstRelatedMaterial; }
            set { _LstRelatedMaterial = value; RaisePropertyChanged(); }
        }
        private List<ModelMaterialMain> _LstRelatedMaterial = new List<ModelMaterialMain>();
        
        /// <summary>
        /// 控样选择项
        /// </summary>
        public ModelProbenDetail SelectedProbenMain
        {
            get { return _SelectedProbenMain; }
            set { _SelectedProbenMain = value; RaisePropertyChanged(); }
        }
        private ModelProbenDetail _SelectedProbenMain = new ModelProbenDetail();

        /// <summary>
        /// 添加控样名称
        /// </summary>
        public ICommand CommandAddProbenMain
        {
            get => new MyCommand((dataGrid) =>
            {
                ViewModelConfigProbenMain ViewModel = new ViewModelConfigProbenMain(EditMode.AddNew);
                sCommon.OpenWindow<winConfigProbenMain>(ViewModel, Owner);
                ViewModel.ActionCommit += (s, e) =>
                {
                    ModelProbenDetail full = e as ModelProbenDetail;
                    CallResult result = _SparkHelper.AddProbenMainFull(full);
                    if (result.Fail)
                    {
                        sCommon.MyMsgBox("添加发生错误！\r\n" + result.Result.ToMyString(), MsgType.Error);
                        return;
                    }
                    CommandUpdateProbenMainView.Execute(null);
                    SelectedProbenMain = LstProbenMain?.MySelectFirst(x => x.Name == full.Name);
                    if (dataGrid is DataGrid dg && SelectedProbenMain != null)
                    {
                        dg.ScrollIntoView(SelectedProbenMain);
                    }
                };
            });
        }

        /// <summary>
        /// 修改控样名称
        /// </summary>
        public ICommand CommandEditProbenMain
        {
            get => new MyCommand((o) =>
            {
                if (SelectedProbenMain == null)
                    return;
                ViewModelConfigProbenMain ViewModel = new ViewModelConfigProbenMain(EditMode.Modify);
                sCommon.OpenWindow<winConfigProbenMain>(ViewModel, Owner);
                ViewModel.ProbenFull = SelectedProbenMain.ModelClone();
                ViewModel.ActionCommit += (s, e) =>
                {
                    ModelProbenDetail full = e as ModelProbenDetail;
                    CallResult result = _SparkHelper.UpdateProbenFullMain(full);
                    if (result.Fail)
                    {
                        sCommon.MyMsgBox("修改发生错误！\r\n" + result.Result.ToMyString(), MsgType.Error);
                        return;
                    }
                    CommandUpdateProbenMainView.Execute(null);
                };
            });
        }

        /// <summary>
        /// 删除控样
        /// </summary>
        public ICommand CommandDeleteProben
        {
            get => new MyCommand((LstSelectedProbenMain) =>
            {
                if (LstSelectedProbenMain == null)
                    return;
                List<ModelProbenDetail> LstProbenMain = LstSelectedProbenMain.ToMyList<ModelProbenDetail>();
                CallResult result = _SparkHelper.DeleteProben(LstProbenMain);
                if (result.Fail)
                {
                    sCommon.MyMsgBox("删除控样发生错误！\r\n" + result.Result.ToMyString(), MsgType.Error);
                    return;
                }
                CommandUpdateProbenMainView.Execute(null);
            });
        }

        /// <summary>
        /// 导入控样
        /// </summary>
        public ICommand CommandImportProben
        {
            get => new MyCommand((param) =>
            {
                //Window win = typeof(winImportProben).OpenWindow(Owner);
                //if (win != null)
                {
                    //ViewModelProbenImport ViewModel = new ViewModelProbenImport(InsName);
                    //ViewModel.ActionCommit += (s, e) =>
                    //{
                    //    if (e == null) return;
                    //    //CallResult res = SparkHelper.Default.ImportProben(e as List<ModelLocalProbenMain>);
                    //    CallResult res = _SparkHelper.ImportProbenAddOrUpdate(e as List<ModelLocalProbenMain>);
                    //    if (res.Success)
                    //        CommandUpdateProbenMainView.Execute(null);
                    //    else
                    //        sCommon.MyMsgBox(res.Result.ToMyString(), MsgType.Error);
                    //};
                    //win.DataContext = ViewModel;
                }
            });
        }

        /// <summary>
        /// 添加分析元素
        /// </summary>
        public ICommand CommandAddProbenElement
        {
            get => new MyCommand((s) =>
            {
                LstUnSelectExtendElem = GetUnSelectElemList();
                if (LstUnSelectExtendElem.MyCount() == 0)
                {
                    sCommon.MyMsgBox("该分析方法下没有可用的拓展元素，请转至该分析方法下添加!", MsgType.Error);
                    return;
                }
                LstUnSelectExtendElem.Sort(new CustomCompare<ModelElemBase>());
                winAttachElement win = sCommon.OpenWindow<winAttachElement>(this, null, null);
            });
        }

        /// <summary>
        /// 保存元素修改值
        /// </summary>
        public ICommand CommandSaveElementUserEdit
        {
            get => new MyCommand(
              (param) =>
              {
                  if (LstProbenElem == null)
                      return;
                  CallResult res = SaveElementEdit();
                  CommandUpdateProbenElemView.Execute(SelectedProbenMain);
                  if (res.Fail)
                      sCommon.MyMsgBox($"保存遇到错误:\r\n{res.Result.ToMyString()}", MsgType.Error);
              },
              (param) =>
              {
                  if (LstProbenElem == null)
                      return false;
                  return LstProbenElem.MyContains(x => x.IsModified);
              });
        }

        /// <summary>
        /// 清除首次值
        /// </summary>
        public ICommand CommandClearStartValue
        {
            get => new MyCommand((param) =>
              {
                  if ((SelectedProbenMain?.ProbenToken).IsEmpty())
                  {
                      sCommon.MyMsgBox("请选择需要操作的目标控样", MsgType.Warning);
                      return;
                  }
                  MessageBoxResult res =  sCommon.MyMsgBox("建议在完全标准化后清除首次值，您是否确定需要清除首次参考值？", MsgType.Question);
                  if (res == MessageBoxResult.No)
                      return;
                  ModelProbenElem CmdClearStartValue = new ModelProbenElem
                  {
                      ProbenToken = SelectedProbenMain?.ProbenToken.MarkWhere(),
                      StartValue = SystemDefault.StringEmpty
                  };
                  CallResult result = DbFactory.Data?.ExcuteUpdate(CmdClearStartValue);
                  CommandUpdateProbenElemView.Execute(SelectedProbenMain);
                  if (result.Fail)
                      sCommon.MyMsgBox($"发生错误: {result.Result.ToMyString()}", MsgType.Error);
              });
        }
        /// <summary>
        /// 选中单元格变化
        /// </summary>
        public ICommand CommandElemSelectCellsChanged
        {
            get => new MyCommand((selected) =>
            {
                var selectedCells = selected as IList<DataGridCellInfo>;
                List<ModelProbenElem> LstSel = new List<ModelProbenElem>();
                foreach (DataGridCellInfo item in selectedCells)
                {
                    ModelProbenElem mod = item.Item as ModelProbenElem;
                    if (!LstSel.Contains(mod))
                        LstSel.Add(mod);
                }
                LstSelectedProbenElem = LstSel;
            });
        }

        /// <summary>
        /// 删除分析元素
        /// </summary>
        public ICommand CommandDeleteProbenElement
        {
            get => new MyCommand((LstSelectedElem) =>
            {
                if (LstSelectedElem == null)
                    return;
                List<ModelProbenElem> LstProbenElem = LstSelectedElem.ToMyList<ModelProbenElem>();
                CallResult result = _SparkHelper.DeleteProbenElement(LstProbenElem);
                if (result.Fail)
                {
                    sCommon.MyMsgBox("删除元素发生错误！\r\n" + result.Result.ToMyString(), MsgType.Error);
                    return;
                }
                CommandUpdateProbenElemView.Execute(SelectedProbenMain);
            });
        }

        /// <summary>
        /// 更新控样列表主信息栏
        /// </summary>
        public ICommand CommandUpdateProbenMainView
        {
            get => new MyCommand((e) =>
            {
                LstProbenMain = _SparkHelper.GetProbenDetail();
            });
        }

        /// <summary>
        /// 更新元素列表显示
        /// </summary>
        public ICommand CommandUpdateProbenElemView
        {
            get => new MyCommand((selectedProben) =>
            {
                if (selectedProben == null) return;
                if (selectedProben is ModelProbenDetail proben)
                {
                    LstProbenElem = _SparkHelper.GetProbenElement(proben.ProbenToken);
                }
            });
        }

        /// <summary>
        /// 更新关联牌号显示
        /// </summary>
        public ICommand CommandUpdateRelatedMaterialView
        {
            get => new MyCommand((selectedProben) =>
            {
                if (selectedProben == null)
                    return;
                ModelProbenDetail proben = selectedProben as ModelProbenDetail;
                LstRelatedMaterial = _SparkHelper.GetRelatedMaterial(proben.Name.ToMyString());
            });
        }

        /// <summary>
        /// 分析程序切换
        /// </summary>
        public ICommand AnaPgmSelectionChanged
        {
            get => new MyCommand((SelectedAnaPgm) =>
            {
                if (SelectedAnaPgm == null) return;
                AnaPgm = SelectedAnaPgm as ModelSpecPgm;
                LstProbenElem = _SparkHelper.GetProbenElement(SelectedProbenMain.ProbenToken);
            });
        }

        /// <summary>
        /// 控样主列表信息切换
        /// </summary>
        public ICommand LstProbenMainSelectionChanged
        {
            get => new MyCommand((selectedProbenMain) =>
            {
                CommandUpdateProbenElemView.Execute(selectedProbenMain);
                CommandUpdateRelatedMaterialView.Execute(selectedProbenMain);
            });
        }

        /// <summary>
        /// 单元格回车
        /// </summary>
        public ICommand EnterCommand
        {
            get => new MyCommand((SelectedProbenElem) =>
            {
                ModelProbenElem elem = SelectedProbenElem as ModelProbenElem;
            });
        }

        /// <summary>
        /// 保存到元素编辑
        /// </summary>
        /// <returns></returns>
        private CallResult SaveElementEdit()
        {
            CallResult result = new CallResult();
            if (LstProbenElem == null)
            {
                result.Success = true;
                return result;
            }
            IEnumerable<ModelProbenElem> lst = LstProbenElem.MySelect(x => x.IsModified == true);
            List<ModelProbenElem> LstSaveChange = new List<ModelProbenElem>();
            foreach (ModelProbenElem item in lst)
            {
                LstSaveChange.Add(new ModelProbenElem()
                {
                    ProbenToken = item.ProbenToken.MarkWhere(),
                    Element = item.Element.MarkWhere(),
                    SetPoint = item.SetPoint.ValueAttachMark(),
                    StartValue = item.StartValue.ValueAttachMark(),
                    TolStart = item.TolStart.ValueAttachMark(),
                    ActualValue = item.ActualValue.ValueAttachMark(),
                    TolActual = item.TolActual.ValueAttachMark(),
                    AdditiveValue = item.AdditiveValue.ValueAttachMark(),
                    MultiplitiveValue = item.MultiplitiveValue.ValueAttachMark(),
                });
            }
            result = DbFactory.Data?.ExcuteUpdate(LstSaveChange);
            return result;
        }


    }

    /// <summary>
    /// 控样添加元素页面交互
    /// </summary>
    public partial class ViewModelPageProben
    {
        /// <summary>
        /// 待选元素 - 从分析方法拓展元素获取
        /// </summary>
        public List<ModelElemBase> LstUnSelectExtendElem
        {
            get { return _LstUnSelectExtendElem; }
            set { _LstUnSelectExtendElem = value; }
        }
        private List<ModelElemBase> _LstUnSelectExtendElem = new List<ModelElemBase>();

        /// <summary>
        /// 获取待选
        /// </summary>
        /// <returns></returns>
        private List<ModelElemBase> GetUnSelectElemList()
        {
            List<ModelElemBase> LstUnSelect = new List<ModelElemBase>();
            List<ModelElemBase> LstAll = _SparkHelper.GetAnaPgmExtendElemList(AnaPgm);
            foreach (ModelElemBase item in LstAll)
            {
                if (!string.IsNullOrEmpty(item.Element) &&
                    !LstProbenElem.MyContains(x => x.Element == item.Element.ToMyString()))
                {
                    LstUnSelect.Add(item);
                }
            }
            return LstUnSelect;
        }

        /// <summary>
        /// 全选
        /// </summary>
        public ICommand CommandSelectAll
        {
            get => new MyCommand((param) =>
            {
                if (LstUnSelectExtendElem == null)
                    return;
                foreach (var item in LstUnSelectExtendElem)
                {
                    item.IsChecked = true;
                }
            });
        }

        /// <summary>
        /// 全不选
        /// </summary>
        public ICommand CommandUnSelectAll
        {
            get => new MyCommand((param) =>
            {
                if (LstUnSelectExtendElem == null)
                    return;
                foreach (var item in LstUnSelectExtendElem)
                {
                    item.IsChecked = false;
                }
            });
        }

        /// <summary>
        /// 确认添加控样元素
        /// </summary>
        public ICommand CommandSureAttachElement
        {
            get => new MyCommand((ObjectWindow) =>
            {
                if (string.IsNullOrEmpty(SelectedProbenMain?.MainToken) ||
                string.IsNullOrEmpty(AnaPgm?.Token))
                {
                    sCommon.MyMsgBox("未指定控样或分析曲线", MsgType.Error);
                    return;
                }
                IEnumerable<ModelElemBase> lstSelect = LstUnSelectExtendElem.MySelect(x => x.IsChecked == true);
                if (lstSelect.MyCount() == 0)
                    return;
                List<ModelProbenElem> LstProbenElem = new List<ModelProbenElem>();
                foreach (ModelElemBase eBase in lstSelect)
                {
                    if (string.IsNullOrWhiteSpace(eBase.Element))
                        continue;
                    ModelProbenElem ele = new ModelProbenElem()
                    {
                        MainToken = SelectedProbenMain.MainToken,
                        ProbenToken = SelectedProbenMain.ProbenToken,
                        Element = eBase.Element
                    };
                    LstProbenElem.Add(ele);
                }
                CallResult result = _SparkHelper.AddProbenElement(LstProbenElem);
                if (result.Fail)
                {
                    sCommon.MyMsgBox("发生错误！\r\n" + result.Result.ToMyString(), MsgType.Error);
                    return;
                }
                CommandUpdateProbenElemView.Execute(SelectedProbenMain);

                //关闭操作窗口
                if (ObjectWindow != null)
                    (ObjectWindow as Window)?.Close();
            });
        }
    }
}
