using System.Windows;

namespace Engine.Automation.Sparker
{
    /// <summary>
    /// winDeviationSet.xaml 的交互逻辑
    /// </summary>
    public partial class winDeviationSet : Window
    {
        public winDeviationSet()
        {
            InitializeComponent();

            _GamaExpress.Items.Clear();
            _GamaExpress.Items.Add("");
            _GamaExpress.Items.Add("lgr = A * lgm - B");
            _GamaExpress.Items.Add("r = A * m + B");
        }
    }
}
