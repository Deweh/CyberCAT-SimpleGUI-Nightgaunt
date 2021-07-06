using System;
using System.Windows;
using CyberCAT.SimpleGUI.Core;
using CyberCAT.SimpleGUI.MVVM.Model;

namespace CyberCAT.SimpleGUI.MVVM.ViewModel
{
    class MainViewModel : ObservableObject
    {
        private object _currentView;
        private string _status;

        public RelayCommand PlayerStatsViewCommand { get; set; }

        public string StatusMessage
        {
            get
            {
                return _status;
            }
            set
            {
                _status = value;
                OnPropertyChanged();
            }
        }

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

            PlayerStatsViewCommand = new RelayCommand(o =>
            {
                CurrentView = new PlayerStatsViewModel();
            });

            StatusMessage = MainModel.Status;
            MainModel.StatusChanged += OnStatusChanged;
        }

        private void OnStatusChanged(string status)
        {
            StatusMessage = status;
        }
    }
}
