using Engine.Common;
using Engine.Data;
using Engine.MVVM;
using System;
using System.Windows;
using System.Windows.Input;

namespace Engine.Automation.Sparker
{
    /// <summary>
    /// 
    /// </summary>
    public class ViewModelConfigProbenMain : NotifyObject
    {
        /// <summary>
        /// 提交事件
        /// </summary>
        public event Action<object, ModelProbenDetail> ActionCommit;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="Mode"></param>
        public ViewModelConfigProbenMain(EditMode Mode)
        {
            editMode = Mode;
            if (Mode == EditMode.AddNew)
            {
                ProbenFull.ProbenToken = SystemDefault.UUID;
                ProbenFull.Type = "控样";
                ProbenFull.AnaCycleTime = "4";
                ProbenFull.PgmToken = "";
            }
        }

        /// <summary>
        /// 编辑模式
        /// </summary>
        public EditMode editMode
        {
            get { return _editMode; }
            set
            {
                _editMode = value;
                RaisePropertyChanged();
                if (_editMode == EditMode.AddNew)
                    WinTitle = "添加控样";
                else
                    WinTitle = "编辑控样";
            }
        }
        private EditMode _editMode = EditMode.AddNew;

        /// <summary>
        /// 连续添加
        /// </summary>
        public bool ContAddMode
        {
            get { return _ContAddMode; }
            set { _ContAddMode = value; RaisePropertyChanged(); }
        }
        private bool _ContAddMode = false;

        /// <summary>
        /// 窗口标题
        /// </summary>
        public string WinTitle
        {
            get { return _WinTitle; }
            set { _WinTitle = value; RaisePropertyChanged(); }
        }
        private string _WinTitle = "";

        /// <summary>
        /// 控样信息
        /// </summary>
        public ModelProbenDetail ProbenFull
        {
            get { return _ProbenFull; }
            set { _ProbenFull = value; RaisePropertyChanged(); }
        }
        private ModelProbenDetail _ProbenFull = new ModelProbenDetail();

        /// <summary>
        /// 确认
        /// </summary>
        public ICommand CommandSure
        {
            get => new MyCommand((parameter) =>
            {
                if (parameter == null) return;
                CallResult res = ProbenFull.Validate();
                if (res.Fail)
                {
                    sCommon.MyMsgBox(res.Result.ToMyString(), MsgType.Error);
                    return;
                }
                if (ActionCommit != null)
                    ActionCommit(parameter as Window, ProbenFull);
                (parameter as Window).Close();
            });
        }

        /// <summary>
        /// 取消
        /// </summary>
        public ICommand CommandCancel
        {
            get => new MyCommand((parameter) =>
            {
                if (parameter == null) return;
                (parameter as Window).Close();
            });
        }
    }
}
