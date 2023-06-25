using Licenta.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Licenta.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PreviousConnections : ContentPage
    {
        public PreviousConnections()
        {
            InitializeComponent();
            BindingContext = new PreviousConnectionsViewModel();
        }
    }
}