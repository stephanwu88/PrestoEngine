﻿using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Engine.WpfBase
{
    public interface IPresenterMessage
    {
        Task<bool> Show<T>(T value, Predicate<T> match = null, string title = null, Action<ContentControl> builder = null, ComponentResourceKey key = null);
        Task<bool> ShowClearly<T>(T value, Predicate<T> match = null, string title = null, Action<ContentControl> builder = null);
        Task<bool> ShowClose<T>(T value, Predicate<T> match = null, string title = null, Action<ContentControl> builder = null);
        Task<bool> ShowLeftClose<T>(T value, Predicate<T> match = null, string title = null, Action<ContentControl> builder = null);
        Task<bool> ShowRightClose<T>(T value, Predicate<T> match = null, string title = null, Action<ContentControl> builder = null);
        Task<bool> ShowTopClose<T>(T value, Predicate<T> match = null, string title = null, Action<ContentControl> builder = null);
        Task<bool> ShowBottomClose<T>(T value, Predicate<T> match = null, string title = null, Action<ContentControl> builder = null);
    }
}
