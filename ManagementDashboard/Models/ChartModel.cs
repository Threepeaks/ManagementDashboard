using Chart.Mvc.ComplexChart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ManagementDashboard.Models
{
    public class ChartModel
    {
        public string Title { get; set; }
        public string ChartID { get; set; }
        public bool ShowTable { get; set; }
        public int ChartHeight { get; set; } = 400;

        public bool Responsive { get; set; } = true;
        public bool MaintainAspectRatio { get; set; } = true;
        public bool ScaleBeginAtZero { get; set; } = true;


        public string[] Labels { get; set; }
        public List<ComplexDataset> ComplexDatasets { get; set; }
        public List<string> DataTableItems { get; set; }

        
    }

    public class DataTableItem
    {
        public string Value { get; set; }
    }

}