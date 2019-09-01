using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace Gears.ViewModels
{
    class SimpleCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;
        public Action<object> TodoAntion;

        public SimpleCommand(Action<object> action)
        {
            TodoAntion = action;
        }

        public bool CanExecute(object parameter)
        {
            return TodoAntion != null;
        }

        public void Execute(object parameter)
        {
            TodoAntion(parameter);
        }
    }
}
