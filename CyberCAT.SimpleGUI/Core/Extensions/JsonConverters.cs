using System.Text.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using WolvenKit.RED4.Types;
using System.Text.Json;

namespace CyberCAT.SimpleGUI.Core.Extensions
{
    public static class JsonConverters
    {
        public class AppearanceResourceConverter : JsonConverter<CResourceReference<appearanceAppearanceResource>>
        {
            public override void Write(Utf8JsonWriter writer, CResourceReference<appearanceAppearanceResource> value, JsonSerializerOptions options)
            {
                writer.WriteStringValue(((ulong)value.DepotPath).ToString());
            }

            public override CResourceReference<appearanceAppearanceResource> Read(ref Utf8JsonReader reader, Type objectType, JsonSerializerOptions options)
            {
                return new CResourceReference<appearanceAppearanceResource>() { DepotPath = reader.GetUInt64() };
            }
        }
    }
}
