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
    }

    public class NoRunClients
    {
        public string Ref { get; set; }
        public DateTime StartDate { get; set; }
        public string Customer { get; set; }
    }
    
    public class CanceledClients
    {
        public string Ref { get; set; }

        public string Customer { get; set; }
        public string Comment { get; set; }

        public DateTime CancelStart { get; set; }
        public DateTime CancelEnd { get; set; }

    }
    public class RetNotReleased
    {
        public string Ref { get; set; }
        public int Rbr { get; set; }
        public DateTime ActionDate { get; set; }
        public DateTime ReleaseDate { get; set; }
        public decimal RetAmount { get; set; }
        public int Age { get; set; }

    }

    public class PendingClients
    {
        public string Ref { get; set; }

        public string Customer { get; set; }

        public DateTime PendingDate { get; set; }
    }

    public class NoRetentionDeposit
    {
        public string Ref { get; set; }
        public string Customer { get; set; }
        public string Collateral { get; set; }
        public string HaveRuns { get; set; }
        public decimal Value { get; set; }

    }
 }