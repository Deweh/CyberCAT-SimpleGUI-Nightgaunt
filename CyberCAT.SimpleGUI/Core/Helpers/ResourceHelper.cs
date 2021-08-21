using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using CyberCAT.Core.Classes.NodeRepresentations;
using System.Windows;

namespace CyberCAT.SimpleGUI.Core.Helpers
{
    public static class ResourceHelper
    {
        public static readonly string[] AppearancePaths;
        public static readonly JObject AppearanceValues;
        public static readonly CharacterCustomizationAppearances FemaleDefault;
        public static readonly CharacterCustomizationAppearances MaleDefault;
        public static readonly byte[] ItemsDB;
        public static readonly Dictionary<ulong, string> Facts;
        public static readonly Dictionary<string, string> ItemClasses;
        public static readonly JObject Config;
        public static readonly Dictionary<string, RGBAColor> Theme;

        static ResourceHelper()
        {
            LoadJsonResource("AppearancePaths.json", ref AppearancePaths);
            LoadJsonResource("AppearanceValues.json", ref AppearanceValues);
            LoadJsonResource("FemaleDefault.preset", ref FemaleDefault);
            LoadJsonResource("MaleDefault.preset", ref MaleDefault);
            LoadBinaryResource("ItemsDB.bin", ref ItemsDB);
            LoadJsonResource("Facts.json", ref Facts);
            LoadJsonResource("ItemClasses.json", ref ItemClasses);
            LoadJsonResource("Config\\Config.json", ref Config, false);
            LoadJsonResource("Config\\Theme.json", ref Theme, false);
        }

        private static void LoadJsonResource<T>(string name, ref T output, bool essential = true)
        {
            try
            {
                output = JsonConvert.DeserializeObject<T>(File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "Resources", name)));
            }
            catch(Exception)
            {
                if (essential)
                {
                    MessageBox.Show("Failed to load required resource: " + name, "Critical Error");
                    Application.Current.Shutdown();
                }
            }
        }

        private static void LoadBinaryResource(string name, ref byte[] output, bool essential = true)
        {
            try
            {
                output = File.ReadAllBytes(Path.Combine(Directory.GetCurrentDirectory(), "Resources", name));
            }
            catch (Exception)
            {
                if (essential)
                {
                    MessageBox.Show("Failed to load required resource: " + name, "Critical Error");
                    Application.Current.Shutdown();
                }
            }
        }

        public struct RGBAColor
        {
            public int R;
            public int G;
            public int B;
            public int A;
        }

        /// <summary>
        /// User Configuration
        /// </summary>
        public static class Settings
        {
            public static readonly double GUIScale;
            public static readonly bool CheckForUpdates;
            public static readonly bool PSDataEnabled;

            static Settings()
            {
                GUIScale = ReadSetting("GUI_Scale", 1.0, JTokenType.Float, JTokenType.Integer);
                CheckForUpdates = ReadSetting("Check_For_Updates", true, JTokenType.Boolean);
                PSDataEnabled = ReadSetting("Enable_PSData", true, JTokenType.Boolean);
            }

            private static T ReadSetting<T>(string name, T defaultValue, JTokenType settingType, JTokenType? secondaryType = null)
            {
                if (Config.TryGetValue(name, StringComparison.OrdinalIgnoreCase, out JToken value) && (value.Type == settingType || (secondaryType != null && value.Type == secondaryType)))
                {
                    return value.ToObject<T>();
                }
                return defaultValue;
            }
        }

        /// <summary>
        /// Lookup Lists
        /// </summary>
        public static class LL
        {
            private static JObject Values = AppearanceValues;
            public static Dictionary<string, ulong> HairStyles { get; } = Values["HairStyles"].ToObject<Dictionary<string, ulong>>();
            public static List<string> HairColors { get; } = Values["HairColors"].ToObject<List<string>>();
            public static List<string> SkinTones { get; } = Values["SkinTones"].ToObject<List<string>>();
            public static List<ulong> SkinTypes { get; } = Values["SkinTypes"].ToObject<List<ulong>>();
            public static List<string> EyeColors { get; } = Values["EyeColors"].ToObject<List<string>>();
            public static List<ulong> Eyebrows { get; } = Values["Eyebrows"].ToObject<List<ulong>>();
            public static List<ulong> LipMakeups { get; } = Values["LipMakeups"].ToObject<List<ulong>>();
            public static List<string> EyebrowColors { get; } = Values["EyebrowColors"].ToObject<List<string>>();
            public static List<string> LipMakeupColors { get; } = Values["LipMakeupColors"].ToObject<List<string>>();
            public static List<ulong> EyeMakeups { get; } = Values["EyeMakeups"].ToObject<List<ulong>>();
            public static List<string> EyeMakeupColors { get; } = Values["EyeMakeupColors"].ToObject<List<string>>();
            public static Dictionary<string, List<ulong>> BodyTattoos { get; } = Values["BodyTattoos"].ToObject<Dictionary<string, List<ulong>>>();
            public static List<string> Nailss { get; } = new List<string> { "Short", "Long" };
            public static List<ulong> FacialTattoos { get; } = Values["FacialTattoos"].ToObject<List<ulong>>();
            public static List<ulong> Piercings { get; } = Values["Piercings"].ToObject<List<ulong>>();
            public static List<string> Teeth { get; } = Values["Teeth"].ToObject<List<string>>();
            public static List<string> FacialScars { get; } = Values["FacialScars"].ToObject<List<string>>();
            public static List<ulong> BodyScars { get; } = Values["BodyScars"].ToObject<List<ulong>>();
            public static List<string> PiercingColors { get; } = Values["PiercingColors"].ToObject<List<string>>();
            public static List<ulong> CheekMakeups { get; } = Values["CheekMakeups"].ToObject<List<ulong>>();
            public static List<string> CheekMakeupColors { get; } = Values["CheekMakeupColors"].ToObject<List<string>>();
            public static List<ulong> Blemishes { get; } = Values["Blemishes"].ToObject<List<ulong>>();
            public static List<string> Genitals { get; } = Values["Genitals"].ToObject<List<string>>();
            public static List<string> PenisSizes { get; } = Values["PenisSizes"].ToObject<List<string>>();
            public static List<ulong> PubicHairStyles { get; } = Values["PubicHairStyles"].ToObject<List<ulong>>();
            public static List<string> NailColors { get; } = Values["NailColors"].ToObject<List<string>>();
            public static List<string> Beards { get; } = Values["Beards"].ToObject<List<string>>();
            public static Dictionary<string, List<string>> BeardStyles { get; } = Values["BeardStyles"].ToObject<Dictionary<string, List<string>>>();
        }
    }
}
