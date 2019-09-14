using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using Xamarin.Forms;
using Gears.Views;
using Gears.Custom.Effects;

namespace Gears.ViewModels
{
    class InputItemViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string Name { get; set; }

        public double Value { get; set; }

        public double Max { get; set; }

        public double Min { get; set; }

        public double Step { get; set; }

        public string GuideString { get; set; }

        PopupController _popupController = new PopupController();

        NumberEntryPopupView _EntryView;
        public NumberEntryPopupView EntryView { get {
                if (_EntryView == null)
                {
                    _EntryView = new NumberEntryPopupView();
                    _EntryView.Entry.Text = Value.ToString();
                    _EntryView.TitleLable.Text = Name;
                    _EntryView.OKButton.Clicked += (o, e) =>
                    {
                        try
                        {
                            Value = Convert.ToDouble(_EntryView.Entry.Text);
                            _popupController.ClosePopup();
                        }
                        catch(Exception ex)
                        {
                            ((Page)Utility.FindPerant<Page>(_EntryView)).DisplayAlert("Error !", ex.Message, "OK");
                        }
                    };
                    _EntryView.CancelButton.Clicked += (o, e) => _popupController.ClosePopup();
                }
                return _EntryView;
            }
            set {
                _EntryView = value;   
            }
        }

        public SimpleCommand ShowEntryCommand { get; set; }

        public InputItemViewModel()
        {
            ShowEntryCommand = new SimpleCommand(
            (area) => {
                EntryView.Entry.Text = Value.ToString();
                _popupController.ShowPopup((AbsoluteLayout)area, EntryView);
            });
        }
    }
}
