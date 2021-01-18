using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ManagementDashboard.Controllers
{
    public class SQLReportsController : Controller
    {
        // GET: SQLReports
        public ActionResult Index()
        {
            return View();
        }

        public static string ConvertDataTableToHTML(DataTable dt)
        {
            string html = "<table class='table table-striped table-condensed'>";

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

        public ActionResult RBR(int id)
        {


            var vm = new Models.SQLReportTableViewModel();

            var db = new DBConnect();

            string file = Server.MapPath("~") + "SQLQueries\\rbr.sql";
            if (System.IO.File.Exists(file))
            {
                StreamReader streamReader = new StreamReader(file);
                var fileContent = streamReader.ReadToEnd();

                var query = fileContent.Replace("{{id}}",id.ToString());

                var result = db.Query(query);
                string htmlTable = ConvertDataTableToHTML(result.Tables[0]);
                vm.HtmlTable = htmlTable;



            }



            return View(vm);
        }

        public ActionResult LiabilityCurrent()
        {

            var vm = new Models.SQLReportTableViewModel();

            var db = new DBConnect();
        
            string file = Server.MapPath("~") + "SQLQueries\\liabilitycurrent.sql";
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

    }
}