using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ExpenceTracker.Modules.Expenses.Domain;

namespace ExpenceTracker.Modules.Expenses.Application.Commands
{
    public record CreateExpenseCommand(
        string Name, 
        decimal Amount, 
        DateTimeOffset Date, 
        ImportanceLevel Importance, 
        string? Notes, 
        bool IsPending, 
        List<Guid> BadgeIds) : IRequest<Guid>;

    public class CreateExpenseCommandHandler : IRequestHandler<CreateExpenseCommand, Guid>
    {
        private readonly IExpenseRepository _repository;

        public CreateExpenseCommandHandler(IExpenseRepository repository)
        {
            _repository = repository;
        }

        public async Task<Guid> Handle(CreateExpenseCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                throw new ArgumentException("Name cannot be empty", nameof(request.Name));
            if (request.Amount <= 0 || request.Amount > 99999999.99m)
                throw new ArgumentException("Amount must be between 0.01 and 99999999.99", nameof(request.Amount));

            var expense = new Expense
            {
                Name = request.Name.Trim(),
                Amount = request.Amount,
                Date = request.Date == default ? DateTimeOffset.Now : request.Date,
                Importance = request.Importance,
                Notes = request.Notes?.Trim(),
                IsPending = request.IsPending,
                BadgeIds = request.BadgeIds ?? new List<Guid>()
            };

            await _repository.AddAsync(expense);
            return expense.Id;
        }
    }
}
