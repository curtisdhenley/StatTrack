using Microsoft.EntityFrameworkCore;
using StatTracker.Data;
using StatTracker.Models;
using StatTracker.Services.Interfaces;

namespace StatTracker.Services
{
    public class ProjectService : IProjectService
    {
        private readonly ApplicationDbContext _context;

        public ProjectService(ApplicationDbContext context)
        {
            _context = context;
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

        public async Task<Project> GetProjectAsync(int projectId)
        {
            try
            {
                Project? project = await _context.Projects
                                                 .Include(p => p.Company)
                                                 .Include(p => p.ProjectPriority)
                                                 .FirstOrDefaultAsync(m => m.Id == projectId);


                return project!;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public Task<IEnumerable<Project>> GetProjectsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Project>> GetProjectsByCompanyAsync(int companyId)
        {
            throw new NotImplementedException();
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
    }
}
