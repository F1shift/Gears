using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace Gears.ViewModels.Converters
{
    internal class StringConverter : IValueConverter
    {
        internal enum SupportedTypes
        {
            Int,
            Double,
            Degree,
            Bool
        }

        internal SupportedTypes Type { get; set; }
        Func<object, Type, object, CultureInfo, object> _Convert;
        Func<object, Type, object, CultureInfo, object> _ConvertBack;
        public StringConverter(SupportedTypes type)
        {
            this.Type = type;
            _ConvertBack = (value, targetType, parameter, culture) => {
                    if (parameter != null)
                    {
                        return String.Format((string)parameter, value);
                    }
                    else
                    {
                        return value.ToString();
                    }
                };
            switch (Type)
            {
                case SupportedTypes.Int:
                    _Convert = (value, targetType, parameter, culture) => System.Convert.ToInt32(value);
                    break;
                case SupportedTypes.Double:
                    _Convert = (value, targetType, parameter, culture) => System.Convert.ToDouble(value);
                    break;
                case SupportedTypes.Degree:
                    throw new NotImplementedException();
                case SupportedTypes.Bool:
                    throw new NotImplementedException();
                default:
                    throw new NotSupportedException("Type " + Type + " はサポートされていません。");
            }
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return _Convert(value, targetType, parameter, culture);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return _ConvertBack(value, targetType, parameter, culture);
        }
    }
}
