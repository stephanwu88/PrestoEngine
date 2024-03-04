using Engine.Data.DBFAC;
using Engine.MVVM;
using System.Windows;

namespace Engine.Mod
{
    /// <summary>
    /// winReportManage.xaml 的交互逻辑
    /// </summary>
    public partial class winSheetManage : Window
    {
        public winSheetManage(IDBFactory<ServerNode> DbConn)
        {
            InitializeComponent();

            PageSheetManage sheetManage = new PageSheetManage(DbConn) { Owner = this };
            ViewModel.RunTime.AddSubWin("SubWinSheetManage", sheetManage);
            DataContext = ViewModel.RunTime;
        }
    }
}
