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

        public BrowseViewModel BrowseViewModel { get; set; }
        public DesignViewModel DesignViewModel { get; set; }
        public SettingsViewModel SettingsViewModel { get; set; }

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
