﻿// Copyright (c) Microsoft. All rights reserved. 
// Licensed under the MIT license. See LICENSE file in the project root for full license information. 
namespace Engine.WpfBase
{
    using System;
    using System.Diagnostics;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Input;
    using System.Globalization;
    using System.Windows.Data;
    using System.Linq;
    using System.Windows.Controls;
    using System.Windows.Media;

    /// <summary> 执行气泡加载效果 </summary>
    public sealed class InvokeFountainAnimationAction : TriggerAction<DependencyObject>
    {
        /// <summary> 绑定的容器对象 </summary>
        public DependencyObject Target
        {
            get { return (DependencyObject)GetValue(TargetProperty); }
            set { SetValue(TargetProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TargetProperty =
            DependencyProperty.Register("Target", typeof(DependencyObject), typeof(InvokeFountainAnimationAction), new PropertyMetadata(default(DependencyObject), (d, e) =>
             {
                 InvokeFountainAnimationAction control = d as InvokeFountainAnimationAction;

                 if (control == null) return;

                 DependencyObject config = e.NewValue as DependencyObject;

             }));

        /// <summary> 是否遍历子集执行动画 </summary>
        public bool IsUseAll
        {
            get { return (bool)GetValue(IsUseAllProperty); }
            set { SetValue(IsUseAllProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsUseAllProperty =
            DependencyProperty.Register("IsUseAll", typeof(bool), typeof(InvokeFountainAnimationAction), new PropertyMetadata(false, (d, e) =>
            {
                InvokeFountainAnimationAction control = d as InvokeFountainAnimationAction;

                if (control == null) return;
            }));

        /// <summary> 左右随机范围 </summary>
        public double HorizontalRange
        {
            get { return (double)GetValue(HorizontalRangeProperty); }
            set { SetValue(HorizontalRangeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HorizontalRangeProperty =
            DependencyProperty.Register("HorizontalRange", typeof(double), typeof(InvokeFountainAnimationAction), new PropertyMetadata(-500.0, (d, e) =>
             {
                 InvokeFountainAnimationAction control = d as InvokeFountainAnimationAction;

                 if (control == null) return;

                 //double config = e.NewValue as double;

             }));

        /// <summary> 上下随机范围 </summary>
        public double VerticalRange
        {
            get { return (double)GetValue(VerticalRangeProperty); }
            set { SetValue(VerticalRangeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VerticalRangeProperty =
            DependencyProperty.Register("VerticalRange", typeof(double), typeof(InvokeFountainAnimationAction), new PropertyMetadata(500.0, (d, e) =>
             {
                 InvokeFountainAnimationAction control = d as InvokeFountainAnimationAction;

                 if (control == null) return;

                 //double config = e.NewValue as double;

             }));

        /// <summary> 放大倍数 </summary>
        public double Mul
        {
            get { return (double)GetValue(MulProperty); }
            set { SetValue(MulProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MulProperty =
            DependencyProperty.Register("Mul", typeof(double), typeof(InvokeFountainAnimationAction), new PropertyMetadata(10.0, (d, e) =>
            {
                InvokeFountainAnimationAction control = d as InvokeFountainAnimationAction;

                if (control == null) return;
            }));

        /// <summary> 放大时间点 </summary>
        public double MiddleValue
        {
            get { return (double)GetValue(MiddleValueProperty); }
            set { SetValue(MiddleValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MiddleValueProperty =
            DependencyProperty.Register("MiddleValue", typeof(double), typeof(InvokeFountainAnimationAction), new PropertyMetadata(0.1, (d, e) =>
            {
                InvokeFountainAnimationAction control = d as InvokeFountainAnimationAction;

                if (control == null) return;
            }));

        /// <summary> 还原时间点 </summary>
        public double EndValue
        {
            get { return (double)GetValue(EndValueProperty); }
            set { SetValue(EndValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EndValueProperty =
            DependencyProperty.Register("EndValue", typeof(double), typeof(InvokeFountainAnimationAction), new PropertyMetadata(1.0, (d, e) =>
            {
                InvokeFountainAnimationAction control = d as InvokeFountainAnimationAction;

                if (control == null) return;
            }));

        /// <summary> 时间间隔 </summary>
        public double Split
        {
            get { return (double)GetValue(SplitProperty); }
            set { SetValue(SplitProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SplitProperty =
            DependencyProperty.Register("Split", typeof(double), typeof(InvokeFountainAnimationAction), new PropertyMetadata(0.05, (d, e) =>
            {
                InvokeFountainAnimationAction control = d as InvokeFountainAnimationAction;

                if (control == null) return;
            }));

        /// <summary> 执行方法 </summary>
        protected override void Invoke(object parameter)
        {
            if (this.AssociatedObject == null) return;

            if (this.Target == null) return;

            if (IsUseAll)
            {

                var items = this.Target.GetChildren<UIElement>().Where(l => l.RenderTransform is TransformGroup);

                items = items.Where(l => (l.RenderTransform as TransformGroup).Children.Count == 4);

                items = items.Where(l => (l as FrameworkElement)?.Tag?.ToString() != "Except");

                StoryBoardService.FountainAnimation(items, (int)this.HorizontalRange, (int)this.VerticalRange, Mul, MiddleValue, EndValue, Split);
            }
            else
            {
                if (this.Target is Panel panel)
                {
                    var items = panel.Children?.Cast<UIElement>()?.Where(l => l.RenderTransform is TransformGroup);

                    items = items.Where(l => (l.RenderTransform as TransformGroup).Children.Count == 4);

                    StoryBoardService.FountainAnimation(items, (int)this.HorizontalRange, (int)this.VerticalRange, Mul, MiddleValue, EndValue, Split);
                }

            }
        }
    }
}
