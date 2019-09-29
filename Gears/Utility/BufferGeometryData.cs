using System;
using System.Collections.Generic;
using System.Text;

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

        public object color { get; set; }
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

        public void SetType(Types type)
        {
            this.type = Enum.GetName(typeof(Types), type);
        }

        public void Merge(params BufferGeometryData[] buffers)
        {
            if (true)
            {

            }
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
        }
    }
}
