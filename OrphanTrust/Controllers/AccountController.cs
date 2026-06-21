using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using OrphanTrust.Infrastructure.Constants;
using OrphanTrust.Models;
using OrphanTrust.Models.DomainModels;
using OrphanTrust.ViewModels.Account;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace OrphanTrust.Controllers
{
    public class AccountController : Controller
    {
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            if (Request.IsAuthenticated)
                return RedirectToAction("Index", "Dashboard");
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
                return View(model);

            using (var db = new ApplicationDbContext())
            {
                var userStore = new UserStore<ApplicationUser,
                    ApplicationRole, int, ApplicationUserLogin,
                    ApplicationUserRole, ApplicationUserClaim>(db);
                var userManager =
                    new UserManager<ApplicationUser, int>(userStore);

                var user = userManager.FindByEmail(model.Email);

                if (user == null)
                {
                    ModelState.AddModelError("",
                        "Invalid email or password.");
                    return View(model);
                }

                if (!user.IsActive)
                {
                    ModelState.AddModelError("",
                        "Account deactivated.");
                    return View(model);
                }

                var result = userManager.CheckPassword(user, model.Password);

                if (result)
                {
                    var roles = userManager.GetRoles(user.Id);
                    string role = roles.FirstOrDefault() ?? RoleConstants.Donor;

                    FormsAuthentication.SetAuthCookie(model.Email,
                        model.RememberMe);

                    Session["UserId"] = user.Id;
                    Session["UserName"] = user.FullName;
                    Session["UserRole"] = role;
                    Session["Email"] = user.Email;

                    if (role == RoleConstants.SuperAdmin)
                        return RedirectToAction(
                            "SuperAdminDashboard", "Dashboard");
                    if (role == RoleConstants.OrphanageAdmin)
                        return RedirectToAction(
                            "OrphanageAdminDashboard", "Dashboard");
                    if (role == RoleConstants.Donor)
                        return RedirectToAction(
                            "DonorDashboard", "Dashboard");

                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError("", "Invalid email or password.");
                return View(model);
            }
        }

        [AllowAnonymous]
        public ActionResult Register()
        {
            if (Request.IsAuthenticated)
                return RedirectToAction("Index", "Dashboard");
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            using (var db = new ApplicationDbContext())
            {
                var userStore = new UserStore<ApplicationUser,
                    ApplicationRole, int, ApplicationUserLogin,
                    ApplicationUserRole, ApplicationUserClaim>(db);
                var userManager =
                    new UserManager<ApplicationUser, int>(userStore);

                if (userManager.FindByEmail(model.Email) != null)
                {
                    ModelState.AddModelError("",
                        "Email already registered.");
                    return View(model);
                }

                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FullName = model.FullName,
                    Region = model.Region,
                    IsActive = true,
                    IsEmailVerified = false,
                    CreatedAt = DateTime.UtcNow
                };

                var result = userManager.Create(user, model.Password);

                if (result.Succeeded)
                {
                    string role = model.Role ?? RoleConstants.Donor;
                    if (role == RoleConstants.SuperAdmin)
                        role = RoleConstants.Donor;

                    userManager.AddToRole(user.Id, role);

                    FormsAuthentication.SetAuthCookie(model.Email, false);
                    Session["UserId"] = user.Id;
                    Session["UserName"] = user.FullName;
                    Session["UserRole"] = role;
                    Session["Email"] = model.Email;

                    return RedirectToAction("DonorDashboard", "Dashboard");
                }

                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error);
            }

            return View(model);
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("Login", "Account");
        }

        [AllowAnonymous]
        public ActionResult ForgotPassword() => View();

        [HttpPost]
        [AllowAnonymous]
        public ActionResult ForgotPassword(ForgotPasswordViewModel model)
        {
            return RedirectToAction("ForgotPasswordConfirmation");
        }

        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation() => View();

        [AllowAnonymous]
        public ActionResult AccessDenied() => View();
    }
}