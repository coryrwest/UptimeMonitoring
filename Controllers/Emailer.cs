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

        public List<string> SendDownEmail(string Name, string Url, string Code)
        {
            List<string> status = new List<string>();
            EmailMessage downEmail = new EmailMessage()
            {
                To = "cory.r.west@gmail.com",
                From = "cory@westroppstudios.com",
                Subject = "One of your monitors is offline.",
                Body = Emails.DownEmail.Replace("{USER}", HttpContext.Current.User.Identity.Name).Replace("{NAME}", Name).Replace("{URL}", Url).Replace("{CODE}", Code)
            };

            emailer.SendEmail(downEmail);
            status.Add("Email Sent");

            return status;
        }
    }
}