using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrphanTrust.Models.DomainModels
{
    [Table("Donations")]
    public class Donation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DonationId { get; set; }

        [Required]
        public int DonorUserId { get; set; }

        [Required]
        public int OrphanageId { get; set; }

        public int? NeedId { get; set; }

        [Required]
        [Column(TypeName = "decimal")]
        public decimal Amount { get; set; }

        [Required, StringLength(10)]
        public string Currency { get; set; } = "TZS";

        [StringLength(100)] public string PaymentMethod { get; set; }
        [StringLength(100)] public string PaymentReference { get; set; }
        [StringLength(66)] public string TxHash { get; set; }

        [StringLength(20)]
        public string BlockchainStatus { get; set; } = BlockchainTxStatus.Pending;

        [StringLength(20)]
        public string Status { get; set; } = DonationStatus.Pending;

        public bool IsAnonymous { get; set; } = false;

        [StringLength(500)] public string DonorMessage { get; set; }
        [StringLength(255)] public string ReceiptUrl { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ConfirmedAt { get; set; }

        [ForeignKey("DonorUserId")]
        public virtual ApplicationUser DonorUser { get; set; }

        [ForeignKey("OrphanageId")]
        public virtual Orphanage Orphanage { get; set; }

        [ForeignKey("NeedId")]
        public virtual OrphanageNeed Need { get; set; }

        public virtual ICollection<Distribution> Distributions { get; set; }
            = new HashSet<Distribution>();
    }

    public static class DonationStatus
    {
        public const string Pending = "Pending";
        public const string Confirmed = "Confirmed";
        public const string Failed = "Failed";
        public const string Refunded = "Refunded";
    }

    public static class BlockchainTxStatus
    {
        public const string Pending = "Pending";
        public const string Confirmed = "Confirmed";
        public const string Failed = "Failed";
    }
}