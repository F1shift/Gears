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
        }

        public BasicRack[] BasicRacks { get; set; } = new BasicRack[2];
        public void SolveProfile() {
            for (int ii = 0; ii < 2; ii++)
            {
                int i = ii;
                //基準ラック輪郭線を計算
                Vector3D p1 = new Vector3D(-mt * PI / 4 + (hf_c * mn - ρ[i] * (1 - Sin(αn))) * Tan(αt), -(hf_c * mn - ρ[i] * (1 - Sin(αn))), 0 );
                Vector3D p2 = new Vector3D( -mt * PI / 4 - ha_c * mn * Tan(αt), ha_c * mn, 0 );
                Vector3D p1Top2 = p2 - p1;
                BasicRacks[i] = new BasicRack();
                BasicRacks[i].Flank_Left = new Func<double, Vector3D>(
                    (u) =>
                {
                    var p = p1 + u * p1Top2;
                    return p;
                });
                BasicRacks[i].Flank_Right = new Func<double, Vector3D>(
                    (u) =>
                {
                    var p = BasicRacks[i].Flank_Left(u);
                    p.X *= -1;
                    return p;
                });
                Vector3D FilletCenter = new Vector3D(( - mn * PI / 4 + hf_c * mn * Tan(αn) + ρ[i] * Tan((PI / 2 - αn) / 2)), -hf_c * mn + ρ[i], 0);//FilletCenter on normal plane
                BasicRacks[i].Fillet_Left = new Func<double, Vector3D>(
                    (u) =>
                {
                    var angle = -PI / 2 - (PI / 2 - αn) * u;
                    var p = FilletCenter + new Vector3D(Cos(angle), Sin(angle), 0) * ρ[i];
                    p.X /= Cos(β);
                    return p;
                });
                BasicRacks[i].Fillet_Right = new Func<double, Vector3D>(
                    (u) =>
                    {
                        var p = BasicRacks[i].Fillet_Left(u);
                        p.X *= -1;
                        return p;
                    });
                Vector3D rootLeftVector = new Vector3D(FilletCenter);
                rootLeftVector.X /= Cos(β);
                rootLeftVector.Y -= ρ[i];
                BasicRacks[i].Root = new Func<double, Vector3D>(
                    (u) =>
                    {
                        var p = new Vector3D(rootLeftVector.X - 2 * (rootLeftVector.X) * u, rootLeftVector.Y, 0);
                        return p;
                    });
                
            }
        }
    }
}
