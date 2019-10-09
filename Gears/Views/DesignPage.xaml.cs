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
    public partial class DesignPage : ContentPage
    {
        int? currentTabIndex;

        public DesignPage()
        {
            InitializeComponent();
            this.BindingContext = Application.Current.Resources["DesignViewModel"];
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
            Navigate(0);
        }

        private void Navigate(int index)
        {
            if (currentTabIndex == index)
            {
                return;
            }
            if (currentTabIndex == null)
            {
                currentTabIndex = 0;
                GearParaPanel.TranslationX = Width;
                DetailPanel.TranslationX = Width;
                GearParaPanel.Opacity = 0;
                DetailPanel.Opacity = 0;
                GearParaPanel.IsVisible = false;
                DetailPanel.IsVisible = false;
            }

            View OldPanel, newPanel;
            switch (index)
            {
                case 0:
                    tabActiveBar.TranslateTo(0, 0);
                    newPanel = BasciRackPanel;
                    break;
                case 1:
                    tabActiveBar.TranslateTo(tabActiveBar.Width * 1, 0);
                    newPanel = GearParaPanel;
                    break;
                case 2:
                    tabActiveBar.TranslateTo(tabActiveBar.Width * 2, 0);
                    newPanel = DetailPanel;
                    break;
                default:
                    throw new Exception("Tab index out of range!");
            }
            switch (currentTabIndex)
            {
                case 0:
                    OldPanel = BasciRackPanel;
                    break;
                case 1:
                    OldPanel = GearParaPanel;
                    break;
                case 2:
                    OldPanel = DetailPanel;
                    break;
                default:
                    throw new Exception("Tab index out of range!");
            }

            uint animateLength = 250;
            if (OldPanel != newPanel)
            {
                if (currentTabIndex < index)
                {
                    OldPanel.TranslateTo(-this.Width, OldPanel.TranslationY, length: animateLength);
                }
                else
                {
                    OldPanel.TranslateTo(this.Width, OldPanel.TranslationY, length: animateLength);
                }
                OldPanel.FadeTo(0, length: animateLength);
                var timer = new System.Timers.Timer();
                timer.Interval = animateLength;
                timer.Elapsed += (s, e) =>
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        OldPanel.IsVisible = false;
                    });
                    timer.Stop();
                };
                timer.Start();
            }

            //AbsLayout.Children.Remove(newPanel);
            //AbsLayout.Children.Add(newPanel);
            newPanel.IsVisible = true;
            newPanel.TranslateTo(0, newPanel.TranslationY, length: animateLength);
            newPanel.FadeTo(1, length: animateLength);

            currentTabIndex = index;
        }

        private void ContentPage_BindingContextChanged(object sender, EventArgs e)
        {
            var vm = (Gears.ViewModels.DesignViewModel)BindingContext;
            vm.ThreeDModelingViewModel.EvalAsync = myWebView.EvaluateJavaScriptAsync;
            myWebView.OnWebViewScriptNotify = vm.InvokeCSHandler;
        }

        private void NavigateButton_Clicked(object sender, EventArgs e)
        {
            var button = (Button)sender;
            switch (button.Text)
            {
                case "Rack":
                    Navigate(0);
                    break;
                case "Gear":
                    Navigate(1);
                    break;
                case "Detail":
                    Navigate(2);
                    break;
                default:
                    break;
            }
        }
    }
}