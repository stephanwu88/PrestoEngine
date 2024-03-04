﻿using System;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Shell;

namespace Engine.WpfBase
{
    public interface ITaskBarMessage
    {
        void Show(Action<TaskbarItemInfo> action);
        void ShowImage(ImageSource image);
        void ShowNormal(Action<TaskbarItemInfo> action);
        Task ShowPercent(Action<TaskbarItemInfo> action);
        Task<bool> ShowWaitting(Func<bool> action);
    }
}