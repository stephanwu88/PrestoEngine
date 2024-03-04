namespace Engine.WpfControl
{
    public interface IZIndexController
    {
        void Stack(params TransitionerSlide[] highestToLowest);
    }
}