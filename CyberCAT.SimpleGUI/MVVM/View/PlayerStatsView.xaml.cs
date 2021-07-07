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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CyberCAT.SimpleGUI.Core.Helpers;
using CyberCAT.SimpleGUI.MVVM.Model;
using CyberCAT.SimpleGUI.MVVM.ViewModel;

namespace CyberCAT.SimpleGUI.MVVM.View
{
    /// <summary>
    /// Interaction logic for PlayerStatsView.xaml
    /// </summary>
    public partial class PlayerStatsView : UserControl
    {
        private Image selectedLifePath;
        private Dictionary<Image, TextBlock> lifePathUI;
        private Dictionary<string, Image> lifePathBindings;

        public PlayerStatsView()
        {
            InitializeComponent();

            lifePathUI = new Dictionary<Image, TextBlock>
            {
                { nomadSelection, nomadArrow },
                { streetKidSelection, streetKidArrow },
                { corpoSelection, corpoArrow }
            };

            lifePathBindings = new Dictionary<string, Image>
            {
                { "Nomad", nomadSelection },
                { "StreetKid", streetKidSelection },
                { "Corpo", corpoSelection }
            };

            selectedLifePath = nomadSelection;

            nomadSelection.MouseEnter += LifePathSelection_MouseEnter;
            streetKidSelection.MouseEnter += LifePathSelection_MouseEnter;
            corpoSelection.MouseEnter += LifePathSelection_MouseEnter;

            nomadSelection.MouseLeave += LifePathSelection_MouseLeave;
            streetKidSelection.MouseLeave += LifePathSelection_MouseLeave;
            corpoSelection.MouseLeave += LifePathSelection_MouseLeave;

            nomadSelection.MouseDown += LifePathSelection_MouseDown;
            streetKidSelection.MouseDown += LifePathSelection_MouseDown;
            corpoSelection.MouseDown += LifePathSelection_MouseDown;

            SaveFileHelper.LoadComplete += RefreshLifePath;

            RefreshLifePath();
        }

        private void RefreshLifePath()
        {
            var lifePath = PlayerStatsModel.GetLifePath();

            lifePathUI[selectedLifePath].Opacity = 0;
            selectedLifePath = lifePathBindings[lifePath];
            lifePathUI[selectedLifePath].Opacity = 1;
        }

        private void LifePathSelection_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender != selectedLifePath && !SaveFileHelper.IsLoading && !SaveFileHelper.IsSaving)
            {
                lifePathUI[selectedLifePath].Opacity = 0;
                var selection = sender as Image;

                selectedLifePath = selection;
                lifePathUI[selection].Opacity = 1;
                PlayerStatsModel.SetLifePath(lifePathBindings.Where(x => x.Value == selection).FirstOrDefault().Key);
            }
        }

        private void LifePathSelection_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender != selectedLifePath)
            {
                lifePathUI[(Image)sender].Opacity = 0;
            }
        }

        private void LifePathSelection_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender != selectedLifePath)
            {
                lifePathUI[(Image)sender].Opacity = 0.4;
            }
        }

        private void pointsButton_Click(object sender, RoutedEventArgs e)
        {
            if (pointsPopup.Visibility == Visibility.Hidden)
            {
                pointsPopup.Visibility = Visibility.Visible;
                pointsButton.Content = "«";
            }
            else
            {
                pointsPopup.Visibility = Visibility.Hidden;
                pointsButton.Content = "»";
            }
        }

        private void skillsButton_Click(object sender, RoutedEventArgs e)
        {
            if (skillsPopup.Visibility == Visibility.Hidden)
            {
                skillsPopup.Visibility = Visibility.Visible;
                skillsButton.Content = "»";
            }
            else
            {
                skillsPopup.Visibility = Visibility.Hidden;
                skillsButton.Content = "«";
            }
        }

        private void swapLifePath_Click(object sender, RoutedEventArgs e)
        {
            lifePathOverlay.Visibility = Visibility.Visible;

            var anim = new DoubleAnimation
            {
                To = 1,
                Duration = TimeSpan.FromMilliseconds(200)
            };

            lifePathOverlay.BeginAnimation(UIElement.OpacityProperty, anim);
        }

        private void lifePathCloseButton_Click(object sender, RoutedEventArgs e)
        {
            var anim = new DoubleAnimation
            {
                To = 0,
                Duration = TimeSpan.FromMilliseconds(200)
            };

            anim.Completed += (object o, EventArgs e) =>
            {
                lifePathOverlay.Visibility = Visibility.Hidden;
            };

            lifePathOverlay.BeginAnimation(UIElement.OpacityProperty, anim);
        }
    }
}
