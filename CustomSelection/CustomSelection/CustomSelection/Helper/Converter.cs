using Syncfusion.DataSource.Extensions;
using Syncfusion.ListView.XForms;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CustomSelection
{
    public class SelectionBoolToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
                return ImageSource.FromResource("CustomSelection.Images.Selected.png");
            else
                return ImageSource.FromResource("CustomSelection.Images.NotSelected.png");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class SelectionModeToVisbileConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((Syncfusion.ListView.XForms.SelectionMode)value == Syncfusion.ListView.XForms.SelectionMode.Multiple)
                return true;
            else
                return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class GroupingSelectionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return value;

            GroupResult groupResult = value as GroupResult;
            SfListView listview = parameter as SfListView;

            var items = new List<MusicInfo>(groupResult.Items.ToList<MusicInfo>());

            if ((items.All(listitem => listitem.IsSelected == false)))
            {
                for (int i = 0; i < items.Count(); i++)
                {
                    var item = items[i];
                    (item as MusicInfo).IsSelected = false;
                    listview.SelectedItems.Remove(item);
                }
                return ImageSource.FromResource("CustomSelection.Images.NotSelected.png");
            }

            else if ((items.All(listitem => listitem.IsSelected == true)))
            {
                for (int i = 0; i < items.Count(); i++)
                {
                    var item = items[i];
                    (item as MusicInfo).IsSelected = true;
                    listview.SelectedItems.Add(item);
                }

                return ImageSource.FromResource("CustomSelection.Images.Selected.png");
            }

            else
            return ImageSource.FromResource("CustomSelection.Images.Intermediate.png");
            
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
