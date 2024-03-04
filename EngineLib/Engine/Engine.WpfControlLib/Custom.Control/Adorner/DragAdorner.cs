﻿using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace Engine.WpfControl
{
    public class DragAdorner : Adorner
    {
        public DragAdorner(UIElement adornedElement, Point offset) : base(adornedElement)
        {
            this.Offset = offset;
            vbrush = new VisualBrush(AdornedElement);
            vbrush.Opacity = .5;
        }

        public void UpdatePosition(Point location)
        {
            this.location = location;

            this.InvalidateVisual();
        }

        protected override void OnRender(DrawingContext dc)
        {
            var p = location;

            if (this.DropAdornerMode == DrapAdornerMode.OnlyY)
            {
                p = new Point(0, p.Y);
                p.Offset(0, -Offset.Y);
            }
            else if (this.DropAdornerMode == DrapAdornerMode.OnlyX)
            {
                p = new Point(p.X, 0);
                p.Offset(-Offset.X, 0);
            }
            else
            {
                p.Offset(-Offset.X, -Offset.Y);
            }

            dc.DrawRectangle(vbrush, null, new Rect(p, this.RenderSize));
        }



        private Brush vbrush;

        private Point location;

        /// <summary> 相对于拖动控件的拖动位置 </summary>
        public Point Offset { get; set; }

        public DrapAdornerMode DropAdornerMode { get; set; }
    }

    public enum DrapAdornerMode
    {
        Both = 0, OnlyX, OnlyY,
    }
}
