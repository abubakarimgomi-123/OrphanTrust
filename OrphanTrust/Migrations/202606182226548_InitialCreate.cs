namespace OrphanTrust.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AuditLogs",
                c => new
                    {
                        LogId = c.Int(nullable: false, identity: true),
                        UserId = c.Int(),
                        UserEmail = c.String(maxLength: 100),
                        Action = c.String(nullable: false, maxLength: 100),
                        Entity = c.String(nullable: false, maxLength: 100),
                        EntityId = c.Int(),
                        OldValues = c.String(maxLength: 2000),
                        NewValues = c.String(maxLength: 2000),
                        IpAddress = c.String(maxLength: 45),
                        UserAgent = c.String(maxLength: 500),
                        Timestamp = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.LogId)
                .ForeignKey("dbo.Users", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FullName = c.String(nullable: false, maxLength: 100),
                        WalletAddress = c.String(maxLength: 100),
                        Region = c.String(maxLength: 50),
                        ProfileImageUrl = c.String(maxLength: 255),
                        IsActive = c.Boolean(nullable: false),
                        IsEmailVerified = c.Boolean(nullable: false),
                        EmailVerificationToken = c.String(maxLength: 255),
                        EmailVerificationTokenExpiry = c.DateTime(),
                        PasswordResetToken = c.String(maxLength: 255),
                        PasswordResetTokenExpiry = c.DateTime(),
                        FailedLoginAttempts = c.Int(nullable: false),
                        LockoutEndDate = c.DateTime(),
                        CreatedAt = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(),
                        LastLoginAt = c.DateTime(),
                        LastLoginIp = c.String(maxLength: 45),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.UserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: false)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.UserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: false)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.UserRoles",
                c => new
                    {
                        UserId = c.Int(nullable: false),
                        RoleId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: false)
                .ForeignKey("dbo.Roles", t => t.RoleId, cascadeDelete: false)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.Children",
                c => new
                    {
                        ChildId = c.Int(nullable: false, identity: true),
                        OrphanageId = c.Int(nullable: false),
                        FullName = c.String(nullable: false, maxLength: 100),
                        DateOfBirth = c.DateTime(nullable: false),
                        Gender = c.String(nullable: false, maxLength: 10),
                        HealthStatus = c.String(maxLength: 50),
                        EducationLevel = c.String(maxLength: 50),
                        BackgroundStory = c.String(maxLength: 500),
                        PhotoUrl = c.String(maxLength: 255),
                        IsAnonymous = c.Boolean(nullable: false),
                        AdmissionDate = c.DateTime(nullable: false),
                        DischargeDate = c.DateTime(),
                        Status = c.String(maxLength: 20),
                        CreatedAt = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(),
                    })
                .PrimaryKey(t => t.ChildId)
                .ForeignKey("dbo.Orphanages", t => t.OrphanageId)
                .Index(t => t.OrphanageId);
            
            CreateTable(
                "dbo.Orphanages",
                c => new
                    {
                        OrphanageId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 150),
                        Region = c.String(nullable: false, maxLength: 100),
                        Address = c.String(nullable: false, maxLength: 255),
                        RegistrationNumber = c.String(nullable: false, maxLength: 50),
                        Description = c.String(maxLength: 500),
                        LogoUrl = c.String(maxLength: 255),
                        ContactPhone = c.String(maxLength: 20),
                        ContactEmail = c.String(maxLength: 100),
                        Status = c.String(nullable: false, maxLength: 20),
                        AdminUserId = c.Int(),
                        BlockchainAddress = c.String(),
                        IsVerifiedOnChain = c.Boolean(nullable: false),
                        VerifiedAt = c.DateTime(),
                        VerifiedByUserId = c.Int(),
                        CreatedAt = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(),
                    })
                .PrimaryKey(t => t.OrphanageId)
                .ForeignKey("dbo.Users", t => t.AdminUserId)
                .ForeignKey("dbo.Users", t => t.VerifiedByUserId)
                .Index(t => t.AdminUserId)
                .Index(t => t.VerifiedByUserId);
            
            CreateTable(
                "dbo.Distributions",
                c => new
                    {
                        DistributionId = c.Int(nullable: false, identity: true),
                        DonationId = c.Int(nullable: false),
                        OrphanageId = c.Int(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Purpose = c.String(nullable: false, maxLength: 100),
                        Description = c.String(maxLength: 500),
                        EvidenceUrl = c.String(maxLength: 255),
                        TxHash = c.String(maxLength: 66),
                        Status = c.String(maxLength: 20),
                        ApprovedByUserId = c.Int(),
                        ApprovedAt = c.DateTime(),
                        CreatedAt = c.DateTime(nullable: false),
                        DistributedAt = c.DateTime(),
                    })
                .PrimaryKey(t => t.DistributionId)
                .ForeignKey("dbo.Users", t => t.ApprovedByUserId)
                .ForeignKey("dbo.Donations", t => t.DonationId)
                .ForeignKey("dbo.Orphanages", t => t.OrphanageId)
                .Index(t => t.DonationId)
                .Index(t => t.OrphanageId)
                .Index(t => t.ApprovedByUserId);
            
            CreateTable(
                "dbo.Donations",
                c => new
                    {
                        DonationId = c.Int(nullable: false, identity: true),
                        DonorUserId = c.Int(nullable: false),
                        OrphanageId = c.Int(nullable: false),
                        NeedId = c.Int(),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Currency = c.String(nullable: false, maxLength: 10),
                        PaymentMethod = c.String(maxLength: 100),
                        PaymentReference = c.String(maxLength: 100),
                        TxHash = c.String(maxLength: 66),
                        BlockchainStatus = c.String(maxLength: 20),
                        Status = c.String(maxLength: 20),
                        IsAnonymous = c.Boolean(nullable: false),
                        DonorMessage = c.String(maxLength: 500),
                        ReceiptUrl = c.String(maxLength: 255),
                        CreatedAt = c.DateTime(nullable: false),
                        ConfirmedAt = c.DateTime(),
                    })
                .PrimaryKey(t => t.DonationId)
                .ForeignKey("dbo.Users", t => t.DonorUserId)
                .ForeignKey("dbo.OrphanageNeeds", t => t.NeedId)
                .ForeignKey("dbo.Orphanages", t => t.OrphanageId)
                .Index(t => t.DonorUserId)
                .Index(t => t.OrphanageId)
                .Index(t => t.NeedId);
            
            CreateTable(
                "dbo.OrphanageNeeds",
                c => new
                    {
                        NeedId = c.Int(nullable: false, identity: true),
                        OrphanageId = c.Int(nullable: false),
                        Category = c.String(nullable: false, maxLength: 50),
                        Title = c.String(nullable: false, maxLength: 200),
                        Description = c.String(maxLength: 1000),
                        EstimatedCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AmountRaised = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Status = c.String(maxLength: 20),
                        IsUrgent = c.Boolean(nullable: false),
                        Deadline = c.DateTime(),
                        CreatedAt = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(),
                    })
                .PrimaryKey(t => t.NeedId)
                .ForeignKey("dbo.Orphanages", t => t.OrphanageId)
                .Index(t => t.OrphanageId);
            
            CreateTable(
                "dbo.OrphanageDocuments",
                c => new
                    {
                        DocumentId = c.Int(nullable: false, identity: true),
                        OrphanageId = c.Int(nullable: false),
                        DocumentType = c.String(nullable: false, maxLength: 100),
                        FileName = c.String(nullable: false, maxLength: 255),
                        FilePath = c.String(nullable: false, maxLength: 500),
                        Status = c.String(maxLength: 20),
                        UploadedByUserId = c.Int(),
                        UploadedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.DocumentId)
                .ForeignKey("dbo.Orphanages", t => t.OrphanageId)
                .ForeignKey("dbo.Users", t => t.UploadedByUserId)
                .Index(t => t.OrphanageId)
                .Index(t => t.UploadedByUserId);
            
            CreateTable(
                "dbo.Roles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Description = c.String(maxLength: 255),
                        CreatedAt = c.DateTime(nullable: false),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserRoles", "RoleId", "dbo.Roles");
            DropForeignKey("dbo.Children", "OrphanageId", "dbo.Orphanages");
            DropForeignKey("dbo.Orphanages", "VerifiedByUserId", "dbo.Users");
            DropForeignKey("dbo.OrphanageDocuments", "UploadedByUserId", "dbo.Users");
            DropForeignKey("dbo.OrphanageDocuments", "OrphanageId", "dbo.Orphanages");
            DropForeignKey("dbo.Distributions", "OrphanageId", "dbo.Orphanages");
            DropForeignKey("dbo.Distributions", "DonationId", "dbo.Donations");
            DropForeignKey("dbo.Donations", "OrphanageId", "dbo.Orphanages");
            DropForeignKey("dbo.Donations", "NeedId", "dbo.OrphanageNeeds");
            DropForeignKey("dbo.OrphanageNeeds", "OrphanageId", "dbo.Orphanages");
            DropForeignKey("dbo.Donations", "DonorUserId", "dbo.Users");
            DropForeignKey("dbo.Distributions", "ApprovedByUserId", "dbo.Users");
            DropForeignKey("dbo.Orphanages", "AdminUserId", "dbo.Users");
            DropForeignKey("dbo.AuditLogs", "UserId", "dbo.Users");
            DropForeignKey("dbo.UserRoles", "UserId", "dbo.Users");
            DropForeignKey("dbo.UserLogins", "UserId", "dbo.Users");
            DropForeignKey("dbo.UserClaims", "UserId", "dbo.Users");
            DropIndex("dbo.Roles", "RoleNameIndex");
            DropIndex("dbo.OrphanageDocuments", new[] { "UploadedByUserId" });
            DropIndex("dbo.OrphanageDocuments", new[] { "OrphanageId" });
            DropIndex("dbo.OrphanageNeeds", new[] { "OrphanageId" });
            DropIndex("dbo.Donations", new[] { "NeedId" });
            DropIndex("dbo.Donations", new[] { "OrphanageId" });
            DropIndex("dbo.Donations", new[] { "DonorUserId" });
            DropIndex("dbo.Distributions", new[] { "ApprovedByUserId" });
            DropIndex("dbo.Distributions", new[] { "OrphanageId" });
            DropIndex("dbo.Distributions", new[] { "DonationId" });
            DropIndex("dbo.Orphanages", new[] { "VerifiedByUserId" });
            DropIndex("dbo.Orphanages", new[] { "AdminUserId" });
            DropIndex("dbo.Children", new[] { "OrphanageId" });
            DropIndex("dbo.UserRoles", new[] { "RoleId" });
            DropIndex("dbo.UserRoles", new[] { "UserId" });
            DropIndex("dbo.UserLogins", new[] { "UserId" });
            DropIndex("dbo.UserClaims", new[] { "UserId" });
            DropIndex("dbo.Users", "UserNameIndex");
            DropIndex("dbo.AuditLogs", new[] { "UserId" });
            DropTable("dbo.Roles");
            DropTable("dbo.OrphanageDocuments");
            DropTable("dbo.OrphanageNeeds");
            DropTable("dbo.Donations");
            DropTable("dbo.Distributions");
            DropTable("dbo.Orphanages");
            DropTable("dbo.Children");
            DropTable("dbo.UserRoles");
            DropTable("dbo.UserLogins");
            DropTable("dbo.UserClaims");
            DropTable("dbo.Users");
            DropTable("dbo.AuditLogs");
        }
    }
}
