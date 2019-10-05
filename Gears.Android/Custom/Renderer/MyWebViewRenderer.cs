using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
using Java.Interop;

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
                e.OldElement.EvaluateJavaScriptRequested -= JavascriptRequestedHandler;
                e.OldElement.PropertyChanged -= UpdateURI;
            }
            if (e.NewElement != null)
            {
                if (Control == null)
                {
                    var webView = new Android.Webkit.WebView(_context);
                    webView.Settings.JavaScriptEnabled = true;
                    webView.Settings.SetRenderPriority(WebSettings.RenderPriority.High);
                    webView.Settings.AllowFileAccessFromFileURLs = true;
                    webView.SetLayerType(LayerType.Hardware, null);
                    SetNativeControl(webView);
                }
                e.NewElement.EvaluateJavaScriptRequested += JavascriptRequestedHandler;
                e.NewElement.PropertyChanged += UpdateURI;
                Control.AddJavascriptInterface(new JSBridge(this), "jsBridge");
                if (this.Element.Uri != null)
                this.Control.LoadUrl(this.Element.Uri);
            }
        }

        private async Task<string> JavascriptRequestedHandler(string script)
        {
            JavascriptResult javascriptResult = new JavascriptResult();
            this.Control.EvaluateJavascript(script, javascriptResult);
            return await javascriptResult.JsResult.ConfigureAwait(false);
        }

        protected void UpdateURI(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(this.Element.Uri))
            {
                try
                {
                    this.Control.LoadUrl(this.Element.Uri);
                }
                catch (Exception msg)
                {
                    Console.WriteLine(msg.Message);
                }
            }
        }

        private class JavascriptResult : Java.Lang.Object, IValueCallback, IJavaObject, IDisposable
        {
            private TaskCompletionSource<string> source;

            public Task<string> JsResult
            {
                get
                {
                    return this.source.Task;
                }
            }

            public JavascriptResult()
            {
                this.source = new TaskCompletionSource<string>();
            }

            public void OnReceiveValue(Java.Lang.Object result)
            {
                this.source.SetResult(((Java.Lang.String)result).ToString());
            }
        }
    }

    class JSBridge : Java.Lang.Object
    {
        readonly WeakReference<MyWebViewRenderer> hybridWebViewRenderer;

        public JSBridge(MyWebViewRenderer hybridRenderer)
        {
            hybridWebViewRenderer = new WeakReference<MyWebViewRenderer>(hybridRenderer);
        }

        [JavascriptInterface]
        [Export("Notify")]
        public void Notify(string data)
        {
            MyWebViewRenderer hybridRenderer;

            if (hybridWebViewRenderer != null && hybridWebViewRenderer.TryGetTarget(out hybridRenderer))
            {
                hybridRenderer.Element.OnWebViewScriptNotify(data);
            }
        }
    }
}