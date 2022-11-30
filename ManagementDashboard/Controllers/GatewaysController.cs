using Chart.Mvc.ComplexChart;
using ManagementDashboard.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ManagementDashboard.Controllers
{

    [Authorize]
    public class GatewaysController : Controller
    {
        // GET: Gateways
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Summary()
        {
            return View();
        }


        [OutputCache(Duration = MD_CONST_DURATIONS.OUTPUTCASH_DURATION)]
        public ActionResult HyphenBatchesStates()
        {

            var cm = new Models.SQLReportTableViewModel();

            var db = new DBConnect();

            string file = Server.MapPath("~") + "SQLQueries\\HyphenBatchesStates.sql";
            if (System.IO.File.Exists(file))
            {
                StreamReader streamReader = new StreamReader(file);
                var fileContent = streamReader.ReadToEnd();

                var query = fileContent;

                var result = db.Query(query);
                string htmlTable = result.Tables[0].ConvertDataTableToHTML();
                cm.HtmlTable = htmlTable;

            }

            return View(cm);
        }
        //[OutputCache("OverViewData",60,System.Web.UI.OutputCacheLocation.Server,false,)]
        [OutputCache(Duration = MD_CONST_DURATIONS.ONE_HOUR, VaryByParam = "id", Location = System.Web.UI.OutputCacheLocation.Client)]
        public DataSet GetData(int bookId, DateTime startDate, DateTime endDate)
        {
            string query = GetOverReportQuery(bookId, startDate, endDate);
            var db = new DBConnect();
            return db.Query(query);

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
        [OutputCache(Duration = MD_CONST_DURATIONS.OUTPUTCASH_DURATION)]
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
        [OutputCache(Duration = MD_CONST_DURATIONS.OUTPUTCASH_DURATION, VaryByParam = "id;startDate;endDate")]
        private string GetOverReportQuery(int bookId, DateTime startDate, DateTime endDate)
        {
                if (bookId < 1)
                    bookId= 1;

            string fulcrumCustomerString = ConcatStringMysql(GetFulcrumCustomerRefernces());

            string query = "select " +
                "date_format(dbt_date, '%Y-%m') as 'YearMonth', " +
                "count(*) as 'Initial Records', " +
                "sum(dbt_amount) as 'Initial Amount', " +
                "sum(if (dbt_cdv = 4,1,0)) + sum(if (dbt_cdv = 3 and dbt_accrej = 4,1,0)) as 'Reject Records', " +
                "sum(if (dbt_cdv = 4,dbt_amount,0)) +sum(if (dbt_cdv = 3 and dbt_accrej = 4, dbt_amount,0))  as 'Reject Amount'," +
                "sum(if (dbt_cdv = 3 and dbt_accrej = 3 and dbt_pass_unpaid > 0,1,0)) as 'Collection Records', " +
                "sum(if (dbt_cdv = 3 and dbt_accrej = 3 and dbt_pass_unpaid > 0,dbt_amount,0)) as 'Collection Amount'," +
                "sum(if (dbt_cdv = 3 and dbt_accrej = 3 and dbt_pass_unpaid > 1 and dbt_accrejcode not in (4, 30, 34, 36, 88),1,0)) as 'Unpaids Records', " +
                "sum(if (dbt_cdv = 3 and dbt_accrej = 3 and dbt_pass_unpaid > 1  and dbt_accrejcode not in (4, 30, 34, 36, 88),dbt_amount,0)) as 'Unpaids Amount'," +
                "sum(if (dbt_cdv = 3 and dbt_accrej = 3 and dbt_pass_unpaid > 1 and dbt_accrejcode  in (4, 30, 34, 36, 88),1,0)) as 'Disputes Records', " +
                "sum(if (dbt_cdv = 3 and dbt_accrej = 3 and dbt_pass_unpaid > 1  and dbt_accrejcode  in (4, 30, 34, 36, 88),dbt_amount,0)) as 'Disputes Amount' " +
                "from tbldebits " +
                "   left join tblrbr on rbr_id = dbt_rbr " +
                "   left join tblcompany on com_ref = dbt_comref " +
                $"where " +
                $"dbt_date between '{endDate.ToString("yyyy-MM-dd")}' and '{startDate.ToString("yyyy-MM-dd")}' and " +
                $"rbr_status in (0, 1, 2, 3, 4) " +
                $"and com_service_book_type_id  = {bookId} " +
                "group by date_format(dbt_date, '%Y-%m')";
            return query;
        }



        //[OutputCache(Duration = MDConst.OUTPUTCASH_DURATION, VaryByParam = "id")]
        public PartialViewResult OverviewReportRecords(int id)
        {
            DateTime currentDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddDays(-1);
            DateTime endDateCalculation = new DateTime(currentDate.Year, currentDate.Month, 1).AddMonths(-13);

            DateTime startDate = currentDate.AddMonths(1);
            DateTime endDate = endDateCalculation;

            //Labels
            DataSet result = GetData(id, startDate, endDate);

            var listLabels = new List<string>();
            //listLabels.Add("Date");
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
                // Type t = dr["YearMonth"].GetType();                

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


            var model = OverviewReportChartModel(labels, data, startDate, endDate, id);
            return PartialView("BarChartPartial", model);

        }

        public ChartModel OverviewReportChartModel(string[] labels, List<ComplexDataset> data, DateTime startDate, DateTime endDate, int id)
        {
            ManagementDashboard.Models.ChartModel model = new ChartModel();
            model.Labels = labels;
            model.ComplexDatasets = data;
            model.Title = $"Overview {endDate.ToString("dd-MM-yyyy")} to {startDate.ToString("dd-MM-yyyy")}";
            //model.DataTableItems = ;
            model.ChartID = $"OverViewChart{id}";
            model.ShowTable = true;
            model.ChartHeight = 100;
            model.Responsive = true;
            model.MaintainAspectRatio = true;
            model.ScaleBeginAtZero = false;
            return model;
        }


        [OutputCache(Duration = MD_CONST_DURATIONS.OUTPUTCASH_DURATION, VaryByParam = "id")]
        public PartialViewResult OverviewReportValues(int id)
        {
            DateTime currentDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddDays(-1);
            DateTime endDateCalculation = new DateTime(currentDate.Year, currentDate.Month, 1).AddMonths(-13);

            DateTime startDate = currentDate.AddMonths(1);
            DateTime endDate = endDateCalculation;

            //Labels
            DataSet result = GetData(id, startDate, endDate);

            var listLabels = new List<string>();
            listLabels.Add("Initial Amount");
            listLabels.Add("Reject Amount");
            listLabels.Add("Collection Amount");
            listLabels.Add("Unpaids Amount");
            listLabels.Add("Dispute Amount");
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

                //Type t = dr["Initial Records"].GetType();

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

            var model = OverviewReportChartModel(labels, data, startDate, endDate, id);

            return PartialView("BarChartPartial", model);

        }
        [OutputCache(Duration = MD_CONST_DURATIONS.OUTPUTCASH_DURATION)]
        public ActionResult LoadedGateway()
        {

            var vm = new Models.SQLReportTableViewModel();

            var db = new DBConnect();

            string file = Server.MapPath("~") + "SQLQueries\\LoadedGateway.sql";
            if (System.IO.File.Exists(file))
            {
                StreamReader streamReader = new StreamReader(file);
                var fileContent = streamReader.ReadToEnd();

                var query = fileContent;

                var result = db.Query(query);
                string htmlTable = result.Tables[0].ConvertDataTableToHTML();
                vm.HtmlTable = htmlTable;

            }

            return View(vm);
        }



        public string GetRGBAToString(Color color, decimal alpha)
        {
            return $"rgba({color.R},{color.G},{color.B},{alpha})";
        }

        public Color ChangeColorBrightness(Color color, float correctionFactor)
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

    }

}