using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace WpfClient.Converters
{
  

    [ValueConversion(typeof(MethodInfo), typeof(Color))]
    public class MethodToColorValueConverter : IValueConverter
    {
        public object Convert(
         object value, Type targetType,
         object parameter, CultureInfo culture)
        {
            MethodInfo number = (MethodInfo)System.Convert.ChangeType(value, typeof(MethodInfo));

            if (number.IsAbstract)
                return Colors.Plum;
            else
           
                return Colors.SeaGreen;

            return Colors.White;
        }

        public object ConvertBack(
         object value, Type targetType,
         object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("ConvertBack not supported");
        }
    }
}
