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
    public class DataColumnAttribute : Attribute { }
    public class SQLReportsController : Controller
    {
        // GET: SQLReports
        public ActionResult Index()
        {
            return View();
        }

        public static string ConvertDataTableToHTML(DataTable dt)
        {
            string html = "<table class='table table-striped table-sm '>";

            //add header row
            html += "<thead>";
            html += "<tr>";
            for (int i = 0; i < dt.Columns.Count; i++)
                html += "<th>" + dt.Columns[i].ColumnName + "</th>";
            html += "</tr>";
            html += "</thead>";
            //add rows
            html += "<tbody>";
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                html += "<tr>";
                for (int j = 0; j < dt.Columns.Count; j++)
                    html += "<td>" + dt.Rows[i][j].ToString() + "</td>";
                html += "</tr>";
            }
            html += "</tbody>";
            html += "</table>";
            return html;
        }
        public static string ConvertLiabilityTableToHTML(DataTable dt)
        {
            string html = "<table class='table table-striped table-sm text-sm'>";

            //add header row
            html += "<thead>";
            html += "<tr>";
            for (int i = 0; i < dt.Columns.Count; i++)
                html += "<th>" + dt.Columns[i].ColumnName + "</th>";
            html += "</tr>";
            html += "</thead>";
            //add rows
            html += "<tbody>";
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                html += "<tr>";
                for (int j = 0; j < dt.Columns.Count; j++)
                    html += "<td>" + dt.Rows[i][j].ToString() + "</td>";
                html += "</tr>";
            }
            html += "</tbody>";
            html += "</table>";
            return html;
        }

        [OutputCache(Duration = MDConst.OUTPUTCASH_DURATION, VaryByParam = "id")]
        public ActionResult RBR(int id)
        {


            var vm = new Models.SQLReportTableViewModel();

            var db = new DBConnect();

            string file = Server.MapPath("~") + "SQLQueries\\rbr.sql";
            if (System.IO.File.Exists(file))
            {
                StreamReader streamReader = new StreamReader(file);
                var fileContent = streamReader.ReadToEnd();

                var query = fileContent.Replace("{{id}}", id.ToString());

                var result = db.Query(query);
                string htmlTable = ConvertDataTableToHTML(result.Tables[0]);
                vm.HtmlTable = htmlTable;



            }



            return View(vm);
        }

        [OutputCache(Duration = MDConst.OUTPUTCASH_DURATION)]
        public ActionResult LiabilityCurrent()
        {

            var vm = new Models.SQLReportTableViewModel();

            var db = new DBConnect();
                var model = new List<ManagementDashboard.Models.LiabilityCurrent>();

            string file = Server.MapPath("~") + "SQLQueries\\liabilitycurrent.sql";
            if (System.IO.File.Exists(file))
            {
                DataTable tbl = new DataTable();
                StreamReader streamReader = new StreamReader(file);
                var fileContent = streamReader.ReadToEnd();

                var query = fileContent;

                var result = db.Query(query);
                if (result.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dRow in result.Tables[0].Rows)
                    {
                        LiabilityCurrent lc = new LiabilityCurrent();
                        lc.Customer = dRow.Field<string>("CREF");
                        lc.Status = dRow.Field<string>("Status");
                        lc.Gateway = dRow.Field<string>("Gateway");
                        lc.BalanceBroughtForward = dRow.Field<decimal>("BCF");
                        lc.Collection = dRow.Field<decimal>("Collection");
                        lc.Unpaids = dRow.Field<decimal>("Unpaids");
                        lc.LateUnpaids = dRow.Field<decimal>("LateUnpaids");
                        lc.RetentionHeld = dRow.Field<decimal>("RetentionHeld");
                        lc.Account = dRow.Field<decimal>("Account");
                        //=[@[B/F]]+[@Collection]-[@Unpaids]-[@[Late Unp]]+[@[Retention Held]]-[@Account]
                        lc.Balance = lc.BalanceBroughtForward + lc.Collection - lc.Unpaids - lc.LateUnpaids + lc.RetentionHeld - lc.Account;
                        //IF([@Balance]<0,-1*[@Balance],0)
                        lc.Debit = (lc.Balance < 0) ? -1 * lc.Balance:0 ;
                        //=IF([@Balance]>0,[@Balance],0)
                        lc.Credit = (lc.Balance > 0) ? lc.Balance : 0;
                        model.Add(lc);
                    }
                    
                }
                
            }
            return View(model);
        }

        [OutputCache(Duration = MDConst.OUTPUTCASH_DURATION)]
        public ActionResult ManagementPrediction()
        {

            var vm = new Models.SQLReportTableViewModel();
            var db = new DBConnect();

            string file = Server.MapPath("~") + "SQLQueries\\ManagementPrediction.sql";
            if (System.IO.File.Exists(file))
            {
                StreamReader streamReader = new StreamReader(file);
                var fileContent = streamReader.ReadToEnd();

                var query = fileContent;

                var result = db.Query(query);
                string htmlTable = ConvertDataTableToHTML(result.Tables[0]);
                vm.HtmlTable = htmlTable;

            }

            return View(vm);
        }

        [OutputCache(Duration = MDConst.OUTPUTCASH_DURATION)]
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
                string htmlTable = ConvertDataTableToHTML(result.Tables[0]);
                vm.HtmlTable = htmlTable;

            }

            return View(vm);
        }

        [OutputCache(Duration = MDConst.OUTPUTCASH_DURATION)]
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
                string htmlTable = ConvertDataTableToHTML(result.Tables[0]);
                cm.HtmlTable = htmlTable;

            }

            return View(cm);
        }

        [OutputCache(Duration = MDConst.OUTPUTCASH_DURATION)]
        public ActionResult LateUnpaidsCreditProvided()
        {

            var cm = new Models.SQLReportTableViewModel();

            var db = new DBConnect();

            string file = Server.MapPath("~") + "SQLQueries\\LateUnpaidsCreditProvided.sql";
            if (System.IO.File.Exists(file))
            {
                StreamReader streamReader = new StreamReader(file);
                var fileContent = streamReader.ReadToEnd();

                var query = fileContent;

                var result = db.Query(query);
                string htmlTable = ConvertDataTableToHTML(result.Tables[0]);
                cm.HtmlTable = htmlTable;

            }

            return View(cm);
        }

        [OutputCache(Duration = MDConst.OUTPUTCASH_DURATION)]
        public PartialViewResult HyphenBatchesStates()
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
                string htmlTable = ConvertDataTableToHTML(result.Tables[0]);
                cm.HtmlTable = htmlTable;

            }

            return PartialView(cm);
        }
    }
}