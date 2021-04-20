using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ManagementDashboard.Models
{
    public class DepositBalance
    {

        public string Customer { get; set; }
        public decimal Amount { get; set; }
    }

    public class NoRunClients
    {
        public string Ref { get; set; }
        public DateTime StartDate { get; set; }
        public string Customer { get; set; }
    }

    public class CanceledClients
    {
        public string Ref { get; set; }

        public string Customer { get; set; }
        public string Comment { get; set; }

        public DateTime CancelStart { get; set; }
        public DateTime CancelEnd { get; set; }

    }

    public class InCancelation
    {
        public string Ref { get; set; }

        public string Customer { get; set; }
        public string Comment { get; set; }

        public string PendingStatus { get; set; }
        public string CancelStatus { get; set; }

    }
    public class RetNotReleased
    {
        public string Ref { get; set; }
        public int Rbr { get; set; }
        public DateTime ActionDate { get; set; }
        public DateTime ReleaseDate { get; set; }
        public decimal RetAmount { get; set; }
        public int Age { get; set; }

    }
    public class ManagementFees
    {
        public DateTime UnpaidDate { get; set; }
        public string CustomerReference { get; set; }
        public string Code { get; set; }
        public string Type { get; set; }
        public decimal Amount { get; set; }
        public string   Reason { get; set; }

    }

    public class PendingClients
    {
        public string Ref { get; set; }

        public string Customer { get; set; }

        public DateTime PendingDate { get; set; }
    }

    public class NoRetentionDeposit
    {
        public string Ref { get; set; }
        public string Customer { get; set; }
        public string Collateral { get; set; }
        public string HaveRuns { get; set; }
        public decimal Value { get; set; }

    }

    public class DormantClients
    {
        public string Ref { get; set; }
        public string State { get; set; }
        public DateTime PrevRun { get; set; }

        public string Next { get; set; }
        public int Span { get; set; }
    }

    public class HighestCustomerValue
    {
        public string Ref { get; set; }
        public DateTime CancelStart { get; set; }
        public DateTime CancelEnd { get; set; }
        public decimal Value { get; set; }
        public int Count { get; set; }
    }


    public class ListOfPendingClient
    {
        public string Ref { get; set; }
        public string Customer { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string PendingStatus { get; set; }
        public string Service { get; set; }
        public string SalesAgent { get; set; }
    }

    public class uManageCustomers
    {
        public string Ref { get; set; }
        public string Customer { get; set; }
        public int NoRuns { get; set; }
        public decimal TotalRecords { get; set; }

    }
    public class NewClients
    {
        public string Ref { get; set; }
        public string Customer { get; set; }

        public DateTime StartDate { get; set; }
    } 
    public class TransactionCodes
    {
        public string State { get; set; }
        public int Count { get; set; }
    }


    public enum DiffEnum
    {
        None = 0,
        Increase = 1,
        Decrease = 2

    }

    public class NewGrowth
    {

        public string CustomerReference { get; set; }
        public decimal Percent { get; set; }
        
        public decimal PrevValue { get; set; }
        public DateTime PrevDateTime { get; set; }
        public decimal NextValue { get; set; }
        public DateTime NextDatetime { get; set; }
        public decimal Difference { get; set; }

        public DiffEnum Type { get; set; }

        public bool IsOverRiskValue { get; set; }
        public decimal RiskMulti { get; set; }


        public string GetTypeEnumString()
        {
            switch (this.Type)
            {
                case DiffEnum.None:
                    return "No Change";
                    
                case DiffEnum.Increase:
                    return "Increase";
                    
                case DiffEnum.Decrease:
                    return "Descrease";
                    
                default:
                    return "Unknown";
            }
        }

        internal void Calculate()
        {
            //Calc 10% of Prev Value

            decimal prevPercentageValue = PrevValue * (RiskMulti / 100);
            decimal diff = NextValue - PrevValue;
            this.Type = DiffEnum.None;

            if (diff < 0)
            {
                this.Type = DiffEnum.Decrease;
                this.Difference = -1 * diff;
            }
            if (diff > 0)
            {
                this.Type = DiffEnum.Increase;
                this.Difference =  diff;
            }

            if (PrevValue == 0)
            {
                this.Percent = 100;
            }
            else
            {
                //(N - P) / P
                //this.Percent = ( NextValue/  (NextValue - PrevValue)  ) * 100;
                this.Percent = ((NextValue - PrevValue) / PrevValue) * 100;


            }
            if (this.Difference >= prevPercentageValue)
            {
                IsOverRiskValue = true;
            }





        }
    }
}