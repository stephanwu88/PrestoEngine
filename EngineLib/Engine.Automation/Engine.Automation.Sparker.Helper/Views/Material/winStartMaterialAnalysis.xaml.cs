using Engine.Common;
using Engine.MVVM;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Engine.Automation.Sparker
{
    /// <summary>
    /// winStartMaterialAnalysis.xaml 的交互逻辑
    /// </summary>
    public partial class winStartMaterialAnalysis : Window
    {
        public winStartMaterialAnalysis()
        {
            InitializeComponent();
            DataContext = this;
        }

        /// <summary>
        /// 
        /// </summary>
        public List<string> LstProbenName
        {
            get => _LstProbenName;
            set
            {
                _LstProbenName = value;
                
                LoadProbenItem(_LstProbenName);
            }
        }
        private List<string> _LstProbenName = new List<string>();

        /// <summary>
        /// 
        /// </summary>
        public List<string> LstSelProbenName { get; set; } = new List<string>();

        /// <summary>
        /// 
        /// </summary>
        public string Material
        {
            get { return _Matrtial; }
            set
            {
                _Matrtial = value;
                Caption.Content = string.Format("您选择分析牌号: 【 {0} 】", _Matrtial);
            }
        }
        private string _Matrtial;

        /// <summary>
        /// 
        /// </summary>
        public ICommand CommandSure
        {
            get => new MyCommand((d) =>
             {
                 foreach (CheckBox item in LstView.Items)
                 {
                     if (item.IsChecked==true)
                         LstSelProbenName.Add(item.Content.ToMyString());
                 }
                 if (LstSelProbenName.MyCount() == 0)
                 {
                     sCommon.MyMsgBox("请至少选择一个关联样品分析！", MsgType.Warning);
                     return;
                 }
                 DialogResult = true;
             });
        }

        /// <summary>
        /// 
        /// </summary>
        public ICommand CommandCancel
        {
            get => new MyCommand((d) =>
            {
                Close();
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="LstProbenName"></param>
        private void LoadProbenItem(List<string> LstProbenName)
        {
            LstView.Items.Clear();
            foreach (string item in LstProbenName)
            {
                CheckBox cb = new CheckBox()
                {
                    Content = item,
                    IsChecked = true,
                    VerticalContentAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(3, 3, 3, 3)
                };
                LstView.Items.Add(cb);
            }
        }
    }
}
