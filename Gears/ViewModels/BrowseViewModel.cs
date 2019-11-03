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
        public DBItemViewModel CurrentProject { get; set; }
        public bool IsSelectionMode { get; set; } = false;
        

        public BrowseViewModel()
        {
            Initialize();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public SimpleCommand AddNewCommand { get; set; }
        public SimpleCommand SwithToSelectionModeCommand { get; set; }
        public SimpleCommand SelectionAllCommand { get; set; }
        public SimpleCommand QuitSelectionModeCommand { get; set; }
        public SimpleCommand DeleteSelectedItemCommand { get; set; }
        public SimpleCommand OpenProjectCommand { get; set; }

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
                if (SwithToSelectionMode())
                {
                    var vm = para as DBItemViewModel;
                    if (vm != null)
                    {
                        vm.IsSelected = true;
                    }
                }
                return true;
            });
            SelectionAllCommand = new SimpleCommand(async (para) =>
            {
                SelectAll();
                return true;
            });
            QuitSelectionModeCommand = new SimpleCommand(async (para) =>
            {
                QuitSelectionMode();
                return true;
            });
            DeleteSelectedItemCommand = new SimpleCommand(async (para) => {
                DeleteSelectedItem();
                return true;
            });
            OpenProjectCommand = new SimpleCommand(async (para) =>
            {
                OpenProject(para as DBItemViewModel);
                return true;
            });
        }

        async Task<bool> AddNew(string projectName = "new project") {
            var gearDBModel = new CylindricalGearDBModel();
            var gearbasic = new CylindricalGearBase();
            gearbasic.SetToDefualt();
            gearDBModel.CopyFrom(gearbasic);
            gearDBModel.Name = projectName;
            await JIS1701DataBase.DataBase.InsertAsync(gearDBModel);
            ProjectList.Add(new DBItemViewModel() { DBModel = gearDBModel });
            return true;
        }

        public bool SwithToSelectionMode()
        {
            if (IsSelectionMode == false)
            {
                IsSelectionMode = true;
                return true;
            }
            else
            {
                return false;
            }
        }

        public void QuitSelectionMode()
        {
            foreach (var item in ProjectList)
            {
                item.IsSelected = false;
            }
            IsSelectionMode = false;
        }

        public async void DeleteSelectedItem()
        {
            var selected = (from item in ProjectList
                           where item.IsSelected
                           select item).ToList();
            var db = JIS1701DataBase.DataBase;
            foreach (var item in selected)
            {
                ProjectList.Remove(item);
                await db.DeleteAsync(item.DBModel);
            }
            QuitSelectionMode();
        }

        public void SelectAll(){
            foreach (var item in ProjectList)
            {
                item.IsSelected = true;
            }
        }

        public void OpenProject(DBItemViewModel dbvm)
        {
            var gearBase = new CylindricalGearBase();
            dbvm.DBModel.CopyTo(gearBase);
            CurrentProject = dbvm;
            App.AppViewModel.DesignViewModel.RackParameterViewModel.CopyFrom(gearBase);
            App.AppViewModel.DesignViewModel.GearParameterViewModel.CopyFrom(gearBase);
            App.AppViewModel.DesignViewModel.GearDetailViewModel.UpdateCommand.Execute(null);
        }

        public async void SaveProject(CylindricalGearBase gearBase)
        {
            CurrentProject.DBModel.CopyFrom(gearBase);
             await JIS1701DataBase.DataBase.InsertOrReplaceAsync(CurrentProject.DBModel);
        }
    }
}
