using System;
using System.Collections.Generic;
using System.Text;
using static Gears.Utility.Math;

namespace Gears.Utility
{
    class GearGeometryData: BufferGeometryData
    {
        public int z { get; set; }
        
        public GearGeometryData() : base(Types.mesh)
        {

        }
    }
}
