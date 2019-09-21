using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.ComponentModel;
using System.Threading.Tasks;
using Xamarin.Forms;
using static System.Math;
using static Gears.Math.Math;
using static Gears.Math.ArrayExtentions;

namespace Gears.ViewModels
{
    class ThreeDModelingViewModel : Gears.Models.CylindricalGear, INotifyPropertyChanged
    {
        public Func<string, Task<string>> EvalAsync;
        public event PropertyChangedEventHandler PropertyChanged;

        public List<double> BasicRackVertices { get; set; }
        public List<double> BasicRackIndex { get; set; }
        public List<double> RackTraceVertices { get; set; }
        public List<double> RackTraceIndex { get; set; }

        public SimpleCommand UpdateCommand { get; set; }

        public ThreeDModelingViewModel()
        {
            UpdateCommand = new SimpleCommand((a) => Update());
        }

        public async void Update() {
            //GearDesignViewModelから数値をコピーする
            GearDesignViewModel gearDesignViewModel = (GearDesignViewModel)Application.Current.Resources[nameof(GearDesignViewModel)];
            if (gearDesignViewModel.GearDetailViewModel.Model == null)
                gearDesignViewModel.GearDetailViewModel.Update();
            var baseModel = gearDesignViewModel.GearDetailViewModel.Model;
            var baseType = baseModel.GetType();
            foreach (var property in baseType.GetProperties())
            {
                property.SetValue(this, (property.GetValue(baseModel)));
            }

            
            SolveFromCenterDistance();
            SolveProfile();

            //Three.jsデータを作る。
            int s = 10;
            for (int i = 0; i < 2; i++)
            {
                var Flank_Left_buffer = MergeToSingleList<double[], double>(CreateList<double[]>(
                    2,
                    (index) => BasicRacks[i].Flank_Left(index)));
                var Flank_Right_buffer = MergeToSingleList<double[], double>(CreateList<double[]>(
                    2,
                    (index) => BasicRacks[i].Flank_Right(index)));
                var Fillet_Left_buffer = MergeToSingleList<double[], double>(CreateList<double[]>(
                    s,
                    (index) => BasicRacks[i].Fillet_Left(index / (s - 1.0))));
                var Fillet_Right_buffer = MergeToSingleList<double[], double>(CreateList<double[]>(
                    s,
                    (index) => BasicRacks[i].Fillet_Right(index / (s - 1.0))));
                var Root_buffer = MergeToSingleList<double[], double>(CreateList<double[]>(
                    2,
                    (index) => BasicRacks[i].Root(index)));
                var TotalPositionBuffer = MergeToSingleList<List<double>, double>(new[] {
                    Flank_Left_buffer,
                    Flank_Right_buffer,
                    Fillet_Left_buffer,
                    Fillet_Right_buffer,
                    Root_buffer });
                var TotalIndexBuffer = MergeToSingleList<object[], object>(new object[][] {
                    new object[] { 0, 1 },
                    new object[] { 2, 3 },
                    MergeToSingleList<object[], object>( CreateList<object[]>(s - 1, (ii) =>new object[]{ ii + 4, ii + 5}).ToArray()).ToArray(),
                    MergeToSingleList<object[], object>( CreateList<object[]>(s - 1, (ii) =>new object[]{ ii + s + 4, ii + s + 5}).ToArray()).ToArray(),
                    new object[] { 24, 25},
                });
                var TotalPositionBufferString = Newtonsoft.Json.JsonConvert.SerializeObject(TotalPositionBuffer);
                var TotalIndexBufferString = Newtonsoft.Json.JsonConvert.SerializeObject(TotalIndexBuffer);
                await EvalAsync($"SceneController.PlotRackTrace({TotalPositionBufferString}, {TotalIndexBufferString}, {TotalPositionBufferString})");
            }
        }
    }
}
