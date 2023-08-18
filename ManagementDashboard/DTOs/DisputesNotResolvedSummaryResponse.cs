using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ManagementDashboard.DTOs
{
    public class DisputesNotResolvedSummaryResponse
    {

        public List<ClientDisputeSummary> ClientDisputeSummaries { get; set; }



    }
    public class ClientDisputeSummary
    {
        public int ClientId { get; set; }
        public string ClientReference { get; set; }
        public int Count { get; set; }
        public float Value { get; set; }
        public string ClientName { get; set; }
    }


    public class Dispute
    {
        public string DebtorName { get; set; }
        public string DebtorReference { get; set; }
        public string UnpaidCode { get; set; }
        public DateTime ResolveDueDate { get; set; }
        public DateTime ActionDate { get; set; }
        public int Id { get; set; }
        public int ClientId { get; set; }
        public string ClientReference { get; set; }
        public string ClientName { get; set; }
        public decimal Amount { get; set; }
    }

}