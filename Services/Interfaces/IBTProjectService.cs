using StatTracker.Models;

namespace StatTracker.Services.Interfaces
{
    public interface IBTProjectService
    {
        #region CRUD Methods
        // CRUD Services

        // AddProjectAsync(Project project)
        public Task AddProjectAsync(Project project);

        // UpdateProjectAsync(Project project)
        public Task UpdateProjectAsync(Project project);

        // GetProjectAsync(int projectId, int companyId)
        /// <summary>
        /// Get a single Project by Id (integer)
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public Task<Project> GetProjectAsync(int projectId, int companyId);

        // ArchiveProjectAsync(Project project)
        public Task ArchiveProjectAsync(Project project);
        #endregion

        #region Get Projects Methods
        // GetProjectsAsync()
        public Task<IEnumerable<Project>> GetProjectsAsync(int? companyId);

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

        public Task<Project> GetProjectByIdAsync(int? projectId, int companyId);


        public Task<BTUser> GetProjectManagerAsync(int? projectId);

        #endregion

        #region Addtional Methods
        public Task<bool> AddProjectManagerAsync(string userId, int projectId);

        public Task<bool> AddMemberToProjectAsync(BTUser member, int projectId);

        public Task AddMembersToProjectAsync(IEnumerable<string> userIds, int? projectId, int companyId);

        public Task RemoveProjectManagerAsync(int projectId);

        public Task<bool> RemoveMemberFromProjectAsync(BTUser member, int projectId);

        public Task RemoveMembersFromProjectAsync(int? projectId, int companyId);

        public IEnumerable<Project> SearchProjects(string? searchString);

        // GetProjectsByPriorityAsync()
        public Task<IEnumerable<ProjectPriority>> GetProjectPrioritiesAsync();
        #endregion
    }
}
