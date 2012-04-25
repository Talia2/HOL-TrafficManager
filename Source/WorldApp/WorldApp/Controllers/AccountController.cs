using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using WorldApp.Models;

namespace WorldApp.Controllers
{
    [HandleError]
    public class AccountController : Controller
    {
        public IFormsAuthenticationService FormsService { get; set; }

        public IMembershipService MembershipService { get; set; }

        //// **************************************
        //// URL: /Account/LogOn
        //// **************************************

        public ActionResult LogOn()
        {
            return View();
        }

        [HttpPost]
        public ActionResult LogOn(LogOnModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                if (this.MembershipService.ValidateUser(model.UserName, model.Password))
                {
                    this.FormsService.SignIn(model.UserName, model.RememberMe);
                    if (!string.IsNullOrEmpty(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "The user name or password provided is incorrect.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //// **************************************
        //// URL: /Account/LogOff
        //// **************************************

        public ActionResult LogOff()
        {
            this.FormsService.SignOut();

            return RedirectToAction("Index", "Home");
        }

        //// **************************************
        //// URL: /Account/Register
        //// **************************************

        public ActionResult Register()
        {
            ViewData["PasswordLength"] = this.MembershipService.MinPasswordLength;
            return View();
        }

        [HttpPost]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                // Attempt to register the user
                MembershipCreateStatus createStatus = this.MembershipService.CreateUser(model.UserName, model.Password, model.Email);

                if (createStatus == MembershipCreateStatus.Success)
                {
                    this.FormsService.SignIn(model.UserName, false /* createPersistentCookie */);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, AccountValidation.ErrorCodeToString(createStatus));
                }
            }

            // If we got this far, something failed, redisplay form
            ViewData["PasswordLength"] = this.MembershipService.MinPasswordLength;
            return View(model);
        }

        //// **************************************
        //// URL: /Account/ChangePassword
        //// **************************************

        [Authorize]
        public ActionResult ChangePassword()
        {
            ViewData["PasswordLength"] = this.MembershipService.MinPasswordLength;
            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {
                if (this.MembershipService.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword))
                {
                    return RedirectToAction("ChangePasswordSuccess");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "The current password is incorrect or the new password is invalid.");
                }
            }

            // If we got this far, something failed, redisplay form
            ViewData["PasswordLength"] = this.MembershipService.MinPasswordLength;
            return View(model);
        }

        //// **************************************
        //// URL: /Account/ChangePasswordSuccess
        //// **************************************

        public ActionResult ChangePasswordSuccess()
        {
            return View();
        }

        protected override void Initialize(RequestContext requestContext)
        {
            if (this.FormsService == null)
            {
                this.FormsService = new FormsAuthenticationService();
            }

            if (this.MembershipService == null)
            {
                this.MembershipService = new AccountMembershipService();
            }

            base.Initialize(requestContext);
        }
    }
}
