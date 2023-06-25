using Licenta.Views;
using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;
using Licenta.Models;
using System.Text.Json;

namespace Licenta.ViewModels
{
    public class MainPageViewModel : BaseViewModel
    {
        public MainPageViewModel()
        {
            //Preferences.Clear();
            RedirectToAddNewConnection = new Command(ToAddNewConnection);
            RedirectToPreviousConnections = new Command(ToPreviousConnections);
        }
        public ICommand RedirectToAddNewConnection { get; set; }
        public ICommand RedirectToPreviousConnections { get; set; }
        public void ToAddNewConnection()
        {
            Application.Current.MainPage = new NavigationPage(new AddNewConnexion());
        }
        public async void ToPreviousConnections()
        {
            if (Preferences.Get(nameof(Server), string.Empty) != string.Empty)
            {
                await Application.Current.MainPage.Navigation.PushAsync(new PreviousConnections());
            }
            else
            {
                ConnexionErrorModel.ErrorText = "No Previous Connections";
                Application.Current.MainPage = new NavigationPage(new ConnexionError());
            }
        }
    }
}
