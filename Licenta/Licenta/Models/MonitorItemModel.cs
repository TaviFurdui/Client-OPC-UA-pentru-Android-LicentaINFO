using Licenta.Views;
using MvvmHelpers;
using Newtonsoft.Json;
using Opc.UaFx;
using Opc.UaFx.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Licenta.Models
{
    public static class MonitorItemModel
    {
        public static ObservableRangeCollection<ItemModel> MonitorItem { get; set; } = SetMonitorItem();
        private static ObservableRangeCollection<ItemModel> SetMonitorItem()
        {
            if (!string.IsNullOrEmpty(Preferences.Get(nameof(Server), string.Empty)))
            {
                string previousServers = Preferences.Get(nameof(Server), string.Empty);
                string servers = string.Empty;
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
                            MonitorItem = new ObservableRangeCollection<ItemModel>();
                            ObservableRangeCollection<ItemJsonModel> jsonItems = JsonConvert.DeserializeObject<ObservableRangeCollection<ItemJsonModel>>(property[2]);
                            foreach (ItemJsonModel item in jsonItems)
                            {
                                MonitorItem.Add(new ItemModel
                                (
                                    item.Index,
                                    OpcNodeId.Parse("ns=" + item.Namespace + ";s=" + item.Name),
                                    item.Name,
                                    item.Description,
                                    item.Namespace,
                                    item.Image,
                                    null,
                                    item.HasAddIcon,
                                    null,
                                    new Command<OpcNodeId>(OnDeleteFromMonitor),
                                    new Command<int>(OnEditItem),
                                    item.HasHistory,
                                    new Command<int>(OnChartItem)
                                ));
                            }
                            return MonitorItem;
                        }
                    }
                }
                return new ObservableRangeCollection<ItemModel>();
            }
            else
            {
                return new ObservableRangeCollection<ItemModel>();
            }
        }
        private static void DeleteFromPreferences(ItemModel item)
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
                        foreach (ItemJsonModel it in items)
                        {
                            if (it.Name.CompareTo(item.Name) == 0)
                            {
                                items.Remove(it);
                                Console.WriteLine("S-A AJUNS AICI!");
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
        private static void OnDeleteFromMonitor(OpcNodeId id)
        {
            foreach (var item in MonitorItem)
            {
                if (item.Id == id)
                {
                    MonitorItem.Remove(item);
                    DeleteFromPreferences(item);
                    break;
                }
            }
        }
        private static void OnChartItem(int index)
        {
            Application.Current.MainPage = new NavigationPage(new Chart());
        }
        private static async void OnEditItem(int index)
        {
            string currentValue = ReadValue(index).ToLower();
            string value = await App.Current.MainPage.DisplayPromptAsync("Change variable value", "Current value is " + currentValue);
            using (var client = new OpcClient(ServerModel.ServerIP))
            {
                client.Connect();
                var node = client.BrowseNode(MonitorItem[index].Id);
                if (node is OpcVariableNodeInfo variableNode)
                {
                    OpcNodeId dataTypeId = variableNode.DataTypeId;
                    OpcDataTypeInfo dataType = client.GetDataTypeSystem().GetType(dataTypeId);
                    Console.WriteLine(dataType.Name.ToString().ToLower());

                    switch (dataType.Name.ToString().ToLower())
                    {
                        case "boolean":
                            client.WriteNode(MonitorItem[index].Id, Convert.ToBoolean(value));
                            break;
                        case "double":
                            client.WriteNode(MonitorItem[index].Id, Convert.ToDouble(value));
                            break;
                    }
                }
            }
        }
        private static string ReadValue(int index)
        {
            using (var client = new OpcClient(ServerModel.ServerIP))
            {
                client.Connect();
                return client.ReadNode(MonitorItem[index].Id).ToString();
            }
        }
    }
}
