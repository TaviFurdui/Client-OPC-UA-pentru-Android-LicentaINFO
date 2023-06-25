using Licenta.Models;
using MvvmHelpers;
using Opc.UaFx;
using Opc.UaFx.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace Licenta.ViewModels
{
    public class EventsViewModel : BaseViewModel
    {
        public ObservableRangeCollection<EventModel> eventItem;
        public ObservableRangeCollection<EventModel> EventItem { get => eventItem; set => SetProperty(ref eventItem, value); }
        public EventsViewModel()
        {
            EventItem = EventModel.EventItem;
        }
        private void HandleLocalEvents(object sender, OpcEventReceivedEventArgs e)
        {
            Console.WriteLine("Severity " + e.Event.Severity);
            Console.WriteLine("Message " + e.Event.Message);
            Console.WriteLine("Time " + e.Event.ReceiveTime);
            Console.WriteLine("SourceID " + e.Event.SourceNodeId);
            Console.WriteLine("EventType " + e.Event.EventType);
            string color = string.Empty;
            string message = "There is no message for this event.";
            int severity = Convert.ToInt32(e.Event.Severity);
            switch (e.Event.Severity)
            {
                case OpcEventSeverity.Undefined:
                    severity = 0;
                    color = "green";
                    break;
                case OpcEventSeverity.Low:
                    color = "green";
                    break;
                case OpcEventSeverity.MediumLow:
                    color = "green";
                    break;
                case OpcEventSeverity.Min:
                    color = "green";
                    break;
                case OpcEventSeverity.MediumHigh:
                    color = "yellow";
                    break;
                case OpcEventSeverity.Medium:
                    color = "yellow";
                    break;
                case OpcEventSeverity.Max:
                    color = "red";
                    break;
                case OpcEventSeverity.High:
                    color = "red";
                    break;
            }
            if (!e.Event.Message.IsNull && !(e.Event.Message == ""))
            {
                message = e.Event.Message;
            }
            EventModel.EventItem.Add(new EventModel(
                message,
                e.Event.EventType.ToString(),
                e.Event.Time.ToString(),
                severity,
                e.Event.SourceNodeId,
                color
            ));
        }
    }
}
