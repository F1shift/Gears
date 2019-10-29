using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using Gears.Models;
using Gears.DataBases;
using Gears.Custom.Effects;

namespace Gears.ViewModels
{
    class BrowseViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<DBItemViewModel> ProjectList { get; set; }
        public ObservableCollection<DBItemViewModel> SelectedProjectList { get; set; }
        public ObservableCollection<DBItemViewModel> SeachResultList { get; set; }
        public ObservableCollection<DBItemViewModel> CurrentProject { get; set; }
        public BrowseViewModel()
        {
            Initialize();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public SimpleCommand AddNewCommand { get; set; }

        protected virtual async void Initialize() {
            ProjectList = new ObservableCollection<DBItemViewModel>();
            SelectedProjectList = new ObservableCollection<DBItemViewModel>();
            SeachResultList = new ObservableCollection<DBItemViewModel>();

            var database = JIS1701DataBase.DataBase;
            await database.CreateTableAsync<CylindricalGearDBModel>();
            if (await database.Table<CylindricalGearDBModel>().CountAsync() == 0)
            {
                await AddNew();
                await AddNew();
                await AddNew();
            }
            foreach (var item in await database.Table<CylindricalGearDBModel>().ToListAsync())
            {
                var dbItemViewModel = new DBItemViewModel() { DBModel = item };
                ProjectList.Add(dbItemViewModel);
            }
            AddNewCommand = new SimpleCommand(async (name) => await AddNew((string)name));
        }

        async Task<bool> AddNew(string projectName = "new project") {
            var gearDBModel = new CylindricalGearDBModel();
            var gearbasic = new CylindricalGearBasic();
            gearbasic.SetToDefualt();
            gearDBModel.CopyFrom(gearbasic);
            gearDBModel.Name = projectName;
            await JIS1701DataBase.DataBase.InsertAsync(gearDBModel);
            ProjectList.Add(new DBItemViewModel() { DBModel = gearDBModel });
            return true;
        }
    }
}
