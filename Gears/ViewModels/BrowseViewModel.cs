using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Gears.Models;
using Gears.DataBases;

namespace Gears.ViewModels
{
    class BrowseViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<DBItemViewModel> GearList { get; set; }
        public BrowseViewModel()
        {
            Initialize();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual async void Initialize() {
            GearList = new ObservableCollection<DBItemViewModel>();

            var database = JIS1701DataBase.DataBase;
            await database.DeleteAllAsync<CylindricalGearDBModel>();
            await database.CreateTableAsync<CylindricalGearDBModel>();
            if (await database.Table<CylindricalGearDBModel>().CountAsync() == 0)
            {
                var gearDBModel = new CylindricalGearDBModel();
                var gearbasic = new CylindricalGearBasic();
                gearbasic.SetToDefualt();
                gearDBModel.CopyFrom(gearbasic);
                await database.InsertAsync(gearDBModel);
            }
            foreach (var item in await database.Table<CylindricalGearDBModel>().ToListAsync())
            {
                GearList.Add(new DBItemViewModel() { DBModel = item });
            }
        }
    }
}
