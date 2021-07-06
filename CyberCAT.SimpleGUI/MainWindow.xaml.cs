using System;
using System.Collections.Generic;
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
using Microsoft.Win32;
using CyberCAT.SimpleGUI.Core.Helpers;
using CyberCAT.SimpleGUI.MVVM.ViewModel;

namespace CyberCAT.SimpleGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void loadSave_Click(object sender, RoutedEventArgs e)
        {
            var openDialog = new OpenFileDialog
            {
                InitialDirectory = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Saved Games", "CD Projekt Red", "Cyberpunk 2077"),
                Filter = "Cyberpunk 2077 Save File|*.dat"
            };

            if (openDialog.ShowDialog() == true)
            {
                loadSave.IsEnabled = false;
                sidebar.IsEnabled = false;
                mainContent.IsEnabled = false;

                await SaveFileHelper.LoadFileAsync(openDialog.FileName);

                if (SaveFileHelper.DataAvailable)
                {
                    loadSave.IsEnabled = true;
                    sidebar.IsEnabled = true;
                    mainContent.IsEnabled = true;
                    playerStats.IsChecked = true;
                    ((MainViewModel)DataContext).PlayerStatsViewCommand.Execute(null);
                }
            }
        }

        private async void saveChanges_Click(object sender, RoutedEventArgs e)
        {
            var saveDialog = new SaveFileDialog
            {
                InitialDirectory = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Saved Games", "CD Projekt Red", "Cyberpunk 2077"),
                Filter = "Cyberpunk 2077 Save File|*.dat"
            };

            if (saveDialog.ShowDialog() == true)
            {
                loadSave.IsEnabled = false;
                sidebar.IsEnabled = false;
                mainContent.IsEnabled = false;

                await SaveFileHelper.SaveFileAsync(saveDialog.FileName);

                loadSave.IsEnabled = true;
                sidebar.IsEnabled = true;
                mainContent.IsEnabled = true;
            }
        }
    }
}
