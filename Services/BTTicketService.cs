using Microsoft.AspNetCore.Identity;
using Microsoft.Build.Evaluation;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using StatTracker.Data;
using StatTracker.Models;
using StatTracker.Models.Enums;
using StatTracker.Services.Interfaces;
using System.ComponentModel.Design;

namespace StatTracker.Services
{

	public class BTTicketService : IBTTicketService
	{
		private readonly ApplicationDbContext _context;
        private readonly UserManager<BTUser> _userManager;
        private readonly IBTRolesService _rolesService;
        private readonly IBTProjectService _projectService;

        public BTTicketService(ApplicationDbContext context, UserManager<BTUser> userManager, IBTRolesService rolesService, IBTProjectService projectService)
        {
            _context = context;
            _userManager = userManager;
            _rolesService = rolesService;
            _projectService = projectService;
        }

        public async Task<bool> AddDeveloperToTicketAsync(BTUser user, int? ticketId)
        {
            try
            {
                Ticket? ticket = await GetTicketByIdAsync(ticketId!.Value);

                if (ticket.DeveloperUserId != null)
                {
                    ticket.DeveloperUser = user;
                    await _context.SaveChangesAsync();
                    return true;
                }

                return false;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task AddTicketAsync(Ticket ticket)
		{
			if (ticket != null)
			{
				try
				{
					await _context.AddAsync(ticket);
					await _context.SaveChangesAsync();
				}
				catch (Exception)
				{

					throw;
				} 
			}
		}

        public async Task AddTicketAttachmentAsync(TicketAttachment ticketAttachment)
        {
            try
            {
                await _context.AddAsync(ticketAttachment);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public Task AddTicketsToTicketAsync(IEnumerable<Ticket> ticketIds, int projectId)
		{
			throw new NotImplementedException();
		}

		public Task DeleteTicketAsync(Ticket ticket)
		{
			throw new NotImplementedException();
		}

        public Task<BTUser> GetDeveloperAsync(int? ticketId)
        {
            throw new NotImplementedException();
        }

        //      public async Task<BTUser> GetDeveloperAsync(int? ticketId)
        //      {
        //	try
        //	{
        //		Ticket? ticket = await _context.Tickets.Include(t => t.DeveloperUser).FirstOrDefaultAsync(t => t.Id == ticketId);

        //		return ticket!;
        //	}
        //	catch (Exception)
        //	{

        //		throw;
        //	}
        //}

        public Task<BTUser> GetSubmitterAsync(int? ticketId)
        {
            throw new NotImplementedException();
        }

        public async Task<Ticket> GetTicketAsNoTrackingAsync(int? ticketId, int? companyId)
        {
			try
			{
                Ticket? ticket = await _context.Tickets
                                              .Include(t => t.Project)
                                                .ThenInclude(p => p!.Company)
                                              .Include(t => t.Attachments)
                                              .Include(t => t.Comments)
                                              .Include(t => t.DeveloperUser)
                                              .Include(t => t.History)
                                              .Include(t => t.SubmitterUser)
                                              .Include(t => t.TicketPriority)
                                              .Include(t => t.TicketStatus)
                                              .Include(t => t.TicketType)
                                              .AsNoTracking()
                                              .FirstOrDefaultAsync(t => t.Id == ticketId && t.Project!.CompanyId == companyId && t.Archived == false);

				return ticket!;
            }
			catch (Exception)
			{

				throw;
			}
            
		}

        public async Task<Ticket> GetTicketAsync()
		{
			try
			{
				Ticket? ticket = await _context.Tickets
											   .Where(t => t.Archived == false)
											   .Include(t => t.DeveloperUser)
											   .Include(t => t.Project)
											   .Include(t => t.SubmitterUser)
											   .Include(t => t.TicketPriority)
											   .Include(t => t.TicketStatus)
											   .Include(t => t.TicketType)
											   .FirstOrDefaultAsync();

				return ticket!;
			}
			catch (Exception)
			{

				throw;
			}
		}

        public async Task<TicketAttachment> GetTicketAttachmentByIdAsync(int ticketAttachmentId)
        {
            try
            {
                TicketAttachment? ticketAttachment = await _context.TicketAttachments
                                                                  .Include(t => t.BTUser)
                                                                  .FirstOrDefaultAsync(t => t.Id == ticketAttachmentId);
                return ticketAttachment!;
            }
            catch (Exception)
            {

                throw;
            }
        }


        public async Task<Ticket> GetTicketByIdAsync(int ticketId)
		{
			try
			{
				Ticket? ticket = await _context.Tickets
											   .Include(t => t.DeveloperUser)
											   .Include(t => t.SubmitterUser)
											   .Include(t => t.Project)
											   .Include(t => t.TicketPriority)
											   .Include(t => t.TicketStatus)
											   .Include(t => t.TicketType)
											   .Include(t => t.Comments)
											   .Include(t => t.Attachments)
											   .Include(t => t.History)
											   .FirstOrDefaultAsync(m => m.Id == ticketId);

				return ticket!;
			}
			catch (Exception)
			{

				throw;
			}
		}

        public async Task<Ticket> GetTicketByIdAsync(int? ticketId, int? companyId)
        {
            try
            {
                Ticket? ticket = await _context.Tickets
                                               .Include(t => t.DeveloperUser)
                                               .Include(t => t.SubmitterUser)
                                               .Include(t => t.Project)
                                               .Include(t => t.TicketPriority)
                                               .Include(t => t.TicketStatus)
                                               .Include(t => t.TicketType)
                                               .Include(t => t.Comments)
                                               .Include(t => t.Attachments)
                                               .Include(t => t.History)
                                               .FirstOrDefaultAsync(m => m.Id == ticketId && m.Project.CompanyId == companyId);

                return ticket!;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<IEnumerable<Ticket>> GetTicketsAsync()
		{
			IEnumerable<Ticket> tickets = await _context.Tickets
											   .Where(t => t.Archived == false)
											   .Include(t => t.DeveloperUser)
											   .Include(t => t.Project)
											   .Include(t => t.SubmitterUser)
											   .Include(t => t.TicketPriority)
											   .Include(t => t.TicketStatus)
											   .Include(t => t.TicketType)
											   .ToListAsync();

			return tickets!;
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

        public async Task<IEnumerable<Ticket>> GetUnassignedTicketsAsync(BTUser? user)
        {
            try
            {
                List<Ticket> tickets = new List<Ticket>();
                //Get all unassigned tickets for the admin int he company
				if (await _rolesService.IsUserInRoleAsync(user!, nameof(BTRoles.Admin)))
                {
                    tickets = await _context.Tickets
											.Where(t => t.Project!.CompanyId == user!.CompanyId && t.Archived == false && t.DeveloperUser == null)
											.ToListAsync();
                }

                if (await _rolesService.IsUserInRoleAsync(user!, nameof(BTRoles.ProjectManager)))
                {
                    tickets = await _context.Tickets
											.Where(t => t.Project!.CompanyId == user!.CompanyId && t.Archived == false && t.DeveloperUser == null && user!.Projects!.Contains(t.Project))
                                            .ToListAsync();
                }
                return tickets;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Task<bool> IsTagOnBlogPostAsync(int tagId, int projectId)
		{
			throw new NotImplementedException();
		}

		public async Task UpdateTicketAsync(Ticket ticket)
		{
			_context.Update(ticket);
			await _context.SaveChangesAsync();
		}

		public Task<bool> ValidateSlugAsync(string title, int blogId)
		{
			throw new NotImplementedException();
		}
	}
}
