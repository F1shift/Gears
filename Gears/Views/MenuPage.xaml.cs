using Gears.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Gears.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MenuPage : ContentPage
    {
        public MasterDetailPage RootPage;
        List<HomeMenuItem> menuItems;
        public MenuPage()
        {
            InitializeComponent();

            menuItems = new List<HomeMenuItem>
            {
                new HomeMenuItem {Title="Browse", PageType = typeof(ItemsPage)},
                new HomeMenuItem {Title="Gear Design", PageType = typeof(DesignPage)},
                new HomeMenuItem {Title="3D Modeling", PageType = typeof(ContentPage)},
                new HomeMenuItem {Title="Settings", PageType = typeof(ContentPage)},
                new HomeMenuItem {Title="About", PageType = typeof(AboutPage) }
            };

            ListViewMenu.ItemsSource = menuItems;

            ListViewMenu.SelectedItem = menuItems[0];
            ListViewMenu.ItemSelected += (sender, e) =>
            {
                if (e.SelectedItem == null)
                    return;
                NavigateTo(e.SelectedItemIndex);
            };
        }

        public async void NavigateTo(int i)
        {
            if (((IList<HomeMenuItem>)ListViewMenu.ItemsSource).IndexOf((HomeMenuItem)ListViewMenu.SelectedItem) != i)
            {
                ListViewMenu.SelectedItem = menuItems[i];
            }

            if (menuItems[i].PageInstance == null)
            {
                menuItems[i].PageInstance = (Page)Activator.CreateInstance(menuItems[i].PageType);
            }
            RootPage.Detail = new NavigationPage(menuItems[i].PageInstance);

            if (Device.RuntimePlatform == Device.Android)
                await Task.Delay(100);

            RootPage.IsPresented = false;
        }
    }
    

    public class HomeMenuItem
    {
        public string Title { get; set; }

        private Type _PageType;
        public Type PageType {
            get{
                return _PageType;
            }
            set {
                if (value.IsSubclassOf(typeof(Page)))
                {
                    _PageType = value;
                }
                else
                {
                    throw new NotSupportedException(value.ToString() + "はネビゲーションできるタイプではない。");
                }
            }
        }
        public Page PageInstance { get; set; }
    }
}