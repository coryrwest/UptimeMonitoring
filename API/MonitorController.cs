using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using UptimeMonitoring.Models;
using UptimeMonitoring.Controllers;

namespace UptimeMonitoring.API
{
    public class MonitorController : ApiController
    {
        UptimeMonitorDb site_db = new UptimeMonitorDb();

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
        }

        [HttpGet]
        public int SiteCheck(int freq, string code)
        {
            if (code == "8vn3879hc3")
            {
                SiteCheck check = new SiteCheck();
                check.CheckSite(freq);
                return 1;
            }
            else
            {
                return 0;
            }
        }
    }
}
