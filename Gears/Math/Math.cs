using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using static System.Math;

namespace Gears.Math
{
    public class Math
    {
        #region 單位轉換
        public static double DegToRad(double tdeg)
        {
            double trad;
            trad = tdeg / 180.0 * System.Math.PI;
            return trad;
        }
        public static double RadToDeg(double trad)
        {
            double tdeg;
            tdeg = trad / System.Math.PI * 180.0;
            return tdeg;
        }
        public static double mmToInch(double tmm)
        {
            double tInch;
            tInch = tmm / 25.4;
            return tInch;
        }
        public static double InchTomm(double tInch)
        {
            double tmm;
            tmm = tInch * 25.4;
            return tmm;
        }
        public static string DegToDMS(double x)
        {
            int x1 = (int)x;
            int x2 = (int)((x - x1) * 60);
            int x3 = (int)((((x - x1) * 60) - x2) * 60);
            string ss = "";
            ss = String.Format("{0}", x1) + "d" + String.Format("{0,3}", x2) + "m" + String.Format("{0,3}", x3) + "s";
            return ss;
        }
        public static string DegToDMS(double x,short n)
        {
            double absx = Abs(x);
            int x1 = (int)absx;
            int x2 = (int)((absx - x1) * 60);
            int x3 = (int)((((absx - x1) * 60) - x2) * 60);
            string ss= "";

            if (n == 3)
            {
                ss = String.Format("{0,3}", x1) + "D" + String.Format("{0,3}", x2) + "M" + String.Format("{0,3}", x3) + "S";
            }
            if (n == 2)
            {
                if (x1!=0)
                    ss = String.Format("{0,3}", x1) + "D" + String.Format("{0,3}", x2) + "M" + String.Format("{0,3}", x3) + "S";
                else
                    ss = String.Format("{0,3}", x2) + "M" + String.Format("{0,3}", x3) + "S";
            }
            if (n ==1)
            {
                if (x1 != 0)
                {
                    ss = String.Format("{0,3}", x1) + "D" + String.Format("{0,3}", x2) + "M" + String.Format("{0,3}", x3) + "S";
                }
                else
                {
                    if (x2 != 0)
                        ss = String.Format("{0,3}", x2) + "M" + String.Format("{0,3}", x3) + "S";
                    else
                        ss = String.Format("{0,3}", x3) + "S";
                }
            }

            if (absx / x == -1)
                ss ="-  " + ss;

            return ss;
        }
        public static string RadToDMS(double y)
        {
            string ss = DegToDMS(RadToDeg(y));
            return ss;
        }
        public static string RadToDMS(double y,short n)
        {
            string ss = DegToDMS(RadToDeg(y),n);
            return ss;
        }
        public static double DMSToDeg(double dd, double mm, double ss)
        {
            double deg = dd + mm / 60.0 + ss / 3600.0;
            return deg;
        }
        public static double DMSToRad(double dd, double mm, double ss)
        {
            double rad = DegToRad(DMSToDeg(dd, mm, ss));
            return rad;
        }
        #endregion

        public static double Inv(double alpha)
        {
            return Tan(alpha) - alpha;
        }

        public static class NonlinearFindRoot
        {
            public static string nError = "";
            public static TimeSpan TimeSpanLimit = new TimeSpan(0, 0, 5);
            public delegate void EquationSet(double[] input, double[] output);

            /// <summary>
            /// return item1:root item2:{times up, check}
            /// </summary>
            /// <param name="Equation"></param>
            /// <param name="InitialGuest"></param>
            /// <returns></returns>
            public static Tuple<double, bool[]> FindRoot(Func<double, double> Equation, double InitialGuest)
            {
                double[] x = new double[] { InitialGuest };
                EquationSet aEquationSet = (double[] xx, double[] yy) => { yy[0] = Equation(xx[0]); };
                Boolean[] Result = FindRoot(x, aEquationSet);
                return new Tuple<double, bool[]>(x[0], Result);
            }

