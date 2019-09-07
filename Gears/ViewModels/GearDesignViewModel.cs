using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using Gears.ViewModels;

namespace Gears.ViewModels
{
    class GearDesignViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        GearParameterViewModel _GearParameterViewModel = new GearParameterViewModel();
        public GearParameterViewModel GearParameterViewModel
        {
            get
            {
                return _GearParameterViewModel;
            }
            set
            {
                if (_GearParameterViewModel != value)
                {
                    _GearParameterViewModel = value;
                    PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(nameof(GearParameterViewModel)));
                }
            }
        }

        RackParameterViewModel _RackParameterViewModel = new RackParameterViewModel();
        public RackParameterViewModel RackParameterViewModel
        {
            get
            {
                return _RackParameterViewModel;
            }
            set
            {
                if (_RackParameterViewModel != value)
                {
                    _RackParameterViewModel = value;
                    PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(nameof(RackParameterViewModel)));
                }
            }
        }

        GearDetailViewModel _GearDetailViewModel = new GearDetailViewModel();
        public GearDetailViewModel GearDetailViewModel
        {
            get
            {
                return _GearDetailViewModel;
            }
            set
            {
                if (_GearDetailViewModel != value)
                {
                    _GearDetailViewModel = value;
                    PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(nameof(GearDetailViewModel)));
                }
            }
        }
    }
}
