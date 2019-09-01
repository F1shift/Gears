using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Platform.UWP;
using Gears.Custom.Controls;
using Gears.UWP.Custom.Renderer;

[assembly: ExportRenderer(typeof(MySlider), typeof(MySliderRenderer))]
namespace Gears.UWP.Custom.Renderer
{
    public class MySliderRenderer : SliderRenderer
    {
    }
}
