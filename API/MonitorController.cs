using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using UptimeMonitoring.Models;

namespace UptimeMonitoring.API
{
    public class MonitorController : ApiController
    {
        UptimeMonitorDb site_db = new UptimeMonitorDb();

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
        }
    }
}
