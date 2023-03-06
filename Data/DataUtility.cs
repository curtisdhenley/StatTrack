using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Techture.Models;

namespace Techture.Data
{
    public class DataUtility
    {
        private const string _adminRole = "Admin";
        private const string _moderatorRole = "Moderator";
        private const string _adminEmail = "curtishenleydev@gmail.com";
        private const string _moderatorEmail = "techturemoderator@mailinator.com;";

        public static DateTime GetPostGresDate(DateTime dateTime)
        {
            return DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
        }

        public static string GetConnectionString(IConfiguration configuration)
        {
            string? connectionString = configuration.GetConnectionString("DefaultConnection");
            string? databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");

            return string.IsNullOrEmpty(databaseUrl) ? connectionString! : BuildConnectionString(databaseUrl);
        }

        private static string BuildConnectionString(string databaseUrl)
        {
            Uri databaseUri = new(databaseUrl);
            string[] userInfo = databaseUri.UserInfo.Split(':');
            NpgsqlConnectionStringBuilder builder = new()
            {
                Host = databaseUri.Host,
                Port = databaseUri.Port,
                Username = userInfo[0],
                Password = userInfo[1],
                Database = databaseUri.LocalPath.TrimStart('/'),
                SslMode = SslMode.Require,
                TrustServerCertificate = true
            };
            return builder.ToString();
        }

        public static async Task ManageDataAsync(IServiceProvider serviceProvider)
        {
            // Service: An instance of Rolemanger
            var dbContextService = serviceProvider.GetRequiredService<ApplicationDbContext>();

            // Service: An instance of Configuration Service
            var configurationService = serviceProvider.GetRequiredService<IConfiguration>();
            // Service: An instance of UserManager
            var userManagerService = serviceProvider.GetRequiredService<UserManager<BlogUser>>();
            // Service: An instance of Rolemanger
            var roleManagerService = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // Migration: This is the programmatic equivalent to Update Database
            await dbContextService.Database.MigrateAsync();

            // Seed Roles
            await SeedRoleAsync(roleManagerService);

            // Seed Users
            await SeedUsersAsync(dbContextService, configurationService, userManagerService);
        }

        private static async Task SeedRoleAsync(RoleManager<IdentityRole> roleManager)
        {
            if (!await roleManager.RoleExistsAsync(_adminRole))
            {
                await roleManager.CreateAsync(new IdentityRole(_adminRole));
            }

            if (!await roleManager.RoleExistsAsync(_moderatorRole))
            {
                await roleManager.CreateAsync(new IdentityRole(_moderatorRole));
            }
        }

        private static async Task SeedUsersAsync(ApplicationDbContext context, IConfiguration configuration, UserManager<BlogUser> userManager)
        {
            // Seed Admin User
            if (!context.Users.Any(u => u.Email == _adminEmail))
            {
                BlogUser adminUser = new()
                {
                    Email = _adminEmail,
                    UserName = _adminEmail,
                    FirstName = "Curtis",
                    LastName = "Henley",
                    EmailConfirmed = true
                };

                await userManager.CreateAsync(adminUser, configuration["AdminPwd"] ?? Environment.GetEnvironmentVariable("AdminPwd")!);
                await userManager.AddToRoleAsync(adminUser, _adminRole);
            }

            // Seed Moderator User
            if (!context.Users.Any(u => u.Email == _moderatorEmail))
            {
                BlogUser moderatorUser = new()
                {
                    Email = _moderatorEmail,
                    UserName = _moderatorEmail,
                    FirstName = "Blog",
                    LastName = "Moderator",
                    EmailConfirmed = true
                };

                await userManager.CreateAsync(moderatorUser, configuration["ModeratorPwd"] ?? Environment.GetEnvironmentVariable("ModeratorPwd")!);
                await userManager.AddToRoleAsync(moderatorUser, _moderatorRole);
            }
        }
    }
}