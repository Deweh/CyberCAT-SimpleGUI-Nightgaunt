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
                OnPropertyChanged();
            }
        }

        public int ReflexesAttribute
        {
            get
            {
                return PlayerStatsModel.Get("ReflexesAttribute");
            }
            set
            {
                PlayerStatsModel.Set("ReflexesAttribute", value);
                OnPropertyChanged();
            }
        }

        public int TechnicalAbilityAttribute
        {
            get
            {
                return PlayerStatsModel.Get("TechnicalAbilityAttribute");
            }
            set
            {
                PlayerStatsModel.Set("TechnicalAbilityAttribute", value);
                OnPropertyChanged();
            }
        }

        public int IntelligenceAttribute
        {
            get
            {
                return PlayerStatsModel.Get("IntelligenceAttribute");
            }
            set
            {
                PlayerStatsModel.Set("IntelligenceAttribute", value);
                OnPropertyChanged();
            }
        }

        public int CoolAttribute
        {
            get
            {
                return PlayerStatsModel.Get("CoolAttribute");
            }
            set
            {
                PlayerStatsModel.Set("CoolAttribute", value);
                OnPropertyChanged();
            }
        }

        public int LevelProfic
        {
            get
            {
                return PlayerStatsModel.Get("LevelProfic");
            }
            set
            {
                PlayerStatsModel.Set("LevelProfic", value);
                OnPropertyChanged();
            }
        }

        public int StreetCredProfic
        {
            get
            {
                return PlayerStatsModel.Get("StreetCredProfic");
            }
            set
            {
                PlayerStatsModel.Set("StreetCredProfic", value);
                OnPropertyChanged();
            }
        }
    }
}
