using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Xamarin.Forms;
using Gears.ViewModels;

namespace Gears.ViewModels
{
    class DesignViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public GearParameterViewModel GearParameterViewModel { get; set; }

        public RackParameterViewModel RackParameterViewModel { get; set; }

        public GearDetailViewModel GearDetailViewModel { get; set; }

        public ThreeDModelingViewModel ThreeDModelingViewModel { get; set; }

        public bool IsDetailViewPined { get; set; }
        public SimpleCommand UpdateCommand { get; set; }
        public SimpleCommand SaveProjectCommand { get; set; }

        public DesignViewModel()
        {
        }

        public async Task<bool> Initialize() {
            RackParameterViewModel = new RackParameterViewModel();
            await RackParameterViewModel.Initialize();
            GearParameterViewModel = new GearParameterViewModel();
            GearDetailViewModel = new GearDetailViewModel();
            ThreeDModelingViewModel = new ThreeDModelingViewModel(GearDetailViewModel);
            GearDetailViewModel.GearParameterViewModel = this.GearParameterViewModel;
            GearDetailViewModel.RackParameterViewModel = this.RackParameterViewModel;

            UpdateCommand = new SimpleCommand(async (para) => { await Update(); return true; });
            SaveProjectCommand = new SimpleCommand(async (para) => { SaveProject(); return true; });
            return true;
        }

        public async Task<object> Update() {
            var updated = await Device.InvokeOnMainThreadAsync<bool>( ()=> GearDetailViewModel.CheckUpdate());
            if (updated)
                await ThreeDModelingViewModel.UpdateOrAddGear();
            return null;
        }

        System.Timers.Timer autoUpdateTimer = new System.Timers.Timer();
        public void StartAutoUpdate(int interval = 100) {
            autoUpdateTimer.Stop();
            autoUpdateTimer = new System.Timers.Timer();
            autoUpdateTimer.Interval = interval;
            autoUpdateTimer.Elapsed += (s, e) => UpdateCommand.Execute(null);
            autoUpdateTimer.Start();
        }

        public void StopAutoUpdate() {
            autoUpdateTimer.Stop();
        }

        public void InvokeCSHandler(string data) {
            switch (data)
            {
                case "Update":
                    UpdateCommand.Execute(null);
                    break;
                case "UpdateWebSide":
                    GearDetailViewModel.CheckUpdate();
                    ThreeDModelingViewModel.UpdateCommand.Execute(null);
                    break;
                case "StartAutoUpdate":
                    StartAutoUpdate();
                    break;
                default:
                    break;
            }
        }

        public void SaveProject() {
            App.AppViewModel.BrowseViewModel.SaveProject(GearDetailViewModel.Model);
        }
    }
}
