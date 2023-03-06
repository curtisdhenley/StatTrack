using StatTracker.Models;
using StatTracker.Services.Interfaces;

namespace StatTracker.Services
{

    public class TicketService : ITicketService
    {
        public Task AddTicketAsync(Ticket ticket)
        {
            throw new NotImplementedException();
        }

        public Task AddTicketsToTicketAsync(IEnumerable<Ticket> ticketIds, int projectId)
        {
            throw new NotImplementedException();
        }

        public Task DeleteTicketAsync(Ticket ticket)
        {
            throw new NotImplementedException();
        }

        public Task<Ticket> GetTicketAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Ticket>> GetTicketsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Ticket>> GetTicketsByCompanyAsync(int companyId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Ticket>> GetTicketsByEndDate()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Ticket>> GetTicketsByEndDate(DateTime endDate)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Ticket>> GetTicketsByPriorityAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Ticket>> GetTicketsByPriorityAsync(int ticketPriorityId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsTagOnBlogPostAsync(int tagId, int projectId)
        {
            throw new NotImplementedException();
        }

        public Task UpdateTicketAsync(Ticket ticket)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ValidateSlugAsync(string title, int blogId)
        {
            throw new NotImplementedException();
        }
    }
}
