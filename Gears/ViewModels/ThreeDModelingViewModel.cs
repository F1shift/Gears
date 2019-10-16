using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Linq;
using System.ComponentModel;
using System.Threading.Tasks;
using Xamarin.Forms;
using static System.Math;
using Gears.Utility;
using static Gears.Utility.Math;
using static Gears.Utility.EnumerableExtentions;

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
        public GearDetailViewModel GearDetailViewModel { get; set; }
        public SimpleCommand UpdateCommand { get; set; }

        TimeMeasurer timeMeasurer = new TimeMeasurer();
        public ThreeDModelingViewModel(GearDetailViewModel gearDetailViewModel)
        {
            this.GearDetailViewModel = gearDetailViewModel;
            UpdateCommand = new SimpleCommand(async (a) =>
                {
                    timeMeasurer.StartOrReset();
                    //await StopAnimate();
                    timeMeasurer.Report(nameof(Initial));
                    //await EvalAsync($"SceneController.Clear();");
                    //AddRackTrace();
                    //double v = 10;
                    //AddRackTrace(v, 0);
                    //AddRackTrace(v, 1);
                    await UpdateOrAddGear();
                    //await StartAnimate();
                    timeMeasurer.PrintAllRecord();
                    return null;
                });
        }

        public void Initial() {
            //GearDesignViewModelから数値をコピーする
            if (GearDetailViewModel.Model == null)
                GearDetailViewModel.CheckUpdate();
            var baseModel = GearDetailViewModel.Model;
            var baseType = baseModel.GetType();
            foreach (var property in baseType.GetProperties())
            {
                property.SetValue(this, (property.GetValue(baseModel)));
            }
            SolveProfile();
        }

        public async Task<string> AddBufferGeometry(BufferGeometryData data) {
            return await EvalAsync($"SceneController.AddBufferGeometryMesh(" +
                $"{Newtonsoft.Json.JsonConvert.SerializeObject(data)});");
        }
        public async Task<string> UpdateOrCreateGear(GearGeometryData data)
        {
            var datastr = Newtonsoft.Json.JsonConvert.SerializeObject(data) ;
            timeMeasurer.Report("JsonConvert");
            var re = await EvalAsync($"SceneController.UpdateOrCreateGear(" + datastr  + ");");
            timeMeasurer.Report("EvalAsync");
            return re;
        }

        //public async Task<string> StopAnimate()
        //{
        //    var re = await EvalAsync($"SceneController.StopAnimate();");
        //    return re;
        //}

        //public async Task<string> StartAnimate()
        //{
        //    var re = await EvalAsync($"SceneController.StartAnimate();");
        //    return re;
        //}

        public void AddRackTrace() {
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
                var bufferData = new BufferGeometryData(BufferGeometryData.Types.line) { position = TotalPositionBuffer, index = TotalIndexBuffer, color = TotalPositionBuffer };
                AddBufferGeometry(bufferData);
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
                    var bufferData = new BufferGeometryData(BufferGeometryData.Types.line) { position = buffer, index = index, color = color };

                    await AddBufferGeometry(bufferData);
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

        int sg = 10;
        int sb = 10;
        public async Task<bool> UpdateOrAddGear() {
            Initial();
            for (int i = 0; i < 2; i++)
            {
                
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
                }, new double[] { 0, 0.5,});
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
                //if (Abs(gearProifile.Flank_Left(u_of_flank_at_tooth_tip).GetNorm() - da[i] / 2) > 1e-7)
                //{
                //    throw new Exception("Tip point answer not converging.");
                //}
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
                    if (i == 1)
                        βp *= -1;
                    var M = CreateRotateMatrix(p, βp);
                    var t2 = RotateVector(M, new Vector3D(0, 0, -1));
                    var N = Cross(t1, t2);
                    return N;
                    });
