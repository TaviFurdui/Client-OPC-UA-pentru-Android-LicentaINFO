using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Licenta.Models
{
    public class ServerWithItemsModel
    {
        public string ServerName { get; set; }
        public string ServerIP { get; set; }
        public ObservableRangeCollection<ItemModel> MonitorItem { get; set; } = new ObservableRangeCollection<ItemModel>();
    }
}
