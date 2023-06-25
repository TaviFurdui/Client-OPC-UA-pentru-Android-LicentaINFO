using Licenta.ViewModels;
using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Licenta.Models
{
    public class ItemModelUI : BaseViewModel
    {
        public string Text { get; set; }

        public string image;
        public string Image
        {
            get => image;
            set => SetProperty(ref image, value);
        }
        public int Index { get; set; }
        public string NodeId { get; set; }
        public int MinValue { get; set; }
        public int MaxValue { get; set; }

        public string color;
        public string Color
        {
            get => color;
            set => SetProperty(ref color, value);
        }
        public string valueNode;
        public string ValueNode
        {
            get => valueNode;
            set => SetProperty(ref valueNode, value);
        }
        public Command DeleteItem { get; set; }
        public Command EditItem { get; set; }
        public Command InfoItem { get; set; }
        public ItemModelUI(int index, string text, string image, Command<int> deleteItem, Command<int> editItem, Command<int> infoItem, string color)
        {
            Index = index;
            Text = text;
            Image = image;
            DeleteItem = deleteItem;
            EditItem = editItem;
            InfoItem = infoItem;
            Color = color;
        }
    }
}
