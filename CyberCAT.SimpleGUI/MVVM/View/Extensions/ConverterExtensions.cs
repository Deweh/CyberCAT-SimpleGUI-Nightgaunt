using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using CyberCAT.Core.Classes.NodeRepresentations;
using CyberCAT.SimpleGUI.Core.Helpers;

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
            var itmClass = "Unknown";
            var strTdbid = valItm.ItemTdbId.ToString();
            if (ResourceHelper.ItemClasses.ContainsKey(strTdbid))
            {
                itmClass = ResourceHelper.ItemClasses[strTdbid];
            }

            if (valItm.Data is ItemData.ModableItemData)
            {
                return $"[M] {itmClass}";
            }
            else if (valItm.Data is ItemData.ModableItemWithQuantityData)
            {
                return $"[M+] {itmClass}";
            }
            else
            {
                return $"[S] {itmClass}";
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}