using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Webkit;
using Xamarin.Forms.Platform.Android;
using Gears.Custom.Controls;
using Xamarin.Forms;
using Gears.Droid.Custom.Renderer;

[assembly:Application(HardwareAccelerated = true)]
[assembly:ExportRenderer(typeof(MyWebView), typeof(MyWebViewRenderer))]
namespace Gears.Droid.Custom.Renderer
{
    
    class MyWebViewRenderer : ViewRenderer<MyWebView, Android.Webkit.WebView>
    {
        Context _context;

        public MyWebViewRenderer(Context context) : base(context)
        {
            _context = context;
        }

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
                    var webView = new Android.Webkit.WebView(_context);
                    webView.Settings.JavaScriptEnabled = true;
                    webView.Settings.SetRenderPriority(WebSettings.RenderPriority.High);
                    webView.SetLayerType(LayerType.Hardware, null);
                    SetNativeControl(webView);
                }
                e.NewElement.PropertyChanged += UpdateURI;
            }
        }

        protected void UpdateURI(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            try
            {
                this.Control.LoadUrl(this.Element.Uri);
            }
            catch(Exception msg)
            {
                Console.WriteLine(msg.Message);
            }
        }
    }
}