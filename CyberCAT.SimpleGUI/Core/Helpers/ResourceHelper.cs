using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using CyberCAT.Core.Classes.NodeRepresentations;

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

        static ResourceHelper()
        {
            AppearancePaths = LoadJsonResource<string[]>("AppearancePaths.json");
            AppearanceValues = LoadJsonResource<JObject>("AppearanceValues.json");

            FemaleDefault = LoadJsonResource<CharacterCustomizationAppearances>("FemaleDefault.preset");
            MaleDefault = LoadJsonResource<CharacterCustomizationAppearances>("MaleDefault.preset");

            ItemsDB = File.ReadAllBytes(Path.Combine(Directory.GetCurrentDirectory(), "Resources", "ItemsDB.bin"));

            Facts = LoadJsonResource<Dictionary<ulong, string>>("Facts.json");
        }

        private static T LoadJsonResource<T>(string name)
        {
            return JsonConvert.DeserializeObject<T>(File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "Resources", name)));
        }

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
