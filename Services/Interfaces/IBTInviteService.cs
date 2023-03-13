using StatTracker.Models;

namespace StatTracker.Services.Interfaces
{
    public interface IBTInviteService
    {
        public Task<bool> AcceptInviteAsync(Guid? token, string? userId);

        public Task AddNewInviteAsync(Invite? invite);

        public Task<bool> AnyInviteAsync(Guid? token, string? email, int? copmanyId);

        public Task<Invite?> GetInviteAsync(int? inviteId, int? companyId);

        public Task<Invite?> GetInviteAsync(Guid? token, string? email, int? companyId);

        public Task<bool> ValidateInviteCodeAsync(Guid? toekn);
    }
}
