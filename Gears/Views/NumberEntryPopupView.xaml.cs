using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Gears.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NumberEntryPopupView : ContentView
    {
        public string Title { get; set; } = "Title";
        public NumberEntryPopupView()
        {
            InitializeComponent();
        }
    }
}