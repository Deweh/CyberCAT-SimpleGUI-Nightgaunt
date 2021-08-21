using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using CyberCAT.Core.Classes;
using CyberCAT.Core.Classes.DumpedClasses;
using CyberCAT.Core.Classes.Interfaces;
using CyberCAT.Core.Classes.Mapping;
using CyberCAT.Core.Classes.NodeRepresentations;
using CyberCAT.Core.Classes.Parsers;
using CyberCAT.SimpleGUI.MVVM.Model;
using CyberCAT.SimpleGUI.Core.Extensions;

namespace CyberCAT.SimpleGUI.Core.Helpers
{
    public static class SaveFileHelper
    {
        public static SaveFile ActiveFile { get; private set; }
        public static bool DataAvailable { get; private set; } = false;
        public static bool IsLoading { get; private set; } = false;
        public static bool IsSaving { get; private set; } = false;

        public delegate void LoadCompleteHandler();
        public static event LoadCompleteHandler LoadComplete;

        private static int _currentProgress;
        private static int _maxProgress;
        private static string _currentNode = string.Empty;
        private static BinaryResolver _tdbidResolver;
        private static StringBuilder _statusBuilder = new();
        private static WrongDefaultValueEventArgs _wrongDefaultBuffer;

        private static DispatcherTimer progressTimer = new DispatcherTimer();
        private static List<INodeParser> activeParsers = new()
        {
            new CharacterCustomizationAppearancesParser(),
            new InventoryParser(),
            new ItemDataParser(),
            new FactsDBParser(),
            new FactsTableParser(),
            new GameSessionConfigParser(),
            new ItemDropStorageManagerParser(),
            new ItemDropStorageParser(),
            new ScriptableSystemsContainerParser()
            //new StatsSystemParser(),
            //new PSDataParser()
        };

        static SaveFileHelper()
        {
            progressTimer.Interval = TimeSpan.FromMilliseconds(1);

            progressTimer.Tick += progressTimer_Tick;
            SaveFile.ProgressChanged += SaveFile_ProgressChanged;
            GenericUnknownStructParser.WrongDefaultValue += (object sender, WrongDefaultValueEventArgs e) =>
            {
                e.Ignore = true;
                _wrongDefaultBuffer = e;
            };
        }

        public static async Task LoadFileAsync(string filePath)
        {
            IsLoading = true;
            Exception error = null;

            progressTimer.Start();

            if (_tdbidResolver == null)
            {
                _tdbidResolver = await Task.Run(() => new BinaryResolver(ResourceHelper.ItemsDB));
                NameResolver.TweakDbResolver = _tdbidResolver;
            }

            FactResolver.UseDictionary(ResourceHelper.Facts);

            var bufferFile = new SaveFile(activeParsers);

            try
            {
                await Task.Run(() => bufferFile.Load(new MemoryStream(File.ReadAllBytes(filePath))));
            }
            catch (Exception e)
            {
                error = e;
                MainModel.Status = "Load canceled.";
            }

            progressTimer.Stop();
            IsLoading = false;

            if (_wrongDefaultBuffer != null)
            {
                await MainModel.OpenNotification($"WrongDefaultValue\n\nClass Name: {_wrongDefaultBuffer.ClassName}\nProperty Name: {_wrongDefaultBuffer.PropertyName}\nValue: {_wrongDefaultBuffer.Value}\n\nYou can safely ignore this warning.", "Warning");
                _wrongDefaultBuffer = null;
            }

            if (error != null)
            {
                try
                {
                    File.WriteAllText("error.txt", error.Message + Environment.NewLine + error.StackTrace);
                    await MainModel.OpenNotification("Failed to parse save file: " + error.Message + " An error.txt file has been generated with additional information.", "Error");
                }
                catch (Exception)
                {
                    await MainModel.OpenNotification("Failed to parse save file: " + error.Message + " \n\n Stack Trace: \n" + error.StackTrace, "Error");
                }
                return;
            }

            ActiveFile = bufferFile;
            DataAvailable = true;
            LoadComplete?.Invoke();
            MainModel.Status = "Save file loaded.";
        }

