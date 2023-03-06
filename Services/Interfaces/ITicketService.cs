using Azure;
using StatTracker.Models;

namespace StatTracker.Services.Interfaces
{
    public interface ITicketService
    {
        #region Ticket CRUD Methods
        // AddTicketAsync(Ticket ticket)
        public Task AddTicketAsync(Ticket ticket);

        // UpdateTicketAsync(Ticket ticket)
        public Task UpdateTicketAsync(Ticket ticket);

        // GetTicketAsync(int ticketId)
        public Task<Ticket> GetTicketAsync(int id);

        // GetTicketsAsync()
        public Task<IEnumerable<Ticket>> GetTicketsAsync();

        // DeleteTicketAsync(Ticket ticket)
        public Task DeleteTicketAsync(Ticket ticket);
        #endregion

        #region Get Tickets Methods

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

        #endregion

        #region Addtional Methods
        public Task AddTicketsToTicketAsync(IEnumerable<Ticket> ticketIds, int projectId);
        public Task<bool> IsTagOnBlogPostAsync(int tagId, int projectId);
        public Task<bool> ValidateSlugAsync(string title, int blogId);

        

        #endregion
    }
}
