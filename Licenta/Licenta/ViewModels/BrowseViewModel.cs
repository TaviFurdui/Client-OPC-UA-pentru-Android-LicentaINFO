using Licenta.Models;
using Licenta.Views;
using MvvmHelpers;
using Opc.UaFx.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;
using Opc.UaFx;
using Xamarin.Essentials;
using System.Linq;
using System.Text.Json;
using Newtonsoft.Json;
using Microcharts;
using SkiaSharp;
using System.Globalization;

namespace Licenta.ViewModels
{
    public class BrowseViewModel : BaseViewModel
    {
        public ObservableRangeCollection<ItemModel> BrowseItem { get; set; }
        public ICommand ItemTapped { get; set; }
        public ICommand Back { get; set; }

        public bool isTapped;
        public string currentItem;
        public bool IsTapped { get => isTapped; set => SetProperty(ref isTapped, value); }
        public string CurrentItem { get => currentItem; set => SetProperty(ref currentItem, value); }
        public OpcNodeId CurrentId { get; set; }
        public OpcNodeId ParentNodeId { get; set; }
        public OpcNodeId GrandParentNodeId { get; set; }
        public BrowseViewModel()
        {
            Back = new Command(OnBack);
            IsTapped = false;
            BrowseItem = new ObservableRangeCollection<ItemModel>();
            AddItem(0, "", OpcObjectTypes.ObjectsFolder, "Objects", string.Empty, "Folder.png", new Command<int>(OnItemTapped));
            AddItem(1, "", OpcObjectTypes.TypesFolder, "Types", string.Empty, "Folder.png", new Command<int>(OnItemTapped));
            AddItem(2, "", OpcObjectTypes.ViewsFolder, "Views", string.Empty, "Folder.png", new Command<int>(OnItemTapped));
        }
        private void AddItem(int index, string nameSpace, OpcNodeId id, string name, string description, string image, Command<int> onItemTapped)
        {
            BrowseItem.Add(new ItemModel
            (
                index,
                id,
                name,
                description,
                nameSpace,
                image,
                onItemTapped,
                false,
                null,
                null,
                null,
                false,
                null
            ));
        }
        public void OnItemTapped(int index)
        {
            using (var client = new OpcClient(ServerModel.ServerIP))
            {
                client.Connect();
                var node = client.BrowseNode(BrowseItem[index].Id);
                if (node.GetType().ToString() == "Opc.UaFx.Client.OpcObjectNodeInfo"
                    || node.Reference.ReferenceType.ToString() == "Organizes"
                    || node.Reference.ReferenceType.ToString() == "HasComponent"
                    )
                {
                    IsTapped = true;
                    CurrentItem = node.Attribute(OpcAttribute.DisplayName).Value.ToString();
                    CurrentId = node.NodeId;
                    DisplayChildren(node);
                }
            }
        }
        private void DisplayChildren(OpcNodeInfo node)
        {
            BrowseItem.Clear();
            int index = 0;
            foreach (var childNode in node.Children())
            {
                BrowseItem.Add(new ItemModel(
                   index,
                    childNode.NodeId,
                    childNode.Attribute(OpcAttribute.DisplayName).Value.ToString(),
                    childNode.Reference.ReferenceType.ToString(),
                    childNode.NodeId.Namespace.Index.ToString(),
                    SetImage(childNode),
                    new Command<int>(OnItemTapped),
                    SetHasAddIcon(childNode),
                    new Command<int>(OnMonitorItem),
                    new Command<OpcNodeId>(OnDeleteFromMonitor),
                    new Command<int>(OnEditItem),
                    SetHasHistory(childNode),
                    new Command<int>(OnChartItem)
                ));
                index++;
            }
        }
        private void OnBack()
        {
            using (OpcClient client = new OpcClient(ServerModel.ServerIP))
            {
                client.Connect();
                OpcNodeInfo parentNode = client.BrowseNode(CurrentId);
                foreach (OpcNodeInfo parent in parentNode.Parents())
                {
                    GrandParentNodeId = parent.NodeId;
                    break;
                }
                if (CurrentId == OpcObjectTypes.ObjectsFolder ||
                    CurrentId == OpcObjectTypes.TypesFolder ||
                    CurrentId == OpcObjectTypes.ViewsFolder)
                {
                    IsTapped = false;
                }
                OpcNodeInfo grandParentNode = client.BrowseNode(GrandParentNodeId);
                CurrentItem = grandParentNode.Attribute(OpcAttribute.DisplayName).Value.ToString();
                CurrentId = grandParentNode.NodeId;
                DisplayChildren(grandParentNode);
            }
        }
        private async void OnMonitorItem(int index)
        {
            bool alreadyExists = false;
            if (MonitorItemModel.MonitorItem.Count == 0)
            {
                MonitorItemModel.MonitorItem.Add(BrowseItem[index]);
                SetPreferences(Preferences.Get(nameof(Server), string.Empty), BrowseItem[index]);
            }
            else
            {
                foreach (var item in MonitorItemModel.MonitorItem)
                {
                    if (item.Id == BrowseItem[index].Id)
                    {
                        alreadyExists = true;
                        await App.Current.MainPage.DisplayAlert("Alert", "This variable is already monitored.", "Ok");
                        break;
                    }
                }
                if (!alreadyExists)
                {
                    MonitorItemModel.MonitorItem.Add(BrowseItem[index]);
                    SetPreferences(Preferences.Get(nameof(Server), string.Empty), BrowseItem[index]);
                }
            }
        }
        private void SetPreferences(string previousServers, ItemModel browseItem)
        {
            string servers = string.Empty;
            string[] server = previousServers.Split(';');
            foreach (string properties in server)
            {
                if (!string.IsNullOrEmpty(properties))
                {
                    string[] property = properties.Split('@');
                    string serverIP = property[0];
                    string serverName = property[1];

                    if (serverIP == ServerModel.ServerIP && serverName == ServerModel.ServerName && property.Length == 2)
                    {
                        ObservableRangeCollection<ItemJsonModel> items = new ObservableRangeCollection<ItemJsonModel>();
                        items.Add(new ItemJsonModel
                        {
                            Index = browseItem.Index,
                            Name = browseItem.Name,
                            Description = browseItem.Description,
                            Namespace = browseItem.Namespace,
                            HasAddIcon = browseItem.HasAddIcon,
                            HasHistory = browseItem.HasHistory,
                            Image = browseItem.Image
                        });
                        string itemsString = JsonConvert.SerializeObject(items);
                        servers += serverIP + "@" + serverName + "@" + itemsString + ";";
                    }
                    else if (serverIP == ServerModel.ServerIP && serverName == ServerModel.ServerName && property.Length == 3)
                    {
                        ObservableRangeCollection<ItemJsonModel> items = JsonConvert.DeserializeObject<ObservableRangeCollection<ItemJsonModel>>(property[2]);
                        items.Add(new ItemJsonModel
                        {
                            Index = browseItem.Index,
                            Name = browseItem.Name,
                            Description = browseItem.Description,
                            Namespace = browseItem.Namespace,
                            HasAddIcon = browseItem.HasAddIcon,
                            HasHistory = browseItem.HasHistory,
                            Image = browseItem.Image
                        });
                        string itemsString = JsonConvert.SerializeObject(items);
                        servers += serverIP + "@" + serverName + "@" + itemsString + ";";
                    }
                    else if (property.Length == 3)
                    {
                        ObservableRangeCollection<ItemJsonModel> items = JsonConvert.DeserializeObject<ObservableRangeCollection<ItemJsonModel>>(property[2]);
                        string itemsString = JsonConvert.SerializeObject(items);
                        servers += serverIP + "@" + serverName + "@" + itemsString + ";";
                    }
                    else
                    {
                        servers += serverIP + "@" + serverName + ";";
                    }
                }
            }
            Preferences.Set(nameof(Server), servers);
        }
        private void DeleteFromPreferences(ItemModel item)
        {
            string servers = string.Empty;
            string previousServers = Preferences.Get(nameof(Server), string.Empty);
            string[] server = previousServers.Split(';');
            foreach (string properties in server)
            {
                if (!string.IsNullOrEmpty(properties))
                {
                    string[] property = properties.Split('@');
                    string serverIP = property[0];
                    string serverName = property[1];

                    if (serverIP == ServerModel.ServerIP && serverName == ServerModel.ServerName && property.Length == 3)
                    {
                        ObservableRangeCollection<ItemJsonModel> items = JsonConvert.DeserializeObject<ObservableRangeCollection<ItemJsonModel>>(property[2]);
                        foreach(ItemJsonModel it in items)
                        {
                            if (it.Name.CompareTo(item.Name) == 0)
                            {
                                items.Remove(it);
                                break;
                            }
                        }
                        string itemsString = JsonConvert.SerializeObject(items);
                        servers += serverIP + "@" + serverName + "@" + itemsString + ";";
                    }
                    else if (property.Length == 3)
                    {
                        ObservableRangeCollection<ItemJsonModel> items = JsonConvert.DeserializeObject<ObservableRangeCollection<ItemJsonModel>>(property[2]);
                        string itemsString = JsonConvert.SerializeObject(items);
                        servers += serverIP + "@" + serverName + "@" + itemsString + ";";
                    }
                    else
                    {
                        servers += serverIP + "@" + serverName + ";";
                    }
                }
            }
            Preferences.Set(nameof(Server), servers);
        }
        private void OnDeleteFromMonitor(OpcNodeId id)
        {
            foreach (var item in MonitorItemModel.MonitorItem)
            {
                if (item.Id == id)
                {
                    MonitorItemModel.MonitorItem.Remove(item);
                    DeleteFromPreferences(item);
                    break;
                }
            }
        }
        private async void OnEditItem(int index)
        {
            string currentValue = ReadValue(index).ToLower();
            string value = await App.Current.MainPage.DisplayPromptAsync("Change variable value", "Current value is " + currentValue);
            using (var client = new OpcClient(ServerModel.ServerIP))
            {
                client.Connect();
                var node = client.BrowseNode(BrowseItem[index].Id);
                if (node is OpcVariableNodeInfo variableNode)
                {
                    OpcNodeId dataTypeId = variableNode.DataTypeId;
                    OpcDataTypeInfo dataType = client.GetDataTypeSystem().GetType(dataTypeId);
                    Console.WriteLine(dataType.Name.ToString().ToLower());

                    switch (dataType.Name.ToString().ToLower())
                    {
                        case "boolean":
                            client.WriteNode(BrowseItem[index].Id, Convert.ToBoolean(value));
                            break;
                        case "double":
                            client.WriteNode(BrowseItem[index].Id, Convert.ToDouble(value));
                            break;
                    }
                }
            }
        }
        private bool SetHasHistory(OpcNodeInfo node)
        {
            if (node.Attribute(OpcAttribute.IsHistorizing) != null)
            {
                return node.Attribute(OpcAttribute.IsHistorizing).Value.ToString().ToLower() == "true";
            }
            return false;
        }
        private async void OnChartItem(int index)
        {
            DateTime endTime = DateTime.Now;
            int k = 9;
            ChartModel.ItemToChart = BrowseItem[index].Name;
            using (var client = new OpcClient(ServerModel.ServerIP))
            {
                client.Connect();
                ChartViewModel.Entries = new ChartEntry[10];
                OpcNodeId node = BrowseItem[index].Id;
                OpcNodeInfo nodeInfo = client.BrowseNode(node);
                var historyNavigator = client.ReadNodeHistory(null, endTime, 10, node);
                do
                {
                    foreach (var value in historyNavigator)
                    {
                        if (nodeInfo is OpcVariableNodeInfo variableNode)
                        {
                            OpcNodeId dataTypeId = variableNode.DataTypeId;
                            OpcDataTypeInfo dataType = client.GetDataTypeSystem().GetType(dataTypeId);
                            Console.WriteLine(dataType.Name.ToString().ToLower());
                            
                            ChartViewModel.Entries[k] = new ChartEntry(ToFloat(value.ToString(), dataType.Name.ToString().ToLower()))
                            {
                                Label = OnlyTwoSeconds(value.Timestamp.TimeOfDay.ToString()),
                                ValueLabel = ToFloat(value.ToString(), dataType.Name.ToString().ToLower()).ToString(),
                                Color = SKColor.Parse("#3498db")
                            };
                            k--;
                        }
                    }
                } while (historyNavigator.MoveNextPage());
                historyNavigator.Close();
            }
            await Application.Current.MainPage.Navigation.PushAsync(new Views.Chart());
        }
        private string OnlyTwoSeconds(string date)
        {
            string[] parts = date.Split(':');
            string dateWithTwoDecimalsSeconds = (Convert.ToInt32(parts[0]) + 3).ToString() + ":" + parts[1] + ":" + parts[2].Substring(0, 2);
            return dateWithTwoDecimalsSeconds;
        }
        private float ToFloat(string number, string type)
        {
            Console.WriteLine(type);
            switch (type)
            {
                case "boolean":
                    if (number == "true")
                        return float.Parse("1");
                    else
                        return float.Parse("0");
                case "double":
                case "float":
                    string[] parts = number.Split(',');
                    if (parts.Length > 2)
                    {
                        number = parts[1].ToString() + "." + parts[2].ToString();
                    }
                    else
                    {
                        number = parts[1].ToString();
                    }
                    return float.Parse(number, CultureInfo.InvariantCulture.NumberFormat);
                default:
                    return float.Parse(number);
            }
        }
        private string ReadValue(int index)
        {
            using (var client = new OpcClient(ServerModel.ServerIP))
            {
                client.Connect();
                return client.ReadNode(BrowseItem[index].Id).ToString();
            }
        }
        private string SetImage(OpcNodeInfo node)
        {
            switch (node.GetType().ToString())
            {
                case "Opc.UaFx.Client.OpcObjectNodeInfo":
                    return "Folder.png";
                case "Opc.UaFx.Client.OpcVariableNodeInfo":
                    return "Edit.png";
                case "Opc.UaFx.Client.OpcMethodNodeInfo":
                    return "Folder.png";
                default:
                    return "Folder.png";
            }
        }

        private bool SetHasAddIcon(OpcNodeInfo node)
        {
            switch (node.GetType().ToString())
            {
                case "Opc.UaFx.Client.OpcObjectNodeInfo":
                    return false;
                case "Opc.UaFx.Client.OpcVariableNodeInfo":
                    return true;
                case "Opc.UaFx.Client.OpcMethodNodeInfo":
                    return false;
                default:
                    return false;
            }
        }
    }
}
