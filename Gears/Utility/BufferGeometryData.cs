using System;
using System.Collections.Generic;
using System.Text;
using static Gears.Utility.Math;

namespace Gears.Utility
{
    class BufferGeometryData
    {
        List<double> _position;
        public List<double> position { get {
                if (_position == null)
                {
                    _position = new List<double>();
                }
                return _position;
            }
            set
            {
                if (value != _position)
                {
                    _position = value;
                }
            }
        }
        List<double> _normal;
        public List<double> normal
        {
            get
            {
                if (_normal == null)
                {
                    _normal = new List<double>();
                }
                return _normal;
            }
            set
            {
                if (value != _normal)
                {
                    _normal = value;
                }
            }
        }
        List<int> _index;
        public List<int> index
        {
            get
            {
                if (_index == null)
                {
                    _index = new List<int>();
                }
                return _index;
            }
            set
            {
                if (value != _index)
                {
                    _index = value;
                }
            }
        }

        public object color { get; set; } = 0xFFFFFF;
        public bool castShadow { get; set; } = false;
        public object receiveShadow { get; set; } = false;
        public string type { get; protected set; }
        public string name { get; set; }

        public enum Types
        {
            line,
            mesh
        }

        public BufferGeometryData(Types type)
        {
            SetType(type);
        }

        /// <summary>
        /// Mesh, Line...etc
        /// </summary>
        /// <param name="type"></param>
        /// <returns>return itself like jquery object</returns>
        public BufferGeometryData SetType(Types type)
        {
            this.type = Enum.GetName(typeof(Types), type);
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffers"></param>
        /// <returns>return itself like jquery object</returns>
        public BufferGeometryData Merge(params BufferGeometryData[] buffers)
        {
            foreach (var buffer in buffers)
            {
                int pointCount = this.position.Count / 3;
                this.position.AddRange(buffer.position);
                this.normal.AddRange(buffer.normal);
                for (int i = 0; i < buffer.index.Count; i++)
                {
                    this.index.Add(buffer.index[i] + pointCount);
                }
            }
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns>return itself like jquery object</returns>
        public BufferGeometryData ApplyMatrix(double[,] matrix)
        {
            for (int i = 0; i < position.Count / 3; i++)
            {
                var p = new Vector3D(position[i * 3], position[i * 3 + 1], position[i * 3 + 2]);
                p = RotateAnMoveVector(matrix, p);
                position[i * 3] = p.X;
                position[i * 3 + 1] = p.Y;
                position[i * 3 + 2] = p.Z;
            }

            for (int i = 0; i < normal.Count / 3; i++)
            {
                var n = new Vector3D(normal[i * 3], normal[i * 3 + 1], normal[i * 3 + 2]);
                n = RotateVector(matrix, n);
                normal[i * 3] = n.X;
                normal[i * 3 + 1] = n.Y;
                normal[i * 3 + 2] = n.Z;
            }
            return this;
        }

        public BufferGeometryData FlipSide() {
            for (int i = 0; i < index.Count / 3; i++)
            {
                var index1 = index[i * 3];
                var index2 = index[i * 3 + 1];
                var index3 = index[i * 3 + 2];
                index[i * 3] = index3;
                index[i * 3 + 1] = index2;
                index[i * 3 + 2] = index1;
            }
            return this;
        }

        public BufferGeometryData FlipNormal()
        {
            for (int i = 0; i < normal.Count; i++)
            {
                normal[i] *= -1;
            }
            return this;
        }

        public BufferGeometryData Clone(string newName = null) {
            var newobj = new BufferGeometryData((Types)Enum.Parse(typeof(Types), this.type));
            newobj.position.AddRange(position);
            newobj.normal.AddRange(normal);
            newobj.index.AddRange(index);
            newobj.color = color;
            newobj.name = newName;
            return newobj;
        }
    }
}
