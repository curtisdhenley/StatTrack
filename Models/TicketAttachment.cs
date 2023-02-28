using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StatTracker.Models
{
    public class TicketAttachment
    {
        // Primary Key
        public int Id { get; set; }

        public string Description { get; set; }

        [DataType(DataType.Date)]
        public DateTime Created { get; set; }

        // Foreign Key
        public int TicketId { get; set; }

        // Foreign Key
        [Required]
        public string BTUserId { get; set; }

        // Image Properties
        public byte[]? ImageData { get; set; }

        public string? ImageType { get; set; }

        [NotMapped]
        public virtual IFormFile? ImageFile { get; set; }

        // Navigation Properties
        public virtual ICollection<Ticket> Tickets { get; set; } = new HashSet<Ticket>();

        public virtual ICollection<BTUser> User { get; set; } = new HashSet<BTUser>();
    }
}
