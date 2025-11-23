using ManagementDashboard.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ManagementDashboard.Controllers
{
    [Authorize]
    [LogUserAccess]
    public class ServiceDigitalMandatesController : Controller
    {
        // GET: ServiceDigitalMandates
        public ActionResult Index()
        {
            return View();
        }
    }
}