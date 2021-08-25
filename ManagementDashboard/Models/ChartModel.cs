using Chart.Mvc.ComplexChart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ManagementDashboard.Models
{
    public class ChartModel
    {
        public List<ComplexDataset> ComplesDatasets { get; set; }
        public string[] Labels { get; set; }
        public string Title { get; internal set; }
        public string ChartID { get; internal set; }

        public bool ShowTable { get; set; }
        public int ChartHeight { get; internal set; } = 400;

        public List<string> DataTableItems { get; set; }

    }

    public class DataTableItem
    {
        public string Value { get; set; }
    }

}