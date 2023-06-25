using System;
using System.Collections.Generic;
using System.Text;

namespace Licenta.Models
{
    public class EditedUIItemModel
    {
        public static string NodeId { get; set; }
        public static int MinValue { get; set; }
        public static int MaxValue { get; set; }
        public EditedUIItemModel(string nodeId, int minValue, int maxValue)
        {
            NodeId = nodeId;
            MinValue = minValue;
            MaxValue = maxValue;
        }
    }
}
