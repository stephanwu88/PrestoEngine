using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Engine.MVVM
{
    public class PrsMenuItem
    {
        public string Type { get; set; }
        public ObservableCollection<PrsMenuItem> SubMenu { get; set; }
        public string Icon { get; set; }
        public string Name { get; set; }
        public string Label { get; set; }
        public string PageID { get; set; }
        public object Page { get; set; }
        public ICommand Command { get; set; }
        public string CommandName { get; set; }
    }
}
