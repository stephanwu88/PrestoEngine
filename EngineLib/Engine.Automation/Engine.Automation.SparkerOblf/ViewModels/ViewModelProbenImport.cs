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
    /// 控样导入
    /// </summary>
    public class ViewModelProbenImport : NotifyObject
    {
        /// <summary>
        /// 提交事件
        /// </summary>
        public event Action<object, List<ModelLocalProbenMain>> ActionCommit;
        private List<ModelLocalProbenMain> ProbenSource = new List<ModelLocalProbenMain>();
        private string InsName;
        private SparkHelperOblf _SparkHelper;

        public ViewModelProbenImport(string insName)
        {
            if (IsDesignMode)
                return;
            InsName = insName;
            _SparkHelper = SparkHelper.Factory.DictFieldValue(InsName) as SparkHelperOblf;
            CommandRefresh.Execute(null);
        }

        /// <summary>
        /// 待选控样源
        /// </summary>
        public List<ModelLocalProbenMain> LstProbenSource
        {
            get { return _LstProbenSource; }
            set { _LstProbenSource = value; RaisePropertyChanged(); }
        }
        private List<ModelLocalProbenMain> _LstProbenSource = new List<ModelLocalProbenMain>();

        /// <summary>
        /// 选区
        /// </summary>
        public List<ModelLocalProbenMain> LstProbenAwait
        {
            get { return _LstProbenAwait; }
            set { _LstProbenAwait = value; RaisePropertyChanged(); }
        }
        private List<ModelLocalProbenMain> _LstProbenAwait = new List<ModelLocalProbenMain>();

        /// <summary>
        /// 刷新
        /// </summary>
        public ICommand CommandRefresh
        {
            get => new MyCommand((parameter) =>
            {
                ProbenSource = _SparkHelper.GetLocalProbenSource();
                LstProbenSource = ProbenSource.Where(x => x.HandFlag != "S").ToList();
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
                List<ModelLocalProbenMain> LstSel = SrcSelectedItems.ToMyList<ModelLocalProbenMain>();
                //验证待选对象
                List<string> LstAwaitName = LstProbenAwait.Select(x => x.Name).Distinct().ToList();
                foreach (ModelLocalProbenMain item in LstSel)
                {
                    if (LstAwaitName.Contains(item.Name))
                    {
                        item.HandFlag = "N";
                    }
                }
                LstSel = LstSel.Where(x => x.HandFlag != "N").ToList();
                //添加到选区队列
                List<string> AddedLstName = new List<string>();
                List<ModelLocalProbenMain> AddedList = new List<ModelLocalProbenMain>();
                foreach (var item in LstSel)
                {
                    if (!AddedLstName.Contains(item.Name))
                        AddedList.Add(item);
                    AddedLstName.Add(item.Name);
                }
                LstProbenAwait.AddRange(AddedList);
                LstProbenAwait = LstProbenAwait.Where(x => x.Name.Length > 0).ToList();
                //修改已选定的列表标记
                foreach (var item in ProbenSource)
                {
                    if (AddedList.Contains(item))
                        item.HandFlag = "S";
                }
                //展示移除后列表
                LstProbenSource = ProbenSource.Where(x => x.HandFlag != "S").ToList();
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
                List<ModelLocalProbenMain> LstSel = AwaitSelectedItems.ToMyList<ModelLocalProbenMain>();
                //清除已选定的列表标记
                foreach (var item in ProbenSource)
                {
                    if (LstSel.Contains(item))
                        item.HandFlag = "";
                }
                //移除待选取
                foreach (var item in LstSel)
                {
                    LstProbenAwait.Remove(item);
                }
                LstProbenAwait = LstProbenAwait.Where(x => x.Name.Length > 0).ToList();
                //展示移除后列表
                LstProbenSource = ProbenSource.Where(x => x.HandFlag != "S").ToList();
            });
        }

        /// <summary>
        /// 确认
        /// </summary>
        public ICommand CommandSure
        {
            get => new MyCommand((CurrentWindow) =>
            {
                if (LstProbenAwait.MyCount() == 0)
                {
                    sCommon.MyMsgBox("未选定导入控样，请先选择控样！", MsgType.Warning);
                    return;
                }
                if (CurrentWindow != null)
                {
                    Window win = CurrentWindow as Window;
                    if (ActionCommit != null)
                        ActionCommit(win, LstProbenAwait);
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
