using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms.Platform.UWP;
using Xamarin.Forms;
using Gears.UWP.Custom.Effects;

[assembly: ResolutionGroupName("Gears")]
[assembly: ExportEffect(typeof(LongPressEffect), nameof(LongPressEffect))]
namespace Gears.UWP.Custom.Effects
{
    class LongPressEffect : PlatformEffect
    {
        bool _attached;
        protected override void OnAttached()
        {
            if (!_attached)
            {
                if (Control != null)
                {
                    Control.RightTapped += Control_RightTapped;
                }
                else
                {
                    Container.RightTapped += Control_RightTapped;
                }
                _attached = true;
            }
        }

        protected override void OnDetached()
        {
            if (_attached)
            {
                if (Control != null)
                {
                    Control.RightTapped -= Control_RightTapped;
                }
                else
                {
                    Container.RightTapped -= Control_RightTapped;
                }
                _attached = false;
            }
        }

        private void Control_RightTapped(object sender, Windows.UI.Xaml.Input.RightTappedRoutedEventArgs e)
        {
            var command = Gears.Custom.Effects.LongPressEffect.GetCommand(Element);
            var commandParameter = Gears.Custom.Effects.LongPressEffect.GetCommandParameter(Element);
            command.Execute(commandParameter);
        }
    }
}
