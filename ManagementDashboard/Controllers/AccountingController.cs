using Google.Protobuf.Reflection;
using ManagementDashboard.Models;
using MySqlX.XDevAPI.Relational;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ManagementDashboard.Controllers
{

    [Authorize]
    public class AccountingController : Controller
    {
        // GET: Accounting
        public ActionResult Index()
        {
            return View();
        }
        [OutputCache(Duration = MD_CONST_DURATIONS.OUTPUTCASH_DURATION)]
        public ActionResult ManagementPrediction()
        {

            var vm = new Models.SQLReportTableViewModel();
            var db = new DBConnect();

            string file = Server.MapPath("~") + "SQLQueries\\ManagementPrediction.sql";
            if (System.IO.File.Exists(file))
            {
                StreamReader streamReader = new StreamReader(file);
                var fileContent = streamReader.ReadToEnd();

                var query = fileContent;

                var result = db.Query(query);

                var colTypes = new ColumnTypeItems();

                //Management Fee
                colTypes.Add("Management Fee", ColumnType.Decimal);
                //Prediction Amount
                colTypes.Add("Prediction Amount", ColumnType.Decimal);

                string htmlTable = result.Tables[0].ConvertDataTableToHTML(colTypes);
                vm.HtmlTable = htmlTable;

            }

            return View(vm);
        }
        public ActionResult DepositBalanceReport()
        {
            return View();
        }


        // GET: Reports
        [OutputCache(Duration = MD_CONST_DURATIONS.OUTPUTCASH_DURATION, VaryByParam = "id")]
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


        [OutputCache(Duration = MD_CONST_DURATIONS.OUTPUTCASH_DURATION)]
        public ActionResult LiabilityCurrent()
        {

            var vm = new Models.SQLReportTableViewModel();

            var db = new DBConnect();
            var model = new List<ManagementDashboard.Models.LiabilityCurrent>();

            string file = Server.MapPath("~") + "SQLQueries\\liabilitycurrent.sql";
            if (System.IO.File.Exists(file))
            {
                DataTable tbl = new DataTable();
                StreamReader streamReader = new StreamReader(file);
                var fileContent = streamReader.ReadToEnd();

                var query = fileContent;

                var result = db.Query(query);
                if (result.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dRow in result.Tables[0].Rows)
                    {
                        LiabilityCurrent lc = new LiabilityCurrent();
                        lc.Customer = dRow.Field<string>("CREF");
                        lc.Status = dRow.Field<string>("Status");
                        lc.Gateway = dRow.Field<string>("Gateway");
                        lc.BalanceBroughtForward = dRow.Field<decimal>("BCF");
                        lc.Collection = dRow.Field<decimal>("Collection");
                        lc.Unpaids = dRow.Field<decimal>("Unpaids");
                        lc.LateUnpaids = dRow.Field<decimal>("LateUnpaids");
                        lc.RetentionHeld = dRow.Field<decimal>("RetentionHeld");
                        lc.Account = dRow.Field<decimal>("Account");
                        //=[@[B/F]]+[@Collection]-[@Unpaids]-[@[Late Unp]]+[@[Retention Held]]-[@Account]
                        lc.Balance = lc.BalanceBroughtForward + lc.Collection - lc.Unpaids - lc.LateUnpaids + lc.RetentionHeld - lc.Account;
                        //IF([@Balance]<0,-1*[@Balance],0)
                        lc.Debit = (lc.Balance < 0) ? -1 * lc.Balance : 0;
                        //=IF([@Balance]>0,[@Balance],0)
                        lc.Credit = (lc.Balance > 0) ? lc.Balance : 0;
                        model.Add(lc);
                    }

                }

            }
            return View(model);
        }

        [OutputCache(Duration = MD_CONST_DURATIONS.OUTPUTCASH_DURATION)]
        public ActionResult LateUnpaidsCreditProvided()
        {

            var cm = new Models.SQLReportTableViewModel();

            var db = new DBConnect();

            string file = Server.MapPath("~") + "SQLQueries\\LateUnpaidsCreditProvided.sql";
            if (System.IO.File.Exists(file))
            {
                StreamReader streamReader = new StreamReader(file);
                var fileContent = streamReader.ReadToEnd();

                var query = fileContent;

                var result = db.Query(query);
                ColumnTypeItems fields = new ColumnTypeItems();
                fields.Add("Customer", ColumnType.String);
                fields.Add("Risk Type", ColumnType.String);
                fields.Add("Total Debits", ColumnType.Decimal);
                fields.Add("Total Holding",ColumnType.Decimal);
                fields.Add("Adjustment Account", ColumnType.Decimal);
                fields.Add("Subs not Settled", ColumnType.Decimal);
                fields.Add("Upcoming Subs", ColumnType.Decimal);
                fields.Add("Credit Provided",ColumnType.Decimal);
                

                string htmlTable = result.Tables[0].ConvertDataTableToHTML(fields);
                cm.HtmlTable = htmlTable;

            }

            return View(cm);
        }
        public ActionResult Revenue()
        {
            return View();
        }
        public ActionResult ManagementView()
        {


            return View();
        }
        public ActionResult AccountingView()
        {
            return View();
        }

        [OutputCache(Duration = MD_CONST_DURATIONS.OUTPUTCASH_DURATION, VaryByParam = "id")]
        public PartialViewResult GetTopRevenue(int id)
        {
            int monthSelected = 0;
            if (id > 0)
                monthSelected = -1 * id;
            DateTime currentDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(monthSelected);
            DateTime startDate = currentDate;
            DateTime endDate = currentDate.AddMonths(1).AddDays(-1);

            var db = new DBConnect();
            string query = $"select * from(select inv_comref as 'ComRef'," +
                $"inv_comname as 'Company', sum(inv_t_total) as 'Total' from threepeaks_tpms.tblinvoice where " +
                $"inv_date between '{startDate.ToString("yyyy-MM-dd")}' and '{endDate.ToString("yyyy-MM-dd")}'" +
                $"group by inv_comref) as g order by g.`Total` desc limit 20";
            var model = new List<ManagementDashboard.Models.GetTopRevenue>();
            var result = db.Query(query);


            foreach (DataRow dRow in result.Tables[0].Rows)
            {
                var rev = new Models.GetTopRevenue();
                rev.comRef = dRow.Field<string>("ComRef");
                rev.Company = dRow.Field<string>("Company");
                rev.total = (int)dRow.Field<decimal>("Total");

                model.Add(rev);
            }
            return PartialView(model);
        }

        [OutputCache(Duration = MD_CONST_DURATIONS.OUTPUTCASH_DURATION, VaryByParam = "id")]
        public PartialViewResult ManagementFees(int id)
        {
            int monthSelected = 0;
            if (id > 0)
                monthSelected = -1 * id;
            DateTime currentDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(monthSelected);
            DateTime startDate = currentDate;
            DateTime endDate = currentDate.AddMonths(1).AddDays(-1);

            var db = new DBConnect();
            string query = "select dbt_unpaid_datetime as 'Unpaid Date', dbt_ref as 'Customer Reference', " +
                "case dbt_pass_unpaid when 2 then 'Current' when 3 then 'Late' end as 'Type', dbt_amount as 'Amount', " +
                "dbt_accrejcode as 'Code', hec_description as 'Reason' from tbldebits left join tblhyphen_errcodes " +
                "on hec_code = dbt_accrejcode where dbt_comref = 'THREE' and dbt_unpaid_datetime " +
                $"between '{startDate.ToString("yyyy-MM-dd")}' and '{endDate.ToString("yyyy-MM-dd")}'" +
                " order by dbt_unpaid_datetime,dbt_ref";
            var model = new List<ManagementDashboard.Models.ManagementFees>();
            var result = db.Query(query);

            foreach (DataRow dRow in result.Tables[0].Rows)
            {
                var manFee = new Models.ManagementFees();
                manFee.UnpaidDate = (DateTime)dRow.Field<DateTime>("Unpaid Date");
                manFee.CustomerReference = dRow.Field<string>("Customer Reference");
                manFee.Type = dRow.Field<string>("Type");
                manFee.Amount = (int)dRow.Field<decimal>("Amount");
                manFee.Code = dRow.Field<string>("Code");
                manFee.Reason = dRow.Field<string>("Reason");
                model.Add(manFee);
            }
            return PartialView(model);

        }
    }
}