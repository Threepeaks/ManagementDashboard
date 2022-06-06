using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ManagementDashboard.Controllers
{
    public class ReportsController : Controller
    {

        public ActionResult DepositBalanceReport()
        {
            return View();
        }


        // GET: Reports
        [OutputCache(Duration = MDConst.OUTPUTCASH_DURATION, VaryByParam = "id")]
        public PartialViewResult DepositBalancePartial(int id)
        {
            int monthSelected = 0;
            if (id > 0)
                monthSelected = -1 * id;
            DateTime currentDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(monthSelected);
            DateTime startDate = currentDate;
            DateTime endDate = currentDate.AddMonths(1).AddDays(-1);

            var db = new DBConnect();
            string query = "select cref as Customer, sum(amount) as Amount from tbl_accounting_depost_tracking a left join tblcompany b on a.cref = b.com_ref " +
                $"where deposit_date <= '{endDate.ToString("yyyy-MM-dd")}' and com_acc_cancel != 2 and com_retterms = 3 group by cref";

            var model = new List<ManagementDashboard.Models.DepositBalance>();
            var result = db.Query(query);



            foreach (DataRow dRow in result.Tables[0].Rows)
            {
                var depMov = new Models.DepositBalance();
                depMov.Customer = dRow.Field<string>("Customer");
                depMov.Amount = (int)dRow.Field<decimal>("Amount");



                model.Add(depMov);

            }

            return PartialView(model);

        }
    }
}