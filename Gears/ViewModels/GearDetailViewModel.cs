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


        string _Result;
        public string Result
        {
            get
            {
                return _Result;
            }
            set
            {
                if (_Result != value)
                {
                    _Result = value;
                    PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(nameof(Result)));
                }
            }
        }

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
            model.αn = DegToRad((from item in RackParameterViewModel.InputItems
                       where item.Name == "圧力角"
                       select item.Value).Single());
            model.ha_c = (from item in RackParameterViewModel.InputItems
                          where item.Name == "歯先係数"
                          select item.Value).Single();
            model.hf_c = (from item in RackParameterViewModel.InputItems
                          where item.Name == "歯元係数"
                          select item.Value).Single();
            model.z[0] = Convert.ToInt32((from item in GearParameterViewModel.InputItems
                       where item.Name == "歯数１"
                                          select item.Value).Single());
            model.z[1] = Convert.ToInt32((from item in GearParameterViewModel.InputItems
                                         where item.Name == "歯数２"
                                         select item.Value).Single());
            model.β = DegToRad((from item in GearParameterViewModel.InputItems
                                         where item.Name == "ねじれ角"
                                          select item.Value).Single());
            model.a = (from item in GearParameterViewModel.InputItems
                                         where item.Name == "中心距離"
                                          select item.Value).Single();
            model.xn[0] = (from item in GearParameterViewModel.InputItems
                                         where item.Name == "歯車1転位係数"
                                           select item.Value).Single();
            model.b_c[0] = model.b_c[1] = (from item in GearParameterViewModel.InputItems
                                         where item.Name == "歯幅率"
                                           select item.Value).Single();
            model.SolveFromCenterDistance();
            Result = Newtonsoft.Json.JsonConvert.SerializeObject(model, Newtonsoft.Json.Formatting.Indented);
        }
    }
}
