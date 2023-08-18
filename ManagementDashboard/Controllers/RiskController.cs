using Google.Protobuf.WellKnownTypes;
using ManagementDashboard.DTOs;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
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
        public ActionResult DisputesNotResolved(int id)
        {

            var options = new RestClientOptions("https://disputetracker.threepeaks.co.za/")
            {
                //Authenticator = new HttpBasicAuthenticator("username", "password")
            };
            var client = new RestClient(options);
            var request = new RestRequest("api/disputes/GetNotResolved/" + id);
            // The cancellation token comes from the caller. You can still make a call without it.
            var response = client.Get(request);
            List<Dispute> a = new List<Dispute>();

            if (response.IsSuccessful)
            {

                a = JsonSerializer.Deserialize<List<Dispute>>(response.Content);
            }



            return View(a);
        }


        [OutputCache(Duration = MD_CONST_DURATIONS.OUTPUTCASH_DURATION)]
        public ActionResult DisputesNotResolvedSummary()
        {
          
            var options = new RestClientOptions("https://disputetracker.threepeaks.co.za/")
            {
                //Authenticator = new HttpBasicAuthenticator("username", "password")
            };
            var client = new RestClient(options);
            var request = new RestRequest("api/disputes/GetNotResolvedSummary");
            // The cancellation token comes from the caller. You can still make a call without it.
            var response = client.Get(request);
            List<ClientDisputeSummary> a = new List<ClientDisputeSummary>();

            if (response.IsSuccessful)
            {
                
                a = JsonSerializer.Deserialize<List<ClientDisputeSummary>>(response.Content);
            }



            return View(a);
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