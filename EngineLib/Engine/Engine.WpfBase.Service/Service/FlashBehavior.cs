﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Engine.WpfBase;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Engine.WpfBase
{
    /// <summary> 动画帧 闪烁效果</summary>
    public class FlashBehavior : Behavior<FrameworkElement>
    {
        protected override void OnAttached()
        {
            AssociatedObject.Loaded += AssociatedObject_Loaded;
        }

        private void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            if (IsUseAll)
            {
                var items = AssociatedObject.GetChildren<UIElement>().Where(l => l.RenderTransform is TransformGroup);

                items = items.Where(l => (l.RenderTransform as TransformGroup).Children.Count == 4);

                this.RefreshAnimation(items);

            }
            else
            {
                if (AssociatedObject is Panel panel)
                {
                    var items = panel.Children?.Cast<UIElement>()?.Where(l => l.RenderTransform is TransformGroup);

                    items = items.Where(l => (l.RenderTransform as TransformGroup).Children.Count == 4);

                    this.RefreshAnimation(items);
                }

            }
        }

        void RefreshAnimation(IEnumerable<UIElement> items)
        {
            items = items.Where(l => (l.RenderTransform as TransformGroup).Children.Count == 4);

            var controls = items?.ToList();

            if (controls == null || controls.Count == 0) return;

            Storyboard storyboard = new Storyboard();

            for (int i = 0; i < controls.Count; i++)
            {
                foreach (var item in Timelines.OfType<DoubleAnimation>())
                {
                    TimeSpan span = TimeSpan.FromMilliseconds(i * (SplitMilliSecond + item.Duration.TimeSpan.TotalMilliseconds));

                    TimeSpan end = item.Duration.TimeSpan + span;

                    if (item.BeginTime == TimeSpan.Zero)
                    {
                        DoubleAnimationUsingKeyFrames frames = new DoubleAnimationUsingKeyFrames();

                        EasingDoubleKeyFrame key1 = new EasingDoubleKeyFrame(item.From.Value, KeyTime.FromTimeSpan(TimeSpan.Zero));

                        frames.KeyFrames.Add(key1);
                        Storyboard.SetTarget(frames, controls[i]);
                        Storyboard.SetTargetProperty(frames, Storyboard.GetTargetProperty(item));
                        storyboard.Children.Add(frames);
                    }

                    DoubleAnimation animation = item.Clone();
                    animation.BeginTime = span + animation.BeginTime;
                    Storyboard.SetTarget(animation, controls[i]);

                    storyboard.Children.Add(animation);
                }

                foreach (var item in Timelines.OfType<ThicknessAnimation>())
                {
                    TimeSpan span = TimeSpan.FromMilliseconds(i * (SplitMilliSecond + item.Duration.TimeSpan.TotalMilliseconds));

                    TimeSpan end = item.Duration.TimeSpan + span;

                    ThicknessAnimationUsingKeyFrames frames = new ThicknessAnimationUsingKeyFrames();

                    EasingThicknessKeyFrame key1 = new EasingThicknessKeyFrame(item.From.Value, KeyTime.FromTimeSpan(TimeSpan.Zero));

                    frames.KeyFrames.Add(key1);
                    Storyboard.SetTarget(frames, controls[i]);
                    Storyboard.SetTargetProperty(frames, Storyboard.GetTargetProperty(item));
                    storyboard.Children.Add(frames);

                    ThicknessAnimation animation = item.Clone();
                    animation.BeginTime = span;
                    Storyboard.SetTarget(animation, controls[i]);

                    storyboard.Children.Add(animation);
                }
            }

            storyboard.FillBehavior = FillBehavior.HoldEnd;
            storyboard.RepeatBehavior = this.RepeatBehavior;
            storyboard.Begin();
        }

        protected override void OnDetaching()
        {
            AssociatedObject.Loaded -= AssociatedObject_Loaded;
        }


        public RepeatBehavior RepeatBehavior
        {
            get { return (RepeatBehavior)GetValue(RepeatBehaviorProperty); }
            set { SetValue(RepeatBehaviorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RepeatBehaviorProperty =
            DependencyProperty.Register("RepeatBehavior", typeof(RepeatBehavior), typeof(FlashBehavior), new PropertyMetadata(default(RepeatBehavior), (d, e) =>
             {
                 FlashBehavior control = d as FlashBehavior;

                 if (control == null) return;

                 //RepeatBehavior config = e.NewValue as RepeatBehavior;

             }));


        public bool IsUseAll
        {
            get { return (bool)GetValue(IsUseAllProperty); }
            set { SetValue(IsUseAllProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsUseAllProperty =
            DependencyProperty.Register("IsUseAll", typeof(bool), typeof(FlashBehavior), new PropertyMetadata(false, (d, e) =>
            {
                FlashBehavior control = d as FlashBehavior;

                if (control == null) return;
            }));

        public double Duration
        {
            get { return (double)GetValue(DurationProperty); }
            set { SetValue(DurationProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DurationProperty =
            DependencyProperty.Register("Duration", typeof(double), typeof(FlashBehavior), new PropertyMetadata(200.0, (d, e) =>
            {
                FlashBehavior control = d as FlashBehavior;

                if (control == null) return;

                //double config = e.NewValue as double;

            }));


        public bool IsAnimation
        {
            get { return (bool)GetValue(IsAnimationProperty); }
            set { SetValue(IsAnimationProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsAnimationProperty =
            DependencyProperty.Register("IsAnimation", typeof(bool), typeof(FlashBehavior), new PropertyMetadata(true, (d, e) =>
            {
                FlashBehavior control = d as FlashBehavior;

                if (control == null) return;

                //bool config = e.NewValue as bool;

            }));

        public ArrayList Timelines
        {
            get { return (ArrayList)GetValue(TimelinesProperty); }
            set { SetValue(TimelinesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TimelinesProperty =
            DependencyProperty.Register("Timelines", typeof(ArrayList), typeof(FlashBehavior), new PropertyMetadata(new ArrayList(), (d, e) =>
            {
                FlashBehavior control = d as FlashBehavior;

                if (control == null) return;

                ArrayList config = e.NewValue as ArrayList;

            }));

        public double SplitMilliSecond
        {
            get { return (double)GetValue(SplitMilliSecondProperty); }
            set { SetValue(SplitMilliSecondProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SplitMilliSecondProperty =
            DependencyProperty.Register("SplitMilliSecond", typeof(double), typeof(FlashBehavior), new PropertyMetadata(5.0, (d, e) =>
            {
                FlashBehavior control = d as FlashBehavior;

                if (control == null) return;
            }));

    }
}