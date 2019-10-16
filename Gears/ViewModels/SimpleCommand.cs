using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Timers;

namespace Gears.ViewModels
{
    class SimpleCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;
        Func<object, Task<object>> _TodoAntion;
        public Func<object, Task<object>> TodoAntion
        {
            get
            {
                return _TodoAntion;
            }
            set
            {
                if (_TodoAntion != value)
                {
                    _TodoAntion = value;
                }
            }
        }

        public bool IsRunning { get; set; }
        Action remaingTask;
        /// <summary>
        /// Min interval between two excution in millisecond.
        /// </summary>
        public uint MinInterval { get; set; } = 0;
        DateTime lastRunnedTime;

        public SimpleCommand(Func<object, Task<object>> action)
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

        public async void Execute(object parameter)
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
                            remaingTask?.Invoke();
                        }
                        timer.Stop();
                    };
                    timer.Start();
                }
                await TodoAntion?.Invoke(parameter);
                IsRunning = false;
                remaingTask?.Invoke();
                if (!IsInInterval())
                {
                    CanExecuteChanged?.Invoke(this, new EventArgs());
                }
            }
            else
            {
                remaingTask = () => {
                    Execute(parameter);
                    remaingTask = null;
                };
            }
        }

        public void ForceExecute(object parameter) {
            TodoAntion?.Invoke(parameter);
        }
    }
}
