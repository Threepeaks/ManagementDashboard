using System;

namespace ManagementDashboard.Models
{
    public class DormantClients
    {
        public string Ref { get; set; }
        public string State { get; set; }
        public DateTime PrevRun { get; set; }

        public string Next { get; set; }
        public int Span { get; set; }
    }
}