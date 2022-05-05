using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using WolvenKit.RED4.Save;
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

    [ValueConversion(typeof(InventoryHelper.ItemData), typeof(string))]
    public class ItemToTypeStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var valItm = value as InventoryHelper.ItemData;

            var itmClass = "Unknown";
            var strTdbid = valItm.ItemTdbId.ToString();

            if (ResourceHelper.ItemClasses.ContainsKey(strTdbid))
            {
                itmClass = ResourceHelper.ItemClasses[strTdbid];
            }

            if (valItm.Data is InventoryHelper.ModableItemData)
            {
                return $"[M] {itmClass}";
            }
            else if (valItm.Data is InventoryHelper.ModableItemWithQuantityData)
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

    [ValueConversion(typeof(InventoryHelper.ItemData), typeof(int))]
    public class ItemToQuantityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var valItm = value as InventoryHelper.ItemData;

            if (valItm.Data is InventoryHelper.ModableItemWithQuantityData mData)
            {
                return mData.Quantity;
            }
            else if (valItm.Data is InventoryHelper.SimpleItemData sData)
            {
                return sData.Quantity;
            }

            return 1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}