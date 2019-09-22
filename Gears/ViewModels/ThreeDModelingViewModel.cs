using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.ComponentModel;
using System.Threading.Tasks;
using Xamarin.Forms;
using static System.Math;
using Gears.Math;
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
            UpdateCommand = new SimpleCommand((a) =>
                {
                    AddRackTrace();
                    AddGear();
                });
        }

        public async void AddRackTrace() {
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
            int su = 10;
            int sv = 5;
            double[] vRange = new double[] { 30, 20 };
            for (int i = 0; i < 2; i++)
            {
                Func<int, double> GetV = (int iv) => (iv - 0.5 * sv) / (0.5 * sv) * vRange[i];
                int count = 0;
                var Flank_Left_buffer = MergeToSingleList<double[], double>(CreateList(
                    sv, 2, 
                    (iv, iu) => (double[])RackTraces[i].Flank_Left(iu, GetV(iv))));
                var Flank_Left_index = MergeToSingleList<int[], int>(CreateList(
                    sv,
                    (iv)=> new int[] { iv * 2, iv * 2 + 1}));
                count += 2 * sv;

                var Flank_Right_buffer = MergeToSingleList<double[], double>(CreateList(
                    sv, 2,
                    (iv, iu) => (double[])RackTraces[i].Flank_Right(iu, GetV(iv))));
                var Flank_Right_index = MergeToSingleList<int[], int>(CreateList(
                    sv,
                    (iv) => new int[] { count + iv * 2, count + iv * 2 + 1 }));
                count += 2 * sv;

                var Fillet_Left_buffer = MergeToSingleList<double[], double>(CreateList<double[]>(
                     sv, su,
                    (iv, iu) => RackTraces[i].Fillet_Left(iu / (su - 1.0), GetV(iv))));
                var Fillet_Left_index = MergeToSingleList<int[], int>(CreateList(
                   sv, su - 1,
                   (iv, iu) => new int[] { count + iv * su + iu, count + iv * su + iu + 1 }));
                count += su * sv;
                
                var Fillet_Right_buffer = MergeToSingleList<double[], double>(CreateList<double[]>(
                    sv, su,
                    (iv, iu) => RackTraces[i].Fillet_Right(iu / (su - 1.0), GetV(iv))));
                var Fillet_Right_index = MergeToSingleList<int[], int>(CreateList(
                   sv, su - 1,
                   (iv, iu) => new int[] { count + iv * su + iu, count + iv * su + iu + 1 }));
                count += su * sv;

                var Root_buffer = MergeToSingleList<double[], double>(CreateList<double[]>(
                    sv, 2,
                    (iv, iu) => RackTraces[i].Root(iu, GetV(iv))));
                var Root_index = MergeToSingleList<int[], int>(CreateList(
                   sv,
                   (iv) => new int[] { count + iv * 2, count + iv * 2 + 1 }));
                count += 2 * sv;

                var TotalPositionBuffer = MergeToSingleList<List<double>, double>(new[] {
                    Flank_Left_buffer,
                    Flank_Right_buffer,
                    Fillet_Left_buffer,
                    Fillet_Right_buffer,
                    Root_buffer });
                var TotalIndexBuffer = MergeToSingleList<List<int>, int>(new [] {
                    Flank_Left_index,
                    Flank_Right_index,
                    Fillet_Left_index,
                    Fillet_Right_index,
                    Root_index });
                var TotalPositionBufferString = Newtonsoft.Json.JsonConvert.SerializeObject(TotalPositionBuffer);
                var TotalIndexBufferString = Newtonsoft.Json.JsonConvert.SerializeObject(TotalIndexBuffer);
                await EvalAsync($"SceneController.PlotRackTrace({TotalPositionBufferString}, {TotalIndexBufferString}, {TotalPositionBufferString})");
            }
        }

        public async void AddGear() {
            for (int i = 0; i < 2; i++)
            {
                var intersectPointPara = NonlinearFindRoot.FindRoot((double[] paras) =>
                {
                    double u1 = paras[0];
                    double v1 = paras[1];
                    double u2 = paras[2];
                    double v2 = paras[3];
                    var p1 = RackTraces[i].Flank_Left(u1, v1);
                    var p2 = RackTraces[i].Fillet_Left(u2, v2);
                    var n1 = RackTraces[i].Flank_Left_Normal(u1, v1);
                    var n2 = RackTraces[i].Fillet_Left_Normal(u2, v2);
                    var t1 = (Vector3D)Cross(n1, new Vector3D(0, 0, 1));
                    var t2 = (Vector3D)Cross(n2, new Vector3D(0, 0, 1));
                    var speed1 = RackTraces[i].GetSpeed(v1, p1).Normalize();
                    var speed2 = RackTraces[i].GetSpeed(v2, p2).Normalize();
                    var re1 = p1.X - p2.X;
                    var re2 = p1.Y - p2.Y;
                    var re3 = t1.X / speed1.X - t1.Y / speed1.Y;
                    var re4 = t2.X / speed2.X - t2.Y / speed2.Y;
                    return new double[] { re1, re2, re3, re4};
                }, new double[] { 0, 0, 1, 0 });

#if DEBUG//intersect point answer check
                var pp1 = RackTraces[i].Flank_Left(intersectPointPara.Item1[0], intersectPointPara.Item1[1]);
                var pp2 = RackTraces[i].Fillet_Left(intersectPointPara.Item1[2], intersectPointPara.Item1[3]);
                var diff = Norm(pp1 - pp2);
                if (diff > 1e-7)
                {
                    throw new Exception("intersect point answer not converging.");
                }
#endif

                var Flank_V_upper_bound = NonlinearFindRoot.FindRoot(
                    (double[] paras) =>
                    {
                        var u = paras[0];
                        var v = paras[1];
                        var p = RackTraces[i].Flank_Left(u, v);
                        var n = RackTraces[i].Flank_Left_Normal(u, v);
                        var t = (Vector3D)Cross(n, new Vector3D(0, 0, 1));
                        var speed = RackTraces[i].GetSpeed(v, p).Normalize();
                        var re1 = t.X / speed.X - t.Y / speed.Y;
                        var re2 = Norm(p) - da[i] / 2;
                        return new double[] { re1, re2 };
                    }, new double[] { 1, 1 });
#if DEBUG//intersect point answer check
                if (Abs(RackTraces[i].Flank_Left(Flank_V_upper_bound.Item1[0], Flank_V_upper_bound.Item1[1]).GetNorm() - da[i] / 2) > 1e-7)
                {
                    throw new Exception("Tip point answer not converging.");
                }
#endif
            }
        }

    }
}
