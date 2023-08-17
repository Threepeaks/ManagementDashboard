using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ManagementDashboard.Controllers
{
    public class RiskController : Controller
    {
        // GET: Risk
        public ActionResult Index()
        {
            return View();
        }


        [OutputCache(Duration = MD_CONST_DURATIONS.OUTPUTCASH_DURATION)]
        public ActionResult ClientsColelctionUnpaidsByActionDate()
        {

            var cm = new Models.SQLReportTableViewModel();

            var db = new DBConnect();

            string file = Server.MapPath("~") + "SQLQueries\\Risk\\ClientsColelctionUnpaidsByActionDate.sql";

            if (System.IO.File.Exists(file))
            {
                StreamReader streamReader = new StreamReader(file);
                var fileContent = streamReader.ReadToEnd();

                DateTime endDate = DateTime.Now.AddDays(1);
                var startDate = new DateTime(endDate.Year, endDate.Month, 1).AddMonths(-3);


                var queryParms= new Dictionary<string, string>();
                queryParms.Add("startDate", startDate.ToString("yyyy-MM-dd"));
                queryParms.Add("endDate", endDate.ToString("yyyy-MM-dd"));

                string query = QueryReplace(fileContent, queryParms);

                var result = db.Query(query);

                ColumnTypeItems columnTypePairs = new ColumnTypeItems();
                columnTypePairs.Add("Deposit", ColumnType.Decimal);
                columnTypePairs.Add("Retention", ColumnType.Decimal);
                columnTypePairs.Add("Debits Value", ColumnType.Decimal);
                columnTypePairs.Add("Unpaids Value", ColumnType.Decimal);
                columnTypePairs.Add("UNP RATIO", ColumnType.Percentage);

                string htmlTable = result.Tables[0].ConvertDataTableToHTML(columnTypePairs);
                cm.HtmlTable = htmlTable;

            }

            return View(cm);
        }

        private string QueryReplace(string fileContent, Dictionary<string, string> queryParms)
        {
            var query = fileContent;
            foreach (var param in queryParms)
            {
                query = query.Replace("{" + param.Key + "}", param.Value);
            }
            return query;
        }
    }
}