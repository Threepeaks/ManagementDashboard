using System;

namespace ManagementDashboard.Models
{
    public class HighestCustomerValue
    {
        public string Ref { get; set; }
        public DateTime CancelStart { get; set; }
        public DateTime CancelEnd { get; set; }
        public decimal Value { get; set; }
        public int Count { get; set; }
    }
}