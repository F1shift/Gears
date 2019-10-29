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
        public BrowesPage()
        {
            InitializeComponent();
            this.BindingContext = App.AppViewModel.BrowseViewModel;
        }

        private void addButton_Clicked(object sender, EventArgs e)
        {
            NewProjectView.Entry.Text = "New Project";
            popupController.ShowPopup(this.absLayout, NewProjectView);
        }
    }
}