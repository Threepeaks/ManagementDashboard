using ManagementDashboard.Attributes;
using ManagementDashboard.Models;
using ManagementDashboard.ModelViews;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ManagementDashboard.Controllers
{

    [Authorize]
    [LogUserAccess]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var vm = new HomeViewModel();



            var actionDates = new List<DateTime>();
            var actionDatesMax = 5;
            var actionDateCounter = -1;
            var lastActionDate = Helpers.DateHelpers.GetPreviousActionDate(DateTime.Now, 1).Date;
            actionDates.Add(lastActionDate);
            while (actionDateCounter < actionDatesMax)
            {
                actionDateCounter++;
                var nextActionDate = Helpers.DateHelpers.GetNextActionDate(DateTime.Now, actionDateCounter).Date;
                actionDates.Add(nextActionDate);
            }

            var actionDateWidgets = new List<ActionDateCountWidget>();
            foreach (var actionDate in actionDates)
            {
                bool includePast = false;
                if (actionDate < DateTime.Now.Date)
                   includePast = true;


                var kvp = CountRunsNotSend(actionDate, includePast);


                string level = "bg-success";
                string title = "Action Date " + actionDate.ToString("dd/MM/yyyy");
                if (actionDate < DateTime.Now.Date)
                {
                    title += " (Past Due)";
                    level = "bg-danger";
                    if (kvp.Value == 0)
                        level = "bg-success";
                }

                if (actionDate == DateTime.Now.Date)
                {
                    title += " (Today)";
                    level = "bg-warning";
                    if (kvp.Value == 0)
                        level = "bg-success";

                }




                var actionDateWidget = new ActionDateCountWidget();
                actionDateWidget.ActionDate = kvp.Key;
                actionDateWidget.Title = title;

                actionDateWidget.Level = level;
                actionDateWidget.Count = kvp.Value;

                

                actionDateWidgets.Add(actionDateWidget);
            }
            vm.ActionDateWidgets = actionDateWidgets.OrderBy(x=>x.ActionDate).ToList();


            


            //var actionDate = Helpers.DateHelpers.GetNextActionDate(DateTime.Now,2);
            //ViewBag.CountRunsNotSendFuture = CountRunsNotSend(actionDate);

            ViewBag.CountPayBlock = CountPayBlock();
            ViewBag.CountBlockProcess = CountBlockProcess();
            return View(vm);
        }



        private KeyValuePair<DateTime, int> CountRunsNotSend(DateTime date, bool includePast = false)
        {
            string actionDateSQL = date.ToString("yyyy-MM-dd");
            string query = $"select count(*) as c from tblrbr left join tblhyphen_batchno on hbn_rbr = rbr_id and hbn_type = 1 where ";
            
            if (includePast)
                query += $"rbr_date <= '{actionDateSQL}' ";
            else
                query += $"rbr_date = '{actionDateSQL}' ";

            query += $"and rbr_status in (0, 1) and hbn_rbr is null";
            
            
            var db = new DBConnect();
           
            var dbResult = db.Query(query);
            DataRow dr = dbResult.Tables[0].Rows[0];
            
            //Type t = a["c"].GetType();
            Int64 count = dr.Field<Int64>("c");

            
            return new KeyValuePair<DateTime, int>(date, Convert.ToInt32(count));

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
   


        public ActionResult OverviewView()
        {
            return View();
        }

   
     
        public ActionResult WithholdingsView()
        {
            return View();
        }

        public PartialViewResult GetEmployeeClockingInfo()
        {
            try
            {
                var url = Properties.Settings.Default.TimeAttendanceUrl;
                var client = new RestClient(url);
                var request = new RestRequest("Api/Employees/GetEmployeesClockingInformation");

                var companyIds = Properties.Settings.Default.CompanyIds;
                request.AddQueryParameter("companyIds", companyIds);

                var response = client.Execute(request);

                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    return PartialView("_GetEmployeeClockingInfo", new List<EmployeeClockingInfoResult>());
                }

                var timeAttendance = Newtonsoft.Json.JsonConvert.DeserializeObject<List<EmployeeClockingInfoResult>>(response.Content);
                return PartialView("_GetEmployeeClockingInfo", timeAttendance);
            }
            catch (Exception)
            {
                return PartialView("_GetEmployeeClockingInfo", new List<EmployeeClockingInfoResult>());
            }
        }

        public JsonResult GetBirthdays()
        {
            var result = new List<BirthdayMessageVM>();

            try
            {
                var url = Properties.Settings.Default.TimeAttendanceUrl;
                var client = new RestClient(url);
                var request = new RestRequest("Api/Employees/GetTodaysBirthday");
                var response = client.Execute(request);

                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    return Json(result, JsonRequestBehavior.AllowGet);
                }

                result = Newtonsoft.Json.JsonConvert.DeserializeObject<List<BirthdayMessageVM>>(response.Content);
            }
            catch (Exception ex)
            {
                // optionally log ex
            }

            // ✅ Always return JsonResult with AllowGet
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetUpcomingBirthdays()
        {
            var result = new List<EmployeeBirthdayDto>();

            try
            {
                var url = Properties.Settings.Default.TimeAttendanceUrl;
                var client = new RestClient(url);
                var request = new RestRequest("Api/Employees/GetUpcomingBirthdays");
                var response = client.Execute(request);

                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    return Json(result, JsonRequestBehavior.AllowGet);
                }

                result = Newtonsoft.Json.JsonConvert.DeserializeObject<List<EmployeeBirthdayDto>>(response.Content);
            }
            catch (Exception ex)
            {

            }

            // ✅ Always return JsonResult with AllowGet
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}