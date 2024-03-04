using Engine.Common;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Engine.ComDriver
{
    /// <summary>
    /// winNewController.xaml 的交互逻辑
    /// </summary>
    public partial class winNewController : Window
    {
        private EditMode _EditMode;
        private ViewDriverItem _ViewItem;
        public event Action<object, EditMode, ViewDriverItem> NodeUpdated;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="EditMode"></param>
        /// <param name="ViewItem"></param>
        public winNewController(EditMode EditMode, ViewDriverItem ViewItem = null)
        {
            InitializeComponent();
            _EditMode = EditMode;
            if (_EditMode == EditMode.Modify && ViewItem == null)
                new ArgumentException("未指定控制器编辑对象");
            if (ViewItem != null)
                _ViewItem = ViewItem;
            if (_EditMode == EditMode.AddNew)
            {
                if (_ViewItem == null)
                    _ViewItem = new ViewDriverItem();
            }
            else if (_EditMode == EditMode.Modify)
            {
                Title = "修改控制器";
                _ContAddMode.Visibility = Visibility.Visible;
            }
            DataContext = _ViewItem;
        }

        /// <summary>
        /// 窗口指令处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cmd_Click(object sender, RoutedEventArgs e)
        {
            string strCmd = (sender as Button).Name;
            switch (strCmd)
            {
                case "CmdSure":
                    CallResult _result = _ViewItem.Validate();
                    if (_result.Fail)
                    {
                        sCommon.MyMsgBox(_result.Result.ToMyString());
                        return;
                    }
                    if (_EditMode == EditMode.AddNew)
                        _ViewItem.DriverToken = SystemDefault.UUID;
                    if (NodeUpdated != null)
                        NodeUpdated(this, _EditMode, _ViewItem);
                    if (_EditMode == EditMode.AddNew && _ContAddMode.IsChecked == true)
                        return;
                    break;

                case "CmdCancel":
                    break;
            }
            this.Close();
        }

  
    }
}
