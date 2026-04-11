using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ExpenceTracker.Modules.Badges.Domain
{
    public interface IBadgeRepository
    {
        Task<List<Badge>> GetAllActiveAsync();
        Task<List<Badge>> GetAllIncludingDeletedAsync();
        Task<Badge?> GetByIdAsync(Guid id);
        Task AddAsync(Badge badge);
        Task UpdateAsync(Badge badge);
        Task<bool> ExistsByNameAsync(string name, Guid? excludeId = null);
    }
}
