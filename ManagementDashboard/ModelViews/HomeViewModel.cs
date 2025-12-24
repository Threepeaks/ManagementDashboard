using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ManagementDashboard.ModelViews
{
    public class HomeViewModel
    {
        public List<ActionDateCountWidget> ActionDateWidgets { get; set; }
    }
    public class ActionDateCountWidget
    {
        public DateTime ActionDate { get; set; }
        public int Count { get; set; }
        public string Level { get; set; }
        public string Title { get; set; }
    }
}