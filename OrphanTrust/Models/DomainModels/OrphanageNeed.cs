using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrphanTrust.Models.DomainModels
{
    [Table("OrphanageNeeds")]
    public class OrphanageNeed
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int NeedId { get; set; }

        [Required]
        public int OrphanageId { get; set; }

        [Required, StringLength(50)]
        public string Category { get; set; }

        [Required, StringLength(200)]
        public string Title { get; set; }

        [StringLength(1000)]
        public string Description { get; set; }

        [Column(TypeName = "decimal")]
        public decimal EstimatedCost { get; set; }

        [Column(TypeName = "decimal")]
        public decimal AmountRaised { get; set; } = 0;

        [StringLength(20)]
        public string Status { get; set; } = NeedStatus.Open;

        public bool IsUrgent { get; set; } = false;
        public DateTime? Deadline { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        [ForeignKey("OrphanageId")]
        public virtual Orphanage Orphanage { get; set; }

        [NotMapped]
        public decimal PercentageFunded =>
            EstimatedCost > 0
                ? Math.Min(100, (AmountRaised / EstimatedCost) * 100)
                : 0;
    }

    public static class NeedStatus
    {
        public const string Open = "Open";
        public const string Funded = "Funded";
        public const string Closed = "Closed";
    }

    public static class NeedCategory
    {
        public const string Food = "Food";
        public const string Education = "Education";
        public const string Healthcare = "Healthcare";
        public const string Clothing = "Clothing";
        public const string Shelter = "Shelter";
        public const string Other = "Other";
    }
}