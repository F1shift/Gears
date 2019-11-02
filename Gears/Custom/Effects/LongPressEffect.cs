using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using System.Windows.Input;

namespace Gears.Custom.Effects
{
    
    public class LongPressEffect : RoutingEffect
    {
        public LongPressEffect(): base("Gears.LongPressEffect")
        {
        }

        /// <summary>
        /// プラットフォームごとのLongPressEffectでLongPressしたときに実行するコマンドを共通プロジェクトで指定できるようにするため、
        /// Command とCommandParameterのBindable Propertyを追加する。
        /// </summary>
        public static readonly BindableProperty CommandProperty = BindableProperty.CreateAttached("Command", typeof(ICommand), typeof(LongPressEffect), null);
        public static ICommand GetCommand(BindableObject view)
        {
            return (ICommand)view.GetValue(CommandProperty);
        }
        public static void SetCommand(BindableObject view, ICommand value)
        {
            view.SetValue(CommandProperty, value);
        }

        public static readonly BindableProperty CommandParameterProperty = BindableProperty.CreateAttached("CommandParameter", typeof(object), typeof(LongPressEffect), null);
        public static object GetCommandParameter(BindableObject view)
        {
            return view.GetValue(CommandParameterProperty);
        }
        public static void SetCommandParameter(BindableObject view, object value)
        {
            view.SetValue(CommandParameterProperty, value);
        }
    }
}
