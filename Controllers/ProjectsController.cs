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

namespace StatTracker.Controllers
{
    [Authorize]
    public class ProjectsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<BTUser> _userManager;
        private readonly IBTFileService _btFileService;
        private readonly IProjectService _projectService;

        public ProjectsController(ApplicationDbContext context, UserManager<BTUser> userManager, IBTFileService btFileService, IProjectService projectService)
        {
            _context = context;
            _userManager = userManager;
            _btFileService = btFileService;
            _projectService = projectService;
        }

        // GET: Projects
        public async Task<IActionResult> Index()
        {
            int companyId = User.Identity!.GetCompanyId();

            IEnumerable<Project> projects = await _context.Projects
                                                          .Where(p => p.Archived == false && p.CompanyId == companyId)
                                                          .Include(p => p.Members)
                                                          .Include(p => p.Tickets)
                                                          .ToListAsync();

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

        // GET: Projects/Create
        public IActionResult Create()
        {
            ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "Name");
            ViewData["ProjectPriorityId"] = new SelectList(_context.ProjectPriorities, "Id", "Id");

            Project project = new Project();
            project.StartDate = DataUtility.GetPostGresDate(DateTime.UtcNow);
            project.EndDate = DataUtility.GetPostGresDate(DateTime.UtcNow);
            return View(project);
        }

        // POST: Projects/Create
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

                project.Created = DataUtility.GetPostGresDate(DateTime.UtcNow);
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
            ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "Name", project.CompanyId);
            ViewData["ProjectPriorityId"] = new SelectList(_context.ProjectPriorities, "Id", "Name", project.ProjectPriorityId);
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
            ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "Name", project.CompanyId);
            ViewData["ProjectPriorityId"] = new SelectList(_context.ProjectPriorities, "Id", "Id", project.ProjectPriorityId);
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

            Project project = await _projectService.GetProjectAsync(id.Value);

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
            if (_projectService.GetProjectAsync(id) == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Projects'  is null.");
            }

            Project? project = await _projectService.GetProjectAsync(id);

            await _projectService.ArchiveProjectAsync(project);

            return RedirectToAction(nameof(Index));
        }

        private bool ProjectExists(int id)
        {
          return (_context.Projects?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
