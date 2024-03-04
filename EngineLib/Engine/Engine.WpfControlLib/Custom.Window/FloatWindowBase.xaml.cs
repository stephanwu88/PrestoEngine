﻿using Engine.WpfBase;
using System;
using System.Collections.Generic;
using System.Windows;

namespace Engine.WpfControl
{
    /// <summary> 浮动的窗口 </summary>
    public partial class FloatWindowBase : WindowBase
    {

        public FloatWindowBase()
        {
            //this.ShowAnimation = l =>
            //{
            //    var engine = DoubleStoryboardEngine.Create(this.Height, 0, 0.5, "(UIElement.RenderTransform).(TransformGroup.Children)[3].(TranslateTransform.Y)");
            //    engine.Start(l);
            //};

            //this.CloseAnimation = l =>
            //{
            //    l.RenderTransformOrigin = new Point(0.5, 0.5);

            //    var engine = DoubleStoryboardEngine.Create(0, this.Height, 0.5, "(UIElement.RenderTransform).(TransformGroup.Children)[3].(TranslateTransform.Y)");

            //    engine.CompletedEvent += (s, e) =>
            //    {
            //        l.Close();
            //    };
            //    engine.Start(l);
            //};

            this.BindCommand(CommandService.Close, (l, k) =>
            {
                this.CloseAnimation?.Invoke(this);
            });

            //this.WindowStartupLocation = WindowStartupLocation.Manual;

            this.MouseLeftButtonDown += (l, k) =>
              {
                  try
                  {
                      this.DragMove();
                  }
                  catch
                  {

                  }
              };

            this.WindowStartupLocation = WindowStartupLocation.Manual;

            this.Loaded += (l, k) =>
            {
                this.Width = SystemParameters.WorkArea.Width;

                this.Height = SystemParameters.WorkArea.Height;

                this.Top = 0;

                this.Left = 0;
            };
        }

        public IEnumerable<object> Source
        {
            get { return (IEnumerable<object>)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(IEnumerable<object>), typeof(FloatWindowBase), new PropertyMetadata(default(IEnumerable<object>), (d, e) =>
            {
                FloatWindowBase control = d as FloatWindowBase;

                if (control == null) return;

                IEnumerable<object> config = e.NewValue as IEnumerable<object>;

            }));
    }


    /// <summary> 带图标和操作的实体 </summary>
    public class IconFloatModel
    {
        public string Icon { get; set; }

        public string DisplayName { get; set; }

        public Action Action { get; set; }

        public void Do()
        {
            this.Action?.Invoke();
        }
    }
}
