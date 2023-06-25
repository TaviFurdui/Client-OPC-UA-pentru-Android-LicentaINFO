using System;
using System.Windows.Input;
using Xamarin.Forms;
using Opc.UaFx.Client;
using Opc.UaFx.Client.Classic;
using Licenta.Views;
using Licenta.Models;
using MvvmHelpers;
using Xamarin.Essentials;
using System.Linq;
using System.Text.Json;
using Newtonsoft.Json;
using System.Collections.Generic;
using Opc.UaFx;

namespace Licenta.ViewModels
{
    public class AddNewConnexionViewModel : BaseViewModel
    {
        public AddNewConnexionViewModel()
        {
            RedirectToMainPage = new Command(Redirect);
            AddNewConnexion = new Command(AddConnexion);
        }
        public ICommand AddNewConnexion { get; set; }
        public ICommand RedirectToMainPage { get; set; }
        
        public OpcClient client;
        string serverIP = "";
        string serverName = "";
        public string ServerIP
        {
            get => serverIP;
            set => SetProperty(ref serverIP, value);
        }
        public string ServerName
        {
            get => serverName;
            set => SetProperty(ref serverName, value);
        }
        private void AddConnexion() 
        {
            if (string.IsNullOrEmpty(ServerIP))
            {
                ConnexionErrorModel.ErrorText = "No IP";
                Application.Current.MainPage = new NavigationPage(new ConnexionError());
            }
            else if (string.IsNullOrEmpty(ServerName))
            {
                ConnexionErrorModel.ErrorText = "No Name";
                Application.Current.MainPage = new NavigationPage(new ConnexionError());
            }
            else
            {
                try
                {
                    OpcClient client = new OpcClient(ServerIP);
                    client.Connect();
                    var filter = OpcFilter.Using(client)
                    .FromEvents(OpcEventTypes.Event)
                    .Select();
                    client.SubscribeEvent(OpcObjectTypes.Server, filter, HandleLocalEvents);
                    ServerModel.ServerIP = ServerIP;
                    ServerModel.ServerName = ServerName;
                    ServerModel.Connected = true;
                    SetPreferences(Preferences.Get(nameof(Server), string.Empty));
                    IsBusy = true;
                    Application.Current.MainPage = new Connected();
                    Application.Current.MainPage = new NavigationPage(new BottomMenu());
                    
                }
                catch (Exception ex)
                {
                    ConnexionErrorModel.ErrorText = ex.GetType().Name;
                    Application.Current.MainPage = new NavigationPage(new ConnexionError());
                }
            }
        }
        private void HandleLocalEvents(object sender, OpcEventReceivedEventArgs e)
        {
            Console.WriteLine("Severity " + e.Event.Severity);
            Console.WriteLine("Message " + e.Event.Message);
            Console.WriteLine("Time " + e.Event.ReceiveTime);
            Console.WriteLine("SourceID " + e.Event.SourceNodeId);
            Console.WriteLine("EventType " + e.Event.EventType);
            string color = string.Empty;
            string message = "There is no message for this event.";
            int severity = Convert.ToInt32(e.Event.Severity);
            switch (e.Event.Severity)
            {
                case OpcEventSeverity.Undefined:
                    severity = 0;
                    color = "green";
                    break;
                case OpcEventSeverity.Low:
                    color = "green";
                    break;
                case OpcEventSeverity.MediumLow:
                    color = "green";
                    break;
                case OpcEventSeverity.Min:
                    color = "green";
                    break;
                case OpcEventSeverity.MediumHigh:
                    color = "yellow";
                    break;
                case OpcEventSeverity.Medium:
                    color = "yellow";
                    break;
                case OpcEventSeverity.Max:
                    color = "red";
                    break;
                case OpcEventSeverity.High:
                    color = "red";
                    break;
            }
            if (!e.Event.Message.IsNull && !(e.Event.Message == ""))
            {
                message = e.Event.Message;
            }
            EventModel.EventItem.Add(new EventModel(
                message,
                e.Event.EventType.ToString(),
                e.Event.Time.ToString(),
                severity,
                e.Event.SourceNodeId,
                color
            ));
        }
        private void Redirect()
        {
            Application.Current.MainPage = new NavigationPage(new MainPage());
        }
        private void SetPreferences(string previousServers)
        {
            if (previousServers.Contains(ServerIP) == false)
            {
                PreviousConnectionsModel.PreviousConnectionItem.Clear();
                previousServers += ServerIP + "@" + ServerName + ";";
                Preferences.Set(nameof(Server), previousServers);
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
            }
        }
        private void PreviousConnectionAdd(string serverIP, string serverName)
        {
            if (!string.IsNullOrEmpty(serverIP) && !string.IsNullOrEmpty(serverName))
            {
                PreviousConnectionsModel.PreviousConnectionItem.Add(new PreviousConnectionModel
                {
                    ServerIP = serverIP,
                    ServerName = serverName,
                    ItemTapped = new Command<string>(OnItemTapped)
                });
            }
        }
        public void OnItemTapped(string ServerIP)
        {
            try
            {
                OpcClient client = new OpcClient(ServerIP);
                client.Connect();
                var filter = OpcFilter.Using(client)
                .FromEvents(OpcEventTypes.Event)
                .Select();
                client.SubscribeEvent(OpcObjectTypes.Server, filter, HandleLocalEvents);
                ServerModel.ServerIP = ServerIP;
                ServerModel.ServerName = GetServerName(ServerIP);
                ServerModel.Connected = true;
                IsBusy = true;
                Application.Current.MainPage = new Connected();
                Application.Current.MainPage = new NavigationPage(new BottomMenu());
            }
            catch (Exception ex)
            {
                ConnexionErrorModel.ErrorText = ex.GetType().Name;
                Console.WriteLine("EX: " + ex);
                Application.Current.MainPage = new NavigationPage(new ConnexionError());
            }
        }

        private static string GetServerName(string ServerIP)
        {
            foreach (PreviousConnectionModel server in PreviousConnectionsModel.PreviousConnectionItem)
            {
                if (server.ServerIP == ServerIP)
                    return server.ServerName;
            }
            return string.Empty;
        }
    }
}
