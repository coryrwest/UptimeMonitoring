using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using UptimeMonitoring.Models;

namespace UptimeMonitoring.Controllers
{
    public class SiteCheck
    {
        UptimeMonitorDb site_db = new UptimeMonitorDb();

        public void CheckSite(string User)
        {
            var query =
                site_db.Sites
                .Where(r => r.user == User);

            foreach (SiteModel site in query)
            {
                var request = (HttpWebRequest)WebRequest.Create(site.site_url);
                request.Method = "HEAD";
                var response = (HttpWebResponse)request.GetResponse();
                site.result = (int)response.StatusCode;
                DateTime Now = DateTime.Now;
                site.site_last_check = DateTime.SpecifyKind(Now, DateTimeKind.Local).ToString();
                if (site.result >= 200 && site.result <= 299)
                    site.site_online = "Yes";
                else
                    site.site_online = "No";
            }

            site_db.SaveChanges();
        }

        public void CheckSite(int Frequency)
        {
            var query =
                site_db.Sites
                .Where(r => r.frequency == Frequency);

            foreach (SiteModel site in query)
            {
                var request = (HttpWebRequest)WebRequest.Create(site.site_url);
                request.Method = "HEAD";
                var response = (HttpWebResponse)request.GetResponse();
                site.result = (int)response.StatusCode;
                DateTime Now = DateTime.Now;
                site.site_last_check = DateTime.SpecifyKind(Now, DateTimeKind.Local).ToString();
                if (site.result >= 200 && site.result <= 299)
                    site.site_online = "Yes";
                else
                    site.site_online = "No";
            }

            site_db.SaveChanges();
        }
    }
}