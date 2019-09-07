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

        string _Name;
        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                if (_Name != value)
                {
                    _Name = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Name)));
                }
            }
        }

        double _Value;
        public double Value
        {
            get
            {
                return _Value;
            }
            set
            {
                if (_Value != value)
                {
                    _Value = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
                }
            }
        }

        double _Max;
        public double Max
        {
            get
            {
                return _Max;
            }
            set
            {
                if (_Max != value)
                {
                    _Max = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Max)));
                }
            }
        }

        double _Min;
        public double Min
        {
            get
            {
                return _Min;
            }
            set
            {
                if (_Min != value)
                {
                    _Min = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Min)));
                }
            }
        }

        double _Step;
        public double Step
        {
            get
            {
                return _Step;
            }
            set
            {
                if (_Step != value)
                {
                    _Step = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Step)));
                }
            }
        }

        string _GuideString;
        public string GuideString
        {
            get
            {
                return _GuideString;
            }
            set
            {
                if (_GuideString != value)
                {
                    _GuideString = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(GuideString)));
                }
            }
        }

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
