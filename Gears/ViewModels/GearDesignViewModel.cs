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

        GearParameterViewModel _GearParameterViewModel;
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

        RackParameterViewModel _RackParameterViewModel;
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

        GearDetailViewModel _GearDetailViewModel;
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

        public GearDesignViewModel()
        {
            GearParameterViewModel = new GearParameterViewModel();
            RackParameterViewModel = new RackParameterViewModel();
            GearDetailViewModel = new GearDetailViewModel();
            GearDetailViewModel.GearParameterViewModel = this.GearParameterViewModel;
            GearDetailViewModel.RackParameterViewModel = this.RackParameterViewModel;
        }
    }
}
