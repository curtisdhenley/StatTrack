using StatTracker.Models;

namespace StatTracker.Services.Interfaces
{
    public interface IBTTicketService
    {
        #region Ticket CRUD Methods
        // AddTicketAsync(Ticket ticket)
        public Task AddTicketAsync(Ticket ticket);

        // UpdateTicketAsync(Ticket ticket)
        public Task UpdateTicketAsync(Ticket ticket);

        // GetTicketAsync(int ticketId)
        public Task<Ticket> GetTicketAsync();

        // GetTicketsAsync()
        public Task<IEnumerable<Ticket>> GetTicketsAsync(int? companyId);

        // DeleteTicketAsync(Ticket ticket)
        public Task DeleteTicketAsync(Ticket ticket);
        #endregion

        #region Get Tickets Methods
        public Task<Ticket> GetTicketByIdAsync(int ticketId);

        public Task<Ticket> GetTicketByIdAsync(int? ticketId, int? companyId);

        public Task<IEnumerable<Ticket>> GetTicketsByCompanyAsync(int companyId);

        // GetTicketsByPriorityAsync()
        public Task<IEnumerable<Ticket>> GetTicketsByPriorityAsync();

        /// <summary>
        /// Gets the specified number of Tickets based on priority.
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public Task<IEnumerable<Ticket>> GetTicketsByPriorityAsync(int ticketPriorityId);

        // GetTicketsByEndDate()
        public Task<IEnumerable<Ticket>> GetTicketsByEndDate();

        /// <summary>
        /// Gets the specified number of Tickets based on endDate.
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public Task<IEnumerable<Ticket>> GetTicketsByEndDate(DateTime endDate);

        public Task<IEnumerable<Ticket>> GetUnassignedTicketsAsync(BTUser? user);

        #endregion

        #region Addtional Methods
        public Task AddTicketsToTicketAsync(IEnumerable<Ticket> ticketIds, int projectId);
        public Task<bool> IsTagOnBlogPostAsync(int tagId, int projectId);
        public Task<bool> ValidateSlugAsync(string title, int blogId);
        public Task AddTicketAttachmentAsync(TicketAttachment ticketAttachment);
        public Task<TicketAttachment> GetTicketAttachmentByIdAsync(int ticketAttachmentId);
        public Task<BTUser> GetDeveloperAsync(int? ticketId);
        public Task<BTUser> GetSubmitterAsync(int? ticketId);
        public Task<bool> AddDeveloperToTicketAsync(string? userId, int? ticketId);
        public Task<Ticket> GetTicketAsNoTrackingAsync(int? ticketId, int? companyId);
        public Task RemoveDeveloperAsync(int? ticketId);

        #endregion
    }
}
