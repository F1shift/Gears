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
        public GearParameterPage()
        {
            InitializeComponent();
        }

        public ContentView PopupContainer { get; set; }
        public void ClosePopup()
        {
            if (PopupContainer != null && AbsParent.Children.Contains(PopupContainer))
            {
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
            this.AbsParent.Children.Add(PopupContainer);
        }

        private void Slider_DragCompleted(object sender, EventArgs e)
        {
            var slider = (Slider)sender;
            var itemVM = (InputItemViewModel)slider.BindingContext;
            double step = itemVM.Step;
            double orgValue = slider.Value; 
            double stepedValue = Math.Round(Math.Round(orgValue / (step)) * step, 10);
            if (slider.Value != stepedValue)
            {
                slider.Value = stepedValue;
            }
        }

        private void Slider_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            var slider = (Slider)sender;
            var itemVM = (InputItemViewModel)slider.BindingContext;
            double step = itemVM.Step;
            double orgValue = slider.Value;
            double stepedValue = Math.Round(Math.Round(orgValue / (step)) * step, 10);
            if (slider.Value != stepedValue)
            {
                slider.Value = stepedValue;
            }
        }

        private void ViewCell_Tapped(object sender, EventArgs e)
        {
            DisplayAlert("Title", "msg", "cancel");
        }

        bool _InputExpanded = true;
        uint _interval = 150;
        private void CollapseButton_Clicked(object sender, EventArgs e)
        {
            
            if (_InputExpanded)
            {
                InputArea.TranslateTo(0, InputArea.Height - CollapseButton.Height, _interval, Easing.CubicInOut);
                ArrowImage.RotateTo(0, _interval, Easing.CubicInOut);
                _InputExpanded = false;
            }
            else
            {
                InputArea.TranslateTo(0, 0, _interval, Easing.CubicInOut);
                ArrowImage.RotateTo(180, _interval, Easing.CubicInOut);
                _InputExpanded = true;
            }
        }
    }
}