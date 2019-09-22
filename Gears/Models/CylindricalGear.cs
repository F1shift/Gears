using System;
using System.Collections.Generic;
using System.Text;
using Gears.Math;
using static System.Math;
using static Gears.Math.Math;
using static Gears.Math.ArrayExtentions;

namespace Gears.Models
{
    /// <summary>
    /// 本計算はKHK小原歯車工業の公開資料による作ったCylindricalGearBaseをもとに
    /// 噛み合い方程式で歯車3D形状を求めるメソッドを実装したクラス。
    /// </summary>
    internal class CylindricalGear : CylindricalGearBasic
    {
        internal class BasicRack
        {
            public Func<double, Vector3D> Flank_Left { get; set; }
            public Func<double, Vector3D> Flank_Right { get; set; }
            public Func<double, Vector3D> Fillet_Left { get; set; }
            public Func<double, Vector3D> Fillet_Right { get; set; }
            public Func<double, Vector3D> Root { get; set; }

            public Func<double, Vector3D> Flank_Left_Normal { get; set; }
            public Func<double, Vector3D> Flank_Right_Normal { get; set; }
            public Func<double, Vector3D> Fillet_Left_Normal { get; set; }
            public Func<double, Vector3D> Fillet_Right_Normal  { get; set; }
            public Func<double, Vector3D> Root_Normal { get; set; }
        }

        public BasicRack[] BasicRacks { get; set; } = new BasicRack[2];

        internal class RackTrace
        {
            public Func<double, double, Vector3D> Flank_Left { get; set; }
            public Func<double, double, Vector3D> Flank_Right { get; set; }
            public Func<double, double, Vector3D> Fillet_Left { get; set; }
            public Func<double, double, Vector3D> Fillet_Right { get; set; }
            public Func<double, double, Vector3D> Root { get; set; }
            public Func<double, double, Vector3D> Flank_Left_Normal { get; set; }
            public Func<double, double, Vector3D> Flank_Right_Normal { get; set; }
            public Func<double, double, Vector3D> Fillet_Left_Normal { get; set; }
            public Func<double, double, Vector3D> Fillet_Right_Normal { get; set; }
            public Func<double, double, Vector3D> Root_Normal { get; set; }
            public Func<double, Vector3D, Vector3D> GetSpeed { get; set; }
        }
        public RackTrace[] RackTraces { get; set; } = new RackTrace[2];

