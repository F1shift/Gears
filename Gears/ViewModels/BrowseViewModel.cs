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
        ObservableCollection<DBItemViewModel> _ProjectList;
        public ObservableCollection<DBItemViewModel> ProjectList { 
            get {
                if (_ProjectList == null)
                {
                    _ProjectList = new ObservableCollection<DBItemViewModel>();
                }
                return _ProjectList;
            }
        }
        ObservableCollection<DBItemViewModel> _SeachResultList;
        public ObservableCollection<DBItemViewModel> SeachResultList { 
            get {
                if (_SeachResultList == null)
                {
                    _SeachResultList = new ObservableCollection<DBItemViewModel>();
                }
                return _SeachResultList;
            }
        }
        public DBItemViewModel CurrentProject { get; set; }
        public bool IsSelectionMode { get; set; } = false;
        

        public BrowseViewModel()
        {
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public SimpleCommand AddNewCommand { get; set; }
        public SimpleCommand SwithToSelectionModeCommand { get; set; }
        public SimpleCommand SelectionAllCommand { get; set; }
        public SimpleCommand QuitSelectionModeCommand { get; set; }
        public SimpleCommand DeleteSelectedItemCommand { get; set; }
        public SimpleCommand OpenProjectCommand { get; set; }

        public virtual async Task<bool> Initialize() {
            var database = JIS1701DataBase.DataBase;
            await database.CreateTableAsync<CylindricalGearDBModel>();
            foreach (var item in await database.Table<CylindricalGearDBModel>().ToListAsync())
            {
                var dbItemViewModel = new DBItemViewModel() { DBModel = item };
                ProjectList.Add(dbItemViewModel);
            }
            AddNewCommand = new SimpleCommand(async (name) => await AddNew((string)name));
            SwithToSelectionModeCommand = new SimpleCommand(async (para) =>
            {
                SwithToSelectionMode();
                var vm = para as DBItemViewModel;
                if (vm != null)
                {
                    vm.IsSelected = true;
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
            return true;
        }

        async Task<bool> AddNew(string projectName = "new project", int? id = null) {
            var gearDBModel = new CylindricalGearDBModel();
            var gearbasic = new CylindricalGearBase();
            gearbasic.SetToDefualt();
            gearDBModel.CopyFrom(gearbasic);
            gearDBModel.Name = projectName;
            gearDBModel.Id = id;
            await JIS1701DataBase.DataBase.InsertOrReplaceAsync(gearDBModel);
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

        public async void OpenProject(int id)
        {
            if (ProjectList.Count == 0)
            {
                await AddNew(id: id);
            }
            var project = ProjectList.FirstOrDefault((pj) => pj.DBModel.Id == id);
            if (project == null)
            {
                await AddNew(id: id);
                project = ProjectList.FirstOrDefault((pj) => pj.DBModel.Id == id);
            }
            if (CurrentProject != null)
            {
                CurrentProject.IsCurrent = false;
            }
            project.IsCurrent = true;
            CurrentProject = project;
            var gearBase = project.DBModel.GetGearBase();
            App.AppViewModel.DesignViewModel.RackParameterViewModel.CopyFrom(gearBase);
            App.AppViewModel.DesignViewModel.GearParameterViewModel.CopyFrom(gearBase);
            App.AppViewModel.DesignViewModel.GearDetailViewModel.UpdateCommand.Execute(null);
        }

        public void OpenProject(DBItemViewModel dbvm)
        {
            var gearBase = new CylindricalGearBase();
            dbvm.DBModel.CopyTo(gearBase);
            if (CurrentProject != null)
            {
                CurrentProject.IsCurrent = false;
            }
            dbvm.IsCurrent = true;
            CurrentProject = dbvm;
            App.AppViewModel.DesignViewModel.RackParameterViewModel.CopyFrom(gearBase);
            App.AppViewModel.DesignViewModel.GearParameterViewModel.CopyFrom(gearBase);
            App.AppViewModel.DesignViewModel.GearDetailViewModel.UpdateCommand.Execute(null);
        }

        public async void SaveProject(CylindricalGearBase gearBase)
        {
            CurrentProject.DBModel.CopyFrom(gearBase);
            if (ProjectList.Contains(CurrentProject) == false)
            {
                ProjectList.Add(CurrentProject);
            }
             await JIS1701DataBase.DataBase.InsertOrReplaceAsync(CurrentProject.DBModel);
        }
    }
}
