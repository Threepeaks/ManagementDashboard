using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ManagementDashboard.Models
{
    public class DepositMovement
    {
        public string Customer { get; set; }
        public DateTime Date { get; set; }
        
        public decimal Value { get; set; }

    }
}