using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ManagementDashboard.Models
{
    public class DepositBalance
    {

        public string Customer { get; set; }
        public decimal Amount { get; set; }
        public string Client { get; set; }
        public bool IsCanceled { get; set; }
        public DateTime? CanceledDate { get; internal set; }
        public TimeSpan AgeSinceCancelled { get; internal set; }
        public string AgeSinceCancelledString { get; internal set; }
    }
}