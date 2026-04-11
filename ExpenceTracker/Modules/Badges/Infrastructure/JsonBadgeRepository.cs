using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using ExpenceTracker.Shared.Infrastructure;
using ExpenceTracker.Modules.Badges.Domain;

namespace ExpenceTracker.Modules.Badges.Infrastructure
{
    public class JsonBadgeRepository : JsonFileRepository<Badge>, IBadgeRepository
    {
        public JsonBadgeRepository(IConfiguration configuration) 
            : base(Path.Combine(configuration["Persistence:DataDirectory"] ?? "data", "badges.json"))
        {
        }

        public async Task<List<Badge>> GetAllActiveAsync()
        {
            var all = await ReadAllAsync();
            return all.Where(b => !b.IsDeleted).ToList();
        }

        public async Task<List<Badge>> GetAllIncludingDeletedAsync()
        {
            return await ReadAllAsync();
        }

        public async Task<Badge?> GetByIdAsync(Guid id)
        {
            var all = await ReadAllAsync();
            return all.FirstOrDefault(b => b.Id == id);
        }

        public async Task AddAsync(Badge badge)
        {
            var all = await ReadAllAsync();
            all.Add(badge);
            await WriteAllAsync(all);
        }

        public async Task UpdateAsync(Badge badge)
        {
            var all = await ReadAllAsync();
            var index = all.FindIndex(b => b.Id == badge.Id);
            if (index >= 0)
            {
                badge.UpdatedAt = DateTimeOffset.UtcNow;
                all[index] = badge;
                await WriteAllAsync(all);
            }
        }

        public async Task<bool> ExistsByNameAsync(string name, Guid? excludeId = null)
        {
            var all = await ReadAllAsync();
            return all.Any(b => !b.IsDeleted && b.Name.Equals(name, StringComparison.OrdinalIgnoreCase) && (!excludeId.HasValue || b.Id != excludeId.Value));
        }
    }
}
