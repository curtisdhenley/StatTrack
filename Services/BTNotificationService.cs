﻿using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using StatTracker.Data;
using StatTracker.Models;
using StatTracker.Models.Enums;
using StatTracker.Services.Interfaces;

namespace StatTracker.Services
{
    public class BTNotificationService : IBTNotificationService
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailSender _emailService;
        private readonly IBTRolesService _rolesService;

        public BTNotificationService(ApplicationDbContext context, IEmailSender emailService, IBTRolesService rolesService)
        {
            _context = context;
            _emailService = emailService;
            _rolesService = rolesService;
        }

        public async Task AddNotificationAsync(Notification? notification)
        {

            try
            {
                if (notification != null)
                {
                    await _context.AddAsync(notification);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task AdminNotificationAsync(Notification? notification, int? companyId)
        {
            try
            {
                if (notification != null)
                {
                    IEnumerable<string> adminIds = (await _rolesService.GetUsersInRoleAsync(nameof(BTRoles.Admin), companyId))!.Select(u => u!.Id);

                    foreach (string adminId in adminIds)
                    {
                        notification.Id = 0;
                        notification.RecipientId = adminId;

                        await _context.AddAsync(notification);
                    }

                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<List<Notification>> GetNotificationByUserIdAsync(string? userId)
        {
            try
            {
                List<Notification> notifications = new();

                if (!string.IsNullOrEmpty(userId))
                {

                    notifications = await _context.Notifications
                                                  .Where(n => n.RecipientId == userId || n.SenderId == userId)
                                                  .Include(n => n.Recipient)
                                                  .Include(n => n.Sender)
                                                  .ToListAsync();

                }

                return notifications;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<bool> SendAdminEmailNotificationAsync(Notification? notification, string? emailSubject, int? companyId)
        {
            try
            {
                if (notification != null)
                {
                    IEnumerable<string> adminEmails = (await _rolesService.GetUsersInRoleAsync(nameof(BTRoles.Admin), companyId))!.Select(u => u!.Email)!;

                    foreach (string adminEmail in adminEmails)
                    {
                        await _emailService.SendEmailAsync(adminEmail, emailSubject!, notification.Message!);
                    }

                    return true;
                }

                return false;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<bool> SendEmailNotificationAsync(Notification? notification, string? emailSubject)
        {
            try
            {
                if (notification != null)
                {
                    BTUser? btUser = await _context.Users
                                                   .FirstOrDefaultAsync(u => u.Id == notification.RecipientId);

                    string? userEmail = btUser?.Email;

                    if (userEmail != null)
                    {
                        await _emailService.SendEmailAsync(userEmail, emailSubject!, notification.Message!);
                        return true;
                    }
                }

                return false;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
