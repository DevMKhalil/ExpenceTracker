using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ExpenceTracker.Modules.Expenses.Domain;

namespace ExpenceTracker.Modules.Expenses.Application.Queries
{
    public record GetDailySummaryQuery(DateTimeOffset Date) : IRequest<decimal>;

    public class GetDailySummaryQueryHandler : IRequestHandler<GetDailySummaryQuery, decimal>
    {
        private readonly IExpenseRepository _repository;

        public GetDailySummaryQueryHandler(IExpenseRepository repository)
        {
            _repository = repository;
        }

        public async Task<decimal> Handle(GetDailySummaryQuery request, CancellationToken cancellationToken)
        {
            var expenses = await _repository.GetAllAsync();
            return expenses
                .Where(e => e.Date.Date == request.Date.Date && !e.IsPending)
                .Sum(e => e.Amount);
        }
    }
}
