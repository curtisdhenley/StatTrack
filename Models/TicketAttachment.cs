using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StatTracker.Models
{
    public class TicketAttachment
    {
        // Primary Key
        public int Id { get; set; }

        [StringLength(200, ErrorMessage = "The {0} must be at least {2} and max {1} characters long.", MinimumLength = 2)]
        public string? Description { get; set; }

        [DataType(DataType.Date)]
        public DateTime Created { get; set; }

        // Foreign Key
        public int TicketId { get; set; }

        // Foreign Key
        [Required]
        public string? BTUserId { get; set; }

        // Attchment Properties
        public byte[]? FileData { get; set; }

        public string? FileType { get; set; }

        [NotMapped]
        public virtual IFormFile? FormFile { get; set; }

        // Navigation Properties
        public virtual Ticket? Ticket { get; set; }

        public virtual BTUser? BTUser { get; set; }
    }
}
