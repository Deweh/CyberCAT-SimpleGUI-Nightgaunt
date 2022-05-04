using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WolvenKit.RED4.Types;
using CyberCAT.SimpleGUI.Core.Helpers;

namespace CyberCAT.SimpleGUI.MVVM.Model
{
    static class PlayerStatsModel
    {
        private static Dictionary<string, Enum> _bindings = new Dictionary<string, Enum>
        {
            {"BodyAttribute", Enums.gamedataStatType.Strength},
            {"ReflexesAttribute", Enums.gamedataStatType.Reflexes},
            {"TechnicalAbilityAttribute", Enums.gamedataStatType.TechnicalAbility},
            {"IntelligenceAttribute", Enums.gamedataStatType.Intelligence},
            {"CoolAttribute", Enums.gamedataStatType.Cool},
            {"LevelProfic", Enums.gamedataProficiencyType.Level},
            {"StreetCredProfic", Enums.gamedataProficiencyType.StreetCred},
            {"AthleticsProfic", Enums.gamedataProficiencyType.Athletics},
            {"AnnihilationProfic", Enums.gamedataProficiencyType.Demolition},
            {"StreetBrawlerProfic", Enums.gamedataProficiencyType.Brawling},
            {"AssaultProfic", null},
            {"HandgunsProfic", Enums.gamedataProficiencyType.Gunslinger},
            {"BladesProfic", Enums.gamedataProficiencyType.Kenjutsu},
            {"CraftingProfic", Enums.gamedataProficiencyType.Crafting},
            {"EngineeringProfic", Enums.gamedataProficiencyType.Engineering},
            {"BreachProtocolProfic", Enums.gamedataProficiencyType.Hacking},
            {"QuickhackingProfic", Enums.gamedataProficiencyType.CombatHacking},
            {"StealthProfic", Enums.gamedataProficiencyType.Stealth},
            {"ColdBloodProfic", Enums.gamedataProficiencyType.ColdBlood},
            {"PerkPoints", Enums.gamedataDevelopmentPointType.Primary},
            {"AttrPoints", Enums.gamedataDevelopmentPointType.Secondary}
        };

        public delegate void LifePathChangedHandler(string lifePath);
        public static event LifePathChangedHandler LifePathChanged;

        public static int Get(string statName)
        {
            if (!SaveFileHelper.DataAvailable)
            {
                return 0;
            }

            var devSystem = SaveFileHelper.GetPlayerDevelopmentData();

            if (statName.EndsWith("Attribute"))
            {
                var attr = devSystem.Attributes.Where(x => x.AttributeName == (Enums.gamedataStatType)_bindings[statName]).FirstOrDefault();
                return attr != null ? attr.Value : 0;
            }
            else if (statName.EndsWith("Profic"))
            {
                var profic = devSystem.Proficiencies.Where(x => x.Type == (Enums.gamedataProficiencyType?)_bindings[statName]).FirstOrDefault();
                return profic != null ? profic.CurrentLevel : 0;
            }
            else if (statName.EndsWith("Points"))
            {
                var points = devSystem.DevPoints.Where(x => x.Type == (Enums.gamedataDevelopmentPointType?)_bindings[statName]).FirstOrDefault();
                return points != null ? points.Unspent : 0;
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

            var devSystem = SaveFileHelper.GetPlayerDevelopmentData();

            if (statName.EndsWith("Attribute"))
            {
                if (value > 20) value = 20;

                devSystem.Attributes.Where(x => x.AttributeName == (Enums.gamedataStatType)_bindings[statName]).FirstOrDefault().Value = value;
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

                var profic = devSystem.Proficiencies.Where(x => x.Type == (Enums.gamedataProficiencyType?)_bindings[statName]).FirstOrDefault();

                if (profic != null)
                {
                    profic.CurrentLevel = value;
                }
            }
            else if (statName.EndsWith("Points"))
            {
                devSystem.DevPoints.Where(x => x.Type == (Enums.gamedataDevelopmentPointType?)_bindings[statName]).FirstOrDefault().Unspent = value;
            }
        }

        public static string GetLifePath()
        {
            if (!SaveFileHelper.DataAvailable)
            {
                throw new Exception("Unable to get LifePath: data not available.");
            }

            var lifePath = SaveFileHelper.GetPlayerDevelopmentData().LifePath;

            if (lifePath == Enums.gamedataLifePath.Nomad)
            {
                return "Nomad";
            }
            else if (lifePath == Enums.gamedataLifePath.StreetKid)
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

            var devData = SaveFileHelper.GetPlayerDevelopmentData();

            if (lifePath == "Nomad")
            {
                devData.LifePath = Enums.gamedataLifePath.Nomad;
            }
            else if (lifePath == "StreetKid")
            {
                devData.LifePath = Enums.gamedataLifePath.StreetKid;
            }
            else
            {
                devData.LifePath = Enums.gamedataLifePath.Corporate;
            }

            LifePathChanged?.Invoke(lifePath);
        }
    }
}
