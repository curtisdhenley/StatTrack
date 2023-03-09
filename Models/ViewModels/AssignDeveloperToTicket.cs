using Microsoft.AspNetCore.Mvc.Rendering;

namespace StatTracker.Models.ViewModels
{
    public class AssignDeveloperToTicket
    {
        public Ticket? Ticket { get; set; }
        public SelectList? DeveloperList { get; set; }
        public string? DeveloperId { get; set; }
    }
}
