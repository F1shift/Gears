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
            public Func<double, Vector3D> Tip { get; set; }
        }

        public BasicRack[] BasicRacks { get; set; } = new BasicRack[2];
        public void SolveProfile() {
            for (int ii = 0; ii < 2; ii++)
            {
                int i = ii;
                //基準ラック輪郭線を計算
                Vector3D p1 = new Vector3D(-mt * PI / 4 + (hf[i] - ρ[i] * (1 - Sin(αn))) * Tan(αt), ha[0], 0 );
                Vector3D p2 = new Vector3D( -mt * PI / 4 - ha[i] * Tan(αt), ha[0], 0 );
                BasicRacks[i] = new BasicRack();
                BasicRacks[i].Flank_Left = new Func<double, Vector3D>(
                (u) =>
                {
                    var p = p1 + u * p2;
                    return p;
                });
                BasicRacks[i].Flank_Right = new Func<double, Vector3D>(
                (u) =>
                {
                    var p = BasicRacks[i].Flank_Left(u);
                    p.X *= -1;
                    return p;
                });
                Vector3D p
                BasicRacks[i].Fillet_Left = new Func<double, Vector3D>(
                (u) =>
                {
                    var p = BasicRacks[i].Flank_Left(u);
                    p.X *= -1;
                    return p;
                });
            }
        }
    }
}
