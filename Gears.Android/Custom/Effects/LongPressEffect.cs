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

[assembly: ResolutionGroupName("Gears")]
[assembly: ExportEffect(typeof(LongPressEffect), nameof(LongPressEffect))]
namespace Gears.Droid.Custom.Effects
{
    class LongPressEffect : PlatformEffect
    {
        protected override void OnAttached()
        {
                if (Control != null)
                {
                    Control.LongClickable = true;
                    Control.LongClick += Control_LongClick;
                }
                else
                {
                    Container.LongClickable = true;
                    Container.LongClick += Control_LongClick;
                }
        }

        protected override void OnDetached()
        {
            try
            {
                if (Control != null )
                {
                    var IsDisposedProperty = Control.GetType().GetProperty("IsDisposed");
                    if (IsDisposedProperty == null || !(bool)IsDisposedProperty.GetValue(Control))
                    {
                        Control.LongClickable = true;
                        Control.LongClick -= Control_LongClick;
                    }
                }
                else
                {
                    var IsDisposedProperty = Container.GetType().GetProperty("IsDisposed");
                    if (IsDisposedProperty == null || !(bool)IsDisposedProperty.GetValue(Container))
                    {
                        Container.LongClickable = true;
                        Container.LongClick -= Control_LongClick;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private void Control_LongClick(object sender, Android.Views.View.LongClickEventArgs e)
        {
            var command = Gears.Custom.Effects.LongPressEffect.GetCommand(Element);
            var commandParameter = Gears.Custom.Effects.LongPressEffect.GetCommandParameter(Element);
            command.Execute(commandParameter);
        }
    }
}