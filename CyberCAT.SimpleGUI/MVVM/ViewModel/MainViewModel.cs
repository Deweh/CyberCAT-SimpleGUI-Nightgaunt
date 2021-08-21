using System;
using System.Reflection;
using System.Windows;
using CyberCAT.SimpleGUI.Core;
using CyberCAT.SimpleGUI.Core.Helpers;
using CyberCAT.SimpleGUI.MVVM.Model;

namespace CyberCAT.SimpleGUI.MVVM.ViewModel
{
    class MainViewModel : ObservableObject
    {
        private object _currentView;
        private string _status;

        public RelayCommand PlayerStatsViewCommand => new(o =>
        {
            CurrentView = new PlayerStatsViewModel();
        });

        public RelayCommand AppearanceViewCommand => new(o =>
        {
            CurrentView = new AppearanceViewModel();
        });

        public RelayCommand InventoryViewCommand => new(o =>
        {
            CurrentView = new InventoryViewModel();
        });

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

        public double GUIScale => ResourceHelper.Settings.GUIScale;

        public MainViewModel()
        {
            StatusMessage = MainModel.Status;
            MainModel.StatusChanged += OnStatusChanged;
        }

        private void OnStatusChanged(string status)
        {
            StatusMessage = status;
        }
    }
}
