using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using CyberCAT.Core.Classes.DumpedClasses;
using CyberCAT.Core.Classes.NodeRepresentations;
using CyberCAT.SimpleGUI.Core.Extensions;
using CyberCAT.SimpleGUI.Core.Helpers;
using static CyberCAT.Core.Classes.NodeRepresentations.CharacterCustomizationAppearances;
using static CyberCAT.SimpleGUI.Core.Helpers.ResourceHelper;

namespace CyberCAT.SimpleGUI.MVVM.Model
{
    public static class AppearanceModel
    {
        #region Properties

        public static AppearanceProperty<Gender> BodyGender => new()
        {
            HasWarning = true,
            Warning = "Changing body gender will reset all appearance options to default.",
            GetSchema = new GenderFromByte(4),
            SetSchema = (Gender value) =>
            {
                if (!SaveFileHelper.PSDataEnabled())
                {
                    return;
                }

                appearanceNode.UnknownFirstBytes[4] = (byte)value;
                var playerPuppet = SaveFileHelper.GetPSDataContainer().ClassList.Where(x => x is PlayerPuppetPS).FirstOrDefault() as PlayerPuppetPS;

                if (value == Gender.Female)
                {
                    playerPuppet.Gender = "Female";
                }
                else
                {
                    playerPuppet.Gender = "Male";
                }
            },
            AfterSet = () =>
            {
                if (!SaveFileHelper.PSDataEnabled())
                {
                    return;
                }

                if (BodyGender.Get() == Gender.Female)
                {
                    SetAllValues(FemaleDefault);
                }
                else
                {
                    SetAllValues(MaleDefault);
                }
            }
        };

        public static AppearanceProperty<Gender> VoiceTone => new()
        {
            GetSchema = new GenderFromByte(5),
            SetSchema = (Gender value) =>
            {
                appearanceNode.UnknownFirstBytes[5] = (byte)value;
            }
        };

        public static AppearanceProperty<int> SkinTone => new()
        {
            MaxValue = LL.SkinTones.Count,
            MinValue = 1,
            GetSchema = new IndexFromList<string>
            (
                LL.SkinTones,
                RetrievalModifier.PlusOne,
                GetConcatedValue,
                "third.main.first.body_color"
            ),
            SetSchema = (int value) =>
            {
                SetConcatedValue("third.main.first.body_color", LL.SkinTones[value - 1], -1, true, LL.SkinTones);
            }
        };

        public static AppearanceProperty<int> SkinType => new()
        {
            MaxValue = LL.SkinTypes.Count,
            MinValue = 1,
            GetSchema = new IndexFromList<ulong>
            (
                LL.SkinTypes,
                RetrievalModifier.PlusOne,
                GetValue<ulong>,
                "first.main.hash.skin_type_"
            ),
            SetSchema = (int value) =>
            {
                SetAllEntries(EntryType.MainListEntry, "skin_type_", (object entry) => { ((HashValueEntry)entry).Hash = LL.SkinTypes[value - 1]; });
            }
        };

        public static AppearanceProperty<int> HairStyle => new()
        {
            MaxValue = LL.HairStyles.Count,
            MinValue = 0,
            StringCollection = LL.HairStyles.Keys.ToArray(),
            GetSchema = new IndexFromList<ulong>
            (
                LL.HairStyles.Values.ToList(),
                RetrievalModifier.None,
                GetValue<ulong>,
                "first.main.hash.hair_color"
            ),
            SetSchema = (int value) =>
            {
                SetNullableHashEntry("hair_color", new HashValueEntry()
                {
                    FirstString = LL.HairColors[0],
                    Hash = LL.HairStyles.Values.ToList()[value],
                    SecondString = "hair_color1"
                },
                new[] { "hairs" });
            }
        };

        public static AppearanceProperty<int> HairColor => new()
        {
            MaxValue = LL.HairColors.Count,
            MinValue = 1,
            GetSchema = new IndexFromList<string>
            (
                LL.HairColors,
                RetrievalModifier.PlusOne,
                GetValue<string>,
                "first.main.first.hair_color"
            ),
            SetSchema = (int value) =>
            {
                SetValue(Field.FirstString, "first.main.hair_color", LL.HairColors[value - 1]);

                if (appearanceNode.Strings.Count < 1)
                {
                    appearanceNode.Strings.Add(LL.HairColors[value - 1].Substring(3));
                    appearanceNode.Strings.Add("Short");
                }
                else
                {
                    appearanceNode.Strings[0] = LL.HairColors[value - 1].Substring(3);
                }
            }
        };

        public static AppearanceProperty<HairLength> HeadwearHairLength => new()
        {
            MaxValue = 2,
            MinValue = 0,
            GetSchema = new CustomGet<HairLength>(() =>
            {
                return Enum.Parse<HairLength>(appearanceNode.Strings[1]);
            }),
            SetSchema = (HairLength value) =>
            {
                appearanceNode.Strings[1] = value.ToString();
            }
        };

        public static AppearanceProperty<int> Eyes => new()
        {
            MaxValue = 21,
            MinValue = 1,
            GetSchema = new FacialValue("eyes"),
            SetSchema = (int value) =>
            {
                SetFacialValue("eyes", 1, value);
            }
        };

        public static AppearanceProperty<int> EyeColor => new()
        {
            MaxValue = LL.EyeColors.Count,
            MinValue = 1,
            GetSchema = new IndexFromList<string>
            (
                LL.EyeColors,
                RetrievalModifier.PlusOne,
                GetConcatedValue,
                "first.main.first.eyes_color"
            ),
            SetSchema = (int value) =>
            {
                SetConcatedValue("first.main.first.eyes_color", LL.EyeColors[value - 1]);
            }
        };

        public static AppearanceProperty<int> Eyebrows => new()
        {
            MaxValue = LL.Eyebrows.Count - 1,
            MinValue = 0,
            GetSchema = new IndexFromList<ulong>
            (
                LL.Eyebrows,
                RetrievalModifier.None,
                GetValue<ulong>,
                "first.main.hash.eyebrows_color"
            ),
            SetSchema = (int value) =>
            {
                SetNullableHashEntry("eyebrows_color", new HashValueEntry()
                {
                    FirstString = $"heb_p{wmGender}a__basehead__01_black",
                    Hash = LL.Eyebrows[value],
                    SecondString = "eyebrows_color1"
                },
                new[] { "TPP", "character_customization" }, Field.Hash, null, true);
            }
        };

