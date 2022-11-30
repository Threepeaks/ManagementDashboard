using System;

namespace ManagementDashboard.Models
{
    public class RetNotReleased
    {
        public string Ref { get; set; }
        public int Rbr { get; set; }
        public DateTime ActionDate { get; set; }
        public DateTime ReleaseDate { get; set; }
        public decimal RetAmount { get; set; }
        public int Age { get; set; }

    }
}