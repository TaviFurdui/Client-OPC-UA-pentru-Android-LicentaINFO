using MvvmHelpers;
using Xamarin.Essentials;
using System.Linq;
using Xamarin.Forms;
using System;
using Licenta.Views;
using Opc.UaFx.Client;

namespace Licenta.Models
{
    public static class PreviousConnectionsModel
    {
        public static ObservableRangeCollection<PreviousConnectionModel> PreviousConnectionItem { get; set; } = InitializeServerList();
        public static ObservableRangeCollection<PreviousConnectionModel> InitializeServerList()
        {
            return GetPreferences(Preferences.Get(nameof(Server), string.Empty));
        }
        private static void PreviousConnectionAdd(string serverIP, string serverName)
        {
            ViewModels.AddNewConnexionViewModel viewmodel = new ViewModels.AddNewConnexionViewModel();
            if (!string.IsNullOrEmpty(serverIP) && !string.IsNullOrEmpty(serverName))
            {
                PreviousConnectionItem.Add(new PreviousConnectionModel
                {
                    ServerIP = serverIP,
                    ServerName = serverName,
                    ItemTapped = new Command<string>(viewmodel.OnItemTapped)
                });
            }
        }
        private static ObservableRangeCollection<PreviousConnectionModel> GetPreferences(string previousServers)
        {
            if (previousServers != string.Empty)
            {
                PreviousConnectionItem = new ObservableRangeCollection<PreviousConnectionModel>();
                string[] server = previousServers.Split(';');
                foreach (string properties in server)
                {
                    if (!string.IsNullOrEmpty(properties))
                    {
                        string[] property = properties.Split('@');
                        string serverIP = property[0];
                        string serverName = property[1];
                        PreviousConnectionAdd(serverIP, serverName);
                    }
                }
                return PreviousConnectionItem;
            }
            else
            {
                PreviousConnectionItem = new ObservableRangeCollection<PreviousConnectionModel>();
                return PreviousConnectionItem;
            }
        }
    }
}