        public static AppearanceProperty<int> EyebrowColor => new()
        {
            MaxValue = LL.EyebrowColors.Count,
            MinValue = 1,
            GetSchema = new IndexFromList<string>
            (
                LL.EyebrowColors,
                RetrievalModifier.PlusOne,
                GetConcatedValue,
                "first.main.first.eyebrows_color"
            ),
            SetSchema = (int value) =>
            {
                SetConcatedValue("first.main.first.eyebrows_color", LL.EyebrowColors[value - 1]);
            }
        };

        public static AppearanceProperty<int> Nose => new()
        {
            MaxValue = 21,
            MinValue = 1,
            GetSchema = new FacialValue("nose"),
            SetSchema = (int value) =>
            {
                SetFacialValue("nose", 2, value);
            }
        };

        public static AppearanceProperty<int> Mouth => new()
        {
            MaxValue = 21,
            MinValue = 1,
            GetSchema = new FacialValue("mouth"),
            SetSchema = (int value) =>
            {
                SetFacialValue("mouth", 3, value);
            }
        };

        public static AppearanceProperty<int> Jaw => new()
        {
            MaxValue = 21,
            MinValue = 1,
            GetSchema = new FacialValue("jaw"),
            SetSchema = (int value) =>
            {
                SetFacialValue("jaw", 4, value);
            }
        };

        public static AppearanceProperty<int> Ears => new()
        {
            MaxValue = 21,
            MinValue = 1,
            GetSchema = new FacialValue("ear"),
            SetSchema = (int value) =>
            {
                SetFacialValue("ear", 5, value);
            }
        };

        public static AppearanceProperty<int> Cyberware => new()
        {
            MaxValue = 8,
            MinValue = 0,
            GetSchema = new CustomGet<int>(() =>
            {
                var value = GetConcatedValue("first.main.first.cyberware_", 1);
                return value == "default" ? 0 : int.Parse(value.Split("_").Last());
            }),
            SetSchema = (int value) =>
            {
                SetNullableHashEntry("cyberware_", new HashValueEntry()
                {
                    FirstString = value > 0 ? $"hx_000_p{wmGender}a__cyberware_{value:00}__{LL.SkinTones[SkinTone.Get() - 1]}" : null,
                    Hash = 6513893019731746558,
                    SecondString = "cyberware_" + value.ToString("00")
                },
                new[] { "TPP", "character_customization" }, Field.FirstString, null, true);
            }
        };

        public static AppearanceProperty<int> FacialScars => new()
        {
            MaxValue = LL.FacialScars.Count,
            MinValue = 0,
            GetSchema = new IndexFromList<string>
            (
                LL.FacialScars,
                RetrievalModifier.PlusOne,
                GetConcatedValue,
                "first.main.first.scars",
                -1,
                true
            ),
            SetSchema = (int value) =>
            {
                SetNullableHashEntry("scars", new HashValueEntry
                {
                    FirstString = value > 0 ? $"h0_000_p{wmGender}a__scars_01__{LL.FacialScars[value - 1]}" : null,
                    Hash = 5491315604699331944,
                    SecondString = "scars"
                },
                new[] { "TPP", "character_customization" }, Field.FirstString);
            }
        };

        public static AppearanceProperty<int> FacialTattoos => new()
        {
            MaxValue = LL.FacialTattoos.Count - 1,
            MinValue = 0,
            GetSchema = new IndexFromList<ulong>
            (
                LL.FacialTattoos,
                RetrievalModifier.None,
                GetValue<ulong>,
                "first.main.hash.facial_tattoo_"
            ),
            SetSchema = (int value) =>
            {
                SetNullableHashEntry("facial_tattoo_", new HashValueEntry
                {
                    FirstString =  $"{wmGender}__{LL.SkinTones[SkinTone.Get() - 1]}",
                    Hash = LL.FacialTattoos[value],
                    SecondString = $"facial_tattoo_{value:00}"
                },
                new[] { "TPP", "character_customization" }, Field.Hash, null, false, true);

                SetNullableHashEntry("tattoo", new HashValueEntry
                {
                    FirstString = value == 0 ? null : $"h0_000_p{wmGender}a__tattoo_{value:00}",
                    Hash = 2355758180805363120,
                    SecondString = "tattoo"
                },
                new[] { "TPP", "character_customization" }, Field.FirstString);
            }
        };

        public static AppearanceProperty<int> Piercings => new()
        {
            MaxValue = LL.Piercings.Count - 1,
            MinValue = 0,
            GetSchema = new CustomGet<int>(() =>
            {
                var index = LL.Piercings.FindIndex(x => x == GetValue<ulong>("first.main.hash.piercings_"));
                return index < 0 ? 1 : index;
            }),
            SetSchema = (int value) =>
            {
                SetNullableHashEntry("piercings_", new HashValueEntry
                {
                    FirstString = $"i0_000_p{wmGender}a__earring__07_pearl",
                    Hash = LL.Piercings[value],
                    SecondString = "piercings_01"
                },
                new[] { "TPP", "character_customization" }, Field.Hash);
            }
        };

        public static AppearanceProperty<int> PiercingColor => new()
        {
            MaxValue = LL.PiercingColors.Count,
            MinValue = 1,
            GetSchema = new IndexFromList<string>
            (
                LL.PiercingColors,
                RetrievalModifier.PlusOne,
                GetConcatedValue,
                "first.main.first.piercings_"
            ),
            SetSchema = (int value) =>
            {
                SetConcatedValue("first.main.first.piercings_", LL.PiercingColors[value - 1]);
            }
        };

