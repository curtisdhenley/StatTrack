using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StatTracker.Data;
using StatTracker.Extensions;
using StatTracker.Models;
using StatTracker.Services;
using StatTracker.Services.Interfaces;
using StatTracker.Data;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using StatTracker.Models.Enums;
using StatTracker.Models.ViewModels;

namespace StatTracker.Controllers
{
    [Authorize]
    public class TicketsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IBTTicketService _ticketService;
        private readonly UserManager<BTUser> _userManager;
        private readonly IBTFileService _fileService;
        private readonly IBTRolesService _rolesService;
        private readonly IBTTicketHistoryService _historyService;
        private readonly IBTProjectService _projectService;
        private readonly IBTNotificationService _notificationService;

        public TicketsController(ApplicationDbContext context, IBTTicketService ticketService, UserManager<BTUser> userManager, IBTFileService fileService, IBTRolesService rolesService, IBTTicketHistoryService historyService, IBTProjectService projectService, IBTNotificationService notificationService)
        {
            _context = context;
            _ticketService = ticketService;
            _userManager = userManager;
            _fileService = fileService;
            _rolesService = rolesService;
            _historyService = historyService;
            _projectService = projectService;
            _notificationService = notificationService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin, ProjectManager")]
        public async Task<IActionResult> AssignDeveloperToTicket(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Get companyId
            int companyId = User.Identity!.GetCompanyId();

            Ticket? ticket = await _ticketService.GetTicketByIdAsync(id.Value);

            IEnumerable<BTUser> developers = await _rolesService.GetUsersInRoleAsync(nameof(BTRoles.Developer), companyId);

            AssignDeveloperToTicket viewModel = new()
            {
                Ticket = ticket,
                DeveloperList = new SelectList(developers, "Id", "FullName", ticket?.DeveloperUserId),
                DeveloperId = ticket?.DeveloperUserId
            };

            return View(viewModel);
        }

        [HttpPost]
        [Authorize(Roles = "Admin, ProjectManager")]
        public async Task<IActionResult> AssignDeveloperToTicket(AssignDeveloperToTicket viewModel)
        {
            if (viewModel.DeveloperId != null)
            {
                int companyId = User.Identity!.GetCompanyId();

                Ticket? oldTicket = await _ticketService.GetTicketAsNoTrackingAsync(viewModel.Ticket?.Id, companyId);

                try
                {
                    //await _ticketService.AddDeveloperToTicketAsync(viewModel.DeveloperList, viewModel.Ticket!.Id!);
                }
                catch (Exception)
                {

                    throw;
                }

                string? userId = _userManager.GetUserId(User);
                Ticket? newTicket = await _ticketService.GetTicketAsNoTrackingAsync(viewModel.Ticket?.Id, companyId);
                await _historyService.AddHistoryAsync(oldTicket, newTicket, userId);

                BTUser? btUser = await _userManager.GetUserAsync(User);

                Notification? notification = new()
                {
                    TicketId = viewModel.Ticket!.Id,
                    Title = "Developer Assigned",
                    Message = $"Ticket: {viewModel.Ticket!.Title} was assigned by {btUser!.FullName}",
                    Created = DataUtility.GetPostGresDate(DateTime.Now),
                    SenderId = userId,
                    RecipientId = viewModel.DeveloperId,
                    NotificationTypeId = (await _context.NotificationTypes.FirstOrDefaultAsync(n => n.Name == nameof(BTNotificationTypes.Ticket)))!.Id
                };

                await _notificationService.AddNotificationAsync(notification);
                await _notificationService.SendEmailNotificationAsync(notification, "New Developer Assigned To Ticket");


                return RedirectToAction(nameof(Details), new { id = viewModel.Ticket?.Id });
            }

            return View(viewModel);
        }

        // GET: Tickets
        public async Task<IActionResult> Index()
        {


            IEnumerable<Ticket> tickets = await _ticketService.GetTicketsAsync();

            //var applicationDbContext = _context.Tickets.Include(t => t.DeveloperUser).Include(t => t.Project).Include(t => t.SubmitterUser).Include(t => t.TicketPriority).Include(t => t.TicketStatus).Include(t => t.TicketType);
            return View(tickets);
        }

        // GET: Tickets/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _ticketService.GetTicketByIdAsync(id.Value);

            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        // GET: Tickets/AddTicketComment
        public IActionResult Create()
        {
            ViewData["DeveloperUserId"] = new SelectList(_context.Users, "Id", "Id");
            ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Description");
            ViewData["SubmitterUserId"] = new SelectList(_context.Users, "Id", "Id");
            ViewData["TicketPriorityId"] = new SelectList(_context.TicketPriorities, "Id", "Id");
            ViewData["TicketStatusId"] = new SelectList(_context.TicketStatuses, "Id", "Id");
            ViewData["TicketTypeId"] = new SelectList(_context.TicketTypes, "Id", "Id");
            return View();
        }

        // POST: Tickets/AddTicketComment
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,Created,Updated,Archived,ArchivedByProject,ProjectId,TicketTypeId,TicketStatusId,TicketPriorityId,DeveloperUserId,SubmitterUserId")] Ticket ticket)
        {
            BTUser? btUser = await _userManager.GetUserAsync(User);

            ModelState.Remove("SubmitterUserId");

            if (ModelState.IsValid)
            {
                string? userId = _userManager.GetUserId(User);

                ticket.Created = DataUtility.GetPostGresDate(DateTime.Now);
                ticket.SubmitterUserId = userId;
                ticket.TicketStatusId = (await _context.TicketStatuses.FirstOrDefaultAsync(s => s.Name == nameof(BTTicketStatuses.New)))!.Id;
                await _ticketService.AddTicketAsync(ticket);
                int companyId = User.Identity!.GetCompanyId();
                Ticket? newTicket = await _ticketService.GetTicketAsNoTrackingAsync(ticket.Id, companyId);
                

                await _historyService.AddHistoryAsync(null, newTicket, userId);

                BTUser? projectManager = await _projectService.GetProjectManagerAsync(ticket.ProjectId);

                Notification? notification = new()
                {
                    TicketId = ticket.Id,
                    Title = "New Ticket Added",
                    Message = $"New Ticket: {ticket.Title} was created by {btUser!.FullName}",
                    Created = DataUtility.GetPostGresDate(DateTime.Now),
                    SenderId = userId,
                    RecipientId = projectManager?.Id,
                    NotificationTypeId = (await _context.NotificationTypes.FirstOrDefaultAsync(n => n.Name == nameof(BTNotificationTypes.Ticket)))!.Id,
                    ProjectId = ticket.ProjectId
                };

                if (projectManager != null )
                {
                    await _notificationService.AddNotificationAsync(notification);
                    await _notificationService.SendEmailNotificationAsync(notification, "New Ticket Added");
                }
                else
                {
                    await _notificationService.AdminNotificationAsync(notification, companyId);
                    await _notificationService.SendAdminEmailNotificationAsync(notification, "New Project underTicket Added", companyId);
                }



                return RedirectToAction(nameof(Index));
            }
            ViewData["DeveloperUserId"] = new SelectList(_context.Users, "Id", "Id", ticket.DeveloperUserId);
            ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Description", ticket.ProjectId);
            ViewData["SubmitterUserId"] = new SelectList(_context.Users, "Id", "Id", ticket.SubmitterUserId);
            ViewData["TicketPriorityId"] = new SelectList(_context.TicketPriorities, "Id", "Id", ticket.TicketPriorityId);
            ViewData["TicketStatusId"] = new SelectList(_context.TicketStatuses, "Id", "Id", ticket.TicketStatusId);
            ViewData["TicketTypeId"] = new SelectList(_context.TicketTypes, "Id", "Id", ticket.TicketTypeId);
            return View(ticket);
        }

        // GET: Tickets/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Tickets == null)
            {
                return NotFound();
            }

            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null)
            {
                return NotFound();
            }
            ViewData["DeveloperUserId"] = new SelectList(_context.Users, "Id", "Id", ticket.DeveloperUserId);
            ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Description", ticket.ProjectId);
            ViewData["SubmitterUserId"] = new SelectList(_context.Users, "Id", "Id", ticket.SubmitterUserId);
            ViewData["TicketPriorityId"] = new SelectList(_context.TicketPriorities, "Id", "Id", ticket.TicketPriorityId);
            ViewData["TicketStatusId"] = new SelectList(_context.TicketStatuses, "Id", "Id", ticket.TicketStatusId);
            ViewData["TicketTypeId"] = new SelectList(_context.TicketTypes, "Id", "Id", ticket.TicketTypeId);
            return View(ticket);
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,Created,Updated,Archived,ArchivedByProject,ProjectId,TicketTypeId,TicketStatusId,TicketPriorityId,DeveloperUserId,SubmitterUserId")] Ticket ticket)
        {
            if (id != ticket.Id)
            {
                return NotFound();
            }

            BTUser? btUser = await _userManager.GetUserAsync(User);

            if (ModelState.IsValid)
            {
                int companyId = User.Identity!.GetCompanyId();
                string? userId = _userManager.GetUserId(User);
                Ticket? oldTicket = await _ticketService.GetTicketAsNoTrackingAsync(ticket.Id, companyId);

                try
                {
                    ticket.Created = DataUtility.GetPostGresDate(ticket.Created);

                    if (ticket.Updated != null)
                    {
                        ticket.Updated = DataUtility.GetPostGresDate(DateTime.Now);
                    }

                    await _ticketService.UpdateTicketAsync(ticket);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TicketExists(ticket.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                // Add history
                Ticket? newTicket = await _ticketService.GetTicketAsNoTrackingAsync(ticket.Id, companyId);

                await _historyService.AddHistoryAsync(oldTicket, newTicket, userId);

                // Notification
                BTUser? projectManager = await _projectService.GetProjectManagerAsync(ticket.ProjectId);

                Notification? notification = new()
                {
                    TicketId = ticket.Id,
                    Title = "New Ticket Added",
                    Message = $"New Ticket: {ticket.Title} was created by {btUser!.FullName}",
                    Created = DataUtility.GetPostGresDate(DateTime.Now),
                    SenderId = userId,
                    RecipientId = projectManager?.Id,
                    NotificationTypeId = (await _context.NotificationTypes.FirstOrDefaultAsync(n => n.Name == nameof(BTNotificationTypes.Ticket)))!.Id
                };

                if (projectManager != null)
                {
                    await _notificationService.AddNotificationAsync(notification);
                    await _notificationService.SendEmailNotificationAsync(notification, "New Ticket Added");
                }
                else
                {
                    await _notificationService.AdminNotificationAsync(notification, companyId);
                    await _notificationService.SendAdminEmailNotificationAsync(notification, "New Project underTicket Added", companyId);
                }


                return RedirectToAction(nameof(Index));
            }
            ViewData["DeveloperUserId"] = new SelectList(_context.Users, "Id", "Id", ticket.DeveloperUserId);
            ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Description", ticket.ProjectId);
            ViewData["SubmitterUserId"] = new SelectList(_context.Users, "Id", "Id", ticket.SubmitterUserId);
            ViewData["TicketPriorityId"] = new SelectList(_context.TicketPriorities, "Id", "Id", ticket.TicketPriorityId);
            ViewData["TicketStatusId"] = new SelectList(_context.TicketStatuses, "Id", "Id", ticket.TicketStatusId);
            ViewData["TicketTypeId"] = new SelectList(_context.TicketTypes, "Id", "Id", ticket.TicketTypeId);
            return View(ticket);
        }

        // GET: Tickets/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Tickets == null)
            {
                return NotFound();
            }

            var ticket = await _context.Tickets
                .Include(t => t.DeveloperUser)
                .Include(t => t.Project)
                .Include(t => t.SubmitterUser)
                .Include(t => t.TicketPriority)
                .Include(t => t.TicketStatus)
                .Include(t => t.TicketType)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Tickets == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Ticket'  is null.");
            }
            Ticket? ticket = await _context.Tickets.FindAsync(id);
            if (ticket != null)
            {
                ticket.Archived = true;
                _context.Tickets.Update(ticket);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // POST: Comments/AddTicketComment
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddTicketComment([Bind("Id,Comment,Created,TicketId,UserId")] TicketComment? ticketComment)
        {
            ModelState.Remove("UserId");

            if (ModelState.IsValid)
            {
                // display information based on user
                ticketComment!.UserId = _userManager.GetUserId(User);

                ticketComment.Created = DataUtility.GetPostGresDate(DateTime.Now);

                Ticket? ticket = await _context.Tickets.FindAsync(ticketComment.TicketId);

                ticket?.Comments.Add(ticketComment);

                await _context.SaveChangesAsync();

                await _historyService.AddHistoryAsync(ticketComment.TicketId, nameof(TicketComment), ticketComment.UserId);
            }
            //return RedirectToAction("Details", "BlogPosts", new { slug = slug });
            return RedirectToAction("Details", "Tickets", ticketComment?.TicketId);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddTicketAttachment([Bind("Id,FormFile,Description,TicketId")] TicketAttachment ticketAttachment)
        {
            string statusMessage;

            if (ModelState.IsValid && ticketAttachment.FormFile != null)
            {
                try
                {
                    ticketAttachment.FileData = await _fileService.ConvertFileToByteArrayAsync(ticketAttachment.FormFile);
                    ticketAttachment.FileType = ticketAttachment.FormFile.ContentType;

                    ticketAttachment.Created = DataUtility.GetPostGresDate(DateTime.UtcNow);
                    ticketAttachment.BTUserId = _userManager.GetUserId(User);

                    await _ticketService.AddTicketAttachmentAsync(ticketAttachment);

                    await _historyService.AddHistoryAsync(ticketAttachment.TicketId, nameof(TicketAttachment), ticketAttachment.BTUserId);

                }
                catch (Exception)
                {

                    throw;
                }
                statusMessage = "Success: New attachment added to Ticket.";
            }
            else
            {
                statusMessage = "Error: Invalid data.";

            }

            return RedirectToAction("Details", new { id = ticketAttachment.TicketId, message = statusMessage });
        }

        public async Task<IActionResult> ShowFile(int id)
        {
            TicketAttachment ticketAttachment = await _ticketService.GetTicketAttachmentByIdAsync(id);
            string fileName = ticketAttachment.FileName!;
            byte[] fileData = ticketAttachment.FileData!;
            string ext = Path.GetExtension(fileName).Replace(".", "");

            Response.Headers.Add("Content-Disposition", $"inline; filename={fileName}");
            return File(fileData, $"application/{ext}");
        }

        private bool TicketExists(int id)
        {
            return (_context.Tickets?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
