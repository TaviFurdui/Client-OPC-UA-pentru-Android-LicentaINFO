using MvvmHelpers;
using Opc.UaFx;
using Opc.UaFx.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace Licenta.Models
{
    public class EventModel
    {
        public static ObservableRangeCollection<EventModel> EventItem { get; set; } = new ObservableRangeCollection<EventModel>();
        public EventModel(string message, string eventType, string dateTime, int severity, OpcNodeId source, string color)
        {
            Message = message;
            EventType = eventType;
            DateTime = dateTime;
            Severity = severity;
            Source = source;
            Color = color;
        }
        public static OpcSubscription OpcSubscription { get; set; }
        public string Message { get; set; }
        public string EventType { get; set; }
        public string DateTime { get; set; }
        public int Severity { get; set; }
        public string Color { get; set; }
        public OpcNodeId Source { get; set; }
    }
}
