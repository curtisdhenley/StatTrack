using System.ComponentModel.DataAnnotations;

namespace StatTracker.Models
{
    public class Invite
    {
        // Primary Key
        public int Id { get; set; }

        [DataType(DataType.Date)]
        public DateTime InviteDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? JoinDate { get; set; }

        public Guid CompanyToken { get; set; }

        // Foriegn Key
        public int CompanyId { get; set; }

        // Foriegn Key
        public int ProjectId { get; set; }

        // Foriegn Key
        [Required]
        public string? InvitorId { get; set; }

        // Foriegn Key
        public string? InviteeId { get; set; }

        [Required]
        [Display(Name = "Recipient Email")]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} and max {1} characters long.", MinimumLength = 2)]
        public string? InviteeEmail { get; set; }

        [Required]
        [Display(Name = "Recipient First Name")]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} and max {1} characters long.", MinimumLength = 2)]
        public string? InviteeFirstName { get; set; }

        [Required]
        [Display(Name = "Recipient Last Name")]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} and max {1} characters long.", MinimumLength = 2)]
        public string? InviteeLastName { get; set; }

        [Display(Name = "Message")]
        [StringLength(200, ErrorMessage = "The {0} must be at least {2} and max {1} characters long.", MinimumLength = 2)]
        public string? Message { get; set; }

        public bool IsValid { get; set; }

        // Navigation Properties
        public virtual ICollection<Company> Companies { get; set; } = new HashSet<Company>();

        public virtual ICollection<Project> Projects { get; set; } = new HashSet<Project>();

        public virtual ICollection<BTUser> Invitors { get; set; } = new HashSet<BTUser>();

        public virtual ICollection<BTUser> Invitee { get; set; } = new HashSet<BTUser>();
    }
}
