using System;
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
    public record GetBadgeSummaryQuery() : IRequest<List<BadgeBreakdownItem>>;

    public class GetBadgeSummaryQueryHandler : IRequestHandler<GetBadgeSummaryQuery, List<BadgeBreakdownItem>>
    {
        private readonly IExpenseRepository _repository;
        private readonly IMediator _mediator;

        public GetBadgeSummaryQueryHandler(IExpenseRepository repository, IMediator mediator)
        {
            _repository = repository;
            _mediator = mediator;
        }

        public async Task<List<BadgeBreakdownItem>> Handle(GetBadgeSummaryQuery request, CancellationToken cancellationToken)
        {
            var expenses = await _repository.GetAllAsync();
            var nonPendingExpenses = expenses.Where(e => !e.IsPending).ToList();

            if (!nonPendingExpenses.Any())
            {
                return new List<BadgeBreakdownItem>();
            }

            var allBadges = await _mediator.Send(new GetAllBadgesIncludingDeletedQuery());
            
            var breakdownDict = new Dictionary<Guid, decimal>();

            foreach (var expense in nonPendingExpenses)
            {
                foreach (var badgeId in expense.BadgeIds)
                {
                    if (breakdownDict.ContainsKey(badgeId))
                    {
                        breakdownDict[badgeId] += expense.Amount;
                    }
                    else
                    {
                        breakdownDict[badgeId] = expense.Amount;
                    }
                }
            }

            var result = new List<BadgeBreakdownItem>();
            foreach (var kvp in breakdownDict)
            {
                var badge = allBadges.FirstOrDefault(b => b.Id == kvp.Key);
                if (badge != null)
                {
                    result.Add(new BadgeBreakdownItem(badge.Id, badge.Name, badge.Color, kvp.Value));
                }
            }

            return result.OrderByDescending(b => b.Total).ToList();
        }
    }
}
