using ManagementDashboard.Attributes;
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
    public class CollateralController : Controller
    {
        private DBConnect db = new DBConnect();

        // GET: Collateral
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult NoRetentionDeposit()
        {
            return View();
        }

        public PartialViewResult CollateralNonePartialView()
        {

            

            string query = $"select com_ref as 'Ref', com_name as 'Customer', com_retterms, case com_retterms when 1 then 'Retention' when 3 then 'Deposit' else '' end 'Collateral', " +
                $"case com_retterms when 1 then ifnull((select sum(rbr_total_retention) from tblrbr where rbr_status = 3 and rbr_comref = com_ref " +
                $"and rbr_ret_released = 0),0) when 2 then - 1 when 3 then ifnull((select sum(amount) from tbl_accounting_depost_tracking where " +
                $"cref = com_ref),0) when 4 then 0 end as 'Value', if ((select count(*) from tblrbr where rbr_comref = com_ref limit 1) >= 1, 'Yes','No') as " +
                $"'Have Runs',com_retper, com_startdate " +
                $" from tblcompany where com_acc_cancel in (0, 1) and com_retterms in (4) ";
            // com_retterms (4) is No Collateral

            var model = new List<ManagementDashboard.Models.NoRetentionDeposit>();
            var result = db.Query(query);

            foreach (DataRow dRow in result.Tables[0].Rows)
            {
                var RetDep = new Models.NoRetentionDeposit();
                RetDep.Ref = dRow.Field<string>("Ref");
                RetDep.Customer = dRow.Field<string>("Customer");
                RetDep.Collateral = dRow.Field<string>("Collateral");
                RetDep.Value = (int)dRow.Field<decimal>("Value");
                RetDep.HaveRuns = dRow.Field<string>("Have Runs");
                RetDep.RiskRatioSet = (decimal)dRow.Field<decimal>("com_retper");
                RetDep.StartDate = (DateTime)dRow.Field<DateTime>("com_startdate");

                model.Add(RetDep);
            }

            List<ManagementDashboard.Models.NoRetentionDeposit> filteredModel = model.Where(x => x.Value <= 0).ToList();

            return PartialView(model);
        }


        [OutputCache(Duration = MD_CONST_DURATIONS.OUTPUTCASH_DURATION, VaryByParam = "id")]
        public PartialViewResult NoRetentionDepositPartialView(int id)
        {
            //var db = new DBConnect();

            string query = $"select com_ref as 'Ref', com_name as 'Customer', com_retterms, case com_retterms when 1 then 'Retention' when 3 then 'Deposit' else '' end 'Collateral', " +
                $"case com_retterms when 1 then ifnull((select sum(rbr_total_retention) from tblrbr where rbr_status = 3 and rbr_comref = com_ref " +
                $"and rbr_ret_released = 0),0) when 2 then - 1 when 3 then ifnull((select sum(amount) from tbl_accounting_depost_tracking where " +
                $"cref = com_ref),0) when 4 then 0 end as 'Value', if ((select count(*) from tblrbr where rbr_comref = com_ref limit 1) >= 1, 'Yes','No') as " +
                $"'Have Runs',com_retper, com_startdate " +
                $" from tblcompany where com_acc_cancel in (0, 1) and com_retterms in (1,2,3) ";
            // com_retterms (4) is No Collateral

            var model = new List<ManagementDashboard.Models.NoRetentionDeposit>();
            var result = db.Query(query);

            foreach (DataRow dRow in result.Tables[0].Rows)
            {
                var RetDep = new Models.NoRetentionDeposit();
                RetDep.Ref = dRow.Field<string>("Ref");
                RetDep.Customer = dRow.Field<string>("Customer");
                RetDep.Collateral = dRow.Field<string>("Collateral");
                RetDep.Value = (int)dRow.Field<decimal>("Value");
                RetDep.HaveRuns = dRow.Field<string>("Have Runs");
                RetDep.RiskRatioSet = (decimal)dRow.Field<decimal>("com_retper");
                RetDep.StartDate = (DateTime)dRow.Field<DateTime>("com_startdate");

                model.Add(RetDep);
            }

            List<ManagementDashboard.Models.NoRetentionDeposit> filteredModel = model.Where(x => x.Value <= 0).ToList();

            return PartialView(filteredModel);
        }

    }




}