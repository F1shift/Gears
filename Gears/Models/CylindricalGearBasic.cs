using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using static System.Math;
using static Gears.Utility.Math;
using static Gears.Utility.EnumerableExtentions;

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
        [IsInput]
        public double mn { get; set; }
        [IsInput]
        public double αn { get; set; } 
        [IsInput]
        public double β { get; set; } 
        [IsInput]
        public int[] z { get; set; } = new int[2];
        [IsInput]
        public double[] xn { get; set; } = new double[2];
        [IsInput]
        public double[] ρ_c { get; set; } = new double[2];
        [IsInput]
        public double[] b_c { get; set; } = new double[2];
        [IsInput]
        public double ha_c { get; set; }
        [IsInput]
        public double hf_c { get; set; }
        public double mt { get; set; }//端面モジュール
        public double αt { get; set; }
        public double αwt { get; set; }
        public double[] L { get; set; } = new double[2];//リード
        public double y { get; set; }
        public double a { get; set; }
        public double[] d { get; set; } = new double[2];
        public double[] db { get; set; } = new double[2];
        public double[] dw { get; set; } = new double[2];
        public double[] ha { get; set; } = new double[2];
        public double[] hf { get; set; } = new double[2];//歯元高さ
        public double[] h { get; set; } = new double[2];
        /// <summary>
        /// 歯元円径(カッターの歯先円径)
        /// </summary>
        public double[] ρ { get; set; } = new double[2];
        /// <summary>
        /// 歯元円径係数(カッターの歯先円径係数)
        /// </summary>
        public double[] da { get; set; } = new double[2];
        public double[] df { get; set; } = new double[2];
        public double[] b { get; set; } = new double[2];
        

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
            mt = mn / Cos(β);
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
                ρ[i] = ρ_c[i] * mn;
            }
            L[0] = d[0] * PI / Tan(β);
            L[1] = d[1] * PI / Tan(-β);

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

        public void SetToDefualt() {
            this.mn = 3;
            this.αn = 20 / 180.0 * PI;
            this.β = 15 / 180.0 * PI;
            this.ρ_c[0] = this.ρ_c[1] = 0.35;
            this.ha_c = 1;
            this.hf_c = 1;
            this.z[0] = 12;
            this.z[1] = 60;
            this.b_c[0] = this.b_c[1] = 20;
            this.xn[0] = 0.09809;
            this.xn[1] = 0;
        }
        public void CopyInputFrom(CylindricalGearBasic target) {
            var type = typeof(CylindricalGearBasic);
            foreach (var property in type.GetProperties())
            {
                if (Attribute.GetCustomAttribute(property, typeof(IsInputAttribute)) != null)
                {
                    var thisValue = property.GetValue(this);
                    var targetValue = property.GetValue(target);
                    if (thisValue != targetValue)
                    {
                        property.SetValue(this, targetValue);
                    }
                }
            }
        }
        public void CopyInputTo(CylindricalGearBasic target)
        {
            var type = typeof(CylindricalGearBasic);
            foreach (var property in type.GetProperties())
            {
                if (Attribute.GetCustomAttribute(property, typeof(IsInputAttribute)) != null)
                {
                    var thisValue = property.GetValue(this);
                    var targetValue = property.GetValue(target);
                    if (thisValue != targetValue)
                    {
                        property.SetValue(target, thisValue);
                    }
                }
            }
        }
        public bool HasSameInputWith(CylindricalGearBasic  target)
        {
            if (target == null)
            {
                return false;
            }
            if (this.mn != target.mn) return false;
            if (this.αn != target.αn) return false;
            if (this.β != target.β) return false;
            if (this.z[0] != target.z[0]) return false;
            if (this.z[1] != target.z[1]) return false;
            if (this.xn[0] != target.xn[0]) return false;
            if (this.xn[1] != target.xn[1]) return false;
            if (this.ρ_c[0] != target.ρ_c[0]) return false;
            if (this.ρ_c[1] != target.ρ_c[1]) return false;
            if (this.b_c[0] != target.b_c[0]) return false;
            if (this.b_c[1] != target.b_c[1]) return false;
            if (this.ha_c != target.ha_c) return false;
            if (this.hf_c != target.hf_c) return false;
            return true;
        }

        [AttributeUsage(AttributeTargets.Property)]
        class IsInputAttribute : Attribute
        {
        }
    }
}
