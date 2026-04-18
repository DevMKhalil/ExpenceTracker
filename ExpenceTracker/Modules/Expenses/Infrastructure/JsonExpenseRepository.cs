using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using ExpenceTracker.Shared.Infrastructure;
using ExpenceTracker.Modules.Expenses.Domain;

namespace ExpenceTracker.Modules.Expenses.Infrastructure
{
    public class JsonExpenseRepository : JsonFileRepository<Expense>, IExpenseRepository
    {
        public JsonExpenseRepository(IConfiguration config) 
            : base(Path.Combine(config["Persistence:DataDirectory"] ?? "data", "expenses.json"))
        {
        }

        public async Task<List<Expense>> GetAllAsync()
        {
            var all = await ReadAllAsync();
            return all.OrderByDescending(e => e.Date).ToList();
        }

        public async Task<Expense?> GetByIdAsync(Guid id)
        {
            var all = await ReadAllAsync();
            return all.FirstOrDefault(e => e.Id == id);
        }

        public async Task AddAsync(Expense expense)
        {
            await ReadModifyWriteAsync(all =>
            {
                all.Add(expense);
                return all;
            });
        }

        public async Task UpdateAsync(Expense expense)
        {
            await ReadModifyWriteAsync(all =>
            {
                var index = all.FindIndex(e => e.Id == expense.Id);
                if (index >= 0)
                {
                    expense.UpdatedAt = DateTimeOffset.UtcNow;
                    all[index] = expense;
                }
                return all;
            });
        }

        public async Task DeleteAsync(Guid id)
        {
            await ReadModifyWriteAsync(all =>
            {
                var index = all.FindIndex(e => e.Id == id);
                if (index >= 0)
                {
                    all.RemoveAt(index);
                }
                return all;
            });
        }

        public async Task<List<Expense>> GetByDateRangeAsync(DateTimeOffset from, DateTimeOffset to)
        {
            var all = await ReadAllAsync();
            return all.Where(e => e.Date >= from && e.Date <= to)
                      .OrderByDescending(e => e.Date)
                      .ToList();
        }

        public async Task<bool> AnyWithBadgeAsync(Guid badgeId)
        {
            var all = await ReadAllAsync();
            return all.Any(e => e.BadgeIds.Contains(badgeId));
        }
    }
}
