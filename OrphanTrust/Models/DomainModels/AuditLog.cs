using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrphanTrust.Models.DomainModels
{
    [Table("AuditLogs")]
    public class AuditLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LogId { get; set; }

        public int? UserId { get; set; }

        [StringLength(100)] public string UserEmail { get; set; }

        [Required, StringLength(100)]
        public string Action { get; set; }

        [Required, StringLength(100)]
        public string Entity { get; set; }

        public int? EntityId { get; set; }

        [StringLength(2000)] public string OldValues { get; set; }
        [StringLength(2000)] public string NewValues { get; set; }
        [StringLength(45)] public string IpAddress { get; set; }
        [StringLength(500)] public string UserAgent { get; set; }

        [Required]
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }
    }
}