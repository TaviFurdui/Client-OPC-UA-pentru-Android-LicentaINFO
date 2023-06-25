using Licenta.Models;
using Licenta.Views;
using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Licenta.ViewModels
{
    public class EditDashboardItemViewModel : BaseViewModel
    {
        private string nodeId;
        private int minValue;
        private int maxValue;
        public string NodeId
        {
            get => nodeId;
            set => SetProperty(ref nodeId, value);
        }
        public int MinValue
        {
            get => minValue;
            set => SetProperty(ref minValue, value);
        }
        public int MaxValue
        {
            get => maxValue;
            set => SetProperty(ref maxValue, value);
        }
        public Command EditComplete { get; set; }
        public EditDashboardItemViewModel()
        {
            EditComplete = new Command(OnEditComplete);
        }
        private void OnEditComplete()
        {
            EditedUIItemModel.NodeId = NodeId;
            EditedUIItemModel.MaxValue = MaxValue;
            EditedUIItemModel.MinValue = MinValue;
            DashboardViewModel.SignalModificationDone();
            Application.Current.MainPage.Navigation.PopAsync();
        }
    }
}
