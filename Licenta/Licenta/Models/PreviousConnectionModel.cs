using Licenta.Views;
using Opc.UaFx.Client;
using System;
using System.Windows.Input;
using Xamarin.Forms;

namespace Licenta.Models
{
    public class PreviousConnectionModel
    {
        public ICommand ItemTapped { get; set; }
        public ICommand Edit { get; set; }
        public ICommand Delete { get; set; }
        public string ServerIP { get; set; }
        public string ServerName { get; set; }
        
    }
}


