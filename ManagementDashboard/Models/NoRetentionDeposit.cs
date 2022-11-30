namespace ManagementDashboard.Models
{
    public class NoRetentionDeposit
    {
        public string Ref { get; set; }
        public string Customer { get; set; }
        public string Collateral { get; set; }
        public string HaveRuns { get; set; }
        public decimal Value { get; set; }

    }
}