            /// <summary>
            /// return item1:root item2:{times up, check}
            /// </summary>
            /// <param name="Equation"></param>
            /// <param name="InitialGuest"></param>
            /// <returns></returns>
            public static Tuple<double[], bool[]> FindRoot(Func<double[], double[]> Equation, double[] InitialGuest)
            {
                double[] x = new double[InitialGuest.Length];
                InitialGuest.CopyTo(x, 0);
                EquationSet aEquationSet = (double[] xx, double[] yy) => 
                    {
                        double[] current = Equation(xx);
                        yy[0] = current[0];
                        yy[1] = current[1];
                    };
                Boolean[] Result = FindRoot(x, aEquationSet);
                return new Tuple<double[],bool[]> (x, Result);
            }
            public static bool[] FindRoot(double[] x, EquationSet objEquationSet)
            {
                bool Check = false;
                bool timesUP = newt(x, objEquationSet, Check);
                bool[] states = new bool[]{timesUP, Check};

                return states;
            }
            private static bool newt(double[] x, EquationSet objEquationSet, bool Check)
            {
                const int MAXITS = 200;
                const double TOLF = 1E-08;
                const double TOLMIN = 1E-10;
                const int STPMX = 100;
                const double TOLX = 1E-11;

                bool timsUP = false;

                double den, f, fold, stpmax, sum, temp, test;
                double d = 0;
                int n = x.Length;
                int[] indx = new int[n];
                double[] g = new double[n];
                double[] p = new double[n];
                double[] xold = new double[n];
                double[,] fjac = new double[n, n];
                double[] fvec = new double[n];

                f = fmin(x, fvec, objEquationSet);
                test = 0.0;
                for (int i = 0; i < n; i++)
                    if ((Abs(fvec[i]) > test)) test = Abs(fvec[i]);
                if (test < 0.01 * TOLF)
                {
                    Check = false;
                    return timsUP;
                }
                sum = 0;
                for (int i = 0; i < n; i++) sum = sum + x[i] * x[i];
                stpmax = STPMX * Max(Sqrt(sum), Convert.ToDouble(n));
                for (int its = 0; its < MAXITS; its++)
                {
                    fdjac(x, fvec, fjac, objEquationSet);
                    for (int i = 0; i < n; i++)
                    {
                        sum = 0;
                        for (int j = 0; j < n; j++) sum = sum + fjac[j, i] * fvec[j];
                        g[i] = sum;
                    }
                    for (int i = 0; i < n; i++) xold[i] = x[i];
                    fold = f;
                    for (int i = 0; i < n; i++) p[i] = -fvec[i];
                    ludcmp(fjac, indx, d);
                    lubksb(fjac, indx, p);
                    timsUP = lnsrch(xold, fold, g, p, x, f, stpmax, Check, fvec, objEquationSet);
                    if (timsUP)
                    {
                        x[0] = 0;
                        return timsUP;
                    } 
                    test = 0.0;
                    for (int i = 0; i < n; i++)
                        if (Abs(fvec[i]) > test) test = Abs(fvec[i]);
                    if (test < TOLF)
                    {
                        Check = false;
                        return timsUP;
                    }
                    if (Check)
                    {
                        test = 0.0;
                        den = Max(f, 0.5 * n);
                        for (int i = 0; i < n; i++)
                        {
                            temp = Abs(g[i]) * Max(Abs(x[i]), 1.0) / den;
                            if (temp > test) test = temp;
                        }
                        Check = (test < TOLMIN);
                        return timsUP;
                    }
                    test = 0.0;
                    for (int i = 0; i < n; i++)
                    {
                        temp = (Abs(x[i] - xold[i])) / Max(Abs(x[i]), 1.0);
                        if (temp > test) test = temp;
                    }
                    if (test < TOLX) return timsUP;
                }
                return timsUP;
            }
            private static void ludcmp(double[,] a, int[] indx, double d)
            {
                const double TINY = 1E-20;
                int imax = 0;
                double big, dum, sum, temp;

                int n = a.GetLength(0);
                double[] vv = new double[n];

                d = 1;

                for (int i = 0; i < n; i++)
                {
                    big = 0;
                    for (int j = 0; j < n; j++)
                        if ((temp = Abs(a[i, j])) > big) big = temp;
                    if (big == 0) nError = "Singular matrix in routine ludcmp";
                    vv[i] = 1 / big;
                }
                for (int j = 0; j < n; j++)
                {
                    for (int i = 0; i < j; i++)
                    {
                        sum = a[i, j];
                        for (int k = 0; k < i; k++) sum = sum - a[i, k] * a[k, j];
                        a[i, j] = sum;
                    }
                    big = 0.0;
                    for (int i = j; i < n; i++)
                    {
                        sum = a[i, j];
                        for (int k = 0; k < j; k++) sum = sum - a[i, k] * a[k, j];
                        a[i, j] = sum;
                        dum = vv[i] * Abs(sum);
                        if ((dum = vv[i] * Abs(sum)) >= big)
                        {
                            big = dum;
                            imax = i;
                        }
                    }
                    if (j != imax)
                    {
                        for (int k = 0; k < n; k++)
                        {
                            dum = a[imax, k];
                            a[imax, k] = a[j, k];
                            a[j, k] = dum;
                        }
                        d = -d;
                        vv[imax] = vv[j];
                    }
                    indx[j] = imax;
                    if (a[j, j] == 0) a[j, j] = TINY;

                    if (j != n - 1)
                    {
                        dum = 1.0 / a[j, j];
                        for (int i = j + 1; i < n; i++) a[i, j] = a[i, j] * dum;
                    }
                }
            }
            private static void lubksb(double[,] a, int[] indx, double[] b)
            {
                int ii = 0, ip;
                double sum;

                int n = a.GetLength(0);
                for (int i = 0; i < n; i++)
                {
                    ip = indx[i];
                    sum = b[ip];
                    b[ip] = b[i];
                    if (ii != 0)
                        for (int j = ii - 1; j < i; j++) sum = sum - a[i, j] * b[j];
                    else if (sum != 0) ii = i + 1;
                    b[i] = sum;
                }
                for (int i = n - 1; i >= 0; i--)
                {
                    sum = b[i];
                    for (int j = i + 1; j < n; j++) sum = sum - a[i, j] * b[j];
                    b[i] = sum / a[i, i];
                }
            }
            private static bool lnsrch(double[] xold, double fold, double[] g, double[] p, double[] x, double f, double stpmax, bool Check, double[] fvec, EquationSet objEquationSet)
            {
                const double ALF = 1E-08;
                const double TOLX = 1E-11;

                double a, alam, alam2 = 0.0, alamin, b, disc, f2 = 0.0;
                double rhs1, rhs2, slope, sum, temp, test, tmplam;

                int n = xold.Length;
                Check = false;
                sum = 0.0;
                for (int i = 0; i < n; i++) sum = sum + p[i] * p[i];
                sum = Sqrt(sum);
                if (sum > stpmax)
                    for (int i = 0; i < n; i++) p[i] = p[i] * stpmax / sum;
                slope = 0.0;
                for (int i = 0; i < n; i++) slope = slope + g[i] * p[i];
                if (slope >= 0.0) nError = "Roundoff problem in lnsrch.";
                test = 0.0;
                for (int i = 0; i < n; i++)
                {
                    temp = Abs(p[i]) / Max(Abs(xold[i]), 1);
                    if (temp > test) test = temp;
                }
                alamin = TOLX / test;
                alam = 1.0;
                DateTime startT = DateTime.Now;
                int i_times = 0;
                while(true)
                {
                    if (i_times == 100)
                    {
                        i_times = 0;
                        if (DateTime.Now - startT > TimeSpanLimit)
                        {
                            Debug.WriteLine("NonLinear Find Root Overtime({0}s)", TimeSpanLimit.Seconds);
                            return true;
                        }
                    }

                    for (int i = 0; i < n; i++) x[i] = xold[i] + alam * p[i];
                    f = fmin(x, fvec, objEquationSet);
                    if (alam < alamin)
                    {
                        for (int i = 0; i < n; i++) x[i] = xold[i];
                        Check = true;
                        return false;
                    }
                    else if (f <= fold + ALF * alam * slope) return false;
                    else
                    {
                        if (alam == 1.0) tmplam = -slope / (2.0 * (f - fold - slope));
                        else
                        {
                            rhs1 = f - fold - alam * slope;
                            rhs2 = f2 - fold - alam2 * slope;
                            a = (rhs1 / (alam * alam) - rhs2 / (alam2 * alam2)) / (alam - alam2);
                            b = (-alam2 * rhs1 / (alam * alam) + alam * rhs2 / (alam2 * alam2)) / (alam - alam2);

                            if (a == 0.0) tmplam = -slope / (2.0 * b);
                            else
                            {
                                disc = b * b - 3.0 * a * slope;
                                if (disc < 0.0) tmplam = 0.5 * alam;
                                else if (b <= 0.0) tmplam = (-b + Sqrt(disc)) / (3.0 * a);
                                else tmplam = -slope / (b + Sqrt(disc));
                            }
                            if (tmplam > 0.5 * alam) tmplam = 0.5 * alam;
                        }
                    }

                    alam2 = alam;
                    f2 = f;
                    alam = Max(tmplam, 0.1 * alam);

                    i_times++;
                }
            }
            private static void fdjac(double[] x, double[] fvec, double[,] df, EquationSet objEquationSet)
            {
                const double EPS = 1E-08;

                double h, temp;

                int n = x.Length;
                double[] f = new double[n];

                for (int j = 0; j < n; j++)
                {
                    temp = x[j];
                    h = EPS * Abs(temp);
                    if (h == 0) h = EPS;
                    x[j] = temp + h;
                    h = x[j] - temp;
                    objEquationSet(x, f);
                    x[j] = temp;
                    for (int i = 0; i < n; i++) df[i, j] = (f[i] - fvec[i]) / h;
                }
            }
            private static double fmin(double[] x, double[] fvec, EquationSet objEquationSet)
            {
                objEquationSet(x, fvec);

                int n = x.Length;
                double sum = 0.0;
                for (int i = 0; i < n; i++) sum = sum + fvec[i] * fvec[i];
                return 0.5 * sum;
            }
            private static double Max(double a, double b)
            {
                double tMax = a;
                if (tMax < b) tMax = b;
                return tMax;
            }
        }