        public void SolveProfile() {
            for (int ii = 0; ii < 2; ii++)
            {
                int i = ii;
                //基準ラック輪郭線を定義
                Vector3D p1 = new Vector3D(-mt * PI / 4 + (hf_c * mn - ρ[i] * (1 - Sin(αn))) * Tan(αt), -(hf_c * mn - ρ[i] * (1 - Sin(αn))), 0 );
                Vector3D p2 = new Vector3D( -mt * PI / 4 - ha_c * mn * Tan(αt), ha_c * mn, 0 );
                Vector3D p1Top2 = p2 - p1;
                BasicRacks[i] = new BasicRack();
                BasicRacks[i].Flank_Left = (u) =>
                {
                    var p = p1 + u * p1Top2;
                    return p;
                };
                BasicRacks[i].Flank_Right = (u) =>
                {
                    var p = BasicRacks[i].Flank_Left(u);
                    p.X *= -1;
                    return p;
                };
                Vector3D FilletCenter = new Vector3D(( - mn * PI / 4 + hf_c * mn * Tan(αn) + ρ[i] * Tan((PI / 2 - αn) / 2)), -hf_c * mn + ρ[i], 0);//FilletCenter on normal plane
                BasicRacks[i].Fillet_Left = (u) =>
                {
                    var angle = -PI / 2 - (PI / 2 - αn) * u;
                    var p = FilletCenter + new Vector3D(Cos(angle), Sin(angle), 0) * ρ[i];
                    p.X /= Cos(β);
                    return p;
                };
                BasicRacks[i].Fillet_Right = (u) =>
                    {
                        var p = BasicRacks[i].Fillet_Left(u);
                        p.X *= -1;
                        return p;
                    };
                Vector3D rootLeftVector = new Vector3D(FilletCenter);
                rootLeftVector.X /= Cos(β);
                rootLeftVector.Y -= ρ[i];
                BasicRacks[i].Root = (u) =>
                    {
                        var p = new Vector3D(rootLeftVector.X - 2 * (rootLeftVector.X) * u, rootLeftVector.Y, 0);
                        return p;
                    };
                var flankNormal_Left = new Vector3D(-Cos(αt), -Sin(αt), 0);
                var flankNormal_Right = new Vector3D(Cos(αt), -Sin(αt), 0);
                BasicRacks[i].Flank_Left_Normal = (u) => flankNormal_Left;
                BasicRacks[i].Flank_Right_Normal = (u) => flankNormal_Right;
                BasicRacks[i].Fillet_Left_Normal = (u) =>
                {
                    var angle = -PI / 2 - (PI / 2 - αn) * u;
                    var v = new Vector3D(-ρ[i] * Cos(angle), -ρ[i] / Cos(β) * Sin(angle), 0);
                    v.Normalize();
                    return v;
                };
                BasicRacks[i].Fillet_Right_Normal = (u) =>
                {
                    var v = BasicRacks[i].Fillet_Left_Normal(u);
                    v.X *= -1;
                    return v;
                };
                var rootNormal = new Vector3D(0, -1, 0);
                BasicRacks[i].Root_Normal = (u) => rootNormal;
                //ラック軌跡方程式を定義
                //vは歯車を固定にし、ラックを回転させるときのラック角度、単位は(°)
                //タックの初期位置は、歯車の+Y軸方向に水平に置く。
                //詳細は https://www.khkgears.co.jp/gear_technology/pdf/gijutu.pdf ページ11に参考。
                Func<double, double[,]> getMetrix = (v) =>
                {
                    var θ = v / 180 * PI;
                    var mRotate = CreateRotateMatrix(Axis.Z, θ);
                    var r = d[i] / 2 + xn[i] * mn;
                    var L = d[i] / 2 * θ;
                    var mTransition = CreateTranslateMatrix(new Vector3D(L, r, 0));
                    var M = MatrixDot(mRotate, mTransition);
                    return M;
                };
                RackTraces[i] = new RackTrace();
                RackTraces[i].Flank_Left = (u, v) =>
                {
                    var org = BasicRacks[i].Flank_Left(u);
                    var m = getMetrix(v);
                    var moved = RotateAnMoveVector(m, org);
                    return moved;
                };
                RackTraces[i].Flank_Right = (u, v) =>
                {
                    var org = BasicRacks[i].Flank_Right(u);
                    var m = getMetrix(v);
                    var moved = RotateAnMoveVector(m, org);
                    return moved;
                };
                RackTraces[i].Fillet_Left = (u, v) =>
                {
                    var org = BasicRacks[i].Fillet_Left(u);
                    var m = getMetrix(v);
                    var moved = RotateAnMoveVector(m, org);
                    return moved;
                };
                RackTraces[i].Fillet_Right = (u, v) =>
                {
                    var org = BasicRacks[i].Fillet_Right(u);
                    var m = getMetrix(v);
                    var moved = RotateAnMoveVector(m, org);
                    return moved;
                };
                RackTraces[i].Root = (u, v) =>
                {
                    var org = BasicRacks[i].Root(u);
                    var m = getMetrix(v);
                    var moved = RotateAnMoveVector(m, org);
                    return moved;
                };
                RackTraces[i].Flank_Left_Normal = (u, v) =>
                {
                    var org = BasicRacks[i].Flank_Left_Normal(u);
                    var m = getMetrix(v);
                    var moved = RotateVector(m, org);
                    return moved;
                };
                RackTraces[i].Flank_Right_Normal = (u, v) =>
                {
                    var org = BasicRacks[i].Flank_Right_Normal(u);
                    var m = getMetrix(v);
                    var moved = RotateVector(m, org);
                    return moved;
                };
                RackTraces[i].Fillet_Left_Normal = (u, v) =>
                {
                    var org = BasicRacks[i].Fillet_Left_Normal(u);
                    var m = getMetrix(v);
                    var moved = RotateVector(m, org);
                    return moved;
                };
                RackTraces[i].Fillet_Right_Normal = (u, v) =>
                {
                    var org = BasicRacks[i].Fillet_Right_Normal(u);
                    var m = getMetrix(v);
                    var moved = RotateVector(m, org);
                    return moved;
                };
                RackTraces[i].Root_Normal = (u, v) =>
                {
                    var org = BasicRacks[i].Root_Normal(u);
                    var m = getMetrix(v);
                    var moved = RotateVector(m, org);
                    return moved;
                };
                RackTraces[i].GetSpeed = (v, Position) =>
                {
                    var θ = v / 180 * PI;
                    var r0 = d[i] / 2 + xn[i] * mn;
                    var instantCenter = new Vector3D(-Sin(θ), Cos(θ), 0) * r0;
                    var r1 = Position - instantCenter;
                    var speed = (Vector3D)Cross(new Vector3D(0, 0, 1), r1) / 180 * PI;
                    return speed;
                };
            }
        }
    }
}
