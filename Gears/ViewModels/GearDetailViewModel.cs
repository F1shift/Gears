using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.ComponentModel;
using System.Linq;
using Gears.Models;
using static Gears.Utility.Math;

namespace Gears.ViewModels
{
    class GearDetailViewModel : INotifyPropertyChanged
    {
        
        public RackParameterViewModel RackParameterViewModel { get; set; }
        public GearParameterViewModel GearParameterViewModel { get; set; }
        public CylindricalGearBasic Model { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public SimpleCommand UpdateCommand { get; set; }

        public GearDetailViewModel()
        {
            UpdateCommand = new SimpleCommand((obj)=> { CheckUpdate(); return null; });
        }

        public bool CheckUpdate() {
            var newModel = new CylindricalGearBasic();

            newModel.mn = RackParameterViewModel.Module.Value;
            newModel.αn = GetValueFromList(RackParameterViewModel.InputItems, "圧力角").DegToRad();
            newModel.ha_c = GetValueFromList(RackParameterViewModel.InputItems, "歯先係数");
            newModel.hf_c = GetValueFromList(RackParameterViewModel.InputItems, "歯元係数");
            newModel.ρ_c[0] = newModel.ρ_c[1] = GetValueFromList(RackParameterViewModel.InputItems, "歯元円径係数");

            newModel.z[0] = Convert.ToInt32(GetValueFromList(GearParameterViewModel.InputItems, "歯数１"));
            newModel.z[1] = Convert.ToInt32(GetValueFromList(GearParameterViewModel.InputItems, "歯数２"));
            newModel.β = GetValueFromList(GearParameterViewModel.InputItems, "ねじれ角").DegToRad();
            newModel.xn[0] = GetValueFromList(GearParameterViewModel.InputItems, "歯車1転位係数");
            newModel.xn[1] = GetValueFromList(GearParameterViewModel.InputItems, "歯車2転位係数");
            newModel.b_c[0] = newModel.b_c[1] = GetValueFromList(GearParameterViewModel.InputItems, "歯幅率");

            if(Model == null || !Model.HasSameInputWith(newModel))
            {
                Model = newModel;
                Model.SolveFromXn();
            }
            else
            {
                return false;
            }
            return true;
        }

        public double GetValueFromList(IEnumerable<InputItemViewModel> list, string name)
        {
            return (from item in list
             where item.Name == name
                    select item.Value).Single();
        }
    }

}
