using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

using Xamarin.Forms;
using Xamarin.Forms.Platform.UWP;
using Gears.Custom.Controls;
using Gears.UWP.Custom.Renderer;

[assembly: ExportRenderer(typeof(MyWebView), typeof(MyWebViewRenderer))]
namespace Gears.UWP.Custom.Renderer
{
    public class MyWebViewRenderer : ViewRenderer<MyWebView, Windows.UI.Xaml.Controls.WebView>
    {

        protected override void OnElementChanged(ElementChangedEventArgs<MyWebView> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                e.OldElement.PropertyChanged -= UpdateURI;
            }
            if (e.NewElement != null)
            {
                if (Control == null)
                {
                    SetNativeControl(new Windows.UI.Xaml.Controls.WebView(WebViewExecutionMode.SeparateProcess));
                }
                e.NewElement.PropertyChanged += UpdateURI;
            }
        }

        protected void UpdateURI(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (this.Element.Uri == null)
            {
            }
            else
            {
                this.Control.Source = new Uri(this.Element.Uri);
            }
            
        }
    }
}
