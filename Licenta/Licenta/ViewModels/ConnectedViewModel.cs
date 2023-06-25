using Licenta.Models;
using Licenta.Views;
using MvvmHelpers;
using Opc.UaFx.Client;
using System.Windows.Input;
using Xamarin.Forms;

namespace Licenta.ViewModels
{
    public class ConnectedViewModel : BaseViewModel
    {
        public ConnectedViewModel()
        {
            RedirectToMainPage = new Command(RedirectAndCloseConnexion);
            IsBusy = false;
        }
        public ICommand RedirectToMainPage { get; set; }
        private void RedirectAndCloseConnexion()
        {
            using (var client = new OpcClient(ServerModel.ServerIP))
            {
                client.Disconnect();
                ServerModel.ServerIP = "";
                ServerModel.ServerName = "";
                ServerModel.Connected = false;
                Application.Current.MainPage = new NavigationPage(new MainPage());
            }
        }
        string getConnectedText(string name)
        {
            return "You are connected to <br> " + name;
        }
        string getIPInfoText(string ip)
        {
            return "IP: " + ip;
        }
        public string connectedText { get => getConnectedText(ServerModel.ServerName); }
        public string ipInfoText { get => getIPInfoText(ServerModel.ServerIP); }
    }
}