#if DEBUG
                //var flankPoints_buffer_Left = MergeToSingleList<double[], double>(flankPoints_Left);
                //var flankPoints_index_Left = MergeToSingleList<int[], int>(CreateList<int[]>(sg - 1, (index) => new[] { index, index + 1 }));
                //AddBufferGeometry(new BufferGeometryData(BufferGeometryData.Types.line)
                //{
                //    position = flankPoints_buffer_Left,
                //    index = flankPoints_index_Left,
                //    color = 0xFF0000
                //});
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
                    if (i == 1)
                        βp *= -1;
                    var M = CreateRotateMatrix(p, βp);
                    var t2 = RotateVector(M, new Vector3D(0, 0, -1));
                    var N = Cross(t1, t2);
                    return N;
                });
#if DEBUG
                //var filletPoints_buffer_Left = MergeToSingleList<double[], double>(filletPoints_Left);
                //var filletPoints_index_Left = MergeToSingleList<int[], int>(CreateList(sg - 1, (index) => new[] { index, index + 1 }));
                //AddBufferGeometry(new BufferGeometryData(BufferGeometryData.Types.line)
                //{
                //    position = filletPoints_buffer_Left,
                //    index = filletPoints_index_Left,
                //    color = 0x0000FF,
                //});
#endif

                var flankPoints_Right = CreateList<double[]>(sg, (index) => gearProifile.Flank_Right(uArray_Flank[index]));
                var flankNormal_Right = CreateList<double[]>(sg, (index) => {
                    var p = flankPoints_Right[index];
                    var sectionNormal = gearProifile.Flank_Right_Normal(uArray_Flank[index]);
                    var t1 = Cross(new Vector3D(0, 0, 1), sectionNormal);
                    var βp = Atan2(FirstNNorm(p, 2) * 2 * PI, L[i]);
                    if (i == 1)
                        βp *= -1;
                    var M = CreateRotateMatrix(p, βp);
                    var t2 = RotateVector(M, new Vector3D(0, 0, -1));
                    var N = Cross(t1, t2);
                    return N;
                });
#if DEBUG
                //var flankPoints_buffer_Right = MergeToSingleList<double[], double>(flankPoints_Right);
                //var flankPoints_index_Right = MergeToSingleList<int[], int>(CreateList(sg - 1, (index) => new[] { index, index + 1 }));
                //AddBufferGeometry(new BufferGeometryData(BufferGeometryData.Types.line)
                //{
                //    position = flankPoints_buffer_Right,
                //    index = flankPoints_index_Right,
                //    color = 0xFF0000,
                //});
#endif
                var filletPoints_Right = CreateList<double[]>(sg, (index) => gearProifile.Fillet_Right(uArray_Fillet[index]));
                var filletNormal_Right = CreateList<double[]>(sg, (index) => {
                    var p = filletPoints_Right[index];
                    var sectionNormal = gearProifile.Fillet_Right_Normal(uArray_Fillet[index]);
                    var t1 = Cross(new Vector3D(0, 0, 1), sectionNormal);
                    var βp = Atan2(FirstNNorm(p, 2) * 2 * PI, L[i]);
                    if (i == 1)
                        βp *= -1;
                    var M = CreateRotateMatrix(p, βp);
                    var t2 = RotateVector(M, new Vector3D(0, 0, -1));
                    var N = Cross(t1, t2);
                    return N;
                });
#if DEBUG
                //var filletPoints_buffer_Right = MergeToSingleList<double[], double>(filletPoints_Right);
                //var filletPoints_index_Right = MergeToSingleList<int[], int>(CreateList(sg - 1, (index) => new[] { index, index + 1 }));
                //AddBufferGeometry(new BufferGeometryData(BufferGeometryData.Types.line)
                //{
                //    position = filletPoints_buffer_Right,
                //    index = filletPoints_index_Right,
                //    color = 0x0000FF,
                //});
#endif

                var rootPoints = CreateList<double[]>(4, (index) => gearProifile.Root((double)index / (4 - 1)));
                var rootNormal = CreateList<double[]>(4, (index) => {
                    var p = rootPoints[index];
                    var sectionNormal = gearProifile.Root_Normal((double)index / (4 - 1));
                    var t1 = Cross(new Vector3D(0, 0, 1), sectionNormal);
                    var βp = Atan2(FirstNNorm(p, 2) * 2 * PI, L[i]);
                    if (i == 1)
                        βp *= -1;
                    var M = CreateRotateMatrix(p, βp);
                    var t2 = RotateVector(M, new Vector3D(0, 0, -1));
                    var N = Cross(t1, t2);
                    return N;
                });
