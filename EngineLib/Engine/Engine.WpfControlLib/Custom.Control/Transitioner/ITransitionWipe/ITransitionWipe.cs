using System.Windows;

namespace Engine.WpfControl
{
    /// <summary> ����Ч�� </summary>
    public interface ITransitionWipe
    {
        void Wipe(TransitionerSlide fromSlide, TransitionerSlide toSlide, Point origin, IZIndexController zIndexController);
    }
}