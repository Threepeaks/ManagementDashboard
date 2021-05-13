using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ManagementDashboard.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
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

    }
}