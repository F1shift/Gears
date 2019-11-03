using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Linq;
using System.Collections.ObjectModel;
using Gears.DataBases;
using Gears.Models;
using static Gears.Utility.Math;

namespace Gears.ViewModels
{
    class RackParameterViewModel : System.ComponentModel.INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ModuleItemViewModel Module { get; set; }
        public List<ModuleItemViewModel> ModuleList { get; set; }
        public ObservableCollection<InputItemViewModel> InputItems { get; set; }

        public RackParameterViewModel(Action callBack = null)
        {
            Init(callBack);
        }

        public async void Init(Action callBack = null)
        {
            var moduleItemList = (await JIS1701DataBase.DataBase.Table<ModuleItem>().OrderBy((item) => item.Value).ToListAsync());
            ModuleList = (from item in moduleItemList
                          select new ModuleItemViewModel() { Value = item.Value, Serial = item.Serial, Annotation = item.Annotation }).ToList();
            Module = ModuleList.Find((item) => item.Value == 3);
            InputItems = new ObservableCollection<InputItemViewModel>() {
                new InputItemViewModel(){ Name = "圧力角", Value = 20.0, Min = 15.0, Max = 35.0,  Step = 0.5  },
                new InputItemViewModel(){ Name = "歯先係数", Value = 1, Min = 0.5, Max = 1.3, Step = 0.01  },
                new InputItemViewModel(){ Name = "歯元係数", Value = 1.25, Min = 0.6, Max = 1.5,  Step = 0.01 },
                new InputItemViewModel(){ Name = "歯元円径係数", Value = 0.35, Min = 0.1, Max = 0.4,  Step = 0.01 },
            };
            callBack?.Invoke();
        }

        public void CopyFrom(CylindricalGearBase gearBase) {
            Module = ModuleList.Find((item) => item.Value == gearBase.mn);
            InputItems.First((item) => item.Name == "圧力角").Value = gearBase.αn.RadToDeg();
            InputItems.First((item) => item.Name == "歯先係数").Value = gearBase.ha_c;
            InputItems.First((item) => item.Name == "歯元係数").Value = gearBase.hf_c;
            InputItems.First((item) => item.Name == "歯元円径係数").Value = gearBase.ρ_c[0];
        }
    }
}
