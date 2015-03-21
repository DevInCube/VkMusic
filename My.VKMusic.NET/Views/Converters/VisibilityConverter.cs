using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace My.VKMusic.Views.Converters
{
    public class VisibilityConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool invert = (parameter == null) ? false : bool.Parse(parameter.ToString()); 
            bool val = true;
            if (value == null)
                val = false;
            else if (value is bool)
                val = bool.Parse(value.ToString());
            else
                val = false; // @todo
            return (invert ? !val : val) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
