using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ManagementDashboard.Models
{
    public class GetTopValueCustomer
    {
        public string Customer { get; set; }
        public decimal Collections { get; set; }
        public int NumOfRecords { get; set; }
    }
}