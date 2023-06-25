using Licenta.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Licenta
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BottomMenu : TabbedPage
    {
        public BottomMenu()
        {
            //Routing.RegisterRoute("AddNewConnexion", typeof(AddNewConnexion));
            InitializeComponent();
        }
    }
}