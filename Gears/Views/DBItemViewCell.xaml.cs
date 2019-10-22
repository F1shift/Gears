using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Gears.ViewModels;
using Gears.Custom.Effects;

namespace Gears.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DBItemViewCell : ViewCell
    {
        public Page Page { get; set; }
        public DBItemViewCell()
        {
            InitializeComponent();
        }
        private void DetailButton_Clicked(object sender, EventArgs e)
        {
            var vm = BindingContext as DBItemViewModel;
            var page = new ContentPage() { Content = new GearDetailView() { BindingContext = vm.GearDetailViewModel } };
            var backbutton = new ImageButton()
            {
                Source = ImageSource.FromResource("Gears.Resources.arrow-left.png"),
                Padding = new Thickness(10),
            };
            switch (Device.RuntimePlatform)
            {
                case Device.Android:
                    backbutton.HeightRequest = 50;
                    backbutton.WidthRequest = 50;
                    break;
                case Device.UWP:
                    backbutton.HeightRequest = 40;
                    backbutton.WidthRequest = 40;
                    break;
                default:
                    throw new NotImplementedException($"Code for {Device.RuntimePlatform} not implement");
            }
            backbutton.SetValue(BackgroundColorEffect.BackgroundColorProperty, Color.Transparent);
            backbutton.Effects.Add(new BackgroundColorEffect());
            backbutton.Clicked += (s, ee) => { Page.Navigation.PopModalAsync(); };
            var stacklayout = new StackLayout();
            stacklayout.Orientation = StackOrientation.Horizontal;
            stacklayout.Children.Add(backbutton);
            page.SetValue(NavigationPage.TitleViewProperty,
                new ContentView()
                {
                    Content = stacklayout
                });
            Page.Navigation.PushModalAsync(new NavigationPage(page));
        }
    }
}