        public static AppearanceProperty<int> Teeth => new()
        {
            MaxValue = LL.Teeth.Count - 1,
            MinValue = 0,
            GetSchema = new CustomGet<int>(() =>
            {
                var value = GetValue<string>("first.main.first.teeth");
                return value.EndsWith("basehead") ? 0 : LL.Teeth.FindIndex(x => x == $"__{value.Split("__", StringSplitOptions.None).Last()}");
            }),
            SetSchema = (int value) =>
            {
                SetValue(Field.FirstString, "first.main.first.teeth",  $"{(BodyGender.Get() == Gender.Female ? "female" : "male")}_ht_000__basehead{LL.Teeth[value]}");
            }
        };

        public static AppearanceProperty<int> EyeMakeup => new()
        {
            MaxValue = LL.EyeMakeups.Count - 1,
            MinValue = 0,
            GetSchema = new IndexFromList<ulong>
            (
                LL.EyeMakeups,
                RetrievalModifier.None,
                GetValue<ulong>,
                "first.main.hash.makeupEyes_"
            ),
            SetSchema = (int value) =>
            {
                SetNullableHashEntry("makeupEyes_", new HashValueEntry()
                {
                    FirstString = $"hx_000_p{wmGender}a__basehead_makeup_eyes__01_black",
                    Hash = LL.EyeMakeups[value],
                    SecondString = "makeupEyes_01"
                },
                new[] { "TPP", "character_customization" }, Field.Hash, null, true);

                SetAllEntries(EntryType.MainListEntry, "makeupEyes_", (object entry) => { ((HashValueEntry)entry).SecondString = $"makeupEyes_{value:00}"; });
            }
        };

        public static AppearanceProperty<int> EyeMakeupColor => new()
        {
            MaxValue = LL.EyeMakeupColors.Count,
            MinValue = 1,
            GetSchema = new IndexFromList<string>
            (
                LL.EyeMakeupColors,
                RetrievalModifier.PlusOne,
                GetConcatedValue,
                "first.main.first.makeupEyes_"
            ),
            SetSchema = (int value) =>
            {
                SetConcatedValue("first.main.first.makeupEyes_", LL.EyeMakeupColors[value - 1]);
            }
        };

        public static AppearanceProperty<int> LipMakeup => new()
        {
            GetSchema = new IndexFromList<ulong>
            (
                LL.LipMakeups,
                RetrievalModifier.None,
                GetValue<ulong>,
                "first.main.hash.makeupLips_"
            ),
            SetSchema = (int value) =>
            {
                var max = LL.LipMakeups.Count - (BodyGender.Get() == Gender.Male ? 2 : 1);

                if (value > max)
                {
                    value = 0;
                }
                else if (value < 0)
                {
                    value = max;
                }

                SetNullableHashEntry("makeupLips_", new HashValueEntry()
                {
                    FirstString = $"hx_000_p{wmGender}a__basehead__makeup_lips_01__01_black",
                    Hash = LL.LipMakeups[value],
                    SecondString = "makeupLips_01"
                },
                new[] { "TPP", "character_customization" }, Field.Hash, null, true);
            }
        };

        public static AppearanceProperty<int> LipMakeupColor => new()
        {
            MaxValue = LL.LipMakeupColors.Count,
            MinValue = 1,
            GetSchema = new IndexFromList<string>
            (
                LL.LipMakeupColors,
                RetrievalModifier.PlusOne,
                GetConcatedValue,
                "first.main.first.makeupLips_"
            ),
            SetSchema = (int value) =>
            {
                SetConcatedValue("first.main.first.makeupLips_", LL.LipMakeupColors[value - 1]);
            }
        };

        public static AppearanceProperty<int> CheekMakeup => new()
        {
            MaxValue = LL.CheekMakeups.Count - 1,
            MinValue = 0,
            GetSchema = new IndexFromList<ulong>
            (
                LL.CheekMakeups,
                RetrievalModifier.None,
                GetValue<ulong>,
                "first.main.hash.makeupCheeks_"
            ),
            SetSchema = (int value) =>
            {

                if (CheekMakeup.Get() != 5 && value == 5)
                {
                    SetConcatedValue("first.main.first.makeupCheeks_", "02_pink");
                }
                else if (CheekMakeup.Get() == 5 && value < 5 && value > 0)
                {
                    SetConcatedValue("first.main.first.makeupCheeks_", "03_light_brown");
                }

                SetNullableHashEntry("makeupCheeks_", new HashValueEntry
                {
                    FirstString = $"hx_000_p{wmGender}a__morphs_makeup_freckles_01__{(value == 5 ? "02_pink" : "03_light_brown")}",
                    Hash = LL.CheekMakeups[value],
                    SecondString = "makeupCheeks_01"
                },
                new[] { "TPP", "character_customization" });
            }
        };

        public static AppearanceProperty<int> CheekMakeupColor => new()
        {
            GetSchema = new IndexFromList<string>
            (
                LL.CheekMakeupColors,
                RetrievalModifier.PlusOne,
                GetConcatedValue,
                "first.main.first.makeupCheeks_"
            ),
            SetSchema = (int value) =>
            {
                var isBlush = CheekMakeup.Get() == 5;

                if (value > (isBlush ? 7 : 4))
                {
                    value = isBlush ? 5 : 1;
                }
                else if (value < (isBlush ? 5 : 1))
                {
                    value = isBlush ? 7 : 4;
                }

                SetConcatedValue("first.main.first.makeupCheeks_", LL.CheekMakeupColors[value - 1]);
            }
        };

        public static AppearanceProperty<int> Blemishes => new()
        {
            MaxValue = LL.Blemishes.Count - 1,
            MinValue = 0,
            GetSchema = new IndexFromList<ulong>
            (
                LL.Blemishes,
                RetrievalModifier.None,
                GetValue<ulong>,
                "first.main.hash.makeupPimples_"
            ),
            SetSchema = (int value) =>
            {
                SetNullableHashEntry("makeupPimples_", new HashValueEntry
                {
                    FirstString = $"hx_000_p{wmGender}a__basehead_pimples_01__brown_01",
                    Hash = LL.Blemishes[value],
                    SecondString = "makeupPimples_01"
                },
                new[] { "TPP", "character_customization" });
            }
        };

