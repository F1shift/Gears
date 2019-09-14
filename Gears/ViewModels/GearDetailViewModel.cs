using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Linq;
using Gears.Models;
using static Gears.Math.Math;

namespace Gears.ViewModels
{
    class GearDetailViewModel : INotifyPropertyChanged
    {
        
        public RackParameterViewModel RackParameterViewModel { get; set; }
        public GearParameterViewModel GearParameterViewModel { get; set; }
        public string Result { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;


        SimpleCommand _UpdateCommand;
        public SimpleCommand UpdateCommand
        {
            get
            {
                return _UpdateCommand;
            }
            set
            {
                if (_UpdateCommand != value)
                {
                    _UpdateCommand = value;
                    PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(nameof(UpdateCommand)));
                }
            }
        }

        public GearDetailViewModel()
        {
            UpdateCommand = new SimpleCommand(Update);
        }

        public void Update(object para) {
            var model = new CylindricalGearBasic();

            model.mn = RackParameterViewModel.Module.Value;
            model.αn = DegToRad(GetValueFromList(RackParameterViewModel.InputItems, "圧力角"));
            model.ha_c = GetValueFromList(RackParameterViewModel.InputItems, "歯先係数");
            model.hf_c = GetValueFromList(RackParameterViewModel.InputItems, "歯先係数"); 
            model.z[0] = Convert.ToInt32(GetValueFromList(GearParameterViewModel.InputItems, "歯数１"));
            model.z[1] = Convert.ToInt32(GetValueFromList(GearParameterViewModel.InputItems, "歯数２"));
            model.β = DegToRad(GetValueFromList(GearParameterViewModel.InputItems, "ねじれ角")); 
            model.a = GetValueFromList(GearParameterViewModel.InputItems, "中心距離");
            model.xn[0] = GetValueFromList(GearParameterViewModel.InputItems, "歯車1転位係数"); 
            model.b_c[0] = model.b_c[1] = GetValueFromList(GearParameterViewModel.InputItems, "歯幅率"); 

            model.SolveFromCenterDistance();
            Result = Newtonsoft.Json.JsonConvert.SerializeObject(model, Newtonsoft.Json.Formatting.Indented);
        }

        public double GetValueFromList(IEnumerable<InputItemViewModel> list, string name)
        {
            return (from item in list
             where item.Name == name
                    select item.Value).Single();
        }
    }
}
