 using System;
using System.IO;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Gears.Services;
using Gears.Views;
using Gears.Models;
using System.Collections.Generic;
using System.Reflection;
using Gears.ViewModels;

namespace Gears
{
    public partial class App : Application
    {
        internal static AppViewModel AppViewModel { get; set; } 
        internal static MainPage AppMainPage { get; set; }
        public App()
        {
            AppViewModel = new ViewModels.AppViewModel();
            InitializeComponent();

            DependencyService.Register<MockDataStore>();
            AppMainPage = new MainPage();
            MainPage = AppMainPage;
        }

        protected override void OnStart()
        {
            
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
