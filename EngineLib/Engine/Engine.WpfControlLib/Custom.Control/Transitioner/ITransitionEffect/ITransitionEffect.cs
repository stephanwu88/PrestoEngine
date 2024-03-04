using System.Windows;
using System.Windows.Media.Animation;

namespace Engine.WpfControl
{
    public interface ITransitionEffect
    {
        Timeline Build<TSubject>(TSubject effectSubject) where TSubject : FrameworkElement, ITransitionEffectSubject;
    }
}