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
using CyberCAT.SimpleGUI.MVVM.Model;
using CyberCAT.SimpleGUI.MVVM.ViewModel;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;

namespace CyberCAT.SimpleGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private NotifyButtons _notifyButtons;

        private DoubleAnimation fadeInAnim = new DoubleAnimation
        {
            To = 1,
            Duration = TimeSpan.FromMilliseconds(200),
            EasingFunction = new CircleEase()
        };

        private DoubleAnimation fadeOutAnim = new DoubleAnimation
        {
            To = 0,
            Duration = TimeSpan.FromMilliseconds(200),
            EasingFunction = new CircleEase()
        };

        public MainWindow()
        {
            InitializeComponent();
            MainModel.NotificationOpened += MainModel_NotificationOpened;
            SaveFileHelper.LoadComplete += SaveFileHelper_LoadComplete;
        }

        private void SaveFileHelper_LoadComplete()
        {
            if (SaveFileHelper.DataAvailable)
            {
                ((MainViewModel)DataContext).PlayerStatsViewCommand.Execute(null);
                playerStats.IsChecked = true;
            }
        }

        private void MainModel_NotificationOpened(string text, string title, NotifyButtons buttons)
        {
            notifyText.Text = text;
            notifyTitle.Text = title;
            _notifyButtons = buttons;

            if (buttons == NotifyButtons.OK)
            {
                notifyButton1.SetValue(Grid.ColumnSpanProperty, 2);
                notifyButton1.Content = "OK";
                notifyButton1.Style = FindResource("BottomButtonTheme") as Style;

                notifyButton2.Visibility = Visibility.Hidden;
            }
            else if (buttons == NotifyButtons.YesNo)
            {
                notifyButton1.SetValue(Grid.ColumnSpanProperty, 1);
                notifyButton1.Content = "Yes";
                notifyButton1.Style = FindResource("LeftBottomButtonTheme") as Style;

                notifyButton2.Visibility = Visibility.Visible;
            }

            var blurAnim = new DoubleAnimation
            {
                To = 15,
                Duration = TimeSpan.FromMilliseconds(200),
                EasingFunction = new CircleEase()
            };

            var scaleAnim = new DoubleAnimation
            {
                From = 0.8,
                To = 1,
                Duration = TimeSpan.FromMilliseconds(200),
                EasingFunction = new CircleEase()
            };

            notifyGrid.Visibility = Visibility.Visible;
            uiGrid.Effect.BeginAnimation(BlurEffect.RadiusProperty, blurAnim);

            notifyBox.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, scaleAnim);
            notifyBox.RenderTransform.BeginAnimation(ScaleTransform.ScaleYProperty, scaleAnim);
            notifyGrid.BeginAnimation(UIElement.OpacityProperty, fadeInAnim);
        }

        private void HideNotifyGrid()
        {
            var blurAnim = new DoubleAnimation
            {
                To = 0,
                Duration = TimeSpan.FromMilliseconds(200),
                EasingFunction = new CircleEase()
            };

            blurAnim.Completed += (object o, EventArgs e) =>
            {
                notifyGrid.Visibility = Visibility.Hidden;
            };

            var scaleAnim = new DoubleAnimation
            {
                To = 0.8,
                Duration = TimeSpan.FromMilliseconds(200),
                EasingFunction = new CircleEase()
            };

            uiGrid.Effect.BeginAnimation(BlurEffect.RadiusProperty, fadeOutAnim);
            notifyBox.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, scaleAnim);
            notifyBox.RenderTransform.BeginAnimation(ScaleTransform.ScaleYProperty, scaleAnim);
            notifyGrid.BeginAnimation(UIElement.OpacityProperty, blurAnim);
        }

        private void notifyButton1_Click(object sender, RoutedEventArgs e)
        {
            if (_notifyButtons == NotifyButtons.OK)
            {
                MainModel.CloseNotification(NotifyResult.OK);
            }
            else if (_notifyButtons == NotifyButtons.YesNo)
            {
                MainModel.CloseNotification(NotifyResult.Yes);
            }
            HideNotifyGrid();
        }

        private void notifyButton2_Click(object sender, RoutedEventArgs e)
        {
            MainModel.CloseNotification(NotifyResult.No);
            HideNotifyGrid();
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

                loadSave.IsEnabled = true;
                if (SaveFileHelper.DataAvailable)
                {
                    sidebar.IsEnabled = true;
                    mainContent.IsEnabled = true;
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
