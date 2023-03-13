using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StatTracker.Models
{
    public class EmailData
    {
        [Required]
        public string? EmailSenderAddress { get; set; }
        public string? EmailSubject { get; set;}
        [Required]
        public string? EmailBody { get; set; }

        [Required]
        public string? EmailSenderName { get; set; }
        public string? LastName { get; set;}

        //[NotMapped]
        //[Display(Name = "Full Name")]
        //public string FullName { get { return $"{FirstName} {LastName}"; } }
        public string? GroupName { get; set;}

    }
}