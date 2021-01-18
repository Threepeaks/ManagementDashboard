using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ManagementDashboard.Controllers
{
    public class TopController : Controller
    {
        // GET: Top

        // Top 10 Value of Debit
        // Customer Reference / Debit Reference / Action Date / RBR /Amount

        public PartialViewResult GetTopValueDebits(int id)
        {
            int monthSelected = 0;
            if (id > 0)
                monthSelected = -1 * id;
            DateTime currentDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(monthSelected);
            DateTime startDate = currentDate;
            DateTime endDate = currentDate.AddMonths(1).AddDays(-1);


            var db = new DBConnect();
            string query = "select dbt_comref,dbt_ref,dbt_date,dbt_rbr,dbt_amount  from tbldebits left join tblrbr on rbr_id = dbt_rbr where rbr_status not in (99) and " +
                $"dbt_date between '{startDate.ToString("yyyy-MM-dd")}' and '{endDate.ToString("yyyy-MM-dd")}' order by dbt_amount  desc limit 20";
            var model = new List<ManagementDashboard.Models.GetTopDebitValue>(); // do you have a model yet  ??? ();
            var result = db.Query(query);

            foreach (DataRow dr in result.Tables[0].Rows)
            {
                var topItem = new Models.GetTopDebitValue();
                topItem.Amount = (decimal)dr.Field<decimal>("dbt_amount");
                topItem.CustomerRef = dr.Field<string>("dbt_comref");
                topItem.DebitRef = dr.Field<string>("dbt_ref");
                topItem.Rbr = (int)dr.Field<Int32>("dbt_rbr");
                topItem.ActionDate = (DateTime)dr.Field<DateTime>("dbt_date");

                model.Add(topItem);
            }
            return PartialView(model);
        }

        public PartialViewResult GetTopValueCustomers(int id)
        {
            int monthSelected = 0;
            if (id > 0)
                monthSelected = -1 * id;
            DateTime currentDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(monthSelected);
            DateTime startDate = currentDate;
            DateTime endDate = currentDate.AddMonths(1).AddDays(-1);

            var db = new DBConnect();
            string query = "select dbt_comref, sum(dbt_amount) as initial, count(*) as c from tbldebits  left join tblrbr on rbr_id = dbt_rbr where dbt_date between" +
                $"'{startDate.ToString("yyyy-MM-dd")}' and '{endDate.ToString("yyyy-MM-dd")}' and rbr_status not in (99) group by dbt_comref order by initial desc limit 10";
            var model = new List<ManagementDashboard.Models.GetTopValueCustomer>();
            var result = db.Query(query);

            foreach (DataRow dRow in result.Tables[0].Rows)
            {
                var topCus = new Models.GetTopValueCustomer();
                topCus.Customer = dRow.Field<string>("dbt_comref");
                topCus.Collections = (decimal)dRow.Field<decimal>("initial");
                topCus.NumOfRecords = (int)dRow.Field<Int64>("c");

                model.Add(topCus);

            }

            return PartialView(model);

        }

        public PartialViewResult GetTopCustomerRecords(int id)
        {
            int monthSelected = 0;
            if (id > 0)
                monthSelected = -1 * id;
            DateTime currentDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(monthSelected);
            DateTime startDate = currentDate;
            DateTime endDate = currentDate.AddMonths(1).AddDays(-1);

            var db = new DBConnect();
            string query = "select dbt_comref, sum(dbt_amount) as initial, count(*) as c from tbldebits  left join tblrbr on rbr_id = dbt_rbr where dbt_date between" +
                $"'{startDate.ToString("yyyy-MM-dd")}' and '{endDate.ToString("yyyy-MM-dd")}' and rbr_status not in (99) group by dbt_comref order by c desc limit 10";
            var model = new List<ManagementDashboard.Models.GetTopValueCustomer>();
            var result = db.Query(query);

            foreach (DataRow dRow in result.Tables[0].Rows)
            {
                var topCus = new Models.GetTopValueCustomer();
                topCus.Customer = dRow.Field<string>("dbt_comref");
                topCus.Collections = (decimal)dRow.Field<decimal>("initial");
                topCus.NumOfRecords = (int)dRow.Field<Int64>("c");

                model.Add(topCus);

            }

            return PartialView(model);
        }


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

        public PartialViewResult GetTop20Unpaids(int id)
        {
            int monthSelected = 0;
            if (id > 0)
                monthSelected = -1 * id;
            DateTime currentDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(monthSelected);
            DateTime startDate = currentDate;
            DateTime endDate = currentDate.AddMonths(1).AddDays(-1);

            var db = new DBConnect();
            string query = "select dbt_comref as 'Company Ref', count(*) as 'Unpaids' from tbldebits " +
                "where dbt_pass_unpaid in (2,3) and dbt_pass_unpaid_datetime between " +
               $"'{startDate.ToString("yyyy-MM-dd")}' and '{endDate.ToString("yyyy-MM-dd")}' group by " +
                "dbt_comref order by `Unpaids` desc limit 20";
            var model = new List<ManagementDashboard.Models.GetTop20Unpaids>();
            var result = db.Query(query);


            foreach (DataRow dRow in result.Tables[0].Rows)
            {
                var t20unpaid = new Models.GetTop20Unpaids();
                t20unpaid.Customer = dRow.Field<string>("Company Ref");
                t20unpaid.Unpaids = (int)dRow.Field<Int64>("Unpaids");

                model.Add(t20unpaid);

            }

            return PartialView(model);

        }

        public PartialViewResult NoRunClients(int id)
        {
            int monthSelected = 0;
            if (id > 0)
                monthSelected = -1 * id;
            DateTime currentDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(monthSelected);
            DateTime startDate = currentDate;
            DateTime endDate = currentDate.AddMonths(-1).AddDays(-1);

            var db = new DBConnect();
            string query = "select com_ref as Ref ,com_startdate as StartDate, com_name as Customer from tblcompany" +
                " where com_acc_cancel in (0,1) and (select count(*) from tblrbr where rbr_comref = com_ref and " +
                $"rbr_status not in (99) and rbr_date <= '{endDate.ToString("yyyy-MM-dd")}' limit 1) = 0 and com_startdate > '{startDate.ToString("yyyy-MM-dd")}'";
            var model = new List<ManagementDashboard.Models.NoRunClients>();
            var result = db.Query(query);

            foreach (DataRow dRow in result.Tables[0].Rows)
            {
                var depMov = new Models.NoRunClients();
                depMov.Ref = dRow.Field<string>("Ref");
                depMov.StartDate = (DateTime)dRow.Field<DateTime>("StartDate");
                depMov.Customer = dRow.Field<string>("Customer");
                model.Add(depMov);
            }
            return PartialView(model);

        }

        public PartialViewResult CanceledClients(int id)
        {
            int monthSelected = 0;
            if (id > 0)
                monthSelected = -1 * id;
            DateTime currentDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(monthSelected);
            DateTime startDate = currentDate;
            DateTime endDate = currentDate.AddMonths(1).AddDays(-1);

            var db = new DBConnect();
            string query = "select com_ref as 'Ref' , com_name as 'Customer', com_acc_cancel_date as 'Cancel Start',com_acc_cancel_enddate as 'Cancel End', com_acc_cancel_note as 'Comment' " +
                $"FROM threepeaks_tpms.tblcompany where com_acc_cancel = 2 and com_acc_cancel_enddate between '{startDate.ToString("yyyy-MM-dd")}' and '{endDate.ToString("yyyy-MM-dd")}'";
            var model = new List<ManagementDashboard.Models.CanceledClients>();
            var result = db.Query(query);

            foreach (DataRow dRow in result.Tables[0].Rows)
            {
                var depMov = new Models.CanceledClients();
                depMov.Ref = dRow.Field<string>("Ref");
                depMov.Customer = dRow.Field<string>("Customer");
                depMov.CancelStart = (DateTime)dRow.Field<DateTime>("Cancel Start");
                depMov.CancelEnd = (DateTime)dRow.Field<DateTime>("Cancel End");
                depMov.Comment = dRow.Field<string>("Comment");
                model.Add(depMov);
            }
            return PartialView(model);

        }
        public PartialViewResult InCancelation(int id)
        {
            int monthSelected = 0;
            if (id > 0)
                monthSelected = -1 * id;
            DateTime currentDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(monthSelected);
            DateTime startDate = currentDate;
            DateTime endDate = currentDate.AddMonths(1).AddDays(-1);

            var db = new DBConnect();
            string query = "select com_ref as 'Customer Ref', com_name as 'Company Name', case com_ac_pending " +
                "when 0 then '' when 1 then 'Pending' when 2 then 'Pending' end as 'Pending Status', case com_acc_cancel " +
                "when 0 then 'Active' when 1 then 'Cancelling' when 2 then 'Cancelled' else '' end as 'Cancel Status', " +
                "com_acc_cancel_note as 'Comment' from `tblcompany` where com_acc_cancel = 1";

            var model = new List<ManagementDashboard.Models.InCancelation>();
            var result = db.Query(query);

            foreach (DataRow dRow in result.Tables[0].Rows)
            {
                var inCancel = new Models.InCancelation();
                inCancel.Ref = dRow.Field<string>("Customer Ref");
                inCancel.Customer = dRow.Field<string>("Company Name");
                inCancel.CancelStatus = dRow.Field<string>("Cancel Status");
                inCancel.PendingStatus = dRow.Field<string>("Pending Status");
                inCancel.Comment = dRow.Field<string>("Comment");
                model.Add(inCancel);
            }
            return PartialView(model);

        }



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

        public PartialViewResult PendingClients(int id)
        {
            int monthSelected = 0;
            if (id > 0)
                monthSelected = -1 * id;
            DateTime currentDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(monthSelected);
            DateTime startDate = currentDate;
            DateTime endDate = currentDate.AddMonths(1).AddDays(-1);

            var db = new DBConnect();
            string query = "select com_Ref as 'Ref', com_name as 'Customer',com_ac_pending_date as 'Pending Date' from tblcompany " +
                $"where com_ac_pending = 1 and com_acc_cancel != 2 and com_ac_pending_date between '{startDate.ToString("yyyy-MM-dd")}' and '{endDate.ToString("yyyy-MM-dd")}'";
            var model = new List<ManagementDashboard.Models.PendingClients>();
            var result = db.Query(query);

            foreach (DataRow dRow in result.Tables[0].Rows)
            {
                var PenCus = new Models.PendingClients();
                PenCus.Ref = dRow.Field<string>("Ref");
                PenCus.Customer = dRow.Field<string>("Customer");
                PenCus.PendingDate = (DateTime)dRow.Field<DateTime>("Pending Date");

                model.Add(PenCus);

            }

            return PartialView(model);
        }

        public PartialViewResult ListOfPendingClient()
        {
            var db = new DBConnect();
            string query = "select com_ref as 'Customer Ref', com_name as 'Company Name', com_ac_pending_date as 'Start Date', " +
                " com_acc_pending_flagdate as 'End Date',  case com_ac_pending when 0 then '' when 1 then 'Pending' when 2 then 'Pending' end as 'Pending Status', " +
                "case com_acc_cancel when 0 then 'Active' when 1 then 'Cancelling' when 2 then 'Cancelled' else '' end as 'CancelCancel Status'," +
                " srv_name as 'Service', rep_name as 'Sales Agent' from tblcompany  left join tblservice on srv_id = com_serviceid left join " +
                "tblrep on rep_id = com_repid where com_acc_cancel = 0 and com_ac_pending<> 0";
            var model = new List<ManagementDashboard.Models.ListOfPendingClient>();
            var result = db.Query(query);

            foreach (DataRow dRow in result.Tables[0].Rows)
            {
                var LisPenCus = new Models.ListOfPendingClient();
                LisPenCus.Ref = dRow.Field<string>("Customer Ref");
                LisPenCus.Customer = dRow.Field<string>("Company Name");
                LisPenCus.StartDate = (DateTime)dRow.Field<DateTime>("Start Date");
                LisPenCus.EndDate = (DateTime)dRow.Field<DateTime>("End Date");
                LisPenCus.PendingStatus = dRow.Field<string>("Pending Status");
                LisPenCus.Service = dRow.Field<string>("Service");
                LisPenCus.SalesAgent = dRow.Field<string>("Sales Agent");
                model.Add(LisPenCus);

            }
            return PartialView(model);
        }

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

        public PartialViewResult DormantClients(int id)
        {
            int monthSelected = 0;
            if (id > 0)
                monthSelected = -1 * id;
            DateTime currentDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(monthSelected);
            DateTime startDate = currentDate;
            DateTime endDate = currentDate.AddMonths(1).AddDays(-1);
            DateTime today = DateTime.Now.Date;
            int span = 45;

            var db = new DBConnect();
            string query = $"select com_ref as 'Ref', case com_acc_cancel when 0 then if (com_ac_pending=1, if(com_acc_pending_flagdate < '{startDate.ToString("yyyy - MM - dd")}','Pending Ended', " +
                $"if (com_ac_pending_date > '{endDate.ToString("yyyy-MM-dd")}','Active', 'Pending')), 'Active') when 1 then 'In Cancellation' when 2 then " +
                $"if (com_acc_cancel_enddate < '{startDate.ToString("yyyy-MM-dd")}','Cancelled Before', if (com_acc_cancel_date < '{startDate.ToString("yyyy-MM-dd")}', " +
                $"'In Cancellation', if (com_acc_cancel_date < '{endDate.ToString("yyyy-MM-dd")}' ,'In Cancellation','Active'))) else -1 end as 'State', " +
                $"(select max(rbr_date) from tblrbr where rbr_status not in (99) and rbr_comref = com_ref and rbr_date< '{startDate.ToString("yyyy-MM-dd")}') as 'Prev Run', " +
                $"ifnull((select min(rbr_date) from tblrbr where rbr_status not in (99) and rbr_comref = com_ref and rbr_date between '{startDate.ToString("yyyy-MM-dd")}' " +
                $"and '{endDate.ToString("yyyy-MM-dd")}'),'na') as 'Next',  datediff((select max(rbr_date) from tblrbr where rbr_status not in (99) and  rbr_comref = com_ref " +
                $"and rbr_date < '{startDate.ToString("yyyy-MM-dd")}'), '{today.ToString("yyyy-MM-dd")}') as 'span' from tblcompany where (select min(rbr_date) from tblrbr " +
                $"where rbr_status not in (99) and rbr_comref = com_ref) < '{startDate.ToString("yyyy-MM-dd")}' and(select count(rbr_id) from tblrbr where rbr_status not in " +
                $"(99) and  rbr_comref = com_ref and rbr_date between '{startDate.ToString("yyyy-MM-dd")}' and '{endDate.ToString("yyyy-MM-dd")}') = 0 and if (com_acc_cancel = 2 , " +
                $"if (com_acc_cancel_enddate < '{startDate.ToString("yyyy-MM-dd")}', 0,if (com_acc_cancel_date < '{startDate.ToString("yyyy-MM-dd")}', 1, " +
                $"if (com_acc_cancel_date < '{endDate.ToString("yyyy-MM-dd")}' ,1,1))),1) = 1 and com_acc_cancel = 0 and com_ac_pending = 0 and datediff((select max(rbr_date) " +
                $" from tblrbr where rbr_status not in (99) and rbr_comref = com_ref and rbr_date< '{startDate.ToString("yyyy-MM-dd")}'), " +
                $"'{endDate.ToString("yyyy-MM-dd")}') <= (-1 * '{span}') order by span, ifnull((select min(rbr_date) from tblrbr where rbr_status not in (99) " +
                $"and  rbr_comref = com_ref and rbr_date > '{endDate.ToString("yyyy-MM-dd")}'),'0000-00-00'), (select max(rbr_date) from tblrbr  where rbr_status not in " +
                $"(99) and rbr_comref = com_ref and rbr_date< '{startDate.ToString("yyyy-MM-dd")}'), com_ref";

            var model = new List<ManagementDashboard.Models.DormantClients>();
            var result = db.Query(query);

            foreach (DataRow dRow in result.Tables[0].Rows)
            {
                var domCus = new Models.DormantClients();
                domCus.Ref = dRow.Field<string>("Ref");
                domCus.State = dRow.Field<string>("State");
                domCus.PrevRun = (DateTime)dRow.Field<DateTime>("Prev Run");
                domCus.Next = dRow.Field<string>("Next");
                Type t = dRow["span"].GetType();
                domCus.Span = (int)dRow.Field<Int32>("span");

                model.Add(domCus);
            }
            return PartialView(model);
        }

        public PartialViewResult HighestCustomerValue(int id)
        {
            int monthSelected = 0;
            if (id > 0)
                monthSelected = -1 * id;
            DateTime currentDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(monthSelected);
            DateTime startDate = currentDate;
            DateTime endDate = currentDate.AddMonths(1).AddDays(-1);

            var db = new DBConnect();
            string query = "select dbt_comref as 'Ref', count(*) as 'Count', sum(dbt_amount) as 'Value' from tbldebits " +
                $"left join tblrbr on rbr_id = dbt_rbr where dbt_date between '{startDate.ToString("yyyy-MM-dd")}' and" +
                $"'{endDate.ToString("yyyy-MM-dd")}' " +
                $"and rbr_status<> 99 and dbt_cdv = 3 and dbt_accrej = 3 group by dbt_comref order by count(*) desc limit 20";
            var model = new List<ManagementDashboard.Models.HighestCustomerValue>();
            var result = db.Query(query);

            foreach (DataRow dRow in result.Tables[0].Rows)
            {
                var highCus = new Models.HighestCustomerValue();
                highCus.Ref = dRow.Field<string>("Ref");
                highCus.Count = (int)dRow.Field<Int64>("Count");
                highCus.Value = (int)dRow.Field<decimal>("Value");
                model.Add(highCus);
            }
            return PartialView(model);

        }


        public PartialViewResult uManageCustomers(int id)
        {
            int monthSelected = 0;
            if (id > 0)
                monthSelected = -1 * id;
            DateTime currentDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(monthSelected);
            DateTime startDate = currentDate;
            DateTime endDate = currentDate.AddMonths(1).AddDays(-1);

            var db = new DBConnect();
            string query = "select rbr_comref as 'Customer Refernece', com_name as 'Company', count(*) as 'No Of Runs', " +
                "sum(rbr_total_records) as 'Total Records' from tblrbr left join tblcompany on com_ref = rbr_comref where " +
                $"rbr_subtype = 'File' and rbr_status not in (99) and rbr_date between '{startDate.ToString("yyyy-MM-dd")}' and '{endDate.ToString("yyyy-MM-dd")}' group by rbr_comref ";
            var model = new List<ManagementDashboard.Models.uManageCustomers>();
            var result = db.Query(query);

            foreach (DataRow dRow in result.Tables[0].Rows)
            {
                var uMan = new Models.uManageCustomers();
                uMan.Ref = dRow.Field<string>("Customer Refernece");
                uMan.Customer = dRow.Field<string>("Company");
                uMan.NoRuns = (int)dRow.Field<Int64>("No Of Runs");
                uMan.TotalRecords = (int)dRow.Field<decimal>("Total Records");
                model.Add(uMan);
            }
            return PartialView(model);

        }

        public PartialViewResult NewClients(int id)
        {
            int monthSelected = 0;
            if (id > 0)
                monthSelected = -1 * id;
            DateTime currentDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(monthSelected);
            DateTime startDate = currentDate;
            DateTime endDate = currentDate.AddMonths(1).AddDays(-1);

            var db = new DBConnect();
            string query = "SELECT com_ref as 'Customer Refernece', com_name as 'Company', com_startdate as 'Start Date'  FROM " +
                $"threepeaks_tpms.tblcompany where com_startdate between '{startDate.ToString("yyyy-MM-dd")}' and '{endDate.ToString("yyyy-MM-dd")}'";
            var model = new List<ManagementDashboard.Models.NewClients>();
            var result = db.Query(query);

            foreach (DataRow dRow in result.Tables[0].Rows)
            {
                var nClient = new Models.NewClients();
                nClient.Ref = dRow.Field<string>("Customer Refernece");
                nClient.Customer = dRow.Field<string>("Company");
                nClient.StartDate = (DateTime)dRow.Field<DateTime>("Start Date");
                model.Add(nClient);
            }
            return PartialView(model);

        }


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
        public PartialViewResult GetTopUnpaids(int id)
        {
            int monthSelected = 0;
            if (id > 0)
                monthSelected = -1 * id;
            DateTime currentDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(monthSelected);
            DateTime startDate = currentDate;
            DateTime endDate = currentDate.AddMonths(1).AddDays(-1);

            var db = new DBConnect();
            string query = "select hec_description as Description , hec_code as Code, count(*) as Count ,sum(dbt_amount) as amount from tbldebits left join tblhyphen_errcodes on dbt_accrejcode = hec_code left join tblrbr on rbr_id = dbt_rbr where dbt_pass_unpaid in (2,3) and rbr_status not in (99) " +
                $" and rbr_date between '{startDate.ToString("yyyy-MM-dd")}' and '{endDate.ToString("yyyy-MM-dd")}' group by hec_code";
            var model = new List<ManagementDashboard.Models.GetTopUnpaids>();
            var result = db.Query(query);

            //got an issue with this query... OK will help just now

            foreach (DataRow dRow in result.Tables[0].Rows)
            {
                var depMov = new Models.GetTopUnpaids();
                depMov.Description = dRow.Field<string>("Description");
                depMov.Code = dRow.Field<string>("Code");
                //Type t = dRow["Count"].GetType();
                depMov.Count = (int)dRow.Field<Int64>("Count");
                depMov.Amount = (int)dRow.Field<decimal>("amount");


                model.Add(depMov);

            }

            return PartialView(model);

        }
    }
}