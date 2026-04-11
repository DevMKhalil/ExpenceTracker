using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ExpenceTracker.Modules.Expenses.Application.DTOs;
using ExpenceTracker.Modules.Expenses.Domain;
using ExpenceTracker.Modules.Badges.Application.Queries;

namespace ExpenceTracker.Modules.Expenses.Application.Queries
{
    public record GetAllExpensesQuery() : IRequest<List<ExpenseDto>>;

    public class GetAllExpensesQueryHandler : IRequestHandler<GetAllExpensesQuery, List<ExpenseDto>>
    {
        private readonly IExpenseRepository _repository;
        private readonly IMediator _mediator;

        public GetAllExpensesQueryHandler(IExpenseRepository repository, IMediator mediator)
        {
            _repository = repository;
            _mediator = mediator;
        }

        public async Task<List<ExpenseDto>> Handle(GetAllExpensesQuery request, CancellationToken cancellationToken)
        {
            var expenses = await _repository.GetAllAsync();
            var allBadges = await _mediator.Send(new GetAllBadgesIncludingDeletedQuery());
            
            var dtos = expenses.Select(e =>
            {
                var expenseBadges = allBadges
                    .Where(b => e.BadgeIds.Contains(b.Id))
                    .Select(b => new ExpenseDto.BadgeInfo(b.Id, b.Name, b.Color, b.IsDeleted))
                    .ToList();

                return new ExpenseDto(
                    e.Id,
                    e.Name,
                    e.Amount,
                    e.Date,
                    e.Importance,
                    e.Notes,
                    e.IsPending,
                    e.BadgeIds,
                    e.CreatedAt,
                    expenseBadges
                );
            }).ToList();

            return dtos;
        }
    }
}
