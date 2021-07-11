using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CyberCAT.SimpleGUI.Core;
using CyberCAT.SimpleGUI.Controls;

namespace CyberCAT.SimpleGUI.MVVM.ViewModel
{
    class AppearanceSliderViewModel : ObservableObject
    {
        private string _name = string.Empty;
        private string _formattedName = string.Empty;
        private int _value = 0;
        private DisplayDataType _dataType = DisplayDataType.Integer;
        private string[] _stringCol = Array.Empty<string>();
        private RelayCommand _onHoverCmd = new RelayCommand((o) => {});

        public Action RefreshValue { get; set; } = () => {};

        public string FormattedName
        {
            get
            {
                return _formattedName;
            }
            private set
            {
                _formattedName = value;
                OnPropertyChanged();
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                OnPropertyChanged();

                FormattedName = Regex.Replace(Name, "([a-z])([A-Z])", "$1 $2").ToUpper();
            }
        }

        public int Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
                OnPropertyChanged();
            }
        }

        public DisplayDataType DataType
        {
            get
            {
                return _dataType;
            }
            set
            {
                _dataType = value;
                OnPropertyChanged();
            }
        }

        public string[] StringCollection
        {
            get
            {
                return _stringCol;
            }
            set
            {
                _stringCol = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand OnHoverCommand
        {
            get
            {
                return _onHoverCmd;
            }
            set
            {
                _onHoverCmd = value;
                OnPropertyChanged();
            }
        }
    }
}
