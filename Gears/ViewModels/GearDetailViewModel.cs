using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Gears.ViewModels
{
    class GearDetailViewModel : INotifyPropertyChanged
    {
        public RackParameterViewModel RackParameterViewModel { get; set; }
        public GearParameterViewModel GearParameterViewModel { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;


        SimpleCommand _UpdateConnamd;
        public SimpleCommand UpdateConnamd
        {
            get
            {
                return _UpdateConnamd;
            }
            set
            {
                if (_UpdateConnamd != value)
                {
                    _UpdateConnamd = value;
                    PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(nameof(UpdateConnamd)));
                }
            }
        }

        public GearDetailViewModel()
        {
            UpdateConnamd = new SimpleCommand(Update);
        }

        public void Update(object para) {

        }
    }
}
