using Microsoft.EntityFrameworkCore;
using StatTracker.Data;
using StatTracker.Models;
using StatTracker.Models.Enums;
using StatTracker.Services.Interfaces;

namespace StatTracker.Services
{
    public class BTProjectService : IBTProjectService
    {
        private readonly ApplicationDbContext _context;
        private readonly IBTRolesService _roleService;

        public BTProjectService(ApplicationDbContext context, IBTRolesService roleService)
        {
            _context = context;
            _roleService = roleService;
        }

        public async Task AddProjectAsync(Project project)
        {
            try
            {
                _context.Add(project);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task ArchiveProjectAsync(Project project)
        {
            if (project != null)
            {
                project.Archived = true;
                _context.Projects.Update(project);
            }

            await _context.SaveChangesAsync();
        }

        public async Task<Project> GetProjectAsync(int projectId, int companyId)
        {
            try
            {
                Project? project = await _context.Projects
                                                 .Where(p => p.CompanyId == companyId)
                                                 .Include(p => p.Company)
                                                 .Include(p => p.Members)
                                                 .Include(p => p.ProjectPriority)
                                                 .Include(p => p.Tickets)
                                                    .ThenInclude(t => t.DeveloperUser)
                                                 .Include(p => p.Tickets)
                                                    .ThenInclude(t => t.SubmitterUser)
                                                 .FirstOrDefaultAsync(m => m.Id == projectId);


                return project!;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<IEnumerable<Project>> GetProjectsAsync(int companyId)
        {
            try
            {
                IEnumerable<Project> projects = await _context.Projects
                                                          .Where(p => p.Archived == false && p.CompanyId == companyId)
                                                          .Include(p => p.Members)
                                                          .Include(p => p.ProjectPriority)
                                                          .Include(p => p.Tickets)
                                                          .ToListAsync();

                return projects!;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<IEnumerable<Company>> GetCompaniesAsync()
        {
            try
            {
                IEnumerable<Company> companies = await _context.Companies.ToListAsync();

                return companies;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public Task<IEnumerable<Project>> GetProjectsByEndDate()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Project>> GetProjectsByEndDate(DateTime endDate)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Project>> GetProjectsByPriorityAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Project>> GetProjectsByPriorityAsync(int projectPriorityId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Project> SearchProjects(string? searchString)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateProjectAsync(Project project)
        {
            _context.Update(project);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<ProjectPriority>> GetProjectPrioritiesAsync()
        {
            try
            {
                IEnumerable<ProjectPriority> projectPriorities = await _context.ProjectPriorities.ToListAsync();

                return projectPriorities;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<Project> GetProjectByIdAsync(int? projectId, int companyId)
        {
            Project? project = await _context.Projects
                                                 .Where(p => p.CompanyId == companyId)
                                                 .Include(p => p.Company)
                                                 .Include(p => p.Members)
                                                 .Include(p => p.ProjectPriority)
                                                 .Include(p => p.Tickets)
                                                    .ThenInclude(t => t.DeveloperUser)
                                                 .Include(p => p.Tickets)
                                                    .ThenInclude(t => t.SubmitterUser)
                                                 .FirstOrDefaultAsync(m => m.Id == projectId);

            return project!;
        }

        public async Task<BTUser> GetProjectManagerAsync(int? projectId)
        {
            try
            {
                Project? project = await _context.Projects.Include(p => p.Members).FirstOrDefaultAsync(p => p.Id == projectId);

                foreach (BTUser member in project!.Members)
                {
                    if (await _roleService.IsUserInRoleAsync(member, nameof(BTRoles.ProjectManager)))
                    {
                        return member;
                    }
                }

                return null!;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<bool> AddProjectManagerAsync(string userId, int projectId)
        {
            try
            {
                BTUser? currentPM = await GetProjectManagerAsync(projectId);
                BTUser? selectedPM = await _context.Users.FindAsync(userId);

                // Remove the current PM
                if (currentPM != null)
                {
                    await RemoveProjectManagerAsync(projectId);
                }

                // Add new/selected PM
                try
                {
                    await AddMemberToProjectAsync(selectedPM!, projectId);
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

        public async Task<bool> AddMemberToProjectAsync(BTUser member, int projectId)
        {
            try
            {
                Project? project = await GetProjectByIdAsync(projectId, member.CompanyId);

                bool isOnProject = project.Members.Any(m => m.Id == member.Id);

                if (!isOnProject)
                {
                    project.Members.Add(member);
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

        public async Task RemoveProjectManagerAsync(int projectId)
        {
            try
            {
                Project? project = await _context.Projects.Include(p => p.Members).FirstOrDefaultAsync(p => p.Id == projectId);

                foreach (BTUser member in project!.Members)
                {
                    if (await _roleService.IsUserInRoleAsync(member, nameof(BTRoles.ProjectManager)))
                    {
                        await RemoveMemberFromProjectAsync(member, projectId);
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<bool> RemoveMemberFromProjectAsync(BTUser member, int projectId)
        {
            try
            {
                Project? project = await GetProjectByIdAsync(projectId, member.CompanyId);

                bool isOnProject = project.Members.Any(m => m.Id == member.Id);

                if (isOnProject)
                {
                    project.Members.Remove(member);
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

        public async Task AddMembersToProjectAsync(IEnumerable<string> userIds, int? projectId, int companyId)
        {
            try
            {
                Project? project = await GetProjectByIdAsync(projectId, companyId);

                foreach (string userId in userIds)
                {
                    BTUser? btUser = await _context.Users.FindAsync(userId);

                    if (project != null && btUser != null)
                    {
                        bool isOnProject = project.Members.Any(m => m.Id == userId);

                        if (!isOnProject)
                        {
                            project.Members.Add(btUser);
                        }
                        else
                        {
                            continue;
                        }
                    }
                }

                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task RemoveMembersFromProjectAsync(int? projectId, int companyId)
        {
            try
            {
                Project? project = await GetProjectByIdAsync(projectId, companyId);

                foreach (BTUser member in project.Members)
                {
                   if (!await _roleService.IsUserInRoleAsync(member, nameof(BTRoles.ProjectManager)))
                    {
                        project.Members.Remove(member);
                    }
                }

                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