        public static AppearanceProperty<NailLength> Nails => new()
        {
            GetSchema = new CustomGet<NailLength>(() =>
            {
                return GetValue<string>("second.additional.second.nails_l") == "default" ? NailLength.Short : NailLength.Long;
            }),
            SetSchema = (NailLength value) =>
            {
                var entries = GetEntries("second.additional.nails_l"); entries.AddRange(GetEntries("second.additional.nails_r"));
                if (value == NailLength.Short)
                {
                    RemoveEntries(entries);
                }
                else
                {
                    if (entries.Count < 1)
                    {
                        var sectionNames = new[]
                        {
                            "holstered_default",
                            "holstered_nanowire",
                            "unholstered_nanowire",
                            "character_customization",
                            "holstered_launcher",
                            "unholstered_launcher",
                            "holstered_mantis",
                            "unholstered_mantis"
                        };

                        if (BodyGender.Get() == Gender.Female)
                        {
                            sectionNames = new[]
                            {
                                "holstered_default_tpp",
                                "holstered_default_fpp",
                                "holstered_nanowire_tpp",
                                "holstered_nanowire_fpp",
                                "unholstered_nanowire",
                                "character_customization",
                                "holstered_launcher_tpp",
                                "holstered_launcher_fpp",
                                "unholstered_launcher",
                                "holstered_mantis_tpp",
                                "holstered_mantis_fpp",
                                "unholstered_mantis"
                            };
                        }

                        var leftRight = new[] { "l", "r" };

                        foreach (string side in leftRight)
                        {
                            CreateEntry(new ValueEntry()
                            {
                                FirstString = $"nails_{side}",
                                SecondString = $"a0_000_p{wmGender}a_base__nails_{side}{(BodyGender.Get() == Gender.Male ? "_001" : string.Empty)}"
                            }, sectionNames, mainSections[1]);
                        }
                    }
                }
            }
        };

        public static AppearanceProperty<int> NailColor => new()
        {
            MaxValue = LL.NailColors.Count,
            MinValue = 1,
            GetSchema = new CustomGet<int>(() =>
            {
                var value = GetValue<string>($"second.main.first.nails_color{(BodyGender.Get() == Gender.Female ? "_tpp" : string.Empty)}").Substring("a0_000_pwa_base__nails_".Length);
                return LL.NailColors.FindIndex(x => x == value) + 1;
            }),
            SetSchema = (int value) =>
            {
                List<object> entries;
                if (BodyGender.Get() == Gender.Female)
                {
                    entries = GetEntries("second.main.nails_color_tpp");
                    entries.AddRange(GetEntries("second.main.nails_color_fpp"));
                }
                else
                {
                    entries = GetEntries("second.main.nails_color");
                }

                entries.AddRange(GetEntries("second.main.u_launcher_nails_color"));
                entries.AddRange(GetEntries("second.main.u_mantise_nails_color"));

                SetAllEntries(entries, (object entry) =>
                {
                    var entryValue = entry as HashValueEntry;
                    entryValue.FirstString = $"a0_000_p{wmGender}a_{(entryValue.FirstString.Contains("fpp") ? "fpp" : "base")}__nails_{LL.NailColors[value - 1]}";
                });
            }
        };

        public static AppearanceProperty<int> Chest => new()
        {
            MaxValue = 3,
            MinValue = 1,
            GetSchema = new CustomGet<int>(() =>
            {
                if (BodyGender.Get() == Gender.Male)
                {
                    return -1;
                }

                var result = GetConcatedValue("third.additional.second.breast");
                switch (result)
                {
                    case "full_breast_big":
                        return 3;
                    case "full_breast_small":
                        return 1;
                    default:
                        return 2;
                }
            }),
            SetSchema = (int value) =>
            {
                var entries = GetEntries("third.additional.breast");
                if (value == 2)
                {
                    RemoveEntries(entries);
                }
                else
                {
                    var valueString = (value == 1 ? "full_breast_small" : "full_breast_big");

                    if (entries.Count < 1)
                    {
                        CreateEntry(new ValueEntry() { FirstString = "breast", SecondString = string.Empty }, new[] { "breast", "character_creation" }, mainSections[2]);
                        entries = GetEntries("third.additional.breast");
                    }

                    foreach (ValueEntry entry in entries)
                    {
                        entry.SecondString = $"t0_000_wa_base__{valueString}";
                        entry.TrailingBytes[0] = 1;
                        entry.TrailingBytes[4] = 1;
                    }
                }
            }
        };

        public static AppearanceProperty<int> Nipples => new()
        {
            GetSchema = new CustomGet<int>(() =>
            {
                var value = GetConcatedValue("third.main.first.nipples_", 0);
                if (value == "default")
                {
                    return 1;
                }
                else
                {
                    var num = int.Parse(value.Split("_")[2]);
                    return (num == 0 ? 0 : (num + 1));
                }
            }),
            SetSchema = (int value) =>
            {
                if (value > (BodyGender.Get() == Gender.Female ? 3 : 1))
                {
                    value = 0;
                }
                else if (value < 0)
                {
                    value = BodyGender.Get() == Gender.Female ? 3 : 1;
                }

                string first = null;
                if (value != 1)
                {
                    first =  $"{(BodyGender.Get() == Gender.Female ? "female" : "male")}_i0_00{(value == 0 ? 0 : (value - 1))}_base__nipple__{LL.SkinTones[SkinTone.Get() - 1]}";
                }

                SetNullableHashEntry("fpp_nipples_", new HashValueEntry
                {
                    FirstString = first == null ? null : first.Replace("base", "fpp"),
                    Hash = 8383615550749140678,
                    SecondString = "fpp_nipples_01"
                },
                new[] { "FPP_Body" }, Field.FirstString, mainSections[2]);

                SetNullableHashEntry("nipples_", new HashValueEntry
                {
                    FirstString = first,
                    Hash = 17949477145130904651,
                    SecondString = "nipples_01"
                },
                new[] { "TPP_Body", "character_creation" }, Field.FirstString, mainSections[2]);
            }
        };

