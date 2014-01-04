using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace WpfClient
{
    class Utils
    {
        internal static Brush PickBrush()
        {
            Brush result = Brushes.Black;

            Random rnd = new Random();

            Type brushesType = typeof(Brushes);

            PropertyInfo[] properties = brushesType.GetProperties();

            int random = rnd.Next(properties.Length);
            result = (Brush)properties[random].GetValue(null, null);

            //return result;
            return new SolidColorBrush(Colors.Red);
        }
    }
}
