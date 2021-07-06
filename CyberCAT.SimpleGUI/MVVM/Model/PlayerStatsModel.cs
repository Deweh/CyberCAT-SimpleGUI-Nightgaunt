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
        };

        public static int Get(string statName)
        {
            if (!SaveFileHelper.DataAvailable)
            {
                return 0;
            }

            if (statName.EndsWith("Attribute"))
            {
                return SaveFileHelper.GetPlayerDevelopmentData().Value.Attributes.Where(x => x.AttributeName == (gamedataStatType)_bindings[statName]).FirstOrDefault().Value;
            }
            else if (statName.EndsWith("Profic"))
            {
                return SaveFileHelper.GetPlayerDevelopmentData().Value.Proficiencies.Where(x => x.Type == (gamedataProficiencyType)_bindings[statName]).FirstOrDefault().CurrentLevel;
            }

            return 0;
        }

        public static void Set(string statName, int value)
        {
            if (!SaveFileHelper.DataAvailable)
            {
                return;
            }

            if (statName.EndsWith("Attribute"))
            {
                SaveFileHelper.GetPlayerDevelopmentData().Value.Attributes.Where(x => x.AttributeName == (gamedataStatType)_bindings[statName]).FirstOrDefault().Value = value;
            }
            else if (statName.EndsWith("Profic"))
            {
                SaveFileHelper.GetPlayerDevelopmentData().Value.Proficiencies.Where(x => x.Type == (gamedataProficiencyType)_bindings[statName]).FirstOrDefault().CurrentLevel = value;
            }
        }
    }
}