        public static class Differential
        {
            public static double Mid2PDiff(Func<double, double> f, double xi, double h)
            {
                double dfdx = (f(xi - h) - f(xi + h)) / 2 / h;
                return dfdx;
            }
        }

        public static class Integration
        {
            const int JMAX = 20, JMAXP = JMAX + 1, K = 5;
            const double EPS = 1.0e-10;
            public delegate double Equation(double input);


            static double s = 0;

            public static double trapzd(Equation func, double a, double b, int n)
            {
                double x, tnm, sum, del;
                //double s=0;

                int it, j;

                if (n == 1)
                {
                    return (s = 0.5 * (b - a) * (func(a) + func(b)));
                }
                else
                {
                    for (it = 1, j = 1; j < n - 1; j++) it <<= 1;
                    tnm = it;
                    del = (b - a) / tnm;
                    x = a + 0.5 * del;
                    for (sum = 0.0, j = 0; j < it; j++, x += del) sum += func(x);
                    s = 0.5 * (s + (b - a) * sum / tnm);
                    return s;
                }
            }

            public static void polint(double[] xa, double[] ya, double x, ref double y, ref double dy)
            {
                int i, m, ns = 0;
                double den, dif, dift, ho, hp, w;

                int n = xa.Length;
                double[] c = new double[n];
                double[] d = new double[n];

                dif = Abs(x - xa[0]);
                for (i = 0; i < n; i++)
                {
                    if ((dift = Abs(x - xa[i])) < dif)
                    {
                        ns = i;
                        dif = dift;
                    }
                    c[i] = ya[i];
                    d[i] = ya[i];
                }
                y = ya[ns--];
                for (m = 1; m < n; m++)
                {
                    for (i = 0; i < n - m; i++)
                    {
                        ho = xa[i] - x;
                        hp = xa[i + m] - x;
                        w = c[i + 1] - d[i];
                        den = ho - hp;
                        //if ((den=ho-hp) == 0.0) nrerror("Error in routine polint");
                        den = w / den;
                        d[i] = hp * den;
                        c[i] = ho * den;
                    }
                    y += (dy = (2 * (ns + 1) < (n - m) ? c[ns + 1] : d[ns--]));
                }
            }

            public static double qromb(Equation func, double a, double b)
            {

                double ss = 0, dss = 0;
                double[] s = new double[JMAX];
                double[] h = new double[JMAXP];
                double[] s_t = new double[K];
                double[] h_t = new double[K];
                int i, j;

                h[0] = 1.0;
                for (j = 1; j <= JMAX; j++)
                {
                    s[j - 1] = trapzd(func, a, b, j);
                    if (j >= K)
                    {
                        for (i = 0; i < K; i++)
                        {
                            h_t[i] = h[j - K + i];
                            s_t[i] = s[j - K + i];
                        }
                        polint(h_t, s_t, 0.0, ref ss, ref dss);
                        if (Abs(dss) <= EPS * Abs(ss)) return ss;
                    }
                    h[j] = 0.25 * h[j - 1];
                }
                //nrerror("Too many steps in routine qromb");
                return 0.0;
            }
        }
    }

    
}