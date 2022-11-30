using System;

namespace ManagementDashboard.Models
{
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