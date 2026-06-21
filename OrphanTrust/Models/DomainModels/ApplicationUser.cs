using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace OrphanTrust.Models.DomainModels
{
    public class ApplicationUser : IdentityUser<int, ApplicationUserLogin,
        ApplicationUserRole, ApplicationUserClaim>
    {
        [Required, StringLength(100)]
        public string FullName { get; set; }

        [StringLength(100)]
        public string WalletAddress { get; set; }

        [StringLength(50)]
        public string Region { get; set; }

        [StringLength(255)]
        public string ProfileImageUrl { get; set; }

        public bool IsActive { get; set; } = true;
        public bool IsEmailVerified { get; set; } = false;

        [StringLength(255)]
        public string EmailVerificationToken { get; set; }

        public DateTime? EmailVerificationTokenExpiry { get; set; }

        [StringLength(255)]
        public string PasswordResetToken { get; set; }

        public DateTime? PasswordResetTokenExpiry { get; set; }

        public int FailedLoginAttempts { get; set; } = 0;
        public DateTime? LockoutEndDate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }

        [StringLength(45)]
        public string LastLoginIp { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(
            UserManager<ApplicationUser, int> manager)
        {
            var userIdentity = await manager.CreateIdentityAsync(
                this, DefaultAuthenticationTypes.ApplicationCookie);
            userIdentity.AddClaim(
                new Claim("FullName", FullName ?? string.Empty));
            userIdentity.AddClaim(
                new Claim("ProfileImageUrl",
                    ProfileImageUrl ?? string.Empty));
            return userIdentity;
        }
    }

    public class ApplicationUserRole : IdentityUserRole<int> { }
    public class ApplicationUserClaim : IdentityUserClaim<int> { }
    public class ApplicationUserLogin : IdentityUserLogin<int> { }

    public class ApplicationRole : IdentityRole<int, ApplicationUserRole>
    {
        [StringLength(255)]
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}