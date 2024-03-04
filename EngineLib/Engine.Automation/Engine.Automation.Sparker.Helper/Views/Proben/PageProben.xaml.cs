using Engine.Common;
using System.Windows;
using System.Windows.Controls;

namespace Engine.Automation.Sparker
{
    /// <summary>
    /// PageProben.xaml 的交互逻辑
    /// </summary>
    public partial class PageProben : UserControl
    {
        public PageProben()
        {
            InitializeComponent();
            DataContext = new ViewModelPageProben() { Owner = this.FindParent<Window>() };
        }
    }
}
