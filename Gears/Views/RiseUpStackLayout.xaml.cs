using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Gears.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RiseUpStackLayout : StackLayout
    {
        public static readonly BindableProperty InnerStackLayoutBindableProperty =
            BindableProperty.Create(nameof(InnerStackLayout), typeof(StackLayout), typeof(RiseUpStackLayout), );
        public static readonly BindableProperty IsExpandedProperty =
            BindableProperty.Create(nameof(IsExpanded), typeof(bool), typeof(RiseUpStackLayout));

        public StackLayout InnerStackLayout
        {
            get
            {
                return (StackLayout)GetValue(InnerStackLayoutBindableProperty);
            }
            set
            {
                if (AbsoluteLayout.Children.Contains(InnerStackLayout))
                {
                    AbsoluteLayout.Children.Remove(InnerStackLayout);
                }
                SetValue(InnerStackLayoutBindableProperty, value);
                AbsoluteLayout.Children.Add(value);
            }
        }

        public bool IsExpanded
        {
            get { return (bool)GetValue(IsExpandedProperty); }
            set {
                SetValue(IsExpandedProperty, value);
            }
        }


        public RiseUpStackLayout()
        {
            InitializeComponent();
            InnerStackLayout = new StackLayout() {
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand};
        }

        uint _interval = 150;
        private void CollapseButton_Clicked(object sender, EventArgs e)
        {
            if (_InputExpanded)
            {
                this.TranslateTo(0, this.Height - CollapseButton.Height, _interval, Easing.CubicInOut);
                ArrowImage.RotateTo(0, _interval, Easing.CubicInOut);
                _InputExpanded = false;
            }
            else
            {
                this.TranslateTo(0, 0, _interval, Easing.CubicInOut);
                ArrowImage.RotateTo(180, _interval, Easing.CubicInOut);
                _InputExpanded = true;
            }
        }
    }
}