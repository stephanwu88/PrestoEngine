using Engine.Common;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Engine.ComDriver.Siemens
{
    /// <summary>
    /// winNewVariable.xaml 的交互逻辑
    /// </summary>
    public partial class winNewVariable : Window
    {
        private EditMode _EditMode;
        private ViewComPLC _ComVarNode;
        public event Action<object, EditMode, ViewComPLC> NodeUpdated;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="EditMode"></param>
        /// <param name="ComVarNode"></param>
        public winNewVariable(EditMode EditMode, ViewComPLC ComVarNode = null)
        {
            InitializeComponent();
            _EditMode = EditMode;
            if(_EditMode== EditMode.Modify && ComVarNode==null)
                new ArgumentException("未指定变量编辑对象");
            if (ComVarNode == null)
                _ComVarNode = ComVarNode;
            if (_EditMode == EditMode.AddNew)
            {
                if (_ComVarNode == null)
                    _ComVarNode = new ViewComPLC();
            }
            else if (_EditMode == EditMode.Modify)
            {
                Title = "修改变量";
                _ContAddMode.Visibility = Visibility.Visible;
            }
            DataContext = _ComVarNode;
        }

        private void Cmd_Click(object sender, RoutedEventArgs e)
        {
            string strCmd = (sender as Button).Name;
            switch (strCmd)
            {
                case "CmdSure":
                    CallResult _result = _ComVarNode.Validate();
                    if (_result.Fail)
                    {
                        sCommon.MyMsgBox(_result.Result.ToMyString());
                        return;
                    }
                    if (_EditMode == EditMode.AddNew)
                        _ComVarNode.Token = SystemDefault.UUID;
                    if (NodeUpdated != null)
                        NodeUpdated(this, _EditMode, _ComVarNode);
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
