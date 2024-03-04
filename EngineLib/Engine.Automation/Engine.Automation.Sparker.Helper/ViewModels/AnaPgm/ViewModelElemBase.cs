using Engine.Common;
using Engine.Data.DBFAC;
using Engine.MVVM;
using Engine.WpfBase;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Engine.Automation.Sparker
{
    /// <summary>
    /// 配置分析程序
    /// </summary>
    [ViewModel()]
    public partial class ViewModelElemBase : ViewFrameBase
    {
        private SparkHelper _SparkHelper;
        private bool CanSave = false;
        /// <summary>
        /// 
        /// </summary>
        private IDBFactory<ServerNode> DbData = DbFactory.Data;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="Mode"></param>
        public ViewModelElemBase()
        {
            _SparkHelper = SparkHelper.Current;
            LstElemBase.CollectionChanged += (s, e) =>
              {
                  SetOrderID();
                  RaisePropertyChanged(nameof(LstElemBase));
              };
        }

        /// <summary>
        /// 分析程序实体
        /// </summary>
        public ModelSpecPgm AnaPgm
        {
            get { return _AnaPgm; }
            set
            {
                _AnaPgm = value;
                CommandUpdateLstElemBase.Execute(null);
                SetOrderID();
                RaisePropertyChanged();
            }
        }
        private ModelSpecPgm _AnaPgm = new ModelSpecPgm();

        /// <summary>
        /// 分析方法拓展元素列表
        /// </summary>
        public ObservableCollection<ModelElemBase> LstElemBase
        {
            get { return _LstElemBase; }
            set { _LstElemBase = value; RaisePropertyChanged(); }
        }
        private ObservableCollection<ModelElemBase> _LstElemBase = new ObservableCollection<ModelElemBase>();

        /// <summary>
        /// 选中项集合
        /// </summary>
        public ObservableCollection<ModelElemBase> SelectedElemBaseItems
        {
            get { return _SelectedElemBaseItems; }
            set { _SelectedElemBaseItems = value; RaisePropertyChanged(); }
        }
        private ObservableCollection<ModelElemBase> _SelectedElemBaseItems = new ObservableCollection<ModelElemBase>();

        /// <summary>
        /// 当前选中项
        /// </summary>
        public ModelElemBase CurrentSelectedElemBase
        {
            get { return _CurrentSelectedElemBase; }
            set { _CurrentSelectedElemBase = value;RaisePropertyChanged(); }
        }
        private ModelElemBase _CurrentSelectedElemBase = new ModelElemBase();
       
        /// <summary>
        /// 选中单元格变化
        /// </summary>
        public ICommand CommandSelectCellsChanged
        {
            get => new MyCommand((selected) =>
            {
                var selectedCells = selected as IList<DataGridCellInfo>;
                ObservableCollection<ModelElemBase> LstSel = new ObservableCollection<ModelElemBase>();
                foreach (DataGridCellInfo item in selectedCells)
                {
                    ModelElemBase mod = item.Item as ModelElemBase;
                    if (!LstSel.Contains(mod))
                        LstSel.Add(mod);
                }
                SelectedElemBaseItems = LstSel;
            });
        }

        /// <summary>
        /// 重新设置排序号
        /// </summary>
        private void SetOrderID()
        {
            int i = 1;
            foreach (var item in LstElemBase)
            {
                item.OrderID = i.ToString();
                i++;
            }
        }

        /// <summary>
        /// 更新拓展元素配置列表
        /// </summary>
        public ICommand CommandUpdateLstElemBase
        {
            get => new MyCommand((d) =>
            {
                if (AnaPgm.Token.IsEmpty())
                    return;
                ModelSpecPgm model = new ModelSpecPgm() { Token = AnaPgm.Token };
                List<ModelElemBase> lst = _SparkHelper.GetAnaPgmExtendElemList(model);
                LstElemBase.Clear();
                foreach (var item in lst)
                {
                    LstElemBase.Add(item);
                }
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
                     LstElemBase.Add(new ModelElemBase() { PgmToken = AnaPgm.Token, DecimalDigits = "4", DataFormat = "0.00##", Unit = "%" });
                 }
                 else if (CommandValue == "AddFromLib")
                 {
                     LstUnSelectExtendElem = GetUnSelectElemList();
                     if (LstUnSelectExtendElem.MyCount() == 0)
                     {
                         sCommon.MyMsgBox("元素库中没有可再用的拓展元素，请转至元素库添加!", MsgType.Error);
                         return;
                     }
                     LstUnSelectExtendElem.Sort(new CustomCompare<ModelElemBase>());
                     winAttachElement win = sCommon.OpenWindow<winAttachElement>(this, null, null);
                 }
                 else if (CommandValue == "Delete")
                 {
                     if (!LstElemBase.MyContains(x => x.Element.IsNotEmpty()))
                         return;
                     MessageBoxResult ret = sCommon.MyMsgBox("删除后不可恢复,您是否确定删除？", MsgType.Question);
                     if (ret == MessageBoxResult.No)
                         return;
                     if (CanSave) _SparkHelper?.SaveOrUpdateElemBaseItems(LstElemBase);
                     _SparkHelper?.DeleteElemBaseItems(SelectedElemBaseItems);
                     CommandUpdateLstElemBase.Execute(null);
                 }
                 else if (CommandValue == "MoveUp")
                 {
                     sCommon.MoveListUp(LstElemBase, SelectedElemBaseItems);
                 }
                 else if (CommandValue == "MoveDn")
                 {
                     sCommon.MoveListDn(LstElemBase, SelectedElemBaseItems);
                 }
                 else if (CommandValue == "Save")
                 {
                     if (CanSave)
                         _SparkHelper?.SaveOrUpdateElemBaseItems(LstElemBase);
                     CommandUpdateLstElemBase.Execute(null);
                 }
                 else if (CommandValue == "Update")
                 {
                     if (CanSave)
                     {
                         MessageBoxResult ret = sCommon.MyMsgBox("是否保存当前设置？", MsgType.Question);
                         if (ret == MessageBoxResult.Yes)
                             _SparkHelper.SaveOrUpdateElemBaseItems(LstElemBase);
                     }
                     CommandUpdateLstElemBase.Execute(null);
                 }
             },
            (SelectedItem) =>
            {
                var selected = SelectedItem as System.Xml.XmlElement;
                string CommandValue = selected.Attributes["Value"].Value;
                if (CommandValue == "Delete")
                {
                    return SelectedElemBaseItems.MyContains(x => x.Element.IsNotEmpty());
                }
                else if (CommandValue == "Save")
                {
                    CanSave = LstElemBase.MyContains(x => (x.ID.IsEmpty() && x.Element.IsNotEmpty()) || x.IsModified);
                    return CanSave;
                }
                return true;
            });
        }
    }

    /// <summary>
    /// 从库添加元素页面
    /// </summary>
    public partial class ViewModelElemBase
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
            List<ModelElemBase> LstAll = _SparkHelper.GetAnaPgmExtendElemList(new ModelSpecPgm() { Token = _SparkHelper?.BaseIns.InsName });
            foreach (ModelElemBase item in LstAll)
            {
                if (!string.IsNullOrEmpty(item.Element) &&
                    !LstElemBase.MyContains(x => x.Element == item.Element.ToMyString()))
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
        /// 确认添加元素
        /// </summary>
        public ICommand CommandSureAttachElement
        {
            get => new MyCommand((ObjectWindow) =>
            {
                IEnumerable<ModelElemBase> lstSelect = LstUnSelectExtendElem.MySelect(x => x.IsChecked == true);
                if (lstSelect.MyCount() == 0)
                    return;
                List<ModelProbenElem> LstProbenElem = new List<ModelProbenElem>();
                foreach (ModelElemBase eBase in lstSelect)
                {
                    if (string.IsNullOrWhiteSpace(eBase.Element))
                        continue;
                    LstElemBase.Add(new ModelElemBase()
                    {
                        PgmToken = AnaPgm.Token,
                        Element = eBase.Element,
                        MassRangeU = eBase.MassRangeU,
                        MassRangeD = eBase.MassRangeD,
                        DecimalDigits = eBase.DecimalDigits.IsNotEmpty() ? eBase.DecimalDigits : "4",
                        DataFormat = eBase.DataFormat.IsNotEmpty() ? eBase.DataFormat : "0.00##",
                        Unit = eBase.Unit.IsNotEmpty() ? eBase.Unit : "%"
                    });
                }

                //关闭操作窗口
                if (ObjectWindow != null)
                    (ObjectWindow as Window)?.Close();
            });
        }
    }
}
