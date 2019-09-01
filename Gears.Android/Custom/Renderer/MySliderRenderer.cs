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

using Xamarin.Forms.Platform.Android;
using Xamarin.Forms;
using Gears.Custom.Controls;
using Gears.Droid.Custom.Renderer;

[assembly: ExportRenderer(typeof(MySlider), typeof(MySliderRenderer))]
namespace Gears.Droid.Custom.Renderer
{
    public class MySliderRenderer : SliderRenderer
    {
        public MySliderRenderer(Context context) : base(context)
        {
            
        }

        public override bool DispatchTouchEvent(MotionEvent e)
        {
            switch (e.Action)
            {
                case MotionEventActions.Down:
                    FindParentTappedPage().ForEach((tabbedPage) => { tabbedPage.RequestDisallowInterceptTouchEvent(true); });
                    break;
                case MotionEventActions.Move:
                    FindParentTappedPage().ForEach((tabbedPage) => { tabbedPage.RequestDisallowInterceptTouchEvent(true); });
                    break;
                default:
                    break;
            }
            return base.DispatchTouchEvent(e);
        }

        List<IViewParent> FindParentTappedPage()
        {
            List<IViewParent> tabbedPages = new List<IViewParent>();
            var node = Parent;
            do
            {
                if (node is Xamarin.Forms.Platform.Android.PageRenderer)
                {
                    tabbedPages.Add(node);
                }
                node = node.Parent;
            } while (node != null);
            return tabbedPages;
        }
    }
}