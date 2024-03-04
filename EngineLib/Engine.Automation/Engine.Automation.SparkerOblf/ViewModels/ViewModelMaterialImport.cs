using Engine.Automation.Sparker;
using Engine.Common;
using Engine.Data;
using Engine.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Engine.Automation.OBLF
{
    /// <summary>
    /// 牌号导入
    /// </summary>
    public class ViewModelMaterialImport : NotifyObject
    {
        /// <summary>
        /// 提交事件
        /// </summary>
        public event Action<object, List<ModelLocalMaterialMain>> ActionCommit;

        private List<ModelLocalMaterialMain> MaterialSource = new List<ModelLocalMaterialMain>();
        private string InsName;
        private SparkHelperOblf _SparkHelper;

        /// <summary>
        /// 构造函数
        /// </summary>
        public ViewModelMaterialImport(string insName)
        {
            if (IsDesignMode)
                return;
            InsName = insName;
            _SparkHelper = SparkHelper.Factory.DictFieldValue(insName) as SparkHelperOblf;
            CommandRefresh.Execute(null);
        }

        /// <summary>
        /// 待选牌号源
        /// </summary>
        public List<ModelLocalMaterialMain> LstMaterialSource
        {
            get { return _LstMaterialSource; }
            set { _LstMaterialSource = value; RaisePropertyChanged(); }
        }
        private List<ModelLocalMaterialMain> _LstMaterialSource = new List<ModelLocalMaterialMain>();

        /// <summary>
        /// 选区
        /// </summary>
        public List<ModelLocalMaterialMain> LstMaterialAwait
        {
            get { return _LstMaterialAwait; }
            set { _LstMaterialAwait = value; RaisePropertyChanged(); }
        }
        private List<ModelLocalMaterialMain> _LstMaterialAwait = new List<ModelLocalMaterialMain>();

        /// <summary>
        /// 刷新
        /// </summary>
        public ICommand CommandRefresh
        {
            get => new MyCommand((parameter) =>
            {
                MaterialSource = _SparkHelper.GetLocalMaterialSource();
                LstMaterialSource = MaterialSource.Where(x => x.HandFlag != "S").ToList();
            });
        }

        /// <summary>
        /// 添加到选区
        /// </summary>
        public ICommand CommandAddAwait
        {
            get => new MyCommand((SrcSelectedItems) =>
            {
                //获取待添加对象列表
                List<ModelLocalMaterialMain> LstSel = SrcSelectedItems.ToMyList<ModelLocalMaterialMain>();
                //验证待选对象
                List<string> LstAwaitName = LstMaterialAwait.Select(x => x.Material).Distinct().ToList();
                foreach (ModelLocalMaterialMain item in LstSel)
                {
                    if (LstAwaitName.Contains(item.Material))
                    {
                        item.HandFlag = "N";
                    }
                }
                LstSel = LstSel.Where(x => x.HandFlag != "N").ToList();
                //添加到选区队列
                List<string> AddedLstName = new List<string>();
                List<ModelLocalMaterialMain> AddedList = new List<ModelLocalMaterialMain>();
                foreach (var item in LstSel)
                {
                    if (!AddedLstName.Contains(item.Material))
                        AddedList.Add(item);
                    AddedLstName.Add(item.Material);
                }
                LstMaterialAwait.AddRange(AddedList);
                LstMaterialAwait = LstMaterialAwait.Where(x => x.Material.Length > 0).ToList();
                //修改已选定的列表标记
                foreach (var item in MaterialSource)
                {
                    if (AddedList.Contains(item))
                        item.HandFlag = "S";
                }
                //展示移除后列表
                LstMaterialSource = MaterialSource.Where(x => x.HandFlag != "S").ToList();
                AddedList.Clear(); AddedLstName.Clear();
            });
        }

        /// <summary>
        /// 移出选区
        /// </summary>
        public ICommand CommandRemoveAwait
        {
            get => new MyCommand((AwaitSelectedItems) =>
            {
                //获取已选对象列表
                List<ModelLocalMaterialMain> LstSel = AwaitSelectedItems.ToMyList<ModelLocalMaterialMain>();
                //清除已选定的列表标记
                foreach (var item in MaterialSource)
                {
                    if (LstSel.Contains(item))
                        item.HandFlag = "";
                }
                //移除待选取
                foreach (var item in LstSel)
                {
                    LstMaterialAwait.Remove(item);
                }
                LstMaterialAwait = LstMaterialAwait.Where(x => x.Material.Length > 0).ToList();
                //展示移除后列表
                LstMaterialSource = MaterialSource.Where(x => x.HandFlag != "S").ToList();
            });
        }

        /// <summary>
        /// 确认
        /// </summary>
        public ICommand CommandSure
        {
            get => new MyCommand((CurrentWindow) =>
            {
                if (LstMaterialAwait.MyCount() == 0)
                {
                    sCommon.MyMsgBox("未选定导入牌号，请先选择牌号！", MsgType.Warning);
                    return;
                }
                if (CurrentWindow != null)
                {
                    Window win = CurrentWindow as Window;
                    if (ActionCommit != null)
                        ActionCommit(win, LstMaterialAwait);
                    win.Close();
                }
            });
        }

        /// <summary>
        /// 取消
        /// </summary>
        public ICommand CommandCancel
        {
            get => new MyCommand((CurrentWindow) =>
            {
                if (CurrentWindow != null)
                    (CurrentWindow as Window).Close();
            });
        }
    }
}
