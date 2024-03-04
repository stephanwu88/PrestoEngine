using System.Windows;
using System.Windows.Controls;

namespace Engine.WpfControl
{
    public class ToggleExpander : ListBox
    {
        static ToggleExpander()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ToggleExpander), new FrameworkPropertyMetadata(typeof(ToggleExpander)));  
        }

        //public bool ToggleButtonIsHitVisible
        //{
        //    get { return (bool)GetValue(ToggleButtonIsHitVisibleProperty); }
        //    set { SetValue(ToggleButtonIsHitVisibleProperty, value); }
        //}

        //// Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty ToggleButtonIsHitVisibleProperty =
        //    DependencyProperty.Register("ToggleButtonIsHitVisible", typeof(bool), typeof(ToggleExpander), new PropertyMetadata(true, (d, e) =>
        //     {
        //         ToggleExpander control = d as ToggleExpander;

        //         if (control == null) return;

        //         //bool config = e.NewValue as bool;

        //     }));



        public bool IsExpanded
        {
            get { return (bool)GetValue(IsExpandedProperty); }
            set { SetValue(IsExpandedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsExpandedProperty =
            DependencyProperty.Register("IsExpanded", typeof(bool), typeof(ToggleExpander), new PropertyMetadata(default(bool), (d, e) =>
             {
                 ToggleExpander control = d as ToggleExpander;

                 if (control == null) return;

                 //bool config = e.NewValue as bool;

             }));


    }

    public class ElementExpander: ToggleExpander
    { 
        public object Content
        {
            get { return (object)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register("Content", typeof(object), typeof(ElementExpander), new PropertyMetadata(default(object), (d, e) =>
             {
                 ElementExpander control = d as ElementExpander;

                 if (control == null) return;

                 object config = e.NewValue as object;

             }));

    }
}
