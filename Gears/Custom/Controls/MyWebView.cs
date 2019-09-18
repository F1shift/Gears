using System;
using System.Collections.Generic;
using System.Text;

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
    }
}
