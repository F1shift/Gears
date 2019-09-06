using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Gears.ViewModels;

namespace Gears.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GearParameterPage : ContentPage, IPopup
    {
        public uint Interval { get; set; } = 150u;
        public GearParameterPage()
        {
            InitializeComponent();
        }

        public ContentView PopupContainer { get; set; }
        public async void ClosePopup()
        {
            if (PopupContainer != null && AbsParent.Children.Contains(PopupContainer))
            {
                await PopupContainer.FadeTo(0, Interval);
                AbsParent.Children.Remove(PopupContainer);
            }
        }

        public Page GetPage()
        {
            return this;
        }

        public void ShowPopup(View content)
        {
            PopupContainer = new ContentView()
            {
                BackgroundColor = Color.FromHex("#C0808080"),
                Padding = new Thickness(10, 0),
                IsVisible = true,
            };
            AbsoluteLayout.SetLayoutBounds(PopupContainer, new Rectangle(0, 0, 1, 1));
            AbsoluteLayout.SetLayoutFlags(PopupContainer, AbsoluteLayoutFlags.All);
            PopupContainer.Content = content;
            PopupContainer.Opacity = 0;
            PopupContainer.Scale = 0.8;
            this.AbsParent.Children.Add(PopupContainer);
            PopupContainer.FadeTo(1, Interval);
            PopupContainer.ScaleTo(1, Interval);
        }
    }
}