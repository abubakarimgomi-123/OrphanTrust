using System;
using System.Configuration;
using System.Net.Mail;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using OrphanTrust.Models;
using OrphanTrust.Models.DomainModels;

namespace OrphanTrust.App_Start
{
    public class EmailService : IIdentityMessageService
    {
        public async Task SendAsync(IdentityMessage message)
        {
            try
            {
                var host = ConfigurationManager.AppSettings["SmtpHost"];
                var port = int.Parse(
                    ConfigurationManager.AppSettings["SmtpPort"] ?? "587");
                var user = ConfigurationManager.AppSettings["SmtpUser"];
                var pass = ConfigurationManager.AppSettings["SmtpPass"];
                var from = ConfigurationManager.AppSettings["SmtpFrom"];

                using (var client = new SmtpClient(host, port))
                {
                    client.Credentials =
                        new System.Net.NetworkCredential(user, pass);
                    client.EnableSsl = true;
                    var mail = new MailMessage(from, message.Destination)
                    {
                        Subject = message.Subject,
                        Body = message.Body,
                        IsBodyHtml = true
                    };
                    await client.SendMailAsync(mail);
                }
            }
            catch { }
        }
    }

    public class ApplicationUserManager
        : UserManager<ApplicationUser, int>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser, int> store)
            : base(store) { }

        public static ApplicationUserManager Create(
            IdentityFactoryOptions<ApplicationUserManager> options,
            IOwinContext context)
        {
            var manager = new ApplicationUserManager(
                new UserStore<ApplicationUser, ApplicationRole, int,
                    ApplicationUserLogin, ApplicationUserRole,
                    ApplicationUserClaim>(
                        context.Get<ApplicationDbContext>()));

            manager.UserValidator =
                new UserValidator<ApplicationUser, int>(manager)
                {
                    AllowOnlyAlphanumericUserNames = false,
                    RequireUniqueEmail = true
                };

            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 8,
                RequireNonLetterOrDigit = true,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true
            };

            int lockout = int.Parse(
                ConfigurationManager.AppSettings["LockoutMinutes"] ?? "15");
            int attempts = int.Parse(
                ConfigurationManager.AppSettings["MaxLoginAttempts"] ?? "5");

            manager.UserLockoutEnabledByDefault = true;
            manager.DefaultAccountLockoutTimeSpan =
                TimeSpan.FromMinutes(lockout);
            manager.MaxFailedAccessAttemptsBeforeLockout = attempts;
            manager.EmailService = new EmailService();

            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider =
                    new DataProtectorTokenProvider<ApplicationUser, int>(
                        dataProtectionProvider.Create("ASP.NET Identity"))
                    {
                        TokenLifespan = TimeSpan.FromHours(24)
                    };
            }
            return manager;
        }
    }

    public class ApplicationRoleManager
        : RoleManager<ApplicationRole, int>
    {
        public ApplicationRoleManager(
            IRoleStore<ApplicationRole, int> roleStore)
            : base(roleStore) { }

        public static ApplicationRoleManager Create(
            IdentityFactoryOptions<ApplicationRoleManager> options,
            IOwinContext context)
        {
            return new ApplicationRoleManager(
                new RoleStore<ApplicationRole, int, ApplicationUserRole>(
                    context.Get<ApplicationDbContext>()));
        }
    }

    public class ApplicationSignInManager
        : SignInManager<ApplicationUser, int>
    {
        public ApplicationSignInManager(
            ApplicationUserManager userManager,
            IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager) { }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(
            ApplicationUser user)
        {
            return user.GenerateUserIdentityAsync(
                (ApplicationUserManager)UserManager);
        }

        public static ApplicationSignInManager Create(
            IdentityFactoryOptions<ApplicationSignInManager> options,
            IOwinContext context)
        {
            return new ApplicationSignInManager(
                context.GetUserManager<ApplicationUserManager>(),
                context.Authentication);
        }
    }
}