using System;

namespace ManagementDashboard.Models
{
    public class NoRunClients
    {
        public string Ref { get; set; }
        public DateTime StartDate { get; set; }
        public string Customer { get; set; }
        public int Days { get; set; }
    }
}