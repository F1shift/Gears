﻿using System;
using System.Collections.Generic;
using System.Text;
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

        public SimpleCommand UpdateCommand { get; set; }

        public DesignViewModel()
        {
            RackParameterViewModel = new RackParameterViewModel();
            GearParameterViewModel = new GearParameterViewModel();
            GearDetailViewModel = new GearDetailViewModel();
            ThreeDModelingViewModel = new ThreeDModelingViewModel(GearDetailViewModel);
            GearDetailViewModel.GearParameterViewModel = this.GearParameterViewModel;
            GearDetailViewModel.RackParameterViewModel = this.RackParameterViewModel;

            UpdateCommand = new SimpleCommand((obj) => { Update(); });
        }

        public void Update() {
            var updated = GearDetailViewModel.CheckUpdate();
            if(updated)
                ThreeDModelingViewModel.UpdateCommand.Execute(null);
        }

        System.Timers.Timer autoUpdateTimer = new System.Timers.Timer();
        public void StartAutoUpdate(int interval = 100) {
            autoUpdateTimer.Interval = interval;
            autoUpdateTimer.Elapsed += (s, e) => Device.BeginInvokeOnMainThread(()=>UpdateCommand.Execute(null));
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
                case "StartAutoUpdate":
                    StartAutoUpdate();
                    break;
                default:
                    break;
            }
        }
    }
}
