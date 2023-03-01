using System.ComponentModel.DataAnnotations;

namespace StatTracker.Models
{
    public class Ticket
    {
        // Primary Key
        public int Id { get; set; }

        [Required]
        [Display(Name = "Title")]
        [StringLength(200, ErrorMessage = "The {0} must be at least {2} and max {1} characters long.", MinimumLength = 2)]
        public string? Title { get; set; }

        [Required]
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
        public virtual Project? Project { get; set; }

        public virtual TicketPriority? TicketPriority { get; set; }

        public virtual TicketType? TicketType { get; set; }

        public virtual TicketStatus? TicketStatus { get; set; }

        public virtual BTUser? DeveloperUser { get; set; }

        public virtual BTUser? SubmitterUser { get; set; }

        public virtual ICollection<TicketComment> Comments { get; set; } = new HashSet<TicketComment>();

        public virtual ICollection<TicketAttachment> Attachments { get; set; } = new HashSet<TicketAttachment>();

        public virtual TicketHistory? History { get; set; }
    }
}
