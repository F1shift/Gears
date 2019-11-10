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
        public bool IsCurrent { get; set; }

        public CylindricalGearDBModel DBModel { get; set; }

        public GearDetailViewModel GearDetailViewModel
        { 
            get {
                var vm = new GearDetailViewModel() { Model = new CylindricalGearBase() };
                DBModel.CopyTo(vm.Model);
                return vm;
            } 
        }

        public DBItemViewModel()
        {
            
        }

        public void CopyFrom(CylindricalGearBase source)
        {
            DBModel.CopyFrom(source);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DBModel)));
        }
    }
}