        public static async Task SaveFileAsync(string filePath)
        {
            IsSaving = true;
            progressTimer.Start();
            byte[] newFile = Array.Empty<byte>();
            Exception error = null;

            try
            {
                newFile = await Task.Run(() => ActiveFile.Save());
            }
            catch(Exception e)
            {
                error = e;
                MainModel.Status = "Save canceled.";
            }

            progressTimer.Stop();
            IsSaving = false;

            if (error != null)
            {
                try
                {
                    File.WriteAllText("error.txt", error.Message + Environment.NewLine + error.StackTrace);
                    await MainModel.OpenNotification("Failed to save changes: " + error.Message + " An error.txt file has been generated with additional information.", "Error");
                }
                catch (Exception)
                {
                    await MainModel.OpenNotification("Failed to save changes: " + error.Message + " \n\n Stack Trace: \n" + error.StackTrace, "Error");
                }
                return;
            }

            File.WriteAllBytes(filePath, newFile);
            MainModel.Status = "File saved.";
        }

        private static void SaveFile_ProgressChanged(object sender, SaveProgressChangedEventArgs e)
        {
            if (e.NodeName != string.Empty)
            {
                _currentProgress = 0;
                _maxProgress = 0;
                _currentNode = e.NodeName;
            }
            else if (e.Maximum > 0)
            {
                Interlocked.Exchange(ref _currentProgress, e.CurrentProgress);
                Interlocked.Exchange(ref _maxProgress, e.Maximum);
            }
        }

        private static void progressTimer_Tick(object sender, EventArgs e)
        {
            _statusBuilder.Clear();
            _statusBuilder.Append(IsLoading ? "Parsing" : "Rebuilding");

            if (_currentNode != string.Empty)
            {
                _statusBuilder.Append(" ");
                _statusBuilder.Append(_currentNode);
                if (_maxProgress > 0 && _currentProgress < _maxProgress)
                {
                    _statusBuilder.Append(" (");
                    _statusBuilder.Append(Math.Round((Decimal.Divide(_currentProgress, _maxProgress) * 100), 0).ToString());
                    _statusBuilder.Append("%)");
                }
            }

            _statusBuilder.Append("...");
            MainModel.Status = _statusBuilder.ToString();
        }

        public static NodeEntry GetNode(string nodeName)
        {
            return ActiveFile.Nodes.FirstOrDefault(x => x.Name == nodeName);
        }

        public static CharacterCustomizationAppearances GetAppearanceContainer()
        {
            return GetNode("CharacetrCustomization_Appearances").Value as CharacterCustomizationAppearances;
        }

        public static GenericUnknownStruct GetScriptableContainer()
        {
            return GetNode("ScriptableSystemsContainer").Value as GenericUnknownStruct;
        }

        public static Handle<PlayerDevelopmentData> GetPlayerDevelopmentData()
        {
            var devSystem = GetScriptableContainer().ClassList.FirstOrDefault(x => x is PlayerDevelopmentSystem) as PlayerDevelopmentSystem;
            return devSystem.PlayerData.FirstOrDefault(x => x.Value.OwnerID.Hash == 1);
        }

        public static bool PSDataEnabled()
        {
            return activeParsers.Any(x => x is PSDataParser);
        }

        public static GenericUnknownStruct GetPSDataContainer()
        {
            return (GenericUnknownStruct)GetNode("PersistencySystem").Children.FirstOrDefault(x => x.Name == "PSData").Value;
        }

        public static Inventory GetInventoriesContainer()
        {
            return GetNode("inventory").Value as Inventory;
        }

        public static Inventory.SubInventory GetInventory(ulong id)
        {
            return GetInventoriesContainer().SubInventories.FirstOrDefault(x => x.InventoryId == id);
        }
    }
}
