using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Engine.WpfControl
{
    public class SlideOutScale : ITransitionWipe
    {
        private readonly SineEase _sineEase = new SineEase();

        public void Wipe(TransitionerSlide fromSlide, TransitionerSlide toSlide, Point origin, IZIndexController zIndexController)
        {
            if (fromSlide == null) throw new ArgumentNullException(nameof(fromSlide));
            if (toSlide == null) throw new ArgumentNullException(nameof(toSlide));
            if (zIndexController == null) throw new ArgumentNullException(nameof(zIndexController));


            fromSlide.Opacity = 0;
            toSlide.Opacity = 1;

            var zeroKeyTime = KeyTime.FromTimeSpan(TimeSpan.Zero);
            var endKeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(400));


            //slide in new slide setup
            var translateTransform = new ScaleTransform(0.3, 0.3);
            toSlide.RenderTransform = translateTransform; 
            var slideAnimation = new DoubleAnimationUsingKeyFrames();
            slideAnimation.KeyFrames.Add(new LinearDoubleKeyFrame(0.3, zeroKeyTime));
            slideAnimation.KeyFrames.Add(new EasingDoubleKeyFrame(1, endKeyTime) { EasingFunction = _sineEase });

            //kick off!
            translateTransform.BeginAnimation(ScaleTransform.ScaleXProperty, slideAnimation);
            translateTransform.BeginAnimation(ScaleTransform.ScaleYProperty, slideAnimation);

            zIndexController.Stack(toSlide, fromSlide);
        }
    }
}