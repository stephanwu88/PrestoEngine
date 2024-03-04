using System.Windows;
using System.Windows.Controls;
using Engine.Common;
using Engine.Access;

namespace Engine.Template
{
    /// <summary>
    /// winHeaoActivation.xaml 的交互逻辑
    /// </summary>
    public partial class winHeaoActivation : Window
    {
        private bool IsLicesed = false;
        public winHeaoActivation()
        {
            InitializeComponent();
            if (HeaoKeyGen.Default.VerifyRegStatus())
            {
                sCommon.MyMsgBox("软件已注册！", MsgType.Infomation);
                IsLicesed = true;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (IsLicesed)
                this.DialogResult = true;
            _MachineSerialNumber.Text = HeaoKeyGen.Default.GetMachineCode();
        }

        private void Cmd_Click(object sender, RoutedEventArgs e)
        {
            string strCmd = (sender as Button).Name;
            switch (strCmd)
            {
                case "CmdRegister":
                    if (HeaoKeyGen.Default.SoftRegister(_MachineSerialNumber.Text, _KeyNumber.Text))
                    {
                        sCommon.MyMsgBox("注册成功,祝您体验愉快！", MsgType.Infomation);
                        this.DialogResult = true;
                    }
                    else
                        sCommon.MyMsgBox("注册未通过，请重试！", MsgType.Error);
                    break;
                case "CmdCancel":
                    this.DialogResult = false;
                    this.Close();
                    break;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //Application.Current.Shutdown(0);
            //Environment.Exit(0);
        }
    }
}
