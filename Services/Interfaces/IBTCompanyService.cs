﻿using StatTracker.Models;

namespace StatTracker.Services.Interfaces
{
    public interface IBTCompanyService
    {
        public Task<Company> GetCompanyInfoAsync(int? companyId);

        public Task<List<BTUser>> GetMembersAsync(int? companyId);

        public Task<IEnumerable<Company>> GetCompaniesAsync();

        public Task<Company> GetCompanyByIdAsync(int companyId);
    }
}
