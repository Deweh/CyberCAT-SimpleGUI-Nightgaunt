using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Specialized;
using CyberCAT.SimpleGUI.Core;
using CyberCAT.SimpleGUI.Core.Helpers;
using WolvenKit.RED4.Save;

namespace CyberCAT.SimpleGUI.MVVM.ViewModel
{
    class InventoryViewModel : ObservableObject
    {
        private InventoryHelper.SubInventory _activeInventory;

        public InventoryHelper.SubInventory ActiveInventory
        {
            get
            {
                return _activeInventory;
            }
            set
            {
                _activeInventory = value;
                //_activeInventory.Items.CollectionChanged += (object sender, NotifyCollectionChangedEventArgs e) => OnPropertyChanged("ActiveInventory");
                OnPropertyChanged();
            }
        }

        public InventoryViewModel()
        {
            ActiveInventory = SaveFileHelper.GetInventory(1);
        }
    }
}
