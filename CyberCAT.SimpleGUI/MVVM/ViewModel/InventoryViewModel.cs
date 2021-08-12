using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Specialized;
using CyberCAT.SimpleGUI.Core;
using CyberCAT.SimpleGUI.Core.Helpers;
using CyberCAT.Core.Classes.NodeRepresentations;

namespace CyberCAT.SimpleGUI.MVVM.ViewModel
{
    class InventoryViewModel : ObservableObject
    {
        private Inventory.SubInventory _activeInventory;

        public Inventory.SubInventory ActiveInventory
        {
            get
            {
                return _activeInventory;
            }
            set
            {
                _activeInventory = value;
                _activeInventory.Items.CollectionChanged += (object sender, NotifyCollectionChangedEventArgs e) => OnPropertyChanged("ActiveInventory");
                OnPropertyChanged();
            }
        }

        public InventoryViewModel()
        {
            ActiveInventory = SaveFileHelper.GetInventory(1);
        }
    }
}
