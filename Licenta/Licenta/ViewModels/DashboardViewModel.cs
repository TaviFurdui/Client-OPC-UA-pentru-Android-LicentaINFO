using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Licenta.Models;
using Licenta.Views;
using MvvmHelpers;
using Opc.UaFx;
using Opc.UaFx.Client;
using Xamarin.Forms;

namespace Licenta.ViewModels
{
    public class DashboardViewModel : BaseViewModel
    {
        private int index = 0;
        private ObservableRangeCollection<string> _items;
        public ObservableRangeCollection<string> Items
        {
            get => _items;
            set => SetProperty(ref _items, value);
        }
        private ObservableRangeCollection<ItemModelUI> _elements = new ObservableRangeCollection<ItemModelUI>();
        public ObservableRangeCollection<ItemModelUI> Elements
        {
            get => _elements;
            set => SetProperty(ref _elements, value);
        }
        private string _selectedItem = "";
        public string SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                OnSelectedItemChangedCommand.Execute(SelectedItem);
            }
        }
        public Command OnSelectedItemChangedCommand { get; set; }
        public Command ChooseItems { get; set; }
        public void OnSelectedItemChanged(string selectedItem)
        {
            if (!string.IsNullOrEmpty(selectedItem))
            {
                Elements.Add(new ItemModelUI(
                    index,
                    selectedItem,
                    SetImage(selectedItem),
                    new Command<int>(OnDeleteItem),
                    new Command<int>(OnEditItem),
                    new Command<int>(OnInfoItem),
                    "Undefined"
                ));
                index++;
            }
        }

        private static TaskCompletionSource<bool> _taskCompletionSource;
        private async void OnEditItem(int index)
        {
            _taskCompletionSource = new TaskCompletionSource<bool>();
            await Application.Current.MainPage.Navigation.PushAsync(new EditDashboardItem());
            await _taskCompletionSource.Task;
            ModifyItem(index);
        }
        public static void SignalModificationDone()
        {
            _taskCompletionSource?.SetResult(true);
        }
        public void ModifyItem(int ind)
        {
            Elements[ind].NodeId = EditedUIItemModel.NodeId;
            Elements[ind].MinValue = EditedUIItemModel.MinValue;
            Elements[ind].MaxValue = EditedUIItemModel.MaxValue;
            using (OpcClient client = new OpcClient(ServerModel.ServerIP))
            {
                client.Connect();
                OpcSubscription subscription = client.SubscribeDataChange(Elements[ind].NodeId, HandleDataChanged);
                var nodeValue = client.ReadNode(Elements[ind].NodeId);
                if (float.Parse(nodeValue.ToString()) > Elements[ind].MaxValue 
                    || float.Parse(nodeValue.ToString()) < Elements[ind].MinValue)
                {
                    Elements[ind].Color = "Red";
                }
                else
                {
                    Elements[ind].Color = "Green";
                }
                Elements[ind].Image = SetImageColor(Elements[ind].Image, Elements[ind].Color);
            }
        }
        private void HandleDataChanged(
        object sender,
        OpcDataChangeReceivedEventArgs e)
        {
            e.Item.Value.ToString();
        }
        private string SetImageColor(string image, string color)
        {
            string imageString = Path.GetFileNameWithoutExtension(image);
            string[] parts = imageString.Split('_');
            string imageColor = "";
            foreach (string part in parts)
            {
                if (part.ToLower() != "green" || part.ToLower() != "red")
                imageColor += part + "_";
            }
            imageColor += color.ToLower() + ".png";
            return imageColor;
        }
        private void OnDeleteItem(int ind)
        {
            for (int i = index - 1; i >= 0; i--)
            {
                if (Elements[i].Index == ind)
                {
                    Elements.RemoveAt(i);
                    index--;
                    break;
                }
            }
        }
        private async void OnInfoItem(int index)
        {
            string info;
            if (Elements[index].NodeId != null)
            {
                string name, datatype, displayname, writeaccess;
                string color = "Color: " + Elements[index].Color;
                string minvalue = "Minimum value: " + Elements[index].MinValue;
                string maxvalue = "Maximum value: " + Elements[index].MaxValue;
                string nodeid = "Node id: " + Elements[index].NodeId;
                using (OpcClient client = new OpcClient(ServerModel.ServerIP))
                {
                    client.Connect();
                    var nodeValue = client.BrowseNode(Elements[index].NodeId);
                    datatype = client.GetDataTypeSystem().GetType(nodeid).Name.ToString();
                    name = "Name: " + nodeValue.Name;
                    displayname = "Display name: " + nodeValue.DisplayName;
                    writeaccess = "Write access: " + nodeValue.Attribute(Opc.UaFx.OpcAttribute.WriteAccess).Value.ToString();
                }
                info = nodeid + '\n'
                    + name + '\n'
                    + displayname + '\n'
                    + datatype + '\n'
                    + writeaccess + '\n'
                    + color + '\n'
                    + minvalue + '\n'
                    + maxvalue + '\n';
            }
            else
            {
                info = "There is no node related to this UI element. Click the edit button in order to attach node info.";
            }
            
            await App.Current.MainPage.DisplayAlert("Info", info, "OK");
        }
        private string SetImage(string selectedItem)
        {
            switch (selectedItem)
            {
                case "Motor":
                    return "Electric_motor.png";
                case "Speedometer":
                    return "Speedometer.png";
                case "Thermometer":
                    return "Thermometer.png";
                case "Water pump":
                    return "Water_pump.png";
                case "Alarm":
                    return "Bell.png";
                default:
                    return "DogError.png";
            }
        }
        private async void OnChooseItems()
        {
            string selectedOption = await App.Current.MainPage.DisplayActionSheet("What item do you want to add?", "Cancel", null, "Motor", "Thermometer", "Speedometer", "Alarm", "Water pump");
            if (selectedOption != null && selectedOption != "Cancel")
            {
                OnSelectedItemChangedCommand.Execute(selectedOption);
            }
        }
        public DashboardViewModel()
        {
            OnSelectedItemChangedCommand = new Command<string>(OnSelectedItemChanged);
            ChooseItems = new Command(OnChooseItems);
            Items = new ObservableRangeCollection<string>
            {
                "Motor",
                "Speedometer",
                "Thermometer",
                "Water pump",
                "Alarm"
            };
        }
    }
}
