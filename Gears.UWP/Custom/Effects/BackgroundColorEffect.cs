using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms.Platform.UWP;
using Xamarin.Forms;
using Gears.UWP.Custom.Effects;

[assembly: ExportEffect(typeof(BackgroundColorEffect), nameof(BackgroundColorEffect))]
namespace Gears.UWP.Custom.Effects
{
    class BackgroundColorEffect : PlatformEffect
    {
        protected override void OnAttached()
        {
            if (this.Control is Windows.UI.Xaml.Controls.Button)
            {
                var button = (Windows.UI.Xaml.Controls.Button)Control;
                button.Background = Gears.Custom.Effects.BackgroundColorEffect.GetBackgroundColor(Element).ToBrush();
            }
            
        }

        protected override void OnDetached()
        {
           
        }
    }
}
