using System.ComponentModel.DataAnnotations;

namespace StatTracker.Models
{
    public class TicketHistory
    {
        // Primary Key
        public int Id { get; set; }

        // Foreign Key
        public int TicketId { get; set; }

        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and max {1} characters long.", MinimumLength = 2)]
        public string? PropertyName { get; set; }

        [StringLength(500, ErrorMessage = "The {0} must be at least {2} and max {1} characters long.", MinimumLength = 2)]
        public string? Description { get; set; }

        [DataType(DataType.Date)]
        public DateTime Created { get; set; }

        public string? OldValue { get; set; }

        public string? NewValue { get; set; }

        [Required]
        public string? UserId { get; set; }

        // Navigation Properties
        public virtual ICollection<Ticket> Tickets { get; set; } = new HashSet<Ticket>();

        public virtual ICollection<BTUser> User { get; set; } = new HashSet<BTUser>();
    }
}
