using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using CyberCAT.SimpleGUI.MVVM.Model;
using WolvenKit.RED4.Save;
using WolvenKit.RED4.Save.IO;

namespace CyberCAT.SimpleGUI.Core.Helpers
{
    public static class SaveFileHelper
    {
        public static CyberpunkSaveFile ActiveFile { get; private set; }
        public static bool DataAvailable { get; private set; } = false;
        public static bool IsLoading { get; private set; } = false;
        public static bool IsSaving { get; private set; } = false;

        public delegate void LoadCompleteHandler();
        public static event LoadCompleteHandler LoadComplete;

        private static int _currentProgress;
        private static int _maxProgress;
        public static WolvenKit.Common.Services.TweakDBService tdbService;
        public static WolvenKit.Common.Services.HashService hashService;
        private static string _currentNode = string.Empty;
        private static StringBuilder _statusBuilder = new();

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
            //SaveFile.ProgressChanged += SaveFile_ProgressChanged;
            //GenericUnknownStructParser.WrongDefaultValue += (object sender, WrongDefaultValueEventArgs e) =>
            //{
            //    e.Ignore = true;
            //    _wrongDefaultBuffer = e;
            //};
        }

        public static async Task LoadFileAsync(string filePath)
        {
            IsLoading = true;
            Exception error = null;

            progressTimer.Start();
            _ = MainModel.OpenNotification("", "Loading", NotifyButtons.TaskNone);

            if (tdbService == null)
            {
                await Task.Run(() => tdbService = new());
            }

            

            //foreach (var propName in record.GetDynamicPropertyNames())
            //{
            //    System.Windows.MessageBox.Show(propName);
            //}

            if (hashService == null)
            {
                await Task.Run(() => hashService = new());
            }

            CyberpunkSaveFile bufferFile = null;

            try
            {
                await Task.Run(() =>
                {
                    using var ms = new MemoryStream(File.ReadAllBytes(filePath));
                    var reader = new CyberpunkSaveReader(ms);

                    if (reader.ReadFile(out var save) == EFileReadErrorCodes.NoError)
                    {
                        bufferFile = save;
                    }

                    reader = null;
                });
            }
            catch (Exception e)
            {
                error = e;
                MainModel.Status = "Load canceled.";
            }

            progressTimer.Stop();
            IsLoading = false;

            //if (_wrongDefaultBuffer != null)
            //{
            //    await MainModel.OpenNotification($"WrongDefaultValue\n\nClass Name: {_wrongDefaultBuffer.ClassName}\nProperty Name: {_wrongDefaultBuffer.PropertyName}\nValue: {_wrongDefaultBuffer.Value}\n\nYou can safely ignore this warning.", "Warning");
            //    _wrongDefaultBuffer = null;
            //}

            if (error != null)
            {
                try
                {
                    File.WriteAllText("error.txt", error.Message + Environment.NewLine + error.StackTrace);
                    MainModel.CloseNotification(NotifyResult.OK);
                    await MainModel.OpenNotification("Failed to parse save file: " + error.Message + " An error.txt file has been generated with additional information.", "Error");
                }
                catch (Exception)
                {
                    MainModel.CloseNotification(NotifyResult.OK);
                    await MainModel.OpenNotification("Failed to parse save file: " + error.Message + " \n\n Stack Trace: \n" + error.StackTrace, "Error");
                }
                return;
            }

            ActiveFile = bufferFile;
            DataAvailable = true;
            LoadComplete?.Invoke();
            MainModel.Status = "Save file loaded.";
            MainModel.CloseNotification(NotifyResult.OK);

            GC.Collect();
        }

        public static async Task SaveFileAsync(string filePath)
        {
            IsSaving = true;
            //progressTimer.Start();
            _ = MainModel.OpenNotification("", "Saving", NotifyButtons.TaskNone);
            Exception error = null;

            try
            {

                var writer = new CyberpunkSaveWriter(new MemoryStream());
                var data = await Task.Run(() => writer.WriteFile(ActiveFile));

                File.WriteAllBytes(filePath, data);
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

            GC.Collect();

            MainModel.CloseNotification(NotifyResult.OK);
            _ = MainModel.OpenNotification($"Saved {filePath}");
            
            MainModel.Status = "File saved.";
        }

        //private static void SaveFile_ProgressChanged(object sender, SaveProgressChangedEventArgs e)
        //{
        //    if (e.NodeName != string.Empty)
        //    {
        //        _currentProgress = 0;
        //        _maxProgress = 0;
        //        _currentNode = e.NodeName;
        //    }
        //    else if (e.Maximum > 0)
        //    {
        //        Interlocked.Exchange(ref _currentProgress, e.CurrentProgress);
        //        Interlocked.Exchange(ref _maxProgress, e.Maximum);
        //    }
        //}

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

        public static WolvenKit.RED4.Archive.Buffer.Package04 GetScriptableContainer()
        {
            return (GetNode("ScriptableSystemsContainer").Value as Package).Content as WolvenKit.RED4.Archive.Buffer.Package04;
        }

        public static WolvenKit.RED4.Types.PlayerDevelopmentData GetPlayerDevelopmentData()
        {
            return (GetScriptableContainer().Chunks.FirstOrDefault(x => x is WolvenKit.RED4.Types.PlayerDevelopmentSystem) as WolvenKit.RED4.Types.PlayerDevelopmentSystem).PlayerData.FirstOrDefault(x => x.Chunk.OwnerID.Hash == 1).Chunk;
        }

        public static bool PSDataEnabled()
        {
            return true;
            //return activeParsers.Any(x => x is PSDataParser);
        }

        public static PersistencySystem2 GetPSDataContainer()
        {
            return (PersistencySystem2)GetNode("PersistencySystem2").Value;
        }

        public static Inventory GetInventoriesContainer()
        {
            return GetNode("inventory").Value as Inventory;
        }

        public static InventoryHelper.SubInventory GetInventory(ulong id)
        {
            return GetInventoriesContainer().SubInventories.FirstOrDefault(x => x.InventoryId == id);
        }
    }
}
