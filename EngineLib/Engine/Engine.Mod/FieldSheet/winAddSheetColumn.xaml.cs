using Engine.Common;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Engine.Mod
{
    /// <summary>
    /// winAddSheetColumn.xaml 的交互逻辑
    /// </summary>
    public partial class winAddSheetColumn : Window
    {
        private EditMode _EditMode;
        private ViewSheetColumn _SheetColumn;
        public event Action<object, EditMode, ViewSheetColumn> NodeUpdated;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="EditMode">编辑模式</param>
        /// <param name="SheetColumn"></param>
        public winAddSheetColumn(EditMode EditMode, ViewSheetColumn SheetColumn = null)
        {
            InitializeComponent();
            _EditMode = EditMode;
            if (_EditMode == EditMode.Modify && SheetColumn == null)
                new ArgumentException("未指定符号编辑对象");
            if (SheetColumn != null)
                _SheetColumn = SheetColumn;
            if (_EditMode == EditMode.AddNew)
            {
                if (_SheetColumn == null)
                    _SheetColumn = new ViewSheetColumn();
            }
            else if (_EditMode == EditMode.Modify)
            {
                Title = "修改列配置";
                _ContAddMode.Visibility = Visibility.Visible;
            }
            DataContext = _SheetColumn;
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
                    CallResult _result = _SheetColumn.Validate();
                    if (_result.Fail)
                    {
                        sCommon.MyMsgBox(_result.Result.ToMyString());
                        return;
                    }
                    if (_EditMode == EditMode.AddNew)
                        _SheetColumn.Token = SystemDefault.UUID;
                    if (NodeUpdated != null)
                        NodeUpdated(this, _EditMode, _SheetColumn);
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
