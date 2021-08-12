using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using CyberCAT.Core.Classes.NodeRepresentations;

namespace CyberCAT.SimpleGUI.MVVM.View
{
    [ValueConversion(typeof(double), typeof(double))]
    public class SubtractionConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double result = 0;

            if (double.TryParse(value.ToString(), out double valDbl) && double.TryParse(parameter.ToString(), out double paramDbl))
            {
                result = valDbl - paramDbl;
            }

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    [ValueConversion(typeof(ItemData), typeof(string))]
    public class ItemToTypeStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var valItm = value as ItemData;

            if (valItm.Data is ItemData.ModableItemData)
            {
                return "[M]";
            }
            else if (valItm.Data is ItemData.ModableItemWithQuantityData)
            {
                return "[M+]";
            }
            else
            {
                return "[S]";
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}