using Chart.Mvc.ComplexChart;
using ManagementDashboard.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ManagementDashboard.Controllers
{
    [Authorize]
    public class SubmissionsController : Controller
    {
        // GET: Submissions
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult NewGrowth()
        {


            return View();
        }

        [OutputCache(Duration = MD_CONST_DURATIONS.OUTPUTCASH_DURATION)]
        public ActionResult RunsNotSentToBank()
        {

            var rnsb = new Models.SQLReportTableViewModel();

            var db = new DBConnect();

            string file = Server.MapPath("~") + "SQLQueries\\RunsNotSentToBank.sql";
            if (System.IO.File.Exists(file))
            {
                StreamReader streamReader = new StreamReader(file);
                var fileContent = streamReader.ReadToEnd();

                var query = fileContent;

                var result = db.Query(query);
                string htmlTable = result.Tables[0].ConvertDataTableToHTML();
                rnsb.HtmlTable = htmlTable;

            }

            return View(rnsb);
        }

        [OutputCache(Duration = MD_CONST_DURATIONS.OUTPUTCASH_DURATION, VaryByParam = "id")]
        public PartialViewResult SubmissionsTrendChart(int id)
        {
            int monthSelected = 0;
            if (id > 0)
                monthSelected = -1 * id;


            DateTime currentDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(monthSelected);
            DateTime startDate = currentDate;
            DateTime endDate = currentDate.AddMonths(1).AddDays(-1);
            var db = new DBConnect();
            //Get data from db - query
            string query = $"select rbr_date,count(*) as 'count' from tblrbr where rbr_date " +
                $"between '{startDate.ToString("yyyy-MM-dd")}' and '{endDate.ToString("yyyy-MM-dd")}' " +
                $"and rbr_status not in (4,99) group by rbr_date";
            DataSet result = db.Query(query);
            //create model to pass to view, 

            var listLabels = new List<string>();
            foreach (DataRow dr in result.Tables[0].Rows)
            {
                listLabels.Add(Convert.ToDateTime(dr["rbr_date"]).ToString(MD_CONST_FORMATS.LABEL_DATE_FORMAT));
            }
            string[] labels = listLabels.ToArray();


            var data = new List<ComplexDataset>();

            var submissionData = new List<double>();
            foreach (DataRow dr in result.Tables[0].Rows)
            {
                submissionData.Add((double)dr.Field<Int64>("count"));
            }
            data.Add(new ComplexDataset
            {

                Data = submissionData,
                Label = "Submissions",
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
            model.Title = $"Date Range {startDate.ToString("dd/MM/yyyy")} to {endDate.ToString("dd/MM/yyyy")}";
            model.ChartID = $"chart{id}";
            return PartialView("LineChartPartial", model);
        }

        [OutputCache(Duration = MD_CONST_DURATIONS.OUTPUTCASH_DURATION, VaryByParam = "id")]
        public PartialViewResult GetTopValueDebits(int id)
        {
            int monthSelected = 0;
            if (id > 0)
                monthSelected = -1 * id;
            DateTime currentDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(monthSelected);
            DateTime startDate = currentDate;
            DateTime endDate = currentDate.AddMonths(1).AddDays(-1);


            var db = new DBConnect();
            string query = "select dbt_comref,dbt_ref,dbt_date,dbt_rbr,dbt_amount  from tbldebits left join tblrbr on rbr_id = dbt_rbr where rbr_status not in (99) and " +
                $"dbt_date between '{startDate.ToString("yyyy-MM-dd")}' and '{endDate.ToString("yyyy-MM-dd")}' order by dbt_amount  desc limit 20";
            var model = new List<ManagementDashboard.Models.GetTopDebitValue>(); // do you have a model yet  ??? ();
            var result = db.Query(query);

            foreach (DataRow dr in result.Tables[0].Rows)
            {
                var topItem = new Models.GetTopDebitValue();
                topItem.Amount = (decimal)dr.Field<decimal>("dbt_amount");
                topItem.CustomerRef = dr.Field<string>("dbt_comref");
                topItem.DebitRef = dr.Field<string>("dbt_ref");
                topItem.Rbr = (int)dr.Field<Int32>("dbt_rbr");
                topItem.ActionDate = (DateTime)dr.Field<DateTime>("dbt_date");

                model.Add(topItem);
            }
            return PartialView(model);
        }

        [OutputCache(Duration = MD_CONST_DURATIONS.OUTPUTCASH_DURATION, VaryByParam = "id")]
        public PartialViewResult GetTop20Unpaids(int id)
        {
            int monthSelected = 0;
            if (id > 0)
                monthSelected = -1 * id;
            DateTime currentDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(monthSelected);
            DateTime startDate = currentDate;
            DateTime endDate = currentDate.AddMonths(1).AddDays(-1);

            var db = new DBConnect();
            string query = "select dbt_comref as 'Company Ref', count(*) as 'Unpaids' from tbldebits " +
                "where dbt_pass_unpaid in (2,3) and dbt_pass_unpaid_datetime between " +
               $"'{startDate.ToString("yyyy-MM-dd")}' and '{endDate.ToString("yyyy-MM-dd")}' group by " +
                "dbt_comref order by `Unpaids` desc limit 20";
            var model = new List<ManagementDashboard.Models.GetTop20Unpaids>();
            var result = db.Query(query);


            foreach (DataRow dRow in result.Tables[0].Rows)
            {
                var t20unpaid = new Models.GetTop20Unpaids();
                t20unpaid.Customer = dRow.Field<string>("Company Ref");
                t20unpaid.Unpaids = (int)dRow.Field<Int64>("Unpaids");

                model.Add(t20unpaid);

            }

            return PartialView(model);

        }


        [OutputCache(Duration = MD_CONST_DURATIONS.OUTPUTCASH_DURATION, VaryByParam = "id")]
        public PartialViewResult DebitsRecordsTrendChart(int id)
        {

            int monthSelected = 0;
            if (id > 0)
                monthSelected = -1 * id;

            DateTime currentDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(monthSelected);
            DateTime startDate = currentDate;
            DateTime endDate = currentDate.AddMonths(1).AddDays(-1);
            var db = new DBConnect();
            //Get data from db - query
            string query = $"select rbr_date,count(*) as 'Initial' from tbldebits left join tblrbr on rbr_id = dbt_rbr " +
                $"where rbr_status not in (99) " +
                $"and rbr_date between '{startDate.ToString("yyyy-MM-dd")}' and '{endDate.ToString("yyyy-MM-dd")}' group by rbr_date";
            DataSet result = db.Query(query);

            var listLabels = new List<string>();
            foreach (DataRow dr in result.Tables[0].Rows)
            {
                listLabels.Add(Convert.ToDateTime(dr["rbr_date"]).ToString(MD_CONST_FORMATS.LABEL_DATE_FORMAT));
            }
            string[] labels = listLabels.ToArray();

            var data = new List<ComplexDataset>();

            var submissionData = new List<double>();
            foreach (DataRow dr in result.Tables[0].Rows)
            {
                submissionData.Add((double)dr.Field<Int64>("Initial"));
            }
            data.Add(new ComplexDataset
            {

                Data = submissionData,
                Label = "Submissions",
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
            model.ChartID = $"DebtorRecordsTrendChart{id}";
            return PartialView("LineChartPartial", model);

        }

        [OutputCache(Duration = MD_CONST_DURATIONS.OUTPUTCASH_DURATION, VaryByParam = "id")]
        public PartialViewResult DebitsValueTrendChart(int id)
        {
            int monthSelected = 0;
            if (id > 0)
                monthSelected = -1 * id;

            DateTime currentDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(monthSelected);
            DateTime startDate = currentDate;
            DateTime endDate = currentDate.AddMonths(1).AddDays(-1);
            var db = new DBConnect();
            //Get data from db - query
            string query = $"select rbr_date,sum(dbt_amount) as 'Initial' from tbldebits left join tblrbr on rbr_id = dbt_rbr " +
                $"where rbr_status not in (99) " +
                $"and rbr_date between '{startDate.ToString("yyyy-MM-dd")}' and '{endDate.ToString("yyyy-MM-dd")}' group by rbr_date";
            DataSet result = db.Query(query);

            var listLabels = new List<string>();
            foreach (DataRow dr in result.Tables[0].Rows)
            {
                listLabels.Add(Convert.ToDateTime(dr["rbr_date"]).ToString(MD_CONST_FORMATS.LABEL_DATE_FORMAT));
            }
            string[] labels = listLabels.ToArray();

            var data = new List<ComplexDataset>();

            var submissionData = new List<double>();
            foreach (DataRow dr in result.Tables[0].Rows)
            {
                submissionData.Add((double)dr.Field<decimal>("Initial"));
            }
            data.Add(new ComplexDataset
            {

                Data = submissionData,
                Label = "Submissions",
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
            model.Title = $"Date Range {startDate.ToString("dd/MM/yyyy")} to {endDate.ToString("dd/MM/yyyy")}";
            model.ChartID = $"DebtorValueTrendChart{id}";
            return PartialView("LineChartPartial", model);

        }

        [OutputCache(Duration = MD_CONST_DURATIONS.OUTPUTCASH_DURATION, VaryByParam = "id")]
        public PartialViewResult SubmissionReceivedTrend(int id)
        {
            int monthSelected = 0;
            if (id > 0)
                monthSelected = -1 * id;


            DateTime currentDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(monthSelected);
            DateTime startDate = currentDate;
            DateTime endDate = currentDate.AddMonths(1).AddDays(-1);
            var db = new DBConnect();
            //Get data from db - query
            string query = $"select date_format(rbr_datetime_sub,'%Y-%m-%d') as 'Received Date', count(*) as 'Count', " +
                $"sum(rbr_total_records) as 'Total Records' from tblrbr where rbr_datetime_sub " +
                $"between '{startDate.ToString("yyyy-MM-dd")}' and '{endDate.ToString("yyyy-MM-dd")}' group by date_format(rbr_datetime_sub, '%Y-%m-%d')";

            DataSet result = db.Query(query);


            var listLabels = new List<string>();
            foreach (DataRow dr in result.Tables[0].Rows)
            {
                listLabels.Add(Convert.ToDateTime(dr["Received Date"]).ToString(MD_CONST_FORMATS.LABEL_DATE_FORMAT));
            }
            string[] labels = listLabels.ToArray();


            var data = new List<ComplexDataset>();

            var ReconData = new List<double>();
            foreach (DataRow dr in result.Tables[0].Rows)
            {
                //Type t = dr["Count"].GetType();
                ReconData.Add((int)dr.Field<Int64>("Count"));
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
            model.ChartID = $"ChartSubmissionReceivedTrend{id}";
            return PartialView("LineChartPartial", model);
        }
        [OutputCache(Duration = MD_CONST_DURATIONS.OUTPUTCASH_DURATION, VaryByParam = "id")]
        public PartialViewResult GetTopUnpaids(int id)
        {
            int monthSelected = 0;
            if (id > 0)
                monthSelected = -1 * id;
            DateTime currentDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(monthSelected);
            DateTime startDate = currentDate;
            DateTime endDate = currentDate.AddMonths(1).AddDays(-1);

            var db = new DBConnect();
            string query = "select hec_description as Description , hec_code as Code, count(*) as Count ,sum(dbt_amount) as amount from tbldebits left join tblhyphen_errcodes on dbt_accrejcode = hec_code left join tblrbr on rbr_id = dbt_rbr where dbt_pass_unpaid in (2,3) and rbr_status not in (99) " +
                $" and dbt_unpaid_datetime between '{startDate.ToString("yyyy-MM-dd")}' and '{endDate.ToString("yyyy-MM-dd")}' group by hec_description";
            var model = new List<ManagementDashboard.Models.GetTopUnpaids>();
            var result = db.Query(query);


            foreach (DataRow dRow in result.Tables[0].Rows)
            {
                var depMov = new Models.GetTopUnpaids();
                depMov.Description = dRow.Field<string>("Description");
                depMov.Code = dRow.Field<string>("Code");
                //Type t = dRow["Count"].GetType();
                depMov.Count = (int)dRow.Field<Int64>("Count");
                depMov.Amount = (int)dRow.Field<decimal>("amount");


                model.Add(depMov);

            }

            return PartialView(model);

        }



    }

}