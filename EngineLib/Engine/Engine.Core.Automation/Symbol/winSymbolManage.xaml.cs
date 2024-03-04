using Engine.MVVM;
using System.Windows;

namespace Engine.Core
{
    /// <summary>
    /// winReportMan.xaml 的交互逻辑
    /// </summary>
    public partial class winSymbolManage : Window
    {
        public winSymbolManage()
        {
            InitializeComponent();
            PageSystemSymbol reportMan = new PageSystemSymbol() { Owner = this };
            ViewModel.RunTime.AddSubWin("SubWinSymbolManage", reportMan);
            DataContext = ViewModel.RunTime;
        }
    }
}
