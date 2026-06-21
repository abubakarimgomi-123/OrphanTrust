using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrphanTrust.Models.DomainModels
{
    [Table("OrphanageDocuments")]
    public class OrphanageDocument
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DocumentId { get; set; }

        [Required]
        public int OrphanageId { get; set; }

        [Required, StringLength(100)]
        public string DocumentType { get; set; }

        [Required, StringLength(255)]
        public string FileName { get; set; }

        [Required, StringLength(500)]
        public string FilePath { get; set; }

        [StringLength(20)]
        public string Status { get; set; } = "Pending";

        public int? UploadedByUserId { get; set; }
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("OrphanageId")]
        public virtual Orphanage Orphanage { get; set; }

        [ForeignKey("UploadedByUserId")]
        public virtual ApplicationUser UploadedByUser { get; set; }
    }
}