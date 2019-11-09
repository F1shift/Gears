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
using Gears.DataBases;
using SQLite;

namespace Gears
{
    public partial class App : Application
    {
        internal static AppViewModel AppViewModel { get; set; } 
        internal static AppSettings AppSettings { get; set; } 
        internal static MainPage AppMainPage { get; set; }
        public App()
        {
            InitializeComponent();
        }

        protected override async void OnStart()
        {
            AppMainPage = new MainPage();
            MainPage = AppMainPage;
            await JIS1701DataBase.Initalize();
            try
            {
                AppSettings = await JIS1701DataBase.DataBase.Table<AppSettings>().FirstAsync();
            }
            catch
            {
                AppSettings = new AppSettings();
            }
            AppViewModel = new AppViewModel();
            await AppViewModel.Initialize();
            if (AppSettings.LastUsedProjectID != null)
            {
                AppViewModel.BrowseViewModel.OpenProject((int)AppSettings.LastUsedProjectID);
            }

            AppMainPage.MenuPage.NavigateTo(1);
        }

        protected override async void OnSleep()
        {
            var current = AppViewModel?.BrowseViewModel?.CurrentProject;
            if (current != null)
            {
                AppSettings.LastUsedProjectID = AppViewModel.BrowseViewModel.CurrentProject.DBModel.Id;
                await JIS1701DataBase.DataBase.InsertOrReplaceAsync(AppSettings);
            }
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
