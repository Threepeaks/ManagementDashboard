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

    public class Demo
    {
        public string Name { get; set; }
    }

    public class TPMSController : Controller
    {

        private const string LABEL_DATE_FORMAT = "dd-MM";

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
                listLabels.Add(Convert.ToDateTime(dr["rbr_date"]).ToString(LABEL_DATE_FORMAT));
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
            model.ComplesDatasets = data;
            model.Title = $"Date Range {startDate.ToString("dd-MM-yyyy")} to {endDate.ToString("dd-MM-yyyy")}";
            model.ChartID = $"DebtorRecordsTrendChart{id}";
            return PartialView("LineChartPartial", model);

        }
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
                listLabels.Add(Convert.ToDateTime(dr["rbr_date"]).ToString(LABEL_DATE_FORMAT));
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
            model.ComplesDatasets = data;
            model.Title = $"Date Range {startDate.ToString("dd/MM/yyyy")} to {endDate.ToString("dd/MM/yyyy")}";
            model.ChartID = $"DebtorValueTrendChart{id}";
            return PartialView("LineChartPartial", model);

        }


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
                listLabels.Add(Convert.ToDateTime(dr["rbr_date"]).ToString(LABEL_DATE_FORMAT));
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
            model.ComplesDatasets = data;
            model.Title = $"Date Range {startDate.ToString("dd/MM/yyyy")} to {endDate.ToString("dd/MM/yyyy")}";
            model.ChartID = $"chart{id}";
            return PartialView("LineChartPartial", model);
        }


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
                listLabels.Add(Convert.ToDateTime(dr["groupDate"]).ToString(LABEL_DATE_FORMAT));
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
            model.ComplesDatasets = data;
            model.Title = $"Date Range {startDate.ToString("dd-MM-yyyy")} to {endDate.ToString("dd-MM-yyyy")}";
            
            model.ChartID = $"TrendPaymentValue{id}";
            model.ShowTable = false;
            return PartialView("LineChartPartial", model);
        }


        private List<string> GetFulcrumCustomerRefernces()
        {
            var db = new DBConnect();
            //Get data from db - query
            string query = $"select customer from tbl_gateway_fulcrum_client";
            DataSet result = db.Query(query);
            var customers = new List<string>();
            foreach (DataRow dr in result.Tables[0].Rows)
            {
                customers.Add(dr.Field<string>("customer"));
            }
            return customers;
        }

        private string ConcatStringMysql(List<string> list)
        {
            var sb = new List<string>();
            foreach (var item in list)
            {
                sb.Add("'" + item + "'");
            }
            return string.Join(",", sb);
        }
        private string GetOverReportQuery(int id,DateTime startDate, DateTime endDate )
        {

            string InOrNot = "not in";
            if (id == 0) // Hyphen
                InOrNot = "not in";
            if (id == 1) // Fulcrum
                InOrNot = "in";


            string fulcrumCustomerString = ConcatStringMysql(GetFulcrumCustomerRefernces());

            string query = "select date_format(dbt_date, '%Y-%m') as 'YearMonth', count(*) as 'Initial Records', sum(dbt_amount) as 'Initial Amount', sum(if (dbt_cdv = 4,1,0)) +sum(if (dbt_cdv = 3 and dbt_accrej = 4,1,0))as 'Reject Records', sum(if (dbt_cdv = 4,dbt_amount,0)) +sum(if (dbt_cdv = 3 and dbt_accrej = 4, dbt_amount,0))  as 'Reject Amount'  ,sum(if (dbt_cdv = 3 and dbt_accrej = 3 and dbt_pass_unpaid > 0,1,0)) as 'Collection Records', sum(if (dbt_cdv = 3 and dbt_accrej = 3 and dbt_pass_unpaid > 0,dbt_amount,0)) as 'Collection Amount' ,sum(if (dbt_cdv = 3 and dbt_accrej = 3 and dbt_pass_unpaid > 1 and dbt_accrejcode not in (4, 30, 34, 36, 88),1,0)) as 'Unpaids Records', sum(if (dbt_cdv = 3 and dbt_accrej = 3 and dbt_pass_unpaid > 1  and dbt_accrejcode not in (4, 30, 34, 36, 88),dbt_amount,0)) as 'Unpaids Amount'  ,sum(if (dbt_cdv = 3 and dbt_accrej = 3 and dbt_pass_unpaid > 1 and dbt_accrejcode  in (4, 30, 34, 36, 88),1,0)) as 'Disputes Records', sum(if (dbt_cdv = 3 and dbt_accrej = 3 and dbt_pass_unpaid > 1  and dbt_accrejcode  in (4, 30, 34, 36, 88),dbt_amount,0)) as 'Disputes Amount' from tbldebits left join tblrbr on rbr_id = dbt_rbr " +
                $"where dbt_date between '{endDate.ToString("yyyy-MM-dd")}' and '{startDate.ToString("yyyy-MM-dd")}' and rbr_status in (0, 1, 2, 3, 4) " +
                
                $"and rbr_comref {InOrNot} ({fulcrumCustomerString}) " +

                "group by date_format(dbt_date, '%Y-%m')";
            return query;
        }

        public PartialViewResult OverviewReportRecords(int id)
        {
            DateTime currentDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddDays(-1);
            DateTime endDateCalculation = new DateTime(currentDate.Year, currentDate.Month, 1).AddMonths(-13);

            DateTime startDate = currentDate.AddMonths(1);
            DateTime endDate = endDateCalculation;
            
            

            //Labels
            DataSet result = GetData(id, startDate, endDate);
            var listLabels = new List<string>();
                listLabels.Add("Initial Records");
                listLabels.Add("Reject Records");
                listLabels.Add("Collection Records");
                listLabels.Add("Unpaids Records");
                listLabels.Add("Disputes Records");
            listLabels.Add("Unp R");
            listLabels.Add("Dis R");
            string[] labels = listLabels.ToArray();
            
            var data = new List<ComplexDataset>();
            
            float correctionFactor = 0.10F;
            
            Color fillColor = Color.FromArgb(151, 187, 205);
            Color strokeColor = Color.FromArgb(151, 187, 205);
            Color pointColor = Color.FromArgb(151, 187, 205, 1);

            foreach (DataRow dr in result.Tables[0].Rows)
            {
                var ReconData = new List<double>();

                Type t = dr["Initial Records"].GetType();
                ReconData.Add((double)dr.Field<Int64>("Initial Records"));
                ReconData.Add((double)dr.Field<decimal>("Reject Records"));
                ReconData.Add((double)dr.Field<decimal>("Collection Records"));
                ReconData.Add((double)dr.Field<decimal>("Unpaids Records"));
                ReconData.Add((double)dr.Field<decimal>("Disputes Records"));
                var unpaidRatio = ((double)dr.Field<decimal>("Unpaids Records") / (double)dr.Field<decimal>("Collection Records")) * 100;
                ReconData.Add(unpaidRatio);
                var disputeRatio = ((double)dr.Field<decimal>("Disputes Records") / (double)dr.Field<decimal>("Collection Records")) * 100;
                ReconData.Add(disputeRatio);

                data.Add(new ComplexDataset
                {

                    Data = ReconData,
                    Label = dr.Field<string>("YearMonth"),
                    FillColor = GetRGBAToString(fillColor,0.2M),
                    StrokeColor = GetRGBAToString(strokeColor, 1),
                    PointColor = GetRGBAToString(pointColor, 1),
                    PointStrokeColor = "#fff",
                    PointHighlightFill = "#fff",
                    PointHighlightStroke = "rgba(151,187,205,1)",
                });
                fillColor = ChangeColorBrightness(fillColor, correctionFactor);
                strokeColor = ChangeColorBrightness(strokeColor, correctionFactor);
                pointColor = ChangeColorBrightness(pointColor, correctionFactor);



            }


            ManagementDashboard.Models.ChartModel model = new ChartModel();
            model.Labels = labels;
            model.ComplesDatasets = data;
            model.Title = $"Overview {endDate.ToString("dd-MM-yyyy")} to {startDate.ToString("dd-MM-yyyy")}";

            model.ChartID = $"OverViewChart{id}";
            model.ShowTable = true;
            return PartialView("BarChartPartial", model);

        }

        //[OutputCache("OverViewData",60,System.Web.UI.OutputCacheLocation.Server,false,)]
        [OutputCache(Duration = 60, VaryByParam = "id", Location = System.Web.UI.OutputCacheLocation.Client)]
        public DataSet GetData(int id,DateTime startDate,DateTime endDate)
        {
            string query = GetOverReportQuery(id, startDate, endDate);
            var db = new DBConnect();
            return db.Query(query);

        }


        public PartialViewResult OverviewReportValues(int id)
        {
            DateTime currentDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddDays(-1);
            DateTime endDateCalculation = new DateTime(currentDate.Year, currentDate.Month, 1).AddMonths(-13);

            DateTime startDate = currentDate.AddMonths(1);
            DateTime endDate = endDateCalculation;

            //Labels
            DataSet result = GetData(id,startDate,endDate);

            var listLabels = new List<string>();
            listLabels.Add("Initial Amount");
            listLabels.Add("Reject Amount");
            listLabels.Add("Collection Amount");
            listLabels.Add("Unpaids Amount");
            listLabels.Add("Dispites Amount");
            listLabels.Add("Unp R");
            listLabels.Add("Dis R");
            string[] labels = listLabels.ToArray();

            var data = new List<ComplexDataset>();

            float correctionFactor = 0.10F;

            Color fillColor = Color.FromArgb(151, 187, 205);
            Color strokeColor = Color.FromArgb(151, 187, 205);
            Color pointColor = Color.FromArgb(151, 187, 205);

            foreach (DataRow dr in result.Tables[0].Rows)
            {
                var ReconData = new List<double>();

                Type t = dr["Initial Records"].GetType();
                ReconData.Add((double)dr.Field<decimal>("Initial Amount"));
                ReconData.Add((double)dr.Field<decimal>("Reject Amount"));
                ReconData.Add((double)dr.Field<decimal>("Collection Amount"));
                ReconData.Add((double)dr.Field<decimal>("Unpaids Amount"));
                ReconData.Add((double)dr.Field<decimal>("Disputes Amount"));
                var unpaidRatio = ((double)dr.Field<decimal>("Unpaids Amount") / (double)dr.Field<decimal>("Collection Amount")) * 100;
                ReconData.Add(unpaidRatio);
                var disputeRatio = ((double)dr.Field<decimal>("Disputes Amount") / (double)dr.Field<decimal>("Collection Amount")) * 100;
                ReconData.Add(disputeRatio);


                data.Add(new ComplexDataset
                {

                    Data = ReconData,
                    Label = dr.Field<string>("YearMonth"),
                    FillColor = GetRGBAToString(fillColor, 0.2M),
                    StrokeColor = GetRGBAToString(strokeColor, 1),
                    PointColor = GetRGBAToString(pointColor, 1),
                    PointStrokeColor = "#fff",
                    PointHighlightFill = "#fff",
                    PointHighlightStroke = "rgba(151,187,205,1)",
                });
                fillColor = ChangeColorBrightness(fillColor, correctionFactor);
                strokeColor = ChangeColorBrightness(strokeColor, correctionFactor);
                pointColor = ChangeColorBrightness(pointColor, correctionFactor);



            }


            ManagementDashboard.Models.ChartModel model = new ChartModel();
            model.Labels = labels;
            model.ComplesDatasets = data;
            model.Title = $"Overview {endDate.ToString("dd-MM-yyyy")} to {startDate.ToString("dd-MM-yyyy")}";

            model.ChartID = $"OverViewChartValue{id}";
            model.ShowTable = true;
            return PartialView("BarChartPartial", model);

        }
        public string GetRGBAToString(Color color,decimal alpha)
        { 
            return $"rgba({color.R},{color.G},{color.B},{alpha})";
        }

        public  Color ChangeColorBrightness(Color color, float correctionFactor)
        {
            float red = (float)color.R;
            float green = (float)color.G;
            float blue = (float)color.B;

            if (correctionFactor < 0)
            {
                correctionFactor = 1 + correctionFactor;
                red *= correctionFactor;
                green *= correctionFactor;
                blue *= correctionFactor;
            }
            else
            {
                red = (255 - red) * correctionFactor + red;
                green = (255 - green) * correctionFactor + green;
                blue = (255 - blue) * correctionFactor + blue;
            }

            return Color.FromArgb(color.A, (int)red, (int)green, (int)blue);
        }


        public PartialViewResult NewClientByIndustry(int id)
        {
            int monthSelected = 0;
            if (id > 0)
                monthSelected = -1 * id;

            DateTime currentDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(monthSelected);
            DateTime startDate = currentDate;
            DateTime endDate = currentDate.AddMonths(1).AddDays(-1);
            var db = new DBConnect();
            //Get data from db - query
            string query = $"SELECT count(*) as 'Count', srv_name as 'Industry' FROM threepeaks_tpms.tblcompany left join tblservice on com_serviceid = srv_id " +
                $"where com_startdate between '{startDate.ToString("yyyy-MM-dd")}' and '{endDate.ToString("yyyy-MM-dd")}' group by srv_name";
            DataSet result = db.Query(query);

            var listLabels = new List<string>();
            foreach (DataRow dr in result.Tables[0].Rows)
            {
                listLabels.Add(dr.Field<string>("Industry"));
            }
            string[] labels = listLabels.ToArray();

            var data = new List<ComplexDataset>();

            var submissionData = new List<double>();
            foreach (DataRow dr in result.Tables[0].Rows)
            {
                //Type t = dr["Count"].GetType();
                submissionData.Add((int)dr.Field<Int64>("Count"));
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
            model.ComplesDatasets = data;
            model.Title = $"Date Range {startDate.ToString("dd/MM/yyyy")} to {endDate.ToString("dd/MM/yyyy")}";
            model.ChartID = $"ChartNewClientByIndustry{id}";
            model.ChartHeight = 350;
            return PartialView("BarChartPartial", model);

        }

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
                listLabels.Add(Convert.ToDateTime(dr["Received Date"]).ToString(LABEL_DATE_FORMAT));
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
            model.ComplesDatasets = data;
            model.Title = $"Date Range {startDate.ToString("dd-MM-yyyy")} to {endDate.ToString("dd-MM-yyyy")}";
            model.ChartID = $"ChartSubmissionReceivedTrend{id}";            
            return PartialView("LineChartPartial", model);
        }
    }
}