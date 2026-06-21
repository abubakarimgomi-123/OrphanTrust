using System;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using OrphanTrust.Infrastructure.Constants;
using OrphanTrust.Models;
using OrphanTrust.Models.DomainModels;

namespace OrphanTrust
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            SeedDatabase();
        }

        private static void SeedDatabase()
        {
            try
            {
                using (var context = new ApplicationDbContext())
                {
                    context.Database.CreateIfNotExists();

                    var roleStore = new RoleStore<ApplicationRole, int,
                        ApplicationUserRole>(context);
                    var roleManager =
                        new RoleManager<ApplicationRole, int>(roleStore);

                    string[] roles =
                    {
                        RoleConstants.SuperAdmin,
                        RoleConstants.OrphanageAdmin,
                        RoleConstants.Donor,
                        RoleConstants.Visitor
                    };

                    foreach (var role in roles)
                    {
                        if (!roleManager.RoleExists(role))
                        {
                            roleManager.Create(new ApplicationRole
                            {
                                Name = role,
                                CreatedAt = DateTime.UtcNow
                            });
                        }
                    }

                    var userStore = new UserStore<ApplicationUser,
                        ApplicationRole, int, ApplicationUserLogin,
                        ApplicationUserRole, ApplicationUserClaim>(context);

                    var userManager =
                        new UserManager<ApplicationUser, int>(userStore);

                    userManager.PasswordValidator = new PasswordValidator
                    {
                        RequiredLength = 6
                    };

                    const string adminEmail = "superadmin@orphantrust.tz";
                    const string adminPassword = "Admin@OrphanTrust2024!";

                    var existingUser = userManager.FindByEmail(adminEmail);

                    if (existingUser == null)
                    {
                        var admin = new ApplicationUser
                        {
                            UserName = adminEmail,
                            Email = adminEmail,
                            FullName = "System Super Administrator",
                            IsActive = true,
                            IsEmailVerified = true,
                            EmailConfirmed = true,
                            CreatedAt = DateTime.UtcNow
                        };

                        var result = userManager.Create(admin, adminPassword);
                        if (result.Succeeded)
                        {
                            userManager.AddToRole(
                                admin.Id, RoleConstants.SuperAdmin);
                        }
                    }
                    else
                    {
                        var userRoles = userManager.GetRoles(existingUser.Id);
                        if (!userRoles.Contains(RoleConstants.SuperAdmin))
                        {
                            userManager.AddToRole(
                                existingUser.Id, RoleConstants.SuperAdmin);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Seed: " + ex.Message);
            }
        }
    }
}