using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ManagementDashboard.Controllers
{
    [Authorize]
    public class TopController : Controller
    {
        // GET: Top

        // Top 10 Value of Debit
        // Customer Reference / Debit Reference / Action Date / RBR /Amount



        [OutputCache(Duration = MD_CONST_DURATIONS.OUTPUTCASH_DURATION, VaryByParam = "id")]
        public PartialViewResult DepositMovement(int id)
        {
            int monthSelected = 0;
            if (id > 0)
                monthSelected = -1 * id;
            DateTime currentDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(monthSelected);
            DateTime startDate = currentDate;
            DateTime endDate = currentDate.AddMonths(1).AddDays(-1);

            var db = new DBConnect();
            string query = "select cref, deposit_date, amount from tbl_accounting_depost_tracking where deposit_date" +
                $" between '{startDate.ToString("yyyy-MM-dd")}' and '{endDate.ToString("yyyy-MM-dd")}' order by cref,deposit_date";
            var model = new List<ManagementDashboard.Models.DepositMovement>();
            var result = db.Query(query);

            foreach (DataRow dRow in result.Tables[0].Rows)
            {
                var depMov = new Models.DepositMovement();
                depMov.Customer = dRow.Field<string>("cref");
                depMov.Date = (DateTime)dRow.Field<DateTime>("deposit_date");
                depMov.Value = (decimal)dRow.Field<decimal>("amount");


                model.Add(depMov);

            }

            return PartialView(model);

        }


       


        [OutputCache(Duration = MD_CONST_DURATIONS.OUTPUTCASH_DURATION, VaryByParam = "id;IsBetween")]
        [Route("api/top/RetNotReleased/{id}/IsBetween")]
        public PartialViewResult RetNotReleased(int id, bool IsBetween)
        {
            int monthSelected = 0;
            if (id > 0)
                monthSelected = -1 * id;
            DateTime currentDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(monthSelected);
            DateTime startDate = currentDate;
            DateTime endDate = currentDate.AddMonths(1).AddDays(1);

            var db = new DBConnect();
            string query = "SELECT rbr_comref as 'Ref' , rbr_id as 'RBR',rbr_date as 'Action Date',rbr_ret_release_date as 'Release Date',rbr_total_retention as 'Retention Amount' , " +
                "datediff(rbr_ret_release_date, now()) as 'Age' FROM threepeaks_tpms.tblrbr left join tblcompany on com_ref = rbr_comref " +
                $"where rbr_status = 3 and rbr_ret_released = 0 and com_retterms in (1, 2) ";
            if (IsBetween)
            {
                query += $" and rbr_ret_release_date between '{startDate.ToString("yyyy-MM-dd")}' and '{endDate.ToString("yyyy-MM-dd")}' and datediff(rbr_ret_release_date, now()) < - 35";
            }
            else
            {
                query += $" and rbr_ret_release_date <=  '{endDate.ToString("yyyy-MM-dd")}' and datediff(rbr_ret_release_date, now()) < - 35";
            }

            var model = new List<ManagementDashboard.Models.RetNotReleased>();
            var result = db.Query(query);

            foreach (DataRow dRow in result.Tables[0].Rows)
            {
                var depMov = new Models.RetNotReleased();
                depMov.Ref = dRow.Field<string>("Ref");
                depMov.Rbr = (int)dRow.Field<Int32>("RBR");
                depMov.ActionDate = (DateTime)dRow.Field<DateTime>("Action Date");
                depMov.ReleaseDate = (DateTime)dRow.Field<DateTime>("Release Date");
                depMov.RetAmount = (int)dRow.Field<decimal>("Retention Amount");
                depMov.Age = (int)dRow.Field<Int64>("Age");

                //model.Add(depMov); 
                if (depMov.Age <= 20)
                {
                    model.Add(depMov);

                }
            }
            return PartialView(model);

        }

   


        [OutputCache(Duration = MD_CONST_DURATIONS.OUTPUTCASH_DURATION, VaryByParam = "id")]
        public PartialViewResult NoRetentionDeposit(int id)
        {
            var db = new DBConnect();

            string query = $"select com_ref as 'Ref', com_name as 'Customer', case com_retterms when 1 then 'Retention' when 3 then 'Deposit' else '' end 'Collateral', " +
                $"case com_retterms when 1 then ifnull((select sum(rbr_total_retention) from tblrbr where rbr_status = 3 and rbr_comref = com_ref " +
                $"and rbr_ret_released = 0),0) when 2 then - 1 when 3 then ifnull((select sum(amount) from tbl_accounting_depost_tracking where " +
                $"cref = com_ref),0) end as 'Value', if ((select count(*) from tblrbr where rbr_comref = com_ref limit 1) >= 1, 'Yes','No') as " +
                $"'Have Runs' from tblcompany where com_acc_cancel in (0, 1) ";

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

                model.Add(RetDep);
            }

            List<ManagementDashboard.Models.NoRetentionDeposit> filteredModel = model.Where(x => x.Value == 0).ToList();
            return PartialView(filteredModel);
        }

     
  


        [OutputCache(Duration = MD_CONST_DURATIONS.OUTPUTCASH_DURATION)]
        public PartialViewResult TransactionCodes()
        {

            var db = new DBConnect();
            string query = "SELECT count(*) as 'Count', case cpr_loaded_at_hyphen " +
                "when 0 then 'Not Sent for Loading' when 1 then 'Waiting on Confirmation' " +
                "when 2 then 'Confirmed' else cpr_loaded_at_hyphen end as 'State' FROM `tbl_customer_profile` " +
                "group by cpr_loaded_at_hyphen";
            var model = new List<ManagementDashboard.Models.TransactionCodes>();
            var result = db.Query(query);

            foreach (DataRow dRow in result.Tables[0].Rows)
            {
                var tranCod = new Models.TransactionCodes();
                tranCod.Count = (int)dRow.Field<Int64>("Count");
                tranCod.State = dRow.Field<string>("State");
                model.Add(tranCod);
            }
            return PartialView(model);

        }

        [OutputCache(Duration = MD_CONST_DURATIONS.OUTPUTCASH_DURATION, VaryByParam = "id")]
        public PartialViewResult NewGrowth(int id)
        {
    
            int monthSelected = id;
            if (id > 0)
                monthSelected = -1 * id;
            DateTime currentDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(monthSelected);
            DateTime prevStartDate = currentDate.AddMonths(-2);
            DateTime prevEndDateTime = currentDate.AddMonths(-1).AddDays(-1);
            DateTime nextStartDate = currentDate.AddMonths(-1);
            DateTime nextEndDateTime = currentDate.AddDays(-1);

            var db = new DBConnect();
            string query = $"select dbt_comref as 'Ref', sum(if (dbt_date between '{prevStartDate.ToString("yyyy-MM-dd")}' and " +
                $"'{prevEndDateTime.ToString("yyyy-MM-dd")}' ,dbt_amount,0)) as 'Prev', " +
                $"sum(if (dbt_date between '{nextStartDate.ToString("yyyy-MM-dd")}' and " +
                $"'{nextEndDateTime.ToString("yyyy-MM-dd")}' ,dbt_amount,0)) as 'Next' from tbldebits dd left join tblrbr on rbr_id = dbt_rbr where " +
                $"dbt_date between '{prevStartDate.ToString("yyyy-MM-dd")}' and '{nextEndDateTime.ToString("yyyy-MM-dd")}' and rbr_status not in (99) group by dbt_comref";

      
            var model = new List<ManagementDashboard.Models.NewGrowth>();
            var result = db.Query(query);

            foreach (DataRow dRow in result.Tables[0].Rows)
            {
                
                var newGro = new Models.NewGrowth
                {
                    CustomerReference = dRow.Field<string>("Ref"), //Customer Reference
                    PrevValue = (int)dRow.Field<decimal>("Prev"), //Prev. Month Collection
                    NextValue = (int)dRow.Field<decimal>("Next"), //
                    PrevDateTime = prevEndDateTime,
                    NextDatetime = nextEndDateTime,
                    RiskMulti = 1
                };
                
                //Don't Include customers with 0 value and no change
                if (newGro.PrevValue == 0 && newGro.NextValue == 0)
                    continue;

                newGro.Calculate();
                if (newGro.IsOverRiskValue)
                    model.Add(newGro);

            }

            model = model.OrderByDescending(x => x.Percent)
                .ThenBy(x => x.CustomerReference)
                //.Take(400)
                .ToList();

            return PartialView(model);

        }


    
    }
}