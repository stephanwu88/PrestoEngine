﻿using HeBianGu.General.WpfControlLib;
using System;
using System.Windows;
using System.Windows.Input;

namespace Engine.WpfBase
{
    public class MessageLayerCloseCommand : ICommand
    {
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            //int index = parameter == null ? 0 : int.Parse(parameter.ToString());
            MessageService.CloseLayer();
        }

        public event EventHandler CanExecuteChanged;
    }
}