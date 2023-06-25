using Licenta.Views;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Licenta
{
    public partial class App : Application
    {

        public App()
        {
            MainPage = new NavigationPage(new MainPage());
            InitializeComponent();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
