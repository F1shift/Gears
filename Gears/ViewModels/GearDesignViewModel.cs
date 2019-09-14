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

        public GearParameterViewModel GearParameterViewModel { get; set; }

        public RackParameterViewModel RackParameterViewModel { get; set; }

        public GearDetailViewModel GearDetailViewModel { get; set; }

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
