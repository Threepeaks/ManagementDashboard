using System;

namespace ManagementDashboard.Models
{
    public class ManagementFees
    {
        public DateTime UnpaidDate { get; set; }
        public string CustomerReference { get; set; }
        public string Code { get; set; }
        public string Type { get; set; }
        public decimal Amount { get; set; }
        public string   Reason { get; set; }

    }
}