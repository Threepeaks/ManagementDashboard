using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ManagementDashboard.Services;

namespace ManagementDashboard.Attributes
{
    public class LogUserAccessAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var user = filterContext.HttpContext.User?.Identity?.Name ?? "Anonymous";
            var controller = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            var action = filterContext.ActionDescriptor.ActionName;
            var method = filterContext.HttpContext.Request.HttpMethod;
            var url = filterContext.HttpContext.Request.RawUrl;

            // Example: write to log file, DB, or your logger
            System.Diagnostics.Debug.WriteLine(
                $"[{DateTime.Now}] User '{user}' accessed {method} {controller}/{action} at {url}"
            );
            var userActionLog = new Models.UserActionLog
            {
                DateTimeUtc = DateTime.UtcNow,
                UserName = user,
                ControllerName = controller,
                ActionName = action,
                HttpMethod = method,
                UrlAccessed = url,
                AccessedAt = DateTime.Now
            };

            UserActionLogBuffer.Enqueue(userActionLog);

            base.OnActionExecuting(filterContext);
        }
    }
}
