using Engine.Common;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Engine.Mod
{
    /// <summary>
    /// winAddFieldItem.xaml 的交互逻辑
    /// </summary>
    public partial class winAddFieldItem : Window
    {
        private EditMode _EditMode;
        private ModelFieldItem _FieldItem;
        public event Action<object, EditMode, ModelFieldItem> NodeUpdated;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="EditMode">编辑模式</param>
        /// <param name="SheetColumn"></param>
        public winAddFieldItem(EditMode editMode, ModelFieldItem fieldItem = null)
        {
            InitializeComponent();
            _EditMode = editMode;
            if (_EditMode == EditMode.Modify && fieldItem == null)
                new ArgumentException("未指定符号编辑对象");
            if (fieldItem != null)
                _FieldItem = fieldItem;
            if (_EditMode == EditMode.AddNew)
            {
                if (_FieldItem == null)
                    _FieldItem = new ModelFieldItem();
            }
            else if (_EditMode == EditMode.Modify)
            {
                Title = "修改配置";
                _ContAddMode.Visibility = Visibility.Visible;
            }
            DataContext = _FieldItem;
        }

        /// <summary>
        /// 命令响应
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cmd_Click(object sender, RoutedEventArgs e)
        {
            string strCmd = (sender as Button).Name;
            switch (strCmd)
            {
                case "CmdSure":
                    CallResult _result = _FieldItem.Validate();
                    if (_result.Fail)
                    {
                        sCommon.MyMsgBox(_result.Result.ToMyString());
                        return;
                    }
                    if (_EditMode == EditMode.AddNew)
                        _FieldItem.Token = SystemDefault.UUID;
                    if (NodeUpdated != null)
                        NodeUpdated(this, _EditMode, _FieldItem);
                    if (_EditMode == EditMode.AddNew && _ContAddMode.IsChecked == true)
                        return;
                    break;

                case "CmdCancel":
                    break;
            }
            Close();
        }
    }
}
