using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrphanTrust.Models.DomainModels
{
    [Table("Children")]
    public class Child
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ChildId { get; set; }

        [Required]
        public int OrphanageId { get; set; }

        [Required, StringLength(100)]
        public string FullName { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }

        [Required, StringLength(10)]
        public string Gender { get; set; }

        [StringLength(50)]
        public string HealthStatus { get; set; } = "Good";

        [StringLength(50)]
        public string EducationLevel { get; set; }

        [StringLength(500)]
        public string BackgroundStory { get; set; }

        [StringLength(255)]
        public string PhotoUrl { get; set; }

        public bool IsAnonymous { get; set; } = false;
        public DateTime AdmissionDate { get; set; } = DateTime.UtcNow;
        public DateTime? DischargeDate { get; set; }

        [StringLength(20)]
        public string Status { get; set; } = "Active";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        [ForeignKey("OrphanageId")]
        public virtual Orphanage Orphanage { get; set; }

        [NotMapped]
        public int Age
        {
            get
            {
                var today = DateTime.Today;
                var age = today.Year - DateOfBirth.Year;
                if (DateOfBirth.Date > today.AddYears(-age)) age--;
                return age;
            }
        }
    }
}