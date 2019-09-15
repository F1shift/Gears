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
    public class MyWebViewRenderer : WebViewRenderer
    {
        public MyWebViewRenderer():base()
        {
            
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.WebView> e)
        {
            base.OnElementChanged(e);
            if ((e.OldElement != null) || (this.Element == null))
                return;
            SetNativeControl(new Windows.UI.Xaml.Controls.WebView(WebViewExecutionMode.SeparateProcess));
        }

    }
}
