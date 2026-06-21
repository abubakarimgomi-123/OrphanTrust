using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrphanTrust.Models.DomainModels
{
    [Table("Orphanages")]
    public class Orphanage
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrphanageId { get; set; }

        [Required, StringLength(150)]
        public string Name { get; set; }

        [Required, StringLength(100)]
        public string Region { get; set; }

        [Required, StringLength(255)]
        public string Address { get; set; }

        [Required, StringLength(50)]
        public string RegistrationNumber { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [StringLength(255)]
        public string LogoUrl { get; set; }

        [StringLength(20)]
        public string ContactPhone { get; set; }

        [StringLength(100), EmailAddress]
        public string ContactEmail { get; set; }

        [Required, StringLength(20)]
        public string Status { get; set; } = OrphanageStatus.Pending;

        public int? AdminUserId { get; set; }
        public string BlockchainAddress { get; set; }
        public bool IsVerifiedOnChain { get; set; } = false;
        public DateTime? VerifiedAt { get; set; }
        public int? VerifiedByUserId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        [ForeignKey("AdminUserId")]
        public virtual ApplicationUser AdminUser { get; set; }

        [ForeignKey("VerifiedByUserId")]
        public virtual ApplicationUser VerifiedByUser { get; set; }

        public virtual ICollection<Child> Children { get; set; } = new HashSet<Child>();
        public virtual ICollection<OrphanageNeed> Needs { get; set; } = new HashSet<OrphanageNeed>();
        public virtual ICollection<Donation> Donations { get; set; } = new HashSet<Donation>();
        public virtual ICollection<Distribution> Distributions { get; set; } = new HashSet<Distribution>();
        public virtual ICollection<OrphanageDocument> Documents { get; set; } = new HashSet<OrphanageDocument>();
    }

    public static class OrphanageStatus
    {
        public const string Pending = "Pending";
        public const string Active = "Active";
        public const string Suspended = "Suspended";
        public const string Rejected = "Rejected";
    }
}