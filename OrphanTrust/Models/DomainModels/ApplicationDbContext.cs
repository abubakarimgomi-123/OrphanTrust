using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;
using OrphanTrust.Models.DomainModels;

namespace OrphanTrust.Models
{
    public class ApplicationDbContext
        : IdentityDbContext<ApplicationUser, ApplicationRole, int,
            ApplicationUserLogin, ApplicationUserRole, ApplicationUserClaim>
    {
        public ApplicationDbContext()
            : base("OrphanTrustContext")
        {
            Configuration.LazyLoadingEnabled = true;
            Configuration.ProxyCreationEnabled = true;
        }

        public static ApplicationDbContext Create()
            => new ApplicationDbContext();

        public DbSet<Orphanage> Orphanages { get; set; }
        public DbSet<Child> Children { get; set; }
        public DbSet<Donation> Donations { get; set; }
        public DbSet<Distribution> Distributions { get; set; }
        public DbSet<OrphanageNeed> OrphanageNeeds { get; set; }
        public DbSet<OrphanageDocument> OrphanageDocuments { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Rename Identity tables
            modelBuilder.Entity<ApplicationUser>()
                .ToTable("Users");
            modelBuilder.Entity<ApplicationRole>()
                .ToTable("Roles");
            modelBuilder.Entity<ApplicationUserRole>()
                .ToTable("UserRoles");
            modelBuilder.Entity<ApplicationUserClaim>()
                .ToTable("UserClaims");
            modelBuilder.Entity<ApplicationUserLogin>()
                .ToTable("UserLogins");

            // Decimal precision
            modelBuilder.Entity<Donation>()
                .Property(d => d.Amount)
                .HasPrecision(18, 2);
            modelBuilder.Entity<Distribution>()
                .Property(d => d.Amount)
                .HasPrecision(18, 2);
            modelBuilder.Entity<OrphanageNeed>()
                .Property(n => n.EstimatedCost)
                .HasPrecision(18, 2);
            modelBuilder.Entity<OrphanageNeed>()
                .Property(n => n.AmountRaised)
                .HasPrecision(18, 2);

            // Orphanage → AdminUser (no cascade)
            modelBuilder.Entity<Orphanage>()
                .HasOptional(o => o.AdminUser)
                .WithMany()
                .HasForeignKey(o => o.AdminUserId)
                .WillCascadeOnDelete(false);

            // Orphanage → VerifiedByUser (no cascade)
            modelBuilder.Entity<Orphanage>()
                .HasOptional(o => o.VerifiedByUser)
                .WithMany()
                .HasForeignKey(o => o.VerifiedByUserId)
                .WillCascadeOnDelete(false);

            // Donation → DonorUser (no cascade)
            modelBuilder.Entity<Donation>()
                .HasRequired(d => d.DonorUser)
                .WithMany()
                .HasForeignKey(d => d.DonorUserId)
                .WillCascadeOnDelete(false);

            // Donation → Orphanage (no cascade)
            modelBuilder.Entity<Donation>()
                .HasRequired(d => d.Orphanage)
                .WithMany(o => o.Donations)
                .HasForeignKey(d => d.OrphanageId)
                .WillCascadeOnDelete(false);

            // Donation → Need (no cascade)
            modelBuilder.Entity<Donation>()
                .HasOptional(d => d.Need)
                .WithMany()
                .HasForeignKey(d => d.NeedId)
                .WillCascadeOnDelete(false);

            // Distribution → Donation (no cascade)
            modelBuilder.Entity<Distribution>()
                .HasRequired(d => d.Donation)
                .WithMany(d => d.Distributions)
                .HasForeignKey(d => d.DonationId)
                .WillCascadeOnDelete(false);

            // Distribution → Orphanage (no cascade)
            modelBuilder.Entity<Distribution>()
                .HasRequired(d => d.Orphanage)
                .WithMany(o => o.Distributions)
                .HasForeignKey(d => d.OrphanageId)
                .WillCascadeOnDelete(false);

            // Distribution → ApprovedByUser (no cascade)
            modelBuilder.Entity<Distribution>()
                .HasOptional(d => d.ApprovedByUser)
                .WithMany()
                .HasForeignKey(d => d.ApprovedByUserId)
                .WillCascadeOnDelete(false);

            // Child → Orphanage (no cascade)
            modelBuilder.Entity<Child>()
                .HasRequired(c => c.Orphanage)
                .WithMany(o => o.Children)
                .HasForeignKey(c => c.OrphanageId)
                .WillCascadeOnDelete(false);

            // OrphanageNeed → Orphanage (no cascade)
            modelBuilder.Entity<OrphanageNeed>()
                .HasRequired(n => n.Orphanage)
                .WithMany(o => o.Needs)
                .HasForeignKey(n => n.OrphanageId)
                .WillCascadeOnDelete(false);

            // OrphanageDocument → Orphanage (no cascade)
            modelBuilder.Entity<OrphanageDocument>()
                .HasRequired(d => d.Orphanage)
                .WithMany(o => o.Documents)
                .HasForeignKey(d => d.OrphanageId)
                .WillCascadeOnDelete(false);

            // OrphanageDocument → UploadedByUser (no cascade)
            modelBuilder.Entity<OrphanageDocument>()
                .HasOptional(d => d.UploadedByUser)
                .WithMany()
                .HasForeignKey(d => d.UploadedByUserId)
                .WillCascadeOnDelete(false);

            // AuditLog → User (no cascade)
            modelBuilder.Entity<AuditLog>()
                .HasOptional(a => a.User)
                .WithMany()
                .HasForeignKey(a => a.UserId)
                .WillCascadeOnDelete(false);
        }
    }
}