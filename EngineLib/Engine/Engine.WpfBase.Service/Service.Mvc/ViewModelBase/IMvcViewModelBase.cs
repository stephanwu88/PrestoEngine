using System.Collections.ObjectModel;
using Engine.MVVM;

namespace Engine.WpfBase
{
    public interface IMvcViewModelBase
    {
        MyCommand<string> DoActionCommand { get; }
        MyCommand<string> GoToActionCommand { get; }
        MyCommand<ILinkActionBase> GoToLinkCommand { get; }
        MyCommand<string> LoadedCommand { get; }
        ObservableCollection<ILinkActionBase> Navigation { get; set; }
        ILinkActionBase SelectLink { get; set; }
    }
}