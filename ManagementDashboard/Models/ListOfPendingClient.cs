using System;

namespace ManagementDashboard.Models
{
    public class ListOfPendingClient
    {
        public string Ref { get; set; }
        public string Customer { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string PendingStatus { get; set; }
        public string Service { get; set; }
        public string SalesAgent { get; set; }
    }
}