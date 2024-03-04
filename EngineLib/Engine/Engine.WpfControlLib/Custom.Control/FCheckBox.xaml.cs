using System;
using System.Windows;
using System.Windows.Controls;

namespace Engine.WpfControl
{
    /// <summary>
    /// FCheckBox.xaml 的交互逻辑
    /// </summary>
    [Obsolete("废弃不用了  直接用 CheckBox")]
    public partial class FCheckBox : CheckBox
    {
        static FCheckBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FCheckBox), new FrameworkPropertyMetadata(typeof(FCheckBox)));
        }
    }
}
