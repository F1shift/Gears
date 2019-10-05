using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.UI.Xaml;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using Xamarin.Forms.Internals;

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
                e.OldElement.EvaluateJavaScriptRequested -= OnEvaluateJavaScriptRequested;
                e.OldElement.PropertyChanged -= UpdateURI;
            }
            if (e.NewElement != null)
            {
                if (Control == null)
                {
                    SetNativeControl(new Windows.UI.Xaml.Controls.WebView(WebViewExecutionMode.SeparateProcess));
                }
                e.NewElement.EvaluateJavaScriptRequested += OnEvaluateJavaScriptRequested;
                e.NewElement.PropertyChanged += UpdateURI;
                Control.ScriptNotify += OnWebViewScriptNotify;
                if (Element.Uri != null)
                    Control.Source = new Uri(Element.Uri);
            }
        }

        protected void UpdateURI(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(this.Element.Uri))
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

        private async Task<string> OnEvaluateJavaScriptRequested(string script)
        {
            return await this.Control.InvokeScriptAsync("eval", new string[] { script });
        }

        void OnWebViewScriptNotify(object sender, NotifyEventArgs e)
        {
            Element.OnWebViewScriptNotify(e.Value);
        }
    }
}
