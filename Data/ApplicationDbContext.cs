using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StatTracker.Models;

namespace StatTracker.Data
{
    public class ApplicationDbContext : IdentityDbContext<BTUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<StatTracker.Models.Company> Companies { get; set; } = default!;
        public DbSet<StatTracker.Models.Invite> Invites { get; set; } = default!;
        public DbSet<StatTracker.Models.Notification> Notifications { get; set; } = default!;
        public DbSet<StatTracker.Models.NotificationType> NotificationTypes { get; set; } = default!;
        public DbSet<StatTracker.Models.Project> Projects { get; set; } = default!;
        public DbSet<StatTracker.Models.ProjectPriority> ProjectPriorities { get; set; } = default!;
        public DbSet<StatTracker.Models.TicketType> TicketTypes { get; set; } = default!;
        public DbSet<StatTracker.Models.TicketStatus> TicketStatuses { get; set; } = default!;
        public DbSet<StatTracker.Models.TicketPriority> TicketPriorities { get; set; } = default!;
        public DbSet<StatTracker.Models.Ticket> Tickets { get; set; } = default!;
        public DbSet<StatTracker.Models.TicketAttachment> TicketAttachments { get; set; } = default!;
        public DbSet<StatTracker.Models.TicketComment> TicketComments { get; set; } = default!;
        public DbSet<StatTracker.Models.TicketHistory> TicketHistories { get; set; } = default!;
    }
}