using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Gears.ViewModels;

namespace Gears.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GearParameterPage : ContentPage
    {
        public GearParameterPage()
        {
            InitializeComponent();
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

        private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            ((ListView)sender).SelectedItem = null;
        }
    }
}