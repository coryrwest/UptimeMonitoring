using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
<<<<<<< HEAD
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using UptimeMonitoring.Models;
=======
using System.Web.Http;
using UptimeMonitoring.Models;
using UptimeMonitoring.Controllers;
>>>>>>> Ajax update and create

namespace UptimeMonitoring.API
{
    public class MonitorController : ApiController
    {
        UptimeMonitorDb site_db = new UptimeMonitorDb();

<<<<<<< HEAD
        [System.Web.Http.HttpGet]
        public string StatusUpdate(string ids)
        {
            //foreach (int id in ids)
            //{
            //    var query =
            //        site_db.Sites
            //            .Where(r => r.Id == id);
            //}

            List<int> idsToCheck = new List<int>();

            string[] checkIds = ids.Split(',');

            foreach (string checkId in checkIds)
            {
                idsToCheck.Add(Convert.ToInt32(checkId));
            }

            return checkIds[0];
=======
        public string StatusUpdate(string ids)
        {
            List<int> idsToCheck = new List<int>();

            string[] idsList = ids.Split(',');

            foreach (string s in idsList)
            {
                idsToCheck.Add(Convert.ToInt32(s));
            }

            SiteCheck check = new SiteCheck();
            foreach (int id in idsToCheck)
            {
                check.CheckOneSite(id);
            }

            return "Success";
>>>>>>> Ajax update and create
        }
    }
}
