using System;
using System.Windows;
using System.IO;
using Engine.Common;

namespace Engine.Template
{
    /// <summary>
    /// winQuickService.xaml 的交互逻辑
    /// </summary>
    public partial class winQuickService : Window
    {
        public winQuickService()
        {
            InitializeComponent();
            LoadQuickServiceMsg();
        }

        private void LoadQuickServiceMsg()
        {
            this._MsgMaintainer.Text = GetTxtContent("ServiceMaintainer.txt");
            this._MsgDesigner.Text = GetTxtContent("ServiceDesigner.txt");
        }

        private string GetTxtContent(string strFileName)
        {
            string TargetFile = Environment.CurrentDirectory + @"\Config\" + strFileName;
            if (!File.Exists(TargetFile))
                return string.Empty;
            return File.ReadAllText(TargetFile);
        }
    }
}
