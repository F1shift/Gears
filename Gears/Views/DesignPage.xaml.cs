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
        bool IsDetailPanelPined = false;
        uint animateLength = 250;

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
                    tabActiveBar.TranslateTo(0, 0, length: animateLength);
                    newPanel = BasciRackPanel;
                    break;
                case 1:
                    tabActiveBar.TranslateTo(tabActiveBar.Width * 1, 0, length: animateLength);
                    newPanel = GearParaPanel;
                    break;
                case 2:
                    tabActiveBar.TranslateTo(tabActiveBar.Width * 2, 0, length: animateLength);
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

            
            if (OldPanel != newPanel)
            {
                if (OldPanel != DetailPanel || IsDetailPanelPined  == false)
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
                else if(OldPanel == DetailPanel || IsDetailPanelPined == true)
                {
                    switch (index)
                    {
                        case 0:
                            DetailPanel.BindingContext = BasciRackPanelEmpityArea;
                            break;
                        case 1:
                            DetailPanel.BindingContext = GearParaPanelEmpityArea;
                            break;
                        default:
                            DetailPanel.BindingContext = null;
                            break;
                    }
                }
            }
            if (newPanel == DetailPanel)
            {
                newPanel.BindingContext = null;
            }
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
            if (sender == RackTabButton)
            {
                Navigate(0);
            }
            else if (sender == GearTabButton)
            {
                Navigate(1);
            }
            else if (sender == DetailTabButton)
            {
                Navigate(2);
            }
            else
            {

            }
        }

        private void ImageButton_Clicked(object sender, EventArgs e)
        {
            IsDetailPanelPined = !IsDetailPanelPined;
            if (IsDetailPanelPined)
            {
                PinSwichButton.Source = ImageSource.FromResource("Gears.Resources.pin-effective.png");
                switch (currentTabIndex)
                {
                    case 0:
                        DetailPanel.BindingContext = BasciRackPanelEmpityArea;
                        break;
                    case 1:
                        DetailPanel.BindingContext = GearParaPanelEmpityArea;
                        break;
                    default:
                        DetailPanel.BindingContext = null;
                        break;
                }
                DetailPanel.IsVisible = true;
                DetailPanel.VerticalOptions = LayoutOptions.Start;
                DetailPanel.FadeTo(1, length: animateLength);
                DetailPanel.TranslateTo(0, 0, length: animateLength);
            }
            else
            {
                PinSwichButton.Source = ImageSource.FromResource("Gears.Resources.pin-ineffective.png");
                if (currentTabIndex != 2)
                {
                    DetailPanel.TranslateTo(DetailPanel.Width, 0, length: animateLength);
                    var timer = new System.Timers.Timer();
                    timer.Interval = animateLength;
                    timer.Elapsed += (s, ee) =>
                    {
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            DetailPanel.VerticalOptions = LayoutOptions.Fill;
                        });
                        timer.Stop();
                    };
                    timer.Start();
                }
            }
        }
    }
}