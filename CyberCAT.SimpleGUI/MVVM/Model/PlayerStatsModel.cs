using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CyberCAT.Core.DumpedEnums;
using CyberCAT.SimpleGUI.Core.Helpers;

namespace CyberCAT.SimpleGUI.MVVM.Model
{
    static class PlayerStatsModel
    {
        private static Dictionary<string, Enum> _bindings = new Dictionary<string, Enum>
        {
            {"BodyAttribute", gamedataStatType.Strength},
            {"ReflexesAttribute", gamedataStatType.Reflexes},
            {"TechnicalAbilityAttribute", gamedataStatType.TechnicalAbility},
            {"IntelligenceAttribute", gamedataStatType.Intelligence},
            {"CoolAttribute", gamedataStatType.Cool},
            {"LevelProfic", gamedataProficiencyType.Level},
            {"StreetCredProfic", gamedataProficiencyType.StreetCred},
            {"AthleticsProfic", gamedataProficiencyType.Athletics},
            {"AnnihilationProfic", gamedataProficiencyType.Demolition},
            {"StreetBrawlerProfic", gamedataProficiencyType.Brawling},
            {"AssaultProfic", null},
            {"HandgunsProfic", gamedataProficiencyType.Gunslinger},
            {"BladesProfic", gamedataProficiencyType.Kenjutsu},
            {"CraftingProfic", gamedataProficiencyType.Crafting},
            {"EngineeringProfic", gamedataProficiencyType.Engineering},
            {"BreachProtocolProfic", gamedataProficiencyType.Hacking},
            {"QuickhackingProfic", gamedataProficiencyType.CombatHacking},
            {"StealthProfic", gamedataProficiencyType.Stealth},
            {"ColdBloodProfic", gamedataProficiencyType.ColdBlood},
            {"PerkPoints", gamedataDevelopmentPointType.Primary},
            {"AttrPoints", null}
        };

        public delegate void LifePathChangedHandler(string lifePath);
        public static event LifePathChangedHandler LifePathChanged;

        public static int Get(string statName)
        {
            if (!SaveFileHelper.DataAvailable)
            {
                return 0;
            }

            var devSystem = SaveFileHelper.GetPlayerDevelopmentData().Value;

            if (statName.EndsWith("Attribute"))
            {
                return devSystem.Attributes.Where(x => x.AttributeName == (gamedataStatType)_bindings[statName]).FirstOrDefault().Value;
            }
            else if (statName.EndsWith("Profic"))
            {
                return devSystem.Proficiencies.Where(x => x.Type == (gamedataProficiencyType?)_bindings[statName]).FirstOrDefault().CurrentLevel;
            }
            else if (statName.EndsWith("Points"))
            {
                return devSystem.DevPoints.Where(x => x.Type == (gamedataDevelopmentPointType?)_bindings[statName]).FirstOrDefault().Unspent;
            }

            return 0;
        }

        public static void Set(string statName, int value)
        {
            if (!SaveFileHelper.DataAvailable)
            {
                return;
            }

            if (value < 0) value = 0;

            var devSystem = SaveFileHelper.GetPlayerDevelopmentData().Value;

            if (statName.EndsWith("Attribute"))
            {
                if (value > 20) value = 20;

                devSystem.Attributes.Where(x => x.AttributeName == (gamedataStatType)_bindings[statName]).FirstOrDefault().Value = value;
            }
            else if (statName.EndsWith("Profic"))
            {
                if (statName == "LevelProfic" || statName == "StreetCredProfic")
                {
                    if (value > 50) value = 50;
                }
                else
                {
                    if (value > 20) value = 20;
                }

                devSystem.Proficiencies.Where(x => x.Type == (gamedataProficiencyType?)_bindings[statName]).FirstOrDefault().CurrentLevel = value;
            }
            else if (statName.EndsWith("Points"))
            {
                devSystem.DevPoints.Where(x => x.Type == (gamedataDevelopmentPointType?)_bindings[statName]).FirstOrDefault().Unspent = value;
            }
        }

        public static string GetLifePath()
        {
            if (!SaveFileHelper.DataAvailable)
            {
                throw new Exception("Unable to get LifePath: data not available.");
            }

            var lifePath = SaveFileHelper.GetPlayerDevelopmentData().Value.LifePath;

            if (lifePath == gamedataLifePath.Nomad)
            {
                return "Nomad";
            }
            else if (lifePath == gamedataLifePath.StreetKid)
            {
                return "StreetKid";
            }
            else
            {
                return "Corpo";
            }
        }

        public static void SetLifePath(string lifePath)
        {
            if (!SaveFileHelper.DataAvailable)
            {
                return;
            }

            var devData = SaveFileHelper.GetPlayerDevelopmentData().Value;

            if (lifePath == "Nomad")
            {
                devData.LifePath = gamedataLifePath.Nomad;
            }
            else if (lifePath == "StreetKid")
            {
                devData.LifePath = gamedataLifePath.StreetKid;
            }
            else
            {
                devData.LifePath = gamedataLifePath.Corporate;
            }

            LifePathChanged?.Invoke(lifePath);
        }
    }
}
