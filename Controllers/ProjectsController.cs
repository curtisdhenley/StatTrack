using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using StatTracker.Data;
using StatTracker.Extensions;
using StatTracker.Models;
using StatTracker.Services.Interfaces;
using StatTracker.Models.ViewModels;
using StatTracker.Models.Enums;
using StatTracker.Services;
using X.PagedList;

namespace StatTracker.Controllers
{
    [Authorize]
    public class ProjectsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<BTUser> _userManager;
        private readonly IBTFileService _btFileService;
        private readonly IBTProjectService _projectService;
        private readonly IBTRolesService _rolesService;
        private readonly IBTCompanyService _companyService;

        public ProjectsController(ApplicationDbContext context, UserManager<BTUser> userManager, IBTFileService btFileService, IBTProjectService projectService, IBTRolesService rolesService, IBTCompanyService companyService)
        {
            _context = context;
            _userManager = userManager;
            _btFileService = btFileService;
            _projectService = projectService;
            _rolesService = rolesService;
            _companyService = companyService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AssignPM(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Get companyId
            int companyId = User.Identity!.GetCompanyId();

            IEnumerable<BTUser?> projectManagers = await _rolesService.GetUsersInRoleAsync(nameof(BTRoles.ProjectManager), companyId);
            BTUser? currentPM = await _projectService.GetProjectManagerAsync(id);
            AssignPMViewModel viewModel = new()
            {
                Project = await _projectService.GetProjectByIdAsync(id, companyId),
                PMList = new SelectList(projectManagers, "Id", "FullName", currentPM?.Id),
                PMId = currentPM?.Id,
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AssignPM(AssignPMViewModel viewModel)
        {
            if (!string.IsNullOrEmpty(viewModel.PMId))
            {
                await _projectService.AddProjectManagerAsync(viewModel.PMId!, viewModel.Project!.Id);
                return RedirectToAction(nameof(Details), new { id = viewModel.Project?.Id });
            }

            ModelState.AddModelError("PMId", "No Project Manager chosen. Please select a PM.");

            // Get companyId
            int companyId = User.Identity!.GetCompanyId();

            IEnumerable<BTUser?> projectManagers = await _rolesService.GetUsersInRoleAsync(nameof(BTRoles.ProjectManager), companyId);
            BTUser? currentPM = await _projectService.GetProjectManagerAsync(viewModel.Project?.Id);
            viewModel.Project = await _projectService.GetProjectByIdAsync(viewModel.Project?.Id, companyId);
            viewModel.PMList = new SelectList(projectManagers, "Id", "FullName", currentPM?.Id);
            viewModel.PMId = currentPM?.Id;

            return View(viewModel);

        }

        [HttpGet]
        [Authorize(Roles = "Admin, ProjectManager")]
        public async Task<IActionResult> AssignProjectMembers(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Get companyId
            int companyId = User.Identity!.GetCompanyId();

            Project? project = await _projectService.GetProjectByIdAsync(id, companyId);

            List<BTUser?> submitters = await _rolesService.GetUsersInRoleAsync(nameof(BTRoles.Submitter), companyId);
            List<BTUser?> developers = await _rolesService.GetUsersInRoleAsync(nameof(BTRoles.Developer), companyId);

            List<BTUser?> userList = submitters.Concat(developers).ToList();

            List<string> currentMembers = project.Members.Select(m => m.Id).ToList();

            ProjectMembersViewModel viewModel = new()
            {
                Project = project,
                UserList = new MultiSelectList(userList, "Id", "FullName", currentMembers)
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, ProjectManager")]
        public async Task<IActionResult> AssignProjectMembers(ProjectMembersViewModel viewModel)
        {
            // Get companyId
            int companyId = User.Identity!.GetCompanyId();

            if (viewModel.SelectedMembers != null)
            {
                // Remove current members
                await _projectService.RemoveMembersFromProjectAsync(viewModel.Project!.Id, companyId);

                // Add newly selected members
                await _projectService.AddMembersToProjectAsync(viewModel.SelectedMembers, viewModel.Project!.Id, companyId);
                return RedirectToAction(nameof(Details), new { id = viewModel.Project?.Id });
            }

            ModelState.AddModelError("SelectedMembers", "No Users chosen. Please select Users.");

            // Reset the form
            viewModel.Project = await _projectService.GetProjectByIdAsync(viewModel.Project!.Id, companyId);

            List<BTUser?> submitters = await _rolesService.GetUsersInRoleAsync(nameof(BTRoles.Submitter), companyId);
            List<BTUser?> developers = await _rolesService.GetUsersInRoleAsync(nameof(BTRoles.Developer), companyId);

            List<BTUser?> userList = submitters.Concat(developers).ToList();

            List<string> currentMembers = viewModel.Project.Members.Select(m => m.Id).ToList();

            viewModel.UserList = new MultiSelectList(userList, "id", "FullName", currentMembers);

            return View(viewModel);
        }

        // GET: Projects
        public async Task<IActionResult> Index(int? pageNum)
        {
            int companyId = User.Identity!.GetCompanyId();
            int pageSize = 9;
            int page = pageNum ?? 1;

            IPagedList<Project> projects = (await _projectService.GetProjectsAsync(companyId)).ToPagedList(page, pageSize);

            return View(projects);
        }

        // GET: Projects/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            int companyId = User.Identity!.GetCompanyId();

            Project project = await _projectService.GetProjectAsync(id.Value, companyId);


            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        // GET: Projects/AddTicketComment
        public async Task<IActionResult> Create()
        {
            ViewData["CompanyId"] = new SelectList(await _companyService.GetCompaniesAsync(), "Id", "Name");
            ViewData["ProjectPriorityId"] = new SelectList(await _projectService.GetProjectPrioritiesAsync(), "Id", "Id");

            Project project = new Project();
            project.StartDate = DataUtility.GetPostGresDate(DateTime.UtcNow);
            project.EndDate = DataUtility.GetPostGresDate(DateTime.UtcNow);
            return View(project);
        }

        // POST: Projects/AddTicketComment
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CompanyId,Name,Description,Created,StartDate,EndDate,ProjectPriorityId,ImageFormFile,Archived")] Project project)
        {
            //ModelState.Remove("CompanyId");

            if (ModelState.IsValid)
            {
                // display information based on company
                //project.CompanyId = _userManager.GetUserId(User);

                project.Created = DataUtility.GetPostGresDate(DateTime.Now);
                project.StartDate = DataUtility.GetPostGresDate(project.StartDate);
                project.EndDate = DataUtility.GetPostGresDate(project.EndDate);

                // image service
                if (project.ImageFormFile != null)
                {
                    project.ImageFileData = await _btFileService.ConvertFileToByteArrayAsync(project.ImageFormFile);
                    project.ImageFileType = project.ImageFormFile.ContentType;
                }

                await _projectService.AddProjectAsync(project);

                return RedirectToAction(nameof(Index));
            }
            ViewData["CompanyId"] = new SelectList(await _companyService.GetCompaniesAsync(), "Id", "Name", project.CompanyId);
            ViewData["ProjectPriorityId"] = new SelectList(await _projectService.GetProjectPrioritiesAsync(), "Id", "Name", project.ProjectPriorityId);
            return View(project);
        }

        // GET: Projects/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Projects == null)
            {
                return NotFound();
            }

            var project = await _context.Projects.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }
            ViewData["CompanyId"] = new SelectList(await _companyService.GetCompaniesAsync(), "Id", "Name", project.CompanyId);
            ViewData["ProjectPriorityId"] = new SelectList(await _projectService.GetProjectPrioritiesAsync(), "Id", "Name", project.ProjectPriorityId);
            return View(project);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CompanyId,Name,Description,Created,StartDate,EndDate,ProjectPriorityId,ImageFormFile,Archived")] Project project)
        {
            if (id != project.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    project.Created = DataUtility.GetPostGresDate(project.Created);
                    project.StartDate = DataUtility.GetPostGresDate(project.StartDate);
                    project.EndDate = DataUtility.GetPostGresDate(project.EndDate);

                    // image service
                    if (project.ImageFormFile != null)
                    {
                        project.ImageFileData = await _btFileService.ConvertFileToByteArrayAsync(project.ImageFormFile);
                        project.ImageFileType = project.ImageFormFile.ContentType;
                    }

                    await _projectService.UpdateProjectAsync(project);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProjectExists(project.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "Name", project.CompanyId);
            ViewData["ProjectPriorityId"] = new SelectList(_context.ProjectPriorities, "Id", "Name", project.ProjectPriorityId);
            return View(project);
        }

        // GET: Projects/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Projects == null)
            {
                return NotFound();
            }

            int companyId = User.Identity!.GetCompanyId();

            Project project = await _projectService.GetProjectAsync(id.Value, companyId);

            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            int companyId = User.Identity!.GetCompanyId();

            if (_projectService.GetProjectAsync(id, companyId) == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Projects'  is null.");
            }

            Project? project = await _projectService.GetProjectAsync(id, companyId);

            await _projectService.ArchiveProjectAsync(project);

            return RedirectToAction(nameof(Index));
        }

        private bool ProjectExists(int id)
        {
            return (_context.Projects?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
