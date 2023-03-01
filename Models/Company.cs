using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StatTracker.Models
{
    public class Company
    {
        // Primary Key
        public int Id { get; set; }

        [Required]
        [Display(Name = "Company Name")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and max {1} characters long.", MinimumLength = 2)]
        public string? Name { get; set; }

        [StringLength(200, ErrorMessage = "The {0} must be at least {2} and max {1} characters long.", MinimumLength = 2)]
        public string? Description { get; set; }

        // Image Properties
        public byte[]? ImageFileData { get; set; }

        public string? ImageFileType { get; set; }

        [NotMapped]
        public virtual IFormFile? ImageFormFile { get; set; }

        // Navigation Properties
        public virtual ICollection<Project> Projects { get; set; } = new HashSet<Project>();

        public virtual ICollection<BTUser> Members { get; set; } = new HashSet<BTUser>();

        public virtual ICollection<Invite> Invites { get; set; } = new HashSet<Invite>();
    }
}
