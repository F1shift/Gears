using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Gears.Custom.Effects;
using Gears.ViewModels;


namespace Gears.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BrowesPage : ContentPage
    {
        private PopupController popupController = new PopupController();
        private NewProjectView _NewProjectView;
        private NewProjectView NewProjectView
        {
            get{
                if (_NewProjectView == null)
                {
                    _NewProjectView = new NewProjectView();
                    _NewProjectView.OKButton.Clicked += (o, e) =>
                    {
                        var vm = (BrowseViewModel)this.BindingContext;
                        vm.AddNewCommand.Execute(_NewProjectView.Entry.Text);
                        popupController.ClosePopup();
                    };
                    _NewProjectView.CancelButton.Clicked += (o, e) =>
                    {
                        popupController.ClosePopup();
                    };
                }
                return _NewProjectView;
            }
            set { 

            }
        }

        ToolbarItem toolbarItem_Search = new ToolbarItem()
        {
            IconImageSource = ImageSource.FromResource("Gears.Resources.search.png"),
            Text = "Search"
        };
        ToolbarItem toolbarItem_Add = new ToolbarItem()
        {
            IconImageSource = ImageSource.FromResource("Gears.Resources.add.png"),
            Text = "Add"
        };
        ToolbarItem toolbarItem_SelectAll = new ToolbarItem()
        {
            IconImageSource = ImageSource.FromResource("Gears.Resources.select-all.png"),
            Text = "Select All"
        };
        ToolbarItem toolbarItem_Delete = new ToolbarItem()
        {
            IconImageSource = ImageSource.FromResource("Gears.Resources.trash.png"),
            Text = "Delete"
        };
        ToolbarItem toolbarItem_ClearSelection = new ToolbarItem()
        {
            IconImageSource = ImageSource.FromResource("Gears.Resources.clear.png"),
            Text = "Clear"
        };

        public BrowesPage()
        {
            InitializeComponent();
            var vm = App.AppViewModel.BrowseViewModel;
            this.BindingContext = vm;

            toolbarItem_Add.Clicked += this.addButton_Clicked;
            toolbarItem_SelectAll.SetBinding(ToolbarItem.CommandProperty, nameof(vm.SelectionAllCommand));
            toolbarItem_Delete.SetBinding(ToolbarItem.CommandProperty, nameof(vm.DeleteSelectedItemCommand));
            toolbarItem_ClearSelection.SetBinding(ToolbarItem.CommandProperty, nameof(vm.QuitSelectionModeCommand));

            App.AppViewModel.BrowseViewModel.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(vm.IsSelectionMode))
                {
                    ToolbarItems.Clear();
                    if (vm.IsSelectionMode)
                    {
                        ToolbarItems.Add(toolbarItem_ClearSelection);
                        ToolbarItems.Add(toolbarItem_SelectAll);
                        ToolbarItems.Add(toolbarItem_Delete);
                    }
                    else
                    {
                        ToolbarItems.Add(toolbarItem_Search);
                        ToolbarItems.Add(toolbarItem_Add);
                    }
                }
            };
            UpdateToolbar();
        }

        private void addButton_Clicked(object sender, EventArgs e)
        {
            NewProjectView.Entry.Text = "New Project";
            popupController.ShowPopup(this.absLayout, NewProjectView);
        }

        public void UpdateToolbar() {
            var vm = (ViewModels.BrowseViewModel)this.BindingContext;
            ToolbarItems.Clear();
            if (vm.IsSelectionMode)
            {
                ToolbarItems.Add(toolbarItem_Delete);
                ToolbarItems.Add(toolbarItem_SelectAll);
            }
            else
            {
                ToolbarItems.Add(toolbarItem_Search);
                ToolbarItems.Add(toolbarItem_Add);
            }
        }
    }
}