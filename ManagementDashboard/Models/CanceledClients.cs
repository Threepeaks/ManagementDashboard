using System;

namespace ManagementDashboard.Models
{
    public class CanceledClients
    {
        public string Ref { get; set; }

        public string Customer { get; set; }
        public string Comment { get; set; }

        public DateTime CancelStart { get; set; }
        public DateTime CancelEnd { get; set; }

    }
}