        public static AppearanceProperty<int> BodyTattoos => new()
        {
            MaxValue = LL.BodyTattoos["TPP"].Count - 1,
            MinValue = 0,
            GetSchema = new IndexFromList<ulong>
            (
                LL.BodyTattoos["TPP"],
                RetrievalModifier.None,
                GetValue<ulong>,
                "third.main.hash.body_tattoo_"
            ),
            SetSchema = (int value) =>
            {
                SetNullableHashEntry("body_tattoo_", new HashValueEntry()
                {
                    FirstString = $"{wmGender}__{LL.SkinTones[SkinTone.Get() - 1]}",
                    Hash = LL.BodyTattoos["TPP"][value],
                    SecondString = "body_tattoo_01"
                },
                new[] { "TPP_Body", "character_creation" }, Field.Hash, mainSections[2]);

                SetNullableHashEntry("fpp_body_tattoo_", new HashValueEntry()
                {
                    FirstString = $"{wmGender}__{LL.SkinTones[SkinTone.Get() - 1]}",
                    Hash = LL.BodyTattoos["FPP"][value],
                    SecondString = "fpp_body_tattoo_01"
                },
                new[] { "FPP_Body" }, Field.Hash, mainSections[2]);
            }
        };

        public static AppearanceProperty<int> BodyScars => new()
        {
            MaxValue = LL.BodyScars.Count - 1,
            MinValue = 0,
            GetSchema = new IndexFromList<ulong>
            (
                LL.BodyScars,
                RetrievalModifier.None,
                GetValue<ulong>,
                "third.main.hash.body_scars_"
            ),
            SetSchema = (int value) =>
            {
                SetNullableHashEntry("body_scars_", new HashValueEntry
                {
                    FirstString = $"scars_p{wmGender}a_001__{LL.SkinTones[SkinTone.Get() - 1]}",
                    Hash = LL.BodyScars[value],
                    SecondString = "body_scars_" + value.ToString("00")
                },
                new[] { "FPP_Body", "TPP_Body", "character_creation" }, Field.Hash, mainSections[2], false, true);
            }
        };

        public static AppearanceProperty<int> Genitals => new()
        {
            MaxValue = LL.Genitals.Count,
            MinValue = 0,
            GetSchema = new IndexFromList<string>
            (
                LL.Genitals,
                RetrievalModifier.PlusOne,
                GetConcatedValue,
                "third.main.first.genitals_",
                1,
                true
            ),
            SetSchema = (int value) =>
            {
                string first = null;
                if (value > 0)
                {
                    first = $"i0_000_p{wmGender}a_base__{LL.Genitals[value - 1]}__{LL.SkinTones[SkinTone.Get() - 1]}";
                }

                if (value < 2)
                {
                    RemoveEntries(GetEntries("third.additional.penis_base"));
                }

                if (Genitals.Get() > 0)
                {
                    var entries = GetEntries($"third.main.{LL.Genitals[Genitals.Get() - 1]}_hairstyle_");
                    if (value < 1)
                    {
                        RemoveEntries(entries);
                    }
                    else
                    {
                        SetAllEntries(entries, (object entry) =>
                        {
                            var entryValue = entry as HashValueEntry;

                            var parts = entryValue.FirstString.Split("__", StringSplitOptions.None);
                            parts[1] = LL.Genitals[value - 1] + "_hairstyle";
                            entryValue.FirstString = string.Join("__", parts);

                            entryValue.SecondString = LL.Genitals[value - 1] + "_hairstyle_01";
                        });
                    }
                }

                SetNullableHashEntry("genitals_", new HashValueEntry
                {
                    FirstString = first,
                    Hash = 3178724759333055970,
                    SecondString = $"genitals_{value:00}"
                },
                new[] { "character_creation", "genitals" }, Field.FirstString, mainSections[2], false, true);
            }
        };

        public static AppearanceProperty<int> PenisSize => new()
        {
            MaxValue = LL.PenisSizes.Count,
            MinValue = 1,
            GetSchema = new CustomGet<int>(() =>
            {
                if (Genitals.Get() < 2)
                {
                    return -1;
                }

                var value = GetValue<string>("third.additional.second.penis_base");
                if (value == "default")
                {
                    return 2;
                }
                else
                {
                    return LL.PenisSizes.FindIndex(x => x == value.Split("__", StringSplitOptions.None)[1]) + 1;
                }
            }),
            SetSchema = (int value) =>
            {
                var entries = GetEntries("third.additional.penis_base");
                if (value == 2)
                {
                    RemoveEntries(entries);
                }
                else
                {
                    var newValue = $"i0_000_p{wmGender}a_base__{LL.PenisSizes[value - 1]}";
                    if (entries.Count < 1)
                    {
                        CreateEntry(new ValueEntry
                        {
                            FirstString = "penis_base",
                            SecondString = newValue
                        },
                        new[] { "character_creation", "genitals" }, mainSections[2]);
                    }
                    else
                    {
                        SetValue(Field.SecondString, "third.additional.penis_base", newValue);
                    }
                }
            }
        };

        public static AppearanceProperty<int> PubicHairStyle => new()
        {
            MaxValue = LL.PubicHairStyles.Count - 1,
            MinValue = 0,
            GetSchema = new CustomGet<int>(() =>
            {
                if (Genitals.Get() < 1)
                {
                    return -1;
                }

                var hash = GetValue<ulong>($"third.main.hash.{LL.Genitals[Genitals.Get() - 1]}_hairstyle_");
                return hash == 0 ? 0 : LL.PubicHairStyles.FindIndex(x => x == hash);
            }),
            SetSchema = (int value) =>
            {
                SetNullableHashEntry($"{LL.Genitals[Genitals.Get() - 1]}_hairstyle_", new HashValueEntry
                {
                    FirstString = $"i0_000_p{wmGender}a_base__{LL.Genitals[Genitals.Get() - 1]}_hairstyle__01_black",
                    Hash = LL.PubicHairStyles[value],
                    SecondString = $"{LL.Genitals[Genitals.Get() - 1]}_hairstyle_01"
                },
                new[] { "character_creation", "genitals" }, Field.Hash, mainSections[2]);
            }
        };

        #endregion

        #region Body

        private static CharacterCustomizationAppearances appearanceNode;
        private static Section[] mainSections;
        private static string wmGender;

