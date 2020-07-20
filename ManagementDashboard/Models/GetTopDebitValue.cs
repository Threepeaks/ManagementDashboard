using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ManagementDashboard.Models
{
    public class GetTopDebitValue
    {
        //Customer Reference / Debit Reference / Action Date / RBR /Amount
        public string CustomerRef { get; set; }
        public string DebitRef { get; set; }

        public DateTime ActionDate { get; set; }
        public int Rbr { get; set; }
        public decimal Amount { get; set; }

    }
}