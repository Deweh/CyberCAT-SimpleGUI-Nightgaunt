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

        private void loadSave_Click(object sender, RoutedEventArgs e)
        {
            var openDialog = new OpenFileDialog
            {
                InitialDirectory = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Saved Games", "CD Projekt Red", "Cyberpunk 2077"),
                Filter = "Cyberpunk 2077 Save File|*.dat"
            };

            if (openDialog.ShowDialog() == true)
            {
                MessageBox.Show("Opened!");
            }

        }
    }
}
