using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StatTracker.Models
{
    public class Project
    {
        // Primary Key
        public int Id { get; set; }

        // Foreign Key
        public int CompanyId { get; set; }

        [Required]
        [Display(Name = "Project Name")]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} and max {1} characters long.", MinimumLength = 2)]
        public string? Name { get; set; }

        [Required]
        [StringLength(200, ErrorMessage = "The {0} must be at least {2} and max {1} characters long.", MinimumLength = 2)]
        public string? Description { get; set; }

        [DataType(DataType.Date)]
        public DateTime Created { get; set; }

        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        public int ProjectPriorityId { get; set; }

        // Image Properties
        public byte[]? ImageData { get; set; }

        public string? ImageType { get; set; }

        [NotMapped]
        public virtual IFormFile? ImageFile { get; set; }

        public bool Archived { get; set; }

        // Navigation Properties
        public virtual ICollection<Company> Companies { get; set; } = new HashSet<Company>();

        public virtual ICollection<ProjectPriority> ProjectPriorities { get; set; } = new HashSet<ProjectPriority>();

        public virtual ICollection<BTUser> Members { get; set; } = new HashSet<BTUser>();

        public virtual ICollection<Ticket> Tickets { get; set; } = new HashSet<Ticket>();
    }
}
