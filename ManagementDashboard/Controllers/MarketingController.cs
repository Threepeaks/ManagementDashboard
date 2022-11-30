using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ManagementDashboard.Controllers
{

    [Authorize]
    public class MarketingController : Controller
    {
        // GET: Marketing
        public ActionResult Index()
        {
            return View();
        }
        [OutputCache(Duration = MD_CONST_DURATIONS.OUTPUTCASH_DURATION)]
        public ActionResult CustomerReferredBy()
        {

            var vm = new Models.SQLReportTableViewModel();

            var db = new DBConnect();

            string file = Server.MapPath("~") + "SQLQueries\\CustomerReferredBy.sql";
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
    }
}