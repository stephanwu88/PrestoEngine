using System;
using System.Windows.Input;

namespace Engine.MVVM
{
    public class MyCommand : ICommand
    {
        /// <summary>
        /// 判断命令是否可以执行的方法
        /// </summary>
        private Func<object, bool> _canExecute;

        /// <summary>
        /// 命令需要执行的方法
        /// </summary>
        private Action<object> _execute;
        private delegate void Task();

        /// <summary>
        /// 检查命令是否可以执行的事件，在UI事件发生导致控件状态或数据发生变化时触发
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add
            {
                if (_canExecute != null)
                {
                    CommandManager.RequerySuggested += value;
                }
            }
            remove
            {
                if (_canExecute != null)
                {
                    CommandManager.RequerySuggested -= value;
                }
            }
        }

        /// <summary>
        /// 创建一个命令
        /// </summary>
        /// <param name="execute">命令要执行的方法</param>
        public MyCommand(Action<object> execute) : this(execute, null)
        {

        }

        /// <summary>
        /// 创建一个命令
        /// </summary>
        /// <param name="execute">命令要执行的方法</param>
        /// <param name="canExecute">判断命令是否能够执行的方法</param>
        public MyCommand(Action<object> execute, Func<object, bool> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        /// <summary>
        /// 判断命令是否可以执行
        /// </summary>
        /// <param name="parameter">命令传入的参数</param>
        /// <returns>是否可以执行</returns>
        public bool CanExecute(object parameter)
        {
            if (_canExecute == null) return true;
            return _canExecute(parameter);
        }

        /// <summary>
        /// 执行命令
        /// </summary>
        /// <param name="parameter"></param>
        public void Execute(object parameter)
        {
            if (_execute != null && CanExecute(parameter))
            {
                _execute(parameter);
            }
        }
    }

    public class SelectionChangedCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Predicate<object> _canExecute;

        public SelectionChangedCommand(Action<object> execute)
            : this(execute, null)
        {
        }

        public SelectionChangedCommand(Action<object> execute,
            Predicate<object> canExecute)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}
