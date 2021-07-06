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

namespace CyberCAT.SimpleGUI.MVVM.View
{
    /// <summary>
    /// Interaction logic for PlayerStatsView.xaml
    /// </summary>
    public partial class PlayerStatsView : UserControl
    {
        public PlayerStatsView()
        {
            InitializeComponent();
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
    }
}
