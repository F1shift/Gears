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
            try
            {
                AppSettings = await JIS1701DataBase.DataBase.Table<AppSettings>().FirstAsync();
            }
            catch
            {
                AppSettings = new AppSettings();
            }
            AppViewModel = new ViewModels.AppViewModel();
            await AppViewModel.Initialize();
            if (AppSettings.LastUsedProjectID != null)
            {
                AppViewModel.BrowseViewModel.OpenProject((int)AppSettings.LastUsedProjectID);
            }
            DependencyService.Register<MockDataStore>();
            AppMainPage = new MainPage();
            MainPage = AppMainPage;
        }

        protected override async void OnSleep()
        {
            AppSettings.LastUsedProjectID = AppViewModel.BrowseViewModel.CurrentProject.DBModel.Id;
            await JIS1701DataBase.DataBase.CreateTableAsync<AppSettings>();
            await JIS1701DataBase.DataBase.InsertOrReplaceAsync(AppSettings);
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
