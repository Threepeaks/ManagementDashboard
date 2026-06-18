using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ManagementDashboard.Models
{
    public class XeroClient
    {
        public string ConnectionStatus { get; set; }
        public string CustomerName { get; set; }
        public string CustomerReference { get; set; }
        public string TenantName { get; set; }
        public int ServiceStatus { get; set; }
        public int ConnectionStatusId { get; set; }
        public bool IsMapped { get; set; }
        public DateTime? LastActionDate { get; set; }
    }

    public class InsiderClient
    {
        public string ClientReference { get; set; }
        public string CustomerName { get; set; }
        public DateTime ActivationDate { get; set; }
    }
}