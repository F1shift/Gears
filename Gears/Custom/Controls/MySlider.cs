using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Gears.Custom.Controls
{
    public class MySlider : Slider
    {
        public static readonly BindableProperty StepProperty =
            BindableProperty.Create(nameof(Step), typeof(double), typeof(MySlider));
        public static readonly BindableProperty IsSteppedProperty =
            BindableProperty.Create(nameof(IsStepped), typeof(bool), typeof(MySlider), defaultValue: true, propertyChanged: IsSteppedPropertyChanged);

        public double Step {
            get
            {
                return (double)GetValue(StepProperty);
            }
            set {
                SetValue(StepProperty, value);
            }
        }

        public bool IsStepped
        {
            get
            {
                return (bool)GetValue(IsSteppedProperty);
            }
            set
            {
                SetValue(IsSteppedProperty, value);
            }
        }

        public MySlider()
        {
            ValueChanged += MySlider_ValueChanged;
        }

        private void MySlider_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            double step = Step;
            if (step != 0)
            {
                var slider = (Slider)sender;
                double orgValue = slider.Value;
                double stepedValue = System.Math.Round(System.Math.Round(orgValue / (step)) * step, 10);
                if (slider.Value != stepedValue)
                {
                    slider.Value = stepedValue;
                }
            }
        }

        protected static void IsSteppedPropertyChanged(BindableObject obj, object oldValue, object newValue)
        {
            var mySlider = (MySlider)obj;
            if ((bool)newValue)
            {
                mySlider.ValueChanged += mySlider.MySlider_ValueChanged;
            }
            else
            {
                mySlider.ValueChanged -= mySlider.MySlider_ValueChanged;
            }
        }
    }
}
