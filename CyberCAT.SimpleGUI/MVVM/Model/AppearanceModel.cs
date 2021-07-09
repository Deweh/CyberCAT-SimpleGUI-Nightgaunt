using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CyberCAT.Core.Classes.NodeRepresentations;
using static CyberCAT.Core.Classes.NodeRepresentations.CharacterCustomizationAppearances;
using CyberCAT.SimpleGUI.Core.Helpers;
using System.Text.RegularExpressions;

namespace CyberCAT.SimpleGUI.MVVM.Model
{
    public static class AppearanceModel
    {
        #region Properties

        public static readonly List<AppearanceProperty> Properties = new()
        {
            new()
            {
                Name = "BodyGender",
                Warning = "Changing body gender will reset all appearance options to default.",
                HasWarning = true,
                Set = (int value) =>
                {

                }
            }
        };

        #endregion

        private static CharacterCustomizationAppearances appearanceNode;
        private static Section[] mainSections;

        static AppearanceModel()
        {
            if (SaveFileHelper.DataAvailable)
            {
                InitData();
            }

            SaveFileHelper.LoadComplete += InitData;
        }

        #region Methods

        private static void InitData()
        {
            appearanceNode = SaveFileHelper.GetAppearanceContainer();
            mainSections = new[] { appearanceNode.FirstSection, appearanceNode.SecondSection, appearanceNode.ThirdSection };
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

        public interface RetrievalSchema
        {
            public int Get();
        }

        #endregion

        #region General Classes

        public class EntryLocation
        {
            public CharacterCustomizationAppearances.Section Section { get; set; }
            public EntryType EntryType { get; set; }
            public Field EntryField { get; set; }
            public string SearchString { get; set; }

            public EntryLocation(CharacterCustomizationAppearances.Section _sec, EntryType _type, string _searchString, Field _field = Field.FirstString)
            {
                Section = _sec;
                EntryType = _type;
                EntryField = _field;
                SearchString = _searchString;
            }
        }

        public class AppearanceProperty
        {
            public string Name;
            public bool HasWarning = false;
            public string Warning;
            public Type ValueType = typeof(int);
            public RetrievalSchema GetSchema;
            public Action<int> Set;

            public int Get()
            {
                return GetSchema.Get();
            }
        }

        #endregion

        #region RetrievalSchema Classes

        public class IndexFromList : RetrievalSchema
        {
            private object[] _lookupList;
            private string _searchString;
            private RetrievalModifier _modifier;
            private Func<string, dynamic> _retrievalMethod;

            public IndexFromList(object[] lookupList, string searchString, RetrievalModifier modifier, Func<string, dynamic> retrievalMethod)
            {
                _lookupList = lookupList;
                _searchString = searchString;
                _modifier = modifier;
                _retrievalMethod = retrievalMethod;
            }

            public int Get()
            {
                var val = Array.FindIndex(_lookupList, x => x == _retrievalMethod(_searchString));

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

        #endregion

        #region Enums

        public enum Gender
        {
            Female,
            Male
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
