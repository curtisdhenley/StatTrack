using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace StatTracker.Models
{
    public class BTUser : IdentityUser
    {
        [Required]
        [Display(Name = "First Name")]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} and max {1} characters long.", MinimumLength = 2)]
        public string? FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} and max {1} characters long.", MinimumLength = 2)]
        public string? LastName { get; set; }

        [NotMapped]
        [Display(Name = "Full Name")]
        public string FullName { get { return $"{FirstName} {LastName}"; } }

        public byte[]? ImageFileData { get; set; }

        public string? ImageFileType { get; set; }

        [NotMapped]
        public IFormFile? ImageFormFile { get; set; }

        // Foriegn Key
        public int CompanyId { get; set; }

        // Navigation Properties
        public virtual ICollection<Company> Companies { get; set; } = new HashSet<Company>();

        public virtual ICollection<Project> Projects { get; set; } = new HashSet<Project>();
    }
}
