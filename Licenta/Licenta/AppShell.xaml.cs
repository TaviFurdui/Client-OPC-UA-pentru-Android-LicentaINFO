using Licenta.ViewModels;
using Licenta.Views;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace Licenta
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        public AppShell()
        {
            SetTabBarIsVisible(this, Models.ServerModel.Connected);
            InitializeComponent();
        }
    }
}
