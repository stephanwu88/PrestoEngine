using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Engine.WpfBase
{
    public interface IContainerMessage
    {
        MessageCloseLayerCommand CloseLayerCommand { get; }
        void Show(FrameworkElement element, Predicate<Panel> predicate = null);
        void Close();
    }

    public class MessageCloseLayerCommand : MarkupCommandBase
    {
        public override void Execute(object parameter)
        {
            MessageProxy.Container.Close();
        }
    }
}
