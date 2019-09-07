using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Gears.Custom.Effects
{
    class PopupController
    {
        AbsoluteLayout _Area { get; set; }
        ContentView _PopupContainer { get; set; }
        public uint AnimationInterval { get; set; } = 150u;
        public Color BackgroundColor { get; set; } = Color.FromHex("#C0808080");
        public Thickness Padding { get; set; } = new Thickness(10, 0);

        public async void ClosePopup()
        {
            if (_PopupContainer != null && _Area.Children.Contains(_PopupContainer))
            {
                await _PopupContainer.FadeTo(0, AnimationInterval);
                _Area.Children.Remove(_PopupContainer);
            }
        }

        public void ShowPopup(AbsoluteLayout area, View content)
        {
            _Area = area;
            _PopupContainer = new ContentView()
            {
                BackgroundColor = BackgroundColor,
                Padding = Padding,
                IsVisible = true,
            };
            AbsoluteLayout.SetLayoutBounds(_PopupContainer, new Rectangle(0, 0, 1, 1));
            AbsoluteLayout.SetLayoutFlags(_PopupContainer, AbsoluteLayoutFlags.All);
            _PopupContainer.Content = content;
            _PopupContainer.Opacity = 0;
            _PopupContainer.Scale = 0.8;
            _Area.Children.Add(_PopupContainer);
            _PopupContainer.FadeTo(1, AnimationInterval);
            _PopupContainer.ScaleTo(1, AnimationInterval);
        }
    }
}
