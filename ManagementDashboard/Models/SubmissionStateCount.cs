using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ManagementDashboard.Models
{
    public class SubmissionCount
    {
        public string Description { get; set; }
        public int Count { get; set; }
    }

    public class SubmissionGroupByDate
    {
        public DateTime Date { get; set; }
        public int Count { get; set; }
    }
}