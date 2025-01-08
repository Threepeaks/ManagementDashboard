using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ManagementDashboard.Models
{
    public class EmployeeClockingInfoResult
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string EmployeeCode { get; set; }
        public DateTime? LastClockedInDate { get; set; }
        public DateTime? LastClockedOutDate { get; set; }
    }
}