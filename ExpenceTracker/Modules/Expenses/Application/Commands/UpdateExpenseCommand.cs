using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ExpenceTracker.Modules.Expenses.Domain;

namespace ExpenceTracker.Modules.Expenses.Application.Commands
{
    public record UpdateExpenseCommand(
        Guid Id, 
        string Name, 
        decimal Amount, 
        DateTimeOffset Date, 
        ImportanceLevel Importance, 
        string? Notes, 
        bool IsPending, 
        List<Guid> BadgeIds) : IRequest<bool>;

    public class UpdateExpenseCommandHandler : IRequestHandler<UpdateExpenseCommand, bool>
    {
        private readonly IExpenseRepository _repository;

        public UpdateExpenseCommandHandler(IExpenseRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(UpdateExpenseCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                throw new ArgumentException("Name cannot be empty", nameof(request.Name));
            if (request.Amount <= 0 || request.Amount > 99999999.99m)
                throw new ArgumentException("Amount must be between 0.01 and 99999999.99", nameof(request.Amount));

            var expense = await _repository.GetByIdAsync(request.Id);
            if (expense == null)
            {
                return false;
            }

            expense.Name = request.Name.Trim();
            expense.Amount = request.Amount;
            expense.Date = request.Date == default ? DateTimeOffset.Now : request.Date;
            expense.Importance = request.Importance;
            expense.Notes = request.Notes?.Trim();
            expense.IsPending = request.IsPending;
            expense.BadgeIds = request.BadgeIds ?? new List<Guid>();

            await _repository.UpdateAsync(expense);
            return true;
        }
    }
}
