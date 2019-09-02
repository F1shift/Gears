using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Gears.Views
{
    public interface IPopup
    {
        void ShowPopup(View content);
        void ClosePopup();
        Page GetPage();
    }
}
