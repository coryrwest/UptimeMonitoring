using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UptimeMonitoring.Resources;
using UptimeMonitoring.Models;
using UptimeMonitoring.Filters;
using Utilities;
using Utilities.Email;
using Utilites_CW;

namespace UptimeMonitoring.Controllers
{
    public class Emailer
    {
        EmailHelper emailer = new EmailHelper();

        public void SendDownEmail(string Email, string Name, string Url, string Code)
        {
            EmailMessage downEmail = new EmailMessage()
            {
                To = Email,
                From = "cory@westroppstudios.com",
                Subject = "One of your monitors is offline.",
                Body = Emails.DownEmail.Replace("{USER}", HttpContext.Current.User.Identity.Name).Replace("{NAME}", Name).Replace("{SITE}", "Site Name: " + Name + "  |  Site URL: " + Url + "  |  Status code:" + Code)
            };

            emailer.SendEmail(downEmail);
        }
    }
}