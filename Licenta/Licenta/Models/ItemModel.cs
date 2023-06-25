using Opc.UaFx;
using System.Windows.Input;
using Xamarin.Forms;
using Licenta.ViewModels;
using Newtonsoft.Json;
using System;
using Newtonsoft.Json.Linq;

namespace Licenta.Models
{
    public class ItemModel
    {
        public ItemModel(int index, OpcNodeId id, string name, string description, string namespacee, string image, Command<int> itemTapped, bool hasAddIcon, Command<int> monitorItem, Command<OpcNodeId> deleteFromMonitor, Command<int> editItem, bool history, Command<int> chart)
        {
            Index = index;
            Id = id;
            Name = name;
            Description = description;
            Namespace = namespacee;
            Image = image;
            ItemTapped = itemTapped;
            HasAddIcon = hasAddIcon;
            MonitorItem = monitorItem;
            DeleteFromMonitor = deleteFromMonitor;
            EditItem = editItem;
            HasHistory = history;
            Chart = chart;
        }
        public int Index { get; set; }
        public Command ItemTapped { get; set; }
        public Command MonitorItem { get; set; }
        public Command Chart { get; set; }
        public Command DeleteFromMonitor { get; set; }
        public Command EditItem { get; set; }
        public bool HasHistory { get; set; }
        public string Namespace { get; set; }
        public OpcNodeId Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public bool HasAddIcon { get; set; }
    }
}
