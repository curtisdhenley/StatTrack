using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StatTracker.Data;
using StatTracker.Extensions;
using StatTracker.Models;
using StatTracker.Models.ViewModels;
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

        public HomeController(ILogger<HomeController> logger, IBTCompanyService companyService, IBTRolesService rolesService, IBTProjectService projectService, IBTFileService btFileService, UserManager<BTUser> userManager, IBTTicketService ticketService)
        {
            _logger = logger;
            _companyService = companyService;
            _rolesService = rolesService;
            _projectService = projectService;
            _btFileService = btFileService;
            _userManager = userManager;
            _ticketService = ticketService;
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
        public IActionResult LandingPage()
        {
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