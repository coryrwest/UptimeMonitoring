using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using UptimeMonitoring.Models;

namespace UptimeMonitoring.Controllers
{
    public class SiteCheck
    {
        UptimeMonitorDb site_db = new UptimeMonitorDb();
        UsersContext user_db = new UsersContext();

        public void CheckSite(string User)
        {
            var query =
                site_db.Sites
                .Where(r => r.user == User);

            var user =
                user_db.UserProfiles
                .Where(r => r.UserName == User);

            foreach (SiteModel site in query)
            {
                var request = (HttpWebRequest) WebRequest.Create(site.site_url);
                request.Method = "HEAD";
                try
                {
                    var response = (HttpWebResponse) request.GetResponse();
                    site.result = (int) response.StatusCode;
                }
                catch (Exception)
                {
                    site.result = 0;
                }
                DateTime Now = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Local);
                string NowFormatted = Now.ToString("M/d/yy - h:mm tt");
                site.site_last_check = NowFormatted;
                if (site.result >= 200 && site.result <= 299)
                    site.site_online = "Yes";
                else
                {
                    site.site_online = "No";
                    SendEmail("cory.r.west@gmail.com", site.site_name, site.site_url, site.result.ToString());
                }
            }
            site_db.SaveChanges();
        }

        public void CheckOneSite(int id)
        {
            var query =
                site_db.Sites
                .Where(r => r.Id == id);

            foreach (SiteModel site in query)
            {
                var request = (HttpWebRequest)WebRequest.Create(site.site_url);
                request.Method = "HEAD";
                try
                {
                    var response = (HttpWebResponse)request.GetResponse();
                    site.result = (int)response.StatusCode;
                }
                catch (Exception)
                {
                    site.result = 0;
                }
                DateTime Now = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Local);
                string NowFormatted = Now.ToString("M/d/yy - h:mm tt");
                site.site_last_check = NowFormatted;
                if (site.result >= 200 && site.result <= 299)
                    site.site_online = "Yes";
                else
                {
                    site.site_online = "No";
                    SendEmail("cory.r.west@gmail.com", site.site_name, site.site_url, site.result.ToString());
                }
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
                try
                {
                    var response = (HttpWebResponse)request.GetResponse();
                    site.result = (int)response.StatusCode;
                }
                catch (Exception)
                {
                    site.result = 0;
                }
                DateTime Now = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Local);
                string NowFormatted = Now.ToString("M/d/yy - h:mm tt");
                site.site_last_check = NowFormatted;
                if (site.result >= 200 && site.result <= 299)
                    site.site_online = "Yes";
                else
                {
                    site.site_online = "No";
                    SendEmail("cory.r.west@gmail.com", site.site_name, site.site_url, site.result.ToString());
                }
            }

            site_db.SaveChanges();
        }

        public void SendEmail(string Email, string Name, string Site, string Code)
        {
            Emailer emailer = new Emailer();
            emailer.SendDownEmail(Email, Name, Site, Code);
        }
    }
}