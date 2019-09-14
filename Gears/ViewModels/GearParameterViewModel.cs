using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.Linq;
using Gears.DataBases;
using Gears.Models;
using Xamarin.Forms;

namespace Gears.ViewModels
{
    class GearParameterViewModel : System.ComponentModel.INotifyPropertyChanged
    {
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

       
        public ObservableCollection<InputItemViewModel> InputItems { get; set; }

        public GearParameterViewModel()
        {
            Init();
        }

        public void Init() {
            InputItems = new ObservableCollection<InputItemViewModel>() {
                new InputItemViewModel(){ Name = "歯数１", Value = 12, Min = 6.0, Max = 200.0, Step = 1  },
                new InputItemViewModel(){ Name = "歯数２", Value = 60, Min = 6.0, Max = 200.0,  Step = 1 },
                new InputItemViewModel(){ Name = "ねじれ角", Value = 30, Min = -45.0, Max = 45.0,  Step = 1  },
                new InputItemViewModel(){ Name = "中心距離", Value = 125, Min = 0, Max = 1000,  Step = 0.00001 },
                new InputItemViewModel(){ Name = "歯車1転位係数", Value = 0.09809, Min = -1.0, Max = 1.0,  Step = 0.00001 },
                new InputItemViewModel(){ Name = "歯幅率", Value = 30, Min = 1.0, Max = 10.0,  Step = 0.1  },
            };
        }
    }
}
