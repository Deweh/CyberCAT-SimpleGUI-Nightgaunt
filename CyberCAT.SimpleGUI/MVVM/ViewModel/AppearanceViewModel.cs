using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CyberCAT.SimpleGUI.Core;

namespace CyberCAT.SimpleGUI.MVVM.ViewModel
{
    class AppearanceViewModel : ObservableObject
    {

        public string PreviewImage
        {
            get
            {
                return System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "Images", "Appearance", "BodyGender", "00.jpg");
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
                        Name = "BodyGender",
                        Value = 22
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
