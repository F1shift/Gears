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
    public partial class DesignPage2 : ContentPage
    {
        int? currentTabIndex;

        public DesignPage2()
        {
            InitializeComponent();
            this.BindingContext = Application.Current.Resources["DesignViewModel2"];
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
            if (currentTabIndex == null)
            {
                currentTabIndex = 0;
                GearParaPanel.TranslationX = Width;
                DetailPanel.TranslationX = Width;
                GearParaPanel.Opacity = 0;
                DetailPanel.Opacity = 0;
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

            if (currentTabIndex < index)
            {
                OldPanel.TranslateTo(-this.Width, OldPanel.TranslationY);
                OldPanel.FadeTo(0);
            }
            else
            {
                OldPanel.TranslateTo(this.Width, OldPanel.TranslationY);
                OldPanel.FadeTo(0);
            }
            AbsLayout.Children.Remove(newPanel);
            AbsLayout.Children.Add(newPanel);
            newPanel.TranslateTo(0, newPanel.TranslationY);
            newPanel.FadeTo(1);
            currentTabIndex = index;
        }

        private void ContentPage_BindingContextChanged(object sender, EventArgs e)
        {
            var vm = (Gears.ViewModels.DesignViewModel2)BindingContext;
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