#if DEBUG
                //var rootPoints_buffer = MergeToSingleList<double[], double>(rootPoints);
                //var rootPoints_index = MergeToSingleList<int[], int>(CreateList(sg - 1, (index) => new[] { index, index + 1 }));
                //AddBufferGeometry(new BufferGeometryData(BufferGeometryData.Types.line)
                //{
                //    position = rootPoints_buffer,
                //    index = rootPoints_index,
                //    color = 0xFF9900,
                //});
#endif
                var tipPoints = new List<double[]>() { flankPoints_Left[flankPoints_Left.Count - 1],
                    RotateVector(CreateRotateMatrix(Axis.Z, 2 * PI / z[i]), flankPoints_Right[flankPoints_Right.Count - 1]) };
                var tipNormal = CreateList<double[]>(tipPoints.Count, (index) => {
                    var N = GetNormalized(tipPoints[index]);
                    return N;
                });
                timeMeasurer.Report("断面プロファイル作成");
                #endregion

                #region 歯面を作成
                Func<List<double[]>, List<double[]>, int, bool, BufferGeometryData> CreateScrewFace = (sectionVectors, vectorNormals, sectionNumber, renderAnotherSide) =>
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
                    return new BufferGeometryData(BufferGeometryData.Types.mesh) {
                        position = FaceTopo_buffer,
                        normal = FaceNormal_buffer,
                        index = FaceTopo_index };
                };
                
                var rightFlankData = CreateScrewFace(flankPoints_Right, flankNormal_Right, sb, true);
                var rightFilletData = CreateScrewFace(filletPoints_Right, filletNormal_Right, sb, true);
                var rooData = CreateScrewFace(rootPoints, rootNormal, sb, true);
                var leftFilletData = CreateScrewFace(flankPoints_Left, flankNormal_Left, sb, false);
                var leftFlankData = CreateScrewFace(filletPoints_Left, filletNormal_Left, sb, false);
                var tipData = CreateScrewFace(tipPoints, tipNormal, sb, false);
                timeMeasurer.Report("歯面を作成");
                #endregion

                #region 前後端面作成
                var frontFacePoints = new List<double[]>();
                frontFacePoints.AddRange(filletPoints_Left);
                frontFacePoints.RemoveLast();
                frontFacePoints.AddRange(flankPoints_Left);
                var MM = CreateRotateMatrix(Axis.Z, 2 * PI / z[i]);
                frontFacePoints.AddRange(filletPoints_Right.ConvertAll<double[]>(new Converter<double[], double[]>((item) => RotateVector(MM, item))));
                frontFacePoints.RemoveLast();
                frontFacePoints.AddRange(flankPoints_Right.ConvertAll<double[]>(new Converter<double[], double[]>((item) => RotateVector(MM, item))));
                var frontFaceIndex = new List<int>();
                for (int j = 0; j < frontFacePoints.Count / 2 - 1; j++)
                {
                    var index11 = j;
                    var index12 = index11 + 1;
                    var index21 = frontFacePoints.Count / 2 + j;
                    var index22 = index21 + 1;
                    frontFaceIndex.AddRange(new[] { index11, index12, index21, index21, index12, index22 });
                }
                var frontFaceNormal = frontFacePoints.ConvertAll<double[]>(new Converter<double[], double[]>((item) => new double[] { 0, 0, 1 }));
                var frontFaceBufferData = new BufferGeometryData(BufferGeometryData.Types.mesh);

                frontFaceBufferData.position = MergeToSingleList<double[], double>(frontFacePoints);
                frontFaceBufferData.index = frontFaceIndex;
                frontFaceBufferData.normal = MergeToSingleList<double[], double>(frontFaceNormal);
                //
                Func<List<Vector3D>, double, double, int, bool, BufferGeometryData> RotateExtrusion = (sectionProfile, startAngle, endAngle, segment, reverseSide) =>
                {
                    var bufferData = new BufferGeometryData(BufferGeometryData.Types.mesh);
                    for (int m = 0; m < segment; m++)
                    {
                        double angle = startAngle + (endAngle - startAngle) / (segment - 1) * m;
                        var MMM = CreateRotateMatrix(Axis.Z, angle);
                        for (int n = 0; n < sectionProfile.Count - 1; n++)
                        {
                            Vector3D p1 = RotateVector(MMM, sectionProfile[n]);
                            Vector3D p2 = RotateVector(MMM, sectionProfile[n + 1]);
                            Vector3D n1 = RotateVector(MMM, new Vector3D(1, 0, 0));
                            Vector3D normal = ((Vector3D)Cross((p2 - p1), n1)).Normalize();
                            bufferData.position.AddRanges(p1.buffer, p2.buffer);
                            bufferData.normal.AddRanges(normal.buffer, normal.buffer);
                        }
                    }

                    int lengthD2 = sectionProfile.Count * 2 - 2;
                    for (int m = 0; m < segment - 1; m++)
                    {
                        for (int n = 0; n < sectionProfile.Count - 1; n++)
                        {
                            int index11 = m * lengthD2 + n * 2;
                            int index12 = index11 + 1;
                            int index21 = (m + 1) * lengthD2 + n * 2;
                            int index22 = index21 + 1;
                            if (reverseSide)
                                bufferData.index.AddRange(new int[] { index11, index12, index21, index21, index12, index22 });
                            else
                                bufferData.index.AddRange(new int[] { index11, index21, index12, index12, index21, index22 });
                        }
                    }
                    return bufferData;
                };
                var SectionProfile = new List<Vector3D>();
                var L1 = df[i] / 2 - 1.7 * mn;
                var L4 = L1 / 2;
                var L2 = L1 - (L1 - L4) * 0.2;
                var L3 = L4 + (L1 - L4) * 0.3;
                var W = b[i] * 0.3 < L2 - L3 ? b[i] * 0.3 : L2 - L3;
                var C = (L1 - L4) * 0.05;
                var R = W * 0.4 < (L2 - L3) * 0.25 ? W * 0.4 : (L2 - L3) * 0.25;
                SectionProfile.Add(new Vector3D(0, L1, 0));
                SectionProfile.Add(new Vector3D(0, L2 + C, 0));
                var test = new List<int>() { 1, 2, 3 };
                SectionProfile.Add(SectionProfile.Last() + new Vector3D(0, -1 , -1) * C);
                var center1 = new Vector3D(0, L2 - R, -W + R);
                SectionProfile.AddRange(CreateList<Vector3D>(10, (index) =>
                {
                    double angle = PI / 2 / (10 - 1) * (10 - 1 - index);
                    return new Vector3D(0, Sin(angle), -Cos(angle)) * R + center1;
                }));
                var center2 = new Vector3D(0, L3 + R, -W + R);
                SectionProfile.AddRange(CreateList<Vector3D>(10, (index) =>
                {
                    double angle =  - PI / 2 / (10 - 1) * index;
                    return new Vector3D(0, Sin(angle), -Cos(angle)) * R + center2;
                }));
                SectionProfile.Add(new Vector3D(0, L3, -C));
                SectionProfile.Add(SectionProfile.Last() + new Vector3D(0, -1, 1) * C);
                SectionProfile.Add(new Vector3D(0, L4, 0));
                SectionProfile.Add(new Vector3D(0, L4, - b[i] / 2));
                var frontFaceSegment = 10;
                frontFaceBufferData.Merge(RotateExtrusion(SectionProfile,
                    Atan2(rootPoints.First()[1], rootPoints.First()[0]) - PI / 2,
                     Atan2(rootPoints.First()[1], rootPoints.First()[0]) - PI / 2 + 2 * PI / z[i],
                    frontFaceSegment, false));

                var GapMeshVectors = new List<double[]>();
                GapMeshVectors.AddRange(rootPoints.GetReversed());
                GapMeshVectors.Add(RotateVector(CreateRotateMatrix(Axis.Z, 2 * PI / z[i]), rootPoints.Last()));
                for (int k = 0; k < frontFaceSegment - rootPoints.Count - 1; k++)
                {
                    GapMeshVectors.Add(GapMeshVectors.Last());
                }
                GapMeshVectors.AddRange(CreateList<double[]>(frontFaceSegment, (index) => {
                    var angle = Atan2(rootPoints.First()[1], rootPoints.First()[0]) - PI / 2 + 2 * PI / z[i] / (frontFaceSegment - 1) * index;
                    return RotateVector(CreateRotateMatrix(Axis.Z, angle), new Vector3D(0, L1, 0));
                    }
                ));
                var GapMeshNormal = CreateList<double[]>(frontFaceSegment * 2, (index) => new Vector3D(0, 0, 1));
                var GapMeshIndex = new List<int>();
                for (int j = 0; j < frontFaceSegment - 1; j++)
                {
                    var index11 = j;
                    var index12 = index11 + 1;
                    var index21 = frontFaceSegment + j;
                    var index22 = index21 + 1;
                    GapMeshIndex.AddRange(new int[] { index11, index22, index21, index11, index12, index22 });
                }
                var GapMeshBufferData = new BufferGeometryData(BufferGeometryData.Types.mesh);
                GapMeshBufferData.position = MergeToSingleList<double[], double>(GapMeshVectors);
                GapMeshBufferData.normal = MergeToSingleList<double[], double>(GapMeshNormal);
                GapMeshBufferData.index = GapMeshIndex;
                frontFaceBufferData.Merge(GapMeshBufferData);

                var backFaceBufferData = frontFaceBufferData.Clone().ApplyMatrix(MatrixDot(
                        CreateTranslateMatrix(new Vector3D(0, 0, -b[i])),
                        CreateRotateMatrix(Axis.Z, (-b[i] / L[i]) * 2 * PI),
                        CreateMirrorMatrix(Axis.Z)
                        )).FlipSide();
                timeMeasurer.Report("前後端面作成");
                #endregion

                #region メッシュ統合
                var firstToothBufferData = new GearGeometryData() { z = z[i]};
                firstToothBufferData.Merge(rightFlankData, rightFilletData, rooData, leftFilletData, leftFlankData, tipData, frontFaceBufferData, backFaceBufferData);
                firstToothBufferData.name = i == 0 ? "pinionTooth" : "gearTooth";
                firstToothBufferData.SetType(BufferGeometryData.Types.mesh);
                firstToothBufferData.color = null;
                firstToothBufferData.castShadow = true;
                firstToothBufferData.receiveShadow = true;
                double[,] gearM;
                if (i == 0)
                    gearM = MatrixDot(
                        CreateTranslateMatrix(new Vector3D(-a * (double)z[i] / z.Sum(), 0, 0)),
                        CreateRotateMatrix(Axis.Z, -PI / 2));
                else
                    gearM = MatrixDot(
                        CreateTranslateMatrix(new Vector3D(a * (double)z[i] / z.Sum(), 0, 0)),
                        CreateRotateMatrix(Axis.Z, PI / 2 + PI / z[i]));
                firstToothBufferData.matrix =  (double[])MatrixTranspose(gearM).GetReshaped(new[] { 4 * 4 });
                timeMeasurer.Report("メッシュ統合");
                #endregion

                UpdateOrCreateGear(firstToothBufferData);
                timeMeasurer.Report("UpdateOrAddGear");
                //for (int j = 1; j < z[i]; j++)
                //{
                //    var theta = 2 * PI / z[i] * j;
                //    await EvalAsync($"SceneController.CopyMesh(" +
                //        $"'{firstToothBufferData.name}', new THREE.Matrix4().makeRotationZ({theta}));");
                //}
            }
            return true;
        }

    }
}
