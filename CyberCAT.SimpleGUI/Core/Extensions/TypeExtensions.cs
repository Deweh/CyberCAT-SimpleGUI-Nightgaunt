using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
}
