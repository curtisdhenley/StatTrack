using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StatTracker.Data;
using StatTracker.Extensions;
using StatTracker.Models;
using StatTracker.Services;
using StatTracker.Services.Interfaces;

namespace StatTracker.Controllers
{
    [Authorize(Roles = "Admin")]
    public class InvitesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IBTInviteService _inviteService;
        private readonly IBTProjectService _projectService;
        private readonly IBTCompanyService _companyService;
        private readonly IEmailSender _emailService;
        private readonly IDataProtector _dataProtector;
        private readonly UserManager<BTUser> _userManager;
        private readonly IConfiguration _configuration;

        public InvitesController(ApplicationDbContext context, IBTInviteService inviteService, IBTProjectService projectService, IBTCompanyService companyService, IEmailSender emailService, UserManager<BTUser> userManager, IDataProtectionProvider dataProtectionProvider, IConfiguration configuration)
        {
            _context = context;
            _inviteService = inviteService;
            _projectService = projectService;
            _companyService = companyService;
            _emailService = emailService;
            _userManager = userManager;
            _configuration = configuration;
            _dataProtector = dataProtectionProvider.CreateProtector(configuration.GetValue<string>("ProtectKey")! ?? Environment.GetEnvironmentVariable("ProtectKey")!);
        }

        // GET: Invites
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Invites.Include(i => i.Company).Include(i => i.Invitee).Include(i => i.Invitor).Include(i => i.Project);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Invites/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Invites == null)
            {
                return NotFound();
            }

            var invite = await _context.Invites
                .Include(i => i.Company)
                .Include(i => i.Invitee)
                .Include(i => i.Invitor)
                .Include(i => i.Project)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (invite == null)
            {
                return NotFound();
            }

            return View(invite);
        }

        // GET: Invites/AddTicketComment
        public async Task<IActionResult> Create()
        {
            int companyId = User.Identity!.GetCompanyId();

            ViewData["ProjectId"] = new SelectList(await _projectService.GetProjectsAsync(companyId), "Id", "Description");
            return View();
        }

        // POST: Invites/AddTicketComment
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ProjectId,InvitorId,InviteeEmail,InviteeFirstName,InviteeLastName,Message")] Invite invite)
        {
            ModelState.Remove("InvitorId");

            int companyId = User.Identity!.GetCompanyId();

            if (ModelState.IsValid)
            {
                try
                {
                    Guid guid = Guid.NewGuid();

                    string? token = _dataProtector.Protect(guid.ToString());
                    string? email = _dataProtector.Protect(invite.InviteeEmail!);
                    string? company = _dataProtector.Protect(companyId.ToString());

                    string? callbackUrl = Url.Action("ProcessInvite", "Invites", new { token, email, company }, protocol: Request.Scheme);

                    string? body = $@"{invite.Message} <br />
                        Please join my Compnay. <br />
                        Click the following link to join our team. <br />
                        <a href=""{callbackUrl}"">COLLABERATE</a>";

                    string? destination = invite.InviteeEmail;

                    Company? btCompany = await _companyService.GetCompanyInfoAsync(companyId);

                    string? subject = $@"StatTrak: {btCompany.Name} Invite";

                    await _emailService.SendEmailAsync(destination!, subject, body);

                    // Save invite in the DB
                    invite.CompanyToken = guid;
                    invite.CompanyId = companyId;
                    invite.InviteDate = DataUtility.GetPostGresDate(DateTime.Now);
                    invite.InvitorId = _userManager.GetUserId(User);
                    invite.IsValid = true;

                    await _inviteService.AddNewInviteAsync(invite);

                    return RedirectToAction("AdmintoIndex", "Home");

                    // TODO: SWAL for "invite sent"

                }
                catch (Exception)
                {

                    throw;
                }


            }

            ViewData["ProjectId"] = new SelectList(await _projectService.GetProjectsAsync(companyId), "Id", "Description");
            return View(invite);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ProcessInvite(string? token, string? email, string? company)
        {
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(company))
            {
                return NotFound();
            }

            Guid companyToken = Guid.Parse(_dataProtector.Unprotect(token));

            string? invtieeEmail = _dataProtector.Unprotect(email);

            int? companyId = int.Parse(_dataProtector.Unprotect(company));

            try
            {
                Invite? invite = await _inviteService.GetInviteAsync(companyToken, invtieeEmail, companyId);

                if (invite != null)
                {
                    return View(invite);
                }

                return NotFound();
            }
            catch (Exception)
            {

                throw;
            }
        }

    }
}
