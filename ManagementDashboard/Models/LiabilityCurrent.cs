namespace ManagementDashboard.Models
{
    public class LiabilityCurrent
    {
        public string Customer { get; set; }
        public string Status { get; set; }
        public string Gateway { get; set; }
        public decimal BalanceBroughtForward { get; set; }
        public decimal Collection { get; set; }
        public decimal Unpaids { get; set; }
        public decimal LateUnpaids { get; set; }
        public decimal RetentionHeld { get; set; }
        public decimal Account { get; set; }
        public decimal Balance { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }

    }


    public class LateUnpaidsCreditProvided
    {
        //rbr_comref, sum(dbt_amount), Total Holding, Risk Type, Credit Provided
        public string Customer { get; set; }
        public decimal TotalDebit { get; set; }
        public decimal TotalHolding { get; set; }
        public decimal CreditProvided { get; set; }
        public string RiskType { get; set; }
    }
}