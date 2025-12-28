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
    public class SubmissionsActionDate
    {
        public DateTime ActionDate { get; set; }
        public int Count { get; set; }
        public bool IsValidDate { get; set; }
        public void SetIfIsValidActionDate()
        {
            this.IsValidDate = Helpers.DateHelpers.IsValidActionDate(this.ActionDate);
        }
            


        private void CountRunsNotSend()
        {
            string actionDateSQL = this.ActionDate.ToString("yyyy-MM-dd");
            string query = $"select count(*) as c from tblrbr left join tblhyphen_batchno on hbn_rbr = rbr_id and hbn_type = 1 where ";
            query += $"rbr_date = '{actionDateSQL}' ";
            query += $"and rbr_status in (0, 1) and hbn_rbr is null";


            var db = new DBConnect();

            var dbResult = db.Query(query);
            DataRow dr = dbResult.Tables[0].Rows[0];

            //Type t = a["c"].GetType();
            Int64 count = dr.Field<Int64>("c");
            this.Count = Convert.ToInt32(count);
        }
        public void Build()
        {
            CountRunsNotSend();
            this.IsValidDate = Helpers.DateHelpers.IsValidActionDate(this.ActionDate);
        }
    }

    public class SubmissionsActionDates : List<SubmissionsActionDate>
    {
        private bool isBuild;
        public void AddMissingNonProcessingDates()
        {
            var minDate = this.Min(x => x.ActionDate);
            var maxDate = this.Max(x => x.ActionDate);

            var currentDate = minDate;

            while (currentDate < maxDate)
            {
                if (!this.Any(x => x.ActionDate.Date == currentDate.Date))
                {
                    this.Add(new SubmissionsActionDate
                    {
                        ActionDate = currentDate,
                        IsValidDate = false
                    });
                }

                currentDate = currentDate.AddDays(1);
            }
        }

        public void ReOrderList()
        {
            var tempList = this.OrderBy(x => x.ActionDate).ToList();
            this.Clear();
            this.AddRange(tempList);
        }
        public void AddSubmissionDates(List<DateTime> allSubmissionActionDates)
        {
            foreach (var item in allSubmissionActionDates)
            {
                if (!this.Any(x => x.ActionDate == item))
                {
                    this.Add(new SubmissionsActionDate()
                    {
                        ActionDate = item,
                        IsValidDate = false
                    });
                }



            }
        }

        public List<ActionDateCountWidget> ToWidgets()
        {
            if (!isBuild)
                throw new Exception("Build not executed before ToWidgets");

            var actionDateWidgets = new List<ActionDateCountWidget>();

            var widgets = new List<ActionDateCountWidget>();
            var minValidActionDate = this.Where(x => x.IsValidDate).Min(x => x.ActionDate);

            foreach (var sad in this)
            {

                string level = "bg-success";
                string title = "Action Date " + sad.ActionDate.ToString("dd/MM/yyyy");
                if (sad.ActionDate < DateTime.Now.Date)
                {
                    title += " (Past Due)";
                    level = "bg-danger";
                    if (sad.Count == 0)
                        level = "bg-success";
                }

                var cutoffTimeForToday = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 15, 0, 0);
                if (sad.ActionDate == DateTime.Now.Date && DateTime.Now < cutoffTimeForToday)
                {
                    title += " (Today)";
                    level = "bg-warning";
                    if (sad.Count == 0)
                        level = "bg-success";
                }
                if (sad.ActionDate == DateTime.Now.Date && DateTime.Now >= cutoffTimeForToday)
                {
                    title += " (Today)";
                    level = "bg-danger";
                    if (sad.Count == 0)
                        level = "bg-success";
                }




                var actionDateWidget = new ActionDateCountWidget();
                actionDateWidget.ActionDate = sad.ActionDate;
                actionDateWidget.Title = title;

                actionDateWidget.Level = level;
                actionDateWidget.Count = sad.Count;



                actionDateWidgets.Add(actionDateWidget);

            }


            return actionDateWidgets
                .Where(x=>x.Count > 0)
                .OrderBy(x=>x.ActionDate).ToList(); 
        }


        public void Build(int maxActionDates = 4)
        {
            var validActionDates = new List<DateTime>();
            var actionDatesMax = maxActionDates;
            var actionDateCounter = 0;

            var lastActionDate = Helpers.DateHelpers.GetPreviousActionDate(DateTime.Now, 1).Date;
            this.Add(new SubmissionsActionDate()
            {
                ActionDate = lastActionDate,
                IsValidDate = true
            });

            //TODO
            if (Helpers.DateHelpers.IsValidActionDate(DateTime.Now))
            {
                this.Add(new SubmissionsActionDate()
                {
                    ActionDate = DateTime.Now.Date,
                    IsValidDate = true
                });
            }

            while (actionDateCounter < actionDatesMax)
            {
                actionDateCounter++;
                var nextActionDate = Helpers.DateHelpers.GetNextActionDate(DateTime.Now, actionDateCounter).Date;
                validActionDates.Add(nextActionDate);

                this.Add(new SubmissionsActionDate()
                {
                    ActionDate = nextActionDate,
                    IsValidDate = true
                });
            }

            var maxActionDate = validActionDates.Max();
            var allSubmissionActionDates = GetSubmissionActionDates(maxActionDate);
            this.AddSubmissionDates(allSubmissionActionDates);

            AddMissingNonProcessingDates();
            ReOrderList();

            foreach (var sad in this)
            {
                sad.Build();
            }

            //Set build flag = true
            isBuild = true;

        }

        List<DateTime> GetSubmissionActionDates(DateTime maxActionDate)
        {
            var sqlDateString = maxActionDate.ToString("yyyy-MM-dd");   
            string query = $"select distinct rbr_date as c from tblrbr left join tblhyphen_batchno on hbn_rbr = rbr_id and hbn_type = 1 where ";
            query += $" rbr_status in (0, 1) and hbn_rbr is null and rbr_date <= '{sqlDateString}'";


            var db = new DBConnect();

            var dbResult = db.Query(query);
            var result = new List<DateTime>();
            foreach (DataRow dr in dbResult.Tables[0].Rows)
            {
                //Convert the c from MySQL DateTime to DateTime
                DateTime actionDate = dr.Field<DateTime>("c");
                result.Add(actionDate.Date);

            }

            return result;


        }

    }


    [Authorize]
    [LogUserAccess]
    public class HomeController : Controller
    {

        List<ActionDateCountWidget> GetActionDateWidgets()
        {
            var submissionActionDates = new SubmissionsActionDates();
            submissionActionDates.Build();
            return submissionActionDates.ToWidgets();
        }
        public ActionResult Index()
        {
            var vm = new HomeViewModel();

            vm.ActionDateWidgets = GetActionDateWidgets();



            


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