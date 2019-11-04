using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Gears.ViewModels
{
    internal class AppViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public BrowseViewModel BrowseViewModel { get; set; } = new BrowseViewModel();
        public DesignViewModel DesignViewModel { get; set; } = new DesignViewModel();
        public SettingsViewModel SettingsViewModel { get; set; } = new SettingsViewModel();

        public AppViewModel()
        {

        }

        public async Task<bool> Initialize() {
            BrowseViewModel = new BrowseViewModel();
            await BrowseViewModel.Initialize();
            DesignViewModel  = new DesignViewModel();
            await DesignViewModel.Initialize();
            SettingsViewModel  = new SettingsViewModel();
            return true;
        }
    }
}
