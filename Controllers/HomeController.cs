using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UptimeMonitoring.Filters;
using Utilites_CW;

namespace UptimeMonitoring.Controllers
{
    [InitializeSimpleMembership]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Monitor website for downtime.";

            return View();
        }
    }
}
