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
            var vm = (Gears.ViewModels.ThreeDModelingViewModel)Application.Current.Resources[nameof(Gears.ViewModels.ThreeDModelingViewModel)];
            vm.EvalAsync = myWebView.EvaluateJavaScriptAsync;
            this.BindingContext = vm;
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
    }
}