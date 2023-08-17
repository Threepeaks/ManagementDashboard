using ManagementDashboard.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace ManagementDashboard.Controllers
{
    public class DataColumnAttribute : Attribute { }

    internal class SqlVarReplacementItems : List<SqlVarReplacementItem>
    {
        public void Add(string name,string value)
        {
            var item = new SqlVarReplacementItem();
            item.Name = name;
            item.Value = value;
            this.Add(item);
        }
    }
    internal class SqlVarReplacementItem
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }

    internal class SQLReportManager
    {
        private string GetFileContent(string fileName)
        {
            if (!System.IO.File.Exists(fileName))
                return "";

            string fileContent = "";
            using (StreamReader streamReader = new StreamReader(fileName))
            {
                fileContent = streamReader.ReadToEnd();
            }
            return fileContent;

        }


        public SQLReportTableViewModel GetSqlReportTableViewModel(string sqlFileFullPath,
            SqlVarReplacementItems varItems )
        {
            if (varItems == null)
                varItems = new SqlVarReplacementItems();

            var vm = new Models.SQLReportTableViewModel();

            var db = new DBConnect();

            string query = GetFileContent(sqlFileFullPath);

            if (string.IsNullOrEmpty(query))
                return vm;

            foreach (var varItem in varItems)
            {
                query = query.Replace("{{" + varItem.Name + "}}", varItem.Value);
            }



            var result = db.Query(query);
            string htmlTable = result.Tables[0].ConvertDataTableToHTML();
            vm.HtmlTable = htmlTable;

            return vm;

        }




    }


    [Authorize]
    public class SQLReportsController : Controller
    {
        // GET: SQLReports
        public ActionResult Index()
        {
            return View();
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

        [OutputCache(Duration = MD_CONST_DURATIONS.OUTPUTCASH_DURATION, VaryByParam = "id")]
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
                string htmlTable = result.Tables[0].ConvertDataTableToHTML();
                vm.HtmlTable = htmlTable;



            }



            return View(vm);
        }




    }
}