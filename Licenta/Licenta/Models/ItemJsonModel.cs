using Opc.UaFx;
using System.Windows.Input;
using Xamarin.Forms;
using Licenta.ViewModels;
using Newtonsoft.Json;
using System;
using Newtonsoft.Json.Linq;

namespace Licenta.Models
{
    public class ItemJsonModel
    {
        public int Index { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Namespace { get; set; }
        public string Image { get; set; }
        public bool HasAddIcon { get; set; }
        public bool HasHistory { get; set; }
    }
}
