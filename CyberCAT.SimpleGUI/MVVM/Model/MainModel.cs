using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberCAT.SimpleGUI.MVVM.Model
{
    public static class MainModel
    {
        private static string _status = "No save file selected.";

        public delegate void StatusChangedHandler(string status);
        public static event StatusChangedHandler StatusChanged;

        public static string Status
        {
            get
            {
                return _status;
            }
            set
            {
                _status = value;
                StatusChanged?.Invoke(_status);
            }
        }
    }
}
