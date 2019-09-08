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

       
        ObservableCollection<object> _InputItems;
        public ObservableCollection<object> InputItems
        {
            get
            {
                return _InputItems;
            }
            set
            {
                if (_InputItems != value)
                {
                    _InputItems = value;
                    PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(nameof(InputItems)));
                }
            }
        }

        public GearParameterViewModel()
        {
            Init();
        }

        public void Init() {
            InputItems = new ObservableCollection<object>() {
                new InputItemViewModel(){ Name = "歯数１", Value = 17.0, Min = 6.0, Max = 200.0, Step = 1  },
                new InputItemViewModel(){ Name = "歯数２", Value = 39.0, Min = 6.0, Max = 200.0,  Step = 1 },
                new InputItemViewModel(){ Name = "ねじれ角", Value = 15.0, Min = -45.0, Max = 45.0,  Step = 1  },
                new InputItemViewModel(){ Name = "中心距離修正", Value = 0.0, Min = -1.0, Max = 1.0,  Step = 0.00001 },
                new InputItemViewModel(){ Name = "歯車1転位係数", Value = 0.0, Min = -1.0, Max = 1.0,  Step = 0.00001 },
                new InputItemViewModel(){ Name = "歯幅率", Value = 10.0, Min = 1.0, Max = 10.0,  Step = 0.1  },
            };
        }
    }
}
