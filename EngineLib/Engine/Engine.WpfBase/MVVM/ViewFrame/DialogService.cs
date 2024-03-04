using System;
using System.Threading.Tasks;
using System.Windows;

namespace Engine.MVVM
{
    public interface IDialogService
    {
        Task OpenWindow<T>(object dataContext = null) where T : Window;
    }

    public class DialogService : IDialogService
    {
        public async Task OpenWindow<T>(object dataContext = null) where T : Window
        {
            var window = Activator.CreateInstance<T>();
            if (dataContext != null)
            {
                window.DataContext = dataContext;
            }
            await window.Dispatcher.InvokeAsync(() => window.ShowDialog());
            await Task.Delay(1);
        }
    }
}
