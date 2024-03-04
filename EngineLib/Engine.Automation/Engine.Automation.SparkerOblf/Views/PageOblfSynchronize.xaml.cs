using System.Windows.Controls;

namespace Engine.Automation.OBLF
{
    /// <summary>
    /// PageOblfSynchronize.xaml 的交互逻辑
    /// </summary>
    public partial class PageOblfSynchronize : UserControl
    {
        public string InsName { get; set; }
        bool winIsLoaded = false;
        public PageOblfSynchronize()
        {
            InitializeComponent();
            Loaded += (s,e)=>
            {
                if (winIsLoaded)
                    return;
                winIsLoaded = true;
                DataContext = new ViewModelPageOblfSynchronize(InsName); 
            };
        }
    }
}
