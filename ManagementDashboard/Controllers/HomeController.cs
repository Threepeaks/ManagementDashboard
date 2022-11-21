using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ManagementDashboard.Controllers
{

    [Authorize]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {

            ViewBag.CountRunsNotSend = CountRunsNotSend();
            ViewBag.CountPayBlock = CountPayBlock();
            ViewBag.CountBlockProcess = CountBlockProcess();
            return View();
        }



        private int CountRunsNotSend()
        {
            string query = "select count(*) as c from tblrbr left join tblhyphen_batchno on hbn_rbr = rbr_id where rbr_date <= CURDATE() and rbr_status in (0, 1) and hbn_rbr is null";
            var db = new DBConnect();
            var result = db.Query(query);
            DataRow a = result.Tables[0].Rows[0];
            
            //Type t = a["c"].GetType();
            Int64 count = a.Field<Int64>("c");

            return Convert.ToInt32(count);
        }

        private int CountPayBlock()
        {
            string query = "select count(*) as c from tblcompany where allowPayment = 0 and com_acc_cancel in (0, 1)";
            var db = new DBConnect();
            var result = db.Query(query);
            DataRow a = result.Tables[0].Rows[0];
            
            //Type t = a["c"].GetType();
            Int64 count = a.Field<Int64>("c");

            return Convert.ToInt32(count);
        }

        private int CountBlockProcess()
        {
            string query = "select count(*) as c from tblcompany where allowBankProcessing = 0 and com_acc_cancel in (0, 1)";
            var db = new DBConnect();
            var result = db.Query(query);
            DataRow a = result.Tables[0].Rows[0];
            
            //Type t = a["c"].GetType();
            Int64 count = a.Field<Int64>("c");

            return Convert.ToInt32(count);
        }

        public ActionResult PrintIndex()
        {
            var report = new Rotativa.MVC.ActionAsPdf("Index");



            return report;
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult TrendView()
        {


            return View();
        }
        public ActionResult ManagementView()
        {


            return View();
        }
        public ActionResult NewGrowthView()
        {
            return View();
        }

        public ActionResult OverviewView()
        {
            return View();
        }

        public ActionResult AccountingView()
        {
            return View();
        }
        public ActionResult SubmissionsView()
        {
            return View();
        }
        public ActionResult WithholdingsView()
        {
            return View();
        }

        public ActionResult CustomerView()
        {
            return View();
        }
        public ActionResult Revenue()
        {
            return View();
        }

    }
}