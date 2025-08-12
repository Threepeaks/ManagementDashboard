using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;

namespace ManagementDashboard.Helpers
{
    public static class DateHelpers
    {
        //ToSAShortDate dd/MM/yyyy
        public static string ToSAShortDate(this DateTime date)
        {
            return date.ToString("dd/MM/yyyy");
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
