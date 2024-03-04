using System;
using System.Collections;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Engine.WpfBase
{
    public interface IAutoColumnPagedDataGridMessage
    {
        Task<bool> Show<T>(T value, Predicate<T> match = null, string title = null, Action<DataGrid> builder = null, ComponentResourceKey key = null) where T : IList;
    }
}
