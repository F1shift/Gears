using System;
using System.Collections.Generic;
using System.Text;
using static System.Math;
using static Gears.Math.Math;
using static Gears.Math.ArrayExtentions;

namespace Gears.Models
{
    /// <summary>
    /// 本計算はKHK小原歯車工業の公開資料
    /// 歯車技術資料(https://www.khkgears.co.jp/gear_technology/pdf/gijutu.pdf)に基づいたものであり、
    /// 変数名は歯車技術資料23ページの定義にご参照ください。
    /// https://ja.wikipedia.org/wiki/%E3%82%AE%E3%83%AA%E3%82%B7%E3%82%A2%E6%96%87%E5%AD%97
    /// </summary>
    class CylindricalGearBasic
    {
        public double mn { get; set; }
        public double αn { get; set; }
        public bool 歯車1が左ねじである { get; set; }
        public double β { get; set; }
        public int[] z { get; set; } = new int[2];
        public double[] xn { get; set; } = new double[2];
        public double αt { get; set; }
        public double αwt { get; set; }
        public double y { get; set; }
        public double a { get; set; }
        public double[] d { get; set; } = new double[2];
        public double[] db { get; set; } = new double[2];
        public double[] dw { get; set; } = new double[2];
        public double[] ha { get; set; } = new double[2];
        public double[] hf { get; set; } = new double[2];
        public double[] h { get; set; } = new double[2];
        public double[] da { get; set; } = new double[2];
        public double[] df { get; set; } = new double[2];
        public double[] b { get; set; } = new double[2];
        public double[] b_c { get; set; } = new double[2];
        public double ha_c { get; set; }
        public double hf_c { get; set; }

        public void SolveFromCenterDistance() {
            αt = Atan(Tan(αn) / Cos(β));
            y = a / mn - z.Sum() / (2 * Cos(β));
            αwt = Acos(Cos(αt) / (
                2 * y * Cos(β) / z.Sum() + 1
                ));
            double xn_Sum = z.Sum() * (Inv(αwt) - Inv(αt)) 
                / (2 * Tan(αn));
            xn[1] = xn_Sum - xn[0];
            SolveFromXn();
        }

        public void SolveFromXn() {
            歯車1が左ねじである = β < 0;
            αt = Atan(Tan(αn) / Cos(β));
            double invαwt = 2 * Tan(αn) * (xn.Sum() / z.Sum()) + Inv(αt);
            αwt = NonlinearFindRoot.FindRoot((α) => Inv(α) - invαwt, αt).Item1;
            y = z.Sum() / (2 * Cos(β)) * (Cos(αt) / Cos(αwt) - 1);
            a = (z.Sum() / (2 * Cos(β)) + y) * mn;
            for (int i = 0; i < 2; i++)
            {
                d[i] = z[i] * mn / Cos(β);
                db[i] = d[i] * Cos(αt);
                dw[i] = db[i] / Cos(αwt);
            }
            ha[0] = (ha_c + y - xn[1]) * mn;
            ha[1] = (ha_c + y - xn[0]) * mn;
            h[0] = h[1] = ((ha_c + hf_c) + y - xn.Sum()) * mn;
            for (int i = 0; i < 2; i++)
            {
                hf[i] = h[i] - ha[i];
                da[i] = d[i] + 2 * ha[i];
                df[i] = da[i] - 2 * h[i];
                b[i] = mn * b_c[i];
            }
        }
    }
}
