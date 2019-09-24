using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Linq;
using Gears.Models;
using static Gears.ThreeDUtility.ThreeDUtility;

namespace Gears.ViewModels
{
    class GearDetailViewModel : INotifyPropertyChanged
    {
        
        public RackParameterViewModel RackParameterViewModel { get; set; }
        public GearParameterViewModel GearParameterViewModel { get; set; }
        public CylindricalGearBasic Model { get; set; }
        public string Result { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public SimpleCommand UpdateCommand { get; set; }

        public GearDetailViewModel()
        {
            UpdateCommand = new SimpleCommand(Update);
        }

        public void Update(object para = null) {
            Model = new CylindricalGearBasic();

            Model.mn = RackParameterViewModel.Module.Value;
            Model.αn = DegToRad(GetValueFromList(RackParameterViewModel.InputItems, "圧力角"));
            Model.ha_c = GetValueFromList(RackParameterViewModel.InputItems, "歯先係数");
            Model.hf_c = GetValueFromList(RackParameterViewModel.InputItems, "歯元係数");
            Model.ρ_c[0] = Model.ρ_c[1] = GetValueFromList(RackParameterViewModel.InputItems, "歯元円径係数");

            Model.z[0] = Convert.ToInt32(GetValueFromList(GearParameterViewModel.InputItems, "歯数１"));
            Model.z[1] = Convert.ToInt32(GetValueFromList(GearParameterViewModel.InputItems, "歯数２"));
            Model.β = DegToRad(GetValueFromList(GearParameterViewModel.InputItems, "ねじれ角")); 
            Model.a = GetValueFromList(GearParameterViewModel.InputItems, "中心距離");
            Model.xn[0] = GetValueFromList(GearParameterViewModel.InputItems, "歯車1転位係数"); 
            Model.b_c[0] = Model.b_c[1] = GetValueFromList(GearParameterViewModel.InputItems, "歯幅率"); 

            Model.SolveFromCenterDistance();
            Result = Newtonsoft.Json.JsonConvert.SerializeObject(Model, Newtonsoft.Json.Formatting.Indented);
        }

        public double GetValueFromList(IEnumerable<InputItemViewModel> list, string name)
        {
            return (from item in list
             where item.Name == name
                    select item.Value).Single();
        }
    }
}
