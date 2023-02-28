using System.ComponentModel.DataAnnotations;

namespace StatTracker.Models
{
    public class TicketComment
    {
        // Primary Key
        public int Id { get; set; }

        [Required]
        public string Comment { get; set; }

        [DataType(DataType.Date)]
        public DateTime Created { get; set; }

        // Foreign Key
        public int TicketId { get; set; }

        // Foreign Key
        public string UserId { get; set; }

        // Navigation Properties
        public virtual ICollection<Ticket> Tickets { get; set; } = new HashSet<Ticket>();

        public virtual ICollection<BTUser> User { get; set; } = new HashSet<BTUser>();
    }
}
