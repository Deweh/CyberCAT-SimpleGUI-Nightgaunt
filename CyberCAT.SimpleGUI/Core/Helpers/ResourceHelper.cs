using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using WolvenKit.RED4.Save;
using System.Windows;
using System.Text.Json;
using System.Text.Json.Serialization;
using CyberCAT.SimpleGUI.Core.Extensions;

namespace CyberCAT.SimpleGUI.Core.Helpers
{
    public static class ResourceHelper
    {
        public static readonly string[] AppearancePaths;
        public static readonly JsonElement AppearanceValues;
        public static readonly CharacterCustomizationAppearances FemaleDefault;
        public static readonly CharacterCustomizationAppearances MaleDefault;
        public static readonly byte[] ItemsDB;
        public static readonly Dictionary<ulong, string> Facts;
        public static readonly Dictionary<string, string> ItemClasses;
        public static readonly JsonElement Config;
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

        public static JsonSerializerOptions GetSerializerOptions()
        {
            var res = new JsonSerializerOptions();
            res.Converters.Add(new Extensions.JsonConverters.AppearanceResourceConverter());
            return res;
        }

        private static void LoadJsonResource<T>(string name, ref T output, bool essential = true)
        {
            try
            {
                output = JsonSerializer.Deserialize<T>(File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "Resources", name)), GetSerializerOptions());
            }
            catch(Exception)
            {
                if (essential)
                {
                    MessageBox.Show("Failed to load required resource: " + name + ". This may cause the application to crash during use.", "Warning");
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
                    MessageBox.Show("Failed to load required resource: " + name + ". This may cause the application to crash during use.", "Warning");
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
                GUIScale = ReadSetting("GUI_Scale", 1.0, JsonValueKind.Number);
                CheckForUpdates = ReadSetting("Check_For_Updates", true, JsonValueKind.True, JsonValueKind.False);
                PSDataEnabled = ReadSetting("Enable_PSData", true, JsonValueKind.True, JsonValueKind.False);
            }

            private static T ReadSetting<T>(string name, T defaultValue, JsonValueKind settingType, JsonValueKind? secondaryType = null)
            {
                if (Config.TryGetProperty(name, out var value) && ((value.ValueKind == settingType) || (secondaryType != null && value.ValueKind == secondaryType))) {
                    return JsonSerializer.Deserialize<T>(value.GetRawText());
                }
                return defaultValue;
            }
        }

        /// <summary>
        /// Lookup Lists
        /// </summary>
        public static class LL
        {
            private static JsonElement Values = AppearanceValues;
            public static Dictionary<string, ulong> HairStyles { get; } = Values.GetProperty("HairStyles").ToObject<Dictionary<string, ulong>>();
            public static List<string> HairColors { get; } = Values.GetProperty("HairColors").ToObject<List<string>>();
            public static List<string> SkinTones { get; } = Values.GetProperty("SkinTones").ToObject<List<string>>();
            public static List<ulong> SkinTypes { get; } = Values.GetProperty("SkinTypes").ToObject<List<ulong>>();
            public static List<string> EyeColors { get; } = Values.GetProperty("EyeColors").ToObject<List<string>>();
            public static List<ulong> Eyebrows { get; } = Values.GetProperty("Eyebrows").ToObject<List<ulong>>();
            public static List<ulong> LipMakeups { get; } = Values.GetProperty("LipMakeups").ToObject<List<ulong>>();
            public static List<string> EyebrowColors { get; } = Values.GetProperty("EyebrowColors").ToObject<List<string>>();
            public static List<string> LipMakeupColors { get; } = Values.GetProperty("LipMakeupColors").ToObject<List<string>>();
            public static List<ulong> EyeMakeups { get; } = Values.GetProperty("EyeMakeups").ToObject<List<ulong>>();
            public static List<string> EyeMakeupColors { get; } = Values.GetProperty("EyeMakeupColors").ToObject<List<string>>();
            public static Dictionary<string, List<ulong>> BodyTattoos { get; } = Values.GetProperty("BodyTattoos").ToObject<Dictionary<string, List<ulong>>>();
            public static List<string> Nailss { get; } = new List<string> { "Short", "Long" };
            public static List<ulong> FacialTattoos { get; } = Values.GetProperty("FacialTattoos").ToObject<List<ulong>>();
            public static List<ulong> Piercings { get; } = Values.GetProperty("Piercings").ToObject<List<ulong>>();
            public static List<string> Teeth { get; } = Values.GetProperty("Teeth").ToObject<List<string>>();
            public static List<string> FacialScars { get; } = Values.GetProperty("FacialScars").ToObject<List<string>>();
            public static List<ulong> BodyScars { get; } = Values.GetProperty("BodyScars").ToObject<List<ulong>>();
            public static List<string> PiercingColors { get; } = Values.GetProperty("PiercingColors").ToObject<List<string>>();
            public static List<ulong> CheekMakeups { get; } = Values.GetProperty("CheekMakeups").ToObject<List<ulong>>();
            public static List<string> CheekMakeupColors { get; } = Values.GetProperty("CheekMakeupColors").ToObject<List<string>>();
            public static List<ulong> Blemishes { get; } = Values.GetProperty("Blemishes").ToObject<List<ulong>>();
            public static List<string> Genitals { get; } = Values.GetProperty("Genitals").ToObject<List<string>>();
            public static List<string> PenisSizes { get; } = Values.GetProperty("PenisSizes").ToObject<List<string>>();
            public static List<ulong> PubicHairStyles { get; } = Values.GetProperty("PubicHairStyles").ToObject<List<ulong>>();
            public static List<string> NailColors { get; } = Values.GetProperty("NailColors").ToObject<List<string>>();
            public static List<string> Beards { get; } = Values.GetProperty("Beards").ToObject<List<string>>();
            public static Dictionary<string, List<string>> BeardStyles { get; } = Values.GetProperty("BeardStyles").ToObject<Dictionary<string, List<string>>>();
        }
    }
}
