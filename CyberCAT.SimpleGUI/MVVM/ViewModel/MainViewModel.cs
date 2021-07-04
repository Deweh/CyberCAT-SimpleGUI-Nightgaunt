using System;
using CyberCAT.SimpleGUI.Core;

namespace CyberCAT.SimpleGUI.MVVM.ViewModel
{
    class MainViewModel : ObservableObject
    {
        private object _currentView;

        public RelayCommand PlayerStatsViewCommand { get; set; }

        public PlayerStatsViewModel PlayerStatsVM { get; set; }

        public object CurrentView
        {
            get
            {
                return _currentView;
            }
            set
            {
                _currentView = value;
                OnPropertyChanged();
            }
        }

        public MainViewModel()
        {
            PlayerStatsVM = new PlayerStatsViewModel();

            PlayerStatsViewCommand = new RelayCommand(o =>
            {
                CurrentView = PlayerStatsVM;
            });
        }
    }
}
