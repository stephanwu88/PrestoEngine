using Engine.Common;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Engine.ComDriver.HEAO
{
    /// <summary>
    /// winAddNew.xaml 的交互逻辑
    /// </summary>
    public partial class winAddNew : Window
    {
        private EditMode _EditMode;
        private ViewComHeao _ViewCom;
        public event Action<object, EditMode, ViewComHeao> NodeUpdated;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="EditMode"></param>
        /// <param name="ViewCom"></param>
        public winAddNew(EditMode EditMode, ViewComHeao ViewCom = null)
        {
            InitializeComponent();
            _EditMode = EditMode;
            if (_EditMode == EditMode.Modify && ViewCom == null)
                new ArgumentException("未指定Heao协议变量编辑对象");
            if (ViewCom != null)
                _ViewCom = ViewCom;
            if (EditMode == EditMode.AddNew)
            {
                if (_ViewCom == null)
                    _ViewCom = new ViewComHeao();
            }
            else if (EditMode == EditMode.Modify)
            {
                Title = "修改变量";
                _ContAddMode.Visibility = Visibility.Visible;
            }
            DataContext = _ViewCom;
        }

        private void Cmd_Click(object sender, RoutedEventArgs e)
        {
            string strCmd = (sender as Button).Name;
            switch (strCmd)
            {
                case "CmdSure":
                    CallResult _result = _ViewCom.Validate();
                    if (_result.Fail)
                    {
                        sCommon.MyMsgBox(_result.Result.ToMyString());
                        return;
                    }
                    if (NodeUpdated != null)
                        NodeUpdated(this, _EditMode, _ViewCom);
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
