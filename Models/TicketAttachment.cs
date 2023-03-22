using StatTracker.Extensions;
using System.ComponentModel;
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

        // Attachment Properties
        public byte[]? FileData { get; set; }

        public string? FileType { get; set; }

		public string? FileName { get; set; }

        [NotMapped]
        [DisplayName("Select a file")]
        [DataType(DataType.Upload)]
        [MaxFileSize(1024 * 1024)]
        [AllowedExtensions(new string[] { ".jpg", ".png", ".doc", ".docx", ".xls", ".xlsx", ".pdf" })]
        public IFormFile? FormFile { get; set; }

        // Navigation Properties
        public virtual Ticket? Ticket { get; set; }

        public virtual BTUser? BTUser { get; set; }
    }
}
