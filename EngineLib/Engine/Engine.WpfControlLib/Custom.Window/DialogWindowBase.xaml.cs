using Engine.WpfBase;
using System.Windows.Input;

namespace Engine.WpfControl
{
    public partial class DialogWindowBase : WindowBase
    {
        public DialogWindowBase()
        {
            //this.ShowAnimation = l =>
            //{
            //    //l.RenderTransformOrigin = new Point(0.5, 0.5);

            //    //var engine2 = DoubleStoryboardEngine.Create(0.5, 1, 0.5, UIElement.OpacityProperty.Name);
            //    //var engine = DoubleStoryboardEngine.Create(0.1, 0.96, 0.3, "(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)");
            //    //var engine1 = DoubleStoryboardEngine.Create(0.1, 0.96, 0.3, "(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleX)");

            //    //engine.Start(l);
            //    //engine1.Start(l);
            //    //engine2.Start(l);

            //    Cattach.SetIsClose(this, true);

            //};

            //this.CloseAnimation = l =>
            //{
            //    //l.RenderTransformOrigin = new Point(0.5, 0.5);

            //    //var engine2 = DoubleStoryboardEngine.Create(1, 0.5, 0.3, "Opacity");
            //    //var engine = DoubleStoryboardEngine.Create(1, 0.1, 0.3, "(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)");
            //    //var engine1 = DoubleStoryboardEngine.Create(1, 0.1, 0.3, "(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleX)");

            //    //engine.CompletedEvent += (s, e) =>
            //    //{
            //    //    this.MouseDown -= DialogWindow_MouseDown;
            //    //    l.Close();
            //    //};

            //    //engine.Start(l);
            //    //engine1.Start(l);
            //    //engine2.Start(l);

            //    Cattach.SetIsClose(this, false);
            //};


            this.BindCommand(CommandService.Close, (l, k) =>
            {
                this.CloseAnimation?.Invoke(this);
            });

            this.MouseDown += DialogWindow_MouseDown;
        }

        private void DialogWindow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
