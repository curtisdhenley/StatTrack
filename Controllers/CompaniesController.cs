using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StatTracker.Data;
using StatTracker.Extensions;
using StatTracker.Models;
using StatTracker.Models.Enums;
using StatTracker.Models.ViewModels;
using StatTracker.Services;
using StatTracker.Services.Interfaces;

namespace StatTracker.Controllers
{
    [Authorize]
    public class CompaniesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IBTFileService _btFileService;
        private readonly IBTProjectService _projectService;
        private readonly IBTRolesService _rolesService;
        private readonly IBTCompanyService _companyService;
        private readonly UserManager<BTUser> _userManager;

        public CompaniesController(ApplicationDbContext context, IBTFileService btFileService, IBTProjectService projectService, IBTRolesService rolesService, IBTCompanyService companyService, UserManager<BTUser> userManager)
        {
            _context = context;
            _btFileService = btFileService;
            _projectService = projectService;
            _rolesService = rolesService;
            _companyService = companyService;
            _userManager = userManager;
        }

        // GET: Companies
        public async Task<IActionResult> Index()
        {
            int companyId = User.Identity!.GetCompanyId();

            Company? company = await _companyService.GetCompanyInfoAsync(companyId);

            return View(company);
                          
        }

        // GET: Companies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Companies == null)
            {
                return NotFound();
            }

            var company = await _context.Companies
                .FirstOrDefaultAsync(m => m.Id == id);
            if (company == null)
            {
                return NotFound();
            }

            return View(company);
        }

        // GET: Companies/AddTicketComment
        public IActionResult Create()
        {
            return View();
        }

        // POST: Companies/AddTicketComment
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,ImageFileData,ImageFileType")] Company company)
        {
            if (ModelState.IsValid)
            {
                _context.Add(company);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(company);
        }

        // GET: Companies/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Companies == null)
            {
                return NotFound();
            }

            var company = await _context.Companies.FindAsync(id);
            if (company == null)
            {
                return NotFound();
            }
            return View(company);
        }

        // POST: Companies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,ImageFileData,ImageFileType")] Company company)
        {
            if (id != company.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(company);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CompanyExists(company.Id))
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
            return View(company);
        }

        // GET: Companies/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Companies == null)
            {
                return NotFound();
            }

            var company = await _context.Companies
                .FirstOrDefaultAsync(m => m.Id == id);
            if (company == null)
            {
                return NotFound();
            }

            return View(company);
        }

        // POST: Companies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Companies == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Company'  is null.");
            }
            var company = await _context.Companies.FindAsync(id);
            if (company != null)
            {
                _context.Companies.Remove(company);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> ManageUserRoles()
        {
            List<ManageUserRolesViewModel> model = new();

            // Get companyId
            int companyId = User.Identity!.GetCompanyId();

            Company? company = await _companyService.GetCompanyByIdAsync(companyId);

            IEnumerable<BTUser> companyUsers = await _context.Users.Where(u => u.CompanyId == companyId).ToListAsync();

            List<IdentityRole> roles = await _rolesService.GetRolesAsync();

            foreach (BTUser user in companyUsers)
            {
                ManageUserRolesViewModel rolesViewModel = new()
                {
                    BTUser = user,
                    Roles = new MultiSelectList(roles, "Name", "Name", await _rolesService.GetUserRolesAsync(user))
                };

                model.Add(rolesViewModel);
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageUserRoles(ManageUserRolesViewModel viewModel)
        {
            // Get companyId
            int companyId = User.Identity!.GetCompanyId();

            BTUser? btUser = await _userManager.FindByIdAsync(viewModel.BTUser!.Id);

            if (btUser == null)
            {
                return NotFound();
            }

            IEnumerable<string> roles = await _rolesService.GetUserRolesAsync(btUser);

            IEnumerable<string> selectedRoles = viewModel.SelectedRoles;

            await _rolesService.RemoveUserFromRolesAsync(btUser, selectedRoles);

            foreach (string role in selectedRoles)
            {
                await _rolesService.AddUserToRoleAsync(btUser, role);
            }

            return RedirectToAction(nameof(Index));
        }


        private bool CompanyExists(int id)
        {
          return (_context.Companies?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
