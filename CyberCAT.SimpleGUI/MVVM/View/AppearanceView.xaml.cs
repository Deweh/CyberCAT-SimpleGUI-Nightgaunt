using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Text.Json;
using CyberCAT.SimpleGUI.MVVM.Model;
using CyberCAT.SimpleGUI.MVVM.ViewModel;
using CyberCAT.SimpleGUI.Core.Helpers;
using WolvenKit.RED4.Save;

namespace CyberCAT.SimpleGUI.MVVM.View
{
    /// <summary>
    /// Interaction logic for AppearanceView.xaml
    /// </summary>
    public partial class AppearanceView : UserControl
    {
        public AppearanceView()
        {
            InitializeComponent();
        }

        private async void loadPreset_Click(object sender, RoutedEventArgs e)
        {
            var openDialog = new OpenFileDialog
            {
                Filter = "Cyberpunk 2077 Character Preset|*.preset"
            };

            if (openDialog.ShowDialog() == true)
            {
                CharacterCustomizationAppearances preset = null;
                try
                {
                    preset = JsonSerializer.Deserialize<CharacterCustomizationAppearances>(File.ReadAllText(openDialog.FileName), ResourceHelper.GetSerializerOptions());
                }
                catch(Exception error)
                {
                    await MainModel.OpenNotification("Failed to parse preset: " + error.Message, "Error");
                    return;
                }

                if (preset.UnknownFirstBytes.Length > 6)
                {
                    preset.UnknownFirstBytes = preset.UnknownFirstBytes.Skip(preset.UnknownFirstBytes.Length - 6).ToArray();
                }

                if (preset.UnknownFirstBytes[4] != SaveFileHelper.GetAppearanceContainer().UnknownFirstBytes[4])
                {
                    AppearanceModel.BodyGender.Set((AppearanceModel.Gender)preset.UnknownFirstBytes[4]);
                }

                AppearanceModel.SetAllValues(preset);
                var viewModel = (AppearanceViewModel)DataContext;

                foreach (var slider in viewModel.Sliders)
                {
                    slider.RefreshValue();
                }

                viewModel.PreviewImage = System.IO.Path.Combine("BodyGender", viewModel.Sliders[0].Value.ToString("00") + ".jpg");
                await MainModel.OpenNotification("Appearance preset applied.");
            }
        }

        private async void savePreset_Click(object sender, RoutedEventArgs e)
        {
            var saveDialog = new SaveFileDialog
            {
                Filter = "Cyberpunk 2077 Character Preset|*.preset"
            };

            if (saveDialog.ShowDialog() == true)
            {
                File.WriteAllText(saveDialog.FileName, JsonSerializer.Serialize(SaveFileHelper.GetAppearanceContainer(), ResourceHelper.GetSerializerOptions()));
                await MainModel.OpenNotification("Appearance preset saved.");
            }
        }
    }
}
