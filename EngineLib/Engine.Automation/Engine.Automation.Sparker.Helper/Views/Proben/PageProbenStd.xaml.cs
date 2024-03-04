using System.Windows.Controls;

namespace Engine.Automation.Sparker
{
    /// <summary>
    /// PageProbenStd.xaml 的交互逻辑
    /// </summary>
    public partial class PageProbenStd : UserControl
    {
        public PageProbenStd()
        {
            InitializeComponent();
            DataContext = new ViewModelProbenStd();
        }
    }
}
