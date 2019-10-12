using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using Xamarin.Forms.Platform.Android;
using Xamarin.Forms;
using Gears.Droid.Custom.Effects;
using Android.Support.V7.Widget;
[assembly: ExportEffect(typeof(BackgroundColorEffect), nameof(BackgroundColorEffect))]
namespace Gears.Droid.Custom.Effects
{
    class BackgroundColorEffect : PlatformEffect
    {
        private Android.Views.View View => Control ?? Container;
        protected override void OnAttached()
        {
            if (View is AppCompatImageButton imageButton)
            {
                imageButton.SetBackgroundColor(Gears.Custom.Effects.BackgroundColorEffect.GetBackgroundColor(Element).ToAndroid());
            }
        }

        protected override void OnDetached()
        {
        }
    }
}