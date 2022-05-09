using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CyberCAT.SimpleGUI.Core.Extensions
{
    public static class StringExtensions
    {
        public static string LastOrIndex(this string[] arr, int position)
        {
            if (position == -1)
            {
                return arr.Last();
            }
            else
            {
                return arr[position];
            }
        }
    }

    public static class JsonElementExtensions
    {
        public static T ToObject<T>(this JsonElement ele)
        {
            return JsonSerializer.Deserialize<T>(ele);
        }
    }
}
