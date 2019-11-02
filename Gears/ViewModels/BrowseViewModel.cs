using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using Gears.Models;
using Gears.ViewModels;
using Gears.DataBases;
using Gears.Custom.Effects;

namespace Gears.ViewModels
{
    class BrowseViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<DBItemViewModel> ProjectList { get; set; }
        public ObservableCollection<DBItemViewModel> SeachResultList { get; set; }
        public ObservableCollection<DBItemViewModel> CurrentProject { get; set; }
        public bool IsSelectionMode { get; set; } = false;
        public SimpleCommand SwithToSelectionModeCommand { get; set; }
        public SimpleCommand QuitSelectionModeCommand { get; set; }

        public BrowseViewModel()
        {
            Initialize();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public SimpleCommand AddNewCommand { get; set; }

        protected virtual async void Initialize() {
            ProjectList = new ObservableCollection<DBItemViewModel>();
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
            SwithToSelectionModeCommand = new SimpleCommand(async (para) =>
            {
                SwithToSelectionMode();
                return true;
            });
            QuitSelectionModeCommand = new SimpleCommand(async (para) =>
            {
                QuitSelectionMode();
                return true;
            });
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

        public void SwithToSelectionMode()
        {
            IsSelectionMode = true;
        }

        public void QuitSelectionMode()
        {
            foreach (var item in ProjectList)
            {
                item.IsSelected = false;
            }
            IsSelectionMode = false;
        }

        public void DeleteSelectedItem()
        {
            var selected = from item in ProjectList
                           where item.IsSelected
                           select item;
            foreach (var item in selected)
            {
                ProjectList.Remove(item);
            }
        }
    }
}
