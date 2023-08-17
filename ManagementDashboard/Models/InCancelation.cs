namespace ManagementDashboard.Models
{
    public class InCancelation
    {
        public string Ref { get; set; }

        public string Customer { get; set; }
        public string Comment { get; set; }

        public string PendingStatus { get; set; }
        public string CancelStatus { get; set; }

    }
}