using Engine.Common;
using Engine.Data.DBFAC;
using Engine.MVVM;
using Engine.MVVM.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Engine.Automation.Sparker
{
    /// <summary>
    /// 牌号管理页
    /// </summary>
    public partial class ViewModelMaterial : ViewFrameBase
    {
        public PageMaterial Page;
        private SparkHelper _SparkHelper;
        public ViewModelMaterial()
        {
            if (IsDesignMode)
                return;
            _SparkHelper = SparkHelper.Current;
            //加载牌号列表
            CommandUpdateMaterialMain.Execute(null);
            //加载分析程序切片器
            CommandUpdateAnaPgmList.Execute(null);
            //初始选中
            if (LstAnaPgm.Count > 0) AnaPgm = LstAnaPgm[0];
            SelectedPage = "Page1";
            PropertyChanged += (sender,args) =>
            {
                if (args.PropertyName == nameof(SelectedPage))
                {
                    CommandMaterialMainSelectionChanged.Execute(SelectedMaterialMain);
                }
            };
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
            set { _AnaPgm = value; RaisePropertyChanged(); _SparkHelper.AnaPgm = _AnaPgm; }
        }
        private ModelSpecPgm _AnaPgm = new ModelSpecPgm();

        /// <summary>
        /// 牌号列表
        /// </summary>
        public List<ModelMaterialMain> LstMaterialMain
        {
            get { return _LstMaterialMain; }
            set { _LstMaterialMain = value; RaisePropertyChanged(); }
        }
        private List<ModelMaterialMain> _LstMaterialMain = new List<ModelMaterialMain>();

        /// <summary>
        /// 牌号关联元素列表
        /// </summary>
        public List<ModelMaterialFull> LstMaterialElem
        {
            get { return _LstMaterialElem; }
            set { _LstMaterialElem = value; RaisePropertyChanged(); }
        }
        private List<ModelMaterialFull> _LstMaterialElem = new List<ModelMaterialFull>();

        /// <summary>
        /// 牌号选择项
        /// </summary>
        public ModelMaterialMain SelectedMaterialMain
        {
            get { return _SelectedMaterialMain; }
            set { _SelectedMaterialMain = value; RaisePropertyChanged(); }
        }
        private ModelMaterialMain _SelectedMaterialMain = new ModelMaterialMain();

        /// <summary>
        /// 牌号元素选择项
        /// </summary>
        public List<ModelMaterialElem> SelectedMaterialElems
        {
            get { return _SelectedMaterialElems; }
            set { _SelectedMaterialElems = value; RaisePropertyChanged(); }
        }
        private List<ModelMaterialElem> _SelectedMaterialElems = new List<ModelMaterialElem>();

        /// <summary>
        /// 页面显示 - 分析程序列表 
        /// </summary>
        public ICommand CommandUpdateAnaPgmList
        {
            get => new MyCommand((p) =>
            {
                LstAnaPgm = _SparkHelper.GetAnaPgmList();
            });
        }

        /// <summary>
        ///页面显示 - 牌号列表
        /// </summary>
        public ICommand CommandUpdateMaterialMain
        {
            get => new MyCommand((d) =>
            {
                LstMaterialMain = _SparkHelper.GetMaterialMain();
            });
        }

        /// <summary>
        /// 页面显示 - 更新显示牌号元素关联列表
        /// </summary>
        public ICommand CommandUpdateProbenElem
        {
            get => new MyCommand((d) =>
            {
                if (SelectedMaterialMain == null)
                    return;
                LstMaterialElem = _SparkHelper.GetMaterialElemByAnaPgm(SelectedMaterialMain.Material.ToMyString());
            });
        }

        /// <summary>
        /// 页面操作 - 添加牌号
        /// </summary>
        public ICommand CommandAddMaterial
        {
            get => new MyCommand((dataGrid) =>
            {
                if (this.AnaPgm == null)
                {
                    sCommon.MyMsgBox("请选择分析曲线!", MsgType.Warning);
                    return;
                }
                if (typeof(winCreateMaterial).IsWindowOpened())
                    return;
                ViewModelConfigMaterial ViewModelPage = new ViewModelConfigMaterial()
                {
                    editMode = EditMode.AddNew,
                    AnaPgm = this.AnaPgm,
                };
                winCreateMaterial winConfig = sCommon.OpenWindow<winCreateMaterial>(ViewModelPage);
                if (winConfig == null) return;
                ViewModelPage.ActionCommit += (sender, data) =>
                {
                    if (data == null) return;
                    ModelMaterialMain main = data as ModelMaterialMain;
                    CallResult res = _SparkHelper.AddMaterialMain(main);
                    if (res.Success)
                    {
                        CommandUpdateMaterialMain.Execute(null);
                        SelectedMaterialMain = LstMaterialMain.MySelectFirst(x => x.Material == main.Material);
                        if (dataGrid is DataGrid dg && SelectedMaterialMain != null)
                            dg.ScrollIntoView(SelectedMaterialMain);
                    }
                    else
                        sCommon.MyMsgBox(res.Result.ToMyString(), MsgType.Error);
                };
            });
        }

        /// <summary>
        /// 页面操作 - 编辑牌号
        /// </summary>
        public ICommand CommandEditMaterial
        {
            get => new MyCommand((selected) =>
            {
                if (selected == null)
                {
                    sCommon.MyMsgBox("请选择编辑项！", MsgType.Warning);
                    return;
                }
                if (typeof(winCreateMaterial).IsWindowOpened())
                    return;
                ViewModelConfigMaterial ViewModelPage = new ViewModelConfigMaterial()
                {
                    editMode = EditMode.Modify,
                    AnaPgm = this.AnaPgm,
                    MaterialMain = (selected as ModelMaterialMain).ModelClone()
                };
                winCreateMaterial winConfig = sCommon.OpenWindow<winCreateMaterial>(ViewModelPage);
                if (winConfig == null) return;
                ViewModelPage.ActionCommit += (sender, data) =>
                {
                    if (data == null) return;
                    CallResult res = _SparkHelper.UpdateMaterialMain(data as ModelMaterialMain);
                    if (res.Success)
                        CommandUpdateMaterialMain.Execute(null);
                    else
                        sCommon.MyMsgBox(res.Result.ToMyString(), MsgType.Error);
                };
            });
        }

        /// <summary>
        /// 页面操作 - 删除牌号
        /// </summary>
        public ICommand CommandDeleteMaterial
        {
            get => new MyCommand((LstSelectedMaterMain) =>
            {
                if (LstSelectedMaterMain == null)
                    return;
                MessageBoxResult ret = sCommon.MyMsgBox("您是否确认删除该牌号，删除后不能恢复？", MsgType.Question);
                if (ret == MessageBoxResult.No)
                    return;
                List<ModelMaterialMain> LstSel = LstSelectedMaterMain.ToMyList<ModelMaterialMain>();
                CallResult result = _SparkHelper.DeleteMaterial(LstSel);
                if (result.Fail)
                {
                    sCommon.MyMsgBox("发生错误！\r\n" + result.Result.ToMyString(), MsgType.Error);
                    return;
                }
                CommandUpdateMaterialMain.Execute(null);
            });
        }

        /// <summary>
        /// 牌号导入
        /// </summary>
        public ICommand CommandImportMaterial
        {
            get => new MyCommand((p) =>
            {
                //Window win = typeof(winImportMaterial).OpenWindow(Application.Current.MainWindow);
                //if (win != null)
                //{
                //    ViewModelMaterialImport ViewModel = new ViewModelMaterialImport(InsName);
                //    ViewModel.ActionCommit += (s, e) =>
                //    {
                //        if (e == null) return;
                //        CallResult res = _SparkHelper.ImportMaterialAddOrUpdate(e as List<ModelLocalMaterialMain>);
                //        if (res.Success)
                //            CommandUpdateMaterialMainView.Execute(null);
                //        else
                //            sCommon.MyMsgBox(res.Result.ToMyString(), MsgType.Error);
                //    };
                //    win.DataContext = ViewModel;
                //}
            });
        }

        /// <summary>
        /// 牌号选切换
        /// </summary>
        public ICommand CommandMaterialMainSelectionChanged
        {
            get => new MyCommand((selectedMatrialMain) =>
            {
                if (selectedMatrialMain == null)
                    return;
                ModelMaterialMain mMain = selectedMatrialMain as ModelMaterialMain;
                if (SelectedPage == "Page1")
                    LstMaterialElem = _SparkHelper.GetMaterialElemByAnaPgm(mMain.Material.ToMyString());
                else if (SelectedPage == "Page2")
                    CommandUpdateDeviationList.Execute(null);
            });
        }

        /// <summary>
        /// 页面操作 - 分析程序切换
        /// </summary>
        public ICommand AnaPgmSelectionChanged
        {
            get => new MyCommand((SelectedAnaPgm) =>
            {
                if (SelectedAnaPgm == null) return;
                AnaPgm = SelectedAnaPgm as ModelSpecPgm;
                LstMaterialElem = _SparkHelper.GetMaterialElemByAnaPgm(SelectedMaterialMain.Material.ToMyString());
            });
        }

        /// <summary>
        /// 牌号元素列表选中切换
        /// </summary>
        public ICommand CommandMaterialElemSelectChanged
        {
            get => new MyCommand((selected) =>
            {
                var selectedCells = selected as IList<DataGridCellInfo>;
                List<ModelMaterialFull> LstSel = new List<ModelMaterialFull>();
                List<ModelMaterialElem> LstSelElem = new List<ModelMaterialElem>();
                foreach (DataGridCellInfo item in selectedCells)
                {
                    ModelMaterialFull mod = item.Item as ModelMaterialFull;
                    if (!LstSel.Contains(mod))
                    {
                        LstSel.Add(mod);
                        ModelMaterialElem modElem = new ModelMaterialElem();
                        mod.MapperToModel<ModelMaterialFull, ModelMaterialElem>(ref modElem);
                        LstSelElem.Add(modElem);
                    }
                }
                SelectedMaterialElems = LstSelElem;
            });
        }

        /// <summary>
        /// 页面操作 - 添加关联控样1
        /// </summary>
        public ICommand CommandAddRelatedProben1
        {
            get => new MyCommand((d) =>
            {
                if (SelectedMaterialMain == null)
                {
                    sCommon.MyMsgBox("请指定目标牌号项", MsgType.Warning);
                    return;
                }
                if (typeof(winRelatedMaterialProben).IsWindowOpened())
                    return;
                winRelatedMaterialProben win = sCommon.OpenWindow<winRelatedMaterialProben>(this,Application.Current.MainWindow);
                if (win == null) return;
                EditProbenID = 1;
                //设置待选控样
                LstProbenSource = _SparkHelper.GetProbenDetail("控样");
                LstAwaitProbenElement.Clear();
                GetCurrentMaterialElemName();
                CurrentEditMaterialMain = SelectedMaterialMain;
            });
        }

        /// <summary>
        /// 页面操作 - 添加关联控样2
        /// </summary>
        public ICommand CommandAddRelatedProben2
        {
            get => new MyCommand((d) =>
            {
                if (SelectedMaterialMain == null)
                {
                    sCommon.MyMsgBox("请指定目标牌号项", MsgType.Warning);
                    return;
                }
                if (typeof(winRelatedMaterialProben).IsWindowOpened())
                    return;
                winRelatedMaterialProben win = sCommon.OpenWindow<winRelatedMaterialProben>(this, Application.Current.MainWindow);
                if (win == null) return;
                EditProbenID = 2;
                //设置待选控样
                LstProbenSource = _SparkHelper.GetProbenDetail("控样");
                LstAwaitProbenElement.Clear();
                GetCurrentMaterialElemName();
                CurrentEditMaterialMain = SelectedMaterialMain;
            });
        }

        /// <summary>
        /// 添加关联检查样
        /// </summary>
        public ICommand CommandAddRelatedCS
        {
            get => new MyCommand((d) =>
            {
                if (SelectedMaterialMain == null)
                {
                    sCommon.MyMsgBox("请指定目标牌号项", MsgType.Warning);
                    return;
                }
                if (typeof(winRelatedMaterialProben).IsWindowOpened())
                    return;
                winRelatedMaterialProben win = sCommon.OpenWindow<winRelatedMaterialProben>(this, Application.Current.MainWindow);
                if (win == null) return;
                win.Title = "关联检查样";
                EditProbenID = 3;
                //设置待选控样
                LstProbenSource = _SparkHelper.GetProbenDetail("控样|检查样");
                LstAwaitProbenElement.Clear();
                GetCurrentMaterialElemName();
                CurrentEditMaterialMain = SelectedMaterialMain;
            });
        }

        /// <summary>
        /// 页面操作 - 删除关联控样1
        /// </summary>
        public ICommand CommandDeleteRelatedProben1
        {
            get => new MyCommand((d) =>
            {
                if (SelectedMaterialElems.MyCount() == 0)
                {
                    sCommon.MyMsgBox("请选择需要删除的元素?", MsgType.Warning);
                    return;
                }
                MessageBoxResult ret = sCommon.MyMsgBox("您是确定删除该关联数据,删除后数据不能恢复,请确认?", MsgType.Question);
                if (ret != MessageBoxResult.Yes)
                    return;
                CallResult res = _SparkHelper.DeleteMaterialElememt(SelectedMaterialElems);
                CommandMaterialMainSelectionChanged.Execute(SelectedMaterialMain);
                if (res.Fail)
                {
                    sCommon.MyMsgBox($"{res.Result.ToMyString()}", MsgType.Error);
                }
            });
        }

        /// <summary>
        /// 页面操作 - 删除关联控样2
        /// </summary>
        public ICommand CommandDeleteRelatedProben2
        {
            get => new MyCommand((d) =>
            {
                MessageBoxResult ret = sCommon.MyMsgBox("您是确定删除该关联数据,删除后数据不能恢复,请确认?", MsgType.Question);
                if (ret != MessageBoxResult.Yes)
                    return;
                sCommon.MyMsgBox("联系吴云飞添加代码，15706112103");
            });
        }

        /// <summary>
        /// 删除关联检查样
        /// </summary>
        public ICommand CommandDeleteRelatedCS
        {
            get => new MyCommand((d) =>
            {
                return;
            });
        }

        /// <summary>
        /// 牌号校正
        /// </summary>
        public ICommand CommandMaterialCalibirate
        {
            get => new MyCommand((txtProbenName) =>
            {
                if (SelectedMaterialMain == null)
                    return;

                if (string.IsNullOrEmpty(txtProbenName.ToMyString()))
                {
                    sCommon.MyMsgBox("无效牌号,未关联到控样！", MsgType.Warning);
                    return;
                }
                winStartMaterialAnalysis win = new winStartMaterialAnalysis()
                {
                    Material = SelectedMaterialMain.Material,
                    LstProbenName = txtProbenName.ToMyString().MySplit(",")
                };
                win.Owner = Page.FindParent<Window>();
                win.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                bool? ret = win.ShowDialog();
                if (ret == false) return;
                //MessageBoxResult ret = sCommon.MyMsgBox(string.Format("您选择的牌号：【{0}】\r\n" +
                //    "关联控样：{1}\r\n是否确定启动分析？",
                //    SelectedMaterialMain.Material, txtProbenName.ToMyString()), MsgType.Question);
                //if (ret == MessageBoxResult.No)
                //    return;
                Messenger.Default.Send(new NotificationMessage<List<string>>
                    (this, win.LstSelProbenName, "牌号校正"));
            });
        }

        /// <summary>
        /// 牌号检查
        /// </summary>
        public ICommand CommandMaterialCheck
        {
            get => new MyCommand((txtProbenName) =>
            {
                if (SelectedMaterialMain == null)
                {
                    sCommon.MyMsgBox("请选择需要分析的牌号！", MsgType.Warning);
                    return;
                }
                if (string.IsNullOrEmpty(txtProbenName.ToMyString()))
                {
                    sCommon.MyMsgBox("无效牌号,未关联到控样！", MsgType.Warning);
                    return;
                }
                MessageBoxResult ret = sCommon.MyMsgBox(string.Format("您选择的牌号：【{0}】\r\n" +
                    "关联控样：{1}\r\n是否确定启动牌号检查验证？",
                    SelectedMaterialMain.Material, txtProbenName.ToMyString()), MsgType.Question);
                if (ret == MessageBoxResult.No)
                    return;
                Messenger.Default.Send(new NotificationMessage<ModelMaterialMain>
                    (this, SelectedMaterialMain, "牌号检查"));
            });
        }
    }

    /// <summary>
    /// 添加控样
    /// </summary>
    public partial class ViewModelMaterial
    {
        /// <summary>
        /// 当前已有牌号元素
        /// </summary>
        private List<ModelProbenElem> LstAlreadyMaterialElem
            = new List<ModelProbenElem>();

        private ModelMaterialMain CurrentEditMaterialMain = new ModelMaterialMain();

        /// <summary>
        /// 附加控样ID 1#  2#
        /// </summary>
        private int EditProbenID = 1;

        /// <summary>
        /// 待选控样
        /// </summary>
        public List<ModelProbenDetail> LstProbenSource
        {
            get { return _LstProbenSource; }
            set { _LstProbenSource = value; RaisePropertyChanged(); }
        }
        private List<ModelProbenDetail> _LstProbenSource = new List<ModelProbenDetail>();

        /// <summary>
        /// 待选关联元素
        /// </summary>
        public List<ModelProbenElem> LstAwaitProbenElement
        {
            get { return _LstAwaitProbenElement; }
            set { _LstAwaitProbenElement = value; RaisePropertyChanged(); }
        }
        private List<ModelProbenElem> _LstAwaitProbenElement = new List<ModelProbenElem>();

        /// <summary>
        /// 切换控样源
        /// </summary>
        public ICommand CommandProbenSourceSelectionChanged
        {
            get => new MyCommand((selectedProbenDetail) =>
            {
                if (selectedProbenDetail == null)
                    return;
                ModelProbenDetail probenDetail = selectedProbenDetail as ModelProbenDetail;
                if (string.IsNullOrEmpty(probenDetail.ProbenToken))
                    return;
                List<ModelProbenElem> LstAwaitALL = _SparkHelper.GetProbenElement(probenDetail.ProbenToken);
                if (EditProbenID == 1)
                    LstAwaitProbenElement = GetListAwaitProben1Element(LstAwaitALL);
                else if (EditProbenID == 2)
                    LstAwaitProbenElement = GetListAwaitProben2Element(LstAwaitALL);
                else if (EditProbenID == 3)
                    LstAwaitProbenElement = GetListAwaitCSElement(LstAwaitALL);
            });
        }

        /// <summary>
        /// 待选元素全选
        /// </summary>
        public ICommand CommandAwaitSelectAll
        {
            get => new MyCommand((e) =>
              {
                  if (LstAwaitProbenElement == null)
                      return;
                  foreach (var item in LstAwaitProbenElement)
                  {
                      item.IsChecked = true;
                  }
              });
        }

        /// <summary>
        /// 待选元素全不选
        /// </summary>
        public ICommand CommandAwaitUnSelectAll
        {
            get => new MyCommand((e) =>
            {
                if (LstAwaitProbenElement == null)
                    return;
                foreach (var item in LstAwaitProbenElement)
                {
                    item.IsChecked = false;
                }
            });
        }

        /// <summary>
        /// 确认关联控样
        /// </summary>
        public ICommand CommandSureAttachProben
        {
            get => new MyCommand((window) =>
            {
                if (LstAwaitProbenElement == null)
                    return;
                CallResult result = AttachProbenToMaterial(LstAwaitProbenElement, EditProbenID);
                if (result.Fail)
                {
                    sCommon.MyMsgBox($"关联控样发生错误:\r\n{result.Result.ToMyString()}", MsgType.Error);
                    return;
                }
                CommandUpdateProbenElem.Execute(null);
                if (window != null)
                    (window as Window).Close();
            });
        }

        /// <summary>
        /// 获取已选
        /// </summary>
        private void GetCurrentMaterialElemName()
        {
            if (LstMaterialElem == null)
                return;
            LstAlreadyMaterialElem.Clear();
            foreach (var item in LstMaterialElem)
            {
                LstAlreadyMaterialElem.Add(new ModelProbenElem()
                {
                    ProbenToken = item.TS1_Token,
                    Element = item.Element,
                });
            }
        }

        /// <summary>
        /// 控样1的关联逻辑 - 排除牌号已关联的元素，获取可选项
        /// </summary>
        /// <param name="LstAwaitALL"></param>
        /// <returns></returns>
        private List<ModelProbenElem> GetListAwaitProben1Element(List<ModelProbenElem> LstAwaitALL)
        {
            List<ModelProbenElem> LstValidAwait = new List<ModelProbenElem>();
            foreach (ModelProbenElem item in LstAwaitALL)
            {
                if (LstAlreadyMaterialElem.MyContains(x => x.Element == item.Element) ||
                    item.Element.IsEmpty())
                    continue;
                item.IsChecked = true;
                LstValidAwait.Add(item);
            }
            return LstValidAwait;
        }

        /// <summary>
        /// 控样2的关联逻辑 - 所有牌号的元素列表，排除当前已关联的控样元素，其他的都有效
        /// </summary>
        /// <param name="LstAwaitALL"></param>
        /// <returns></returns>
        private List<ModelProbenElem> GetListAwaitProben2Element(List<ModelProbenElem> LstAwaitALL)
        {
            List<ModelProbenElem> LstValidAwait = new List<ModelProbenElem>();
            foreach (ModelProbenElem item in LstAwaitALL)
            {
                if (!LstAlreadyMaterialElem.MyContains(x => x.Element == item.Element) ||
                    item.Element.IsEmpty())
                    continue;
                if (LstAlreadyMaterialElem.MyContains(x => x.ProbenToken == item.ProbenToken && x.Element == item.Element))
                    continue;
                LstValidAwait.Add(item);
            }
            return LstValidAwait;
        }

        /// <summary>
        /// 检查样的关联逻辑 - 所有牌号的元素列表都有效
        /// </summary>
        /// <param name="LstAwaitALL"></param>
        /// <returns></returns>
        private List<ModelProbenElem> GetListAwaitCSElement(List<ModelProbenElem> LstAwaitALL)
        {
            List<ModelProbenElem> LstValidAwait = new List<ModelProbenElem>();
            foreach (ModelProbenElem item in LstAwaitALL)
            {
                if (!LstAlreadyMaterialElem.MyContains(x => x.Element == item.Element) ||
                    item.Element.IsEmpty())
                    continue;
                LstValidAwait.Add(item);
            }
            return LstValidAwait;
        }

        /// <summary>
        /// 添加关联控样
        /// </summary>
        /// <param name="LstAttach"></param>
        private CallResult AttachProbenToMaterial(List<ModelProbenElem> LstAttach, int ProbenID = 1)
        {
            try
            {
                CallResult result = new CallResult();
                IEnumerable<ModelProbenElem> LstSel = LstAttach?.MySelect(x => x.IsChecked == true);
                if (LstAttach == null || LstSel.MyCount() == 0)
                {
                    result.Result = "未确认到添加项";
                    return result;
                }
                if (CurrentEditMaterialMain.Token.IsEmpty())
                {
                    result.Result = "未指定添加牌号";
                    return result;
                }
                List<ModelMaterialElem> LstCmdAddMaterilElem = new List<ModelMaterialElem>();
                foreach (ModelProbenElem item in LstSel)
                {
                    if ((CurrentEditMaterialMain?.Token).IsEmpty() || item.Element.IsEmpty())
                        continue;
                    ModelMaterialElem materialElem = null;
                    if (ProbenID == 1)
                    {
                        materialElem = new ModelMaterialElem()
                        {
                            MaterialToken = CurrentEditMaterialMain?.Token,
                            Element = item.Element,
                            TS1_Token = item.ProbenToken
                        };
                    }
                    else if (ProbenID == 2)
                    {
                        materialElem = new ModelMaterialElem()
                        {
                            MaterialToken = CurrentEditMaterialMain?.Token.MarkWhere(),
                            Element = item.Element.MarkWhere(),
                            TS2_Token = item.ProbenToken
                        };
                    }
                    else if (ProbenID == 3)
                    {
                        materialElem = new ModelMaterialElem()
                        {
                            MaterialToken = CurrentEditMaterialMain?.Token.MarkWhere(),
                            Element = item.Element.MarkWhere(),
                            CS_Token = item.ProbenToken
                        };
                    }
                    if (materialElem != null)
                        LstCmdAddMaterilElem.Add(materialElem);
                }
                if (LstCmdAddMaterilElem.MyCount() == 0)
                {
                    result.Result = "未判定有效关联项";
                    return result;
                }
                if (ProbenID == 1)
                    result = DbFactory.Data?.ExcuteInsert(LstCmdAddMaterilElem, InsertMode.IngoreInsert);
                else if (ProbenID >= 2)
                    result = DbFactory.Data?.ExcuteUpdate(LstCmdAddMaterilElem);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    /// <summary>
    /// 修正设定
    /// </summary>
    public partial class ViewModelMaterial
    {
        /// <summary>
        /// 修正方法页面调用
        /// </summary>
        public ICommand CommandModifyCorretion
        {
            get => new MyCommand((d) =>
            {
                sCommon.OpenWindow<winCorrectMethod>(this);
            });
        }

        /// <summary>
        /// 乘法修正
        /// </summary>
        public ICommand CommandCorrection
        {
            get => new MyCommand((MethodText) =>
            {
                string strMethod = MethodText.ToMyString();
                if (strMethod.IsEquals("不修正"))
                    strMethod = SystemDefault.StringEmpty;
                if (SelectedMaterialElems.MyCount() == 0)
                    return;
                List<ModelMaterialElem> LstUpdate = new List<ModelMaterialElem>();
                foreach (ModelMaterialElem item in SelectedMaterialElems)
                {
                    if (item.MaterialToken.IsEmpty() || item.Element.IsEmpty())
                        continue;
                    LstUpdate.Add(new ModelMaterialElem()
                    {
                        MaterialToken = item.MaterialToken.MarkWhere(),
                        Element = item.Element.MarkWhere(),
                        TolMethod = strMethod
                    });
                }
                if (LstUpdate.MyCount() > 0)
                {
                    DbFactory.Data?.ExcuteUpdate(LstUpdate);
                    CommandUpdateProbenElem.Execute(null);
                }
                sCommon.CloseWindow<winCorrectMethod>();
            });
        }
    }

    /// <summary>
    /// 牌号上下限保存
    /// </summary>
    public partial class ViewModelMaterial
    {
        public ICommand CommandSaveElementUserEdit
        {
            get => new MyCommand(
                (param) =>
                {
                    if (LstMaterialElem == null)
                        return;
                    CallResult res = SaveElementEdit();
                    CommandMaterialMainSelectionChanged.Execute(SelectedMaterialMain);
                    if (res.Fail)
                        sCommon.MyMsgBox($"保存遇到错误:\r\n{res.Result.ToMyString()}", MsgType.Error);
                },
                (param) =>
                {
                    if (LstMaterialElem == null)
                        return false;
                    return LstMaterialElem.MyContains(x => x.IsModified);
                });
        }

        /// <summary>
        /// 保存到元素编辑
        /// </summary>
        /// <returns></returns>
        private CallResult SaveElementEdit()
        {
            CallResult result = new CallResult();
            if (LstMaterialElem == null)
            {
                result.Success = true;
                return result;
            }
            IEnumerable<ModelMaterialFull> lst = LstMaterialElem.MySelect(x => x.IsModified == true);
            List<ModelMaterialElem> LstSaveChange = new List<ModelMaterialElem>();
            foreach (ModelMaterialFull item in lst)
            {
                if (item.MaterialToken.IsEmpty() || item.Element.IsEmpty())
                    continue;
                LstSaveChange.Add(new ModelMaterialElem()
                {
                    MaterialToken = item.MaterialToken.MarkWhere(),
                    Element = item.Element.MarkWhere(),
                    LimitMax = item.LimitMax.ValueAttachMark(),
                    LimitMin = item.LimitMin.ValueAttachMark()
                });
            }
            result = DbFactory.Data?.ExcuteUpdate(LstSaveChange);
            return result;
        }
    }

    /// <summary>
    /// 关于偏差管理
    /// </summary>
    public partial class ViewModelMaterial
    {
        /// <summary>
        /// 牌号偏差设置列表
        /// </summary>
        public ObservableCollection<ModelMaterialDeviation> ListMaterialDeviation
        {
            get { return _ListMaterialDeviation; }
            set { _ListMaterialDeviation = value;RaisePropertyChanged(); }
        }
        private ObservableCollection<ModelMaterialDeviation> _ListMaterialDeviation = 
            new ObservableCollection<ModelMaterialDeviation>();

        /// <summary>
        /// 偏差设置项选中列表
        /// </summary>
        public List<ModelMaterialDeviation> SelectedMaterialDeviationItems
        {
            get { return _SelectedMaterialDeviationItems; }
            set { _SelectedMaterialDeviationItems = value; RaisePropertyChanged(); }
        }
        private List<ModelMaterialDeviation> _SelectedMaterialDeviationItems = new List<ModelMaterialDeviation>();

        /// <summary>
        /// 偏差设置选中项
        /// </summary>
        public ModelMaterialDeviation SelectedMaterialDeviationItem
        {
            get { return _SelectedMaterialDeviationItem; }
            set { _SelectedMaterialDeviationItem = value; RaisePropertyChanged(); }
        }
        private ModelMaterialDeviation _SelectedMaterialDeviationItem = new ModelMaterialDeviation();

        /// <summary>
        /// 页面编辑对象
        /// </summary>
        public ModelDeviationBase CurrentEditMaterialDeviation
        {
            get { return _CurrentEditMaterialDeviation; }
            set { _CurrentEditMaterialDeviation = value; RaisePropertyChanged(); }
        }
        private ModelDeviationBase _CurrentEditMaterialDeviation = new ModelDeviationBase();


        /// <summary>
        /// 获取偏差管理列表
        /// </summary>
        public ICommand CommandUpdateDeviationList
        {
            get => new MyCommand((param) =>
            {
                if (SelectedMaterialMain == null || (SelectedMaterialMain?.Material).IsEmpty())
                    return;
                ListMaterialDeviation = _SparkHelper.GetMaterialDeviations(SelectedMaterialMain?.Material).ToMyObservableCollection();
            });
        }

        /// <summary>
        /// 获取偏差设置选中列表
        /// </summary>
        public ICommand CommandDeviationSelectionChanged
        {
            get => new MyCommand((selectDeviationList) =>
              {
                  SelectedMaterialDeviationItems = selectDeviationList.ToMyList<ModelMaterialDeviation>();
              });
        }

        /// <summary>
        /// 偏差设置配置页面
        /// </summary>
        public ICommand CommandDeviationRelatedSetting
        {
            get => new MyCommand((dgGrid) =>
            {
                if (SelectedMaterialDeviationItems.MyCount() == 0 || sCommon.IsWindowOpened<winDeviationSet>())
                    return;
                ModelDeviationBase mod = new ModelDeviationBase();
                SelectedMaterialDeviationItems[0].MapperToModel(ref mod);
                CurrentEditMaterialDeviation = mod;
                winDeviationSet win = sCommon.OpenWindow<winDeviationSet>(this);
            });
        }

        /// <summary>
        /// 打开偏差模式设置
        /// </summary>
        public ICommand CommandOpenGamaModePage
        {
            get => new MyCommand((d) =>
            {
                if (SelectedMaterialDeviationItems.MyCount() == 0)
                    return;
                winDeviationModeSet win = sCommon.OpenWindow<winDeviationModeSet>(this);
            },
            (d)=>
            {
                return SelectedMaterialDeviationItems.MyCount() > 0;
            });
        }

        /// <summary>
        /// 设置偏差管理模式
        /// </summary>
        public ICommand CommandDeviationModeSet
        {
            get => new MyCommand((DeviationmodeText) =>
            {
                if (SelectedMaterialDeviationItems == null || DeviationmodeText.ToMyString().IsEmpty())
                    return;
                foreach (ModelMaterialDeviation item in SelectedMaterialDeviationItems)
                {
                    ModelMaterialDeviation itm = ListMaterialDeviation.MySelectFirst(x => x.MaterialToken == item.MaterialToken && x.Element == item.Element);
                    if (itm == null)
                        continue;
                    item.GamaMode = DeviationmodeText.ToMyString();
                    itm.IsModified = true;
                }
                ListMaterialDeviation = ListMaterialDeviation.Where(x => x.MaterialToken.IsNotEmpty()).ToList().ToMyObservableCollection();
                CommandSaveOrUpdateDeviations.Execute(null);
                sCommon.CloseWindow<winDeviationModeSet>();
            },
            (d) =>
            {
                return SelectedMaterialDeviationItems.MyCount() > 0;
            });
        }

        /// <summary>
        /// 设置偏差管控
        /// </summary>
        public ICommand CommandGamaEnableSetting
        {
            get => new MyCommand((SettingText) =>
            {
                //0 or 1
                string strSettingText = SettingText.ToMyString();
                if (SelectedMaterialDeviationItems == null || strSettingText.IsEmpty())
                    return;
                foreach (ModelMaterialDeviation item in SelectedMaterialDeviationItems)
                {
                    ModelMaterialDeviation itm = ListMaterialDeviation.MySelectFirst(x => x.MaterialToken == item.MaterialToken && x.Element == item.Element);
                    if (itm == null)
                        continue;
                    item.IsEnabled = strSettingText;
                    itm.IsModified = true;
                }
                ListMaterialDeviation = ListMaterialDeviation.Where(x => x.MaterialToken.IsNotEmpty()).ToList().ToMyObservableCollection();
                CommandSaveOrUpdateDeviations.Execute(null);
            });
        }

        /// <summary>
        /// 提交偏差设置项
        /// </summary>
        public ICommand CommandCommitDeviationSet
        {
            get => new MyCommand((param) =>
            {
                CallResult result = CurrentEditMaterialDeviation?.Validate();
                if (result.Fail)
                {
                    sCommon.MyMsgBox(result.Result.ToMyString(), MsgType.Error);
                    return;
                }
                if (SelectedMaterialDeviationItems == null || CurrentEditMaterialDeviation == null)
                    return;
                foreach (ModelMaterialDeviation item in SelectedMaterialDeviationItems)
                {
                    ModelMaterialDeviation itm = ListMaterialDeviation.MySelectFirst(x => x.MaterialToken == item.MaterialToken && x.Element == item.Element);
                    if (itm == null)
                        continue;
                    CurrentEditMaterialDeviation.MapperToModel(ref itm);
                    itm.IsModified = true;
                }
                ListMaterialDeviation = ListMaterialDeviation.Where(x => x.MaterialToken.IsNotEmpty()).ToList().ToMyObservableCollection();
                CommandSaveOrUpdateDeviations.Execute(null);
                sCommon.CloseWindow<winDeviationSet>();
            });
        }

        /// <summary>
        /// 保存偏差设置修改
        /// </summary>
        public ICommand CommandSaveOrUpdateDeviations
        {
            get => new MyCommand((d) =>
            {
                if (ListMaterialDeviation.MyCount() == 0)
                    return;
                _SparkHelper.SaveOrUpdateMaterialDeviation(ListMaterialDeviation.Where(x=>x.IsModified));
                foreach (var item in ListMaterialDeviation)
                {
                    item.IsModified = false;
                }
            });
        }
    }
}
