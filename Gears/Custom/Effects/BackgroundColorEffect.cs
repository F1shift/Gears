using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using System.Windows.Input;

namespace Gears.Custom.Effects
{
    
    public class BackgroundColorEffect : RoutingEffect
    {
        public BackgroundColorEffect(): base("Gears.BackgroundColorEffect")
        {
        }
        /// <summary>
        /// プラットフォームごとのLongPressEffectでLongPressしたときに実行するコマンドを共通プロジェクトで指定できるようにするため、
        /// Command とCommandParameterのBindable Propertyを追加する。
        /// </summary>
        public static readonly BindableProperty BackgroundColorProperty = BindableProperty.CreateAttached("BackgroundColor", typeof(Color), typeof(BackgroundColorEffect), null);
        public static Color GetBackgroundColor(BindableObject view) {
            return (Color)view.GetValue(BackgroundColorProperty);
        }
        public static void SetBackgroundColor(BindableObject view, Color value)
        {
            view.SetValue(BackgroundColorProperty, value);
        }
    }
}
