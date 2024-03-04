using System.Collections.ObjectModel;

namespace Engine.WpfBase
{
    public interface IMvcEntityViewModelBase<M> where M : new()
    {
        M AddItem { get; set; }
        ObservableCollection<M> Collection { get; set; }
        M SelectedItem { get; set; }
    }
}