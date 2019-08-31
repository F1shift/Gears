using System;
using System.Collections.Generic;
using System.Text;

namespace Gears.Models
{
    class ModuleItem
    {
        public double Value { get; set; }
        public int Serial { get; set; }
        public string Annotation { get; set; }
        public string DisplayName {
            get {
                var str = string.Format("{0}  mm  [{1}系列]", Value, Serial);
                if (!String.IsNullOrWhiteSpace(Annotation))
                {
                    str += String.Format("({0})", Annotation);
                }
                return str;
            }
        }
    }
}
