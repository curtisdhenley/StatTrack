using System.ComponentModel.DataAnnotations;

namespace StatTracker.Models
{
    public class Ticket
    {
        // Primary Key
        public int Id { get; set; }

        [Required]
        [Display(Name = "Title")]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} and max {1} characters long.", MinimumLength = 2)]
        public string? Title { get; set; }

        [Required]
        [StringLength(200, ErrorMessage = "The {0} must be at least {2} and max {1} characters long.", MinimumLength = 2)]
        public string? Description { get; set; }

        [DataType(DataType.Date)]
        public DateTime Created { get; set; }

        [DataType(DataType.Date)]
        public DateTime? Updated { get; set; }

        public bool Archived { get; set; }

        public bool ArchivedByProject { get; set; }

        // Foreign Key
        public int ProjectId { get; set; }

        // Foreign Key
        public int TicketTypeId { get; set; }

        // Foreign Key
        public int TicketStatusId { get; set; }

        // Foreign Key
        public int TicketPriorityId { get; set; }

        // Foreign Key
        public string? DeveloperUserId { get; set; }

        // Foreign Key
        [Required]
        public string? SubmitterUserId { get; set; }

        // Navigation Properties
        public virtual ICollection<Project> Projects { get; set; } = new HashSet<Project>();

        public virtual ICollection<TicketPriority> TicketPriorities { get; set; } = new HashSet<TicketPriority>();

        public virtual ICollection<TicketType> TicketTypes { get; set; } = new HashSet<TicketType>();

        public virtual ICollection<TicketStatus> TicketStatuses { get; set; } = new HashSet<TicketStatus>();

        public virtual ICollection<BTUser> DeveloperUser { get; set; } = new HashSet<BTUser>();

        public virtual ICollection<BTUser> SubmitterUser { get; set; } = new HashSet<BTUser>();

        public virtual ICollection<TicketComment> Comments { get; set; } = new HashSet<TicketComment>();

        public virtual ICollection<TicketAttachment> Attachments { get; set; } = new HashSet<TicketAttachment>();

        public virtual ICollection<TicketHistory> History { get; set; } = new HashSet<TicketHistory>();
    }
}
