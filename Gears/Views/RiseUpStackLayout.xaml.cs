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
        public static readonly BindableProperty ContentProperty =
            BindableProperty.Create(nameof(Content), typeof(View), typeof(RiseUpStackLayout), defaultValue: null, propertyChanged: ContentPropertyChanged);
        public static readonly BindableProperty IsExpandedProperty =
            BindableProperty.Create(nameof(IsExpanded), typeof(bool), typeof(RiseUpStackLayout), defaultValue: true, propertyChanged: IsExpandedPropertyChanged);
        public static readonly BindableProperty IntervalProperty =
            BindableProperty.Create(nameof(Interval), typeof(uint), typeof(RiseUpStackLayout), defaultValue: 150u);

        public View Content
        {
            get
            {
                return (View)GetValue(ContentProperty);
            }
            set
            {
                SetValue(ContentProperty, value);
            }
        }

        public bool IsExpanded
        {
            get { return (bool)GetValue(IsExpandedProperty); }
            set {
                SetValue(IsExpandedProperty, value);
            }
        }
        public uint Interval
        {
            get
            {
                return (uint)GetValue(IntervalProperty);
            }
            set {
                SetValue(IntervalProperty, value);
            }
        }

        public RiseUpStackLayout()
        {
            InitializeComponent();
            this.SizeChanged += RiseUpStackLayout_SizeChanged;
        }

        private void RiseUpStackLayout_SizeChanged(object sender, EventArgs e)
        {
            UpdateUI(this);
        }

        private void CollapseButton_Clicked(object sender, EventArgs e)
        {
            IsExpanded = !IsExpanded;
        }

        public static void IsExpandedPropertyChanged(BindableObject bindableObject, object oldValue, object newValue)
        {
            UpdateUI((RiseUpStackLayout)bindableObject, newValue);
        }

        public static void ContentPropertyChanged(BindableObject bindableObject, object oldValue, object newValue) {
            var RiseUpStackLayout = (RiseUpStackLayout)bindableObject;
            if (oldValue is View)
            {
                RiseUpStackLayout.Children.Remove((View)oldValue);
            }
            if (newValue is View)
            {
                var view = (View)newValue;
                view.HorizontalOptions = LayoutOptions.FillAndExpand;
                view.VerticalOptions = LayoutOptions.FillAndExpand;
                RiseUpStackLayout.Children.Add(view);
            }
        }

        public static void UpdateUI(RiseUpStackLayout obj, object knownValue = null) {
            var isExpanded = knownValue != null ? (bool)knownValue : obj.IsExpanded;
            if (isExpanded)
            {
                obj.TranslateTo(0, 0, obj.Interval, Easing.CubicInOut);
                obj.ArrowImage.RotateTo(180, obj.Interval, Easing.CubicInOut);
            }
            else
            {
                obj.TranslateTo(0, obj.Height - obj.CollapseButton.Height, obj.Interval, Easing.CubicInOut);
                obj.ArrowImage.RotateTo(0, obj.Interval, Easing.CubicInOut);
            }
        }
    }
}