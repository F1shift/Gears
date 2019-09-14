using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Urho;
using Urho.Forms;
using Urho.Gui;
namespace Gears.Views
{
    [Xamarin.Forms.Xaml.XamlCompilation(Xamarin.Forms.Xaml.XamlCompilationOptions.Compile)]
    public partial class ThreeDModelingPage : Xamarin.Forms.ContentPage
    {
        public UrhoSurface UrhoSurface { get; set; }
        public ThreeDModelingPage()
        {
            InitializeComponent();
            myWebView.Source = "http://xamarin.com";
        }
    }
}