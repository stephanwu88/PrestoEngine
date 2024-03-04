using Engine.Common;
using Engine.MVVM;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace Engine.Automation.Sparker
{
    public class ViewModelPageAnaData : ViewFrameBase
    {
        public ViewModelPageAnaData()
        {
            if (!IsDesignMode)
            {
               
                MenuItemList = new ObservableCollection<PrsMenuItem>()
                {
                    new PrsMenuItem(){ PageID="1", Icon="&#xe63c;".UnicodeToString(),Label="当前分析",Page = new PageAnaDataCurrent() },
                    new PrsMenuItem(){ PageID="1", Icon="&#xe63c;".UnicodeToString(),Label="历史分析",Page = new PageAnaDataHistory() },
                };
            }
        }
    }

    /// <summary>
    /// PageAnaData.xaml 的交互逻辑
    /// </summary>
    public partial class PageAnaData : UserControl
    {
        public Window Owner { get; set; }
        public PageAnaData()
        {
            InitializeComponent();
            DataContext = new ViewModelPageAnaData();
        }
    }
}
