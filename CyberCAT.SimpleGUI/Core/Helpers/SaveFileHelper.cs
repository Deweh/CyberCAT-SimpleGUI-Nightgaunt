using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using CyberCAT.Core;
using CyberCAT.Core.Classes;
using CyberCAT.Core.Classes.DumpedClasses;
using CyberCAT.Core.Classes.Interfaces;
using CyberCAT.Core.Classes.Mapping;
using CyberCAT.Core.Classes.NodeRepresentations;
using CyberCAT.Core.Classes.Parsers;
using CyberCAT.SimpleGUI.MVVM.Model;

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

        private static int _currentProgress = 0;
        private static int _maxProgress = 0;
        private static string _currentNode = "";

        private static DispatcherTimer progressTimer = new DispatcherTimer();

        static SaveFileHelper()
        {
            progressTimer.Interval = TimeSpan.FromMilliseconds(1);

            progressTimer.Tick += progressTimer_Tick;
            SaveFile.ProgressChanged += SaveFile_ProgressChanged;
        }

        public static async Task LoadFileAsync(string filePath)
        {
            IsLoading = true;
            progressTimer.Start();
            Exception error = null;

            var parsers = await Task.Run(() =>
            {
                return new List<INodeParser>
                {
                    new CharacterCustomizationAppearancesParser(), new InventoryParser(), new ItemDataParser(), new FactsDBParser(),
                    new FactsTableParser(), new GameSessionConfigParser(), new ItemDropStorageManagerParser(), new ItemDropStorageParser(),
                    new StatsSystemParser(), new ScriptableSystemsContainerParser(), new PSDataParser()
                };
            });

            SaveFile bufferFile = new SaveFile(parsers); 

            try
            {
                await Task.Run(() =>
                {
                    bufferFile.Load(new MemoryStream(File.ReadAllBytes(filePath)));
                });
            }
            catch (Exception e)
            {
                error = e;
                MainModel.Status = "Load canceled.";
            }

            progressTimer.Stop();
            IsLoading = false;

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
            byte[] newFile = new byte[0]; Exception error = null;

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
                    MessageBox.Show("Failed to save changes: " + error.Message + " An error.txt file has been generated with additional information.");
                }
                catch (Exception)
                {
                    MessageBox.Show("Failed to save changes: " + error.Message + " \n\n Stack Trace: \n" + error.StackTrace);
                }
                return;
            }

            File.WriteAllBytes(filePath, newFile);
            MainModel.Status = "File saved.";
        }

        private static void SaveFile_ProgressChanged(object sender, CyberCAT.Core.Classes.Mapping.SaveProgressChangedEventArgs e)
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
            StringBuilder status = new StringBuilder();
            status.Append(IsLoading ? "Parsing" : "Rebuilding");

            if (_currentNode != string.Empty)
            {
                status.Append(" ");
                status.Append(_currentNode);
                if (_maxProgress > 0 && _currentProgress < _maxProgress)
                {
                    status.Append(" (");
                    status.Append(Math.Round((Decimal.Divide(_currentProgress, _maxProgress) * 100), 0).ToString());
                    status.Append("%)");
                }
            }

            status.Append("...");
            MainModel.Status = status.ToString();
        }

        public static NodeEntry GetNode(string nodeName)
        {
            return ActiveFile.Nodes.Where(x => x.Name == nodeName).FirstOrDefault();
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
            var devSystem = GetScriptableContainer().ClassList.Where(x => x.GetType().Name == "PlayerDevelopmentSystem").FirstOrDefault() as PlayerDevelopmentSystem;
            return devSystem.PlayerData.Where(x => x.Value.OwnerID.Hash == 1).FirstOrDefault();
        }

        public static GenericUnknownStruct GetPSDataContainer()
        {
            return (GenericUnknownStruct)GetNode("PersistencySystem").Children.Where(x => x.Name == "PSData").FirstOrDefault().Value;
        }
    }
}
