using Licenta.Models;
using Microcharts;
using MvvmHelpers;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace Licenta.ViewModels
{
    public class ChartViewModel : BaseViewModel
    {
        public Chart ChartData { get; set; }
        public static ChartEntry[] Entries { get; set; }

        public string itemToChart;
        public string ItemToChart { get => itemToChart; set => SetProperty(ref itemToChart, value); }
        public ChartViewModel()
        {
            ItemToChart = ChartModel.ItemToChart;
            ChartData = new LineChart()
            {
                Entries = Entries,
                ValueLabelOrientation = Orientation.Horizontal,
                LabelOrientation = Orientation.Vertical,
                LabelTextSize = 26
            };
        }
    }
}
