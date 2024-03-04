﻿using System;
using System.Windows;
using System.Windows.Input;

namespace Engine.WpfBase
{
    public class VisibleOfOpacityCommand : ICommand
    {  
        public bool CanExecute(object parameter)
        { 
            return true;
        }

        public void Execute(object parameter)
        {
            if (parameter is UIElement element)
            {
                element.Visibility = Visibility.Visible;

                var engine = DoubleStoryboardEngine.Create(0, 1, 0.4, UIElement.OpacityProperty.Name);

                engine.Start(element);
            }
        }

        public event EventHandler CanExecuteChanged;
    }
}