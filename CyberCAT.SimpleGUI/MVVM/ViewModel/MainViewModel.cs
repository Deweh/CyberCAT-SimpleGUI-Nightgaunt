using System;
using System.Reflection;
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

        public RelayCommand AppearanceViewCommand { get; set; }

        public RelayCommand InventoryViewCommand { get; set; }

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

        public string Version
        {
            get
            {
                var ver = Assembly.GetExecutingAssembly().GetName().Version;
                return $"v{ver.Major}.{ver.Minor}{ver.Build}";
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

            AppearanceViewCommand = new RelayCommand(o =>
            {
                CurrentView = new AppearanceViewModel();
            });

            InventoryViewCommand = new RelayCommand(o =>
            {
                CurrentView = new InventoryViewModel();
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
