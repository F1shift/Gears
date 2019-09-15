using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Urho.Forms;
using Gears.Resources;
using System.IO;
using Gears.Custom.Controls;

namespace Gears.Views
{
    [Xamarin.Forms.Xaml.XamlCompilation(Xamarin.Forms.Xaml.XamlCompilationOptions.Compile)]
    public partial class ThreeDModelingPage : Xamarin.Forms.ContentPage
    {
        //public UrhoSurface UrhoSurface { get; set; }
        public WebView myWebView { get; set; }
        public ThreeDModelingPage()
        {
            InitializeComponent();

            myWebView = new MyWebView();
            AbsoluteLayout.SetLayoutFlags(myWebView, AbsoluteLayoutFlags.All);
            AbsoluteLayout.SetLayoutBounds(myWebView, new Rectangle(0, 0, 1, 1));
            this.RootLayout.Children.Add(myWebView);

            //UrhoSurface = new UrhoSurface();
            //UrhoSurface.BackgroundColor = Color.Transparent;
            //AbsoluteLayout.SetLayoutFlags(UrhoSurface, AbsoluteLayoutFlags.All);
            //AbsoluteLayout.SetLayoutBounds(UrhoSurface, new Rectangle(0, 0, 1, 1));
            //this.RootLayout.Children.Add(UrhoSurface);
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
            myWebView.Source = url;
        }
    }
}