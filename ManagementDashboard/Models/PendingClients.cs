using System;

namespace ManagementDashboard.Models
{
    public class PendingClients
    {
        public string Ref { get; set; }

        public string Customer { get; set; }

        public DateTime PendingDate { get; set; }
    }
}