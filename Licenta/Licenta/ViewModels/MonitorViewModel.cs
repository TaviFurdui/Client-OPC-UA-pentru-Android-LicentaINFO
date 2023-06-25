using Licenta.Models;
using Licenta.Views;
using MvvmHelpers;
using Newtonsoft.Json;
using System;
using System.Text.Json;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Licenta.ViewModels
{
    public class MonitorViewModel : BaseViewModel
    {
        public Command ChatBot { get; set; }

        public ObservableRangeCollection<ItemModel> monitorItem;
        public ObservableRangeCollection<ItemModel> MonitorItem { get => monitorItem; set => SetProperty(ref monitorItem, value); }
        public MonitorViewModel()
        {
            MonitorItem = MonitorItemModel.MonitorItem;
            ChatBot = new Command(OnChatBot);
        }
        private async void OnChatBot()
        {
            await Application.Current.MainPage.Navigation.PushAsync(new ChatBot());
        }
    }
}

