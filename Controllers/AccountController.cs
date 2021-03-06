﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DotNetOpenAuth.AspNet;
using Microsoft.Web.WebPages.OAuth;
using WebMatrix.WebData;
using UptimeMonitoring.Filters;
using UptimeMonitoring.Models;
using System.Data.Entity;
using System.Net;
using Utilities.Email;

namespace UptimeMonitoring.Controllers
{
    [Authorize]
    [InitializeSimpleMembership]
    public class AccountController : Controller
    {
        // REFACTOR CRAPPY CODING
        // GET: /Account/Status
        
        UptimeMonitorDb site_db = new UptimeMonitorDb();
        UsersContext user_db = new UsersContext();

        public ActionResult Status()
        {
            var model =
                site_db.Sites
                .OrderBy(r => r.site_last_check)
                .Where(r => r.user == User.Identity.Name);

            StatusModel statusModel = new StatusModel();
            List<SiteModel> listSiteModel = new List<SiteModel>();
            SiteModel createSiteModel = new SiteModel();

            foreach (SiteModel siteModel in model)
            {
                listSiteModel.Add(siteModel);
            }

            statusModel.ListSiteModel = listSiteModel;
            statusModel.CreateSiteModel = createSiteModel;

            SelectList freq = new SelectList(new[] { "5", "10", "15" });
            ViewData["freq"] = freq;

            return View(statusModel);
        }

        //
        // POST: /Account/Status

        [HttpPost]
        public ActionResult Status(StatusModel model)
        {
            SiteModel createModel = model.CreateSiteModel;
            var query = site_db.Sites;
            createModel.user = User.Identity.Name;

            if (ModelState.IsValid)
            {
                query.Add(createModel);

                site_db.SaveChanges();

                SiteCheck check = new SiteCheck();
                check.CheckSite(User.Identity.Name);

                return Content("Success");
            }
            else
            {
                return Content("Something Went Wrong!");
            }
        }

        // REFACTOR CRAPPY CODING
        // GET: /Account/StatusTable

        public ActionResult StatusTable()
        {

            SiteCheck check = new SiteCheck();
            check.CheckSite(User.Identity.Name);

            var model =
                site_db.Sites
                    .OrderBy(r => r.site_last_check)
                    .Where(r => r.user == User.Identity.Name);

            StatusModel statusModel = new StatusModel();
            List<SiteModel> listSiteModel = new List<SiteModel>();

            foreach (SiteModel siteModel in model)
            {
                listSiteModel.Add(siteModel);
            }

            statusModel.ListSiteModel = listSiteModel;

            return PartialView(statusModel);
        }

        //
        // DELETE: /Account/Delete/ID

        [HttpPost]
        public string Delete(int? id)
        {
            if (id == null)
            {
                return "No Such ID";
            }
            else
            {
                var query =
                    site_db.Sites.Single(r => r.Id == id && r.user == User.Identity.Name);

                if (query.user == User.Identity.Name)
                {
                    site_db.Sites.Remove(query);
                }
                else
                {
                    return "Fail";
                }

                site_db.SaveChanges();

                return "Success";
            }
        }

        //
        // POST: /Account/Start

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Start()
        {
            return RedirectToAction("Status");
        }

        //
        // GET: /Account/Login

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Feedback
        [HttpPost]
        [AllowAnonymous]
        public ActionResult Feedback(string feedback, string returnUrl)
        {
            EmailHelper emailer = new EmailHelper();
            EmailMessage feedbackemail = new EmailMessage()
            {
                To = "cory@westroppstudios.com",
                From = "cory@westroppstudios.com",
                Subject = "Feedback submission. " + User.Identity.Name,
                Body = feedback
            };

            emailer.SendEmail(feedbackemail);

            return RedirectToLocal(returnUrl);
        }

        //
        // POST: /Account/Login

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            if (ModelState.IsValid && WebSecurity.Login(model.UserName, model.Password, persistCookie: model.RememberMe))
            {
                return RedirectToAction("Status");
            }

            // If we got this far, something failed, redisplay form
            ModelState.AddModelError("", "The user name or password provided is incorrect.");
            return View(model);
        }

        //
        // POST: /Account/LogOff

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            WebSecurity.Logout();

            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/Register

        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                // Attempt to register the user
                try
                {
                    WebSecurity.CreateUserAndAccount(model.UserName, model.Password);
                    WebSecurity.Login(model.UserName, model.Password);
                    return RedirectToAction("Index", "Home");
                }
                catch (MembershipCreateUserException e)
                {
                    ModelState.AddModelError("", ErrorCodeToString(e.StatusCode));
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/Manage

        public ActionResult Manage(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : "";
            ViewBag.HasLocalPassword = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            ViewBag.ReturnUrl = Url.Action("Manage");
            return View();
        }

        //
        // POST: /Account/Manage

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Manage(LocalPasswordModel model, UserProfile model2)
        {
            bool hasLocalAccount = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            ViewBag.HasLocalPassword = hasLocalAccount;
            ViewBag.ReturnUrl = Url.Action("Manage");
            if (hasLocalAccount)
            {
                if (ModelState.IsValid)
                {
                    // ChangePassword will throw an exception rather than return false in certain failure scenarios.
                    bool changePasswordSucceeded;
                    try
                    {
                        changePasswordSucceeded = WebSecurity.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword);
                    }
                    catch (Exception)
                    {
                        changePasswordSucceeded = false;
                    }

                    if (changePasswordSucceeded)
                    {
                        return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
                    }
                    else
                    {
                        ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                    }


                }
            }
            else
            {
                // User does not have a local password so remove any validation errors caused by a missing
                // OldPassword field
                ModelState state = ModelState["OldPassword"];
                if (state != null)
                {
                    state.Errors.Clear();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        WebSecurity.CreateAccount(User.Identity.Name, model.NewPassword);
                        return RedirectToAction("Manage", new { Message = ManageMessageId.SetPasswordSuccess });
                    }
                    catch (Exception e)
                    {
                        ModelState.AddModelError("", e);
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        #region Helpers
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
        }

        internal class ExternalLoginResult : ActionResult
        {
            public ExternalLoginResult(string provider, string returnUrl)
            {
                Provider = provider;
                ReturnUrl = returnUrl;
            }

            public string Provider { get; private set; }
            public string ReturnUrl { get; private set; }

            public override void ExecuteResult(ControllerContext context)
            {
                OAuthWebSecurity.RequestAuthentication(Provider, ReturnUrl);
            }
        }

        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
        #endregion
    }
}
