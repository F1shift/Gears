using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using static Gears.ThreeDUtility.ThreeDUtility;

namespace Gears.ThreeDUtility
{
    public class Vector3D
    {
        public double[] buffer;
        public double X { get { return buffer[0]; } set { buffer[0] = value; } }
        public double Y { get { return buffer[1]; } set { buffer[1] = value; } }
        public double Z { get { return buffer[2]; } set { buffer[2] = value; } }
        public double this[int index]
        {
            get
            {
                return buffer[index];
            }
            set
            {
                buffer[index] = value;
            }
        }

        #region Constructor and Destructor
        public Vector3D(double x, double y, double z)
        {
            buffer = new double[] { x, y, z };
        }

        public Vector3D(double[] vector, bool asBuffer)
        {
            if (asBuffer == true)
            {
                this.buffer = vector;
            }
            else
            {
                this.buffer = new double[vector.Length];
                vector.CopyTo(this.buffer, 0);
            }
        }

        public Vector3D()
        {
            this.buffer = new double[] { 0, 0, 0};
        }

        public Vector3D(Vector3D vector)
        {
            this.buffer = new double[vector.buffer.Length];
            vector.buffer.CopyTo(this.buffer, 0);
        }
        #endregion

        #region Operation
        public static Vector3D operator -(Vector3D p1, Vector3D p2)
        {
            Vector3D p3 = new Vector3D();

            p3.X = p1.X - p2.X;
            p3.Y = p1.Y - p2.Y;
            p3.Z = p1.Z - p2.Z;

            return p3;
        }
        public static Vector3D operator +(Vector3D p1, Vector3D p2)
        {
            Vector3D p3 = new Vector3D();

            p3.X = p1.X + p2.X;
            p3.Y = p1.Y + p2.Y;
            p3.Z = p1.Z + p2.Z;

            return p3;
        }
        public static Vector3D operator *(double scale, Vector3D p1)
        {
            Vector3D po = new Vector3D();

            po.X = scale * p1.X;
            po.Y = scale * p1.Y;
            po.Z = scale * p1.Z;

            return po;
        }
        public static Vector3D operator *(Vector3D p1, double scale)
        {
            Vector3D po = new Vector3D();

            po.X = scale * p1.X;
            po.Y = scale * p1.Y;
            po.Z = scale * p1.Z;

            return po;
        }
        public static Vector3D operator /(Vector3D p1, double denominator)
        {
            Vector3D po = new Vector3D();

            po.X = p1.X / denominator;
            po.Y = p1.Y / denominator;
            po.Z = p1.Z / denominator;

            return po;
        }
        public static implicit operator double[] (Vector3D v)
        {
            return v.buffer;
        }
        public static implicit operator Vector3D(double[] buffer)
        {
            return new Vector3D(buffer, true);
        }
        #endregion

        public object Clone()
        {
            Vector3D v = new Vector3D(X, Y, Z);
            return v;
        }

        public override string ToString()
        {
            return "{" + this.X + "," + this.Y + "," + this.Z + "}";
        }

        public string ToString(string Format)
        {
            return "{" + this.X.ToString(Format) + "," + this.Y.ToString(Format) + "," + this.Z.ToString(Format) + "}";
        }

        public double[] ToArray()
        {
            return new double[] { this.X, this.Y, this.Z };
        }
    }
}
