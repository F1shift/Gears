using System;
using System.Windows.Input;

using Xamarin.Forms;

namespace Gears.ViewModels
{
    public class AboutViewModel : BaseViewModel
    {
        public AboutViewModel()
        {
            Title = "About";

            OpenWebCommand = new Command(() => Device.OpenUri(new Uri("https://youtu.be/mgseR7lVBEg")));
        }

        public ICommand OpenWebCommand { get; }
    }
}