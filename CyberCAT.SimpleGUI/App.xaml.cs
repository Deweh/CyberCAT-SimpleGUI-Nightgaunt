using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using CyberCAT.SimpleGUI.Core.Helpers;

namespace CyberCAT.SimpleGUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            //Generate default theme. (Only uncomment for one-time use.)

            //var dict = new Dictionary<string, ResourceHelper.RGBAColor>();

            //foreach (var mergedDict in Current.Resources.MergedDictionaries)
            //{
            //    foreach (var name in mergedDict.Keys)
            //    {
            //        if (mergedDict[name] is SolidColorBrush brushVal)
            //        {
            //            dict.Add(name.ToString(), new ResourceHelper.RGBAColor() { R = brushVal.Color.R, G = brushVal.Color.G, B = brushVal.Color.B, A = brushVal.Color.A });
            //        }
            //        else if (mergedDict[name] is Color clrVal)
            //        {
            //            dict.Add(name.ToString(), new ResourceHelper.RGBAColor() { R = clrVal.R, G = clrVal.G, B = clrVal.B, A = clrVal.A });
            //        }
            //    }
            //}

            //System.IO.File.WriteAllText("Theme.json", Newtonsoft.Json.JsonConvert.SerializeObject(dict, Newtonsoft.Json.Formatting.Indented));

            if (ResourceHelper.Theme != null)
            {
                foreach (var mergedDict in Current.Resources.MergedDictionaries)
                {
                    foreach (var name in mergedDict.Keys)
                    {
                        if (ResourceHelper.Theme.ContainsKey(name.ToString()))
                        {
                            var clrTemp = ResourceHelper.Theme[name.ToString()];

                            if (clrTemp.R > 255 || clrTemp.R < 0 || clrTemp.G > 255 || clrTemp.G < 0 || clrTemp.B > 255 || clrTemp.B < 0 || clrTemp.A > 255 || clrTemp.A < 0)
                            {
                                continue;
                            }

                            var clr = Color.FromArgb((byte)clrTemp.A, (byte)clrTemp.R, (byte)clrTemp.G, (byte)clrTemp.B);

                            if (mergedDict[name] is SolidColorBrush)
                            {
                                mergedDict[name] = new SolidColorBrush(clr);
                            }
                            else if (mergedDict[name] is Color)
                            {
                                mergedDict[name] = clr;
                            }
                        }
                    }
                }
            }
        }
    }
}
