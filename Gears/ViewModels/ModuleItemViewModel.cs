using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Gears.ViewModels
{
    class ModuleItemViewModel : Gears.Models.ModuleItem
    {
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
