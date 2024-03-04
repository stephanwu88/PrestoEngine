using System.Windows.Controls;

namespace Engine.Automation.Sparker
{
    /// <summary>
    /// PageMaterial.xaml 的交互逻辑
    /// </summary>
    public partial class PageMaterial : UserControl
    {
        public PageMaterial()
        {
            InitializeComponent();
            DataContext = new ViewModelMaterial() { Page = this };
        }
    }
}
