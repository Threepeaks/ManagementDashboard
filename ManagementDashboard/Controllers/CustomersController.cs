using Chart.Mvc.ComplexChart;
using ManagementDashboard.Models;
using NLog;
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
    public class CustomersController : Controller
    {
        public object debMov { get; private set; }

        // GET: Customers
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Dormant()
        {
            return View();
        }

        public ActionResult TopStats()
        {
            return View();
        }
        public ActionResult NewCustomers()
        {
            return View();
        }

        [OutputCache(Duration = MD_CONST_DURATIONS.OUTPUTCASH_DURATION)]
        public ActionResult CustomerComments()
        {

            var cm = new Models.SQLReportTableViewModel();

            var db = new DBConnect();

            string file = Server.MapPath("~") + "SQLQueries\\CustomerComments.sql";
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

        [OutputCache(Duration = MD_CONST_DURATIONS.OUTPUTCASH_DURATION)]
        public ActionResult CustomerPaymentBlocked()
        {

            var vm = new Models.SQLReportTableViewModel();
            var db = new DBConnect();

            string file = Server.MapPath("~") + "SQLQueries\\CustomerPaymentBlocked.sql";
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


        [OutputCache(Duration = MD_CONST_DURATIONS.OUTPUTCASH_DURATION)]
        public ActionResult CustomerBlockedProcessing()
        {

            var vm = new Models.SQLReportTableViewModel();
            var db = new DBConnect();

            string file = Server.MapPath("~") + "SQLQueries\\CustomerBlockedProcessing.sql";
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
        [OutputCache(Duration = MD_CONST_DURATIONS.OUTPUTCASH_DURATION, VaryByParam = "id")]
        public PartialViewResult GetTopValueCustomers(int id)
        {
            int monthSelected = 0;
            if (id > 0)
                monthSelected = -1 * id;
            DateTime currentDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(monthSelected);
            DateTime startDate = currentDate;
            DateTime endDate = currentDate.AddMonths(1).AddDays(-1);

            var db = new DBConnect();
            string query = "select dbt_comref, sum(dbt_amount) as initial, count(*) as c from tbldebits  left join tblrbr on rbr_id = dbt_rbr where dbt_date between" +
                $"'{startDate.ToString("yyyy-MM-dd")}' and '{endDate.ToString("yyyy-MM-dd")}' and rbr_status not in (99) group by dbt_comref order by initial desc limit 10";
            var model = new List<ManagementDashboard.Models.GetTopValueCustomer>();
            var result = db.Query(query);

            foreach (DataRow dRow in result.Tables[0].Rows)
            {
                var topCus = new Models.GetTopValueCustomer();
                topCus.Customer = dRow.Field<string>("dbt_comref");
                topCus.Collections = (decimal)dRow.Field<decimal>("initial");
                topCus.NumOfRecords = (int)dRow.Field<Int64>("c");

                model.Add(topCus);

            }

            return PartialView(model);

        }


        [OutputCache(Duration = MD_CONST_DURATIONS.OUTPUTCASH_DURATION, VaryByParam = "id")]
        public PartialViewResult GetTopCustomerRecords(int id)
        {
            int monthSelected = 0;
            if (id > 0)
                monthSelected = -1 * id;
            DateTime currentDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(monthSelected);
            DateTime startDate = currentDate;
            DateTime endDate = currentDate.AddMonths(1).AddDays(-1);

            var db = new DBConnect();
            string query = "select dbt_comref, sum(dbt_amount) as initial, count(*) as c from tbldebits  left join tblrbr on rbr_id = dbt_rbr where dbt_date between" +
                $"'{startDate.ToString("yyyy-MM-dd")}' and '{endDate.ToString("yyyy-MM-dd")}' and rbr_status not in (99) group by dbt_comref order by c desc limit 10";
            var model = new List<ManagementDashboard.Models.GetTopValueCustomer>();
            var result = db.Query(query);

            foreach (DataRow dRow in result.Tables[0].Rows)
            {
                var topCus = new Models.GetTopValueCustomer();
                topCus.Customer = dRow.Field<string>("dbt_comref");
                topCus.Collections = (decimal)dRow.Field<decimal>("initial");
                topCus.NumOfRecords = (int)dRow.Field<Int64>("c");

                model.Add(topCus);

            }

            return PartialView(model);
        }

        [OutputCache(Duration = MD_CONST_DURATIONS.OUTPUTCASH_DURATION, VaryByParam = "id")]
        public PartialViewResult DormantClients(int id)
        {
            int monthSelected = 0;
            if (id > 0)
                monthSelected = -1 * id;
            DateTime currentDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(monthSelected);
            DateTime startDate = currentDate;
            DateTime endDate = currentDate.AddMonths(1).AddDays(-1);
            DateTime today = DateTime.Now.Date; //Filter for current Date
            int span = 45;

            var db = new DBConnect();
            string query = $"select com_ref as 'Ref', case com_acc_cancel when 0 then if (com_ac_pending=1, if(com_acc_pending_flagdate < '{startDate.ToString("yyyy-MM-dd")}','Pending Ended', " +
                $"if (com_ac_pending_date > '{endDate.ToString("yyyy-MM-dd")}','Active', 'Pending')), 'Active') when 1 then 'In Cancellation' when 2 then " +
                $"if (com_acc_cancel_enddate < '{startDate.ToString("yyyy-MM-dd")}','Cancelled Before', if (com_acc_cancel_date < '{startDate.ToString("yyyy-MM-dd")}', " +
                $"'In Cancellation', if (com_acc_cancel_date < '{endDate.ToString("yyyy-MM-dd")}' ,'In Cancellation','Active'))) else -1 end as 'State', " +
                $"(select max(rbr_date) from tblrbr where rbr_status not in (99) and rbr_comref = com_ref and rbr_date< '{startDate.ToString("yyyy-MM-dd")}') as 'Prev Run', " +
                $"ifnull((select min(rbr_date) from tblrbr where rbr_status not in (99) and rbr_comref = com_ref and rbr_date between '{startDate.ToString("yyyy-MM-dd")}' " +
                $"and '{endDate.ToString("yyyy-MM-dd")}'),'na') as 'Next',  datediff((select max(rbr_date) from tblrbr where rbr_status not in (99) and  rbr_comref = com_ref " +
                $"and rbr_date < '{startDate.ToString("yyyy-MM-dd")}'), '{today.ToString("yyyy-MM-dd")}') as 'span' from tblcompany where (select min(rbr_date) from tblrbr " +
                $"where rbr_status not in (99) and rbr_comref = com_ref) < '{startDate.ToString("yyyy-MM-dd")}' and(select count(rbr_id) from tblrbr where rbr_status not in " +
                $"(99) and  rbr_comref = com_ref and rbr_date between '{startDate.ToString("yyyy-MM-dd")}' and '{endDate.ToString("yyyy-MM-dd")}') = 0 and if (com_acc_cancel = 2 , " +
                $"if (com_acc_cancel_enddate < '{startDate.ToString("yyyy-MM-dd")}', 0,if (com_acc_cancel_date < '{startDate.ToString("yyyy-MM-dd")}', 1, " +
                $"if (com_acc_cancel_date < '{endDate.ToString("yyyy-MM-dd")}' ,1,1))),1) = 1 and com_acc_cancel = 0 and com_ac_pending = 0 and datediff((select max(rbr_date) " +
                $" from tblrbr where rbr_status not in (99) and rbr_comref = com_ref and rbr_date< '{startDate.ToString("yyyy-MM-dd")}'), " +
                $"'{endDate.ToString("yyyy-MM-dd")}') <= (-1 * '{span}') order by span, ifnull((select min(rbr_date) from tblrbr where rbr_status not in (99) " +
                $"and  rbr_comref = com_ref and rbr_date > '{endDate.ToString("yyyy-MM-dd")}'),'0000-00-00'), (select max(rbr_date) from tblrbr  where rbr_status not in " +
                $"(99) and rbr_comref = com_ref and rbr_date< '{startDate.ToString("yyyy-MM-dd")}'), com_ref";

            var model = new List<ManagementDashboard.Models.DormantClients>();
            var result = db.Query(query);

            foreach (DataRow dRow in result.Tables[0].Rows)
            {
                var domCus = new Models.DormantClients();
                domCus.Ref = dRow.Field<string>("Ref");
                domCus.State = dRow.Field<string>("State");
                domCus.PrevRun = (DateTime)dRow.Field<DateTime>("Prev Run");
                domCus.Next = dRow.Field<string>("Next");
                Type t = dRow["span"].GetType();
                domCus.Span = (int)dRow.Field<Int32>("span");
                if (domCus.Span < -45)
                    model.Add(domCus);
            }
            return PartialView(model);
        }
        [OutputCache(Duration = MD_CONST_DURATIONS.OUTPUTCASH_DURATION, VaryByParam = "id")]
        public PartialViewResult HighestCustomerValue(int id)
        {
            int monthSelected = 0;
            if (id > 0)
                monthSelected = -1 * id;
            DateTime currentDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(monthSelected);
            DateTime startDate = currentDate;
            DateTime endDate = currentDate.AddMonths(1).AddDays(-1);

            var db = new DBConnect();
            string query = "select dbt_comref as 'Ref', count(*) as 'Count', sum(dbt_amount) as 'Value' from tbldebits " +
                $"left join tblrbr on rbr_id = dbt_rbr where dbt_date between '{startDate.ToString("yyyy-MM-dd")}' and" +
                $"'{endDate.ToString("yyyy-MM-dd")}' " +
                $"and rbr_status<> 99 and dbt_cdv = 3 and dbt_accrej = 3 group by dbt_comref order by count(*) desc limit 20";
            var model = new List<ManagementDashboard.Models.HighestCustomerValue>();
            var result = db.Query(query);

            foreach (DataRow dRow in result.Tables[0].Rows)
            {
                var highCus = new Models.HighestCustomerValue();
                highCus.Ref = dRow.Field<string>("Ref");
                highCus.Count = (int)dRow.Field<Int64>("Count");
                highCus.Value = (decimal)dRow.Field<decimal>("Value");
                model.Add(highCus);
            }
            return PartialView(model);

        }

        [OutputCache(Duration = MD_CONST_DURATIONS.OUTPUTCASH_DURATION, VaryByParam = "id")]
        public PartialViewResult uManageCustomers(int id)
        {
            int monthSelected = 0;
            if (id > 0)
                monthSelected = -1 * id;
            DateTime currentDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(monthSelected);
            DateTime startDate = currentDate;
            DateTime endDate = currentDate.AddMonths(1).AddDays(-1);

            var db = new DBConnect();
            string query = "select rbr_comref as 'Customer Refernece', com_name as 'Company', count(*) as 'No Of Runs', " +
                "sum(rbr_total_records) as 'Total Records' from tblrbr left join tblcompany on com_ref = rbr_comref where " +
                $"rbr_subtype = 'File' and rbr_status not in (99) and rbr_date between '{startDate.ToString("yyyy-MM-dd")}' and '{endDate.ToString("yyyy-MM-dd")}' group by rbr_comref ";
            var model = new List<ManagementDashboard.Models.uManageCustomers>();
            var result = db.Query(query);

            foreach (DataRow dRow in result.Tables[0].Rows)
            {
                var uMan = new Models.uManageCustomers();
                uMan.Ref = dRow.Field<string>("Customer Refernece");
                uMan.Customer = dRow.Field<string>("Company");
                uMan.NoRuns = (int)dRow.Field<Int64>("No Of Runs");
                uMan.TotalRecords = (int)dRow.Field<decimal>("Total Records");
                model.Add(uMan);
            }
            return PartialView(model);

        }

        [OutputCache(Duration = MD_CONST_DURATIONS.OUTPUTCASH_DURATION, VaryByParam = "id")]
        public PartialViewResult NewClients(int id)
        {
            int monthSelected = 0;
            if (id > 0)
                monthSelected = -1 * id;
            DateTime currentDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(monthSelected);
            DateTime startDate = currentDate;
            DateTime endDate = currentDate.AddMonths(1).AddDays(-1);

            var db = new DBConnect();
            string query = "SELECT com_ref as 'Customer Refernece', com_name as 'Company', com_startdate as 'Start Date'  FROM " +
                $"threepeaks_tpms.tblcompany where com_startdate between '{startDate.ToString("yyyy-MM-dd")}' and '{endDate.ToString("yyyy-MM-dd")}'";
            var model = new List<ManagementDashboard.Models.NewClients>();
            var result = db.Query(query);

            foreach (DataRow dRow in result.Tables[0].Rows)
            {
                var nClient = new Models.NewClients();
                nClient.Ref = dRow.Field<string>("Customer Refernece");
                nClient.Customer = dRow.Field<string>("Company");
                nClient.StartDate = (DateTime)dRow.Field<DateTime>("Start Date");
                model.Add(nClient);
            }
            return PartialView(model);

        }


        [OutputCache(Duration = MD_CONST_DURATIONS.OUTPUTCASH_DURATION, VaryByParam = "id")]
        public PartialViewResult NoRunClients(int id)
        {
            int monthSelected = 0;
            if (id > 0)
                monthSelected = -1 * id;

            DateTime currentDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(monthSelected);
            DateTime startDate = currentDate;
            DateTime endDate = startDate.AddMonths(1).AddDays(-1);

            var db = new DBConnect();
            string query = "select com_ref as Ref ,com_startdate as StartDate, com_name as Customer from tblcompany" +
                " where com_acc_cancel in (0,1) and (select count(*) from tblrbr where rbr_comref = com_ref and " +
                $"rbr_status not in (99) and rbr_date <= '{endDate.ToString("yyyy-MM-dd")}' limit 1) = 0 ";

            /* string query = "select com_ref as Ref ,com_startdate as StartDate, com_name as Customer from tblcompany" +
                " where com_acc_cancel in (0,1) and (select count(*) from tblrbr where rbr_comref = com_ref and " +
                $"rbr_status not in (99) and rbr_date <= '{endDate.ToString("yyyy-MM-dd")}' limit 1) = 0 and com_startdate > '{startDate.ToString("yyyy-MM-dd")}'";*/

            List<string> ignoreCustomers = new List<string>() { "ZZZZZSD018", "ZZZZZRD002", "ZZZZZOD008", "SA00000001" , 
                "ZZZZZDD009", "DEMO", "3PXERO" ,"TPSA000002"};

            var model = new List<ManagementDashboard.Models.NoRunClients>();
            var result = db.Query(query);

            foreach (DataRow dRow in result.Tables[0].Rows)
            {
                string clientReference = dRow.Field<string>("Ref");
                if (ignoreCustomers.Contains(clientReference))
                    continue;

                var depMov = new Models.NoRunClients();
                
                depMov.Ref = clientReference;
                depMov.StartDate = (DateTime)dRow.Field<DateTime>("StartDate");
                depMov.Customer = dRow.Field<string>("Customer");
                depMov.Days = (int)(DateTime.Now.Date - depMov.StartDate).TotalDays;
                model.Add(depMov);
            }
            ViewBag.Date = startDate.ToString("MMMMMM yyyy");
            return PartialView(model);

        }


        [OutputCache(Duration = MD_CONST_DURATIONS.OUTPUTCASH_DURATION, VaryByParam = "id")]
        public PartialViewResult CanceledClients(int id)
        {
            int monthSelected = 0;
            if (id > 0)
                monthSelected = -1 * id;
            DateTime currentDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(monthSelected);
            DateTime startDate = currentDate;
            DateTime endDate = currentDate.AddMonths(1).AddDays(-1);

            var db = new DBConnect();
            string query = "select com_ref as 'Ref' , com_name as 'Customer', com_acc_cancel_date as 'Cancel Start',com_acc_cancel_enddate as 'Cancel End', com_acc_cancel_note as 'Comment' " +
                $"FROM threepeaks_tpms.tblcompany where com_acc_cancel = 2 and com_acc_cancel_enddate between '{startDate.ToString("yyyy-MM-dd")}' and '{endDate.ToString("yyyy-MM-dd")}'";
            var model = new List<ManagementDashboard.Models.CanceledClients>();
            var result = db.Query(query);

            foreach (DataRow dRow in result.Tables[0].Rows)
            {
                var depMov = new Models.CanceledClients();
                depMov.Ref = dRow.Field<string>("Ref");
                depMov.Customer = dRow.Field<string>("Customer");
                depMov.CancelStart = (DateTime)dRow.Field<DateTime>("Cancel Start");
                depMov.CancelEnd = (DateTime)dRow.Field<DateTime>("Cancel End");
                depMov.Comment = dRow.Field<string>("Comment");
                model.Add(depMov);
            }
            return PartialView(model);

        }


        [OutputCache(Duration = MD_CONST_DURATIONS.OUTPUTCASH_DURATION, VaryByParam = "id")]
        public PartialViewResult InCancelation(int id)
        {
            int monthSelected = 0;
            if (id > 0)
                monthSelected = -1 * id;
            DateTime currentDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(monthSelected);
            DateTime startDate = currentDate;
            DateTime endDate = currentDate.AddMonths(1).AddDays(-1);

            var db = new DBConnect();
            string query = "select com_ref as 'Customer Ref', com_name as 'Company Name', case com_ac_pending " +
                "when 0 then '' when 1 then 'Pending' when 2 then 'Pending' end as 'Pending Status', case com_acc_cancel " +
                "when 0 then 'Active' when 1 then 'Cancelling' when 2 then 'Cancelled' else '' end as 'Cancel Status', " +
                "com_acc_cancel_note as 'Comment' from `tblcompany` where com_acc_cancel = 1";

            var model = new List<ManagementDashboard.Models.InCancelation>();
            var result = db.Query(query);

            foreach (DataRow dRow in result.Tables[0].Rows)
            {
                var inCancel = new Models.InCancelation();
                inCancel.Ref = dRow.Field<string>("Customer Ref");
                inCancel.Customer = dRow.Field<string>("Company Name");
                inCancel.CancelStatus = dRow.Field<string>("Cancel Status");
                inCancel.PendingStatus = dRow.Field<string>("Pending Status");
                inCancel.Comment = dRow.Field<string>("Comment");
                model.Add(inCancel);
            }
            return PartialView(model);

        }
        [OutputCache(Duration = MD_CONST_DURATIONS.OUTPUTCASH_DURATION, VaryByParam = "id")]
        public PartialViewResult PendingClients(int id)
        {
            int monthSelected = 0;
            if (id > 0)
                monthSelected = -1 * id;
            DateTime currentDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(monthSelected);
            DateTime startDate = currentDate;
            DateTime endDate = currentDate.AddMonths(1).AddDays(-1);

            var db = new DBConnect();
            string query = "select com_Ref as 'Ref', com_name as 'Customer',com_ac_pending_date as 'Pending Date' from tblcompany " +
                $"where com_ac_pending = 1 and com_acc_cancel != 2 and com_ac_pending_date between '{startDate.ToString("yyyy-MM-dd")}' and '{endDate.ToString("yyyy-MM-dd")}'";
            var model = new List<ManagementDashboard.Models.PendingClients>();
            var result = db.Query(query);

            foreach (DataRow dRow in result.Tables[0].Rows)
            {
                var PenCus = new Models.PendingClients();
                PenCus.Ref = dRow.Field<string>("Ref");
                PenCus.Customer = dRow.Field<string>("Customer");
                PenCus.PendingDate = (DateTime)dRow.Field<DateTime>("Pending Date");

                model.Add(PenCus);

            }

            return PartialView(model);
        }

        [OutputCache(Duration = MD_CONST_DURATIONS.OUTPUTCASH_DURATION)]
        public PartialViewResult ListOfPendingClient()
        {
            var db = new DBConnect();
            string query = "select com_ref as 'Customer Ref', com_name as 'Company Name', com_ac_pending_date as 'Start Date', " +
                " com_acc_pending_flagdate as 'End Date',  case com_ac_pending when 0 then '' when 1 then 'Pending' when 2 then 'Pending' end as 'Pending Status', " +
                "case com_acc_cancel when 0 then 'Active' when 1 then 'Cancelling' when 2 then 'Cancelled' else '' end as 'CancelCancel Status'," +
                " srv_name as 'Service', rep_name as 'Sales Agent' from tblcompany  left join tblservice on srv_id = com_serviceid left join " +
                "tblrep on rep_id = com_repid where com_acc_cancel = 0 and com_ac_pending<> 0";
            var model = new List<ManagementDashboard.Models.ListOfPendingClient>();
            var result = db.Query(query);

            foreach (DataRow dRow in result.Tables[0].Rows)
            {
                var LisPenCus = new Models.ListOfPendingClient();
                LisPenCus.Ref = dRow.Field<string>("Customer Ref");
                LisPenCus.Customer = dRow.Field<string>("Company Name");
                LisPenCus.StartDate = (DateTime)dRow.Field<DateTime>("Start Date");
                LisPenCus.EndDate = (DateTime)dRow.Field<DateTime>("End Date");
                LisPenCus.PendingStatus = dRow.Field<string>("Pending Status");
                LisPenCus.Service = dRow.Field<string>("Service");
                LisPenCus.SalesAgent = dRow.Field<string>("Sales Agent");
                model.Add(LisPenCus);

            }
            return PartialView(model);
        }


        [OutputCache(Duration = MD_CONST_DURATIONS.OUTPUTCASH_DURATION, VaryByParam = "id")]
        public PartialViewResult NewClientByIndustry(int id)
        {
            ManagementDashboard.Models.ChartModel model = new ChartModel();
            try
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



                model.Labels = labels;
                model.ComplexDatasets = data;
                model.Title = $"Date Range {startDate.ToString("dd/MM/yyyy")} to {endDate.ToString("dd/MM/yyyy")}";
                model.ChartID = $"ChartNewClientByIndustry{id}";
                model.ChartHeight = 350;
                
                return PartialView("BarChartPartial", model);
            }
            catch (Exception e)
            {
                

            }
            
            return PartialView("BarChartPartial", model);
        }

        public ActionResult Xero()
        {
            return View();
        }


        [OutputCache(Duration = MD_CONST_DURATIONS.OUTPUTCASH_DURATION)]
        public PartialViewResult XeroClients()
        {
            var db = new DBConnect(new MySqlConfig() { 
                Database = Properties.Settings.Default.MySqlDBPortal,
                Host = Properties.Settings.Default.MySQLHostPortal,
                Username = Properties.Settings.Default.MySqlUsernamePortal,
                Password = Properties.Settings.Default.MySqlPasswordPortal,
            });
            var query = "select " +
                " comref,com_name,ifnull(tenantName,'Not available') as tenantName, " +
                " case connection_status when 0 then 'Disconnected' when 1 then 'Connected' end  as connection_status" +
                " from tblxeroauth " +
                " left join tblcompany on com_ref = comref";


            var xeroClients = new List<XeroClient>();
            var result =  db.Query(query);

            foreach (DataRow dr in result.Tables[0].Rows)
            {
                XeroClient xeroClient = new XeroClient();
                xeroClient.CustomerReference = dr.Field<string>("comref");
                xeroClient.CustomerName = dr.Field<string>("com_name");
                xeroClient.TenantName = dr.Field<string>("tenantName");

                xeroClient.ConnectionStatus = dr.Field<string>("connection_status");
                xeroClients.Add(xeroClient);

            }



            return PartialView(xeroClients);
        }

    }
}