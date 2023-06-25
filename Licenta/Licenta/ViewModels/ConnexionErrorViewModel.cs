using Licenta.Models;
using Licenta.Views;
using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace Licenta.ViewModels
{
    public class ConnexionErrorViewModel : BaseViewModel
    {
        public ConnexionErrorViewModel()
        {
            RedirectToAddNewConnexion = new Command(Redirect);
        }
        public ICommand RedirectToAddNewConnexion { get; set; }
        private async void Redirect()
        {
            await Application.Current.MainPage.Navigation.PushAsync(new AddNewConnexion());
        }
        string getErrorText(string errorText)
        {
            switch (errorText)
            {
                case "UriFormatException":
                    return "Sorry, <br> The IP address does not look like a valid address.";
                case "OpcException":
                    return "Sorry, <br> We did not find the IP address.";
                case "No IP":
                    return "Did you forget the IP address?";
                case "No Name":
                    return "You have to give your server a name.";
                case "No Previous Connections":
                    return "There are no previous connections";
                default:
                    return "Something went wrong.";
            }
        }
        string getErrorInfo(string errorText)
        {
            switch (errorText)
            {
                case "UriFormatException":
                    return "A valid URI looks like this: <br> opc.tcp://address:port";
                case "OpcException":
                    return "The server is not running <br> or the IP Address is not correct.";
                case "No IP":
                    return "A valid URI looks like this: <br> opc.tcp://address:port";
                case "No Name":
                    return "You can give it any name you want.";
                case "No Previous Connections":
                    return "You have to add a new connection";
                default:
                    return "Unknown error";
            }
        }
        public string ErrorText { get => getErrorText(ConnexionErrorModel.ErrorText); }
        public string ErrorInfo { get => getErrorInfo(ConnexionErrorModel.ErrorText); }
    }
}
