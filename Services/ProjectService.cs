﻿using Microsoft.EntityFrameworkCore;
using StatTracker.Data;
using StatTracker.Models;
using StatTracker.Services.Interfaces;

namespace StatTracker.Services
{
    public class ProjectService : IBTProjectService
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
    }
}
