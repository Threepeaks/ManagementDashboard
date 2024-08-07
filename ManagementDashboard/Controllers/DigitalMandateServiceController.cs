using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ManagementDashboard.Controllers
{
    public class DigitalMandateServiceController : Controller
    {

        //[OutputCache(Duration = MD_CONST_DURATIONS.OUTPUTCASH_DURATION)]
        public ActionResult ActiveDigitalMandateConfigs()
        {
            var dbConfig = new MySqlConfig()
            {
                Database = Properties.Settings.Default.MySqlDBPortal,
                Host = Properties.Settings.Default.MySQLHostPortal,
                Password = Properties.Settings.Default.MySqlPasswordPortal,
                Username = Properties.Settings.Default.MySqlUsernamePortal
            };
            var db = new DBConnect(dbConfig);
            string file = Server.MapPath("~") + "SQLQueries\\DigitalMandate\\ActiveDigitalMandateConfigsPortal.sql";
            
            var vm = new Models.SQLReportTableViewModel();
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