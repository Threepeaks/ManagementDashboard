using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace ManagementDashboard.Controllers
{

    public class XeroController : Controller
    {

        public string DataTableToJSONWithJSONNet(DataTable table)
        {


            string JSONString = string.Empty;
            JSONString  = Newtonsoft.Json.JsonConvert.SerializeObject(table);
            return JSONString;
        }

        // GET: Xero
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult XeroClientsRecordsStat()
        {
            var dbConfig = new MySqlConfig()
            {
                Database = Properties.Settings.Default.MySqlDBPortal,
                Host = Properties.Settings.Default.MySQLHostPortal,
                Password = Properties.Settings.Default.MySqlPasswordPortal,
                Username = Properties.Settings.Default.MySqlUsernamePortal
            };
            var db = new DBConnect(dbConfig);
            var dbREsult = db.Query("call XeroClientRecords()");
            var jsonString = DataTableToJSONWithJSONNet(dbREsult.Tables[0]);
            var vm = new ModelViews.XeroClientsRecords();
            vm.JsonData = jsonString;


            return View(vm);
        }
    }
}