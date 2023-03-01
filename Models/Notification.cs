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
        public string? Title { get; set; }

        [Required]
        public string? Message { get; set; }

        [DataType(DataType.Date)]
        public DateTime Created { get; set; }

        // Foriegn Key
        [Required]
        public string? SenderId { get; set; }

        // Foriegn Key
        [Required]
        public string? RecipientId { get; set; }

        public int NotificationTypeId { get; set; }

        public bool HasBeenViewed { get; set; }

        // Navigation Properties
        public virtual NotificationType? NotificationType { get; set; }

        public virtual Ticket? Ticket { get; set; }

        public virtual Project? Project { get; set; }

        public virtual BTUser? Sender { get; set; }

        public virtual BTUser? Recipient { get; set; }
    }
}
