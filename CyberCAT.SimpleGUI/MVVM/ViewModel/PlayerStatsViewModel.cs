using CyberCAT.SimpleGUI.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CyberCAT.SimpleGUI.MVVM.Model;

namespace CyberCAT.SimpleGUI.MVVM.ViewModel
{
    class PlayerStatsViewModel : ObservableObject
    {
        public int BodyAttribute
        {
            get
            {
                return PlayerStatsModel.Get("BodyAttribute");
            }
            set
            {
                PlayerStatsModel.Set("BodyAttribute", value);
            }
        }
        
    }
}
