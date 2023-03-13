using StatTracker.Models;

namespace StatTracker.Services.Interfaces
{
    public interface IBTNotificationService
    {
        public Task AddNotificationAsync(Notification? notification);

        public Task AdminNotificationAsync(Notification? notification, int? companyId);

        public Task<List<Notification>> GetNotificationByUserIdAsync(string? userId);

        public Task<bool> SendAdminEmailNotificationAsync(Notification? notification, string? emailSubject, int? companyId);

        public Task<bool> SendEmailNotificationAsync(Notification? notification, string? emailSubject);
    }
}
