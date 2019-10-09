using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Gears.Views
{
    class GearDetailViewDataTamplateSelector : DataTemplateSelector
    {
        public DataTemplate TitleTemplate { get; set; }
        public DataTemplate SubtitleTemplate { get; set; }
        public DataTemplate DockPanelRowTemplate { get; set; }
        public DataTemplate SingleParamTemplate { get; set; }
        public DataTemplate DoubleParamTemplate { get; set; }
        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            if (item is TitleRow)
            {
                return TitleTemplate;
            }
            if (item is SubtitleRow)
            {
                return SubtitleTemplate;
            }
            if (item is DockPanelRow)
            {
                return DockPanelRowTemplate;
            }
            if (item is SingleParameterRow)
            {
                return SingleParamTemplate;
            }
            if (item is DoubleParameterRow)
            {
                return DoubleParamTemplate;
            }
            throw new Exception();
        }
    }
}
