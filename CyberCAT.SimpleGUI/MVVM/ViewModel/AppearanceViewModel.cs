using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using CyberCAT.SimpleGUI.Core;
using CyberCAT.SimpleGUI.Controls;
using CyberCAT.SimpleGUI.MVVM.Model;
using System.ComponentModel;
using System.Reflection;

namespace CyberCAT.SimpleGUI.MVVM.ViewModel
{
    class AppearanceViewModel : ObservableObject
    {
        private List<AppearanceSliderViewModel> _sliders = new();
        private string _previewImg = string.Empty;

        public AppearanceViewModel()
        {
            var properties = typeof(AppearanceModel).GetProperties();

            foreach(var prop in properties)
            {
                var slider = new AppearanceSliderViewModel { Name = prop.Name };
                var propVal = prop.GetValue(prop);

                if (prop.PropertyType.GetGenericArguments()[0].IsEnum)
                {
                    slider.DataType = DisplayDataType.String;
                    slider.StringCollection = Enum.GetNames(prop.PropertyType.GetGenericArguments()[0]).Select(x => x.ToUpper()).ToArray();
                }

                var stringCol = prop.PropertyType.GetField("StringCollection").GetValue(propVal) as string[];

                if (stringCol != null)
                {
                    slider.DataType = DisplayDataType.String;
                    slider.StringCollection = stringCol.Select(x => x.ToUpper()).ToArray();
                }

                slider.OnHoverCommand = new((param) =>
                {
                    if (!(bool)param)
                    {
                        return;
                    }

                    if (File.Exists(GetPreviewPath(Path.Combine(prop.Name, AppearanceModel.BodyGender.Get().ToString() + slider.Value.ToString("00") + ".jpg"))))
                    {
                        PreviewImage = Path.Combine(prop.Name, AppearanceModel.BodyGender.Get().ToString() + slider.Value.ToString("00") + ".jpg");
                    }
                    else if (File.Exists(GetPreviewPath(Path.Combine(prop.Name, slider.Value.ToString("00") + ".jpg"))))
                    {
                        PreviewImage = Path.Combine(prop.Name, slider.Value.ToString("00") + ".jpg");
                    }
                });

                slider.RefreshValue = () =>
                {
                    try
                    {
                        slider.Value = (int)prop.PropertyType.GetMethod("GetInt").Invoke(propVal, null);
                    }
                    catch (Exception)
                    {
                        slider.Value = -1;
                    }
                    
                    slider.Enabled = slider.Value > -1;
                };

                slider.RefreshValue();

                slider.PropertyChanged += async (object sender, PropertyChangedEventArgs e) =>
                {
                    if (e.PropertyName == "Value")
                    {
                        var currentValue = 0;

                        try
                        {
                            currentValue = (int)prop.PropertyType.GetMethod("GetInt").Invoke(propVal, null);
                        }
                        catch (Exception)
                        {
                            return;
                        }

                        if (slider.Value != currentValue)
                        {
                            if ((bool)prop.PropertyType.GetField("HasWarning").GetValue(propVal) == true)
                            {
                                if (await MainModel.OpenNotification(((string)prop.PropertyType.GetField("Warning").GetValue(propVal)) + " Do you wish to continue?", "Warning", NotifyButtons.YesNo) == NotifyResult.No)
                                {
                                    slider.RefreshValue();
                                    return;
                                }
                            }

                            prop.PropertyType.GetMethod("SetInt").Invoke(propVal, new object[] { slider.Value });
                            prop.PropertyType.GetMethod("RunAfterSet").Invoke(propVal, null);

                            foreach (var singleSlider in Sliders)
                            {
                                singleSlider.RefreshValue();
                            }
                        }

                        slider.OnHoverCommand.Execute(true);
                    }
                };

                Sliders.Add(slider);
            }

            PreviewImage = Path.Combine("BodyGender", Sliders[0].Value.ToString("00") + ".jpg");
        }

        private string GetPreviewPath(string value)
        {
            return Path.Combine(Directory.GetCurrentDirectory(), "Images", "Appearance", value);
        }

        public string PreviewImage
        {
            get
            {
                return _previewImg;
            }
            set
            {
                _previewImg = GetPreviewPath(value);
                OnPropertyChanged();
            }
        }

        public List<AppearanceSliderViewModel> Sliders
        {
            get
            {
                return _sliders;
            }
            set
            {
                _sliders = value;
                OnPropertyChanged();
            }
        }
    }
}