        static AppearanceModel()
        {
            if (SaveFileHelper.DataAvailable)
            {
                InitData();
            }

            SaveFileHelper.LoadComplete += InitData;
        }

        #endregion

        #region Methods

        private static void InitData()
        {
            appearanceNode = SaveFileHelper.GetAppearanceContainer();
            mainSections = new[] { appearanceNode.FirstSection, appearanceNode.SecondSection, appearanceNode.ThirdSection };
            wmGender = BodyGender.Get() == Gender.Female ? "w" : "m";
        }

        public static List<object> GetEntries(Section appearanceSection, EntryType _entryType, string searchString)
        {
            var foundEntries = new List<object>();
            if (_entryType == EntryType.MainListEntry)
            {
                foundEntries.AddRange(appearanceSection.AppearanceSections.SelectMany(x => x.MainList).Where(x => CompareMainListAppearanceEntries(x.SecondString, searchString)).ToList());
            }
            else
            {
                foundEntries.AddRange(appearanceSection.AppearanceSections.SelectMany(x => x.AdditionalList).Where(x => x.FirstString == searchString).ToList());
            }
            return foundEntries;
        }

        public static List<object> GetEntries(string searchString)
        {
            var location = StringToLocation(searchString);
            if (location != null)
            {
                return GetEntries(location.Section, location.EntryType, location.SearchString);
            }
            else
            {
                return new List<object>();
            }
        }

        public static void SetAllEntries(EntryType _entryType, string searchString, Action<object> entriesAction)
        {
            var entries = GetAllEntries(_entryType, searchString);
            foreach (object entry in entries)
            {
                entriesAction(entry);
            }
        }

        public static void SetAllEntries(List<object> entries, Action<object> entriesAction)
        {
            foreach (object entry in entries)
            {
                entriesAction(entry);
            }
        }

        public static List<object> GetAllEntries(EntryType _entryType, string searchString)
        {
            var foundEntries = new List<object>();
            foreach (Section section in mainSections)
            {
                foundEntries.AddRange(GetEntries(section, _entryType, searchString));
            }
            return foundEntries;
        }

        public static void EnumerateAllEntries(Action<object> entryAction)
        {
            foreach (Section section in mainSections)
            {
                foreach (AppearanceSection subSection in section.AppearanceSections)
                {
                    foreach (HashValueEntry mainEntry in subSection.MainList)
                    {
                        entryAction(mainEntry);
                    }
                    foreach (ValueEntry additionalEntry in subSection.AdditionalList)
                    {
                        entryAction(additionalEntry);
                    }
                }
            }
        }

        public static void RemoveEntry(object entry)
        {
            foreach (Section section in mainSections)
            {
                foreach (AppearanceSection subSection in section.AppearanceSections)
                {
                    if (entry is HashValueEntry)
                    {
                        if (subSection.MainList.Contains((HashValueEntry)entry))
                        {
                            subSection.MainList.Remove((HashValueEntry)entry);
                        }
                    }
                    else
                    {
                        if (subSection.AdditionalList.Contains((ValueEntry)entry))
                        {
                            subSection.AdditionalList.Remove((ValueEntry)entry);
                        }
                    }
                }
            }
        }

        public static void RemoveEntries(List<object> entries)
        {
            foreach (object entry in entries)
            {
                RemoveEntry(entry);
            }
        }

        public static void CreateEntry(object entry, string[] sectionNames, Section section)
        {
            var subSections = section.AppearanceSections.Where(x => sectionNames.Contains(x.SectionName));
            if (subSections != null)
            {
                foreach (AppearanceSection singleSubSection in subSections)
                {
                    if (entry is HashValueEntry)
                    {
                        singleSubSection.MainList.Add((HashValueEntry)entry);
                    }
                    else
                    {
                        singleSubSection.AdditionalList.Add((ValueEntry)entry);
                    }

                }
            }
        }

        public static void SetValue(Field _field, string searchString, object value)
        {
            var entries = GetEntries(searchString);
            foreach (object entry in entries)
            {
                if (entry is HashValueEntry)
                {
                    switch (_field)
                    {
                        case Field.FirstString:
                            ((HashValueEntry)entry).FirstString = (string)value;
                            break;
                        case Field.Hash:
                            ((HashValueEntry)entry).Hash = (ulong)value;
                            break;
                        case Field.SecondString:
                            ((HashValueEntry)entry).SecondString = (string)value;
                            break;
                    }
                }
                else if (entry is ValueEntry)
                {
                    switch (_field)
                    {
                        case Field.FirstString:
                            ((ValueEntry)entry).FirstString = (string)value;
                            break;
                        case Field.SecondString:
                            ((ValueEntry)entry).SecondString = (string)value;
                            break;
                    }
                }
            }
        }

        public static string GetStringValue(Section appearanceSection, EntryType _entryType, Field fieldToGet, string searchString)
        {
            var entries = GetEntries(appearanceSection, _entryType, searchString);

            if (entries.Count < 1)
            {
                return "default";
            }

            if (entries[0] is HashValueEntry)
            {
                var castedEntry = (HashValueEntry)entries[0];
                if (fieldToGet == Field.FirstString)
                {
                    return castedEntry.FirstString;
                }
                else if (fieldToGet == Field.Hash)
                {
                    return castedEntry.Hash.ToString();
                }
                else
                {
                    return castedEntry.SecondString;
                }
            }
            else
            {
                var castedEntry = (ValueEntry)entries[0];
                if (fieldToGet == Field.FirstString)
                {
                    return castedEntry.FirstString;
                }
                else
                {
                    return castedEntry.SecondString;
                }
            }
        }

        public static ulong GetHashValue(Section appearanceSection, string searchString)
        {
            var entries = GetEntries(appearanceSection, EntryType.MainListEntry, searchString);

            if (entries.Count < 1)
            {
                return 0;
            }

            var castedEntry = (HashValueEntry)entries[0];
            return castedEntry.Hash;
        }

