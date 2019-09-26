using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.ComponentModel;
using System.Threading.Tasks;
using Xamarin.Forms;
using static System.Math;
using Gears.ThreeDUtility;
using static Gears.ThreeDUtility.ThreeDUtility;
using static Gears.ThreeDUtility.ArrayExtentions;

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
            UpdateCommand = new SimpleCommand(async (a) =>
                {
                    Initial();
                    await EvalAsync($"SceneController.Clear();");
                    AddRackTrace();
                    //double v = 10;
                    //AddRackTrace(v, 0);
                    //AddRackTrace(v, 1);
                    AddGear();
                });
        }

        public void Initial() {
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
        }

        public async void AddRackTrace() {
            

            //Three.jsデータを作る。
            int su = 10;
            int sv = 15;
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
                await EvalAsync($"SceneController.AddBufferGeometryMesh({TotalPositionBufferString}, {TotalIndexBufferString}, {TotalPositionBufferString}, 'line')");
            }
        }

        public void AddRackTrace(double v, int i)
        {
            int su = 10;
            Action<
                Func<double, double, Vector3D>, 
                Func<double, double, Vector3D>, 
                Func<double, Vector3D, Vector3D>,
                double, int, double, double> plotCurveAndNormal =
                async (curveFunction, curveNormalFunc, speedFunc, vv, segment, color, normalLength) =>
                {
                    var uu = CreateList(segment, (iu) => (double)iu / (segment - 1));
                    var points = CreateList(uu.Count, (iu) => curveFunction(uu[iu], vv).buffer);
                    var normalTips = CreateList(uu.Count, (iu) => (curveFunction(uu[iu], vv) + curveNormalFunc(uu[iu], vv) * normalLength).buffer);
                    var speedTips = CreateList(uu.Count, (iu) => (curveFunction(uu[iu], vv) + speedFunc(vv, points[iu]) * 10).buffer);
                    points.AddRange(normalTips);
                    points.AddRange(speedTips);
                    var buffer = MergeToSingleList<double[], double>(points);
                    var curveLineIndex = MergeToSingleList<int[], int>(CreateList(segment - 1,
                        (ii) => new int[] { ii, ii + 1 }));
                    var normalLineIndex = MergeToSingleList<int[], int>(CreateList(segment,
                        (ii) => new int[] { ii, segment + ii}));
                    var speedLineIndex = MergeToSingleList<int[], int>(CreateList(segment,
                        (ii) => new int[] { ii, segment * 2 + ii }));
                    var index = MergeToSingleList<List<int>, int>(new[] { curveLineIndex, normalLineIndex, speedLineIndex });
                    await EvalAsync($"SceneController.AddBufferGeometryMesh(" +
                    $"{Newtonsoft.Json.JsonConvert.SerializeObject(buffer)}," +
                    $" {Newtonsoft.Json.JsonConvert.SerializeObject(index)}, " +
                    $" {color}, 'line')");
                };
            plotCurveAndNormal(
                RackTraces[i].Flank_Left,
                RackTraces[i].Flank_Left_Normal,
                RackTraces[i].GetSpeed,
                v,
                su, 0xFF0000,
                mn * 0.5);
            plotCurveAndNormal(
                RackTraces[i].Flank_Right,
                RackTraces[i].Flank_Right_Normal,
                RackTraces[i].GetSpeed,
                v,
                su, 0xFF0000,
                mn * 0.5);
            plotCurveAndNormal(
                RackTraces[i].Fillet_Left,
                RackTraces[i].Fillet_Left_Normal,
                RackTraces[i].GetSpeed,
                v,
                su, 0x00FF00,
                mn * 0.4);
            plotCurveAndNormal(
                RackTraces[i].Fillet_Right,
                RackTraces[i].Fillet_Right_Normal,
                RackTraces[i].GetSpeed,
                v,
                su, 0x00FF00,
                mn * 0.4);
            plotCurveAndNormal(
                RackTraces[i].Root,
                RackTraces[i].Root_Normal,
                RackTraces[i].GetSpeed,
                v,
                su, 0x0000FF,
                mn * 0.3);
        }

        public async void AddGear() {
            for (int i = 0; i < 2; i++)
            {
                int sg = 10;
                var gearProifile = GearProfiles[i];

                #region 断面プロファイル作成
                var FlankAndFilletIntersectPointPara = NonlinearFindRoot.FindRoot((double[] paras) =>
                {
                    double u1 = paras[0];
                    double u2 = paras[1];
                    var p1 = gearProifile.Flank_Left(u1);
                    var p2 = gearProifile.Fillet_Left(u2);
                    var re1 = p1.X - p2.X;
                    var re2 = p1.Y - p2.Y;
                    return new double[] { re1, re2 };
                }, new double[] { 0, 1,});
                var u_of_flank_at_intersectionPoint = FlankAndFilletIntersectPointPara.Item1[0];
                var u_of_fillet_at_intersectionPoint = FlankAndFilletIntersectPointPara.Item1[1];

#if DEBUG//intersect point answer check
                var pp1 = gearProifile.Flank_Left(u_of_flank_at_intersectionPoint);
                var pp2 = gearProifile.Fillet_Left(u_of_fillet_at_intersectionPoint);
                var diff = Norm(pp1 - pp2);
                if (diff > 1e-7)
                {
                    throw new Exception("intersect point answer not converging.");
                }
#endif

                var TipIntersectPointPara = NonlinearFindRoot.FindRoot(
                    (double u) =>
                    {
                        var p = gearProifile.Flank_Left(u);
                        var re = Norm(p) - da[i] / 2;
                        return re;
                    }, 1 );
                var u_of_flank_at_tooth_tip = TipIntersectPointPara.Item1;
#if DEBUG//intersect point answer check
                if (Abs(gearProifile.Flank_Left(u_of_flank_at_tooth_tip).GetNorm() - da[i] / 2) > 1e-7)
                {
                    throw new Exception("Tip point answer not converging.");
                }
#endif
                var uArray_Flank = CreateList<double>(sg, (index) => 
                    u_of_flank_at_intersectionPoint + (double)index / (sg - 1) * (u_of_flank_at_tooth_tip - u_of_flank_at_intersectionPoint)
                    );
                var flankPoints_Left = CreateList<double[]>(sg, (index) => gearProifile.Flank_Left(uArray_Flank[index]));
                var flankNormal_Left = CreateList<double[]>(sg, (index) => {
                    var p = flankPoints_Left[index];
                    var sectionNormal = gearProifile.Flank_Left_Normal(uArray_Flank[index]);
                    var t1 = Cross(new Vector3D(0, 0, 1), sectionNormal);
                    var βp = Atan2(FirstNNorm(p, 2) * 2 * PI, L[i]);
                    if ((歯車1が左ねじである && i == 1) || (!歯車1が左ねじである && i == 0))
                        βp *= -1;
                    var M = CreateRotateMatrix(p, βp);
                    var t2 = RotateVector(M, new Vector3D(0, 0, -1));
                    var N = Cross(t1, t2);
                    return N;
                    });
#if DEBUG
                var flankPoints_buffer_Left = MergeToSingleList<double[], double>(flankPoints_Left);
                var flankPoints_index_Left = MergeToSingleList<int[], int>(CreateList<int[]>(sg - 1, (index) => new[] { index, index + 1 }));
                await EvalAsync($"SceneController.AddBufferGeometryMesh(" +
                    $" {Newtonsoft.Json.JsonConvert.SerializeObject(flankPoints_buffer_Left)}," +
                    $" {Newtonsoft.Json.JsonConvert.SerializeObject(flankPoints_index_Left)}, " +
                    $" 0xFF0000, 'line')");
#endif
                var uArray_Fillet = CreateList<double>(sg, (index) =>
                     (double)index / (sg - 1) * u_of_fillet_at_intersectionPoint
                    );
                var filletPoints_Left = CreateList<double[]>(sg, (index) => gearProifile.Fillet_Left(uArray_Fillet[index]));
                var filletNormal_Left = CreateList<double[]>(sg, (index) => {
                    var p = filletPoints_Left[index];
                    var sectionNormal = gearProifile.Fillet_Left_Normal(uArray_Fillet[index]);
                    var t1 = Cross(new Vector3D(0, 0, 1), sectionNormal);
                    var βp = Atan2(FirstNNorm(p, 2) * 2 * PI, L[i]);
                    if ((歯車1が左ねじである && i == 1) || (!歯車1が左ねじである && i == 0))
                        βp *= -1;
                    var M = CreateRotateMatrix(p, βp);
                    var t2 = RotateVector(M, new Vector3D(0, 0, -1));
                    var N = Cross(t1, t2);
                    return N;
                });
#if DEBUG
                var filletPoints_buffer_Left = MergeToSingleList<double[], double>(filletPoints_Left);
                var filletPoints_index_Left = MergeToSingleList<int[], int>(CreateList(sg - 1, (index) => new[] { index, index + 1 }));
                await EvalAsync($"SceneController.AddBufferGeometryMesh(" +
                    $" {Newtonsoft.Json.JsonConvert.SerializeObject(filletPoints_buffer_Left)}," +
                    $" {Newtonsoft.Json.JsonConvert.SerializeObject(filletPoints_index_Left)}, " +
                    $" 0x0000FF, 'line')");
#endif

                var flankPoints_Right = CreateList<double[]>(sg, (index) => gearProifile.Flank_Right(uArray_Flank[index]));
                var flankNormal_Right = CreateList<double[]>(sg, (index) => {
                    var p = flankPoints_Right[index];
                    var sectionNormal = gearProifile.Flank_Right_Normal(uArray_Flank[index]);
                    var t1 = Cross(new Vector3D(0, 0, 1), sectionNormal);
                    var βp = Atan2(FirstNNorm(p, 2) * 2 * PI, L[i]);
                    if ((歯車1が左ねじである && i == 1) || (!歯車1が左ねじである && i == 0))
                        βp *= -1;
                    var M = CreateRotateMatrix(p, βp);
                    var t2 = RotateVector(M, new Vector3D(0, 0, -1));
                    var N = Cross(t1, t2);
                    return N;
                });
#if DEBUG
                var flankPoints_buffer_Right = MergeToSingleList<double[], double>(flankPoints_Right);
                var flankPoints_index_Right = MergeToSingleList<int[], int>(CreateList(sg - 1, (index) => new[] { index, index + 1 }));
                await EvalAsync($"SceneController.AddBufferGeometryMesh(" +
                    $" {Newtonsoft.Json.JsonConvert.SerializeObject(flankPoints_buffer_Right)}," +
                    $" {Newtonsoft.Json.JsonConvert.SerializeObject(flankPoints_index_Right)}, " +
                    $" 0xFF0000, 'line')");
#endif
                var filletPoints_Right = CreateList<double[]>(sg, (index) => gearProifile.Fillet_Right(uArray_Fillet[index]));
                var filletNormal_Right = CreateList<double[]>(sg, (index) => {
                    var p = filletPoints_Right[index];
                    var sectionNormal = gearProifile.Fillet_Right_Normal(uArray_Fillet[index]);
                    var t1 = Cross(new Vector3D(0, 0, 1), sectionNormal);
                    var βp = Atan2(FirstNNorm(p, 2) * 2 * PI, L[i]);
                    if ((歯車1が左ねじである && i == 1) || (!歯車1が左ねじである && i == 0))
                        βp *= -1;
                    var M = CreateRotateMatrix(p, βp);
                    var t2 = RotateVector(M, new Vector3D(0, 0, -1));
                    var N = Cross(t1, t2);
                    return N;
                });
#if DEBUG
                var filletPoints_buffer_Right = MergeToSingleList<double[], double>(filletPoints_Right);
                var filletPoints_index_Right = MergeToSingleList<int[], int>(CreateList(sg - 1, (index) => new[] { index, index + 1 }));
                await EvalAsync($"SceneController.AddBufferGeometryMesh(" +
                    $" {Newtonsoft.Json.JsonConvert.SerializeObject(filletPoints_buffer_Right)}," +
                    $" {Newtonsoft.Json.JsonConvert.SerializeObject(filletPoints_index_Right)}, " +
                    $" 0x0000FF, 'line')");
#endif

                var rootPoints = CreateList<double[]>(4, (index) => gearProifile.Root((double)index / (4 - 1)));
                var rootNormal = CreateList<double[]>(4, (index) => {
                    var p = rootPoints[index];
                    var sectionNormal = gearProifile.Root_Normal((double)index / (4 - 1));
                    var t1 = Cross(new Vector3D(0, 0, 1), sectionNormal);
                    var βp = Atan2(FirstNNorm(p, 2) * 2 * PI, L[i]);
                    if ((歯車1が左ねじである && i == 1) || (!歯車1が左ねじである && i == 0))
                        βp *= -1;
                    var M = CreateRotateMatrix(p, βp);
                    var t2 = RotateVector(M, new Vector3D(0, 0, -1));
                    var N = Cross(t1, t2);
                    return N;
                });
#if DEBUG
                var rootPoints_buffer = MergeToSingleList<double[], double>(rootPoints);
                var rootPoints_index = MergeToSingleList<int[], int>(CreateList(sg - 1, (index) => new[] { index, index + 1 }));
                await EvalAsync($"SceneController.AddBufferGeometryMesh(" +
                    $" {Newtonsoft.Json.JsonConvert.SerializeObject(rootPoints_buffer)}," +
                    $" {Newtonsoft.Json.JsonConvert.SerializeObject(rootPoints_index)}, " +
                    $" 0xFF9900, 'line')");
#endif
                var tipPoints = new List<double[]>() { flankPoints_Left[flankPoints_Left.Count - 1],
                    RotateVector(CreateRotateMatrix(Axis.Z, 2 * PI / z[i]), flankPoints_Right[flankPoints_Right.Count - 1]) };
                var tipNormal = CreateList<double[]>(tipPoints.Count, (index) => {
                    var N = GetNormalized(tipPoints[index]);
                    return N;
                });
                #endregion

                #region 歯面を作成
                Action<List<double[]>, List<double[]>, int, bool, string> CreateScrewFace = async (sectionVectors, vectorNormals, sectionNumber, renderAnotherSide, name) =>
                {
                    List<List<double[]>> FaceTopo = new List<List<double[]>>();
                    List<List<double[]>> FaceNormal = new List<List<double[]>>();
                    for (int index = 0; index < sectionNumber; index++)
                    {
                        var bz = -b[i] * index / (sectionNumber - 1);
                        var theta = 2 * PI * (bz / L[i]);
                        var M = CreateRotateMatrix(Axis.Z, theta);
                        M[2, 3] = bz;
                        var sectionTopo = sectionVectors.ConvertAll<double[]>(
                            new Converter<double[], double[]>(
                                (vector) =>
                                {
                                    return RotateAnMoveVector(M, vector);
                                }));
                        var sectionNormal = vectorNormals.ConvertAll<double[]>(
                            new Converter<double[], double[]>(
                                (vector) =>
                                {
                                    return RotateVector(M, vector);
                                }));
                        FaceTopo.Add(sectionTopo);
                        FaceNormal.Add(sectionNormal);
                    }
                    var FaceTopo_buffer = MergeToSingleList<double[], double>(
                        MergeToSingleList<List<double[]>, double[]>(
                            FaceTopo
                            )
                        );
                    var FaceNormal_buffer = MergeToSingleList<double[], double>(
                        MergeToSingleList<List<double[]>, double[]>(
                            FaceNormal
                            )
                        );
                    var FaceTopo_index = MergeToSingleList<int[], int>(CreateList(sectionNumber - 1, sectionVectors.Count - 1, (ii, jj) => {
                        var index_11 = ii * sectionVectors.Count + jj;
                        var index_12 = index_11 + 1;
                        var index_21 = index_11 + sectionVectors.Count;
                        var index_22 = index_12 + sectionVectors.Count;
                        if (renderAnotherSide)
                            return new[] { index_11, index_12, index_21,  index_12, index_22, index_21 };
                        else
                            return new[] { index_11, index_21, index_12, index_12, index_21, index_22 };
                    }));
                    await EvalAsync($"SceneController.AddBufferGeometryMesh(" +
                        $" {Newtonsoft.Json.JsonConvert.SerializeObject(FaceTopo_buffer)}," +
                        $" {Newtonsoft.Json.JsonConvert.SerializeObject(FaceTopo_index)}, " +
                        $" 0xFFFFFF, 'mesh'," +
                        $" {Newtonsoft.Json.JsonConvert.SerializeObject(FaceNormal_buffer)}," +
                        $" '{name}');");
                };
                int sb = 10;
                
                CreateScrewFace(flankPoints_Right, flankNormal_Right, sb, true, nameof(flankPoints_Right) + i);
                CreateScrewFace(filletPoints_Left, filletNormal_Left, sb, false, nameof(filletPoints_Left) + i);
                CreateScrewFace(rootPoints, rootNormal, sb, true, nameof(rootPoints) + i);
                CreateScrewFace(filletPoints_Right, filletNormal_Right, sb, true, nameof(filletPoints_Right) + i);
                CreateScrewFace(flankPoints_Left, flankNormal_Left, sb, false, nameof(flankPoints_Left) + i);
                CreateScrewFace(tipPoints, tipNormal, sb, false, nameof(tipPoints) + i);
                #endregion

                for (int j = 1; j < z[i]; j++)
                {
                    var theta = 2 * PI / z[i] * j;
                    await EvalAsync($"SceneController.CopyMesh(" +
                        $"'{nameof(flankPoints_Right) + i}', new THREE.Matrix4().makeRotationZ({theta}));");
                    await EvalAsync($"SceneController.CopyMesh(" +
                        $"'{nameof(filletPoints_Right) + i}', new THREE.Matrix4().makeRotationZ({theta}));");
                    await EvalAsync($"SceneController.CopyMesh(" +
                        $"'{nameof(rootPoints) + i}', new THREE.Matrix4().makeRotationZ({theta}));");
                    await EvalAsync($"SceneController.CopyMesh(" +
                        $"'{nameof(filletPoints_Left) + i}', new THREE.Matrix4().makeRotationZ({theta}));");
                    await EvalAsync($"SceneController.CopyMesh(" +
                        $"'{nameof(flankPoints_Left) + i}', new THREE.Matrix4().makeRotationZ({theta}));");
                    await EvalAsync($"SceneController.CopyMesh(" +
                        $"'{nameof(tipPoints) + i}', new THREE.Matrix4().makeRotationZ({theta}));");
                }
            }
        }

    }
}
