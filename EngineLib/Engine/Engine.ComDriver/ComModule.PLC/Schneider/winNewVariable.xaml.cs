using System;
using System.Windows;
using System.Windows.Controls;

namespace Engine.ComDriver.Schneider
{
    /// <summary>
    /// winNewVarible.xaml 的交互逻辑
    /// </summary>
    public partial class winNewVariable : Window
    {
        private ModelComPLC _Node;
        private string _Authority;
        public Action<object, object> NodeUpdated; 
        public winNewVariable()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="node"></param>
        /// <param name="authority">New or Edit</param>
        public winNewVariable(ModelComPLC node,string authority)
        {
            InitializeComponent();
            _Node = node;
            if (_Node == null)
                _Node = new ModelComPLC();
            _Authority = authority;
            this.Title = _Authority == "New" ? "新建变量" : "修改变量";
            _ContAddMode.Visibility = _Authority == "New" ? Visibility.Visible : Visibility.Hidden;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _VarType.Items.Clear();
            _VarType.Items.Add("Bool");
            _VarType.Items.Add("Byte");
            _VarType.Items.Add("Int");
            _VarType.Items.Add("DInt");
            _VarType.Items.Add("Word");
            _VarType.Items.Add("DWord");
            _VarType.Items.Add("Real");
            _VarType.SelectedIndex = 0;

            _VarAccess.Items.Add("读/写");
            _VarAccess.Items.Add("只读");
            _VarAccess.SelectedIndex = 0;

            //_Node.strTag = _Authority;

            if (_Authority == "Edit")
            {
                //_VarRemark.Text = _Node.V_REMARK;
                //_VarName.Text = _Node.V_NAME;
                //_VarAddr.Text = _Node.V_ADDR;
                //_VarType.Text = _Node.V_TYPE ;
                //_VarAccess.Text = _Node.V_ACCESS;
                //_ChkHmiView.IsChecked = _Node.IsHmiVisible;
                //_ChkAlarm.IsChecked = _Node.IsAlarm;
                //_BindExp1.Text = _Node.BindExp1;
                //_BindExp2.Text = _Node.BindExp2;
            }
        }

        private void _Cmd_Click(object sender, RoutedEventArgs e)
        {
            string strCmd = (sender as Button).Name;
            switch (strCmd)
            {
                case "_CmdSure":
                    if (!CheckAndUpdNode())
                        return;
                    if (NodeUpdated != null)
                        NodeUpdated(this,_Node);
                    if (_Authority == "New" && _ContAddMode.IsChecked == true)
                        return;
                    break;
                case "_CmdCancel":
                    break;
            }
            this.Close();
        }

        private bool CheckAndUpdNode()
        {
            // _Node.V_REMARK = _VarRemark.Text;
            // _Node.V_NAME = _VarName.Text ;
            // _Node.V_ADDR = _VarAddr.Text ;
            // _Node.V_TYPE = _VarType.Text ;
            // _Node.V_ACCESS = _VarAccess.Text;
            // _Node.V_FORMATE= "{0}";
            // _Node.IsHmiVisible = _ChkHmiView.IsChecked == true;
            // _Node.IsAlarm = _ChkAlarm.IsChecked == true;
            //_Node.BindExp1 = _BindExp1.Text.Trim();
            //_Node.BindExp2 = _BindExp2.Text.Trim();
            return true;
        }

        private bool CheckVarAddrValid()
        {

            return true;
        }
    }
}
