using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.ComponentModel;
using Xamarin.Forms;
using static System.Math;
using static Gears.Math.Math;
using static Gears.Math.ArrayExtentions;

namespace Gears.ViewModels
{
    class ThreeDModelingViewModel : Gears.Models.CylindricalGearBasic, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public List<double> BasicRackVertices { get; set; }
        public List<double> BasicRackIndex { get; set; }
        public List<double> RackTraceVertices { get; set; }
        public List<double> RackTraceIndex { get; set; }

        public void Update() {
            //GearDesignViewModelから数値をコピーする
            GearDesignViewModel gearDesignViewModel = (GearDesignViewModel)Application.Current.Resources[nameof(GearDesignViewModel)];
            var baseModel = gearDesignViewModel.GearDetailViewModel.Model;
            if (baseModel == null)
                gearDesignViewModel.GearDetailViewModel.Update();
            var baseType = baseModel.GetType();
            foreach (var property in baseType.GetProperties())
            {
                property.SetValue(this, (property.GetValue(baseModel)));
            }

            
            
        }
    }
}
