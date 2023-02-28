using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace StatTracker.Models
{
    public class Notification
    {
        // Primary Key
        public int Id { get; set; }

        // Foriegn Key
        public int ProjectId { get; set; }

        // Foriegn Key
        public int TicketId { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Message { get; set; }

        [DataType(DataType.Date)]
        public DateTime Created { get; set; }

        // Foriegn Key
        [Required]
        public string SenderId { get; set; }

        // Foriegn Key
        [Required]
        public string RecipientId { get; set; }

        public int NotificationTypeId { get; set; }

        public bool HasBeenViewed { get; set; }

        // Navigation Properties
        public virtual ICollection<NotificationType?> NotificationType { get; set; } = new HashSet<NotificationType?>();

        public virtual ICollection<Ticket> Tickets { get; set; } = new HashSet<Ticket>();

        public virtual ICollection<Project> Projects { get; set; } = new HashSet<Project>();

        public virtual ICollection<BTUser> Senders { get; set; } = new HashSet<BTUser>();

        public virtual ICollection<BTUser> Recipients { get; set; } = new HashSet<BTUser>();
    }
}
