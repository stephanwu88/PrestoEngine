using System.Windows.Controls;

namespace Engine.Automation.Sparker
{
    /// <summary>
    /// PageInsConfig.xaml 的交互逻辑
    /// </summary>
    public partial class PageOblfMain : UserControl
    {
        private bool winIsLoaded = false;
        private string InsName;
        public PageOblfMain(string insName)
        {
            InitializeComponent();
            InsName = insName;
            //Loaded += (s, e) =>
            {
                if (winIsLoaded)
                    return;
                winIsLoaded = true;
                DataContext = new ViewModelPageConfigMain(insName); 
            };
        }
    }
}
