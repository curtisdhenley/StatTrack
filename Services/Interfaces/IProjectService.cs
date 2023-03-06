using StatTracker.Models;

namespace StatTracker.Services.Interfaces
{
    public interface IProjectService
    {
        #region CRUD Methods
        // CRUD Services

        // AddProjectAsync(Project project)
        public Task AddProjectAsync(Project project);

        // UpdateProjectAsync(Project project)
        public Task UpdateProjectAsync(Project project);

        // GetProjectAsync(int projectId)
        /// <summary>
        /// Get a single Project by Id (integer)
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public Task<Project> GetProjectAsync(int projectId);

        // ArchiveProjectAsync(Project project)
        public Task ArchiveProjectAsync(Project project);
        #endregion

        #region Get Projects Methods
        // GetProjectsAsync()
        public Task<IEnumerable<Project>> GetProjectsAsync();

        public Task<IEnumerable<Project>> GetProjectsByCompanyAsync(int companyId);

        // GetProjectsByPriorityAsync()
        public Task<IEnumerable<Project>> GetProjectsByPriorityAsync();

        /// <summary>
        /// Gets the specified number of Projects based on priority.
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public Task<IEnumerable<Project>> GetProjectsByPriorityAsync(int projectPriorityId);

        // GetProjectsByEndDate()
        public Task<IEnumerable<Project>> GetProjectsByEndDate();

        /// <summary>
        /// Gets the specified number of Projects based on endDate.
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public Task<IEnumerable<Project>> GetProjectsByEndDate(DateTime endDate);

        #endregion

        #region Addtional Methods
        public IEnumerable<Project> SearchProjects(string? searchString);
        #endregion
    }
}
