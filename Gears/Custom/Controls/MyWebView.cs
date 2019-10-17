using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Gears.Custom.Controls
{
    public class MyWebView : View
    {
        public static readonly BindableProperty UriProperty = BindableProperty.Create(
          propertyName: "Uri",
          returnType: typeof(string),
          declaringType: typeof(MyWebView),
          defaultValue: default(string));

        public string Uri
        {
            get { return (string)GetValue(UriProperty); }
            set { SetValue(UriProperty, value); }
        }

        public delegate Task<string> EvaluateJavaScriptDelegate(string script);
        public delegate void WebViewScriptNotifyDelegate(string script);

        public event EvaluateJavaScriptDelegate EvaluateJavaScriptRequested;
        public WebViewScriptNotifyDelegate OnWebViewScriptNotify;

        /// <param name="script">評価するスクリプト。</param>
        /// <summary>//From Xamarin.Forms.WebView  : JavaScript 評価をサポートするプラットフォームで、<paramref name="script" /> を評価します。</summary>
        /// <returns>評価の結果を文字列として含むタスク。</returns>
        public async Task<string> EvaluateJavaScriptAsync(string script)
        {
            if (script == null)
                return (string)null;
            if (Device.RuntimePlatform != "Android")
            {
                script = EscapeJsString(script);
                script = "try{JSON.stringify(eval('" + script + "'))}catch(e){console.log(e)};";
            }

            if (EvaluateJavaScriptRequested != null)
            {
                string str = await Device.InvokeOnMainThreadAsync<string>(()=>EvaluateJavaScriptRequested?.Invoke(script));
                if (str != null)
                    str = str.Trim('"');
                return str;
            }
            else
                return null;
        }

        //From Xamarin.Forms.WebView 
        private static string EscapeJsString(string js)
        {
            if (js == null)
                return (string)null;
            if (!js.Contains("'"))
                return js;
            MatchCollection matchCollection = Regex.Matches(js, "(\\\\*?)'");
            List<string> stringList = new List<string>();
            for (int index = 0; index < matchCollection.Count; ++index)
            {
                string str = matchCollection[index].Value;
                if (!stringList.Contains(str))
                    stringList.Add(str);
            }
            stringList.Sort((Comparison<string>)((x, y) => y.Length.CompareTo(x.Length)));
            for (int index = 0; index < stringList.Count; ++index)
            {
                string str = stringList[index];
                string replacement = "'".PadLeft((str.Length - 1) * 2 + 1 + 1, '\\');
                js = Regex.Replace(js, "(?<=[^\\\\])" + Regex.Escape(str), replacement);
            }
            return js;
        }
    }
}
