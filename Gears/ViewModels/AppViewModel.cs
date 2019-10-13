using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Gears.ViewModels
{
    internal class AppViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public BrowseViewModel BrowseViewModel { get; set; } = new BrowseViewModel();
        public DesignViewModel DesignViewModel { get; set; } = new DesignViewModel();
        public SettingsViewModel SettingsViewModel { get; set; } = new SettingsViewModel();
    }
}
