using System;
using System.Data.Entity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using OrphanTrust.App_Start;
using OrphanTrust.Infrastructure.Constants;
using OrphanTrust.Models;
using OrphanTrust.Models.DomainModels;

namespace OrphanTrust.Infrastructure.Seed
{
    public class ApplicationDbInitializer
        : CreateDatabaseIfNotExists<ApplicationDbContext>
    {
        protected override void Seed(ApplicationDbContext context)
        {
            SeedRoles(context);
            SeedSuperAdmin(context);
            base.Seed(context);
        }

        private static void SeedRoles(ApplicationDbContext context)
        {
            var roleStore = new RoleStore<ApplicationRole, int,
                ApplicationUserRole>(context);
            var roleManager = new ApplicationRoleManager(roleStore);

            string[] roles =
            {
                RoleConstants.SuperAdmin,
                RoleConstants.OrphanageAdmin,
                RoleConstants.Donor,
                RoleConstants.Visitor
            };

            string[] descriptions =
            {
                "Full system access — platform super administrator",
                "Manages a single orphanage profile and its data",
                "Registered donor who can make and track donations",
                "Public visitor with read-only access"
            };

            for (int i = 0; i < roles.Length; i++)
            {
                if (!roleManager.RoleExists(roles[i]))
                {
                    roleManager.Create(new ApplicationRole
                    {
                        Name = roles[i],
                        Description = descriptions[i],
                        CreatedAt = DateTime.UtcNow
                    });
                }
            }
        }

        private static void SeedSuperAdmin(ApplicationDbContext context)
        {
            var userStore = new UserStore<ApplicationUser, ApplicationRole,
                int, ApplicationUserLogin, ApplicationUserRole,
                ApplicationUserClaim>(context);

            var userManager = new UserManager<ApplicationUser, int>(userStore);

            const string adminEmail = "superadmin@orphantrust.tz";
            const string adminPassword = "Admin@OrphanTrust2024!";

            if (userManager.FindByEmail(adminEmail) == null)
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
                    userManager.AddToRole(admin.Id, RoleConstants.SuperAdmin);
                }
            }
        }
    }
}