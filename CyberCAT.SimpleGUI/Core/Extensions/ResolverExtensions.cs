using System;
using System.IO;
using System.Linq;
using System.Text;
using System.IO.Compression;
using System.Collections.Generic;
using CyberCAT.Core.Classes;
using CyberCAT.Core.Classes.Interfaces;
using CyberCAT.Core.Classes.Mapping;
using CyberCAT.Extra;

namespace CyberCAT.SimpleGUI.Core.Extensions
{
    public class BinaryResolver : ITweakDbResolver
    {
        private BinaryReader br;
        private TweakDbParser parser = new TweakDbParser();
        public Dictionary<ulong, TdbIdInfo> TdbIdIndex;

        public string GetName(TweakDbId tdbid)
        {
            if (tdbid == null)
            {
                return "<null>";
            }

            return TdbIdIndex.Keys.Contains(tdbid.Raw64) ? TdbIdIndex[tdbid.Raw64].Name : $"Unknown_{tdbid}";
        }

        public string GetGameName(TweakDbId tdbid)
        {
            if (tdbid == null)
            {
                return "<null>";
            }

            if (TdbIdIndex.Keys.Contains(tdbid.Raw64) && TdbIdIndex[tdbid.Raw64].InfoOffset != 0)
            {
                br.BaseStream.Seek(TdbIdIndex[tdbid.Raw64].InfoOffset, SeekOrigin.Begin);
                return br.ReadString();
            }
            else
            {
                return string.Empty;
            }
        }

        public string GetGameDescription(TweakDbId tdbid)
        {
            if (tdbid == null)
            {
                return "<null>";
            }

            if (TdbIdIndex.Keys.Contains(tdbid.Raw64) && TdbIdIndex[tdbid.Raw64].InfoOffset != 0)
            {
                br.BaseStream.Seek(TdbIdIndex[tdbid.Raw64].InfoOffset, SeekOrigin.Begin);
                br.ReadString();
                return br.ReadString();
            }
            else
            {
                return string.Empty;
            }
        }

        public ulong GetHash(string itemName)
        {
            return TdbIdIndex.Where(x => x.Value.Name == itemName).Select(x => x.Key).FirstOrDefault();
        }

        public BinaryResolver(byte[] data)
        {
            SaveFile.ReportProgress(new SaveProgressChangedEventArgs(0, 0, "Item Database"));

            var decompressed = new MemoryStream(Decompress(data));
            br = new BinaryReader(decompressed, Encoding.UTF8);

            if (br.ReadByte() != 0x5 || br.ReadByte() != 0x8) throw new Exception("Corrupted item database.");

            var totalItemsCount = br.ReadUInt32();
            var infoItemsCount = br.ReadUInt32();
            var infoIndex = new List<ulong>();

            TdbIdIndex = new Dictionary<ulong, TdbIdInfo>((int)totalItemsCount);

            for (uint i = 0; i < totalItemsCount; i++)
            {
                var name = br.ReadString();
                var tdbid = parser.GetTweakDBID(name);
                TdbIdIndex.Add(tdbid, new TdbIdInfo() { Name = name, InfoOffset = 0 });
                if (i < infoItemsCount) infoIndex.Add(tdbid);

                SaveFile.ReportProgress(new SaveProgressChangedEventArgs((int)i, (int)totalItemsCount));
            }

            for (uint i = 0; i < infoItemsCount; i++)
            {
                TdbIdIndex[infoIndex[(int)i]].InfoOffset = br.BaseStream.Position;
                br.BaseStream.Seek(br.Read7BitEncodedInt(), SeekOrigin.Current);
                br.BaseStream.Seek(br.Read7BitEncodedInt(), SeekOrigin.Current);
            }
        }

        private static byte[] Decompress(byte[] input)
        {
            using (var source = new MemoryStream(input))
            {
                byte[] lengthBytes = new byte[4];
                source.Read(lengthBytes, 0, 4);

                var length = BitConverter.ToInt32(lengthBytes, 0);
                using (var decompressionStream = new GZipStream(source,
                    CompressionMode.Decompress))
                {
                    var result = new byte[length];
                    decompressionStream.Read(result, 0, length);
                    return result;
                }
            }
        }

        public class TdbIdInfo
        {
            public string Name { get; set; }
            public long InfoOffset { get; set; }
        }
    }
}
