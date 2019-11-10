using System;
using System.Collections.Generic;
using System.Globalization;
using System.ComponentModel;
using System.Text;
using Xamarin.Forms;
using Gears.Models;
using static Gears.Utility.Math;

namespace Gears.Views
{
    class GearModelConverter : IValueConverter
    {
        public string BasicFormat { get; set; } = "0.###";
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }
            var model = (CylindricalGearBase)value;
            List<TableRow> RowCollection = new List<TableRow>();

            RowCollection.Add(new TitleRow() { Title = "Cylindrical Gear Parameters" });
            RowCollection.Add(new SubtitleRow() { Title = "Input Parameters" });
            RowCollection.Add(new DoubleParameterRow() { Value1 = "Pinion", Value2 = "Wheel"});
            RowCollection.Add(new SingleParameterRow() { Name = "Normal Module", Value = model.mn.ToString(BasicFormat) });
            RowCollection.Add(new SingleParameterRow() { Name = "Normal Pressure Angle", Value = model.αn.RadToDeg().ToString(BasicFormat) });
            RowCollection.Add(new SingleParameterRow() { Name = "Helix Angle", Value = String.Format("{0:" + BasicFormat + "}°", model.β.RadToDeg()) });
            RowCollection.Add(new SingleParameterRow() { Name = "Addendum Coeffifient", Value = model.ha_c.ToString(BasicFormat) });
            RowCollection.Add(new SingleParameterRow() { Name = "Dedendum Coeffifient", Value = model.hf_c.ToString(BasicFormat) });
            RowCollection.Add(new DoubleParameterRow() { Name = "Teeth Number", Value1 = model.z[0].ToString(BasicFormat), Value2 = model.z[1].ToString(BasicFormat) });
            RowCollection.Add(new DoubleParameterRow() { Name = "Fillet Coeffifient", Value1 = model.ρ_c[0].ToString(BasicFormat), Value2 = model.ρ_c[1].ToString(BasicFormat) });
            RowCollection.Add(new DoubleParameterRow() { Name = "Face Width Coeffifient", Value1 = model.b_c[0].ToString(BasicFormat), Value2 = model.b_c[1].ToString(BasicFormat) });
            RowCollection.Add(new SubtitleRow() { Title = "Calculated Parameters" });
            RowCollection.Add(new SingleParameterRow() { Name = "Transverse Module", Value = model.mt.ToString(BasicFormat)});
            RowCollection.Add(new SingleParameterRow() { Name = "Center Distance Modify Coefficient", Value = model.y.ToString(BasicFormat)});
            RowCollection.Add(new SingleParameterRow() { Name = "Center Distance", Value = model.a.ToString(BasicFormat)});
            RowCollection.Add(new SingleParameterRow() { Name = "Transverse Pressure Angle", Value = model.αt.RadToDeg().ToString(BasicFormat)});
            RowCollection.Add(new SingleParameterRow() { Name = "Operating Pressure Angle", Value = model.αwt.RadToDeg().ToString(BasicFormat)});
            RowCollection.Add(new DoubleParameterRow() { Name = "Lead", Value1 = model.L[0].ToString(BasicFormat), Value2 = model.L[1].ToString(BasicFormat)});
            RowCollection.Add(new DoubleParameterRow() { Name = "Standard Pitch Diameter", Value1 = model.d[0].ToString(BasicFormat), Value2 = model.d[1].ToString(BasicFormat) });
            RowCollection.Add(new DoubleParameterRow() { Name = "Operating Pitch Diameter", Value1 = model.dw[0].ToString(BasicFormat), Value2 = model.dw[1].ToString(BasicFormat) });
            RowCollection.Add(new DoubleParameterRow() { Name = "Addendum Diameter", Value1 = model.da[0].ToString(BasicFormat), Value2 = model.da[1].ToString(BasicFormat) });
            RowCollection.Add(new DoubleParameterRow() { Name = "Root Diameter", Value1 = model.df[0].ToString(BasicFormat), Value2 = model.df[1].ToString(BasicFormat) });
            RowCollection.Add(new DoubleParameterRow() { Name = "Base Circle Diameter", Value1 = model.db[0].ToString(BasicFormat), Value2 = model.db[1].ToString(BasicFormat) });
            RowCollection.Add(new DoubleParameterRow() { Name = "Addendum Height", Value1 = model.ha[0].ToString(BasicFormat), Value2 = model.ha[1].ToString(BasicFormat) });
            RowCollection.Add(new DoubleParameterRow() { Name = "Dedendum Height", Value1 = model.hf[0].ToString(BasicFormat), Value2 = model.hf[1].ToString(BasicFormat) });
            RowCollection.Add(new DoubleParameterRow() { Name = "Hall Teeth Height", Value1 = model.h[0].ToString(BasicFormat), Value2 = model.h[1].ToString(BasicFormat) });
            RowCollection.Add(new DoubleParameterRow() { Name = "Face Width", Value1 = model.b[0].ToString(BasicFormat), Value2 = model.b[1].ToString(BasicFormat) });

            return RowCollection;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }

    public abstract class TableRow : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class TitleRow : TableRow
    {
        public string Title { get; set; }
    }
    public class SubtitleRow : TitleRow
    {
    }
    public class DockPanelRow : TableRow
    {
        public string Left { get; set; }
        public string Center { get; set; }
        public string Right { get; set; }
    }

    public class SingleParameterRow : TableRow
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
    public class DoubleParameterRow : TableRow
    {
        public string Name { get; set; }
        public string Value1 { get; set; }
        public string Value2 { get; set; }
    }
}
