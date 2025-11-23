using ManagementDashboard.Attributes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ManagementDashboard.Controllers
{
    [Authorize]
    [LogUserAccess]
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

                ColumnTypeItems columnTypePairs = new ColumnTypeItems();
                columnTypePairs.Add("Client Reference", ColumnType.ClientReference);
                columnTypePairs.Add("Activation Date", ColumnType.Date);

                string htmlTable = result.Tables[0].ConvertDataTableToHTML(columnTypePairs);
                vm.HtmlTable = htmlTable;

            }

            return View(vm);
        }


        public ActionResult RecordsTrend()
        {
            var dbConfig = new MySqlConfig()
            {
                Database = Properties.Settings.Default.MySqlDBPortal,
                Host = Properties.Settings.Default.MySQLHostPortal,
                Password = Properties.Settings.Default.MySqlPasswordPortal,
                Username = Properties.Settings.Default.MySqlUsernamePortal
            };
            var db = new DBConnect(dbConfig);

            var query = "SELECT " +
                " ifnull(cpr_ref,'NOT SET') as 'ClientReference'," +
                " date_format(m.createdDate,'%Y-%m-%d') as 'CreationDate'," +
                " count(1) as 'Records' " +
                " FROM threesmq_webportal.tblcustomer_mandate m" +
                " left join tbl_customer_profile on cpr_id = m.clientProfileId" +
                " group by cpr_ref,date_format(m.createdDate,'%Y-%m-%d')";

            var dbREsult = db.Query(query);
            var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(dbREsult.Tables[0]);
            var vm = new ModelViews.XeroClientsRecords();
            vm.JsonData = jsonString;


            return View(vm);
        }

    }
}