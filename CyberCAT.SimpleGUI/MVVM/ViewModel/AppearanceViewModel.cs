using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CyberCAT.SimpleGUI.Core;
using CyberCAT.SimpleGUI.Controls;

namespace CyberCAT.SimpleGUI.MVVM.ViewModel
{
    class AppearanceViewModel : ObservableObject
    {
        private List<AppearanceSliderViewModel> _sliders = new();
        private string _previewImg = string.Empty;

        public string PreviewImage
        {
            get
            {
                return System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "Images", "Appearance", "BodyGender", "00.jpg");
            }
            set
            {
                _previewImg = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "Images", "Appearance", value);
                OnPropertyChanged();
            }
        }

        public List<AppearanceSliderViewModel> Sliders
        {
            get
            {
                return new List<AppearanceSliderViewModel>
                {
                    new AppearanceSliderViewModel
                    {
                        StringCollection = new string[] { "FEMALE", "MALE" },
                        Name = "BodyGender",
                        DataType = DisplayDataType.String
                    },
                    new AppearanceSliderViewModel
                    {
                        Name = "VoiceTone",
                        Value = 6
                    },
                    new AppearanceSliderViewModel
                    {
                        Name = "VoiceTone",
                        Value = 6
                    },
                    new AppearanceSliderViewModel
                    {
                        Name = "VoiceTone",
                        Value = 6
                    },
                    new AppearanceSliderViewModel
                    {
                        Name = "VoiceTone",
                        Value = 6
                    },
                    new AppearanceSliderViewModel
                    {
                        Name = "VoiceTone",
                        Value = 6
                    },
                    new AppearanceSliderViewModel
                    {
                        Name = "VoiceTone",
                        Value = 6
                    }
                };
            }
        }
    }
}
