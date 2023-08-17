using Chart.Mvc.ComplexChart;
using ManagementDashboard.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace ManagementDashboard.Controllers
{

    [Authorize]
    public class TPMSController : Controller
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        

        [OutputCache(Duration = MD_CONST_DURATIONS.OUTPUTCASH_DURATION, VaryByParam = "id")]
        public PartialViewResult ReconDebitTrendChart(int id)
        {
            int monthSelected = 0;
            if (id > 0)
                monthSelected = -1 * id;


            DateTime currentDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(monthSelected);
            DateTime startDate = currentDate;
            DateTime endDate = currentDate.AddMonths(1).AddDays(-1);
            var db = new DBConnect();
            //Get data from db - query
            string query = $"SELECT date_format(rcd_date,'%Y-%m-%d') as groupDate, sum(rcd_paid) as paid FROM threepeaks_tpms.tblrecon_data where rcd_date between '{startDate.ToString("yyyy-MM-dd")}' and '{endDate.ToString("yyyy-MM-dd")}' group by date_format(rcd_date,'%Y-%m-%d')";
            DataSet result = db.Query(query);


            var listLabels = new List<string>();
            foreach (DataRow dr in result.Tables[0].Rows)
            {
                listLabels.Add(Convert.ToDateTime(dr["groupDate"]).ToString(MD_CONST_FORMATS.LABEL_DATE_FORMAT));
            }
            string[] labels = listLabels.ToArray();


            var data = new List<ComplexDataset>();

            var ReconData = new List<double>();
            foreach (DataRow dr in result.Tables[0].Rows)
            {
                //Type t = dr["paid"].GetType();
                ReconData.Add((double)dr.Field<decimal>("paid"));
            }



            data.Add(new ComplexDataset
            {

                Data = ReconData,
                Label = "Recon Debit Trend",
                FillColor = "rgba(151,187,205,0.2)",
                StrokeColor = "rgba(151,187,205,1)",
                PointColor = "rgba(151,187,205,1)",
                PointStrokeColor = "#fff",
                PointHighlightFill = "#fff",
                PointHighlightStroke = "rgba(151,187,205,1)",
            });


            ManagementDashboard.Models.ChartModel model = new ChartModel();
            model.Labels = labels;
            model.ComplexDatasets = data;
            model.Title = $"Date Range {startDate.ToString("dd-MM-yyyy")} to {endDate.ToString("dd-MM-yyyy")}";

            model.ChartID = $"TrendPaymentValue{id}";
            model.ShowTable = false;
            return PartialView("LineChartPartial", model);
        }






  
 



    }
}