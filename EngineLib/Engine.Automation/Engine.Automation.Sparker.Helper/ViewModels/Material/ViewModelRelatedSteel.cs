using Engine.Common;
using Engine.MVVM;
using Engine.WpfBase;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Engine.Automation.Sparker
{
    [ViewModel()]
    public partial class ViewModelRelatedSteel : ViewFrameBase
    {
        private SparkHelper _SparkHelper;
        private bool IsRelatedEditing = false;
        private bool CanSave = false;

        public ViewModelRelatedSteel()
        {
            if (IsDesignMode)
                return;
            _SparkHelper = SparkHelper.Current;

            LstRelatedSteelItems.CollectionChanged += (s, e) =>
            {
                SetOrderID();
                RaisePropertyChanged(nameof(LstRelatedSteelItems));
            };
            CommandUpdateSteelList.Execute(null);
        }

        /// <summary>
        /// 关联钢种列表
        /// </summary>
        public ObservableCollection<ModelRelatedSteelType> LstRelatedSteelItems
        {
            get { return _LstRelatedSteelItems; }
            set { _LstRelatedSteelItems = value; RaisePropertyChanged(); }
        }
        private ObservableCollection<ModelRelatedSteelType> _LstRelatedSteelItems =
            new ObservableCollection<ModelRelatedSteelType>();

        /// <summary>
        /// 已选钢种列表
        /// </summary>
        public ObservableCollection<ModelRelatedSteelType> SelectedRelatedSteelItems
        {
            get { return _SelectedRelatedSteelItems; }
            set { _SelectedRelatedSteelItems = value; RaisePropertyChanged(); }
        }
        private ObservableCollection<ModelRelatedSteelType> _SelectedRelatedSteelItems =
            new ObservableCollection<ModelRelatedSteelType>();

        /// <summary>
        /// 更新列表
        /// </summary>
        public ICommand CommandUpdateSteelList
        {
            get => new MyCommand((e) =>
            {
                List<ModelRelatedSteelType> lst = _SparkHelper?.GetRelatedSteelTypeItems();
                LstRelatedSteelItems.Clear();
                foreach (ModelRelatedSteelType item in lst)
                {
                    LstRelatedSteelItems.Add(item);
                }
            });
        }

        /// <summary>
        /// 选中单元格变化
        /// </summary>
        public ICommand CommandSelectCellsChanged
        {
            get => new MyCommand((selected) =>
            {
                if (IsRelatedEditing)
                    return;
                var selectedCells = selected as IList<DataGridCellInfo>;
                ObservableCollection<ModelRelatedSteelType> LstSel = new ObservableCollection<ModelRelatedSteelType>();
                foreach (DataGridCellInfo item in selectedCells)
                {
                    ModelRelatedSteelType mod = item.Item as ModelRelatedSteelType;
                    if (!LstSel.Contains(mod))
                        LstSel.Add(mod);
                }
                SelectedRelatedSteelItems = LstSel;
            });
        }

        /// <summary>
        /// 单元格编辑结束
        /// </summary>
        public ICommand CommandCellsEditEnding
        {
            get => new MyCommand((selected) =>
            {
                //if (IsRelatedEditing)
                //    return;d
                //var selectedCells = selected as IList<DataGridCellInfo>;
                //foreach (DataGridCellInfo item in selectedCells)
                //{
                //    ModelRelatedSteelType modNew = item.Item as ModelRelatedSteelType;
                //    ModelRelatedSteelType modCurrent = LstRelatedSteelItems.MySelectFirst(x => x.Equals(modNew));
                //    if (modCurrent != null) modCurrent.IsModified = true;
                //}
            });
        }

        /// <summary>
        /// 重新设置排序号
        /// </summary>
        private void SetOrderID()
        {
            int i = 1;
            foreach (var item in LstRelatedSteelItems)
            {
                item.OrderID = i.ToString();
                i++;
            }
        }

        /// <summary>
        /// 打开编辑钢种绑定
        /// </summary>
        public ICommand CommandOpenSteelBinding
        {
            get => new MyCommand((d) =>
            {
                if (IsRelatedEditing)
                    return;
                if (!SelectedRelatedSteelItems.MyContains(x=>x.SteelType.IsNotEmpty()))
                {
                    //sCommon.MyMsgBox("请选中需要管理的钢种", MsgType.Warning);
                    return;
                }
                IsRelatedEditing = true;
                winRelatedSteel win = sCommon.OpenWindow<winRelatedSteel>(this);
                win.Closing += (s, e) => { IsRelatedEditing = false; };
                //加载待选资源
                LoadDefaultAwaitSource.Execute(null);
            });
        }

        /// <summary>
        /// 菜单指令响应
        /// </summary>
        public ICommand CommandMenuItemInvoked
        {
            get => new MyCommand(SelectedItem =>
            {
                var selected = SelectedItem as System.Xml.XmlElement;
                string CommandValue = selected.Attributes["Value"].Value;
                if (CommandValue == "AddNew")
                {
                    LstRelatedSteelItems.Add(new ModelRelatedSteelType());
                }
                else if (CommandValue == "Delete")
                {
                    if (!SelectedRelatedSteelItems.MyContains(x => x.SteelType.IsNotEmpty()))
                        return;
                    MessageBoxResult ret = sCommon.MyMsgBox("删除后不可恢复,您是否确定删除？", MsgType.Question);
                    if (ret == MessageBoxResult.No)
                        return;
                    _SparkHelper?.SaveOrUpdateRelatedSteelType(SelectedRelatedSteelItems);
                    _SparkHelper?.DeleteSteelRangeList(SelectedRelatedSteelItems);
                    CommandUpdateSteelList.Execute(null);
                }
                else if (CommandValue == "MoveUp")
                {
                    sCommon.MoveListUp(LstRelatedSteelItems, SelectedRelatedSteelItems);
                }
                else if (CommandValue == "MoveDn")
                {
                    sCommon.MoveListDn(LstRelatedSteelItems, SelectedRelatedSteelItems);
                }
                else if (CommandValue == "RelatedMaterial")
                {
                    CommandOpenSteelBinding.Execute(null);
                }
                else if (CommandValue == "Save")
                {
                    _SparkHelper?.SaveOrUpdateRelatedSteelType(LstRelatedSteelItems);
                    CommandUpdateSteelList.Execute(null);
                }
                else if (CommandValue == "Update")
                {
                    if (CanSave)
                    {
                        MessageBoxResult ret = sCommon.MyMsgBox("是否保存当前设置？", MsgType.Question);
                        if (ret == MessageBoxResult.Yes)
                            _SparkHelper?.SaveOrUpdateRelatedSteelType(LstRelatedSteelItems);
                    }
                    CommandUpdateSteelList.Execute(null);
                }
            
            },
            (SelectedItem) =>
                {
                    var selected = SelectedItem as System.Xml.XmlElement;
                    string CommandValue = selected.Attributes["Value"].Value;
                    if (CommandValue == "Delete")
                    {
                        return SelectedRelatedSteelItems.MyContains(x => x.SteelType.IsNotEmpty());
                    }
                    else if (CommandValue == "Save")
                    {
                        CanSave =  LstRelatedSteelItems.MyContains(x => (x.ID.IsEmpty() && x.SteelType.IsNotEmpty()) || x.IsModified);
                        return CanSave;
                    }
                    else if (CommandValue == "RelatedMaterial")
                    {
                        return SelectedRelatedSteelItems.MyContains(x => x.SteelType.IsNotEmpty());
                    }
                    return true;
                });
        }
    }

    /// <summary>
    /// 钢种关联分析曲线、牌号编辑页
    /// </summary>
    public partial class ViewModelRelatedSteel
    {
        /// <summary>
        /// 当前编辑绑定的分析曲线、牌号
        /// </summary>
        public ModelMaterialMain CurrentSelectedRelatedMaterialMain 
        {
            get { return _CurrentSelectedRelatedMaterialMain; }
            set { _CurrentSelectedRelatedMaterialMain = value; RaisePropertyChanged(); }
        }
        private ModelMaterialMain _CurrentSelectedRelatedMaterialMain = new ModelMaterialMain();
        
        /// <summary>
        /// 待选分析曲线列表
        /// </summary>
        public List<ModelSpecPgm> LstAwaitAnaPgm
        {
            get { return _LstAwaitAnaPgm; }
            set { _LstAwaitAnaPgm = value; RaisePropertyChanged(); }
        }
        private List<ModelSpecPgm> _LstAwaitAnaPgm = new List<ModelSpecPgm>();

        /// <summary>
        /// 当前选定分析程序
        /// </summary>
        public ModelSpecPgm SelectedAnaPgm
        {
            get { return _SelectedAnaPgm; }
            set { _SelectedAnaPgm = value; RaisePropertyChanged(); }
        }
        private ModelSpecPgm _SelectedAnaPgm = new ModelSpecPgm();

        /// <summary>
        /// 待选牌号列表
        /// </summary>
        public List<ModelMaterialMain> LstAwaitMaterialMain
        {
            get { return _LstAwaitMaterialMain; }
            set { _LstAwaitMaterialMain = value; RaisePropertyChanged(); }
        }
        private List<ModelMaterialMain> _LstAwaitMaterialMain = new List<ModelMaterialMain>();

        /// <summary>
        /// 关联牌号列表
        /// </summary>
        public List<ModelMaterialMain> LstRelatedAwaitMaterialMain
        {
            get { return _LstRelatedAwaitMaterialMain; }
            set { _LstRelatedAwaitMaterialMain = value; RaisePropertyChanged(); }
        }
        private List<ModelMaterialMain> _LstRelatedAwaitMaterialMain = new List<ModelMaterialMain>();

        /// <summary>
        /// 加载默认待选资源
        /// </summary>
        public ICommand LoadDefaultAwaitSource
        {
            get => new MyCommand((d) =>
            {
                LstAwaitAnaPgm = _SparkHelper.GetAnaPgmList();
                LstAwaitMaterialMain = _SparkHelper.GetMaterialMain();
                if (LstAwaitAnaPgm.MyCount() > 0)
                    SelectedAnaPgm = LstAwaitAnaPgm.MySelectAny(0);
                CommandRelatedAnaPgmSelectChanged.Execute(null);
            });
        }

        /// <summary>
        /// 关联分析程序切换
        /// </summary>
        public ICommand CommandRelatedAnaPgmSelectChanged
        {
            get => new MyCommand((e) =>
            {
                if ((SelectedAnaPgm?.Token).IsEmpty())
                    return;
                LstRelatedAwaitMaterialMain = LstAwaitMaterialMain.MySelect(x => x.PgmToken == SelectedAnaPgm?.Token).ToList();
            });
        }

        /// <summary>
        /// 提交钢种关联
        /// </summary>
        public ICommand CommandCommitSteelTypeBinding
        {
            get => new MyCommand((d) =>
            {
                if ((_CurrentSelectedRelatedMaterialMain?.Token).IsEmpty() ||
                        SelectedRelatedSteelItems.MyCount() == 0 || 
                        LstRelatedSteelItems.MyCount()==0)
                    return;
                foreach (var item in LstRelatedSteelItems)
                {
                    if (item.SteelType.IsEmpty())
                        continue;
                    if (SelectedRelatedSteelItems.Contains(item))
                    {
                        item.MaterialToken = _CurrentSelectedRelatedMaterialMain?.Token;
                        item.IsModified = true;
                    }
                }
                CallResult result = _SparkHelper?.SaveOrUpdateRelatedSteelType(LstRelatedSteelItems);
                //CallResult result = DbFactory.Data?.ExcuteUpdate(LstUpd);
                CommandUpdateSteelList.Execute(null);
                if (result.Fail)
                {
                    sCommon.MyMsgBox($"关联牌号发生错误：\r\n{result.Result.ToMyString()}", MsgType.Error);
                }
                sCommon.CloseWindow<winRelatedSteel>();
            });
        }
    }
}
