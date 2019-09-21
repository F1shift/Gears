using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Gears.Resources;
using System.IO;
using Gears.Custom.Controls;
using Gears.Services;

namespace Gears.Views
{
    [Xamarin.Forms.Xaml.XamlCompilation(Xamarin.Forms.Xaml.XamlCompilationOptions.Compile)]
    public partial class ThreeDModelingPage : Xamarin.Forms.ContentPage
    {
        public ThreeDModelingPage()
        {
            InitializeComponent();
        }

        

        protected override void OnAppearing()
        {
            string url;
            switch (Device.RuntimePlatform)
            {
                case Device.Android:
                    url = "file:///android_asset/www/index.html";
                    break;
                case Device.UWP:
                    url = "ms-appx-web:///Assets/www/index.html";
                    break;
                default:
                    url = "";
                    break;
            }
            myWebView.Uri = url;
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            await myWebView.EvaluateJavaScriptAsync("SceneController.PlotRackTrace([-200, 150, 0, 200, 150, 0]);");
        }
    }
}