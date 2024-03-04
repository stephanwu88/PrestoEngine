using System.Windows;
using System.Windows.Controls;

namespace Engine.WpfControl
{
    /// <summary> 自定义导航框架 </summary>

    public class MvcEditFrame : ContentControl
    {
        static MvcEditFrame()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MvcEditFrame), new FrameworkPropertyMetadata(typeof(MvcEditFrame)));
        }



        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }


    }

}
