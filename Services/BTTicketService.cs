﻿using Microsoft.AspNetCore.Identity;
using Microsoft.Build.Evaluation;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using StatTracker.Data;
using StatTracker.Models;
using StatTracker.Models.Enums;
using StatTracker.Services.Interfaces;
using System.ComponentModel.Design;
using Project = StatTracker.Models.Project;

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

        public async Task<bool> AddDeveloperToTicketAsync(string? userId, int? ticketId)
        {
            try
            {
                BTUser? currentDev = await GetDeveloperAsync(ticketId);
                BTUser? selectedDev = await _context.Users.FindAsync(userId);

                // Remove the current Dev
                if (currentDev != null)
                {
                    await RemoveDeveloperAsync(ticketId);
                }

                // Add new/selected Dev
                try
                {
                    //Project? project = (await _projectService.GetProjectsAsync(selectedDev!.CompanyId)).FirstOrDefault();
                    //await _rolesService.AddUserToRoleAsync(selectedDev!, nameof(BTRoles.Developer));
                    //await _projectService.AddMemberToProjectAsync(selectedDev!, project!.Id);

                    Ticket? ticket = await GetTicketAsNoTrackingAsync(ticketId, selectedDev!.CompanyId);

                    ticket.DeveloperUserId = userId;

                    return true;
                }
                catch (Exception)
                {

                    throw;
                }
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

        public async Task<BTUser> GetDeveloperAsync(int? ticketId)
        {
            try
            {
                Ticket? ticket = await _context.Tickets.Include(t => t.DeveloperUser).SingleOrDefaultAsync(t => t.Id == ticketId);

                    if (ticket!.DeveloperUser != null && await _rolesService.IsUserInRoleAsync(ticket!.DeveloperUser, nameof(BTRoles.Developer)))
                    {
                        return ticket.DeveloperUser!;
                    }
                

                return null!;
            }
            catch (Exception)
            {

                throw;
            }
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
                                               .FirstOrDefaultAsync(m => m.Id == ticketId && m.Project!.CompanyId == companyId);

                return ticket!;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<IEnumerable<Ticket>> GetTicketsAsync(int? companyId)
		{
			IEnumerable<Ticket> tickets = await _context.Tickets
											            .Where(t => t.Project!.CompanyId == companyId && t.Project.Archived == false && t.Archived == false)
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

        public async Task RemoveDeveloperAsync(int? ticketId)
        {
            try
            {
                Ticket? ticket = await _context.Tickets.Include(t => t.DeveloperUser).Include(t => t.ProjectId).FirstOrDefaultAsync(t => t.Id == ticketId);

                    if (await _rolesService.IsUserInRoleAsync(ticket!.DeveloperUser, nameof(BTRoles.Developer)))
                    {
                        await _projectService.RemoveMemberFromProjectAsync(ticket.DeveloperUser!, ticket.ProjectId);
                    }
                
            }
            catch (Exception)
            {

                throw;
            }
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
