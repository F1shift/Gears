using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;
using Gears.Models;

namespace Gears.ViewModels
{
    class DBItemViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string Name { 
            get { return DBModel.Name; }
            set
            {
                if (DBModel.Name != value)
                {
                    DBModel.Name = value;
                }
            }
        }
        public string Discription
        {
            get
            {
                return $"Module : {DBModel.mn}, Teeth Number : [{DBModel.z1}, {DBModel.z2}]";
            }
        }
        public string Created
        {
            get { return DBModel.Created; }
            set
            {
                if (DBModel.Created != value)
                {
                    DBModel.Created = value;
                }
            }
        }
        public string LastUsed
        {
            get { return DBModel.LastUsed; }
            set
            {
                if (DBModel.LastUsed != value)
                {
                    DBModel.LastUsed = value;
                }
            }
        }
        public bool IsSelected { get; set; }
        public static StaticSettings Settings { get; set;} = new StaticSettings();
        public static SimpleCommand SwithToSelectionMode { get; set; } = new SimpleCommand(async (para) =>
        {
            Settings.IsSelectionMode = true;
            return true;
        });

        public CylindricalGearDBModel DBModel { get; set; }

        public GearDetailViewModel GearDetailViewModel
        { get {
                var vm = new GearDetailViewModel() { Model = new CylindricalGearBasic() };
                DBModel.CopyTo(vm.Model);
                return vm;
            } }


        public class StaticSettings : INotifyPropertyChanged
        {
            public bool IsSelectionMode { get; set; } = false;

            public event PropertyChangedEventHandler PropertyChanged;
        }
    }
}
