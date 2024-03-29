﻿using System.ComponentModel.DataAnnotations;

namespace StatTracker.Models
{
    public class TicketComment
    {
        // Primary Key
        public int Id { get; set; }

        [Required]
        [StringLength(500, ErrorMessage = "The {0} must be at least {2} and max {1} characters long.", MinimumLength = 2)]
        public string? Comment { get; set; }

        [DataType(DataType.Date)]
        public DateTime Created { get; set; }

        // Foreign Key
        public int TicketId { get; set; }

        // Foreign Key
        [Required]
        public string? UserId { get; set; }

        // Navigation Properties
        public virtual Ticket? Ticket { get; set; }

        public virtual BTUser? BTUser { get; set; }
    }
}
