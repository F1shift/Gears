using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using System.Timers;

namespace Gears.ViewModels
{
    class SimpleCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;
        public Action<object> TodoAntion;

        public bool IsRunning { get; set; }
        public Action RemaingTask { get; set; }
        /// <summary>
        /// Min interval between two excution in millisecond.
        /// </summary>
        public uint MinInterval { get; set; } = 0;
        DateTime lastRunnedTime;

        public SimpleCommand(Action<object> action)
        {
            TodoAntion = action;
        }

        public bool CanExecute(object parameter)
        {
            return TodoAntion != null && !IsRunning && !IsInInterval();
        }

        public bool IsInInterval() {
            return (DateTime.Now - lastRunnedTime).TotalMilliseconds < MinInterval;
        }

        public void Execute(object parameter)
        {
            if (TodoAntion != null && !IsRunning && !IsInInterval())
            {
                IsRunning = true;
                CanExecuteChanged?.Invoke(this, new EventArgs());
                lastRunnedTime = DateTime.Now;
                Timer timer = new Timer();
                if (MinInterval > 0)
                {
                    timer.Interval = MinInterval;
                    timer.Elapsed += (o, e) => {
                        if (!IsRunning)
                        {
                            CanExecuteChanged?.Invoke(this, new EventArgs());
                            RemaingTask?.Invoke();
                        }
                        timer.Stop();
                    };
                    timer.Start();
                }
                TodoAntion?.Invoke(parameter);
                IsRunning = false;
                RemaingTask?.Invoke();
                if (!IsInInterval())
                {
                    CanExecuteChanged?.Invoke(this, new EventArgs());
                }
            }
            else
            {
                RemaingTask = () => {
                    Execute(parameter);
                    RemaingTask = null;
                };
            }
        }

        public void ForceExecute(object parameter) {
            TodoAntion?.Invoke(parameter);
        }
    }
}
