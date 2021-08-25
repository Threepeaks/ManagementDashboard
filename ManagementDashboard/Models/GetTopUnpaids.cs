using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ManagementDashboard.Models
{
    public class GetTopUnpaids
    {
        public string Description { get; set; }
        public string Code { get; set; }
        public int Count { get; set; }
        public int Amount { get; internal set; }
    }

    public class GetTop20Unpaids
    {
        public string Customer { get; set; }
        public int Unpaids { get; set; }
    }


}