        public static T GetValue<T>(string searchString, int compatibility = 0)
        {
            var location = StringToLocation(searchString);
            if (location != null)
            {
                if (typeof(T) == typeof(string))
                {
                    return (T)Convert.ChangeType(GetStringValue(location.Section, location.EntryType, location.EntryField, location.SearchString), typeof(T));
                }
                else if (typeof(T) == typeof(ulong))
                {
                    return (T)Convert.ChangeType(GetHashValue(location.Section, location.SearchString), typeof(T));
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
            else
            {
                if (typeof(T) == typeof(string))
                {
                    return (T)Convert.ChangeType("default", typeof(T));
                }
                else if (typeof(T) == typeof(ulong))
                {
                    return (T)Convert.ChangeType(0, typeof(T));
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
        }

        public static void SetConcatedValue(string searchString, string newValue, int position = -1, bool wideSearch = false, IEnumerable<string> searchCollection = null)
        {
            if (searchCollection == null)
            {
                searchCollection = new[] { GetValue<string>(searchString).Split("__", StringSplitOptions.None).LastOrIndex(position) };
            }

            EnumerateAllEntries((object entry) =>
            {
                if (entry is HashValueEntry mainEntry)
                {
                    try
                    {
                        if (CompareMainListAppearanceEntries(mainEntry.SecondString, searchString.Split(".").Last()) != true && wideSearch == false)
                        {
                            return;
                        }

                        var valueParts = mainEntry.FirstString.Split("__", StringSplitOptions.None);
                        var targetPart = valueParts.LastOrIndex(position);

                        if (searchCollection.Contains(targetPart))
                        {
                            if (position < 0)
                            {
                                valueParts[valueParts.Length - 1] = newValue;
                            }
                            else
                            {
                                valueParts[position] = newValue;
                            }

                            mainEntry.FirstString = string.Join("__", valueParts);
                        }
                    }
                    catch (Exception)
                    {
                        return;
                    }
                }
            });
        }

        public static string GetConcatedValue(string searchString, int position = -1)
        {
            var result = GetValue<string>(searchString).Split("__", StringSplitOptions.None);
            if (result[0] == "default")
            {
                return "default";
            }

            if (position < 0)
            {
                return result.Last();
            }
            else
            {
                return result[position];
            }
        }

        public static void SetFacialValue(string fieldName, int fieldNum, int value)
        {
            var entries = GetEntries("first.additional." + fieldName);

            if (entries.Count < 1)
            {
                var newEntry = new ValueEntry();
                newEntry.FirstString = fieldName;
                newEntry.SecondString = "h000";

                CreateEntry(newEntry, new[] { "TPP", "character_customization" }, mainSections[0]);
                SetFacialValue(fieldName, fieldNum, value);
            }
            else
            {
                if (value == 1)
                {
                    foreach (ValueEntry entry in entries)
                    {
                        RemoveEntry(entry);
                    }
                }
                else
                {
                    foreach (ValueEntry entry in entries)
                    {
                        var finalValue = value;
                        if (fieldName == "nose" && BodyGender.Get() == Gender.Female)
                        {
                            //Those are some fine spaghetti values ya got there CDPR.
                            if (finalValue > 11 && finalValue < 17)
                            {
                                finalValue++;
                            }
                            else if (finalValue == 17)
                            {
                                finalValue = 12;
                            }
                        }
                        finalValue--;
                        entry.SecondString = "h" + finalValue.ToString("00") + fieldNum.ToString();
                    }
                }
            }
        }

        public static int GetFacialValue(string fieldName)
        {
            var result = GetValue<string>("first.additional.second." + fieldName);
            if (result == "default")
            {
                return 1;
            }
            else
            {
                var finalValue = int.Parse(result.Substring(1, 2)) + 1;
                if (fieldName == "nose" && BodyGender.Get() == Gender.Female)
                {
                    //Spaghetti for days.
                    if (finalValue > 12 && finalValue < 18)
                    {
                        finalValue--;
                    }
                    else if (finalValue == 12)
                    {
                        finalValue = 17;
                    }
                }
                return finalValue;
            }
        }

        public static void SetNullableHashEntry(string searchString, HashValueEntry defaultEntry, string[] sectionNames, Field setValueField = Field.Hash, Section baseMainSection = null, bool createAllMainSections = false, bool allFields = false)
        {
            var entries = GetAllEntries(EntryType.MainListEntry, searchString);
            if (defaultEntry.Hash == 0 || defaultEntry.FirstString == null || defaultEntry.SecondString == null)
            {
                RemoveEntries(entries);
            }
            else
            {
                if (entries.Count < 1)
                {
                    if (createAllMainSections == true)
                    {
                        foreach (Section mainSection in mainSections)
                        {
                            CreateEntry(defaultEntry, sectionNames, mainSection);
                        }
                    }
                    else
                    {
                        if (baseMainSection == null)
                        {
                            baseMainSection = mainSections[0];
                        }
                        CreateEntry(defaultEntry, sectionNames, baseMainSection);
                    }
                }
                else
                {
                    SetAllEntries(entries, (object entry) =>
                    {
                        if (setValueField == Field.FirstString || allFields == true)
                        {
                            ((HashValueEntry)entry).FirstString = defaultEntry.FirstString;
                        }

                        if (setValueField == Field.SecondString || allFields == true)
                        {
                            ((HashValueEntry)entry).SecondString = defaultEntry.SecondString;
                        }

                        if (setValueField == Field.Hash || allFields == true)
                        {
                            ((HashValueEntry)entry).Hash = defaultEntry.Hash;
                        }
                    });
                }
            }
        }

        public static void SetAllValues(CharacterCustomizationAppearances newValues, bool setBodyGender = false)
        {
            var newSections = new[] { newValues.FirstSection, newValues.SecondSection, newValues.ThirdSection };

            var i = 0;
            foreach (Section section in mainSections)
            {
                section.AppearanceSections.Clear();
                foreach (AppearanceSection subSection in newSections[i].AppearanceSections)
                {
                    section.AppearanceSections.Add(subSection);
                }
                i++;
            }

            if (newValues.Strings != null)
            {
                appearanceNode.Strings.Clear();
                foreach (string singleString in newValues.Strings)
                {
                    appearanceNode.Strings.Add(singleString);
                }
            }

            if (newValues.StringTriples != null)
            {
                appearanceNode.StringTriples.Clear();
                foreach (var tripleString in newValues.StringTriples)
                {
                    appearanceNode.StringTriples.Add(tripleString);
                }
            }

            if (newValues.UnknownFirstBytes.Length == 6)
            {
                appearanceNode.UnknownFirstBytes[5] = newValues.UnknownFirstBytes[5];
                if (setBodyGender) appearanceNode.UnknownFirstBytes[4] = newValues.UnknownFirstBytes[4];
            }
        }

        public static EntryLocation StringToLocation(string searchString)
        {
            var searchValues = searchString.Split('.');
            if (searchValues.Length < 3 && searchValues.Length > 4)
            {
                return null;
            }

            var result = new EntryLocation(mainSections[0], EntryType.MainListEntry, (searchValues.Length == 3) ? searchValues[2] : searchValues[3], Field.FirstString);
            if (searchValues[0] == "second")
            {
                result.Section = mainSections[1];
            }
            else if (searchValues[0] == "third")
            {
                result.Section = mainSections[2];
            }

            if (searchValues[1] == "additional")
            {
                result.EntryType = EntryType.AdditionalListEntry;
            }

            if (searchValues.Length == 4)
            {
                if (searchValues[2] == "hash")
                {
                    result.EntryField = Field.Hash;
                }
                else if (searchValues[2] == "second")
                {
                    result.EntryField = Field.SecondString;
                }
            }

            return result;
        }

        public static bool CompareMainListAppearanceEntries(string entry1, string entry2)
        {
            return Regex.Replace(entry1, @"[\d-]", string.Empty) == Regex.Replace(entry2, @"[\d-]", string.Empty);
        }

        #endregion

        #region Interfaces

        public interface RetrievalSchema<T>
        {
            public T Get();
        }

        #endregion

        #region General Classes

        public class EntryLocation
        {
            public Section Section { get; set; }
            public EntryType EntryType { get; set; }
            public Field EntryField { get; set; }
            public string SearchString { get; set; }

            public EntryLocation(Section _sec, EntryType _type, string _searchString, Field _field = Field.FirstString)
            {
                Section = _sec;
                EntryType = _type;
                EntryField = _field;
                SearchString = _searchString;
            }
        }

        public class AppearanceProperty<T>
        {
            public bool HasWarning = false;
            public string Warning;
            public Type ValueType;
            public RetrievalSchema<T> GetSchema;
            public Action<T> SetSchema;
            public Action AfterSet = () => {};
            public int MaxValue = -1;
            public int MinValue = -1;
            public string[] StringCollection = null;

            public AppearanceProperty()
            {
                if (typeof(T).IsEnum)
                {
                    MaxValue = Enum.GetNames(typeof(T)).Length - 1;
                    MinValue = 0;
                }
                else if (StringCollection != null)
                {
                    MaxValue = StringCollection.Length - 1;
                    MinValue = 0;
                }
            }

            public void Set(T value)
            {
                SetSchema(value);
            }

            public void RunAfterSet()
            {
                AfterSet();
            }

            public T Get()
            {
                return GetSchema.Get();
            }

            public void SetInt(int value)
            {
                if (MaxValue > -1 && value > MaxValue)
                {
                    value = MinValue;
                }
                else if (MinValue > -1 && value < MinValue)
                {
                    value = MaxValue;
                }

                if (typeof(T).IsEnum)
                {
                    Set((T)Enum.ToObject(typeof(T), value));
                }
                else
                {
                    Set((T)Convert.ChangeType(value, typeof(T)));
                }
                
            }

            public int GetInt()
            {
                return Convert.ToInt32(Get());
            }
        }

        #endregion

        #region RetrievalSchema Classes

        public class GenderFromByte : RetrievalSchema<Gender>
        {
            private int _byteIndex;

            public GenderFromByte(int byteIndex)
            {
                _byteIndex = byteIndex;
            }

            public Gender Get()
            {
                return (Gender)appearanceNode.UnknownFirstBytes[_byteIndex];
            }
        }

        public class IndexFromList<T> : RetrievalSchema<int>
        {
            private List<T> _lookupList;
            private RetrievalModifier _modifier;
            private Func<string, int, T> _lookupMethod;
            private string _lookupStr;
            private int _lookupInt;
            private bool _overrideDefault;

            public IndexFromList(List<T> lookupList, RetrievalModifier modifier, Func<string, int, T> lookupMethod, string lookupString, int lookupInt = -1, bool overrideDefaultBehavior = false)
            {
                _lookupList = lookupList;
                _modifier = modifier;
                _lookupMethod = lookupMethod;
                _lookupStr = lookupString;
                _lookupInt = lookupInt;
                _overrideDefault = overrideDefaultBehavior;
            }

            public int Get()
            {
                var val = _lookupList.FindIndex(x => EqualityComparer<T>.Default.Equals(x, _lookupMethod(_lookupStr, _lookupInt)));

                if (val < 0 && !_overrideDefault)
                {
                    return val;
                }

                if (_modifier == RetrievalModifier.PlusOne)
                {
                    val++;
                }
                else if (_modifier == RetrievalModifier.MinusOne)
                {
                    val--;
                }

                return val;
            }
        }

        public class FacialValue : RetrievalSchema<int>
        {
            private string _feature;

            public FacialValue(string facialFeature)
            {
                _feature = facialFeature;
            }

            public int Get()
            {
                return GetFacialValue(_feature);
            }
        }

        public class CustomGet<T> : RetrievalSchema<T>
        {
            private Func<T> _get;

            public CustomGet(Func<T> getFunc)
            {
                _get = getFunc;
            }

            public T Get()
            {
                return _get();
            }
        }

        #endregion

        #region Enums

        public enum Gender
        {
            Female,
            Male
        }

        public enum NailLength
        {
            Short,
            Long
        }

        public enum HairLength
        {
            Buzz,
            Short,
            Long
        }

        public enum EntryType
        {
            MainListEntry,
            AdditionalListEntry
        }

        public enum Field
        {
            FirstString,
            Hash,
            SecondString
        }

        public enum RetrievalModifier
        {
            None,
            PlusOne,
            MinusOne
        }

        #endregion
    }
}
