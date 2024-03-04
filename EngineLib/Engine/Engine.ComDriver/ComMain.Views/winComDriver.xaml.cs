using Engine.Data.DBFAC;
using Engine.MVVM;
using System.Windows;

namespace Engine.ComDriver
{
    /// <summary>
    /// winComDriver.xaml 的交互逻辑
    /// </summary>
    public partial class winComDriver : Window
    {
        public winComDriver(ServerNode serverNode = null)
        {
            InitializeComponent();
            ViewModel.RunTime.AddSubWin("SubWinComDrvier", new PageController(serverNode) { Owner = this });
            DataContext = ViewModel.RunTime;
        }
    }
}
