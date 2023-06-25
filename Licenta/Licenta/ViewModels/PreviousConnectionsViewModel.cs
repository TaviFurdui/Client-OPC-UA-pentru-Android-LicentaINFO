using Licenta.Models;
using MvvmHelpers;

namespace Licenta.ViewModels
{
    class PreviousConnectionsViewModel : BaseViewModel
    {
        public ObservableRangeCollection<PreviousConnectionModel> previousConnectionItem;
        public ObservableRangeCollection<PreviousConnectionModel> PreviousConnectionItem { get => previousConnectionItem; set => SetProperty(ref previousConnectionItem, value); }
        public PreviousConnectionsViewModel()
        {
            PreviousConnectionItem = new ObservableRangeCollection<PreviousConnectionModel>();
            PreviousConnectionItem = PreviousConnectionsModel.PreviousConnectionItem;
        }
    }
}
