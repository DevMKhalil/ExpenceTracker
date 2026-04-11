using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ExpenceTracker.Modules.Expenses.Application.DTOs;
using ExpenceTracker.Modules.Expenses.Domain;
using ExpenceTracker.Modules.Badges.Application.Queries;

namespace ExpenceTracker.Modules.Expenses.Application.Queries
{
    public record GetExpenseByIdQuery(Guid Id) : IRequest<ExpenseDto?>;

    public class GetExpenseByIdQueryHandler : IRequestHandler<GetExpenseByIdQuery, ExpenseDto?>
    {
        private readonly IExpenseRepository _repository;
        private readonly IMediator _mediator;

        public GetExpenseByIdQueryHandler(IExpenseRepository repository, IMediator mediator)
        {
            _repository = repository;
            _mediator = mediator;
        }

        public async Task<ExpenseDto?> Handle(GetExpenseByIdQuery request, CancellationToken cancellationToken)
        {
            var e = await _repository.GetByIdAsync(request.Id);
            if (e == null)
            {
                return null;
            }

            var expenseBadges = new System.Collections.Generic.List<ExpenseDto.BadgeInfo>();
            foreach (var badgeId in e.BadgeIds)
            {
                var badge = await _mediator.Send(new GetBadgeByIdQuery(badgeId));
                if (badge != null)
                {
                    expenseBadges.Add(new ExpenseDto.BadgeInfo(badge.Id, badge.Name, badge.Color, badge.IsDeleted));
                }
            }

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
        }
    }
}
