using CyberCAT.SimpleGUI.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CyberCAT.SimpleGUI.MVVM.Model;
using System.IO;

namespace CyberCAT.SimpleGUI.MVVM.ViewModel
{
    class PlayerStatsViewModel : ObservableObject
    {
        private string _lifePathImage = string.Empty;

        public PlayerStatsViewModel()
        {
            LifePathImage = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "Images", PlayerStatsModel.GetLifePath() + ".png");

            PlayerStatsModel.LifePathChanged += (string lifePath) =>
            {
                LifePathImage = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "Images", lifePath + ".png");
            };
        }

        public string LifePathImage
        {
            get
            {
                return _lifePathImage;
            }
            set
            {
                _lifePathImage = value;
                OnPropertyChanged();
            }
        }

        public string NomadLifePath
        {
            get
            {
                return Path.Combine(Directory.GetCurrentDirectory(), "Images", "Nomad.png");
            }
        }

        public string StreetKidLifePath
        {
            get
            {
                return Path.Combine(Directory.GetCurrentDirectory(), "Images", "StreetKid.png");
            }
        }

        public string CorpoLifePath
        {
            get
            {
                return Path.Combine(Directory.GetCurrentDirectory(), "Images", "Corpo.png");
            }
        }

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

        public int AthleticsProfic
        {
            get
            {
                return PlayerStatsModel.Get("AthleticsProfic");
            }
            set
            {
                PlayerStatsModel.Set("AthleticsProfic", value);
                OnPropertyChanged();
            }
        }

        public int AnnihilationProfic
        {
            get
            {
                return PlayerStatsModel.Get("AnnihilationProfic");
            }
            set
            {
                PlayerStatsModel.Set("AnnihilationProfic", value);
                OnPropertyChanged();
            }
        }

        public int StreetBrawlerProfic
        {
            get
            {
                return PlayerStatsModel.Get("StreetBrawlerProfic");
            }
            set
            {
                PlayerStatsModel.Set("StreetBrawlerProfic", value);
                OnPropertyChanged();
            }
        }

        public int AssaultProfic
        {
            get
            {
                return PlayerStatsModel.Get("AssaultProfic");
            }
            set
            {
                PlayerStatsModel.Set("AssaultProfic", value);
                OnPropertyChanged();
            }
        }

        public int HandgunsProfic
        {
            get
            {
                return PlayerStatsModel.Get("HandgunsProfic");
            }
            set
            {
                PlayerStatsModel.Set("HandgunsProfic", value);
                OnPropertyChanged();
            }
        }

        public int BladesProfic
        {
            get
            {
                return PlayerStatsModel.Get("BladesProfic");
            }
            set
            {
                PlayerStatsModel.Set("BladesProfic", value);
                OnPropertyChanged();
            }
        }

        public int CraftingProfic
        {
            get
            {
                return PlayerStatsModel.Get("CraftingProfic");
            }
            set
            {
                PlayerStatsModel.Set("CraftingProfic", value);
                OnPropertyChanged();
            }
        }

        public int EngineeringProfic
        {
            get
            {
                return PlayerStatsModel.Get("EngineeringProfic");
            }
            set
            {
                PlayerStatsModel.Set("EngineeringProfic", value);
                OnPropertyChanged();
            }
        }

        public int BreachProtocolProfic
        {
            get
            {
                return PlayerStatsModel.Get("BreachProtocolProfic");
            }
            set
            {
                PlayerStatsModel.Set("BreachProtocolProfic", value);
                OnPropertyChanged();
            }
        }

        public int QuickhackingProfic
        {
            get
            {
                return PlayerStatsModel.Get("QuickhackingProfic");
            }
            set
            {
                PlayerStatsModel.Set("QuickhackingProfic", value);
                OnPropertyChanged();
            }
        }

        public int StealthProfic
        {
            get
            {
                return PlayerStatsModel.Get("StealthProfic");
            }
            set
            {
                PlayerStatsModel.Set("StealthProfic", value);
                OnPropertyChanged();
            }
        }

        public int ColdBloodProfic
        {
            get
            {
                return PlayerStatsModel.Get("ColdBloodProfic");
            }
            set
            {
                PlayerStatsModel.Set("ColdBloodProfic", value);
                OnPropertyChanged();
            }
        }

        public int PerkPoints
        {
            get
            {
                return PlayerStatsModel.Get("PerkPoints");
            }
            set
            {
                PlayerStatsModel.Set("PerkPoints", value);
                OnPropertyChanged();
            }
        }

        public int AttrPoints
        {
            get
            {
                return PlayerStatsModel.Get("AttrPoints");
            }
            set
            {
                PlayerStatsModel.Set("AttrPoints", value);
                OnPropertyChanged();
            }
        }
    }
}
