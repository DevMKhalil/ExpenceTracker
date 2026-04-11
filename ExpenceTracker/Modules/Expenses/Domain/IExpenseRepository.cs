using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ExpenceTracker.Modules.Expenses.Domain
{
    public interface IExpenseRepository
    {
        Task<List<Expense>> GetAllAsync();
        Task<Expense?> GetByIdAsync(Guid id);
        Task AddAsync(Expense expense);
        Task UpdateAsync(Expense expense);
        Task DeleteAsync(Guid id);
        Task<List<Expense>> GetByDateRangeAsync(DateTimeOffset from, DateTimeOffset to);
        Task<bool> AnyWithBadgeAsync(Guid badgeId);
    }
}
