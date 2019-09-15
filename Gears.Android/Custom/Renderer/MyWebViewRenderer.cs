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
    
    class MyWebViewRenderer : WebViewRenderer
    {
        public MyWebViewRenderer(Context context = null):base(context)
        {
            
        }

        //protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.WebView> e)
        //{
        //    base.OnElementChanged(e);
        //    var webView = (Android.Webkit.WebView)this.Control;
        //    //webView.Settings.JavaScriptEnabled = true;
        //    //webView.Settings.DefaultTextEncodingName = "UTF-8";
        //    //webView.Settings.SetAppCacheEnabled(true);
        //    //webView.Settings.SetAppCacheMaxSize(16 * 1024 * 1024);
        //    webView.SetLayerType(View.w, null);
        //}

        //protected override void OnElementChanged(ElementChangedEventArgs<WebView> e)
        //{
        //    base.OnElementChanged(e);
        //    if ((e.OldElement != null) || (this.Element == null))
        //        return;
        //    var webview = new Android.Webkit.WebView(Context);
        //    var a = webview.IsHardwareAccelerated;
        //SetNativeControl(webview);
        //}
    }
}