using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using StatTracker.Data;
using StatTracker.Extensions;
using StatTracker.Models;
using StatTracker.Models.ViewModels;
using StatTracker.Services;
using StatTracker.Services.Interfaces;
using System.Diagnostics;

namespace StatTracker.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<BTUser> _userManager;
        private readonly IBTFileService _btFileService;
        private readonly IBTProjectService _projectService;
        private readonly IBTRolesService _rolesService;
        private readonly IBTCompanyService _companyService;
        private readonly IBTTicketService _ticketService;
        private readonly IEmailSender _emailSender;

        public HomeController(ILogger<HomeController> logger, IBTCompanyService companyService, IBTRolesService rolesService, IBTProjectService projectService, IBTFileService btFileService, UserManager<BTUser> userManager, IBTTicketService ticketService, IEmailSender emailSender)
        {
            _logger = logger;
            _companyService = companyService;
            _rolesService = rolesService;
            _projectService = projectService;
            _btFileService = btFileService;
            _userManager = userManager;
            _ticketService = ticketService;
            _emailSender = emailSender;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ContactMe(EmailData emailData)
        {
            if (ModelState.IsValid)
            {
                string? swalMessage = string.Empty;

                try
                {
                    emailData.EmailSubject = ($"{emailData.EmailSenderName} Sent You A Message From StatTrack");

                    emailData.EmailBody = ($"""<strong>{emailData.EmailSenderName}</strong> sent a message:<br><br>{emailData.EmailBody}<br><br><strong>Their email is:<a href="mailto:{emailData.EmailSenderAddress}">{emailData.EmailSenderAddress}</a></strong>""");

                    await _emailSender.SendEmailAsync("henleydcurtis@gmail.com", emailData.EmailSubject, emailData.EmailBody!);

                    swalMessage = "Sucess! Your email has been sent.";

                    return RedirectToAction(nameof(LandingPage), new { swalMessage });
                }
                catch (Exception)
                {
                    swalMessage = "Error! Your Email Failed to Send.";

                    return RedirectToAction(nameof(LandingPage), new { swalMessage });

                    throw;
                }
            }

            return View(emailData);
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult AdmintoIndex()
        {
            return View();
        }

		public async Task<IActionResult> Dashboard()
		{
            // Get companyId
            int companyId = User.Identity!.GetCompanyId();

            // Create an instance of a user
            BTUser? user = await _userManager.GetUserAsync(User);

            Company? company = await _companyService.GetCompanyInfoAsync(companyId);

            List<Project>? projects = (await _projectService.GetProjectsAsync(companyId)).ToList();

            List<Ticket>? tickets = (await _ticketService.GetTicketsAsync(companyId)).ToList();

            List<BTUser>? members = (await _companyService.GetMembersAsync(companyId)).ToList();

            DashboardViewModel viewModel = new()
            {
                Company = company,
                Projects = projects,
                Tickets = tickets,
                Members = members
            };

            return View(viewModel);
		}

        [AllowAnonymous]
        public IActionResult LandingPage(string? swalMessage = null)
        {
            ViewData["SwalMessage"] = swalMessage;
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}