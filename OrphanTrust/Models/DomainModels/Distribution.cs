using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrphanTrust.Models.DomainModels
{
    [Table("Distributions")]
    public class Distribution
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DistributionId { get; set; }

        [Required]
        public int DonationId { get; set; }

        [Required]
        public int OrphanageId { get; set; }

        [Required]
        [Column(TypeName = "decimal")]
        public decimal Amount { get; set; }

        [Required, StringLength(100)]
        public string Purpose { get; set; }

        [StringLength(500)] public string Description { get; set; }
        [StringLength(255)] public string EvidenceUrl { get; set; }
        [StringLength(66)] public string TxHash { get; set; }

        [StringLength(20)]
        public string Status { get; set; } = DistributionStatus.Pending;

        public int? ApprovedByUserId { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? DistributedAt { get; set; }

        [ForeignKey("DonationId")]
        public virtual Donation Donation { get; set; }

        [ForeignKey("OrphanageId")]
        public virtual Orphanage Orphanage { get; set; }

        [ForeignKey("ApprovedByUserId")]
        public virtual ApplicationUser ApprovedByUser { get; set; }
    }

    public static class DistributionStatus
    {
        public const string Pending = "Pending";
        public const string Approved = "Approved";
        public const string Completed = "Completed";
        public const string Rejected = "Rejected";
    }
}