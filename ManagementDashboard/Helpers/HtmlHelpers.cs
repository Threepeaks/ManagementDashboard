using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;

namespace ManagementDashboard.Helpers
{
    public class DateRangeRequest
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
    public static class DateHelpers
    {

        public static List<DateTime> NonProcessingDates { get => GetNonProcessingDates();  }
        private static List<DateTime> _nonProcessingDates { get; set; }
        static List<DateTime> GetNonProcessingDates()
        {
            if (_nonProcessingDates != null)
                return _nonProcessingDates;

            if (_nonProcessingDates == null)
                _nonProcessingDates = new List<DateTime>();

            //API Call to get NonProcessingDates could be placed here
            
            var db = new Controllers.DBConnect();
            string query = "select nrd_date from tblholdates";
            var ds = db.Query(query);
            foreach (System.Data.DataRow row in ds.Tables[0].Rows)
            {
                DateTime npdDate = Convert.ToDateTime(row["nrd_date"]);
                _nonProcessingDates.Add(npdDate.Date);
            }


            return _nonProcessingDates;

        }


        //ToSAShortDate dd/MM/yyyy
        public static string ToSAShortDate(this DateTime date)
        {
            return date.ToString("dd/MM/yyyy");
        }

        /// <summary>
        /// Gets the next action date from a given date, skipping weekends and non-processing dates
        /// </summary>
        /// <param name="fromDate">The starting date</param>
        /// <param name="value">The number of business days to skip</param>
        /// <returns>The next action date</returns>
        /// <exception cref="ArgumentException">Throws if value is negative</exception>
        public static DateTime GetNextActionDate(DateTime fromDate, int value)
        {
            if (value < 0)
                throw new ArgumentException("Value must be non-negative", nameof(value));
            var daysCounter = 0;
            var nextDate = fromDate;
            while (daysCounter < value)
            {
                nextDate = nextDate.AddDays(1);
                if (IsValidActionDate(nextDate))
                    daysCounter++;
            }
            return nextDate;
        }

        /// <summary>
        ///  Gets the previous action date from a given date, skipping weekends and non-processing dates
        /// </summary>
        /// <param name="fromDate">The starting date</param>
        /// <param name="value">The number of business days to skip</param>
        /// <returns>The previous action date</returns>
        /// <exception cref="ArgumentException">Throws if value is negative</exception>
        public static DateTime GetPreviousActionDate(DateTime fromDate, int value)
        {
            if (value < 0)
                throw new ArgumentException("Value must be non-negative", nameof(value));
            var daysCounter = 0;
            var previousDate = fromDate;
            while (daysCounter < value)
            {
                previousDate = previousDate.AddDays(-1);
                if (IsValidActionDate(previousDate))
                    daysCounter++;
            }
            return previousDate;
        }

        //Is Valid Action Date
        public static bool IsValidActionDate(DateTime date)
        {
            if (date.DayOfWeek == DayOfWeek.Sunday || NonProcessingDates.Contains(date.Date))
                return false;
            return true;
        }
    }
    public static class HtmlHelpers
    {
        public static MvcHtmlString LinkToTPMSWeb<TModel>(
             this HtmlHelper<TModel> htmlHelper,
             Expression<Func<TModel, string>> expression)
        {
            // Get the raw value of the model property
            var metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
            string value = metadata.Model as string ?? string.Empty;

            // Get the property name for extra context (optional)
            string propertyName = metadata.DisplayName ?? metadata.PropertyName;

            var html = "<a class='mr-2' href='https://tpmsweb.threepeaks.co.za/Clients/Profile/ByRef/" + value + "' target='_blank'><i class=\"bi bi-box-arrow-up-right\"></i></a>";


            return new MvcHtmlString(html);
        }
    }
}
