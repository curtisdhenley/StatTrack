using System.ComponentModel.DataAnnotations;

namespace StatTracker.Models
{
    public class NotificationType
    {
        //Primary Key
        public int Id { get; set; }

        [Display(Name = "Notification Type")]
        public string? Name { get; set; }
    }
}
